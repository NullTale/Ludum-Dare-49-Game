using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Assembly = UnityEditor.Compilation.Assembly;

namespace CoreLib
{
    public abstract class StringKeyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.LabelField(position, "Property field must be of text type");
                return;
            }

            StringKeyField(_getKeyList(), position, property, _getSelectionWindowLabel());
        }

        // =======================================================================
        public static void StringKeyField(List<string> keyList, Rect position, SerializedProperty property, string selectionWindowLabel,
                                          Action<string> onSelected = null, Action<string> onHover = null, Action onClose = null, Action onButtonContext = null)
        {
            if (keyList != null)
            {
                keyList.Sort();

                var currentKeyIndex = keyList.IndexOf(property.stringValue);
                var presented       = currentKeyIndex != -1;
                keyList.Insert(0, "");

                // label
                EditorGUI.LabelField(position, property.displayName);

                if (Event.current.type == EventType.MouseUp && Event.current.button == 1 && position.Field().Contains(Event.current.mousePosition))
                {
                    onButtonContext?.Invoke();
                }
                else
                if (presented)
                {
                    // selection button
                    _showButton();
                }
                else
                {
                    // change & save color
                    var tmpColor = GUI.backgroundColor;
                    GUI.backgroundColor = Color.red;

                    // selection button
                    _showButton();

                    // restore bg color
                    GUI.backgroundColor = tmpColor;
                }

                // context click
                if (Event.current.type == EventType.ContextClick && position.Label().Contains(Event.current.mousePosition))
                {
                    _showContextMenu();
                }

                // -----------------------------------------------------------------------
                void _showButton()
                {
                    if (GUI.Button(new Rect(position.x + EditorGUIUtility.labelWidth, position.y, position.width - EditorGUIUtility.labelWidth, position.height), property.stringValue))
                        ObjectPickerWindow.Show(picked =>
                        {
                            property.stringValue = (string)picked;
                            property.serializedObject.ApplyModifiedProperties();
                        }, property.stringValue, keyList, 0, s => new GUIContent(s), selectionWindowLabel,
                        true, s => onSelected?.Invoke(s.IsNull() ? "": (string)s), s => onHover?.Invoke(s.IsNull() ? "": (string)s), onClose);
                }

                void _showContextMenu()
                { // create context menu
                    var context = new GenericMenu();

                    // fill the menu, list could be large but for small projects, this is more convenient
                    foreach (var key in keyList)
                    {
                        context.AddItem(new GUIContent(key == "" ? "\t" : key), key == property.stringValue, setKey, key);

                        // do not create lambda for all items
                        void setKey(object userData)
                        {
                            property.stringValue = (string)userData;
                            property.serializedObject.ApplyModifiedProperties();
                        }
                    }

                    // show menu
                    context.ShowAsContext();
                }
            }
            else
            {
                // show default string
                EditorGUI.PropertyField(position, property, true);
            }

        }

        protected abstract List<string> _getKeyList();
        protected abstract string _getSelectionWindowLabel();
    }
}