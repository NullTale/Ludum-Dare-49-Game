using System;
using System.Collections.Generic;
using System.Linq;
using CoreLib;
using CoreLib.PlayerData;
using UnityEngine;

namespace Game
{
    public class PlayerProgress : PlayerData.DataHandle
    {
        public List<LevelData> m_Levels;

        // =======================================================================
        [Serializable]
        public class LevelScores : SaveData
        {
            public List<LevelData> m_Levels;
        }

        [Serializable]
        public class LevelData
        {
            public string Name;
            public int    Scores;
            public bool   Complete;
            public bool   Gold;
        }

        // =======================================================================
        public override void InitDefault()
        {
        }

        public LevelData GetData(string levelName)
        {
            if (m_Levels.TryGetValue(n => n.Name == levelName, out var data) == false)
            {
                data = new PlayerProgress.LevelData()
                {
                    Name     = levelName,
                    Complete = false,
                    Gold     = false,
                    Scores   = 0
                };

                m_Levels.Add(data);
            }
            return data;
        }

        public override void Init(string data)
        {
            JsonUtility.FromJson<LevelScores>(data).InjectTo(this);
        }

        public override string Serialize()
        {
            return JsonUtility.ToJson(new LevelScores().SetupFrom(this));
        }
    }
}