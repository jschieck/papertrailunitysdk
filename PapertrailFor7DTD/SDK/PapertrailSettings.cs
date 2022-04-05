using System;
using System.IO;
using System.Xml.Linq;

namespace PapertrailFor7DTD.SDK {
    public class PapertrailSettings {
        private const string ROOT_KEY = "settings";
        private const string HOSTNAME_KEY = "hostname";
        private const string PORT_KEY = "port";
        private const string SYSTEM_NAME_KEY = "system-name";
        private const string MINIMUM_LOGGING_LEVEL_KEY = "minimum-logging-level";
        private const string FACILITY_KEY = "facility";
        private const string LOG_STACK_TRACE_KEY = "log-stack-trace";
        private const string LOG_CLIENT_IP_ADDRESS_KEY = "log-client-ip-address";

        /**
         * <summary>Resources path where the logger settings are stored.</summary>
         */
        public static string SettingsPath { get; private set; } = Path.Combine(GameIO.GetSaveGameDir(), "papertrail.xml");

        /**
         * <summary>Remote server to send the messages to.</summary>
         */
        public string hostname = string.Empty;
        /**
         * <summary>Remote server port.</summary>
         */
        public int port = -1;
        /**
         * <summary>Default facility tag to use for logs.</summary>
         */
        public string systemName = string.Empty;
        /**
         * <summary>Minimum severity of logs to send to the server.</summary>
         */
        public Severity minimumLoggingLevel = Severity.Debug;
        /**
         * <summary>Default facility tag to use for logs.</summary>
         */
        public Facility facility = Facility.local7;
        /**
         * <summary>Minimum severity of logs to send to the server.</summary>
         */
        public bool logStackTrace = true;
        /**
         * <summary>Minimum severity of logs to send to the server.</summary>
         */
        public bool logClientIPAddress = true;

        /**
         * <summary>Loads the default settings file.</summary>
         */
        public static PapertrailSettings LoadSettings() {
            var settings = new PapertrailSettings();
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

        /**
         * <summary>Save current settings to file.</summary>
         */
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

        /**
         * <summary>Ensures all settings are correct.</summary>
         */
        private void OnValidate() {
            systemName = systemName.Replace('.', '-');
        }
    }
}