using UnityEngine;

namespace CoreLib.Steer
{
    public class CharacterControllerMover : Mover
    {
        public CharacterController m_Target;

        private void Update()
        {
            if (UpdateRotation)
                m_Target.transform.forward = _calculateForward(m_Target.transform.forward, Time.deltaTime);;
        }

        public void FixedUpdate()
        {
            m_Target.Move(_calculateOffset(Time.fixedDeltaTime));
        }
    }
}