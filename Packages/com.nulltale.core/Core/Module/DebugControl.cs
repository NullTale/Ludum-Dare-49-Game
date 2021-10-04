using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;


namespace CoreLib.Module
{
    [CreateAssetMenu(fileName = nameof(DebugControl), menuName = Core.k_CoreModuleMenu + nameof(DebugControl))]
    public class DebugControl : Core.Module
    {
        [SerializeField]
        private Core.ProjectSpace   m_ProjectSpace;

        [SerializeField]
        private float               m_MouseScrollScale = 1.0f;
        [SerializeField]
        private float               m_MouseMoveScale = 0.06f;
        [SerializeField]
        private float               m_KeyboardMoveScale = 10.0f;

        private Vector2             m_MousePosLast;

	    private Vector3             m_ForvardVector;
	    private Vector3             m_RightVector;
	    private Vector3             m_UpVector;

        [SerializeField]
        private MouseButton         m_DragMouseButton;

        [SerializeField]
        private bool                m_EnableArrowKeysMovement = true;

        // =======================================================================
        [Serializable] [Flags]
        public enum MouseButton
        {
            None = 0,

            Left   = 1 << 1,
            Right  = 1 << 2,
            Middle = 1 << 3
        }

        // =======================================================================
        public override void Init()
	    {
            // init space
		    switch (m_ProjectSpace)
		    {
			    case Core.ProjectSpace.XY:
				    m_ForvardVector = Vector3.up;
				    m_RightVector = Vector3.right;
				    break;
			    case Core.ProjectSpace.XZ:
				    m_ForvardVector = Vector3.forward;
				    m_RightVector = Vector3.right;
				    break;
			    default:
				    m_ForvardVector = Vector3.up;
				    m_RightVector = Vector3.right;
				    break;
		    }

            // set up vector
		    m_UpVector = Vector3.Cross(m_ForvardVector, m_RightVector);

            // create updater
            Core.Instance.gameObject.AddComponent<OnUpdateCallback>().Action = _update;
	    }

	    private void _update()
	    {
            // arrow keys movement
		    if (m_EnableArrowKeysMovement)
		    {
			    var translateVector = Vector3.zero;

                // sum vectors, calculate move normal
				
                if (Keyboard.current.upArrowKey.isPressed)
				    translateVector += m_ForvardVector;
                if (Keyboard.current.downArrowKey.isPressed)
				    translateVector -= m_ForvardVector;
                if (Keyboard.current.leftArrowKey.isPressed)
				    translateVector -= m_RightVector;
                if (Keyboard.current.rightArrowKey.isPressed)
				    translateVector += m_RightVector;
			    
                if (Keyboard.current.rightShiftKey.isPressed)
				    translateVector += m_UpVector;
                if (Keyboard.current.rightCtrlKey.isPressed)
				    translateVector -= m_UpVector;

                // move by normal if has vector
			    if (translateVector != Vector3.zero)
			    {
				    translateVector.Normalize();
                    Camera.position += (translateVector * m_KeyboardMoveScale * Time.deltaTime);
                }
		    }

            // implement drag
		    if (m_DragMouseButton != MouseButton.None)
		    {
                var view = Core.Instance.Camera.ScreenToViewportPoint(Mouse.current.position.ReadValue());

			    if ((view.x < 0 || view.x > 1 || view.y < 0 || view.y > 1) == false)
			    {
                    if (m_DragMouseButton.HasFlag(MouseButton.Left) && Mouse.current.leftButton.isPressed
                     || m_DragMouseButton.HasFlag(MouseButton.Right) && Mouse.current.rightButton.isPressed
                     || m_DragMouseButton.HasFlag(MouseButton.Middle) && Mouse.current.middleButton.isPressed)
				    {
					    var offset = (m_MousePosLast - PointerPosition.Instance.ScreenPosition).To3DXY();
					    if (offset.magnitude < 40.0f)
						    switch (m_ProjectSpace)
						    {
							    case Core.ProjectSpace.XY:
								    Camera.position += ((offset * m_MouseMoveScale).WithZ(0.0f));
								    break;
							    case Core.ProjectSpace.XZ:
								    Camera.position += ((offset * m_MouseMoveScale).WithZ(0.0f)).XZY();
								    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
				    }

                    // implement scroll
                    var scrollImpact = Mouse.current.scroll.y.ReadValue() * m_MouseScrollScale;

					if (scrollImpact != 0.0f)
					    if (Core.Instance.Camera.orthographic)
						    Core.Instance.Camera.orthographicSize = Mathf.Clamp(Core.Instance.Camera.orthographicSize - scrollImpact, 1.0f, int.MaxValue);
					    else
					    {
						    switch (m_ProjectSpace)
						    {
							    case Core.ProjectSpace.XY:
								    Camera.transform.position += scrollImpact.ToVector3Z();
								    break;
							    case Core.ProjectSpace.XZ:
								    Camera.transform.position += scrollImpact.ToVector3Y();
								    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
					    }
                }

			    m_MousePosLast = PointerPosition.Instance.ScreenPosition;
		    }
	    }

        private Transform Camera
        {
            get
            {
				if (Core.Instance.Camera.TryGetComponent(out CinemachineBrain brain) == false)
                    return Core.Instance.Camera.transform;
				if (brain.ActiveVirtualCamera == CinemachineBrain.SoloCamera)
					return Core.Instance.Camera.transform;

				return ((MonoBehaviour)brain.ActiveVirtualCamera).transform;
            }
        }
    }
}