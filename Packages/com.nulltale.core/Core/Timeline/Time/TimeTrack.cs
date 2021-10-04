using System;
using CoreLib.Module;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace CoreLib.Timeline
{
    [TrackColor(0.3337487f, 0.8903498f, 0.9433962f)]
    [TrackClipType(typeof(TimeAsset))]
    public class TimeTrack : TrackAsset
    {
        public TimeControl.TimeHandle.BlendingMode m_Blending  = TimeControl.TimeHandle.BlendingMode.MinOverride;

        // =======================================================================
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var mixerTrack = ScriptPlayable<TimeMixerBehaviour>.Create(graph, inputCount);
            var behaviour = mixerTrack.GetBehaviour();
            behaviour.m_Blending = m_Blending;
            return mixerTrack;
        }
    }
}