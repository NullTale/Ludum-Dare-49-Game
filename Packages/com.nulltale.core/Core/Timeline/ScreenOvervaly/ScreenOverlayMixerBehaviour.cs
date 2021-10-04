using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace CoreLib.Timeline
{
    public class ScreenOverlayMixerBehaviour : PlayableBehaviour
    {
        private Module.CameraVFX.ScreenOverlayHandle m_OverlayHandle;

        // =======================================================================
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            m_OverlayHandle = new Module.CameraVFX.ScreenOverlayHandle();
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            m_OverlayHandle?.Dispose();
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            var inputCount = playable.GetInputCount();

            // calculate weights
            var scale       = 0f;
            var color       = Color.clear;
            var imageWeight = 0f;

            Sprite image = null;

            var fullWeight = 0f;
            var soloInput = 0;

            for (var n = 0; n < inputCount; n++)
            {
                // get clips data
                var inputWeight   = playable.GetInputWeight(n);
                if (inputWeight <= 0f)
                    continue;

                soloInput  =  n;
                fullWeight += inputWeight;

                var inputPlayable = (ScriptPlayable<ScreenOverlayBehaviour>)playable.GetInput(n);
                var behaviour     = inputPlayable.GetBehaviour();

                scale += behaviour.m_Scale * inputWeight;
                color += behaviour.m_Color * inputWeight;

                if (imageWeight < inputWeight)
                {
                    image       = behaviour.m_Image;
                    imageWeight = inputWeight;
                }
            }

            if (fullWeight > 0f)
                m_OverlayHandle.Open();
            else
            {
                m_OverlayHandle.Close();
                return;
            }

            // if single input, blend alpha, do nothing with scale
            if (fullWeight < 1f)
            {
                var behaviour     = ((ScriptPlayable<ScreenOverlayBehaviour>)playable.GetInput(soloInput)).GetBehaviour();

                scale = behaviour.m_Scale;
                color = behaviour.m_Color.MulA(fullWeight);
            }
            

            m_OverlayHandle.Scale  = scale.ToVector2();
            m_OverlayHandle.Color  = color;
            m_OverlayHandle.Sprite = image;
        }
    }
}