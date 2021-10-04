using CoreLib.Sound;
using UnityEngine;
using UnityEngine.UI;

namespace CoreLib
{
    [RequireComponent(typeof(AudioPlayer))]
    public class UIToggleSound : MonoBehaviour
    {
        [AudioKey]
        public string m_Sound;

        // =======================================================================
        private void Awake()
        {
            GetComponentInParent<Toggle>().onValueChanged.AddListener(on => GetComponent<AudioPlayer>().Play(m_Sound));
        }
    }
}