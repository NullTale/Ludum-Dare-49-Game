using System;
using UnityEngine;

namespace CoreLib.Sound
{
    [RequireComponent(typeof(AudioPlayer))]
    public sealed class AudioPlayerInvoker : MonoBehaviour
    {
        private AudioPlayer m_Player;
        [SerializeField] [AudioKey]
        private string m_AudioKey;

        public Optional<float>  m_Volume;

        public string AudioKey
        {
            get => m_AudioKey;
            set => m_AudioKey = value;
        }

        // =======================================================================
        private void Awake()
        {
            m_Player = GetComponent<AudioPlayer>();
        }

        public void Invoke()
        {
            if (m_Volume)
                m_Player.Play(m_AudioKey, m_Volume);
            else
                m_Player.Play(m_AudioKey);
        }

    }
}