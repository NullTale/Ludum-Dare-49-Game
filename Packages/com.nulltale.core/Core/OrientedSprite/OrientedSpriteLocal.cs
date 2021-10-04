using UnityEngine;

namespace CoreLib
{
    public class OrientedSpriteLocal : OrientedSprite
    {
        // =======================================================================
        private void OnWillRenderObject()
        {
            // update values
            var toTarget = transform.parent.worldToLocalMatrix.MultiplyPoint3x4(m_CurrentTarget.position) - transform.localPosition;
            m_Orientation = Mathf.Atan2(toTarget.z, -toTarget.x) + Core.Math.PIHalf;
		
            m_Rotation = m_Orientation * Mathf.Rad2Deg;
        
            // set rotation to target
            transform.localRotation = Quaternion.AngleAxis(m_Rotation, Vector3.up);
        }
    }
}