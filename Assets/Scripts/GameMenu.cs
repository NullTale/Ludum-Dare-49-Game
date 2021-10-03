using CoreLib;
using CoreLib.Sound;
using UnityEngine;

namespace Game
{
    public class GameMenu : MonoBehaviour
    {
        [AudioKey]
        public string m_Sound;

        public bool          m_IsOn;
        public DirectorState m_State;

        // =======================================================================
        public void Toggle()
        {
            SoundManager.Instance.Play(in m_Sound);
            m_IsOn              = !m_IsOn;
            m_State.DesiredTime = m_IsOn ? 1d : 0d;
        }

        public void Show()
        {
            m_IsOn = true;
            m_State.DesiredTime = 1d;
        }
    }
}