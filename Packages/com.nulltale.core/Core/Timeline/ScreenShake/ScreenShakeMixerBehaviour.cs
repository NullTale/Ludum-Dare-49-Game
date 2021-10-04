using UnityEngine;
using UnityEngine.Playables;

namespace CoreLib.Timeline
{
    public class ScreenShakeMixerBehaviour : PlayableBehaviour
    {
        public Module.CameraVFX.NoiseHandle m_Noise;
        
        // =======================================================================
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if (Application.isPlaying)
                m_Noise = Module.CameraVFX.Shake();
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (Application.isPlaying)
            {
                m_Noise?.Dispose();
                m_Noise = null;
            }
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            var inputCount = playable.GetInputCount();

            // calculate weights
            var amplitude = 0f;
            var frequency = 0f;
            for (var n = 0; n < inputCount; n++)
            {
                // get clips data
                var inputWeight   = playable.GetInputWeight(n);
                var inputPlayable = (ScriptPlayable<ScreenShakeBehaviour>)playable.GetInput(n);
                var behaviour     = inputPlayable.GetBehaviour();
                
                amplitude += behaviour.m_Amplitude * inputWeight;
                frequency +=  behaviour.m_Frequency * inputWeight;
            }

            if (Application.isPlaying)
            {
                m_Noise.ValidateCamera();
                m_Noise.Amplitude = amplitude * info.weight;
                m_Noise.Frequency = frequency * info.weight;
            }
        }
    }
}