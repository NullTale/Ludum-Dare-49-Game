using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEventBus;

namespace CoreLib
{
    public interface IUIMediator
    {
        void Add(MonoBehaviour module);
        void Remove(MonoBehaviour module);
    }

    [DefaultExecutionOrder(-1)]
    public abstract class UIMediator : EventBusBase, IUIMediator
    {
        private List<MonoBehaviour>                m_Modules = new List<MonoBehaviour>();
        public  IReadOnlyCollection<MonoBehaviour> Modules => m_Modules;

        // =======================================================================
        public void Add(MonoBehaviour module)
        {
            m_Modules.Add(module);
        }

        public void Remove(MonoBehaviour module)
        {
            m_Modules.Remove(module);
        }

    }

    public interface IUIMediatorModule<out TMediator> where TMediator : IUIMediator
    {
        public TMediator    Mediator { get; }
    }
    
    public abstract class UIMediatorModule<TMediator> : MonoBehaviour, IUIMediatorModule<TMediator>
        where TMediator : IUIMediator
    {
        public TMediator    Mediator { get; private set; }

        // =======================================================================
        protected virtual void Awake()
        {
            Mediator = GetComponentInParent<TMediator>();
        }

        protected virtual void OnEnable()
        {
            Mediator?.Add(this);
        }

        protected virtual void OnDisable()
        {
            Mediator?.Remove(this);
        }
    }

}