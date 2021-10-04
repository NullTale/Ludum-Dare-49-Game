using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace CoreLib.Timeline
{
    [TrackColor(0.8f, 0.2f, 0.2f)]
    [TrackClipType(typeof(ScreenShakeAsset))]
    public class ScreenShakeTrack : TrackAsset
    {
        // =======================================================================
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var mixerTrack = ScriptPlayable<ScreenShakeMixerBehaviour>.Create(graph, inputCount);
            return mixerTrack;
        }
    }
}