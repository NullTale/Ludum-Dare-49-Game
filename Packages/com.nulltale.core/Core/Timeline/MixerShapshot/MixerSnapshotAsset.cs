using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Playables;


namespace CoreLib.Timeline
{
    [Serializable]
    public class MixerSnapshotAsset : PlayableAsset
    {
        [InplaceField(nameof(MixerSnapshotBehaviour.Snapshot), nameof(MixerSnapshotBehaviour.Weight))]
        public MixerSnapshotBehaviour m_Template;

        // =======================================================================
        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            var playable = ScriptPlayable<MixerSnapshotBehaviour>.Create(graph, m_Template);
            return playable;
        }
    }
}