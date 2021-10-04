using System;
using System.Collections;
using System.Collections.Generic;
using Unity.EditorCoroutines.Editor;
using UnityEditor;

namespace CoreLib
{
    public static class DelayedUpdate
    {
        private static Dictionary<string, UpdateData> s_Coroutines = new Dictionary<string, UpdateData>();

        // =======================================================================
        private class UpdateData
        {
            public double          StartTime;
            public double          Delay;
            public EditorCoroutine Coroutine;
        }

        // =======================================================================
        public static void Plan(Action action, double delay = 2d)
        {
            Plan(action.Method.Name, delay, action);
        }

        public static void Plan(string key, double delay, Action action)
        {

            if (s_Coroutines.ContainsKey(key) == false)
                s_Coroutines.Add(key, new UpdateData());

            var updateData = s_Coroutines[key];
            updateData.StartTime = EditorApplication.timeSinceStartup;
            updateData.Delay     = delay;

            if (updateData.Coroutine != null)
                return;

            updateData.Coroutine = EditorCoroutineUtility.StartCoroutineOwnerless(_dirtyUpdate());

            // ===================================
            IEnumerator _dirtyUpdate()
            {
                while (EditorApplication.timeSinceStartup - updateData.StartTime < updateData.Delay)
                    yield return null;

                action?.Invoke();
                updateData.Coroutine = null;
            }
        }
    }
}