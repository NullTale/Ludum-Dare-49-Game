using System.Linq;
using UnityEngine;

namespace CoreLib.Steer
{
    public class SteerObstacleAvoidanceRadius : Steering
    {
        public          LayerMask m_PhysicsMask;
        public          float     m_Force;
        public          float     m_Radius;

        protected override Vector3 _calculateForce()
        {
            return Physics.OverlapSphere(transform.position, m_Radius, m_PhysicsMask.value, QueryTriggerInteraction.Ignore)
                   .Select(n => n.GetComponent<Obstacle>())
                   .Where(n => n != null)
                   .Aggregate(Vector3.zero, (vector3, obstacle) =>
                   {
                       var toObstacle = obstacle.transform.position - transform.position;
                       return vector3 - toObstacle.normalized * Mathf.Clamp01((m_Radius - toObstacle.magnitude) / m_Radius) * m_Force * obstacle.m_Threatening;
                   });
        }
    }
}