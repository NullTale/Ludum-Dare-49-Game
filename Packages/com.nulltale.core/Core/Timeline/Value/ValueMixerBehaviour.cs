using UnityEngine.Playables;

namespace CoreLib.Timeline
{
    public class ValueMixerBehaviour : PlayableBehaviour
    {
        private IPlayableValue m_PlayableTarget;

        // =======================================================================
        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            m_PlayableTarget?.UnLock();
            m_PlayableTarget = null;
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            var target = playerData as IPlayableValue;
            if (m_PlayableTarget != target)
            {
                m_PlayableTarget?.UnLock();
                m_PlayableTarget = target;
                m_PlayableTarget?.Lock();
            }

            if (m_PlayableTarget == null)
                return;

            var inputCount = playable.GetInputCount();
            // calculate weights
            var value  = 0f;
            var weight = 0f;
            for (var n = 0; n < inputCount; n++)
            {
                // get clips data
                var inputWeight   = playable.GetInputWeight(n);
                var inputPlayable = (ScriptPlayable<ValueBehaviour>)playable.GetInput(n);
                var behaviour     = inputPlayable.GetBehaviour();
                
                weight += inputWeight;
                value  += behaviour.m_Value * inputWeight;
            }

            m_PlayableTarget.Set(value, weight);
        }
    }
}