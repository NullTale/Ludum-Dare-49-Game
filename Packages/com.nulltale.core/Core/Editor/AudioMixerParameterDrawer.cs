using System;
using System.Collections.Generic;
using CoreLib.Sound;
using UnityEditor;
using Object = UnityEngine.Object;

namespace CoreLib
{
    [CustomPropertyDrawer(typeof(AudioMixerParameterAttribute))]
    public class AudioMixerParameterDrawer : StringKeyDrawer
    {
        protected override List<string> _getKeyList()
        {
            var result = new List<string>();

            var mixer = Object.FindObjectOfType<SoundManager>()?.Mixer;
            if (mixer == null)
                return result;

            var parameters = (Array)mixer.GetType().GetProperty("exposedParameters")?.GetValue(mixer, null);
            if (parameters == null)
                return result;
   
            for (var n = 0; n < parameters.Length; n++)
            {
                var o  = parameters.GetValue(n);
                var param   = (string)o.GetType().GetField("name").GetValue(o);

                result.Add(param);
            }

            return result;
        }

        protected override string _getSelectionWindowLabel() => "Select a parameter";
    }
}