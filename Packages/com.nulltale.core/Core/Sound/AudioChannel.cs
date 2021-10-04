using UnityEngine;

namespace CoreLib.Sound
{
    [RequireComponent(typeof(AudioSource))]
    public sealed class AudioChannel : MonoBehaviour, IAudioChannelContext
    {
        [SerializeField]
        private AnimationCurve m_Leave = new AnimationCurve(new Keyframe(0.0f, 1.0f), new Keyframe(1.2f, 0.0f));
        [SerializeField]
        private AnimationCurve m_Enter = new AnimationCurve(new Keyframe(0.0f, 0.0f), new Keyframe(2.0f, 1.0f));

        public AnimationCurve Leave => m_Leave;
        public AnimationCurve Enter => m_Enter;

        private AudioSource m_AudioSource;
        public  AudioSource AudioSource => m_AudioSource;

        // =======================================================================
        private void Awake()
        {
            m_AudioSource = GetComponent<AudioSource>();
        }
    }
}