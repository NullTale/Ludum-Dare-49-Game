using System;
using NaughtyAttributes;
using UnityEngine;

namespace CoreLib
{
    public class OrientedSprite : MonoBehaviour, IOrientedSprite
    {
        [SerializeField]
        protected OrientationTarget m_Mode = OrientationTarget.MainCamera;

        [SerializeField] [ShowIf(nameof(m_Mode), OrientationTarget.Target)]
        protected Transform m_Target;

        protected Transform m_CurrentTarget;

        [Space] [SerializeField] [ReadOnly]
        protected float m_Rotation;

        protected float m_Orientation;

        public OrientationTarget    Target
        {
            get => m_Mode;
            set
            { 
                if (m_Mode == value)
                    return;

                // apply mode
                m_Mode = value;
                _applyMode();
            }
        }

        public float Orientation => m_Orientation;

        // =======================================================================
        [Serializable]
        public enum OrientationTarget
        {
            MainCamera = 1,
            Target,
        }

        // =======================================================================
        private void Awake()
        {
            _applyMode();
        }

        private void OnWillRenderObject()
        {
            // update values
            var toTarget = (m_CurrentTarget.position - transform.position);
            m_Orientation     = Mathf.Atan2(toTarget.z, -toTarget.x) + Core.Math.PIHalf;
		
            m_Rotation = m_Orientation * Mathf.Rad2Deg;
        
            // set rotation to target
            transform.rotation = Quaternion.AngleAxis(m_Rotation, transform.up);
        }

        // =======================================================================
        private void _applyMode()
        {
            switch (m_Mode)
            {
                case OrientationTarget.MainCamera:
                    m_CurrentTarget = Core.Instance.Camera.transform;
                    break;
                case OrientationTarget.Target:
                    m_CurrentTarget = m_Target;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(m_Mode), m_Mode, null);
            }
        }
    }
}