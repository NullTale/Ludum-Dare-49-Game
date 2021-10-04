using System;
using UnityEngine;

namespace CoreLib
{
    [Serializable]
    public class ResourceFolder
    {
        [SerializeField]
        private string m_GUID;
        [SerializeField]
        private string m_Path;

        public string Path
        {
            get => m_Path;
            internal set => m_Path = value;
        }

        public static implicit operator string(ResourceFolder rf) => rf.Path;
    }
}
