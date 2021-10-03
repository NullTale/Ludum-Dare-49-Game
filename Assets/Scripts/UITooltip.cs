using System;
using CoreLib;
using CoreLib.Module;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GoalSystem.Hub
{
    [DefaultExecutionOrder(-100)]
    public class UITooltip : MonoBehaviour
    {
        public static UITooltip Instance;
        
        private TMP_Text m_Text;

        // =======================================================================
        private void Awake()
        {
            Instance = this;
            m_Text = GetComponentInChildren<TMP_Text>();

            _set(null);
        }

        public static void Set(string text)
        {
            Instance._set(text);
        }

        private void Update()
        {
            transform.position = PointerPosition.Instance.ScreenPosition.To3DXY();
        }

        // -----------------------------------------------------------------------
        private void _set(string text)
        {
            if (text.IsNullOrEmpty())
            {
                gameObject.SetActive(false);
                return;
            }

            m_Text.text = text;
            Update();
            gameObject.SetActive(true);
        }
    }
}