using UnityEngine;

namespace CoreLib.Steer
{
    public class SteerTarget : Steering
    {
        public Transform m_Target;
        public float     m_Force;

        protected override Vector3 _calculateForce()
        {
            var toTarget = m_Target.position - m_Vehicle.transform.position;
            return toTarget.sqrMagnitude <= m_Vehicle.SquaredArrivalRadius
                ? Vector3.zero
                : toTarget.normalized * m_Force;
        }
    }
}