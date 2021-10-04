using System;
using UnityEngine;

namespace CoreLib
{
    [Serializable]
    public sealed class GlobalValueLink<T> : IRefGet<T>, IRefSet<T>
    {
        [SerializeField]
        private bool m_Override;
        [SerializeField]
        private GlobalValue m_GlobalValue;
        [SerializeField]
        private T m_GlobalValueOverride;
        public bool HasValue => true;

        public T Value
        {
            get => m_Override ? m_GlobalValueOverride : ((GlobalValue<T>)m_GlobalValue).Value;
            set
            {
                if (m_Override)
                    m_GlobalValueOverride = value;
                else
                    ((GlobalValue<T>)m_GlobalValue).Value = value;
            }
        }
    }
}