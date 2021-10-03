using System;
using CoreLib;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class StressBar : MonoBehaviour
    {
        [NonSerialized]
        public Pig Owner;
        public Image     m_Progress;
        [Range(0, 1)]
        public float m_ScaleIndicationThreshod;
        private SmoothScale m_Scale;
        public ParticleSystem   m_Dirt;
        public float Value
        {
            set
            {
                m_Scale.enabled                 = value >= m_ScaleIndicationThreshod;
                m_Progress.transform.localScale = Vector3.one.WithX(value.Clamp01());
            }
        }

        private void Awake()
        {
            Owner = GetComponentInParent<Pig>();
            m_Scale = GetComponent<SmoothScale>();
            transform.SetParent(null, true);
        }


        private void OnEnable()
        {
            Update();
        }

        private void Update()
        {
            Value = Owner.m_Stress;
            transform.position = Owner.transform.position;
        }
    }
}