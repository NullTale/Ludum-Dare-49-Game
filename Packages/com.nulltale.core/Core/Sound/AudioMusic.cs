using UnityEngine;

namespace CoreLib.Sound
{
    public class AudioMusic : AudioProvider, IAudio
    {
        public override IAudio    Audio => this;
        public          AudioClip Clip  => m_Clip;

        [Range(0, 1)]
        public float     m_Volume = 1;
        public AudioClip m_Clip;

        public Optional<ParticleSystem.MinMaxCurve> m_Time;
        public Optional<AnimationCurve>             m_Enter;

        // =======================================================================
        public void Play(IAudioContext context)
        {
            var time = m_Time.Enabled ? m_Time.Value.Evaluate() : 0f;

            switch (context)
            {
                case SoundManager sm:
                {
                    var source = sm.Music.AudioSource;
                    if (source.clip == m_Clip)
                        break;
                    sm.StartCoroutine(SoundManager.TransitionCoroutine(source, m_Clip, m_Volume, sm.Music.Leave, m_Enter ? m_Enter.Value : sm.Music.Enter, time));
                } break;
                case IAudioChannelContext channel:
                {
                    var source = channel.AudioSource;
                    if (source.clip == m_Clip)
                        break;
                    ((MonoBehaviour)channel).StartCoroutine(SoundManager.TransitionCoroutine(source, m_Clip, m_Volume, channel.Leave, m_Enter ? m_Enter.Value : channel.Enter, time));
                } break;
                default:
                {
                    var source = context.AudioSource;
                    if (source.clip == m_Clip)
                        break;

                    source.clip   = m_Clip;
                    source.volume = m_Volume;
                    source.time   = time;
                    source.Play();
                } break;
            }
        }
    }
}