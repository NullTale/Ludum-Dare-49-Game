using CoreLib;
using UnityEngine;

namespace Game
{
    public class SmoothScale : MonoBehaviour
    {
        public AnimationCurve m_Amplitude;
        public float          m_Interval;
        public Vector3        m_ValueScale = Vector3.one;
        public bool           m_UnScaledTime;

        private Vector3 m_Impact;
        private float   m_StartTime;
        public bool     m_Random;

        // =======================================================================
        private void Awake()
        {
            m_StartTime = (m_UnScaledTime ? Time.unscaledTime : Time.time);
            if (m_Random)
                m_StartTime -= m_Interval.Range();
        }

        private void OnDisable()
        {
            transform.localScale -= m_Impact;
            m_Impact             =  Vector3.zero;
        }

        private void Update()
        {
            var currentTime = (m_UnScaledTime ? Time.unscaledTime : Time.time) - m_StartTime;

            var impact = m_Amplitude.Evaluate(currentTime / m_Interval) * m_ValueScale;
            transform.localScale += impact - m_Impact;
            m_Impact = impact;
        }
    }
}