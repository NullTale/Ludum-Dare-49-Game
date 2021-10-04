using UnityEngine;

namespace CoreLib.Steer
{
    public class RigidBodyMover : Mover
    {
        public Rigidbody m_Target;

        public void FixedUpdate()
        {
            var forward = _calculateForward(m_Target.transform.forward, Time.fixedDeltaTime);
            
            m_Target.MovePosition(m_Target.position + _calculateOffset(Time.fixedDeltaTime));
            m_Target.MoveRotation(Quaternion.LookRotation(forward, m_Target.transform.up));
        }
    }
}