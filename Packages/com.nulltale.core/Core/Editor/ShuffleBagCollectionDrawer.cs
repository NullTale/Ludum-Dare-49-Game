using UnityEditor;
using UnityEngine;

namespace CoreLib
{
    [CustomPropertyDrawer(typeof(ShuffleBag<>), true)]
    public class ShuffleBagCollectionDrawer : PropertyDrawer 
    {
        public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label) 
        {
            EditorGUI.PropertyField(pos, prop.FindPropertyRelative("m_Values"), new GUIContent(prop.displayName), true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_Values"));
        }
    }
}