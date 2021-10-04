using System;
using CoreLib;
using UnityEngine;

namespace CoreLib
{
    public abstract class GlobalValue : ScriptableObject
    {
        internal virtual void Init() {}
    }

    public interface IGlobalValue<T>
    {
        T Value { get; set; }
        event Action<T>  OnChanged;
    }

    public class GlobalValue<T> : GlobalValue, IGlobalValue<T>, IRefGet<T>, IRefSet<T>
    {
        [SerializeField]
        private T m_Value;
        public event Action<T>  OnChanged;
        
        public T Value
        {
            get => m_Value;
            set
            {
                if (Equals(m_Value, value))
                    return;

                m_Value = value;
                OnChanged?.Invoke(m_Value);
            }
        }

        public bool HasValue => true;

        [SerializeField]
        protected T m_InitialValue;

        // =======================================================================
        internal override void Init()
        {
            OnChanged = null;
            Value     = m_InitialValue;
        }
    }

    public abstract class GlobalValuePlayable<T> : GlobalValue, IGlobalValue<T>, IRefGet<T>, IRefSet<T>, IPlayableValue
    {
        [SerializeField]
        protected T m_InitialValue;
        [SerializeField]
        private T m_Value;
        public event Action<T>  OnChanged;
        
        public T Value
        {
            get => m_Value;
            set
            {
                if (IsLocked)
                {
                    m_LockValue = value;
                    SetValue(_set(m_LockValue, m_CurveValue, m_LockWeight));
                }
                else
                    SetValue(value);
            }
        }

        public bool HasValue => true;

        private T     m_LockValue;
        private float m_LockWeight;
        private float m_CurveValue;
        private int   m_Lock;

        private bool IsLocked => m_Lock > 0;

        // =======================================================================
        internal override void Init()
        {
            OnChanged = null;
            Value = m_InitialValue;
        }

        void IPlayableValue.Lock()
        {
            if (m_Lock ++ <= 0)
                m_LockValue = m_Value;
        }

        void IPlayableValue.Set(float curveValue, float weight)
        {
            m_LockWeight = weight;
            m_CurveValue = curveValue;

            SetValue(_set(m_LockValue, m_CurveValue, m_LockWeight));
        }

        void IPlayableValue.UnLock()
        {
            if (m_Lock -- > 0)
                Value = m_LockValue;
        }

        // =======================================================================
        private void SetValue(T value)
        {
            if (Equals(m_Value, value))
                return;

            m_Value = value;
            OnChanged?.Invoke(m_Value);
        }

        private void OnValidate()
        {
            OnChanged?.Invoke(m_Value);
        }

        protected abstract T _set(T initialValue, float curveValue, float weight);
    }
}