using UnityEngine;
using UnityEngine.InputSystem;

namespace CoreLib
{
    public class InputActionContainer : InputActionBase
    {
        [SerializeField]
        private InputAction m_InputAction;

        // =======================================================================
        private void OnEnable()
        {
            m_InputAction.performed += _invoke;
            m_InputAction.Enable();
        }

        private void OnDisable()
        {
            m_InputAction.performed -= _invoke;
            m_InputAction.Disable();
        }

        private void _invoke(InputAction.CallbackContext context)
        {
            m_Action?.Invoke(context);
        }
    }
}