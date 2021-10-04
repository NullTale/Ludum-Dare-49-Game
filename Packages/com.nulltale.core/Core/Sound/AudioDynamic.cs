using System;
using UnityEngine;

namespace CoreLib.Sound
{
    [Serializable]
    public class AudioDynamic : IAudio, IDisposable
    {
        [SerializeField]
        private AudioProvider            m_Provider;
        private IAudio                   m_Audio;
        private AudioPlayable.PlayHandle m_PlayHandle;

        public  AudioClip Clip => m_Audio.Clip;

        [SerializeField]
        private AudioPlayer m_Context;

        private bool IsPlaying;

        public float Volume
        {
            get => m_PlayHandle.Volume;
            set
            {
                switch (IsPlaying)
                {
                    // turn on or off playing
                    case false when value > 0f:
                        IsPlaying = true;
                        m_Audio             = m_Provider.Audio;
                        m_PlayHandle.Clip   = m_Audio.Clip;
                        m_PlayHandle.Play = true;
                        break;
                    case true when value <= 0f:
                        IsPlaying = false;
                        m_PlayHandle.Play = false;
                        break;
                }

                m_PlayHandle.Volume = value;
            }
        }

        // =======================================================================
        public AudioDynamic() { }

        public AudioDynamic(AudioProvider provider, AudioPlayer context)
        {
            m_Provider      = provider;
            m_Context       = context;

            if (context.AudioSource.TryGetComponent<AudioPlayable>(out var audioPlayable) == false)
                audioPlayable = context.AudioSource.gameObject.AddComponent<AudioPlayable>();
            m_PlayHandle    = audioPlayable.CreatePlayHandle();
        }

        public void Play(IAudioContext context)
        {
            m_Context       = (AudioPlayer)context;

            m_PlayHandle.Dispose();
            if (context.AudioSource.TryGetComponent<AudioPlayable>(out var audioPlayable) == false)
                audioPlayable = context.AudioSource.gameObject.AddComponent<AudioPlayable>();
            m_PlayHandle    = audioPlayable.CreatePlayHandle();
        }

        public void Dispose()
        {
            m_PlayHandle.Dispose();
        }
    }
}