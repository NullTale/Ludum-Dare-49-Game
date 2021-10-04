using System;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering;

namespace CoreLib.CameraVFX
{
    [RequireComponent(typeof(Volume))]
    public class PostProcessState : MonoBehaviour
    {
        public  AnimationCurve m_FadeIn  = new AnimationCurve(new Keyframe(0.0f, 0.0f), new Keyframe(1.0f, 1.0f));
        public  AnimationCurve m_FadeOut = new AnimationCurve(new Keyframe(0.0f, 1.0f), new Keyframe(1.0f, 0.0f));
        private float          m_CurrentTime;
        private float          m_Duration;
        private Volume         m_Volume;

        private Coroutine      m_Animation;

        [SerializeField]
        private State        m_On;

        public State On
        {
            get => m_On;
            set
            {
                if (m_On == value)
                    return;

                _apply(value);
            }
        }

        public Volume Volume => m_Volume;

        // =======================================================================
        [Serializable]
        public enum State
        {
            Show,
            Hide,
            Disable
        }

        // =======================================================================
        private void Awake()
        {
            m_Volume = GetComponent<Volume>();
        }

        private void OnDisable()
        {
            m_Volume.weight = 0.0f;
            m_On = State.Disable;
        }

        // =======================================================================
        private IEnumerator _fadeIn()
        {
            var initialWeight = m_Volume.weight;

            m_Duration    = m_FadeIn.FinishTime();
            m_CurrentTime = 0.0f;

            // skip until initial weight
            while (initialWeight >= m_Volume.weight && m_CurrentTime <= m_Duration)
            {
                m_Volume.weight = m_FadeIn.Evaluate(m_CurrentTime);
                m_CurrentTime   += Time.fixedDeltaTime;
            }

            while (m_CurrentTime <= m_Duration)
            {
                yield return null;
                m_Volume.weight = m_FadeIn.Evaluate(m_CurrentTime);
                m_CurrentTime   += Time.deltaTime;
            }

            m_Animation = null;
        }

        private IEnumerator _fadeOut()
        {
            m_Duration    = m_FadeOut.FinishTime();
            m_CurrentTime = 0;
            
            // skip until initial weight
            var initialWeight = m_Volume.weight;
            while (initialWeight <= m_Volume.weight && m_CurrentTime <= m_Duration)
            {
                m_Volume.weight = m_FadeOut.Evaluate(m_CurrentTime);
                m_CurrentTime   += Time.fixedDeltaTime;
            }

            while (m_CurrentTime <= m_Duration)
            {
                yield return null;
                m_Volume.weight = m_FadeOut.Evaluate(m_CurrentTime);
                m_CurrentTime   += Time.deltaTime;
            }
            
            m_Animation = null;
            On = State.Disable;
        }

        private void _apply(State state)
        {
            switch (state)
            {
                case State.Disable:
                    m_On = State.Disable;
                    gameObject.SetActive(false);
                    break;

                case State.Show:
                    m_On = State.Show;

                    if (m_Animation != null)
                        StopCoroutine(m_Animation);

                    gameObject.SetActive(true);
                    m_Animation = StartCoroutine(_fadeIn());
                    break;

                case State.Hide:
                    if (gameObject.activeSelf == false)
                        return;

                    m_On = State.Hide;

                    if (m_Animation != null)
                        StopCoroutine(m_Animation);

                    m_Animation = StartCoroutine(_fadeOut());
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        [Button] public void Disable() => On = State.Disable;
        [Button] public void FadeIn() => On = State.Show;
        [Button] public void FadeOut() => On = State.Hide;
    }
}