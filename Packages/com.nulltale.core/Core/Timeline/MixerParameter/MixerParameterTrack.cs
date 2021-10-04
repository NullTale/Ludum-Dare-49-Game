using CoreLib.Sound;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace CoreLib.Timeline
{
    [TrackColor(1.0f, 0.85f, 0.1f)]
    [TrackClipType(typeof(MixerParameterAsset))]
    [TrackBindingType(typeof(MixerExposedParameter))]
    public class MixerParameterTrack : TrackAsset
    {
        // =======================================================================+
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var mixerTrack = ScriptPlayable<MixerParameterMixerBehaviour>.Create(graph, inputCount);
            return mixerTrack;
        }
    }
}