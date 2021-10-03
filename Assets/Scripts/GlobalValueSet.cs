using CoreLib;
using UnityEngine;

namespace Game
{
    public class GlobalValueSet : MonoBehaviour
    {
        public float            m_Value;
        public GlobalValueFloat m_GlobalValue;

        // =======================================================================
        private void Update()
        {
            if (m_GlobalValue.Value != m_Value)
                m_GlobalValue.Value = m_Value;
        }
    }
}