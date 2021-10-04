using System;
using UnityEditor;
using UnityEngine;

namespace CoreLib
{
    public class RenameWindow : EditorWindow
    {
        private string         m_Text;
        private Action<string> m_OnApply;

        // =======================================================================
        public static void Show(string text, Vector2 pos, Action<string> onApply)
        {
            var window = ScriptableObject.CreateInstance<RenameWindow>();

            window.position  = new Rect(pos.x, pos.y, 250, EditorGUIUtility.singleLineHeight * 2f);
            window.m_Text    = text;
            window.m_OnApply = onApply;

            window.ShowAsDropDown(window.position, new Vector2(250f, EditorGUIUtility.singleLineHeight * 2.5f));
        }

        private void OnGUI()
        {
            var e = Event.current;
            if (e.type == EventType.KeyDown && (e.keyCode == KeyCode.Return || e.keyCode == KeyCode.KeypadEnter))
            {
                m_OnApply?.Invoke(m_Text);
                Close();
            }

            GUI.SetNextControlName("edit");
            m_Text = EditorGUILayout.TextField(GUIContent.none, m_Text);

            EditorGUI.FocusTextInControl("edit");

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Apply"))
            {
                m_OnApply?.Invoke(m_Text);
                Close();
            }
            if (GUILayout.Button("Cancel"))
            {
                Close();
            }
            GUILayout.EndHorizontal();
        }
    }
}