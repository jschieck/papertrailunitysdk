using UnityEngine;

namespace Papertrail
{
    public class PapertrailSettings : ScriptableObject
    {
        // Resources path where the logger settings are stored.
        private const string s_settingsPath = "PapertrailSettings";

        // Remote server to send the messages to.
        [Header("Remote server IP or hostname to log messages")]
        public string hostname = "localhost";
        // Remote server port.
        [Header("Remote server port")]
        public int port = 514;
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
        public bool logClientIPAdress = true;

        /// <summary>
        /// Loads the default settings file
        /// </summary>
        public static PapertrailSettings LoadSettings()
        {
            return Resources.Load<PapertrailSettings>(s_settingsPath);
        }

        /// <summary>
        /// Ensures all settings are correct
        /// </summary>
        private void OnValidate()
        {
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