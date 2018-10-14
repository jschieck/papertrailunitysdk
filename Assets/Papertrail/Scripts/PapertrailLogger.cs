using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Papertrail
{
    public class PapertrailLogger : MonoBehaviour
    {
        private const string s_taggedLogFormat = "tag=[{0}] message=[{1}] stacktrace=[{2}]";
        private const string s_logFormat = "message=[{0}] stacktrace=[{1}]";
        private const string s_ipPrefixFormat = "ip=[{0}] {1}";
        private static PapertrailLogger Instance
        {
            get
            {
                if (s_instance == null)
                {
                    Initialize();
                }
                return s_instance;
            }
        }
        private static PapertrailLogger s_instance;

        private PapertrailSettings m_settings;
        private UdpClient m_udpClient = null;
        private readonly object m_sendLock = new object();
        private StringBuilder m_stringBuilder = new StringBuilder();
        private string m_processName;
        private string m_platform;
        private int m_processId;
        private string m_localIp;
        private bool m_isReady;
        private string m_tag;
        private Queue<string> m_queuedMessages = new Queue<string>();

        [RuntimeInitializeOnLoadMethod]
        private static void Initialize()
        {
            if (s_instance == null)
            {
                s_instance = FindObjectOfType<PapertrailLogger>();
                if (s_instance == null)
                {
                    s_instance = new GameObject("PapertrailLogger").AddComponent<PapertrailLogger>();
                }
                DontDestroyOnLoad(s_instance.gameObject);
            }
        }

        private void Awake()
        {
            if (s_instance != null && s_instance != this)
            {
                Destroy(this);
                return;
            }
            m_isReady = false;
            m_settings = PapertrailSettings.LoadSettings();
            m_processId = System.Diagnostics.Process.GetCurrentProcess().Id;
            m_processName = Application.identifier.Replace(" ", string.Empty);
            m_platform = Application.platform.ToString().ToLowerInvariant();
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
            Application.logMessageReceivedThreaded -= Application_LogMessageReceived;
            Close();
        }
        private void Close()
        {
            if (m_udpClient != null)
            {
                m_udpClient.Close();
                m_udpClient = null;
            }
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
            m_isReady = true;
            Debug.Log("Papertrail Logger Initialized");
            while (m_queuedMessages.Count > 0)
            {
                BeginSend(m_queuedMessages.Dequeue());
                yield return null;
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
            try
            {
                if (!string.IsNullOrEmpty(m_tag))
                {
                    Log(severity, string.Format(s_taggedLogFormat, m_tag, condition, stackTrace));
                }
                else
                {
                    Log(severity, string.Format(s_logFormat, condition, stackTrace));
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
        private void BeginSend(string msg)
        {
            if (string.IsNullOrEmpty(msg)) return;
            if (!string.IsNullOrEmpty(m_settings.hostname) && m_settings.port > 0)
            {
                if (!m_isReady)
                {
                    m_queuedMessages.Enqueue(msg);
                    return;
                }
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
        private void LogInternal(string msg)
        {
            Log(m_settings.facility, Severity.Debug, msg);
        }
        private void LogInternal(Severity severity, string msg)
        {
            Log(m_settings.facility, severity, msg);
        }
        private void LogInternal(Facility facility, Severity severity, string msg)
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
                m_stringBuilder.Append("unity-client");
                m_stringBuilder.Append(' ');
                m_stringBuilder.Append(m_processName);
                m_stringBuilder.Append(' ');
                m_stringBuilder.Append(m_platform);
                m_stringBuilder.Append(' ');
                m_stringBuilder.Append(string.Format(s_ipPrefixFormat, m_localIp, msg));
                message = m_stringBuilder.ToString();
            }
            if (m_udpClient != null)
            {
                BeginSend(message);
            }
        }

        public static void SetTag(string tag)
        {
            Instance.m_tag = tag;
        }
        public static void Log(string msg)
        {
            Instance.LogInternal(Severity.Debug, msg);
        }
        public static void Log(Severity severity, string msg)
        {
            Instance.LogInternal(severity, msg);
        }
        public static void Log(Facility facility, Severity severity, string msg)
        {
            Instance.LogInternal(facility, severity, msg);
        }
    }
}
