using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UltEvents;
using UnityEngine;

namespace CoreLib
{
    [Serializable]
    public sealed class CoroutineWrapper : ISerializationCallbackReceiver
    {
        private static Dictionary<string, MethodInfo> s_MethoInfoCache = new Dictionary<string, MethodInfo>();

        [SerializeField]
        private MonoBehaviour		m_Owner;
        [SerializeField]
        private string              m_EnumeratorFunctionName;

        private Coroutine			m_Coroutine;
        private EnumeratorFunction	m_EnumeratorFunction;

        public bool					IsRunning => m_Coroutine != null;
        public bool                 IsInitialized => m_Owner != null;

        public delegate IEnumerator EnumeratorFunction();

        // =======================================================================
        public CoroutineWrapper(EnumeratorFunction func, MonoBehaviour owner = null)
        {
            m_Owner              = owner ? owner : Core.Instance;
            m_EnumeratorFunction = func;
        }

        public bool Start()
        {
            if (m_Coroutine == null)
            {
                m_Coroutine = m_Owner.StartCoroutine(_enumeratorWrapper());
                return true;
            }

            return false;
        }

        public bool Stop()
        {
            if (m_Coroutine != null)
            {
                m_Owner.StopCoroutine(m_Coroutine);
                m_Coroutine = null;
                return true;
            }

            return false;
        }

        public void Restart()
        {
            Stop();
            Start();
        }

        public void OnBeforeSerialize()
        {
        }


        public void OnAfterDeserialize()
        {
            // initialize wrapper if values a set
            if (m_Owner != null && string.IsNullOrEmpty(m_EnumeratorFunctionName) == false)
            {
                var type = m_Owner.GetType();
                var methodPath = $"{type.GUID}{m_EnumeratorFunctionName}";

                // get method info
                if (s_MethoInfoCache.TryGetValue(methodPath, out var method) == false)
                    s_MethoInfoCache.Add(methodPath, method = type.GetMethod(m_EnumeratorFunctionName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));

                m_EnumeratorFunction = (EnumeratorFunction)method.CreateDelegate(typeof(EnumeratorFunction), m_Owner);
            }
        }
	
        // =======================================================================
        private IEnumerator _enumeratorWrapper()
        {
            yield return m_EnumeratorFunction();
            m_Coroutine = null;
        }
    }
}