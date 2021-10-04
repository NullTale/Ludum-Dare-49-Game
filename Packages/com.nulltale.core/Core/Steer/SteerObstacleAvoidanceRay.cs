using UnityEngine;

namespace CoreLib.Steer
{
    public class SteerObstacleAvoidanceRay : Steering
    {
        public RayCommponent m_Ray;
        public float         m_Force;

        //////////////////////////////////////////////////////////////////////////
        protected override Vector3 _calculateForce()
        {
            if (m_Ray.RayCast(out var hitinfo) && hitinfo.collider.TryGetComponent(out Obstacle obstacle))
            {
                return hitinfo.normal * Mathf.Clamp01(m_Ray.m_Length - hitinfo.distance / m_Ray.m_Length) * m_Force *  obstacle.m_Threatening;
            }

            return Vector3.zero;
        }
    }
}