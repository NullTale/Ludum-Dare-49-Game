using System;
using System.Collections.Generic;
using System.Linq;
using RotaryHeart.Lib.SerializableDictionary;
using Random = UnityEngine.Random;

namespace CoreLib
{
    [Serializable]
    public class WeightedSet<T> : SerializableDictionaryBase<T, float>
    {
        private float                m_MinWeight;
        private Dictionary<T, float> m_OpenSet = new Dictionary<T, float>();

        // =======================================================================
        public T Next()
        {
            // refresh open set, memory leak if value was removed
            var weight = m_OpenSet.Values.Sum();
            if (weight <= 0.0f)
            {
                foreach (var item in this)
                {
                    if (m_OpenSet.ContainsKey(item.Key))
                        m_OpenSet[item.Key] += item.Value;
                    else
                        m_OpenSet.Add(item.Key, item.Value);
                }

                m_MinWeight = m_OpenSet.Values.Min();
                weight      = m_OpenSet.Values.Sum();
            }

            // make a roll
            var roll = Random.Range(0.0f, weight);
            foreach (var n in m_OpenSet)
            {
                if (n.Value < 0.0f)
                    continue;

                if (roll < n.Value)
                {
                    m_OpenSet[n.Key] -= m_MinWeight;
                    return n.Key;
                }
                roll -= n.Value;
            }

            // not reachable
            throw new IndexOutOfRangeException();
        }

        public void Reset()
        {
            m_OpenSet.Clear();
        }
    }
}
