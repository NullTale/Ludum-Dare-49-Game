using UnityEngine;

namespace CoreLib.States
{
    public class WaitStateYieldInstruction : CustomYieldInstruction
    {
        private GlobalState m_State;
        private bool          m_WaitUntil;

        public override bool keepWaiting => m_State.IsOpen == m_WaitUntil;

        public WaitStateYieldInstruction(GlobalState state, bool waitForClosing)
        {
            m_State = state;
            m_WaitUntil = waitForClosing;
        }
    }
}