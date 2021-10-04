using UnityEngine;

namespace CoreLib.Sound
{
    public interface IAudioContext
    {
        AudioSource AudioSource { get; }
    }

    public interface IAudioChannelContext : IAudioContext
    {
        AnimationCurve Leave { get; }
        AnimationCurve Enter { get; }
    }

    public interface IAudioProvider
    {
        string Key   { get; }
        IAudio Audio { get; }

        void Init();
    }

    public interface IAudio
    {
        AudioClip Clip { get; }

        void Play(IAudioContext context);
    }
}