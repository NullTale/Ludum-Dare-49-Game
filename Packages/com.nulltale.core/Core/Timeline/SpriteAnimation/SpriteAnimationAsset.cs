using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace CoreLib.Timeline
{
    public class SpriteAnimationAsset : PlayableAsset, ITimelineClipAsset
    {
        public SpriteAnimation.SpriteAnimationAsset m_Animation;
        public ColorMode                            m_Color;
        public bool                                 m_Loop;
        public bool                                 m_AutoScale;
        public ClipCaps                             clipCaps
        {
            get
            {
                var result = ClipCaps.ClipIn | ClipCaps.SpeedMultiplier;

                if (m_Loop)
                    result |= ClipCaps.Looping;

                if (m_AutoScale)
                    result |= ClipCaps.AutoScale;

                return result;
            }
        }

        [InplaceField(nameof(SpriteAnimationBehaviour.m_Alpha))]
        public SpriteAnimationBehaviour m_Template;

        public override double duration => m_Animation?.Duration ?? base.duration;

        // =======================================================================
        [Serializable]
        public enum ColorMode
        {
            Ignore,
            Override,
            Multiply
        }

        // =======================================================================
        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            var playable  = ScriptPlayable<SpriteAnimationBehaviour>.Create(graph, m_Template);
            var behaviour = playable.GetBehaviour();
            behaviour.m_Loop      = m_Loop;
            behaviour.m_Color     = m_Color;
            behaviour.m_Animation = m_Animation;

            return playable;
        }
    }
}