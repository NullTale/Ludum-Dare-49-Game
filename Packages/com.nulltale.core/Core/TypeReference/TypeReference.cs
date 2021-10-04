using System;
using UnityEngine;

namespace CoreLib
{
    [Serializable]
    public class TypeReference : ISerializationCallbackReceiver
    {
        private Type		m_Type;
        [SerializeField]
        private string		m_TypeName;

        public Type			Type => m_Type;

        // =======================================================================
        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            if (!string.IsNullOrEmpty(m_TypeName)) 
            {
                m_Type = Type.GetType(m_TypeName);

                if (m_Type == null)
                    Debug.LogWarning(string.Format("'{0}' was referenced but class type was not found.", m_TypeName));
            }
            else 
            {
                m_Type = null;
            }
        }

        protected bool Equals(TypeReference other)
        {
            return m_TypeName == other.m_TypeName;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            var objType = obj.GetType();
            if (objType == this.GetType())
                return Equals((TypeReference)obj);
            if (objType == typeof(Type))
                return Equals(m_Type, obj);

            return false;

        }

        public override int GetHashCode()
        {
            return (m_TypeName != null ? m_TypeName.GetHashCode() : 0);
        }
    }
}