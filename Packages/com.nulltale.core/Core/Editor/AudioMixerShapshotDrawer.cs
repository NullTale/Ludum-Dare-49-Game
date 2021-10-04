using System.Collections.Generic;
using System.Linq;
using CoreLib.Sound;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace CoreLib
{
    [CustomPropertyDrawer(typeof(AudioMixerShapshotAttribute))]
    public class AudioMixerShapshotDrawer : StringKeyDrawer
    {
        protected override List<string> _getKeyList()
        {
            var result = new List<string>();

            var mixer = Object.FindObjectOfType<SoundManager>()?.Mixer;
            if (mixer == null)
                return result;

            return ((AudioMixerSnapshot[])mixer.GetType().GetProperty("snapshots")?.GetValue(mixer, null))?.Select(n => n.name).ToList();
        }

        protected override string _getSelectionWindowLabel() => "Select a snapshot";
    }
}