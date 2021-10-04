using UnityEngine.Playables;

namespace CoreLib.Timeline
{
    public class PlayerBehaviour : PlayableBehaviour
    {
        public IPlayerHandle m_Player;

        public IPlayerHandle Player
        {
            get => m_Player;
            set
            {
                if (m_Player == value)
                    return;
                
                m_Player?.Stop();
                m_Player = value;
                m_Player?.Play();
            }
        }

        // =======================================================================
        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            Player = null;
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            Player = (IPlayerHandle)playerData;
        }
    }
}