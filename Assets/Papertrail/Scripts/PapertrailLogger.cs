using System;
using System.Collections;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Papertrail
{
    public class PapertrailLogger : MonoBehaviour
    {
        private static PapertrailLogger m_instance;

        private PapertrailSettings m_settings;
        private UdpClient m_udpClient = null;
        private readonly object m_sendLock = new object();
        private StringBuilder m_stringBuilder = new StringBuilder();
        private string m_processName;
        private int m_processId;
        private string m_localIp;

        [RuntimeInitializeOnLoadMethod]
        private static void Initialize()
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<PapertrailLogger>();
                if (m_instance == null)
                {
                    m_instance = new GameObject("PapertrailLogger").AddComponent<PapertrailLogger>();
                }
                DontDestroyOnLoad(m_instance.gameObject);
            }
        }

        private void Awake()
        {
            m_settings = PapertrailSettings.LoadSettings();
            m_processId = System.Diagnostics.Process.GetCurrentProcess().Id;
            m_processName = Application.identifier.Replace(" ", string.Empty);
            m_localIp = Network.player.ipAddress;
            StartCoroutine(GetExternalIP());

            if (!string.IsNullOrEmpty(m_settings.hostname) && m_settings.port > 0)
            {
                try
                {
                    m_udpClient = new UdpClient(m_settings.hostname, m_settings.port);
                    Application.logMessageReceivedThreaded += Application_LogMessageReceived;
                }
                catch (Exception ex)
                {
                    m_udpClient = null;
                    Debug.LogException(ex);
                }
            }
            else
            {
                m_udpClient = null;
            }
        }
        private void OnDestroy()
        {
            Close();
        }
        private IEnumerator GetExternalIP()
        {
            while (Application.internetReachability == NetworkReachability.NotReachable)
            {
                yield return new WaitForSeconds(1);
            }
            while (true)
            {
                UnityWebRequest webRequest = UnityWebRequest.Get("https://api.ipify.org?format=text");
                yield return webRequest.Send();
                if (!webRequest.isError)
                {
                    m_localIp = webRequest.downloadHandler.text;
                    break;
                }
                yield return new WaitForSeconds(1);
            }
        }
        private void Application_LogMessageReceived(string condition, string stackTrace, LogType type)
        {
            Severity severity = Severity.Debug;
            switch (type)
            {
                case LogType.Assert:
                    severity = Severity.Alert;
                    break;
                case LogType.Error:
                case LogType.Exception:
                    severity = Severity.Error;
                    break;
                case LogType.Log:
                    severity = Severity.Debug;
                    break;
                case LogType.Warning:
                    severity = Severity.Warning;
                    break;
            }
            Log(severity, string.Format("{0} {1}", condition, stackTrace));
        }
        private void BeginSend(string msg)
        {
            if (string.IsNullOrEmpty(msg)) return;
            if (!string.IsNullOrEmpty(m_settings.hostname) && m_settings.port > 0)
            {
                lock (m_sendLock)
                {
                    if (m_udpClient != null)
                    {
                        byte[] data = Encoding.UTF8.GetBytes(msg);
                        try
                        {
                            m_udpClient.BeginSend(data, data.Length, OnEndSend, m_udpClient);
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(e);
                        }
                    }
                }
            }
        }
        private void OnEndSend(IAsyncResult result)
        {
            try
            {
                UdpClient udpClient = (UdpClient)result.AsyncState;
                udpClient.EndSend(result);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public void Close()
        {
            if (m_udpClient != null)
            {
                m_udpClient.Close();
                m_udpClient = null;
            }
        }
        public void Log(string msg)
        {
            Log(m_settings.facility, Severity.Debug, msg);
        }
        public void Log(Severity severity, string msg)
        {
            Log(m_settings.facility, severity, msg);
        }
        public void Log(Facility facility, Severity severity, string msg)
        {
            if (string.IsNullOrEmpty(msg) || severity > m_settings.loggingLevel || m_udpClient == null) return;
            int severityValue = ((int)facility) * 8 + (int)severity;
            string message = string.Empty;
            lock (m_stringBuilder)
            {
                m_stringBuilder.Length = 0;
                m_stringBuilder.Append('<');
                m_stringBuilder.Append(severityValue);
                m_stringBuilder.Append('>');
                m_stringBuilder.Append('1');
                m_stringBuilder.Append(' ');
                m_stringBuilder.Append(Rfc3339DateTime.ToString(DateTime.UtcNow));
                m_stringBuilder.Append(' ');
                m_stringBuilder.Append(m_localIp);
                m_stringBuilder.Append(' ');
                m_stringBuilder.Append(m_processName);
                m_stringBuilder.Append(' ');
                m_stringBuilder.Append(msg);
                message = m_stringBuilder.ToString();
            }
            if (m_udpClient != null)
            {
                BeginSend(message);
            }
        }
    }
}
