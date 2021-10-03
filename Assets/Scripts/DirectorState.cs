using System;
using CoreLib;
using UnityEngine;
using UnityEngine.Playables;

namespace Game
{
    [RequireComponent(typeof(PlayableDirector))]
    public class DirectorState : MonoBehaviour
    {
        public  double           m_SpeedUp   = 1d;
        public  double           m_SpeedDown = 1d;
        private PlayableDirector m_Director;

        public bool m_UnScaledTime;

        [SerializeField] [Range(0, 1)]
        private double m_DesiredTime;

        public double DesiredTime
        {
            get => m_DesiredTime;
            set { m_DesiredTime = value.Clamp01(); }
        }

        private double m_CurrentTime;
        public double CurrentTime
        {
            get => m_CurrentTime;
        }
        public PlayableDirector Director => m_Director;

        // =======================================================================
        public void SetDesiredTime(float normalizedTime)
        {
            DesiredTime = normalizedTime;
        }

        private void Awake()
        {
            m_Director                 = GetComponent<PlayableDirector>();
            Director.timeUpdateMode    = DirectorUpdateMode.Manual;
            Director.extrapolationMode = DirectorWrapMode.Hold;
            Director.time              = 0d;
            m_CurrentTime              = 0d;
        }

        private void OnEnable()
        {
            Director.Evaluate();
        }

        private void Update()
        {
            var timeOffset = Director.duration * DesiredTime - Director.time;
            if (Math.Abs(timeOffset) > 0d)
            {
                var time = (Director.time + (m_UnScaledTime ? Time.unscaledDeltaTime : Time.deltaTime) * (timeOffset > 0d ? m_SpeedUp : -m_SpeedDown)).Clamp(0d, Director.duration);
                if (Director.time == time)
                    return;

                Director.time = time;
                m_CurrentTime = Director.time / Director.duration;
                Director.Evaluate();
            }
        }
    }
}