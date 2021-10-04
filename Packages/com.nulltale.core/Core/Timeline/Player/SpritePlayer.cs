using System;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;

namespace CoreLib
{
    public sealed class SpritePlayer : Player
    {
        public float m_Duration = 0.6f;

        [CurveRange(0, 0, 1, 1)]
        public AnimationCurve m_Transition = AnimationCurve.Linear(0, 0, 1, 1);

        private SpriteRenderer m_SpriteRenderer;
        private float          m_InitialAlpha;
        private float          m_Impact;

        public RendererState m_State = RendererState.Hidden;

        public RendererState State
        {
            get => m_State;
            set
            {
                if (Application.isPlaying)
                {
                    if (m_State == value)
                        return;

                    _init();
                    m_State = value; 

                    switch (m_State)
                    {
                        case RendererState.Visible:
                        {
                            gameObject.SetActive(true);
                            if (m_TransitionCoroutine != null)
                                StopCoroutine(m_TransitionCoroutine);
                            m_TransitionCoroutine = StartCoroutine(_fadeIn());
                        }
                            break;

                        case RendererState.Hidden:
                        {
                            if (m_TransitionCoroutine != null)
                                StopCoroutine(m_TransitionCoroutine);
                            m_TransitionCoroutine = StartCoroutine(_fadeOut());
                        }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                else
                {
                    switch (value)
                    {
                        case RendererState.Visible:
                        {
                            gameObject.SetActive(true);
                        } break;

                        case RendererState.Hidden:
                        {
                            gameObject.SetActive(false);
                        } break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        private Coroutine m_TransitionCoroutine;
        private bool IsInit;

        // =======================================================================
        [Serializable]
        public enum RendererState
        {
            Visible,
            Hidden,
        }

        // =======================================================================
        private void Awake()
        {
            _init();
        }

        private void _init()
        {
            if (IsInit)
                return;

            IsInit = true;
            m_SpriteRenderer = GetComponent<SpriteRenderer>();
            m_InitialAlpha   = m_SpriteRenderer.color.a;
            switch (State)
            {
                case RendererState.Visible:
                    m_Impact = m_InitialAlpha;
                    break;
                case RendererState.Hidden:
                    m_Impact               = 0f;
                    m_SpriteRenderer.color = m_SpriteRenderer.color.WithA(0f);
                    gameObject.SetActive(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [Button]
        public override void _play()
        {
            State = RendererState.Visible;
        }

        [Button]
        public override void _stop()
        {
            State = RendererState.Hidden;
        }

        private IEnumerator _fadeOut()
        {
            var time     = m_Duration;
            var distance = m_Impact;
            var impactLocal = distance;

            while (time > 0f)
            {
                time -= Time.deltaTime;

                var impact = distance * m_Transition.Evaluate(time / m_Duration) - impactLocal;
                impactLocal            += impact;
                m_Impact               += impact;
                m_SpriteRenderer.color =  m_SpriteRenderer.color.IncA(impact);

                yield return null;
            }

            m_TransitionCoroutine = null;
            gameObject.SetActive(false);
        }

        private IEnumerator _fadeIn()
        {
            var time     = 0f;
            var distance = m_InitialAlpha - m_Impact;
            var impactLocal = 0f;

            while (time < m_Duration)
            {
                time += Time.deltaTime;

                var impact = distance * m_Transition.Evaluate(time / m_Duration) - impactLocal;
                impactLocal            += impact;
                m_Impact               += impact;
                m_SpriteRenderer.color =  m_SpriteRenderer.color.IncA(impact);

                yield return null;
            }

            m_TransitionCoroutine = null;
        }
    }
}