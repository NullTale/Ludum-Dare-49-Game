using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.States
{
    public class StateTrigger : MonoBehaviour
    {
        [SerializeField] [Expandable]
        private GlobalState m_State;

        public bool m_ApplyInitial;

        public UnityEvent   OnOpen;
        public UnityEvent   OnClose;

        // =======================================================================
        protected void Start()
        {
            m_State.OnOpen  += OnOpen.Invoke;
            m_State.OnClose += OnClose.Invoke;

            if (m_ApplyInitial)
                (m_State.IsOpen ? OnOpen : OnClose).Invoke();
        }

        private void OnDestroy()
        {
            m_State.OnOpen  -= OnOpen.Invoke;
            m_State.OnClose -= OnClose.Invoke;
        }
    }
}