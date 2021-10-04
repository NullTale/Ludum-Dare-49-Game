using System;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;

namespace CoreLib.Builder
{
    public class BuilderRoot : BranchPass
    {
        [SerializeField]
        private RunEvent m_RunEvent;

        //////////////////////////////////////////////////////////////////////////
        [Serializable]
        public enum RunEvent
        {
            None,

            OnAwake,
            OnStart,
            OnLate
        }

        //////////////////////////////////////////////////////////////////////////
        private void Awake()
        {
            if (m_RunEvent == RunEvent.OnAwake)
                Run();
        }

        private IEnumerator Start()
        {
            if (m_RunEvent == RunEvent.OnStart)
            {
                Run();
                yield break;
            }
            if (m_RunEvent == RunEvent.OnLate)
            {
                yield return Core.k_WaitForEndOfFrame;
                Run();
            }
        }

        [Button]
        public void Run()
        {
            StartCoroutine(Build());
        }

        public override IEnumerator Build()
        {
            yield return base.Build();
        }
    }
}