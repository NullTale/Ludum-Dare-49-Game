using System.Linq;

namespace Game
{
    public class HateBreed : PigModule, IStressSource
    {
        public PigBreed m_Breed;

        public float m_StressPreSec;

        // =======================================================================
        float IStressSource.GetStress()
        {
            return m_StressPreSec * GamePreferences.Instance.StressModMul * Owner.Neighbors.Count(n => n.m_Breed == m_Breed);
        }
    }
}