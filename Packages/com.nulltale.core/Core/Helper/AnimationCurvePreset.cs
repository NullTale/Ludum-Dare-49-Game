using UnityEngine;

namespace CoreLib
{
    public class AnimationCurvePreset : ScriptableObject
    {
        [SerializeField]
        private float          m_Duration = 1f;
        [SerializeField]
        private AnimationCurve m_Curve;

        public float          Duration => m_Duration;
        public AnimationCurve Curve    => m_Curve;
    }
}