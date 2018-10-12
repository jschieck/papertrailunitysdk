using UnityEditor;
using UnityEngine;

namespace Papertrail
{
    [CustomEditor(typeof(PapertrailSettings))]
    public class PapertrailSettingsInspector : Editor
    {
        private Texture2D m_logo;
        private void OnEnable()
        {
            m_logo = Resources.Load<Texture2D>("pt-logo");
        }
        public override void OnInspectorGUI()
        {
            if (m_logo != null)
            {
                GUILayout.Label(new GUIContent(m_logo));
            }
            base.OnInspectorGUI();
        }
    }
}