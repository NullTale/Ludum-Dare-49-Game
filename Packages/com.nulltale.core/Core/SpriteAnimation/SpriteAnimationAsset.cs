using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Timeline;

namespace CoreLib.SpriteAnimation
{
    [CreateAssetMenu(menuName = "2D/Sprite Animation", fileName = "SpriteAnimation", order = 0)]
    public class SpriteAnimationAsset : ScriptableObject
    {
        public float          m_FrameRate = 1f / 0.1f;
        public List<KeyFrame> m_KeyFrames;

        public AnimationClip m_SpriteAnimationClip;
        public AnimationClip m_ImageAnimationClip;

        public bool m_CreateSpriteAnimation;
        public bool m_CreateImageAnimation;

        public bool  m_Color;
        public float m_Duration;

        public float Duration => m_Duration;

        // =======================================================================
        [Serializable]
        public class KeyFrame
        {
            public float  Duration;
            public Color  Color;
            public Sprite Sprite;
        }

        // =======================================================================
        public Sprite SpriteAt(float time, bool loop)
        {
            return KeyFrameAt(time, loop)?.Sprite;
        }

        public KeyFrame KeyFrameAt(float time, bool loop)
        {
            var frame = Mathf.Abs(time);

            if (frame > m_Duration)
                if (loop)
                    frame = frame % m_Duration;
                else
                    frame = m_Duration;

            foreach (var keyFrame in m_KeyFrames)
            {
                frame -= keyFrame.Duration / m_FrameRate;
                if (frame < 0f)
                    return keyFrame;
            }

            return m_KeyFrames.Count > 0 ? m_KeyFrames[m_KeyFrames.Count - 1] : null;
        }
    }

    public static class SpriteAnimationExtensions
    {
        public class OneShotChecker : MonoBehaviour
        {
            [NonSerialized]
            public SpriteAnimationPlayer Animation;

            private void LateUpdate()
            {
                if (Animation.Animation.Duration < Animation.Time)
                {
                    Destroy(Animation);
                    Destroy(this);
                }
            }
        }

        // =======================================================================
        public static void Play(this SpriteAnimationAsset sa, GameObject go)
        {
            var psa = go.AddComponent<PlaySpriteAnimation>();
            psa.Animation = sa;
        }

        public static void PlayOneShot(this SpriteAnimationAsset sa, GameObject go)
        {
            var psa = go.AddComponent<PlaySpriteAnimation>();
            psa.Animation                               = sa;
            go.AddComponent<OneShotChecker>().Animation = psa;
        }
    }
}