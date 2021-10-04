using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Playables;

namespace CoreLib.Timeline
{
    [Serializable]
    public class TimeAsset : PlayableAsset
    {
        [InplaceField(nameof(TimeBehaviour.m_TimeScale))]
        public TimeBehaviour m_Template;

        // =======================================================================
        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            var playable  = ScriptPlayable<TimeBehaviour>.Create(graph, m_Template);
            var behaviour = playable.GetBehaviour();

            return playable;
        }
    }
}