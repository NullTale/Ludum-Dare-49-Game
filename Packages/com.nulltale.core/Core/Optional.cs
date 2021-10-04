using System;
using UnityEngine;

namespace CoreLib
{
    [Serializable]
    public sealed class Optional<T> : IRefGet<T>, IRefSet<T>
    {
        [SerializeField]
        private bool enabled;

        [SerializeField]
        private T value;
    
        public bool Enabled
        {
            get => enabled;
            set => enabled = value;
        }

        public T    Value
        {
            get => value;
            set => this.value = value;
        }
        
        public bool HasValue => Enabled;

        // =======================================================================
        public Optional(bool enabled)
        {
            this.enabled = enabled;
        }

        public Optional(T value, bool enabled)
        {
            this.enabled = enabled;
            this.value   = value;
        }

        public T GetValue(T defaultValue)
        {
            return enabled ? value : defaultValue;
        }

        public static implicit operator bool(Optional<T> opt)
        {
            return opt.enabled;
        }

        public static implicit operator T(Optional<T> opt)
        {
            return opt.value;
        }
    }
}