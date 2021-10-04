using CoreLib.Sound;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Playables;


namespace CoreLib.Timeline
{
    public class MixerParameterMixerBehaviour : PlayableBehaviour
    {
        // =======================================================================
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            var parameter = ((MixerExposedParameter)playerData).m_Parameter;

            // try to get mixer
            var audioMixer = SoundManager.Instance.Mixer;
            if (audioMixer == null)
                return;

            var finalValue = 0.0f;

            // calculate weights
            var inputCount = playable.GetInputCount();
            for (var i = 0; i < inputCount; i++)
            {
                //if (playable.IsPlayableOfType<ScriptPlayable<MixerPlayableBehaviour>>() == false)
                //    continue;

                // get clips data
                var inputWeight = playable.GetInputWeight(i);
                if (inputWeight <= 0f)
                    continue;

                var inputPlayable = (ScriptPlayable<MixerParameterBehaviour>)playable.GetInput(i);
                var input = inputPlayable.GetBehaviour();
                
                // add weighted impact of the clip to final value
                finalValue += input.Value * inputWeight;
            }

            // assign result
            audioMixer.SetFloat(parameter, finalValue);
        }
    }
}