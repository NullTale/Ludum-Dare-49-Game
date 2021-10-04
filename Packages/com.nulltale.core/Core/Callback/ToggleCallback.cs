using System;
using UnityEngine;

namespace CoreLib
{
    public class ToggleCallback : MonoBehaviour
    {
        public Action Enable;
        public Action Disable;

        //////////////////////////////////////////////////////////////////////////
        protected void Awake()
        {
            hideFlags = HideFlags.HideInInspector;
        }

        private void OnEnable()
        {
            Enable?.Invoke();
        }

        private void OnDisable()
        {
            Disable?.Invoke();
        }
    }
}