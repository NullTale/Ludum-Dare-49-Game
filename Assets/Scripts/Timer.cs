using System.Linq;
using CoreLib;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game
{
    public class Timer : MonoBehaviour
    {
        private float     m_Initial;
        public  Image     m_Progress;
        public  TinyTimer m_Duration;

        public UnityEvent OnComplete;

        public  TMP_Text m_PigsCounter;
        private int      m_PigsLimit;
        public  float    m_Delay;
        private bool     m_Active;

        // =======================================================================
        private void Awake()
        {
            m_Initial = m_Progress.rectTransform.sizeDelta.x;

            var difficulty = SceneInitializer.Instance.m_Difficulty;
            m_Duration.FinishTime = difficulty.Time;
            m_PigsLimit = difficulty.MinPigs;

            this.Delayed(() => m_Active = true, m_Delay);
        }

        private void Update()
        {
            if (GamePreferences.Instance.Pause.IsOpen)
                return;

            m_PigsCounter.text = $"{GlobalList<Pig>.Content.Count(n => n.IsDead == false)}\\{m_PigsLimit}";

            m_Progress.rectTransform.sizeDelta = m_Progress.rectTransform.sizeDelta.WithX(Mathf.Lerp(m_Initial, 0f, m_Duration.Scale));
            m_Duration.AddDeltaTime();

            if (m_Duration.IsExpired)
                _complete();

            if (m_Active && GlobalList<Pig>.Content.Count(n => n.IsDead == false) < m_PigsLimit)
                _complete();
        }

        private void _complete()
        {
            OnComplete.Invoke();
            enabled = false;
        }
    }
}