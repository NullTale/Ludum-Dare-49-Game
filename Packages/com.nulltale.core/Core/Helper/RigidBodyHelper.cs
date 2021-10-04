using UnityEngine;

namespace CoreLib
{
    public class RigidBodyHelper : MonoBehaviour
    {
        public Vector3      m_CenterOfMass = Vector3.positiveInfinity;

        //////////////////////////////////////////////////////////////////////////
        private void Awake()
        {
            if (TryGetComponent(out Rigidbody rb3D))
                rb3D.centerOfMass = m_CenterOfMass;
            else
            if (TryGetComponent(out Rigidbody2D rb2D))
                rb2D.centerOfMass = m_CenterOfMass.To2DXY();
        }

        private void OnDrawGizmos()
        {
            var centerOfMass = new Vector3?();
            if (TryGetComponent(out Rigidbody rb3D))
                centerOfMass = rb3D.centerOfMass;
            else
            if (TryGetComponent(out Rigidbody2D rb2D))
                centerOfMass = rb2D.centerOfMass.To3DXY();

            if (centerOfMass.HasValue)
                Gizmos.DrawIcon(transform.localToWorldMatrix.MultiplyPoint(centerOfMass.Value), "Animation.FilterBySelection");
        }
    }
}