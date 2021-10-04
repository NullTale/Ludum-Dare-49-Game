using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace CoreLib.CameraVFX
{
    [RequireComponent(typeof(Volume))]
    public class PostEffectInvoker : MonoBehaviour
    {
        public  float                    m_Duration;
        public  Optional<AnimationCurve> m_Interpolation;
        private Volume                   m_Volume;
        private Coroutine                m_Coroutine;
        private float                    m_CurrentTime;
        public  bool                     m_UnScaled;

        // =======================================================================
        private void Awake()
        {
            m_Volume = GetComponent<Volume>();
        }

        public void Invoke()
        {
            if (m_Coroutine != null)
                StopCoroutine(m_Coroutine);

            m_Coroutine = StartCoroutine(_lerp());
        }

        private void OnDisable()
        {
            m_Coroutine = null;
        }

        // =======================================================================
        private IEnumerator _lerp()
        {
            m_CurrentTime = 0f;
            while (m_CurrentTime < m_Duration)
            {
                m_Volume.weight =  m_Interpolation ? m_Interpolation.Value.Evaluate((m_CurrentTime / m_Duration) * m_Interpolation.Value.Duration()) : 1f;
                m_CurrentTime   += m_UnScaled ? Time.unscaledDeltaTime : Time.deltaTime;
                yield return null;
            }

            m_Coroutine = null;
        }
    }
}