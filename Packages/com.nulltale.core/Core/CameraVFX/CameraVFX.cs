using System;
using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using CoreLib.CameraVFX;
using CoreLib.States;
using UnityEngine;
using Image = UnityEngine.UI.Image;

namespace CoreLib.Module
{
    [CreateAssetMenu(fileName = nameof(CameraVFX), menuName = Core.k_CoreModuleMenu + nameof(CameraVFX))]
    public class CameraVFX : Core.Module<CameraVFX>
    {
        public Color             m_FadeColor = Color.black;
        public AnimationCurve    m_FadeIn = new AnimationCurve(new Keyframe(0.0f, 1.0f), new Keyframe(1.0f, 0.0f));
        public AnimationCurve    m_FadeOut = new AnimationCurve(new Keyframe(0.0f, 0.0f), new Keyframe(1.0f, 1.0f));

        public State             m_FadeState;

        public Color             m_FlashColor = Color.white;
        public AnimationCurve    m_Flash = new AnimationCurve(new Keyframe(0.0f, 1.0f), new Keyframe(0.2f, 0.0f));

        [Tooltip("Main Camera must contain CinemachineImpulseSource Component")]
        public float             m_ImpulseForce;
        public NoiseSettings     m_ShakeNoise;

        // =======================================================================
        public interface IScreenOverlay
        {
            Color   Color { get; set; }
            Sprite  Sprite { get; set; }
            Vector2 Scale { get; set; }
        }

        private interface IScreenLine : IScreenOverlay
        {
            float Angle { get; set; }
            Vector2 Offset { get; set; }
        }

        public class ScreenOverlayHandle : IScreenOverlay, IDisposable
        {
            protected Image         m_Image;
            protected RectTransform m_Transform;
            protected Canvas        m_Canvas;

            public Color   Color
            {
                get => m_Image.color;
                set
                {
                    if (m_Image.color != value)
                        m_Image.color = value;
                }
            }

            public Sprite  Sprite
            {
                get => m_Image.sprite;
                set
                {
                    if (m_Image.sprite != value)
                        m_Image.sprite = value;
                }
            }

            public Vector2 Scale
            {
                get => m_Transform.localScale.To2DXY();
                set
                {
                    if (m_Transform.localScale.To2DXY() != value)
                        m_Transform.localScale = value.To3DXY(1.0f);
                }
            }

            // =======================================================================
            public ScreenOverlayHandle()
            {
                // instantiate canvas & image
                m_Canvas = new GameObject("SO", typeof(Canvas)).GetComponent<Canvas>();
                m_Canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
                m_Canvas.sortingOrder = 10000;
                // 
                m_Canvas.transform.SetParent(Core.Instance.transform);

                var imgGO = new GameObject("Image", typeof(CanvasRenderer), typeof(Image));
                imgGO.transform.SetParent(m_Canvas.transform);
                
                m_Image = imgGO.GetComponent<Image>();
                m_Image.raycastTarget = false;
                m_Image.maskable = false;

                m_Transform = m_Image.GetComponent<RectTransform>();
                m_Transform.anchorMax = new Vector2(1.0f, 1.0f);
                m_Transform.anchorMin = new Vector2(0.0f, 0.0f);

                m_Transform.offsetMin = new Vector2(0.0f, 0.0f);
                m_Transform.offsetMax = new Vector2(0.0f, 0.0f);

                m_Canvas.gameObject.SetActive(false);
            }

            public void Open()
            {
                m_Canvas.gameObject.SetActive(true);
            }

            public void Close()
            {
                m_Canvas.gameObject.SetActive(false);
            }

            public void Dispose()
            {
                if (m_Canvas != null && m_Canvas.gameObject != null)
                {
#if UNITY_EDITOR
                    if (Application.isPlaying == false)
                        DestroyImmediate(m_Canvas.gameObject);
                    else
                        Destroy(m_Canvas.gameObject);
#else
                    Destroy(m_Canvas.gameObject);
#endif
                }
            }
        }

        public class ScreenOverlayLineHandle : ScreenOverlayHandle, IScreenLine
        {
            private float   m_Angle;
            private Vector2 m_Offset;

            public  float Angle
            {
                get => m_Angle;
                set
                {
                    m_Angle              = value;
                    m_Transform.rotation = Quaternion.AngleAxis(m_Angle, Vector3.forward);
                }
            }

            public Vector2 Offset
            {
                get => m_Offset;
                set
                {
                    if (m_Offset == value)
                        return;

                    m_Offset = value;
                    m_Transform.anchoredPosition = m_Offset * new Vector2(Screen.width, Screen.height);
                }
            }

            // =======================================================================
            public ScreenOverlayLineHandle()
            {
                m_Transform.anchorMax = new Vector2(0.5f, 0.5f);
                m_Transform.anchorMin = new Vector2(0.5f, 0.5f);

                m_Transform.sizeDelta = m_Canvas.GetComponent<RectTransform>().sizeDelta;
                m_Transform.anchoredPosition = m_Offset * new Vector2(Screen.width, Screen.height);
            }
        }

        internal class NoiseEffect : MonoBehaviour
        {
            private CinemachineVirtualCamera           m_Camera;
            private List<INoisehandle>                 m_NoiseHandles = new List<INoisehandle>();
            private CinemachineBasicMultiChannelPerlin m_Noise;

            public CinemachineVirtualCamera Camera => m_Camera;

            // =======================================================================
            private class NoiseHandle : INoisehandle
            {
                private float       m_Amplitude;
                private float       m_Freaquency;
                private float       m_Duration;
                private float       m_StartTime;
                private NoiseEffect m_NoiseEffect;

                NoiseEffect INoisehandle.NoiseEffect
                {
                    set => m_NoiseEffect = value;
                }

                public bool              IsExpired   => Time.time - m_StartTime >= m_Duration;
                public float             Amplitude   => m_Amplitude;
                public float             Frequency   => m_Freaquency;

                public NoiseHandle(float amplitude, float freaquency, float duration)
                {
                    m_Freaquency = freaquency;
                    m_Amplitude = amplitude;
                    m_Duration  = duration;

                    m_StartTime = Time.time;
                }
            }

            // =======================================================================
            private void Awake()
            {
                m_Camera = GetComponent<CinemachineVirtualCamera>();
                m_Noise = m_Camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                m_Noise.m_NoiseProfile = Instance.m_ShakeNoise;
            }

            public void Update()
            {
                if (m_NoiseHandles.Count == 0)
                {
                    m_Noise.m_NoiseProfile = null;
                    Destroy(this);
                    return;
                }

                var mainHandle = m_NoiseHandles.MaxBy(n => n.Amplitude);
                m_Noise.m_AmplitudeGain = mainHandle.Amplitude;
                m_Noise.m_FrequencyGain = mainHandle.Frequency;
                m_NoiseHandles.RemoveAll(n => n.IsExpired);
            }

            public void Push(float amplitude, float freaquency, float duration)
            {
                Push(new NoiseHandle(amplitude, freaquency, duration));
            }

            public void Push(INoisehandle noisehandle)
            {
                noisehandle.NoiseEffect = this;
                m_NoiseHandles.Add(noisehandle);
            }

            public void Remove(INoisehandle noisehandle)
            {
                m_NoiseHandles.Remove(noisehandle);
            }
        }
        
        public interface INoisehandle
        {
            internal NoiseEffect NoiseEffect { set; }
            bool  IsExpired { get; }
            float Amplitude { get; }
            float Frequency { get; }
        }

        public class NoiseHandle : INoisehandle, IDisposable
        {
            private NoiseEffect m_NoiseEffect;

            NoiseEffect INoisehandle.NoiseEffect
            {
                set => m_NoiseEffect = value;
            }

            public bool              IsExpired   { get; set; }
            public float             Amplitude   { get; set; }
            public float             Frequency   { get; set; }

            public void Dispose()
            {
                IsExpired = true;
            }

            public void ValidateCamera()
            {
                var camera = ((CinemachineVirtualCamera)Core.Instance.CameraBrain.ActiveVirtualCamera);
                if (m_NoiseEffect.Camera != camera)
                {
                    m_NoiseEffect.Remove(this);
                    _getNoiseEffect().Push(this);
                }
            }
        }

        // =======================================================================
        public override void Init()
        {
        }

        public static void CreateScreenOverlayEffect(Color color, AnimationCurve fade, Sprite sprite = null, State state = null)
        {
            var go = new GameObject("SOE");
            go.SetActive(false);

            var sof = go.AddComponent<ScreenOverlayEffect>();

            sof.m_OnComplete = ScreenOverlayEffect.OnComplete.Destroy;
            sof.m_Fade = fade;
            sof.m_Color = color;
            sof.m_Sprite = sprite;

            
            if (state != null)
            {
                state.Open();
                sof.gameObject.AddComponent<OnDestroyCallback>().Action = state.Close;
            }

            go.SetActive(true);
        }

        [Button]
        public static void FadeIn()
        {
            CreateScreenOverlayEffect(Instance.m_FadeColor, Instance.m_FadeIn, null, Instance.m_FadeState);
        }
        
        [Button]
        public static void FadeOut()
        {
            CreateScreenOverlayEffect(Instance.m_FadeColor, Instance.m_FadeOut, null, Instance.m_FadeState);
        }

        [Button]
        public static void Flash()
        {
            CreateScreenOverlayEffect(Instance.m_FlashColor, Instance.m_Flash, null, null);
        }
        
        public static void Flash(Color color)
        {
            CreateScreenOverlayEffect(color, Instance.m_Flash, null, null);
        }

        [Button]
        public static void GenerateImpulse()
        {
            Core.Instance.Camera.GetComponent<CinemachineImpulseSource>().GenerateImpulse(Instance.m_ImpulseForce);
        }

        public static NoiseHandle Shake()
        {
            var noiseHandle = new NoiseHandle();
            _getNoiseEffect().Push(noiseHandle);
            return noiseHandle;
        }

        public static void Shake(float amplitude, float freaquency, float duration)
        {
            _getNoiseEffect().Push(amplitude, freaquency, duration);
        }

        private static NoiseEffect _getNoiseEffect()
        {
            var targetCam = (CinemachineVirtualCamera)Core.Instance.CameraBrain.ActiveVirtualCamera;
            if (targetCam == null)
                return null;

            var noise = targetCam.GetComponent<NoiseEffect>();
            if (noise == null)
                noise = targetCam.gameObject.AddComponent<NoiseEffect>();

            return noise;
        }
    }

}