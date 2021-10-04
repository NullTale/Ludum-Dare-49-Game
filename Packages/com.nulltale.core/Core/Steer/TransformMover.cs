using UnityEngine;

namespace CoreLib.Steer
{
    public class TransformMover : Mover
    {
        public Transform m_Target;

        public void Update()
        {
            m_Target.position += _calculateOffset(Time.deltaTime);

            if (UpdateRotation)
                m_Target.forward = _calculateForward(m_Target.forward, Time.deltaTime);
        }
    }
}