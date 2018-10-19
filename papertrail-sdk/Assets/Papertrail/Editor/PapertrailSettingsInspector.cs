using UnityEditor;
using UnityEngine;

namespace Papertrail
{
    [CustomEditor(typeof(PapertrailSettings))]
    public class PapertrailSettingsInspector : Editor
    {
        private const string s_dashboardUrl = "https://papertrailapp.com/dashboard";
        private Texture2D m_logo;
        private GUIStyle m_urlStyle;

        private void OnEnable()
        {
            m_logo = Resources.Load<Texture2D>("pt-logo");
        }
        public override void OnInspectorGUI()
        {
            if (m_urlStyle == null)
            {
                m_urlStyle = new GUIStyle(GUI.skin.button);
                m_urlStyle.fontStyle = FontStyle.Bold;
                m_urlStyle.fontSize = 15;
            }
            if (m_logo != null)
            {
                GUILayout.Label(new GUIContent(m_logo));
            }
            if (GUILayout.Button("Open Dashboard", m_urlStyle))
            {
                Application.OpenURL(s_dashboardUrl);
            }
            GUILayout.Space(10);
            base.OnInspectorGUI();
        }
    }
}
