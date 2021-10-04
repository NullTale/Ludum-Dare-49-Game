using System;
using UnityEngine;
using UnityEngine.Events;

namespace CoreLib
{
    [ExecuteInEditMode]
    public class GlobalValueTracker<TType, TGlobalValue> : MonoBehaviour
        where TGlobalValue : GlobalValue, IGlobalValue<TType>
    {
        [SerializeField]
        private TGlobalValue m_GlobalValue;

        [SerializeField]
        private UnityEvent<TType> m_OnValueChanged;

        public TGlobalValue Value => m_GlobalValue;

        // =======================================================================
        private void OnEnable()
        {
            if (Value == null)
                return;

            Value.OnChanged += _onValueChanged;
            _onValueChanged(Value.Value);
        }

        private void OnDisable()
        {
            if (Value == null)
                return;

            Value.OnChanged -= _onValueChanged;
        }

        private void _onValueChanged(TType value)
        {
            m_OnValueChanged.Invoke(value);
        }
    }
}