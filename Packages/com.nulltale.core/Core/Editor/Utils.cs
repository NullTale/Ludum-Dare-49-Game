using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CoreLib
{
    public static class Utils
    {
        public class DisablingScope : IDisposable
        {
            private static int s_DisableCounter = 0;

            private bool m_Disable;

            // -----------------------------------------------------------------------
            public void Dispose()
            {
                if (m_Disable && (-- s_DisableCounter <= 0))
                    GUI.enabled = true;
            }

            public DisablingScope(bool disable = true)
            {
                m_Disable = disable;
                if (m_Disable && (++ s_DisableCounter > 0))
                    GUI.enabled = false;
            }
        }

        // =======================================================================
        public static Rect GetRowDown(this Rect rect, int line, float lineHeight)
        {
            return new Rect(rect.xMin, rect.yMax - lineHeight * (line + 1), rect.width, lineHeight);
        }
        
        public static Rect GetRowUp(this Rect rect, int line, float lineHeight)
        {
            return new Rect(rect.xMin, rect.yMin + lineHeight * (line + 1), rect.width, lineHeight);
        }

        public static IEnumerable<Object> GetNestedAssets(this SerializedObject so)
        {
            return AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(so.targetObject))
                                .Except(new []{so.targetObject});
        }

        public static Object CreateNestedAsset(this SerializedObject so, Object instance)
        {
            AssetDatabase.AddObjectToAsset(instance, so.targetObject);
            AssetDatabase.SaveAssets();

            so.ApplyModifiedProperties();

            return instance;
        }

        public static Object CreateNestedAsset(this SerializedObject so, Type type, string name)
        {
            var result = ScriptableObject.CreateInstance(type);
            result.name = name;
            AssetDatabase.AddObjectToAsset(result, so.targetObject);
            AssetDatabase.SaveAssets();

            so.ApplyModifiedProperties();

            return result;
        }

        public static T RequireNesteAsset<T>(this SerializedObject so, string name) where T : Object
        {
            var result = so.GetNestedAssets().OfType<T>().FirstOrDefault(n => n.name == name);
            if (result == null)
                result = (T)so.CreateNestedAsset(typeof(T), name);

            return result;
        }
        
        public static Rect Label(this Rect rect)
        {
            return new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
        }
        
        public static Rect Field(this Rect rect)
        {
            var offset = EditorGUIUtility.labelWidth + EditorGUIUtility.standardVerticalSpacing;
            return new Rect(rect.x + offset, rect.y, rect.width - offset, EditorGUIUtility.singleLineHeight);
        }
        
        public static T GetSerializedValue<T>(this SerializedProperty property)
        {
            object   targetObject  = property.serializedObject.targetObject;
            string[] propertyNames = property.propertyPath.Split('.');
     
            // Clear the property path from "Array" and "data[i]".
            if (propertyNames.Length >= 3 && propertyNames[propertyNames.Length - 2] == "Array")
                propertyNames = propertyNames.Take(propertyNames.Length - 2).ToArray();
     
            // Get the last object of the property path.
            foreach (string path in propertyNames)
            {
                targetObject = targetObject.GetType()
                                           .GetField(path, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                                           .GetValue(targetObject);
            }
     
            if (targetObject != null && targetObject.GetType().GetInterfaces().Contains(typeof(IList<T>)))
            {
                int propertyIndex = int.Parse(property.propertyPath[property.propertyPath.Length - 2].ToString());
     
                return ((IList<T>) targetObject)[propertyIndex];
            }
            else return (T) targetObject;
        }

        public static Type GetFieldType(this SerializedProperty property)
        {
            var slices = property.propertyPath.Split('.');
            var type   = property.serializedObject.targetObject.GetType();

            for (var i = 0; i < slices.Length; i++)
                if (slices[i] == "Array")
                {
                    i++;                          //skips "data[x]"
                    type = type.GetElementType(); //gets info on array elements
                }
                //gets info on field and its type
                else 
                    type = type.GetField(slices[i], BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance).FieldType;

            return type;
        }
        
        public static IEnumerable<SerializedProperty> GetList(this SerializedProperty property)
        {
            for (var n = 0; n < property.arraySize; n++)
            {
                yield return property.GetArrayElementAtIndex(n);
            }
        }
        public static void SetList<T>(this SerializedProperty property, IEnumerable<T> list)
        {
            var index = 0;
            foreach (var n in list)
            {
                var item = property.GetArrayElementAtIndex(index);
                switch (n)
                {
                    case int i:
                        item.intValue = i;
                        break;

                    case Object obj:
                        item.objectReferenceValue = obj;
                        break;

                    case string str:
                        item.stringValue = str;
                        break;
                }

                index ++;
            }

        }
        
        public static void PlayClip(AudioClip clip, int startSample = 0, bool loop = false)
        {
            var unityEditorAssembly = typeof(AudioImporter).Assembly;
         
            var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            var method = audioUtilClass.GetMethod(
                "PlayPreviewClip",
                BindingFlags.Static | BindingFlags.Public,
                null,
                new[] { typeof(AudioClip), typeof(int), typeof(bool) },
                null
            );
     
            method.Invoke(null, new object[] { clip, startSample, loop });
        }
        
        public static void StopAllClips()
        {
            var unityEditorAssembly = typeof(AudioImporter).Assembly;
     
            var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            var method = audioUtilClass.GetMethod(
                "StopAllPreviewClips",
                BindingFlags.Static | BindingFlags.Public,
                null,
                new Type[] { },
                null
            );
     
            method.Invoke(null, new object[] { });
        }

        public static float GetObjectReferenceHeight(this SerializedProperty element)
        {
            return GetObjectReferenceHeight(element.objectReferenceValue, element.isExpanded);
        }

        public static float GetObjectReferenceHeight(this Object obj, bool isExpanded)
        {
            if (isExpanded)
            {
                using var so          = new SerializedObject(obj);
                var       totalHeight = EditorGUIUtility.singleLineHeight;

                using (var iterator = so.GetIterator())
                {
                    if (iterator.NextVisible(true))
                    {
                        do
                        {
                            var childProperty = so.FindProperty(iterator.name);
                            if (childProperty.name.Equals("m_Script", System.StringComparison.Ordinal))
                                continue;
							if (NaughtyAttributes.Editor.PropertyUtility.IsVisible(childProperty) == false)
                                continue;

                            totalHeight += EditorGUI.GetPropertyHeight(childProperty);
                        }
                        while (iterator.NextVisible(false));
                    }
                }

                totalHeight += EditorGUIUtility.standardVerticalSpacing;
                return totalHeight;
            }

            return EditorGUIUtility.singleLineHeight;
        }

        public static void DrawObjectReference(this SerializedProperty element, Rect position)
        {
            DrawObjectReference(element.objectReferenceValue, element.isExpanded, position);
        }

        public static void DrawObjectReference(this Object obj, bool isExpanded, Rect position, bool decorativeBox = false)
        {
            if (obj == null)
                return;

            if (isExpanded)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    using var so = new SerializedObject(obj);

                    EditorGUI.BeginChangeCheck();

                    using (var iterator = so.GetIterator())
                    {
                        var yOffset = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                        if (iterator.NextVisible(true))
                        {
                            do
                            {
                                var childProperty = so.FindProperty(iterator.name);

                                if (childProperty.name.Equals("m_Script", StringComparison.Ordinal))
                                    continue;

                                if (NaughtyAttributes.Editor.PropertyUtility.IsVisible(childProperty) == false)
                                    continue;

                                var childHeight = EditorGUI.GetPropertyHeight(childProperty);
                                var childRect = new Rect()
                                {
                                    x      = position.x,
                                    y      = position.y + yOffset,
                                    width  = position.width,
                                    height = childHeight
                                };

                                EditorGUI.PropertyField(childRect, iterator, true);
                                
                                yOffset += childHeight;
                            }
                            while (iterator.NextVisible(false));
                        }

                        if (decorativeBox)
			                GUI.Box(position.WithX(0f).IncY(EditorGUIUtility.singleLineHeight).IncWidth(100f).WithHeight(yOffset - EditorGUIUtility.singleLineHeight), GUIContent.none);
                    }

                    if (EditorGUI.EndChangeCheck())
                        so.ApplyModifiedProperties();
                }
            }
        }
    }
}