using UnityEngine;

namespace Game
{
    public class Rotation : MonoBehaviour
    {
        public float m_Speed;

        // =======================================================================
        private void Update()
        {
            transform.rotation *= Quaternion.AngleAxis(m_Speed * Time.deltaTime, Vector3.forward);
        }
    }
}