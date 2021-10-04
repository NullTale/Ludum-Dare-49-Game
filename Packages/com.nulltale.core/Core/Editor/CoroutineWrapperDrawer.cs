using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace CoreLib
{
    [CustomPropertyDrawer(typeof(CoroutineWrapper))]
    public class CoroutineWrapperDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var ownerField = property.FindPropertyRelative("m_Owner");

            if (ownerField.objectReferenceValue != null)
            {
                property.isExpanded = EditorGUI.Foldout(position.WithHeight(EditorGUIUtility.singleLineHeight).WithWidth(EditorGUIUtility.labelWidth),
                    property.isExpanded, label, true);


                var functionNameField = property.FindPropertyRelative("m_EnumeratorFunctionName");
                // get only (IEnumerator ..(Void)) functions
                var methods = ownerField.objectReferenceValue
                    .GetType()
                    .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .Where(n => n.ReturnType == typeof(IEnumerator))
                    .ToList();

                // prepare data for popup
                var methodInfo = methods.FirstOrDefault((n) => n.Name == functionNameField.stringValue);
                var index = methodInfo == null ? 0 : methods.IndexOf(methodInfo) + 1;
                var options = methods.Select(n => n.Name).Prepend("_").ToArray();

                // finally show popup
                var selected = EditorGUI.Popup(position.WithHeight(EditorGUIUtility.singleLineHeight), " ", index, options);

                // update string value
                if (selected != index)
                {
                    functionNameField.stringValue = selected == 0 ? "" : options[selected];
                    functionNameField.serializedObject.ApplyModifiedProperties();
                }
                
                if (property.isExpanded)
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        // object
                        EditorGUI.ObjectField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight), ownerField);
                    }
                }
            }
            else
            {
                EditorGUI.ObjectField(position, ownerField, label);
                property.isExpanded = false;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.isExpanded == false || property.FindPropertyRelative("m_Owner").objectReferenceValue == null)
                return EditorGUIUtility.singleLineHeight;

            return EditorGUIUtility.singleLineHeight * 2.0f;
        }
    }
}