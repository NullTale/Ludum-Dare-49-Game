using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CoreLib
{
    [CustomPropertyDrawer(typeof(InplaceFieldAttribute))]
    public class InplaceFieldDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var pos = position.WithHeight(0f);
            foreach (var propPath in (attribute as InplaceFieldAttribute).PropertyPath)
            {
                var prop = property.FindPropertyRelative(propPath);
                pos.y += pos.height;
                pos.height = EditorGUI.GetPropertyHeight(prop, true);
                EditorGUI.PropertyField(pos, prop, true);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (attribute as InplaceFieldAttribute).PropertyPath.Sum(n => EditorGUI.GetPropertyHeight(property.FindPropertyRelative(n), true));
        }
    }
}