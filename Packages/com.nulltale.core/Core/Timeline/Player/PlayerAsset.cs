using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace CoreLib.Timeline
{
    public class PlayerAsset : PlayableAsset, ITimelineClipAsset
    {
        public ClipCaps clipCaps => ClipCaps.None;

        // =======================================================================
        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            var playable = ScriptPlayable<PlayerBehaviour>.Create(graph);
            return playable;
        }
    }
}