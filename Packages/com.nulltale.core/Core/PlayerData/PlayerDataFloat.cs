using System.Globalization;
using UnityEngine;

namespace CoreLib.PlayerData
{
    public class PlayerDataFloat : PlayerData.DataHandle
    {
        [SerializeField]
        private float m_Value;
        [SerializeField]
        private float m_Default;

        public float Value
        {
            get => m_Value;
            set => m_Value = value;
        }

        // =======================================================================
        public override void InitDefault()
        {
            m_Value = m_Default;
        }

        public override void Init(string data)
        {
            m_Value = float.Parse(data);
        }

        public override string Serialize()
        {
            return m_Value.ToString(CultureInfo.InvariantCulture);
        }
    }
}