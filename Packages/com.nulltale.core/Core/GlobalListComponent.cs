using System.Collections.Generic;
using UnityEngine;

namespace CoreLib
{
    public abstract class GlobalListComponent<T> : MonoBehaviour
        where T : GlobalListComponent<T>
    {
        public static List<T> GlobalList => GlobalList<T>.s_List;
        
        // =======================================================================
        protected virtual void OnEnable()
        {
            GlobalList.Add((T)this);
        }
        
        protected virtual void OnDisable()
        {
            GlobalList.Remove((T)this);
        }
    }
}