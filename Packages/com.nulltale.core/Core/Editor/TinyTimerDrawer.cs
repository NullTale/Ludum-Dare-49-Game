using UnityEditor;
using UnityEngine;

namespace CoreLib
{
    [CustomPropertyDrawer(typeof(TinyTimer), true)]
    public class TinyTimerDrawer : PropertyDrawer
    {
        private const int k_AutoResetWidth = 44;

        // =======================================================================
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var autoReset = property.FindPropertyRelative("m_AutoReset");
            var currentTime = property.FindPropertyRelative("m_CurrentTime");
            var finishTime = property.FindPropertyRelative("m_FinishTime");
            
            if (Application.isPlaying)
            {
                EditorGUI.PrefixLabel(position, label);
                var space = position.IncXY(EditorGUIUtility.labelWidth, 0.0f).IncWidth(-(EditorGUIUtility.labelWidth + k_AutoResetWidth + 4));
                var half  = space.width * 0.5f;

                currentTime.floatValue = EditorGUI.FloatField(space.IncWidth(-half), currentTime.floatValue);
                finishTime.floatValue  = EditorGUI.FloatField(space.IncWidth(-half).IncX(half), finishTime.floatValue);
            }
            else
            {
                currentTime.floatValue = 0.0f;
                finishTime.floatValue = EditorGUI.FloatField(position.IncWidth(-(k_AutoResetWidth + 4)), property.displayName, finishTime.floatValue);
            }
            autoReset.boolValue = EditorGUI.ToggleLeft(position.WithXY(position.xMax - k_AutoResetWidth, position.yMin).WithWidth(k_AutoResetWidth).WithHeight(position.height), 
                                                       new GUIContent("Auto", "Auto reset Timer"), 
                                                       autoReset.boolValue);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}