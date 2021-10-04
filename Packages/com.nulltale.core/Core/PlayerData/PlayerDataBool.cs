using System;
using UnityEngine;

namespace CoreLib.PlayerData
{
    public class PlayerDataBool : PlayerData.DataHandle
    {
        [SerializeField]
        private bool m_Value;
        [SerializeField]
        private bool m_Default;

        public bool Value
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
            m_Value = bool.Parse(data);
        }

        public override string Serialize()
        {
            return m_Value.ToString();
        }
    }
}