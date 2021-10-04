using UnityEditor.Callbacks;
using UnityEngine;

namespace CoreLib
{
    public static class EditorGlobal
    {
        [PostProcessScene (2)]
        public static void OnPostprocessScene()
        {
            foreach (var comment in GameObject.FindObjectsOfType<Comment>())
            {
                Object.Destroy(comment);
            }
        }
    }
}