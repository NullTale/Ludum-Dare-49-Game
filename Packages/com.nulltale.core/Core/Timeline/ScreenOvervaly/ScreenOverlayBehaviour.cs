using System;
using UnityEngine;
using UnityEngine.Playables;

namespace CoreLib.Timeline
{
    [Serializable]
    public class ScreenOverlayBehaviour : PlayableBehaviour
    {
        public float  m_Scale = 1f;
        public Color  m_Color = Color.black;
        public Sprite m_Image;

    }
}