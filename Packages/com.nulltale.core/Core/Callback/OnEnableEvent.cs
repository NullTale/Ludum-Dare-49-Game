using UnityEngine.Events;

namespace CoreLib
{
    public sealed class OnEnableEvent : CallbackBase
    {
        public UnityEvent Event;

        // =======================================================================
        private void OnEnable()
        {
            Event.Invoke();
        }
    }
}