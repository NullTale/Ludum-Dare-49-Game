using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace CoreLib
{
    public class InputActionEventSystem : InputActionBase
    {
        [SerializeField]
        private ActionType m_Type;
        public ActionType Type => m_Type;

        //////////////////////////////////////////////////////////////////////////
        [Serializable]
        public enum ActionType
        {
            Cancel,
            Submit,
            Click,
        }

        //////////////////////////////////////////////////////////////////////////
        private void OnEnable()
        {
            var action = getInputAction();
            if (action != null)
                action.performed += _invoke;
        }

        private void OnDisable()
        {
            var action = getInputAction();
            if (action != null)
                action.performed -= _invoke;
        }

        private InputAction getInputAction()
        {
            var inputModule = UnityEngine.EventSystems.EventSystem.current?.GetComponent<InputSystemUIInputModule>();

            if (inputModule == null)
                return null;

            var action = Type switch
            {
                ActionType.Cancel => inputModule.cancel.action,
                ActionType.Submit => inputModule.submit.action,
                ActionType.Click  => inputModule.leftClick.action,
                _                 => throw new ArgumentOutOfRangeException()
            };
            return action;
        }

        private void _invoke(InputAction.CallbackContext context)
        {
            m_Action?.Invoke(context);
        }
    }
}