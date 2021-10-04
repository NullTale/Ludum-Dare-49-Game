using System.Collections.Generic;
using UnityEngine;

namespace CoreLib.Sound
{
    [CreateAssetMenu(menuName = "Audio/Audio Database", fileName = "AudioDatabase", order = 0)]
    public class AudioDatabase : SoundManager.AudioDatabase
    {
        [SerializeField]
        private ScriptableObjectCollection<AudioProvider> m_AudioData;

        // =======================================================================
        public override IEnumerable<AudioProvider> GetAudioProviders()
        {
            return m_AudioData.Values;
        }
    }
}