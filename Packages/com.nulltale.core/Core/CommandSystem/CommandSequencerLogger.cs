using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

namespace CoreLib.CommandSystem
{
    public sealed class CommandSequencerLogger : MonoBehaviour
    {
        private CommandSequencer m_Sequencer;
        [ReadOnly]
        public string m_Current;
        [ReadOnly]
        public List<string> m_Sequence;

        // =======================================================================
        private void OnEnable()
        {
            m_Sequencer = GetComponent<CommandSequencer>();
        }

        private void Update()
        {
            m_Sequence.Clear();
            m_Sequence.AddRange(m_Sequencer.Sequence.Select(n => n.ToString()));
            
            m_Current = m_Sequencer.Current.ToString();
        }
    }
}