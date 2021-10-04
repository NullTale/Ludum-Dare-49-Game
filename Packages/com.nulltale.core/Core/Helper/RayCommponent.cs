using UnityEngine;

namespace CoreLib
{
    public class RayCommponent : MonoBehaviour
    {
        public float                   m_Length;
        public LayerMask               m_LayerMask;
        public QueryTriggerInteraction m_TriggerInteraction = QueryTriggerInteraction.UseGlobal;

        public Ray Ray => new Ray(transform.position, transform.forward);

        //////////////////////////////////////////////////////////////////////////
        private void OnDrawGizmosSelected()
        {
            if (RayCast(out var hitinfo))
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, transform.position + transform.forward * hitinfo.distance);
            }
            else
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, transform.position + transform.forward * m_Length);
            }
        }

        public bool RayCast()
        {
            return Physics.Raycast(Ray, m_Length, m_LayerMask.value, m_TriggerInteraction);
        }

        public bool RayCast(out RaycastHit hitinfo)
        {
            return Physics.Raycast(Ray, out hitinfo, m_Length, m_LayerMask.value, m_TriggerInteraction);
        }
    }
}