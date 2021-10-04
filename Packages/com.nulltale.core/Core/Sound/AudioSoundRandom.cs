using NaughtyAttributes;
using UnityEngine;

namespace CoreLib.Sound
{
    public class AudioSoundRandom : AudioProvider, IAudio
    {
        public override IAudio    Audio => this;
        public          AudioClip Clip  => m_Clips.RandomItem();

        [Range(0, 1)]
        public float m_Volume = 1;
        public AudioClip[] m_Clips;

        // =======================================================================
        public void Play(IAudioContext context)
        {
            context.AudioSource.PlayOneShot(m_Clips.RandomItem(), m_Volume);
        }
    }
}