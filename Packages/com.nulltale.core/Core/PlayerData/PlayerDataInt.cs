using UnityEngine;

namespace CoreLib.PlayerData
{
    public class PlayerDataInt : PlayerData.DataHandle
    {
        [SerializeField]
        private int m_Value;
        [SerializeField]
        private int m_Default;

        public int Value
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
            m_Value = int.Parse(data);
        }

        public override string Serialize()
        {
            return m_Value.ToString();
        }
    }
}