using CoreLib;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class Level : MonoBehaviour
    {
        public Level  m_Next;
        public Sprite m_Idle;
        public Sprite m_Complete;
        public Sprite m_Golden;

        public Image  m_Image;
        public Button m_Button;

        public bool IsOpen {get; private set; }

        public bool m_AlwaysOpen;

        // =======================================================================
        public void Awake()
        {
            if (GamePreferences.Instance.Data.m_Levels.TryGetValue(n => n.Name == name, out var data) == false)
            {
                data = new PlayerProgress.LevelData()
                {
                    Name     = name,
                    Complete = false,
                    Gold     = false,
                    Scores   = 0
                };

                GamePreferences.Instance.Data.m_Levels.Add(data);
            }

            if (data.Gold)
                m_Image.sprite = m_Golden;
            else
            if (data.Complete)
                m_Image.sprite = m_Complete;
            else
                m_Image.sprite = m_Idle;

            IsOpen                = m_Image.sprite != m_Idle;
            m_Button.interactable = IsOpen;

            if (m_AlwaysOpen && IsOpen == false)
                _forceToOpen();
        }

        private void Start()
        {
            if (IsOpen && m_Next != null && m_Next.IsOpen == false)
                m_Next._forceToOpen();
        }

        // -----------------------------------------------------------------------
        private void _forceToOpen()
        {
            m_Image.sprite        = m_Idle;
            m_Button.interactable = true;
        }
    }
}