using System.Collections;
using UnityEngine;

namespace CoreLib
{
    public sealed class TinyCoroutine
    {
        private MonoBehaviour m_Owner;
        private Coroutine     m_Coroutine;

        public bool IsRunning => m_Coroutine != null;

        public MonoBehaviour Owner
        {
            get => m_Owner;
            set
            {
                if (m_Owner == value)
                    return;

                Stop();
                m_Owner = value;
            }
        }

        // =======================================================================
        public TinyCoroutine(MonoBehaviour owner)
        {
            m_Owner = owner;
        }

        public TinyCoroutine()
        {
            m_Owner = Core.Instance;
        }

        public void Start(IEnumerator coroutine)
        {
            // start new or cancel current if value is null
            if (m_Coroutine != null)
                Stop();

            if (coroutine == null)
                return;

            m_Coroutine = m_Owner.StartCoroutine(_enumeratorWrapper(coroutine));
        }

        public void Stop()
        {
            if (m_Coroutine == null)
                return;

            m_Owner.StopCoroutine(m_Coroutine);
            m_Coroutine = null;
        }

        // =======================================================================
        private IEnumerator _enumeratorWrapper(IEnumerator enumerator)
        {
            yield return enumerator;
            m_Coroutine = null;
        }
    }
}