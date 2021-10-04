using UnityEngine;

namespace CoreLib.Steer
{
    public class SimpleVehicle : TickedVehicle
    {
        private         Vector3 m_Velocity;
        private         float   m_Speed;
        public override float   Speed    => m_Speed;
        public override Vector3 Velocity => m_Velocity;

        //////////////////////////////////////////////////////////////////////////
        protected override void _updateVelocity(Vector3 velocity)
        {
            _setVelocity(velocity);
        }

        protected override Vector3 _calculateOffset(float deltaTime)
        {
            return Velocity * deltaTime;
        }

        protected override void _zeroVelocity()
        {
            _setVelocity(Vector3.zero);
        }
        
        protected void _setVelocity(Vector3 velocity)
        {
            m_Velocity  = Vector3.ClampMagnitude(velocity, MaxSpeed);
            m_Speed     = m_Velocity.magnitude;
            TargetSpeed = m_Speed;
            if (m_Speed.IsAproximatlyZero() == false)
                Orientation = m_Velocity.normalized;
        }
    }
}