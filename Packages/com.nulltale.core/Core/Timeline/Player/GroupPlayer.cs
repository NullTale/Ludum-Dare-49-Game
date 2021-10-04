using System.Linq;

namespace CoreLib
{
    public sealed class GroupPlayer : Player
    {
        private IPlayerHandle[] m_Handles;

        // =======================================================================
        private void Awake()
        {
            m_Handles = GetComponentsInChildren<IPlayerHandle>();
        }

        public override void _play()
        {
            foreach (var handle in m_Handles)
                handle.Play();
        }

        public override void _stop()
        {
            foreach (var handle in m_Handles)
                handle.Stop();
        }
    }
}