using System;
using UnityEngine;

namespace CoreLib
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class InplaceFieldAttribute : PropertyAttribute
    {
        public string[] PropertyPath;

        public InplaceFieldAttribute(params string[] propertyPath)
        {
            PropertyPath = propertyPath;
        }
    }
}