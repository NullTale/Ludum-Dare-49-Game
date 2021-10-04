using UnityEditor;
using UnityEngine;

namespace CoreLib
{
    [CustomEditor(typeof(RigidBodyHelper))]
    public class RigidBodyHelperEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var rbh = (RigidBodyHelper)target;
            if (rbh.TryGetComponent(out Rigidbody2D rb2D))
            {
                if (rbh.m_CenterOfMass == Vector3.positiveInfinity)
                    rbh.m_CenterOfMass = rb2D.centerOfMass.To3DXY();

                rbh.m_CenterOfMass = EditorGUILayout.Vector2Field("Center of mass", rbh.m_CenterOfMass.To2DXY()).To3DXY();

                if (EditorGUI.EndChangeCheck())
                    rb2D.centerOfMass = rbh.m_CenterOfMass.To2DXY();
            }
            else
            if (rbh.TryGetComponent(out Rigidbody rb3D))
            {
                if (rbh.m_CenterOfMass == Vector3.positiveInfinity)
                    rbh.m_CenterOfMass = rb3D.centerOfMass;

                rbh.m_CenterOfMass = EditorGUILayout.Vector3Field("Center of mass", rbh.m_CenterOfMass);

                if (EditorGUI.EndChangeCheck())
                    rb3D.centerOfMass = rbh.m_CenterOfMass;
            }
        }

        [MenuItem("CONTEXT/" + nameof(Rigidbody) + "/Reset Center Of Mass")]
        private static void ResetCenterOfMass3D(MenuCommand menuCommand)
        {
            if (menuCommand.context is Rigidbody rb)
                rb.ResetCenterOfMass();
        }
        
        [MenuItem("CONTEXT/" + nameof(Rigidbody2D) + "/Reset Center Of Mass")]
        private static void ResetCenterOfMass(MenuCommand menuCommand)
        {
            if (menuCommand.context is Rigidbody2D rb)
                Debug.Log($"Not implemented");
        }
    }
}