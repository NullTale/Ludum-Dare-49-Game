using System;
using System.Collections.Generic;
using System.Linq;
using CoreLib.Sound;
using NaughtyAttributes.Editor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CoreLib
{
    [CustomPropertyDrawer(typeof(AudioKeyAttribute))]
    public class AudioKeyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position = position.WithHeight(EditorGUIUtility.singleLineHeight);

            var soundManager = Object.FindObjectOfType<SoundManager>();

            // right clip play clip
            if (Event.current.type == EventType.MouseDown && position.Label().Contains(Event.current.mousePosition))
            {
                if (Event.current.button == 0)
                    _playSound(property.stringValue);
                else
                    Utils.StopAllClips();
            }

            Object provider = null;
            var    keyList  = soundManager?.GetAudioKeys().ToList();
            if (keyList != null && keyList.Contains(property.stringValue))
            {
                provider = soundManager.GetAudioContent().FirstOrDefault(n => n.key == property.stringValue).provider as Object;
                if (provider != null)
                {
                    property.isExpanded = EditorGUI.Foldout(position.WithHeight(EditorGUIUtility.singleLineHeight).WithWidth(5), property.isExpanded, GUIContent.none, toggleOnLabelClick: true);
                    provider.DrawObjectReference(property.isExpanded, position);
                }
            }
            
            StringKeyDrawer.StringKeyField(keyList, position, property, "Select an audio key",
                                           _playSound, _playSound, Utils.StopAllClips, () => EditorGUIUtility.PingObject(provider));

            // -----------------------------------------------------------------------
            static void _playSound(string key)
            {
                if (key.IsNullOrEmpty())
                    return;

                try
                {
                    Utils.StopAllClips();
                    var clip = Object.FindObjectOfType<SoundManager>()?.GetAudioContent().FirstOrDefault(n => n.key == key).provider.Audio?.Clip;
                    if (clip != null)
                        Utils.PlayClip(clip);
                }
                catch
                {
                    // ignored
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.isExpanded)
            {
                var provider = (Object.FindObjectOfType<SoundManager>()?.GetAudioContent().FirstOrDefault(n => n.key == property.stringValue).provider as Object);
                if (provider != null)
                    return provider.GetObjectReferenceHeight(true);
            }

            return EditorGUIUtility.singleLineHeight;
        }
    }
}