using System.Collections;
using NaughtyAttributes;
using UnityEngine;

namespace CoreLib.States
{
    public class State : GlobalState
    {
        [SerializeField]
        private int m_CloseDelayFrames = 4;

        [SerializeField] [ReadOnly]
        private int m_OpenCounter;

        private Coroutine m_CloseCoroutine;

        [SerializeField] [ReadOnly]
        private bool    m_IsOpen;
        public override bool IsOpen => m_IsOpen;

        // =======================================================================
        public override void Open()
        {
            if (++ m_OpenCounter == 1)
            {
                if (m_CloseCoroutine != null)
                    Core.Instance.StopCoroutine(m_CloseCoroutine);

                if (m_IsOpen == false)
                {
                    m_IsOpen = true;
                    InvokeOnOpen();
                }
            }
        }

        public override void Close()
        {
            if (-- m_OpenCounter == 0)
                m_CloseCoroutine = Core.Instance.StartCoroutine(_close(m_CloseDelayFrames));

            // ===================================
            IEnumerator _close(int frames)
            {
                while (frames -- > 0)
                    yield return null;

                m_IsOpen = false;
                m_CloseCoroutine = null;
                InvokeOnClose();
            }
                
        }

        internal override void Init()
        {
            m_OpenCounter = 0;
            m_IsOpen = false;
            m_CloseCoroutine = null;
        }
    }
}