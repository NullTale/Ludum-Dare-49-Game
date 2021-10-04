using UnityEngine;

namespace CoreLib.Sound
{
    [RequireComponent(typeof(AudioSource))]
    public sealed class SoundChannel : MonoBehaviour, IAudioContext
    {
        private AudioSource m_AudioSource;
        public  AudioSource AudioSource => m_AudioSource;

        // =======================================================================
        private void Awake()
        {
            m_AudioSource = GetComponent<AudioSource>();
        }
    }
}