using System;
using System.IO;
using System.Xml.Linq;
using UnityEngine;

namespace PapertrailFor7DTD.SDK {
    public class PapertrailSettings : ScriptableObject {
        private const string ROOT_KEY = "settings";
        private const string HOSTNAME_KEY = "hostname";
        private const string PORT_KEY = "port";
        private const string SYSTEM_NAME_KEY = "system-name";
        private const string MINIMUM_LOGGING_LEVEL_KEY = "minimum-logging-level";
        private const string FACILITY_KEY = "facility";
        private const string LOG_STACK_TRACE_KEY = "log-stack-trace";
        private const string LOG_CLIENT_IP_ADDRESS_KEY = "log-client-ip-address";

        // Resources path where the logger settings are stored.
        public static string SettingsPath { get; private set; } = Path.Combine(GameIO.GetSaveGameDir(), "papertrail.xml");

        // Remote server to send the messages to.
        [Header("Remote server IP or hostname to log messages")]
        public string hostname = string.Empty;
        // Remote server port.
        [Header("Remote server port")]
        public int port = -1;
        // Default facility tag to use for logs.
        [Header("System name to appear in the Dashboard")]
        public string systemName = string.Empty;
        // Minimum severity of logs to send to the server.
        [Header("Minimum severity of logs to send to the server")]
        public Severity minimumLoggingLevel = Severity.Debug;
        // Default facility tag to use for logs.
        [Header("Default facility tag to use for logs")]
        public Facility facility = Facility.local7;
        // Minimum severity of logs to send to the server.
        [Header("Append the log stack trace")]
        public bool logStackTrace = true;
        // Minimum severity of logs to send to the server.
        [Header("Append the client's IP address")]
        public bool logClientIPAddress = true;

        /// <summary>
        /// Loads the default settings file
        /// </summary>
        public static PapertrailSettings LoadSettings() {
            var settings = CreateInstance<PapertrailSettings>();
            try {
                var x = XElement.Load(SettingsPath);
                settings.hostname = x.Element(HOSTNAME_KEY).Value;
                if (string.IsNullOrEmpty(settings.hostname)) {
                    Log.Error($"[PAPERTRAIL] Unable to parse required value {HOSTNAME_KEY}");
                }
                if (!int.TryParse(x.Element(PORT_KEY).Value, out settings.port)) {
                    Log.Error($"[PAPERTRAIL] Unable to parse required value {PORT_KEY}");
                }

                settings.systemName = x.Element(SYSTEM_NAME_KEY).Value;
                if (!Enum.TryParse(x.Element(MINIMUM_LOGGING_LEVEL_KEY).Value, out settings.minimumLoggingLevel)) {
                    Log.Warning($"[PAPERTRAIL] {MINIMUM_LOGGING_LEVEL_KEY} missing or cannot be parsed - using default value of {settings.minimumLoggingLevel}");
                }
                if (!Enum.TryParse(x.Element(FACILITY_KEY).Value, out settings.facility)) {
                    Log.Warning($"[PAPERTRAIL] {FACILITY_KEY} missing or cannot be parsed - using default value of {settings.facility}");
                }
                if (!bool.TryParse(x.Element(LOG_STACK_TRACE_KEY).Value, out settings.logStackTrace)) {
                    Log.Warning($"[PAPERTRAIL] {LOG_STACK_TRACE_KEY} missing or cannot be parsed - using default value of {settings.logStackTrace}");
                }
                if (!bool.TryParse(x.Element(LOG_CLIENT_IP_ADDRESS_KEY).Value, out settings.logClientIPAddress)) {
                    Log.Warning($"[PAPERTRAIL] {LOG_CLIENT_IP_ADDRESS_KEY} missing or cannot be parsed - using default value of {settings.logClientIPAddress}");
                }
                return settings;
            } catch (FileNotFoundException) {
                Log.Warning("[PAPERTRAIL] Settings file not present; creating a new one.");
                settings.SaveSettings();
                return settings;
            } catch (Exception e) {
                Log.Error("[PAPERTRAIL] Unexpected error while trying to load settings file.");
                Log.Exception(e);
                throw e;
            }
        }

        public void SaveSettings() {
            try {
                var x = new XElement(ROOT_KEY);
                x.Add(new XElement(HOSTNAME_KEY, hostname));
                x.Add(new XElement(PORT_KEY, port));
                x.Add(new XElement(SYSTEM_NAME_KEY, systemName));
                x.Add(new XElement(MINIMUM_LOGGING_LEVEL_KEY, minimumLoggingLevel));
                x.Add(new XElement(FACILITY_KEY, facility));
                x.Add(new XElement(LOG_STACK_TRACE_KEY, logStackTrace));
                x.Add(new XElement(LOG_CLIENT_IP_ADDRESS_KEY, logClientIPAddress));
                x.Save(SettingsPath);
            } catch (Exception e) {
                Log.Error("[PAPERTRAIL] Unable to save papertrail settings file.");
                Log.Exception(e);
                throw e;
            }
        }

        /// <summary>
        /// Ensures all settings are correct
        /// </summary>
        private void OnValidate() {
            systemName = systemName.Replace('.', '-');
        }

#if UNITY_EDITOR
        /// <summary>
        /// Ensures that a settings file exists in the project.
        /// </summary>
        [UnityEditor.InitializeOnLoadMethod]
        private static void EnsureSettingsExist()
        {
            PapertrailSettings settings = LoadSettings();
            if (settings == null)
            {
                var folders = System.IO.Directory.GetDirectories(Application.dataPath, "Papertrail");
                string createPath = string.Empty;
                if (folders.Length == 0)
                {
                    createPath = "Assets/Resources/" + s_settingsPath + ".asset";
                }
                else
                {
                    createPath = GetRelativeProjectPath(folders[0]);
                    createPath += "Resources/" + s_settingsPath + ".asset";
                }
                string[] split = createPath.Split('/');
                string currentDepth = string.Empty;
                for (int j = 0; j < split.Length - 1; j++)
                {
                    string parentFolder = currentDepth;
                    if (j > 0)
                        currentDepth += '/';
                    currentDepth += split[j];
                    if (!UnityEditor.AssetDatabase.IsValidFolder(currentDepth))
                        UnityEditor.AssetDatabase.CreateFolder(parentFolder, split[j]);
                }
                settings = CreateInstance<PapertrailSettings>();
                UnityEditor.AssetDatabase.CreateAsset(settings, createPath);
            }
            if (string.IsNullOrEmpty(settings.systemName))
            {
                settings.systemName = Application.identifier;
                UnityEditor.EditorUtility.SetDirty(settings);
            }
        }

        /// <summary>
        /// Gets the relative path of an asset to the project Assets folder
        /// </summary>
        private static string GetRelativeProjectPath(string absolutePath)
        {
            string[] split = absolutePath.Replace('\\', '/').Split('/');
            string path = string.Empty;
            for (int i = 0; i < split.Length; i++)
            {
                if (split[i] != "Assets" && path.Length == 0)
                    continue;
                path += split[i] + '/';
            }
            return path;
        }
#endif
    }
}