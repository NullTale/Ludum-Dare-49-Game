using System;
using System.Collections;
using UnityEngine;

namespace CoreLib
{
    /// <summary>
    /// helper class which calls update function at the start on the next frame
    /// </summary>
    public class LateUpdate
    {
        public event Action Update;
        private bool        m_Require;

        private Coroutine m_Coroutine;

        // =======================================================================
        public void Require()
        {
            if (m_Require)
                return;

            m_Require = true;
            m_Coroutine = Core.Instance.StartCoroutine(_lateUpdate());

            // ===================================
            IEnumerator _lateUpdate()
            {
                yield return Core.k_WaitForEndOfFrame;

                Update?.Invoke();

                m_Coroutine = null;
                m_Require   = false;
            }
        }

        public LateUpdate() { }
        public LateUpdate(Action action, bool require = false)
        {
            Update  = action;
            if (require)
                Require();
        }
    }
}