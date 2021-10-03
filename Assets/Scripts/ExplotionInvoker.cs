using System.Linq;
using NaughtyAttributes;
using UnityEngine;

namespace Game
{
    public class ExplotionInvoker : MonoBehaviour
    {
        public float m_Force;
        public float m_Radius;
        public float m_Upward;

        // =======================================================================
        [Button]
        public void Explotion()
        {
            foreach (var pig in Physics.OverlapSphere(transform.position, m_Radius, 1 << GamePreferences.Instance.PigLayer, QueryTriggerInteraction.Ignore))
            {
                pig.attachedRigidbody.AddExplosionForce(m_Force, transform.position, m_Radius, m_Upward, ForceMode.Impulse);
            }
        }
    }
}