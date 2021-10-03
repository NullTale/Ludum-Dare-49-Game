using System;
using System.Linq;
using CoreLib;
using CoreLib.PlayerData;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game
{
    public class LevelResult : MonoBehaviour
    {
        public int  m_ScoresToComplete;

        public TMP_Text m_Scores;
        public TMP_Text m_Result;

        public UnityEvent OnComplete;
        public UnityEvent OnLoose;


        // =======================================================================
        public void Complete()
        {
            var pigs = GlobalList<Pig>.Content.Count(n => n.IsDead == false);
            var difficulty = SceneInitializer.Instance.m_Difficulty;
            var isLoose = pigs < difficulty.MinPigs;

            if (isLoose == false)
            {
                m_Scores.text = $"{pigs}/{GlobalList<Pig>.Content.Count}";

                if (pigs == difficulty.Pigs)
                    m_Result.text = $"<color=yellow>Great</color> {difficulty.Rank}!";
                else if (pigs == difficulty.MinPigs)
                    m_Result.text = $"<color=blue>Honest</color> {difficulty.Rank}!";
                else
                    m_Result.text = $"<color=red>Wild</color> {difficulty.Rank}";

                OnComplete.Invoke();
            }
            else
            {
                OnLoose.Invoke();
            }
        }
    }
}