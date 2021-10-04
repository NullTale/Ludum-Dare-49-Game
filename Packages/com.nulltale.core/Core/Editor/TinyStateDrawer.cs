using UnityEditor;
using UnityEngine;

namespace CoreLib
{
    [CustomPropertyDrawer(typeof(TinyState))]
    public class TinyStateDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var tinyState = property.GetSerializedValue<TinyState>();

            EditorGUI.BeginChangeCheck();
            var state = EditorGUI.Toggle(position, property.displayName, tinyState.IsActive);
            if (EditorGUI.EndChangeCheck())
            {
                if (tinyState.IsActive != state)
                {
                    tinyState.IsActive = state;
                    property.serializedObject.ApplyModifiedProperties();
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}