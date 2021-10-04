using UnityEngine;

namespace CoreLib.Sound
{
    [CreateAssetMenu(menuName = "Audio/Mixer Exposed Parameter", fileName = "MixerParameter", order = 0)]
    public class MixerExposedParameter : ScriptableObject
    {
        [AudioMixerParameter]
        public string m_Parameter;
    }
}