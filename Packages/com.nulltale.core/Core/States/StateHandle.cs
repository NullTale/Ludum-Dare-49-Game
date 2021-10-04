using System;
using NaughtyAttributes;
using UnityEngine;
using CoreLib.Module;

namespace CoreLib.States
{
    public interface IStateHandle
    {
        void Open();
        void Close();
    }

    [Serializable]
    public sealed class StateHandle : IStateHandle, IDisposable
    {
        [SerializeField] [Expandable]
        private GlobalState   m_State;
        public GlobalState Group
        {
            get => m_State;
            set
            {
                var wasOpen = IsOpen;
                Close();
                m_State = value;

                if (wasOpen)
                    Open();

            }
        }

        private bool m_Open;

        public bool IsOpen
        {
            get => m_Open;
            set
            {
                if (m_Open == value)
                    return;

                if (value)
                    Open();
                else 
                    Close();
            }
        }

        // =======================================================================
        public StateHandle(GlobalState state)
        {
            m_State = state;
        }

        public void Open()
        {
            if (m_State.IsNull())
                return;

            if (m_Open)
                return;

            m_Open = true;

            m_State.Open();
        }

        public void Close()
        {
            if (m_State.IsNull())
                return;

            if (m_Open == false)
                return;

            m_Open = false;

            m_State.Close();
        }

        public void Dispose()
        {
            Close();
        }
    }
}