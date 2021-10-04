using System;
using System.Collections.Generic;
using System.Reflection;

namespace CoreLib.StateMachine
{
    public class StateDriver : IState
    {
        protected static Dictionary<Type, Dictionary<string, MethodInfo>> s_MethodCache = new Dictionary<Type, Dictionary<string, MethodInfo>>();

        public Type Lable { get; private set; }

        protected Action m_OnEnter;
        protected Action m_OnExit;

        // =======================================================================
        public void OnEnter()
        {
            m_OnEnter?.Invoke();
        }

        public void OnExit()
        {
            m_OnExit?.Invoke();
        }

        public virtual void Setup(object source, Type state)
        {
            Lable = state;

            m_OnEnter  = _createBindAction(nameof(IStateDefault.OnEnter), source, state);
            m_OnExit   = _createBindAction(nameof(IStateDefault.OnExit), source, state);
        }

        protected static Action _createBindAction(string methodName, object source, Type state)
        {
            // get method info from cache, create delegate
            if (s_MethodCache.TryGetValue(state, out var methods) == false)
            {
                methods = new Dictionary<string, MethodInfo>(6);
                s_MethodCache.Add(state, methods);
            }

            if (methods.TryGetValue(methodName, out var methodInfo) == false)
            {
                methodInfo = state.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                methods.Add(methodName, methodInfo);
            }

            return methodInfo != null ? (Action)Delegate.CreateDelegate(typeof(Action), source, methodInfo) : null;
        }
    }
}