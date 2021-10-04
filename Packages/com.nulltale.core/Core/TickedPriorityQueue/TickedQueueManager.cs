using System;
using System.Collections.Generic;
using UnityEngine;

namespace CoreLib
{
    public class TickedQueueManager : MonoBehaviour
    {
        public static TickedQueueManager        Instance;

        [SerializeField]
        private List<TickedQueue> m_TimeUpdateGroup = new List<TickedQueue>();
        [SerializeField]
        private List<TickedQueue> m_UnscaledTimeUpdateGroup = new List<TickedQueue>();
        [SerializeField]
        private List<TickedQueue> m_FixedTimeUpdateGroup = new List<TickedQueue>();

        private float m_FixedTime;
        private Dictionary<string, TickedQueue>     m_TickedQueues = new Dictionary<string, TickedQueue>();

        public  IReadOnlyDictionary<string, TickedQueue> TickedQueues => m_TickedQueues;

        //////////////////////////////////////////////////////////////////////////
        private void Awake()
        {
            Instance = this;
        }

        public static float GetCurrentTime(TickedQueue.UpdateGroup group) => group switch
        {
            TickedQueue.UpdateGroup.Time         => Time.time,
            TickedQueue.UpdateGroup.UnscaledTime => Time.unscaledTime,
            TickedQueue.UpdateGroup.FixedTime    => Instance.m_FixedTime,
            _                                    => throw new ArgumentOutOfRangeException(nameof(@group), @group, null)
        };

        internal void Init(params TickedQueue[] queues)
        {
            m_TickedQueues.Clear();

            foreach (var tickedQueue in queues)
            {
                m_TickedQueues[tickedQueue.name] = tickedQueue;

                tickedQueue.Init();
                switch (tickedQueue.m_UpdateGroup)
                {
                    case TickedQueue.UpdateGroup.Time:
                        m_TimeUpdateGroup.Add(tickedQueue);
                        break;
                    case TickedQueue.UpdateGroup.UnscaledTime:
                        m_UnscaledTimeUpdateGroup.Add(tickedQueue);
                        break;
                    case TickedQueue.UpdateGroup.FixedTime:
                        m_FixedTimeUpdateGroup.Add(tickedQueue);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            m_FixedTime = 0.0f;
        }

        private void Update()
        {
            foreach (var tickedQueue in m_TimeUpdateGroup)
                tickedQueue.Update(Time.time);
            
            foreach (var tickedQueue in m_UnscaledTimeUpdateGroup)
                tickedQueue.Update(Time.unscaledTime);
        }

        private void FixedUpdate()
        {
            foreach (var tickedQueue in m_FixedTimeUpdateGroup)
                tickedQueue.Update(m_FixedTime);

            m_FixedTime += Time.fixedDeltaTime;
        }
    }
}