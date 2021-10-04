using UnityEngine;
using UnityEngine.Events;

namespace CoreLib.Effect
{
    public class ActionModule : ModuleBase
    {
        [SerializeField]
        private UnityEvent            m_OnBegin;
        
        [SerializeField]
        private UnityEvent            m_OnEnd;

        // =======================================================================
        public override void Begin()
        {
            m_OnBegin.Invoke();
        }

        public override void End()
        {
            m_OnEnd.Invoke();
        }
    }
}