using UnityEditor;
using UnityEngine;

namespace CoreLib
{
    [CustomPropertyDrawer(typeof(GlobalValueLink<>))]
    public class GlobalValueLinkDrawer : PropertyDrawer
    {
        private const float k_ToggleWidth = 18;

        // =======================================================================
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var isOverride = property.FindPropertyRelative("m_Override");

            position.width -= k_ToggleWidth;
            if (isOverride.boolValue)
            {
                var globalValueOverride = property.FindPropertyRelative("m_GlobalValueOverride");
                EditorGUI.PropertyField(position, globalValueOverride, label, true);
            }
            else
            {
                var globalValue = property.FindPropertyRelative("m_GlobalValue");
                GlobalValueDrawer.DrawGlobalValue(position, globalValue, label);
            }

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var togglePos = new Rect(position.x + position.width + EditorGUIUtility.standardVerticalSpacing, position.y, k_ToggleWidth, EditorGUIUtility.singleLineHeight);
            isOverride.boolValue = EditorGUI.Toggle(togglePos, GUIContent.none, isOverride.boolValue);

            EditorGUI.indentLevel = indent;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.FindPropertyRelative("m_Override").boolValue)
                return GlobalValueDrawer.GetGlobalValueHeight(property.FindPropertyRelative("m_GlobalValue"));

            return EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_GlobalValueOverride"), true);;
        }
    }
}