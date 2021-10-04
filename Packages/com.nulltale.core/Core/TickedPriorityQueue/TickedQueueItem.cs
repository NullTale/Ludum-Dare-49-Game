using System;
using UnityEngine;

namespace CoreLib
{
	/// <summary> Internal class used for storing a reference to a ticked object, and the times involved for ticking it </summary>
	internal sealed class TickedQueueItem
	{
		public static TickedQueueItemComparer   s_Comparer = new TickedQueueItemComparer();

        internal ITicked m_Ticked;
        private  float   m_NextTickTime;
        private  float   m_LastTick;

        /// <summary> Gets the tick length associated with the wrapped ticked object. </summary>
		/// <value> The minimum length of the tick. </value>
		public float TickLength => m_Ticked.TickLength;
		
		/// <summary> Returns the earliest time the instance can be ticked. </summary>
		public float NextTickTime => m_NextTickTime;

        /// <summary> Returns the wrapped <see cref="ITicked"/> object. </summary>
		public ITicked Ticked => m_Ticked;

        /// <summary> Sets whether the instance will be repeatedly ticked, or ticked once </summary>
		public bool Loop { get; set; }

		/// <summary> Gets or sets a value indicating whether this instance is active. </summary>
		/// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
		public bool IsActive { get; set; }

        private Ref<float> m_DeltaTime = new Ref<float>();
		
        //////////////////////////////////////////////////////////////////////////
        public TickedQueueItem(){ }

		/// <summary> Initializes a new instance of the <see cref="TickedQueueItem"/> class. </summary>
		/// <param name='ticked'> Object to be ticked. </param>
		/// <exception cref='ArgumentNullException'> Is thrown when an argument passed to a method is invalid because it is <see langword="null" /> . </exception>
		public TickedQueueItem(ITicked ticked) : this(ticked, Time.time, true)
		{
		}

		internal void ResetTickFromTime(float time)
		{
            m_LastTick = time;
			m_NextTickTime = time + m_Ticked.TickLength;
		}

        public override bool Equals(object obj)
        {
            return Equals(m_Ticked, ((TickedQueueItem)obj).m_Ticked);
        }

        public override int GetHashCode()
        {
            return m_Ticked.GetHashCode();
        }
		
		/// <summary> Initializes a new instance of the <see cref="TickedQueueItem"/> class. </summary>
		/// <param name='ticked'> Object to be ticked. </param>
		/// <param name='currentTime'> Current time. </param>
		/// <param name="isLooped">Indicates if this item should be looped</param>
		/// <exception cref='ArgumentNullException'> Is thrown when an argument passed to the constructor is invalid because it is <see langword="null" /> . </exception>
		public TickedQueueItem(ITicked ticked, float currentTime, bool isLooped)
		{
#if DEBUG
            if (ticked == null)
                throw new ArgumentNullException("Missing a valid ITicked reference");
#endif
			if (ticked is ITickedDelta ti)
                ti.TickedDelta = m_DeltaTime;

            m_Ticked = ticked;
			ResetTickFromTime(currentTime);
			Loop = isLooped;
			IsActive = true;
		}
		
		
		/// <summary> Checks whether the class is ready to be ticked. </summary>
		/// <returns> The tick ready status. </returns>
		/// <param name='current'> The current time in <see cref="System.DateTime"/> format. </param>
		public bool CheckTickReady(float current)
		{
			return (current > m_NextTickTime);
		}
		
		/// <summary> Called when the class is being processed for tick elapsed. </summary>
		/// <param name='current'> The current time in <see cref="System.DateTime"/> format. </param>
		public void Tick(float current)
		{
			m_DeltaTime.Value = current - m_LastTick;
			m_Ticked.OnTicked();
			ResetTickFromTime(current);
		}

        /// <summary> Checks if this instance wraps the specified <see cref="ITicked"/> object. </summary>
		/// <returns> <c>true</c> if the wrapped object is ticked, else <c>false</c>. </returns>
		/// <param name='ticked'> The <see cref="ITicked"/> object to check. </param>
		public bool ContainsTicked(ITicked ticked)
		{
			return Equals(ticked, m_Ticked);
		}
    }
}

