using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CoreLib
{
    public abstract class InputActionBase : MonoBehaviour
    {
        protected Action<InputAction.CallbackContext> m_Action;
        public Action<InputAction.CallbackContext> ActionCallback
        {
            set => m_Action = value;
        }
        public Action Action
        {
            set => m_Action = c => value?.Invoke();
        }
    }
}