using System;
using NaughtyAttributes;
using UnityEngine;

namespace CoreLib.Sound
{
    public class AudioPlayer : MonoBehaviour, IAudioContext
    {
        [SerializeField]
        private Context                 m_Context;
        [SerializeField]
        [ShowIf(nameof(m_Context), Context.This)]
        private AudioSource             m_AudioSource;
        public AudioSource AudioSource => m_AudioSource;

        // =======================================================================
        [Serializable]
        public enum Context
        {
            Global,
            This
        }

        // =======================================================================
        public void Play(in string audioKey)
        {
            var audio = SoundManager.Instance.GetAudio(in audioKey);
            PlayAudio(audio);
        }

        public void PlayAudio(IAudio audio)
        {
            if (audio == null)
                return;

            switch (m_Context)
            {
                case Context.Global:
                    audio.Play(SoundManager.Instance);
                    break;
                case Context.This:
                    audio.Play(this);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}