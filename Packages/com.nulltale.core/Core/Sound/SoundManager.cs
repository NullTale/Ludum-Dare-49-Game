using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Malee;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEventBus;

namespace CoreLib.Sound
{
    [DefaultExecutionOrder(Core.k_ManagerDefaultExecutionOrder)]
    public class SoundManager : MonoBehaviour, Core.IModule, IAudioContext
    {
        public class MixerData
        {
            public AudioMixerGroup[]        AudioMixerGroups;
            public AudioMixerSnapshot[]     AudioSnapshots;
            public List<string>             ExposedParams;
            public List<string>[]           GroupEffects;

            // =======================================================================
            public void SyncToMixer(AudioMixer mixer)
            {
                Debug.Log("----Syncing to Mixer---------------------------------------------------------------------");
                //Fetch all audio groups under MASTER
                AudioMixerGroups = mixer.FindMatchingGroups("Master");

                GroupEffects = new List<string>[AudioMixerGroups.Length];
                for (var x = 0; x < AudioMixerGroups.Length; x++)
                {
                    Debug.Log("AudioGroup " + AudioMixerGroups[x].name + "---------------------------");
                    Debug.Log("----Effects----");
                    GroupEffects[x] = new List<string>();
                    var effects = (Array)AudioMixerGroups[x].GetType().GetProperty("effects")
                                                            .GetValue(AudioMixerGroups[x], null);
                    for (var i = 0; i < effects.Length; i++)
                    {
                        var o      = effects.GetValue(i);
                        var effect = (string)o.GetType().GetProperty("effectName").GetValue(o, null);
                        GroupEffects[x].Add(effect);
                        Debug.Log(effect);
                    }
                }

                //Exposed Params
                Array parameters = (Array)mixer.GetType().GetProperty("exposedParameters").GetValue(mixer, null);

                Debug.Log("----ExposedParams----------------------------------------------------");
                for (var i = 0; i < parameters.Length; i++)
                {
                    var    o     = parameters.GetValue(i);
                    var Param = (string)o.GetType().GetField("name").GetValue(o);
                    ExposedParams.Add(Param);
                    Debug.Log(Param);
                }

                //Snapshots
                AudioSnapshots = (AudioMixerSnapshot[])mixer.GetType().GetProperty("snapshots").GetValue(mixer, null);

                Debug.Log("----Snapshots----------------------------------------------------");
                for (var i = 0; i < AudioSnapshots.Length; i++)
                {
                    Debug.Log(AudioSnapshots[i].name);
                }
            }
        }

        public abstract class AudioDatabase : ScriptableObject
        {
            [SerializeField]
            private string                  m_Prefix;
            public string                   Prefix => m_Prefix;

            public abstract IEnumerable<AudioProvider> GetAudioProviders();
        }

        // =======================================================================
        private static SoundManager s_Instance;
        public static SoundManager Instance
        {
            get
            {
#if UNITY_EDITOR
                if (Application.isPlaying == false || s_Instance == null)
                    s_Instance = FindObjectOfType<SoundManager>();
#endif

                return s_Instance;
            }
            private set => s_Instance = value;
        }

        [SerializeField]
        private AudioMixer m_Mixer;
        public AudioMixer Mixer => m_Mixer;

        private Dictionary<string, AudioProvider>      m_AudioData;

        [SerializeField]
        private AudioChannel m_Music;
        public AudioChannel Music => m_Music;
        [SerializeField]
        private AudioChannel m_Ambient;
        public AudioChannel Ambient => m_Ambient;
        [SerializeField]
        private SoundChannel m_Sound;
        public SoundChannel Sound => m_Sound;

        public AudioSource AudioSource => m_Sound.AudioSource;

        [SerializeField]
        [Reorderable(labels = false)]
        private ReorderableArray<AudioDatabase>         m_AudioDatabases;

        // =======================================================================
        public void Init()
        {
            // set as main instance
            Instance = this;

            // allocate audio dictionary
            m_AudioData = new Dictionary<string, AudioProvider>();

            // move audio sources to the main camera
            m_Music.transform.SetParent(Core.Instance.Camera.transform, false);
            m_Ambient.transform.SetParent(Core.Instance.Camera.transform, false);
            m_Sound.transform.SetParent(Core.Instance.Camera.transform, false);

            // add from linked databases
            foreach (var audioDatabase in m_AudioDatabases)
                if (audioDatabase != null)
                    addDatabase(audioDatabase.Prefix, audioDatabase.GetAudioProviders());

            // -----------------------------------------------------------------------
            void addDatabase(string prefix,  IEnumerable<AudioProvider> audioProviders)
            {
                if (prefix.IsNullOrWhiteSpace())
                    foreach (var provider in audioProviders)
                    {
                        if (provider == null)
                            continue;

                        m_AudioData.Add(provider.Key, provider);
                        provider.Init();
                    }
                else
                    foreach (var provider in audioProviders)
                    {
                        if (provider == null)
                            continue;

                        m_AudioData.Add($"{prefix}{provider.Key}", provider);
                        provider.Init();
                    }

            }
        }

        public IAudio GetAudio(in string audioKey)
        {
            if (m_AudioData.TryGetValue(audioKey, out var audioProvider))
                return audioProvider.Audio;

            return null;
        }

        public AudioProvider GetAudioProvider(in string audioKey)
        {
            if (m_AudioData.TryGetValue(audioKey, out var audioProvider))
                return audioProvider;

            return null;
        }

        public void Play(in string audioKey)
        {
            var audio = SoundManager.Instance.GetAudio(in audioKey);
            audio.Play(this);
        }

        public IEnumerable<string> GetAudioKeys()
        {
            foreach (var audioDatabase in m_AudioDatabases)
            foreach (var container in audioDatabase.GetAudioProviders())
                yield return audioDatabase.Prefix + container.Key;
        }

        public IEnumerable<(string key, IAudioProvider provider)> GetAudioContent()
        {
            foreach (var audioDatabase in m_AudioDatabases)
            foreach (var container in audioDatabase.GetAudioProviders())
                yield return (audioDatabase.Prefix + container.Key, container);
        }

        // -----------------------------------------------------------------------
        internal static IEnumerator TransitionCoroutine(AudioSource audioSource, AudioClip clip, float volume, AnimationCurve leave, AnimationCurve enter, float time = 0f)
        {
            // run leave transition, if playing
            if (audioSource.isPlaying)
                yield return volumeCurve(leave, audioSource.volume);

            // run enter transition, if clip not null
            audioSource.clip = clip;
            if (clip != null)
            {
                audioSource.time = time;
                audioSource.Play();
                yield return volumeCurve(enter, volume);
            }
            else
                audioSource.Stop();


            // -----------------------------------------------------------------------
            IEnumerator volumeCurve(AnimationCurve curve, float desiredVolume)
            {
                var lastKey = curve.keys.Last();
                var duration = lastKey.time;
                for (var currentTime = 0f; currentTime < duration; currentTime += Time.deltaTime)
                {
                    audioSource.volume = desiredVolume * curve.Evaluate(currentTime);
                    yield return null;
                }

                // apply last key
                audioSource.volume = desiredVolume * lastKey.value;
            }
        }
    }
}