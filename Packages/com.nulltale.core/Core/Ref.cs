using System;
using UnityEngine;

namespace CoreLib
{
    public interface IRef
    {
        bool HasValue { get; }
    }

    public interface IRefGet<out T> : IRef
    {
        T Value { get; }
    }
    
    public interface IRefSet<in T> : IRef
    {
        T Value { set; }
    }

    [Serializable]
    public class Ref<T> : IRefSet<T>, IRefGet<T>
    {
        [SerializeField]
        private T m_Value;

        public T Value
        {
            get => m_Value;
            set => m_Value = value;
        }

        public bool HasValue => Equals(m_Value, default);

        // =======================================================================
        public Ref() { }

        public Ref(T value)
        {
            Value = value;
        }

        public Ref<T> With(T value)
        {
            m_Value = value;
            return this;
        }

        public static implicit operator T(Ref<T> r)
        {
            return r.m_Value;
        }

        public static implicit operator Ref<T>(T v)
        {
            return new Ref<T>(v);
        }
    }
    
    public class RefGeneric<T> : IRefGet<T>,  IRefSet<T>
    {
        private Action<T> m_Set;
        private Func<T>   m_Get;

        public bool HasValue => Equals(Value, default);
        public T    Value    { get => m_Get.Invoke(); set => m_Set.Invoke(value); }

        public RefGeneric(Action<T> set, Func<T> get)
        {
            m_Set = set;
            m_Get = get;
        }
    }
}