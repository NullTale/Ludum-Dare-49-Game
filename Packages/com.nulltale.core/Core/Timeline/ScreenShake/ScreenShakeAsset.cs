using System;
using UnityEngine;
using UnityEngine.Playables;

namespace CoreLib.Timeline
{
    [Serializable]
    public class ScreenShakeAsset : PlayableAsset
    {
        [InplaceField(nameof(ScreenShakeBehaviour.m_Amplitude), nameof(ScreenShakeBehaviour.m_Frequency))]
        public ScreenShakeBehaviour m_Template;

        // =======================================================================
        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            var playable = ScriptPlayable<ScreenShakeBehaviour>.Create(graph, m_Template);
            var behaviour = playable.GetBehaviour();

            return playable;
        }
    }
}