using UnityEngine;

namespace CoreLib.CameraVFX
{
    public class ScreenLine : MonoBehaviour, Module.CameraVFX.IScreenOverlay
    {
        public Color   m_Color;
        public Sprite  m_Sprite;
        public Vector2 m_Scale = new Vector2(1.0f, 1.0f);
        public Vector2          m_Offset = new Vector2(0.0f, 0.0f);
        public float            m_Angle;

        protected Module.CameraVFX.ScreenOverlayLineHandle m_Handle;

        public Color   Color    { get => m_Color;  set => m_Color = value; }
        public Sprite  Sprite   { get => m_Sprite; set => m_Sprite = value; }
        public Vector2 Scale    { get => m_Scale;  set => m_Scale = value; }
        public float   Angle    { get => m_Angle;  set => m_Angle = value; }
        public Vector2 Position { get => m_Offset; set => m_Offset = value; }

        //////////////////////////////////////////////////////////////////////////
        private void Awake()
        {
            m_Handle = new Module.CameraVFX.ScreenOverlayLineHandle();
        }

        protected virtual void OnEnable()
        {
            m_Handle.Open();
        }

        protected virtual void Update()
        {
            m_Handle.Color  = m_Color;
            m_Handle.Sprite = m_Sprite;
            m_Handle.Scale  = m_Scale;
            m_Handle.Angle  = m_Angle;
            m_Handle.Offset = m_Offset;
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