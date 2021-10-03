using System.Linq;
using CoreLib;

namespace Game
{
    public class HateGender : PigModule, IStressSource
    {
        public PigGender m_Gender;

        public float m_StressPreSec;

        // =======================================================================
        float IStressSource.GetStress()
        {
            return m_StressPreSec * GamePreferences.Instance.StressModMul * Owner.Neighbors.Count(n => n.m_Gender == m_Gender);
        }
    }
}