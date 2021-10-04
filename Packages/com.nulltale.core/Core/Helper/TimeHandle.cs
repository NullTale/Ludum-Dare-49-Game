using CoreLib.Module;
using UnityEngine;

namespace CoreLib
{
    public class TimeHandle : MonoBehaviour
    {
        [SerializeField]
        private float m_TimeScale = 1f;
        [SerializeField]
        private TimeControl.TimeHandle.BlendingMode m_Blending = TimeControl.TimeHandle.BlendingMode.MinOverride;
        [Tooltip("Default priorities: Instant -100, SlowDown 0, SpeedUp 100")]
        public Optional<int>            m_Order = new Optional<int>(TimeControl.k_SlowDownDefaultPriority, false);
        public Optional<float>          m_TransitionDuration = new Optional<float>(false);
        public Optional<AnimationCurve> m_TransitionCurve = new Optional<AnimationCurve>(false);

        private TimeControl.TimeHandle m_TimeHandle;

        public float Scale
        {
            get => m_TimeScale;
            set => m_TimeScale = value;
        }

        // =======================================================================
        private void Awake()
        {
            m_TimeHandle =  m_Order.Enabled ? TimeControl.TimeHandle.Create(0f, m_Blending, m_Order.Value) : TimeControl.TimeHandle.Create(0f, m_Blending);
        }

        private void Update()
        {
            m_TimeHandle.Scale = m_TimeScale;
        }

        private void OnEnable()
        {
            m_TimeHandle.Open();

            if (m_TransitionDuration.Enabled)
                m_TimeHandle.TransitionTime = m_TransitionDuration.Value;
            if (m_TransitionCurve.Enabled)
                m_TimeHandle.TransitionCurve = m_TransitionCurve.Value;
        }

        private void OnDisable()
        {
            m_TimeHandle.Close();
        }

        private void OnDestroy()
        {
            m_TimeHandle?.Dispose();
        }
    }
}