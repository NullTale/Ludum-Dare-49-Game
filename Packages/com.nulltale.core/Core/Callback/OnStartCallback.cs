using UnityEngine;
using System;
using UnityEngine.Events;

namespace CoreLib
{
    [DefaultExecutionOrder(-1)]
    public class OnStartCallback : MonoBehaviour
    {
        public UnityEvent OnStartEvent;

        // =======================================================================
        private void Start()
        {
            OnStartEvent.Invoke();
        }
    }
}