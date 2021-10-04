#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace CoreLib
{
    [CustomPropertyDrawer(typeof(ClassReferenceAttribute))]
    public class ClassReferenceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.ManagedReference)
            {
                // draw default
                EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, position.height),
                    property, true);
                return;
            }

            // draw select type btn
            if (GUI.Button(new Rect(position.x + EditorGUIUtility.labelWidth, position.y, position.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight),
                           _fullTypeNameToTypeName(property.managedReferenceFullTypename)))
            {
                // object type means null value
                var typeList = _getTypeList();
                typeList.Insert(0, typeof(object));

                ObjectPickerWindow.Show(picked =>
                {
                    var selectedType = (Type)picked;
                    property.managedReferenceValue = selectedType == typeof(object) ? null : Activator.CreateInstance(selectedType);
                    property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
                }, null, typeList, 0, n => n == typeof(object) ? new GUIContent("Null") : new GUIContent(n.FullName), null);
            }
            // in header rect
            if (Event.current.type == EventType.ContextClick 
                && new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight)
                    .Contains(Event.current.mousePosition))
            {
                // create context menu
                var context = new GenericMenu();
                
                // null option
                context.AddItem(new GUIContent ("Null"), false, () =>
                {
                    property.managedReferenceValue = null;
                    property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
                });

                // fill context menu types
                foreach (var type in _getTypeList())
                {
                    context.AddItem(new GUIContent(type.FullName), false, () => _setProperty(type));
                }

                // show context window
                context.ShowAsContext();
            }

            // draw default
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, position.height),
                property, true);
		
        
            // ===================================
            void filterTypes(Assembly asm, List<Type> output) 
            {
                Type fieldType = null;
            
                // is list field
                if (fieldInfo.FieldType.IsGenericType && fieldInfo.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                    fieldType = fieldInfo.FieldType.GetGenericArguments()[0];
                else 
                    // is array
                if (fieldInfo.FieldType.IsArray)
                    fieldType = fieldInfo.FieldType.GetElementType();
                else 
                    // default field
                    fieldType = fieldInfo.FieldType;

                foreach (var type in asm.GetTypes()) 
                {
                    if (type.IsVisible == false)
                        continue;

                    if (type.IsClass == false)
                        continue;
				
                    if (type.IsGenericType)
                        continue;

                    if (type.IsAbstract)
                        continue;

                    if (type.IsSerializable == false)
                        continue;

                    if (fieldType.IsAssignableFrom(type) == false)
                        continue;

                    //if (filterCheck(type) == false)
                    //continue;

                    output.Add(type);
                }
            }

            List<Type> _getTypeList()
            {
                var typeList = new List<Type>();
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                    filterTypes(assembly, typeList);

                typeList.Sort((x, y) => string.CompareOrdinal(x.FullName, y.FullName));
                return typeList;
            }

            void _setProperty(Type type)
            {
                property.managedReferenceValue = Activator.CreateInstance(type);
                property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }

            string _fullTypeNameToTypeName(string fullTypeName)
            {
                if (string.IsNullOrEmpty(fullTypeName))
                    return "";

                var index = fullTypeName.LastIndexOf('.');
                if (index == -1)
                    return fullTypeName;

                return fullTypeName.Substring(index + 1);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property);
        }
    }
}

#endif