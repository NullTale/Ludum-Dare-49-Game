using CoreLib;
using UnityEngine;

namespace Game
{
    public class SmoothRotation : MonoBehaviour
    {
        public AnimationCurve m_Amplitude;
        public float          m_Interval;
        public float          m_ValueScale;
        public bool           m_UnScaledTime;

        private Quaternion m_Impact;
        private float      m_StartTime;
        public  bool       m_Random;

        // =======================================================================
        private void Awake()
        {
            m_StartTime = (m_UnScaledTime ? Time.unscaledTime : Time.time);
            if (m_Random)
                m_StartTime -= m_Interval.Range();
        }

        private void Update()
        {
            var currentTime = (m_UnScaledTime ? Time.unscaledTime : Time.time) - m_StartTime;

            var impact = Quaternion.AngleAxis(m_Amplitude.Evaluate(currentTime / m_Interval) * m_ValueScale, Vector3.forward);
            transform.rotation *= impact * Quaternion.Inverse(m_Impact);
            m_Impact           =  impact;
        }
    }
}