using UnityEngine;

namespace CoreLib.PlayerData
{
    public class PlayerDataString : PlayerData.DataHandle
    {
        [SerializeField]
        private string m_Value;
        [SerializeField]
        private string m_Default;

        public string Value
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
            m_Value = data;
        }

        public override string Serialize()
        {
            return m_Value;
        }
    }
}