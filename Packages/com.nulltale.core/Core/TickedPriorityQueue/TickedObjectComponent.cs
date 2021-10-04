using System;
using UnityEngine;

namespace CoreLib
{
    public class TickedObjectComponent : MonoBehaviour, ITicked
    {
        [SerializeField]
        private TickedQueue m_UpdateQueue;
        [SerializeField]
        private float m_TickLength;

        public float TickLength
        {
            get => m_TickLength;
            set => m_TickLength = value;
        }

        public Action Action;

        //////////////////////////////////////////////////////////////////////////
        public void OnTicked() => Action?.Invoke();

        private void OnEnable()
        {
            m_UpdateQueue.Add(this);
        }

        private void OnDisable()
        {
            m_UpdateQueue.Remove(this);
        }
    }
}