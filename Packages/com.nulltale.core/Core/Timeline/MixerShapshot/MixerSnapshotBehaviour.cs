using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace CoreLib.Timeline
{
    [Serializable]
    public class MixerSnapshotBehaviour : PlayableBehaviour
    {
        public AudioMixerSnapshot                   Snapshot;
        [Range(0.0f, 1.0f)]
        public float                                Weight = 1.0f;
    }
}