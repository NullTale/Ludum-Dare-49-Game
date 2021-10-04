using UnityEngine;

namespace CoreLib
{
    public class GlobalID : ScriptableObject, IUniqueID
    {
        [SerializeField]
        private UniqueID    m_ID;
        public string ID => m_ID;
    }
}