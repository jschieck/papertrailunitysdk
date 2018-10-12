using UnityEngine;

namespace Papertrail
{
    public class PapertrailSettings : ScriptableObject
    {
        private const string s_settingsPath = "PapertrailSettings";

        public string hostname = "yourid.papertrail.com";
        public int port = 123;
        public Severity loggingLevel = Severity.Debug;
        public Facility facility = Facility.local0;

        public static PapertrailSettings LoadSettings()
        {
            return Resources.Load<PapertrailSettings>(s_settingsPath);
        }

#if UNITY_EDITOR
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
        }
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