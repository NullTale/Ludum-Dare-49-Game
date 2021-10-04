using UnityEngine;
using UnityEngine.InputSystem;

namespace CoreLib
{
    public class InputActionLink : InputActionBase
    {
        [SerializeField]
        private InputActionReference m_ActionReference;

        //////////////////////////////////////////////////////////////////////////
        private void OnEnable()
        {
            m_ActionReference.action.performed += _invoke;
        }

        private void OnDisable()
        {
            m_ActionReference.action.performed -= _invoke;
        }

        private void _invoke(InputAction.CallbackContext context)
        {
            m_Action?.Invoke(context);
        }
    }
}