using UnityEngine;

namespace CoreLib.Sound
{
    public class AudioSound : AudioProvider, IAudio
    {
        public override IAudio    Audio => this;
        public          AudioClip Clip  => m_Clip;

        [Range(0, 1)]
        public float     m_Volume = 1;
        public AudioClip m_Clip;

        // =======================================================================
        public void Play(IAudioContext context)
        {
            context.AudioSource.PlayOneShot(m_Clip, m_Volume);
        }
    }
}