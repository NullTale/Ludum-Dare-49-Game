using System;
using UnityEngine;

namespace CoreLib.CameraVFX
{
    public class ScreenLineEffect : ScreenLine
    {
        public  AnimationCurve m_Move;
        public  OnComplete     m_OnComplete = OnComplete.Disable;
        private float          m_CurrentTime;
        private float          m_Duration;

        //////////////////////////////////////////////////////////////////////////
        [Serializable]
        public enum OnComplete
        {
            Nothing,
            Disable,
            Destroy,
        }

        //////////////////////////////////////////////////////////////////////////
        protected override void OnEnable()
        {
            m_Handle.Offset = (Mathf.Deg2Rad * (m_Angle + 90.0f)).Normal() * m_Move.Evaluate(m_CurrentTime);
            m_Handle.Sprite = m_Sprite;
            m_Handle.Scale  = m_Scale;

            m_CurrentTime = 0.0f;
            m_Duration    = m_Move.Duration();

            m_Handle.Open();
        }

        protected override void Update()
        {
            m_Handle.Offset = (Mathf.Deg2Rad * (m_Angle + 90.0f)).Normal() * m_Move.Evaluate(m_CurrentTime);
            m_Handle.Sprite = m_Sprite;
            m_Handle.Scale  = m_Scale;
            m_Handle.Angle  = m_Angle;
            
            if (m_CurrentTime >= m_Duration)
                switch (m_OnComplete)
                {
                    case OnComplete.Disable:
                        gameObject.SetActive(false);
                        break;
                    case OnComplete.Destroy:
                        Destroy(gameObject);
                        break;
                    case OnComplete.Nothing:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            m_CurrentTime += Time.deltaTime;
        }
    }
}