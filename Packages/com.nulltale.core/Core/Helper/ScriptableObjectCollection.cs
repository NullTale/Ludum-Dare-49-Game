using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

namespace CoreLib
{
    [Serializable]
    public class ScriptableObjectCollection<T> : IReadOnlyDictionary<string, T>
        where T : ScriptableObject
    {
        [SerializeField]
        private List<T> m_List = new List<T>();

        // =======================================================================
        private class EnumeratorWrapper : IEnumerator<KeyValuePair<string, T>>
        {
            private IEnumerator<T>          m_Source;
            public  KeyValuePair<string, T> Current => new KeyValuePair<string, T>(m_Source.Current.name, m_Source.Current);
            object IEnumerator.             Current => Current;

            // =======================================================================
            public EnumeratorWrapper(IEnumerable<T> source)
            {
                m_Source = source.GetEnumerator();
            }

            public bool MoveNext()
            {
                return m_Source.MoveNext();
            }

            public void Reset()
            {
            }

            public void Dispose()
            {
                m_Source.Dispose();
            }
        }

        // =======================================================================
        public int Count => m_List.Count;

        public T this[int index] => m_List[index];
        public T this[string key] => m_List.FirstOrDefault(n => n.name == key);

        public IEnumerable<string> Keys   => m_List.Select(n => n.name);
        public IEnumerable<T>      Values => m_List;

        IEnumerator<KeyValuePair<string, T>> IEnumerable<KeyValuePair<string, T>>.GetEnumerator()
        {
            return new EnumeratorWrapper(m_List);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return m_List.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public bool ContainsKey(string key)
        {
            return m_List.Any(n => n.name == key);
        }

        public bool TryGetValue(string key, out T value)
        {
            return m_List.TryGetValue(n => n.name == key, out value);
        }

    }
}