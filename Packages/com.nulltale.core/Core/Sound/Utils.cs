using UnityEngine;

namespace CoreLib.Sound
{
    public static class Utils
    {
        private static AudioSound s_PlaySoundAudio = new AudioSound();

        // =======================================================================
        private class AudioSound : IAudio
        {
            public AudioClip Clip   { get; set; }
            public float     Volume { get; set; }

            public void Play(IAudioContext context)
            {
                context.AudioSource.PlayOneShot(Clip, Volume);
            }
        }

        // =======================================================================
        public static void Play(this AudioPlayer player, in string audioKey, float volume)
        {
            var audio = SoundManager.Instance.GetAudio(in audioKey);
            if (audio == null)
                return;

            s_PlaySoundAudio.Clip   = audio.Clip;
            s_PlaySoundAudio.Volume = volume;

            player.PlayAudio(s_PlaySoundAudio);
        }

        public static AudioDynamic PlayDynamic(this AudioPlayer player, in string audioKey)
        {
            var provider = SoundManager.Instance.GetAudioProvider(in audioKey);
            if (provider == null)
                return null;

            return new AudioDynamic(provider, player);
        }

        public static void Play(this AudioPlayer player, IAudio audio)
        {
            player.PlayAudio(audio);
        }
    }
}