using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

namespace CoreLib
{
    /// <summary> A class which manages ITicked objects, ticking them in order of priority. </summary>
	/// <remarks>
	/// Will never tick an item more than once in a frame, and sets the updated tick
	/// time to the sum of processed time and the object's Tick Length.
	/// And and Update can use a user provided DateTime for the current time, allowing for custom timing, e.g. for pausing the game.
	/// </remarks>
	public sealed class TickedQueue : ScriptableObject
	{
        public const float       k_DefaultTickLength                 = 0.25f;
        public const int         k_PreAllocateSize                   = 100;
        public const int         k_DefaultMaxProcessedPerUpdate      = 1000;
        public const float       k_DefaultMaxProcessingTimePerUpdate = (1000.0f / 30.0f) / 1000.0f;
        public       UpdateGroup m_UpdateGroup                       = UpdateGroup.Time;
        public       Ref<float>  m_DeltaTime                         = new Ref<float>();

        public       IRefGet<float> DeltaTime => m_DeltaTime;

		[SerializeField]
        private float m_Interval;

        private float m_NextUpdate;

		/// <summary>
		/// Sets whether new items added are looped or not.
		/// Overriden by setting the loop mode in Add.
		/// </summary>
		public bool m_LoopByDefault = true;

        /// <summary>
		/// Gets or sets a value indicating whether this queue instance is paused.
		/// Paused queues will ignore Update calls.
		/// </summary>
		/// <value><c>true</c> if this instance is paused; otherwise, <c>false</c>.</value>
		public bool m_IsPaused;
		
		/// <summary> The queue. </summary>
		private List<TickedQueueItem> m_Queue = new List<TickedQueueItem>(k_PreAllocateSize);
		
		/// <summary> Pre-allocated working queue from which items will be evaluated. </summary>
		private TickedQueueItem[] m_WorkingQueue;
		
		/// <summary> Max ITicked objects to be processed in a single Update call. </summary>
		[SerializeField]
        private int m_MaxProcessedPerUpdate = k_DefaultMaxProcessedPerUpdate;

		[SerializeField]
        private float m_MaxProcessingTimePerUpdate = k_DefaultMaxProcessingTimePerUpdate;
		
		/// <summary>
		/// Gets or sets the max time allowed for processing ITicked objects in a single Update call.
		/// Note - this is in real time, setting custom update times will not affect it.
		/// </summary>
		public float MaxProcessingTimePerUpdate
		{ 
			get => m_MaxProcessingTimePerUpdate;
            set => m_MaxProcessingTimePerUpdate = value;
        }
		
		/// <summary> Gets the internal queue count. </summary>
		public int QueueCount => m_Queue.Count;

        /// <summary> Returns an IEnumerable for the ticked items on the queue </summary>
		/// <value>The ticked items.</value>
		public IEnumerable<ITicked> Items => m_Queue.Select(x => x.Ticked);

        //////////////////////////////////////////////////////////////////////////
        private class TickedSimple : ITicked
        {
            public float         TickLength { get; }
            public object        Key;
            public Action		 Action;

            //////////////////////////////////////////////////////////////////////////
            public void OnTicked()
            {
				Action.Invoke();
            }

            public override bool Equals(object obj)
            {
                if (obj.GetType() != this.GetType())
                    return false;

                return Equals(Key, ((TickedSimple)obj).Key);
            }

            public override int GetHashCode()
            {
                return (Key != null ? Key.GetHashCode() : 0);
            }

            public TickedSimple(object key, Action action, float tickLength)
            {
                Key        = key;
                Action     = action;
                TickLength = tickLength;
            }
        }

        [Serializable]
        public enum UpdateGroup
        {
            Time,
			UnscaledTime,
			FixedTime,
        }

        //////////////////////////////////////////////////////////////////////////
        /// <summary> Add the specified ticked object to the queue. </summary>
		/// <param name='ticked'> The ITicked object. </param>
		public void Add(ITicked ticked)
		{
			_add(ticked, TickedQueueManager.GetCurrentTime(m_UpdateGroup), m_LoopByDefault);
		}
		
		public void Add(Action onTicked, float interval, object key, bool looped = true)
		{
            Add(new TickedSimple(key, onTicked, interval), looped);
		}

		public void Add(Action onTicked, float interval = ATicked.k_DefaultTickLength)
		{
            Add(new TickedSimple(onTicked, onTicked, interval));
		}

		/// <summary> Add the specified ticked object to the queue. </summary>
		/// <param name='ticked'> The ITicked object. </param>
		/// <param name='looped'> Sets whether the ticked item will be called once, or looped. </param>
		public void Add(ITicked ticked, bool looped)
		{
			_add(ticked, TickedQueueManager.GetCurrentTime(m_UpdateGroup), looped);
		}

        private static TickedQueueItem s_ForRemoveWrapper = new TickedQueueItem();
		/// <summary>
		/// Remove the specified ticked object from the queue.
		/// Will only remove the same object once, even if multiple instances exist.
		/// </summary>
		/// <param name='ticked'> The ITicked object to remove. </param>
		/// <returns>True if the item was successfully removed, false otherwise</returns>
		public bool Remove(ITicked ticked)
		{
			s_ForRemoveWrapper.m_Ticked = ticked;
			var index = m_Queue.IndexOf(s_ForRemoveWrapper);
			if (index == -1)
				return false;

            // In case the item is added to a work queue
			m_Queue[index].IsActive = false;
			m_Queue.RemoveAt(index);

			return true;
		}
		
        private static TickedSimple s_ForRemoveTickedSimple = new TickedSimple(null, null, 0);
		public bool Remove(object tickedKey)
		{
			s_ForRemoveTickedSimple.Key = tickedKey;
			return Remove(s_ForRemoveTickedSimple);
        }

		public bool Remove(Action tickedKey)
		{
			s_ForRemoveTickedSimple.Key = tickedKey;
			return Remove(s_ForRemoveTickedSimple);
        }
		
		/// <summary> Updates the queue, calling OnTicked for the first MaxProcessedPerUpdate items which have timed out. </summary>
        internal void Update()
		{
			Update(Time.time);
		}
		
		/// <summary>
		/// Updates the queue, calling OnTicked for the first MaxProcessedPerUpdate items which have timed out.
		/// Uses a user provided DateTime for the current time, allowing for custom timing, e.g. for pausing the game.
		/// </summary>
		/// <param name='currentTime'> Current time to use. </param>
        internal void Update(float currentTime)
		{
			Profiler.BeginSample($"Ticked queue: {name}", this);
			if (m_IsPaused)
            {
				m_NextUpdate = currentTime;
                return;
            }

			if (m_NextUpdate > currentTime)
				return;

			m_NextUpdate += m_Interval;

			var found = 0;
			var startTime = Time.realtimeSinceStartup;

			var length = m_Queue.Count < m_MaxProcessedPerUpdate ? m_Queue.Count : m_MaxProcessedPerUpdate;
			m_Queue.CopyTo(0, m_WorkingQueue, 0, length);

            for (var index = 0; index < length; index++)
            {
                var item = m_WorkingQueue[index];
                if (item.IsActive)
                {
                    if (item.CheckTickReady(currentTime))
                    {
                        ++found;
                        m_Queue.Remove(item);
                        if (item.Loop)
                            _add(item, currentTime);

                        item.Tick(currentTime);
                    }
                    else
                        break;
                }

                if (found >= m_MaxProcessedPerUpdate)
                    break;

                if (Time.realtimeSinceStartup - startTime > m_MaxProcessingTimePerUpdate)
                    break;
            }

            Profiler.EndSample();
		}

        internal void Init()
        {
            m_WorkingQueue = new TickedQueueItem[m_MaxProcessedPerUpdate];

			m_NextUpdate = TickedQueueManager.GetCurrentTime(m_UpdateGroup);
			m_Queue.Clear();
        }
		
        //////////////////////////////////////////////////////////////////////////
		/// <summary> Add the specified item and currentTime. </summary>
		/// <param name='item'> The TickedQueueItem element to add to the list. </param>
		/// <param name='currentTime'> Current time. Doesn't have to be the real time. </param>
		/// <remarks>
		/// Notice that unlike the two public methods that receive an ITicked, 
		/// this one expects a TickedQueueItem.  It was done to avoid having to
		/// discard a TickedQueueItem instance every time that a looped item is
		/// ticked and re-added to the queue.  As such, it expects to already 
		/// have been configured for if to loop or not.
		/// </remarks>
		private void _add(TickedQueueItem item, float currentTime)
		{
			item.ResetTickFromTime(currentTime);
			var index = m_Queue.BinarySearch(item, TickedQueueItem.s_Comparer);
			
			//if the binary search doesn't find something identical, it'll return a
			//negative value signifying where the new item should reside, so bitflipping
			//that gives the new index
			if (index < 0) index = ~index;
			m_Queue.Insert(index, item);
		}
		
		/// <summary> Add the specified ticked object to the queue, using currentTime as the time to use for the tick check. </summary>
		/// <param name='ticked'> The ITicked object. </param>
		/// <param name='currentTime'> Current time. Doesn't have to be the real time. </param>
        private void _add(ITicked ticked, float currentTime)
		{
			_add(ticked, currentTime, m_LoopByDefault);
		}
		
		/// <summary> Add the specified ticked object to the queue, using currentTime as the time to use for the tick check. </summary>
		/// <param name='ticked'> The ITicked object. </param>
		/// <param name='currentTime'> Current time. Doesn't have to be the real time. </param>
		/// <param name='looped'> Sets whether the ticked item will be called once, or looped. </param>
        private void _add(ITicked ticked, float currentTime, bool looped)
		{
			var item = new TickedQueueItem(ticked, currentTime, looped);
			_add(item, currentTime);
		}
    }
}

