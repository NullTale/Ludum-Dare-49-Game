using UnityEditor;
using UnityEditor.AI;
using UnityEngine;

namespace CoreLib
{
    [CustomPropertyDrawer(typeof(NavMeshAreaAttribute))]
    public class NavMeshAreaDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
        {
            var areaIndex = -1;
            var areaNames = GameObjectUtility.GetNavMeshAreaNames();
            for (var i = 0; i < areaNames.Length; i++)
            {
                var areaValue = GameObjectUtility.GetNavMeshAreaFromName(areaNames[i]);
                if (areaValue == prop.intValue)
                    areaIndex = i;
            }
            ArrayUtility.Add(ref areaNames, "");
            ArrayUtility.Add(ref areaNames, "Open Area Settings...");

            var rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
            EditorGUI.BeginProperty(rect, GUIContent.none, prop);

            EditorGUI.BeginChangeCheck();
            areaIndex = EditorGUI.Popup(rect, prop.displayName, areaIndex, areaNames);

            if (EditorGUI.EndChangeCheck())
            {
                if (areaIndex >= 0 && areaIndex < areaNames.Length - 2)
                    prop.intValue = GameObjectUtility.GetNavMeshAreaFromName(areaNames[areaIndex]);
                else if (areaIndex == areaNames.Length - 1)
                    NavMeshEditorHelpers.OpenAreaSettings();
            }

            EditorGUI.EndProperty();
        }
    }
}