using System;

namespace CoreLib
{
	public sealed class TickedObject : ITicked
	{
        public event Action Tick;

        public float TickLength { get; }

        private TickedQueue m_TickedQueue;
        private bool        m_Loop;

        //////////////////////////////////////////////////////////////////////////
		public TickedObject(Action callback, TickedQueue tickedQueue, float tickLength, bool loop)
		{
            Tick          = callback;
            TickLength    = tickLength;
            m_TickedQueue = tickedQueue;
            m_Loop        = loop;
		}

        public void Enqueue()
        {
            m_TickedQueue.Add(this, m_Loop);
        }

        public void Dequeue()
        {
            m_TickedQueue.Remove(this);
        }

        public void OnTicked()
        {
            Tick?.Invoke();
        }
	}
}

