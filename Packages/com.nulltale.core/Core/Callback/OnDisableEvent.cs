using UnityEngine;
using UnityEngine.Events;

namespace CoreLib
{
    public sealed class OnDisableEvent : MonoBehaviour
    {
        public UnityEvent Event;

        // =======================================================================
        private void OnDisable()
        {
            Event.Invoke();
        }
    }
}