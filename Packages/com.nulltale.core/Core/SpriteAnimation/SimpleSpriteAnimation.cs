using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CoreLib.SpriteAnimation
{
    public class SimpleSpriteAnimation : MonoBehaviour
    {
        public  TimeMode                       m_TimeMode;
        public  float                          m_FramesPerSec = 10;
        public  List<Sprite>                   m_Animation;
        private float                          m_StartTime;
        private SpriteAnimationPlayer.IAdapter m_Adapter;

        // =======================================================================
        [Serializable]
        public enum TimeMode
        {
            Individual,
            Discreet,
            Sync
        }

        // =======================================================================
        private void Start()
        {
            if (TryGetComponent<SpriteRenderer>(out var sr))
                m_Adapter = new SpriteAnimationPlayer.SpriteAdapter(sr);
            else
            if (TryGetComponent<Image>(out var im))
                m_Adapter = new SpriteAnimationPlayer.ImageAdapter(im);
        }

        private void OnEnable()
        {
            switch (m_TimeMode)
            {
                case TimeMode.Individual:
                    m_StartTime = Time.time;
                    break;
                case TimeMode.Discreet:
                    m_StartTime = Time.time - Time.time % (1f / m_FramesPerSec);
                    break;
                case TimeMode.Sync:
                    m_StartTime = Time.time - Time.time % (m_FramesPerSec);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Update()
        {
            int frame = Mathf.CeilToInt((Time.time - m_StartTime) * m_FramesPerSec) % m_Animation.Count;
            m_Adapter.Set(m_Animation[frame]);
        }
    }
}