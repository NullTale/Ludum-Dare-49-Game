using System.Collections.Generic;
using System.Linq;
using CoreLib;
using TMPro;
using UnityEngine;

namespace Game
{
    public class DifficultyButton : MonoBehaviour
    {
        public TMP_Text Rank;

        private CircledCollection<KeyValuePair<string, GamePreferences.Difficulty>> m_Difficulties;

        // =======================================================================
        private void Awake()
        {
            var values = GamePreferences.Instance.m_DifficultyPresets.ToList();
            m_Difficulties              = new CircledCollection<KeyValuePair<string, GamePreferences.Difficulty>>(values);
            m_Difficulties.CurrentIndex = values.FindIndex(n => n.Key == GamePreferences.Instance.m_Difficulty);

            _update();
        }

        private void _update()
        {
            Rank.text                             = m_Difficulties.CurrentValue.Value.Rank;
            GamePreferences.Instance.m_Difficulty = m_Difficulties.CurrentValue.Key;
        }

        public void Next()
        {
            m_Difficulties.MoveNext();
            _update();
        }

        public void Prev()
        {
            m_Difficulties.MoveBack();
            _update();
        }
    }
}