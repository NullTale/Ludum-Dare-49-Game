using UnityEngine;

namespace CoreLib
{
    public abstract class Player : MonoBehaviour, IPlayerHandle
    {
        private Lock m_Lock;

        public void Play()
        {
            if (m_Lock.On())
                _play();
        }
        public void Stop()
        {
            if (m_Lock.Off())
                _stop();
        }

        public abstract void _play();
        public abstract void _stop();
    }
}