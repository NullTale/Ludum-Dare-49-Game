using System;
using UnityEngine;

namespace Game
{
    public class ForceRotation : MonoBehaviour
    {
        public Quaternion m_Quaternion;

        private void FixedUpdate()
        {
            transform.rotation = m_Quaternion;
        }
    }
}