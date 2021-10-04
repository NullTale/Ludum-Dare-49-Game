using System;
using System.Collections;
using System.Collections.Generic;
using CoreLib.Module;
using CoreLib.States;
using UnityEngine;
using UnityEngine.Playables;

namespace CoreLib.Timeline
{
    public class LoopBehaviour : PlayableBehaviour
    {
        public GlobalState   m_State;
        public LoopCondition m_LoopWhile;

        //public LoopMode      m_Mode;

        public PlayableDirector m_Director;

        private bool     m_IsPlaing;

        // =======================================================================
        [Serializable]
        public enum LoopCondition
        {
            Never,

            Open,
            Close,
            Always
        }

        [Serializable]
        public enum LoopMode
        {
            Repeat,
            Hold
        }

        // =======================================================================
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            m_IsPlaing = true;
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (m_IsPlaing == false)
                return;

            if (m_Director == null)
                return;

            if (m_State == null)
            {
                Debug.LogWarning($"Loop State must be set director: ({m_Director.name})");
                return;
            }

            var loop = m_LoopWhile switch
            {
                LoopCondition.Never   => false,
                LoopCondition.Open   => m_State.IsOpen,
                LoopCondition.Close  => m_State.IsOpen == false,
                LoopCondition.Always => true,
                _                    => throw new ArgumentOutOfRangeException()
            };

            if (Application.isPlaying == false)
                loop = true;

            if (loop)
            {
                m_Director.time -= playable.GetDuration();
            }
            else
            {
                m_IsPlaing = false;
            }
        }
    }
}
