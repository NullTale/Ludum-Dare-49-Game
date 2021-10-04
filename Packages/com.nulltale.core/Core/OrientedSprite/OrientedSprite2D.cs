using System;
using UnityEngine;

namespace CoreLib
{
    public class OrientedSprite2D : MonoBehaviour, IOrientedSprite
    {
        public Animator m_Animator;
        
        [Tooltip("Animator rotation in radians")]
        public float m_Orientation;

        public float LookDegree
        {
            set
            {
                m_Orientation = value;
                _setAnimatorValues();
            }
        }
        public Vector3 LookAt
        {
            set
            {
                m_Orientation = Mathf.Atan2(value.y, -value.x);
                _setAnimatorValues();
            }
        }
	
        public float Orientation => m_Orientation;

        //////////////////////////////////////////////////////////////////////////
        [Serializable]
        public enum AxisVector
        {
            X,
            Y,
            Z,
            TargetY,
            Custom,
        }

        //////////////////////////////////////////////////////////////////////////
        private void OnValidate()
        {
            _setAnimatorValues();
        }

        public void _setAnimatorValues()
        {
            m_Animator.SetFloat(OrientedSpriteAnimator.k_DirectionX, Mathf.Cos(m_Orientation));
            m_Animator.SetFloat(OrientedSpriteAnimator.k_DirectionY, Mathf.Sin(m_Orientation));
        }
    }
}