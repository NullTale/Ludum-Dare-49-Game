using UnityEngine;

namespace CoreLib
{
    public class OrientedSpriteAnimator : MonoBehaviour
    {
        public static int k_DirectionX = Animator.StringToHash("X"); 
        public static int k_DirectionY = Animator.StringToHash("Y");

        [SerializeField] [Space]
        private Animator m_Animator;
        [SerializeField] [Tooltip("Animator rotation in radians")]
        private float m_Orientation;
        
        private IOrientedSprite m_OrientedSprite;

        public float OrientationRad
        {
            get => m_Orientation;
            set => m_Orientation = value;
        }

        public Vector3 AnimatorOrientationDirection
        {
            set => OrientationRad = value.To2DXZ().AngleRad();
        }

        //////////////////////////////////////////////////////////////////////////
        private void Awake()
        {
            m_OrientedSprite = GetComponent<IOrientedSprite>();
        }

        private void FixedUpdate()
        {
            m_Animator.SetFloat(k_DirectionX, Mathf.Cos(m_OrientedSprite.Orientation + m_Orientation));
            m_Animator.SetFloat(k_DirectionY, Mathf.Sin(m_OrientedSprite.Orientation + m_Orientation));
        }
    }
}