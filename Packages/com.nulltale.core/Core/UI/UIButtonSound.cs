using CoreLib.Sound;
using UnityEngine;
using UnityEngine.UI;

namespace CoreLib
{
    [RequireComponent(typeof(AudioPlayer))]
    public class UIButtonSound : MonoBehaviour
    {
        [AudioKey]
        public string m_Sound;

        // =======================================================================
        private void Awake()
        {
            GetComponentInParent<Button>().onClick.AddListener(() => GetComponent<AudioPlayer>().Play(m_Sound));
        }
    }
}