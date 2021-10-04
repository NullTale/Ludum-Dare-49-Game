using System;
using UnityEngine;

namespace CoreLib.CameraVFX
{
    public class ScreenOverlayEffect : ScreenOverlay
    {
        public  AnimationCurve m_Fade       = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
        public  OnComplete     m_OnComplete = OnComplete.Disable;
        private float          m_CurrentTime;
        private float          m_Duration;

        [SerializeField]
        private float m_Speed = 1.0f;
        [SerializeField] [Range(0, 1)]
        private float m_Alpha = 1.0f;

        public float Speed
        {
            get => m_Speed;
        }

        public float Alpha
        {
            get => m_Alpha;
        }

        // =======================================================================
        [Serializable]
        public enum OnComplete
        {
            Nothing,
            Disable,
            Destroy,
        }

        // =======================================================================
        public ScreenOverlayEffect SetAlpha(float alpha)
        {
            m_Alpha = alpha;
            return this;
        }

        public void Play(float speed)
        {
            m_Speed = speed;
            Play();
        }

        public void Play()
        {
            if (gameObject.activeSelf)
            {
                m_CurrentTime = 0.0f;
                m_Duration    = m_Fade.Duration() / m_Speed;
            }
            else
                gameObject.SetActive(true);
        }

        protected override void OnEnable()
        {
            m_Handle.Color = m_Color.WithA(m_Color.a * m_Fade.Evaluate(m_CurrentTime));
            m_Handle.Sprite = m_Sprite;
            m_Handle.Scale = m_Scale;

            m_CurrentTime = 0.0f;
            m_Duration = m_Fade.Duration() / m_Speed;

            m_Handle.Open();
        }

        protected override void Update()
        {
            m_Handle.Color = m_Color.WithA(m_Color.a * m_Fade.Evaluate(m_CurrentTime * m_Speed) * m_Alpha);
            m_Handle.Sprite = m_Sprite;
            m_Handle.Scale = m_Scale;
            
            if (m_CurrentTime >= m_Duration)
                switch (m_OnComplete)
                {
                    case OnComplete.Disable:
                        gameObject.SetActive(false);
                        break;
                    case OnComplete.Destroy:
                        Destroy(gameObject);
                        break;
                    case OnComplete.Nothing:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            m_CurrentTime += Time.deltaTime;
        }
    }
}