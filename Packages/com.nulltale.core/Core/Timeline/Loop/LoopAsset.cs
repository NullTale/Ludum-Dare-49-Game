using System;
using System.Collections;
using System.Collections.Generic;
using CoreLib.States;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace CoreLib.Timeline
{
    [Serializable]
    [NotKeyable]
    public class LoopAsset : PlayableAsset, ITimelineClipAsset
    {
        public GlobalState                 m_State;
        public LoopBehaviour.LoopCondition m_LoopWhile = LoopBehaviour.LoopCondition.Open;
        
        public ClipCaps clipCaps => ClipCaps.SpeedMultiplier;

        // =======================================================================
        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            var playable  = ScriptPlayable<LoopBehaviour>.Create(graph);
            var behaviour = playable.GetBehaviour();
            behaviour.m_State     = m_State;
            behaviour.m_Director  = go.GetComponent<PlayableDirector>();
            behaviour.m_LoopWhile = m_LoopWhile;

            return playable;
        }
    }
}