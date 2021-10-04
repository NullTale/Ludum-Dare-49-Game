using System;
using UnityEngine;
using UnityEngine.Playables;

namespace CoreLib.Timeline
{
    [Serializable]
    public class ParticleSystemBehaviour : PlayableBehaviour
    {
        public  ParticleSystem m_ParticleSystem;
        private float          m_LastPlayableTime;

        public ParticleSystem ParticleSystem
        {
            get => m_ParticleSystem;
            set
            {
                if (m_ParticleSystem == value)
                    return;
                
                m_ParticleSystem?.Stop(true);
                m_ParticleSystem = value;
                m_ParticleSystem?.Play(true);
            }
        }

        // =======================================================================
        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
#if UNITY_EDITOR
            if (ParticleSystem.IsNull())
                return;

            if (Application.isPlaying == false)
            {
                ParticleSystem?.Clear(true);
            }
#endif
            ParticleSystem = null;
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            ParticleSystem = (ParticleSystem)playerData;

#if UNITY_EDITOR
            if (ParticleSystem.IsNull())
                return;

            if (Application.isPlaying == false)
            {
                var time = (float)playable.GetTime();

                _simulate(time - m_LastPlayableTime, false);
                m_LastPlayableTime = time;
            }
#endif
        }

        private void _simulate(float time, bool restart)
        {
            const bool withChildren  = false;
            const bool fixedTimeStep = false;
            float      maxTime       = Time.maximumDeltaTime;

            if (restart)
                ParticleSystem.Simulate(0, withChildren, true, fixedTimeStep);

            // simulating by too large a time-step causes sub-emitters not to work, and loops not to
            // simulate correctly
            while (time > maxTime)
            {
                ParticleSystem.Simulate(maxTime, withChildren, false, fixedTimeStep);
                time -= maxTime;
            }

            if (time > 0)
                ParticleSystem.Simulate(time, withChildren, false, fixedTimeStep);
        }
    }
}