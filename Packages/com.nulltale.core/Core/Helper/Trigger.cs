using System;
using UnityEngine;
using UnityEngine.Events;

namespace CoreLib
{
    public class Trigger : MonoBehaviour
    {
        public UnityEvent m_OnEnbale;
        public UnityEvent m_OnDisable;

        // =======================================================================
        public void Setup(Action onEnable, Action onDisable)
        {
            (m_OnEnbale ??= new UnityEvent()).AddListener(onEnable.Invoke);
            (m_OnDisable ??= new UnityEvent()).AddListener(onDisable.Invoke);
        }

        private void OnEnable()
        {
            m_OnEnbale?.Invoke();
        }

        private void OnDisable()
        {
            m_OnDisable?.Invoke();
        }
    }
}