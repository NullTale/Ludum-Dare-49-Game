using UnityEngine;
using UnityEngine.Timeline;

namespace CoreLib
{
    [RequireComponent(typeof(SignalReceiver))]
    public class SignalReceiverAdapter : MonoBehaviour
    {
        private SignalReceiver m_SignalReceiver;

        // =======================================================================
        private void Awake()
        {
            m_SignalReceiver = GetComponent<SignalReceiver>();
        }

        public void InvokeSignal(SignalAsset signal)
        {
            m_SignalReceiver.GetReaction(signal)?.Invoke();
        }
    }
}