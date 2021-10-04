using System;
using System.Linq;
using CoreLib;
using UnityEditor;
using UnityEngine;

namespace CoreLib
{
    [CustomPropertyDrawer(typeof(GlobalValue))]
    public class GlobalValueDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DrawGlobalValue(position, property, label);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return GetGlobalValueHeight(property);
        }

        public static void DrawGlobalValue(Rect position, SerializedProperty property, GUIContent label)
        {
            var isSet = property.objectReferenceValue != null;
            if (isSet)
            {
                EditorGUI.PropertyField(position.WithWidth(EditorGUIUtility.labelWidth).WithHeight(EditorGUIUtility.singleLineHeight), property, GUIContent.none, true);
                if (property.objectReferenceValue != null)
                    using (var so = new SerializedObject(property.objectReferenceValue))
                    {
                        var val = so.FindProperty("m_Value");
                        EditorGUI.BeginChangeCheck();
                        EditorGUI.PropertyField(position, val, new GUIContent(" "), true);
                        if (EditorGUI.EndChangeCheck())
                            so.ApplyModifiedProperties();
                    }

            }
            else
            {
                EditorGUI.PropertyField(position.WithHeight(EditorGUIUtility.singleLineHeight), property, label, true);
            }
        }

        public static float GetGlobalValueHeight(SerializedProperty property)
        {
            if (property.objectReferenceValue != null)
            {
                using (var so = new SerializedObject(property.objectReferenceValue))
                    return EditorGUI.GetPropertyHeight(so.FindProperty("m_Value"), true);
            }

            return EditorGUIUtility.singleLineHeight;
        }
    }
}