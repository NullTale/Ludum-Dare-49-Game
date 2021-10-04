using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.Playables;
using UnityEventBus;

namespace CoreLib
{
    public class Cutscene : MonoBehaviour
    {
        public PlayableDirector m_Director;
        public bool             m_SkipOnCancel = true;
        public bool             m_SkipOnSubmit = true;
        public bool             m_SkipOnClick;
        public List<InputActionReference>     m_SkipActions;
        public OnSkip           m_OnSkip;
        [ShowIf(nameof(m_OnSkip), OnSkip.Rewind)]
        public float    m_RewindSpeed = 2.5f;
        public OnCancel m_OnCancel            = OnCancel.Disable;

        private bool m_Complete;

        public UnityEvent m_OnSkipEvent;
        public UnityEvent m_OnCancelEvent;

        // =======================================================================
        [Serializable]
        public enum OnSkip
        {
            None,
            Skip,
            Rewind
        }

        [Serializable]
        public enum OnCancel
        {
            Nothing,
            Disable,
            DestroySelf
        }

        // =======================================================================
        private void OnEnable()
        {
            m_Complete = false;

            var inputModule = UnityEngine.EventSystems.EventSystem.current.GetComponent<InputSystemUIInputModule>();
            if (inputModule != null)
            {
                if (m_SkipOnCancel)
                    m_SkipActions.AddUnique(inputModule.cancel);
                if (m_SkipOnSubmit)
                    m_SkipActions.AddUnique(inputModule.submit);
                if (m_SkipOnClick)
                    m_SkipActions.AddUnique(inputModule.leftClick);
            }

            if (m_Director == null)
                m_Director = GetComponentInChildren<PlayableDirector>();

            m_Director.extrapolationMode = DirectorWrapMode.Hold;
            m_Director.time = 0f;

            // activate actions
            foreach (var actionReference in m_SkipActions)
                actionReference.action.performed += _onSkip;
        }

        private void OnDisable()
        {
            // deactivate actions
            foreach (var actionReference in m_SkipActions)
                actionReference.action.performed -= _onSkip;
        }

        /// <summary> Activates self game object </summary>
        public void Play()
        {
            gameObject.SetActive(true);
        }

        public void Stop()
        {
            gameObject.SetActive(false);
        }

        private void Update()
        {
            if (m_Complete)
                return;

            if (m_Director.time >= m_Director.duration)
            {
                m_Complete = true;

                m_OnCancelEvent.Invoke();

                switch (m_OnCancel)
                {
                    case OnCancel.Nothing:
                        break;
                    case OnCancel.DestroySelf:
                    {
                        Destroy(gameObject);
                    } break;
                    case OnCancel.Disable:
                    {
                        gameObject.SetActive(false);
                    } break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        // =======================================================================
        private void _onSkip(InputAction.CallbackContext content)
        {
            if (m_Complete)
                return;

            m_OnSkipEvent.Invoke();

            switch (m_OnSkip)
            {
                case OnSkip.None:
                    break;
                case OnSkip.Skip:
                {
                    m_Director.time = m_Director.duration;
                } break;
                case OnSkip.Rewind:
                {
                    if (m_Director.playableGraph.IsValid() == false)
                        m_Director.RebuildGraph();
                    m_Director.playableGraph.GetRootPlayable(0).SetSpeed(m_RewindSpeed);
                } break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}