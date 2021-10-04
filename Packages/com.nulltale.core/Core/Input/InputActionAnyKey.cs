using System.Linq;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;

namespace CoreLib
{
    public class InputActionAnyKey : InputActionBase
    {
        private void OnEnable()
        {
            InputSystem.onEvent += _anyKeyCheck;
        }

        private void OnDisable()
        {
            InputSystem.onEvent -= _anyKeyCheck;
        }

        private void _anyKeyCheck(InputEventPtr eventPtr, InputDevice device)
        {
            if (!eventPtr.IsA<StateEvent>() && !eventPtr.IsA<DeltaStateEvent>())
                return;

            var controls         = device.allControls;
            var buttonPressPoint = InputSystem.settings.defaultButtonPressPoint;
            foreach (var control in controls.OfType<ButtonControl>())
            {
                if (control.synthetic || control.noisy)
                    continue;
                if (control.ReadValueFromEvent(eventPtr, out var value) && value >= buttonPressPoint)
                {
                    m_Action?.Invoke(default);
                    return;
                }
            }
        }
    }
}