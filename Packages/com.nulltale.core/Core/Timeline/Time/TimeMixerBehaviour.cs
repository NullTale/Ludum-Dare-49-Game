using CoreLib.Module;
using UnityEngine;
using UnityEngine.Playables;

namespace CoreLib.Timeline
{
    public class TimeMixerBehaviour : PlayableBehaviour
    {
        public  TimeControl.TimeHandle.BlendingMode m_Blending;
        private TimeControl.TimeHandle              m_TimeHandle;

        // =======================================================================
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if (Application.isPlaying)
            {
                m_TimeHandle = TimeControl.TimeHandle.Create(1f, m_Blending);
                m_TimeHandle.Open();
            }
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (Application.isPlaying)
            {
                m_TimeHandle.Dispose();
                m_TimeHandle = null;
            }
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            var inputCount = playable.GetInputCount();

            // calculate weights
            var timeScale  = 0f;
            var fullWeight = 0f;
            for (var n = 0; n < inputCount; n++)
            {
                // get clips data
                var inputWeight   = playable.GetInputWeight(n);
                var inputPlayable = (ScriptPlayable<TimeBehaviour>)playable.GetInput(n);
                var behaviour     = inputPlayable.GetBehaviour();
                
                fullWeight += inputWeight;
                timeScale  += behaviour.m_TimeScale * inputWeight;
            }

            if (fullWeight < 1f)
                timeScale += fullWeight.OneMinus();

            if (Application.isPlaying)
                m_TimeHandle.Scale = timeScale;
        }
    }
}