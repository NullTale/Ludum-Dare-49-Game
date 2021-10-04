using System;

namespace CoreLib
{
    public class ValueMultiplier : IDisposable
    {
        public float Value { get; private set; }

        private float m_Multiplier = 1.0f;
        public float Multiplier
        {
            get => m_Multiplier;
            set
            {
                Value        /= m_Multiplier;
                m_Multiplier =  value;
                Value        *= m_Multiplier;
            }
        }

        // =======================================================================
        public ValueMultiplier(float value)
        {
            Value = value;
        }

        public void Dispose()
        {
            Multiplier = 1.0f;
        }
    }
}