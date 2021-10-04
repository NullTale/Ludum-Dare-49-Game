using UnityEngine;

namespace CoreLib.CameraVFX
{
    public class ScreenOverlay : MonoBehaviour, Module.CameraVFX.IScreenOverlay
    {
        public Color            m_Color;
        public Sprite           m_Sprite;
        public Vector2          m_Scale = new Vector2(1.0f, 1.0f);

        protected Module.CameraVFX.ScreenOverlayHandle m_Handle;

        public Color   Color  { get => m_Color; set => m_Color = value; }
        public Sprite  Sprite { get => m_Sprite; set => m_Sprite = value; }
        public Vector2 Scale  { get => m_Scale; set => m_Scale = value; }

        //////////////////////////////////////////////////////////////////////////
        private void Awake()
        {
            m_Handle = new Module.CameraVFX.ScreenOverlayHandle();
        }

        protected virtual void OnEnable()
        {
            m_Handle.Open();
        }

        protected virtual void Update()
        {
            m_Handle.Color = m_Color;
            m_Handle.Sprite = m_Sprite;
            m_Handle.Scale = m_Scale;
        }

        protected virtual void OnDisable()
        {
            m_Handle.Close();
        }

        private void OnDestroy()
        {
            m_Handle?.Dispose();
        }
    }
}