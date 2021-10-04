using System;
using UnityEngine;

namespace CoreLib
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class WrapperOfAttribute : PropertyAttribute
    {
        private string m_PropertyName;
        public  string PropertyName => m_PropertyName;

        public WrapperOfAttribute(string propertyName)
        {
            m_PropertyName = propertyName;
        }
    }
}