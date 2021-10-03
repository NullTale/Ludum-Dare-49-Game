using System;
using CoreLib;
using CoreLib.States;
using NaughtyAttributes;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEventBus;

namespace Game
{
    public class GamePreferences : Core.Module<GamePreferences>
    {
        public GlobalState    Pause;
        public PlayerProgress Data;

        public float StressMul;
        public float ChillMul;
        public float StressModMul;
        public float DragHeight;

        [HorizontalLine]
        public float DeadStress;

        [Layer]
        public int  PigLayer;

        public string m_Difficulty;

        public SerializableDictionary<string, Difficulty> m_DifficultyPresets;

        public Difficulty GetDifficulty() => m_DifficultyPresets[m_Difficulty];

        // =======================================================================
        [Serializable]
        public class Difficulty
        {
            public string Rank;
            public int    Pigs;
            public int    MinPigs;
            public float  Time;
        }

        // =======================================================================
        public override void Init()
        {
        }
    }
}