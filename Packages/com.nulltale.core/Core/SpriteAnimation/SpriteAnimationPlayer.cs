using System;
using UnityEngine;
using UnityEngine.UI;

namespace CoreLib.SpriteAnimation
{
    public abstract class SpriteAnimationPlayer : MonoBehaviour
    {
        [SerializeField]
        protected Optional<GameObject> m_Target;
        [SerializeField]
        protected SpriteAnimationAsset m_Animation;
        [SerializeField]
        protected float m_Speed = 1f;
        [SerializeField]
        protected bool m_Loop = true;
        [SerializeField]
        protected UpdateMethod m_UpdateMethod = UpdateMethod.Time;
        [SerializeField]
        protected float m_Time;

        // =======================================================================
        public interface IAdapter
        {
            void Set(Sprite sprite);
        }

        public class SpriteAdapter : IAdapter
        {
            private SpriteRenderer m_SpriteRenderer;

            public void Set(Sprite sprite)
            {
                m_SpriteRenderer.sprite = sprite;
            }

            public SpriteAdapter(SpriteRenderer spriteRenderer)
            {
                m_SpriteRenderer = spriteRenderer;
            }
        }
        
        public class ImageAdapter : IAdapter
        {
            private Image m_Image;

            public void Set(Sprite sprite)
            {
                if (m_Image.sprite != sprite)
                    m_Image.sprite = sprite;
            }

            public ImageAdapter(Image image)
            {
                m_Image = image;
            }
        }

        // =======================================================================
        [Serializable]
        public enum UpdateMethod
        {
            None,

            Time,
            Unscaled,
            Physics
        }

        // =======================================================================
        public float Speed
        {
            get => m_Speed;
            set => m_Speed = value;
        }

        public virtual SpriteAnimationAsset Animation
        {
            get => m_Animation;
            set => m_Animation = value;
        }

        public virtual float Time
        {
            get => m_Time;
            set => m_Time = value;
        }
    }
}