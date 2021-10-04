using UnityEngine;
using UnityEngine.Events;

namespace CoreLib
{
    public class UnityEventComponent : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent m_Action;

        // =======================================================================
        public void Invoke()
        {
            m_Action?.Invoke();
        }

        public void InvokeUnityAction()
        {
            m_Action?.Invoke();
        }
    }
}