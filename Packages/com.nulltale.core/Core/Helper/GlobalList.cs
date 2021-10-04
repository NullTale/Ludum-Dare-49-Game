using System.Collections.Generic;
using UnityEngine;

namespace CoreLib
{
    public class GlobalList<T>
    {
        internal static List<T>                s_List = new List<T>();
        public static   IReadOnlyCollection<T> Content => s_List;

        /// <summary> creates wrapper component </summary>
        public static void Create(GameObject owner)
        {
            var callback = owner.AddComponent<ToggleCallback>();
            // OnEnable was called
            s_List.Add(owner.GetComponent<T>());

            callback.Enable = () => s_List.Add(owner.GetComponent<T>());
            callback.Disable = () => s_List.Remove(owner.GetComponent<T>());
        }

        public static void Add(T item)
        {
            s_List.Add(item);
        }
        
        public static void Remove(T item)
        {
            s_List.Remove(item);
        }
    }
}