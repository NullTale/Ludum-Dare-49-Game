using System;
using UnityEngine;

namespace CoreLib
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class SoloFieldAttribute : PropertyAttribute
    {
        public string PropertyPath;

        public SoloFieldAttribute(string propertyPath)
        {
            PropertyPath = propertyPath;
        }
    }
}