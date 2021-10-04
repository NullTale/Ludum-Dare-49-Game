using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CoreLib
{
    [Serializable]
    public class ShuffleBag<T> : ICollection<T>
    {
        [SerializeField]
        private List<T> m_Values;
        private List<T> m_OpenSet = new List<T>();
        [SerializeField]
        private int     m_Duplicate     = 1;

        public int  Count      => m_Values.Count;
        public bool IsReadOnly => false;
        public int  Remains => m_OpenSet.Count;

        public int Duplicate
        {
            get => m_Duplicate;
            set => m_Duplicate = value;
        }

        // =======================================================================
        public ShuffleBag(IEnumerable<T> values)
        {
             m_Values = values.ToList();
        }

        public ShuffleBag()
        {
             m_Values = new List<T>();
        }

        public void Add(T item)
        {
            m_Values.Add(item);
            m_OpenSet.Add(item);
        }

        public bool Remove(T item)
        {
            if (m_Values.Remove(item))
            {
                m_OpenSet.Remove(item);
                return true;
            }

            return false;
        }

        public void Reset()
        {
            m_OpenSet.Clear();
            for (var n = 0; n < Duplicate; n++)
                m_OpenSet.AddRange(m_Values);
        }

        public T Next()
        {
            // refill if empty, take item
            if (m_OpenSet.Count == 0)
                Reset();

            var index = Random.Range(0, m_OpenSet.Count);
            var result = m_OpenSet[index];
            m_OpenSet.RemoveAt(index);

            return result;
        }

        public IEnumerable<T> Enumerate()
        {
            return Enumerate(Remains);
        }

        public IEnumerable<T> Enumerate(int count)
        {
            for (var n = 0; n < count; n++)
                yield return Next();
        }

        public void Clear()
        {
            m_Values.Clear();
            m_OpenSet.Clear();
        }

        public bool Contains(T item)
        {
            return m_Values.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            m_Values.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return m_Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}