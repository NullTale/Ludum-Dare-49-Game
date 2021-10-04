using UnityEditor;
using UnityEngine;

namespace CoreLib
{
    [CustomPropertyDrawer(typeof(ResourceFolder))]
    public class ResourceFolderDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var guid = property.FindPropertyRelative("m_GUID");
            var path = property.FindPropertyRelative("m_Path");

            var guidToAssetPath = AssetDatabase.GUIDToAssetPath(guid.stringValue);
            var folder          = AssetDatabase.LoadAssetAtPath<DefaultAsset>(guidToAssetPath);
            
            // if folder was renamed
            var res = _getResourcePath(guidToAssetPath);
            if (res != path.stringValue)
                path.stringValue = res;

            // if folder was lost
            if (folder == null)
                path.stringValue = string.Empty;

            if (Event.current.alt)
            {
                // as string
                EditorGUI.LabelField(position, label);
                using (new Utils.DisablingScope())
                    EditorGUI.TextField(position, new GUIContent(" "), path.stringValue);
            }
            else
            {
                // as object field
                var picked = EditorGUI.ObjectField(position, label, folder, typeof(DefaultAsset), false);
                if (picked != folder)
                {
                    var assetPath    = AssetDatabase.GetAssetPath(picked);
                    var resourcePath = _getResourcePath(assetPath);

                    if (resourcePath.IsNullOrEmpty() == false)
                    {
                        path.stringValue = resourcePath;
                        guid.stringValue = AssetDatabase.AssetPathToGUID(assetPath);
                    }
                    else
                    {
                        path.stringValue = string.Empty;
                        guid.stringValue = string.Empty;
                    }
                }
            }

            // -----------------------------------------------------------------------
            string _getResourcePath(string assetPath)
            {
                const string k_Resources = "Resources/";

                var index = assetPath.IndexOf(k_Resources);
                if (index == -1)
                    return string.Empty;

                return assetPath.Substring(index + k_Resources.Length);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}