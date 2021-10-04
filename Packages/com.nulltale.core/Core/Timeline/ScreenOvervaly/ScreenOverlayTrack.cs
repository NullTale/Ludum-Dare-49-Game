using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace CoreLib.Timeline
{
    [TrackColor(0.09803922f, 0.09803922f, 0.09803922f)]
    [TrackClipType(typeof(ScreenOverlayAsset))]
    public class ScreenOverlayTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var mixerTrack = ScriptPlayable<ScreenOverlayMixerBehaviour>.Create(graph, inputCount);
            return mixerTrack;
        }
    }
}