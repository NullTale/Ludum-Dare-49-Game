using System;
using UnityEngine;

namespace CoreLib
{
    [Serializable]
    public class TinyState
    {
        public bool   IsActive
        {
            get => LockValue > 0;
            set => LockValue = (value ? 1 : 0);
        }

        [SerializeField]
        private int m_LockValue;
        private int LockValue
        {
            get => m_LockValue;
            set
            {
                if (m_LockValue == value)
                    return;

                if (IsActive)
                {
                    m_LockValue = value;
                    if (IsActive == false)
                        OnDisable?.Invoke();
                }
                else
                {
                    m_LockValue = value;
                    if (IsActive)
                        OnEnable?.Invoke();
                }
            }
        }

        public event Action OnEnable;
        public event Action OnDisable;

        // =======================================================================
        public TinyState(bool enable)
        {
            m_LockValue = enable ? 1 : 0;
        }

        public TinyState(Action onEnable, Action onDisable = null)
        {
            OnEnable += onEnable;
            OnDisable += onDisable;
        }

        public TinyState()
        {
        }

        public void On()
        {
            LockValue ++;
        }
        
        public void Off()
        {
            LockValue --;
        }

        public void Invoke()
        {
            (IsActive ? OnEnable : OnDisable)?.Invoke();
        }

        public void SetActive(bool on)
        {
            IsActive = on;
        }

        public TinyState Ready(bool enable, bool invokeEvent = true)
        {
            m_LockValue = enable ? 1 : 0;
            if (invokeEvent)
                (IsActive ? OnEnable : OnDisable)?.Invoke();

            return this;
        }

        public static implicit operator bool(TinyState state)
        {
            return state.IsActive;
        }
    }
}