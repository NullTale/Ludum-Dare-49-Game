using UnityEngine;

namespace CoreLib.Sound
{
    public class AudioAmbient : AudioProvider, IAudio
    {
        public override IAudio    Audio => this;
        public          AudioClip Clip  => m_Clip;

        [Range(0, 1)]
        public float     m_Volume = 1;
        public AudioClip m_Clip;

        public Optional<ParticleSystem.MinMaxCurve> m_Time;

        // =======================================================================
        public void Play(IAudioContext context)
        {
            var time = m_Time.Enabled ? m_Time.Value.Evaluate() : 0f;

            switch (context)
            {
                case SoundManager sm:
                    sm.StartCoroutine(SoundManager.TransitionCoroutine(sm.Ambient.AudioSource, m_Clip, m_Volume, sm.Ambient.Leave, sm.Ambient.Enter, time));
                    break;
                case IAudioChannelContext channel:
                    ((MonoBehaviour)channel).StartCoroutine(SoundManager.TransitionCoroutine(channel.AudioSource, m_Clip, m_Volume, channel.Leave, channel.Enter, time));
                    break;
                default:
                    var audioSource = context.AudioSource;
                    audioSource.clip   = m_Clip;
                    audioSource.volume = m_Volume;
                    audioSource.time   = time;
                    audioSource.Play();
                    break;
            }
        }
    }
}