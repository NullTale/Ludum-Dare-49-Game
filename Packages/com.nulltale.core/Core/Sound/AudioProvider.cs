using UnityEngine;

namespace CoreLib.Sound
{
    public abstract class AudioProvider : ScriptableObject, IAudioProvider
    {
        public          string Key   => name;
        public abstract IAudio Audio { get; }

        public virtual void Init() {}
    }
}