using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace CoreLib.CameraVFX
{
    [RequireComponent(typeof(Volume))]
    public class PostProcessEffect : MonoBehaviour
    {
        public  AnimationCurve m_Weight;
        public  OnComplete     m_OnComplete;
        private float          m_CurrentTime;
        private float          m_Duration;
        private Volume         m_Volume;

        //////////////////////////////////////////////////////////////////////////
        [Serializable]
        public enum OnComplete
        {
            Disable,
            Destroy,
            None
        }

        //////////////////////////////////////////////////////////////////////////
        private void OnEnable()
        {
            m_Volume = GetComponent<Volume>();

            m_Volume.weight = m_Weight.Evaluate(0.0f);
            m_CurrentTime = 0.0f;
            m_Duration = m_Weight.Duration();
        }

        private void Update()
        {
            m_Volume.weight = m_Weight.Evaluate(m_CurrentTime);
            
            if (m_CurrentTime >= m_Duration)
                switch (m_OnComplete)
                {
                    case OnComplete.Disable:
                        gameObject.SetActive(false);
                        break;
                    case OnComplete.Destroy:
                        Destroy(gameObject);
                        break;
                    case OnComplete.None:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            m_CurrentTime += Time.deltaTime;
        }
    }
}