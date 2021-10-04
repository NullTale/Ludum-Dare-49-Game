using UnityEditor;
using UnityEngine;

namespace CoreLib
{
    [CustomPropertyDrawer(typeof(WrapperOfAttribute), true)]
    public class WrapperOfDrawer : PropertyDrawer 
    {
        public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label) 
        {
            EditorGUI.PropertyField(pos, prop.FindPropertyRelative(((WrapperOfAttribute)attribute).PropertyName), new GUIContent(prop.displayName), true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property.FindPropertyRelative(((WrapperOfAttribute)attribute).PropertyName));
        }
    }
}