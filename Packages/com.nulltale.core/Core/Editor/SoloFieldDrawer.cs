using UnityEditor;
using UnityEngine;

namespace CoreLib
{
    [CustomPropertyDrawer(typeof(SoloFieldAttribute))]
    public class SoloFieldDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var prop = property.FindPropertyRelative((attribute as SoloFieldAttribute).PropertyPath);
            EditorGUI.PropertyField(position, prop, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property.FindPropertyRelative((attribute as SoloFieldAttribute).PropertyPath), true);
        }
    }
}