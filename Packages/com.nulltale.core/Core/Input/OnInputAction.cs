using UnityEngine;
using UnityEngine.Events;

namespace CoreLib
{
    [RequireComponent(typeof(InputActionBase))]
    public class OnInputAction : MonoBehaviour
    {
        public UnityEvent   m_Action;

        private void Awake()
        {
            GetComponent<InputActionBase>().Action = m_Action.Invoke;
        }
    }
}