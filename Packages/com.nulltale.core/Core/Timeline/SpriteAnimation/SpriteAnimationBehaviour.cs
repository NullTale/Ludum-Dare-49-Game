using System;
using UnityEngine;
using UnityEngine.Playables;

namespace CoreLib.Timeline
{
    [Serializable]
    public class SpriteAnimationBehaviour : PlayableBehaviour
    {
        [NonSerialized]
        public  SpriteAnimation.SpriteAnimationAsset m_Animation;
        [NonSerialized]
        public Sprite                                m_InitialSprite;
        [NonSerialized]
        public Color                                 m_InitialColor;
        [NonSerialized]
        public  SpriteRenderer                       m_SpriteRenderer;
        [NonSerialized]
        public  bool                                 m_Loop;
        [NonSerialized]
        public SpriteAnimationAsset.ColorMode        m_Color;
        public float                                 m_Alpha = 1f;

        // =======================================================================
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            m_SpriteRenderer = null;
        }
        
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            var spriteRenderer = (SpriteRenderer)playerData;
            if (m_SpriteRenderer == null && spriteRenderer != null)
            {
                m_SpriteRenderer = spriteRenderer;
                m_InitialSprite = m_SpriteRenderer.sprite;
                m_InitialColor = m_SpriteRenderer.color;
            }

            var frame = m_Animation.KeyFrameAt((float)playable.GetTime(), m_Loop);
            m_SpriteRenderer.sprite = frame.Sprite;
            var alpha = m_Alpha.Clamp01();
            switch (m_Color)
            {
                case SpriteAnimationAsset.ColorMode.Ignore:
                    m_SpriteRenderer.color = m_SpriteRenderer.color.MulA(alpha);
                    break;
                case SpriteAnimationAsset.ColorMode.Override:
                    m_SpriteRenderer.color = frame.Color.MulA(alpha);
                    break;
                case SpriteAnimationAsset.ColorMode.Multiply:
                    m_SpriteRenderer.color *= frame.Color.MulA(alpha);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (m_SpriteRenderer != null)
            {
                m_SpriteRenderer.sprite = m_InitialSprite;
                m_SpriteRenderer.color = m_InitialColor;
            }

            m_SpriteRenderer = null;
        }
    }
}