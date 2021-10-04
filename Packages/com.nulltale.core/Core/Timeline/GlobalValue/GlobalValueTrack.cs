using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace CoreLib.Timeline
{
    [TrackColor(0.28f, 0.28f, 1f)]
    [TrackClipType(typeof(ValueAsset))]
    [TrackBindingType(typeof(GlobalValue))]
    public class GlobalValueTrack : TrackAsset
    {
        // =======================================================================
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var mixerTrack = ScriptPlayable<ValueMixerBehaviour>.Create(graph, inputCount);
            var behaviour  = mixerTrack.GetBehaviour();
            return mixerTrack;
        }
    }
}