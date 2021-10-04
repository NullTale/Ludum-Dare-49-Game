using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CoreLib
{
    [CustomPropertyDrawer(typeof(NavMeshAreaMaskAttribute))]
    public class NavMeshAreaMaskDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();
            var masks = GetMasks();

            var mask = _fromMask(EditorGUI.MaskField(pos, prop.displayName, _toMask(), masks.Select(n => n.Item1).ToArray()));
            if (EditorGUI.EndChangeCheck())
                prop.intValue = mask;

            ///////////////////////////////////
            int _toMask()
            {
                var result = 0;
                var bit = 1;
                for (var n = 0; n < 32; n++)
                {
                    if ((prop.intValue & bit) != 0)
                    {
                        var bitLocal = bit;
                        var index = masks.FindIndex(tuple => tuple.Item2 == bitLocal);
                        if (index != -1)
                            result |= 1 << index;
                    }

                    bit <<= 1;
                }

                return result;
            }

            int _fromMask(int maskField)
            {
                var result = 0;
                var bit    = 1;
                for (var n = 0; n < masks.Count; n++)
                {
                    if ((maskField & bit) != 0)
                        result |= masks[n].Item2;

                    bit <<= 1;
                }

                return result;
            }
        }

        public static List<(string, int)> GetMasks()
        {
            var areaNames = GameObjectUtility.GetNavMeshAreaNames();
            var result = new List<(string, int)>(areaNames.Length);
            foreach (var areaName in areaNames)
                result.Add((areaName, 1 << GameObjectUtility.GetNavMeshAreaFromName(areaName)));

            return result;
        }
    }
}