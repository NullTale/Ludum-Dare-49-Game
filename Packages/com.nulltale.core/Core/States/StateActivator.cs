using System;
using UnityEngine;

namespace CoreLib.States
{
    public class StateActivator : MonoBehaviour
    {
        [SerializeField]
        private GlobalState    m_State;
        private StateHandle    m_StateHandle;

        public GlobalState State
        {
            get => m_StateHandle.Group;
            set
            {
                m_State = value;
                m_StateHandle.Group = value;
                m_StateHandle.IsOpen = isActiveAndEnabled;
            }
        }

        // =======================================================================
        private void Awake()
        {
            m_StateHandle = new StateHandle(m_State);
        }

        protected void OnEnable()
        {
            m_StateHandle.Open();
        }

        private void OnDisable()
        {
            m_StateHandle.Close();
        }
    }
}