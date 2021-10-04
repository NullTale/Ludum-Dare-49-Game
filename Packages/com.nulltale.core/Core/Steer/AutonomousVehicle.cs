using UnityEngine;

namespace CoreLib.Steer
{
    public class AutonomousVehicle : TickedVehicle
    {
        [SerializeField]
        private float m_AccelerationRate = 5.0f;

        [SerializeField] 
        private float m_DecelerationRate = 8.0f;

        private float m_Speed;

        public override float Speed => m_Speed;

        public override Vector3 Velocity => transform.forward * Speed;

        //////////////////////////////////////////////////////////////////////////
        protected override void _updateVelocity(Vector3 velocity)
        {
            TargetSpeed = velocity.magnitude;
            if (m_Speed.IsAproximatlyZero() == false)
                Orientation = velocity.normalized;
        }

        protected override Vector3 _calculateOffset(float deltaTime)
        {
            var targetSpeed = Mathf.Clamp(TargetSpeed, 0, MaxSpeed);
            if (m_Speed.Aproximatly(targetSpeed))
                m_Speed = targetSpeed;
            else
            {
                var rate = TargetSpeed > m_Speed ? m_AccelerationRate : m_DecelerationRate;
                m_Speed = Mathf.Lerp(m_Speed, targetSpeed, deltaTime * rate);
            }

            return Velocity * deltaTime;
        }

        protected override void _zeroVelocity()
        {
            TargetSpeed = 0;
        }
    }
}