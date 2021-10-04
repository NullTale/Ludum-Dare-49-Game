using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Playables;
using UnityEngine.Timeline;


namespace CoreLib.Timeline
{
    [TrackColor(1.0f, 0.85f, 0.1f)]
    [TrackClipType(typeof(MixerSnapshotAsset))]
    //[TrackBindingType(typeof(AudioMixer))]
    public class MixerSnapshotTrack : TrackAsset
    {
        // =======================================================================
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var mixerTrack = ScriptPlayable<MixerSnapshotMixerBehaviour>.Create(graph, inputCount);
            return mixerTrack;
        }
    }
}