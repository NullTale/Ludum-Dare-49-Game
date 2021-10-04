using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CoreLib.Module
{
    public class TimeController : MonoBehaviour
    {
        private const float k_MaxGameSpeed = 10.0f;
        private const float k_MinGameSpeed = 0.0f;

        [SerializeField, Range(k_MinGameSpeed, k_MaxGameSpeed)]
        private float       m_GameSpeed;
        private float       GameSpeed
        {
            set
            {
                // save & apply value
                m_GameSpeed = value;
                TimeControl.SetGameSpeed(m_GameSpeed);
            }
        }

        [NonSerialized]
        public bool         m_EnableKeyControls;

        [Header("Current time")]
        [ReadOnly, SerializeField]
        private float        m_TimeScale;
        [ReadOnly, SerializeField]
        private float        m_FixedDeltaTime;

        //////////////////////////////////////////////////////////////////////////
        public void Init(float gameSpeed, bool enableControls)
        {
            // save & apply value on start
            TimeControl.SetGameSpeed(m_GameSpeed = gameSpeed);
            
            m_EnableKeyControls = enableControls;
        }

        public void OnValidate()
        {
            TimeControl.SetGameSpeed(m_GameSpeed);
        }

        public void Update()
        {
            m_TimeScale = Time.timeScale;
            m_FixedDeltaTime = Time.fixedDeltaTime;

            // run if enabled
            if (m_EnableKeyControls == false)
                return;

            // listen keys
            if (Keyboard.current.numpadPlusKey.isPressed)
                GameSpeed = Mathf.Clamp(m_GameSpeed + 1.0f * Time.unscaledDeltaTime, k_MinGameSpeed, k_MaxGameSpeed);
            else
            if (Keyboard.current.numpadMinusKey.isPressed)
                GameSpeed = Mathf.Clamp(m_GameSpeed - 1.0f * Time.unscaledDeltaTime, k_MinGameSpeed, k_MaxGameSpeed);
        }
    }
}