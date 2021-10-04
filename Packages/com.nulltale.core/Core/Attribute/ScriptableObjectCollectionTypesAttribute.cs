using System;
using UnityEngine;

namespace CoreLib
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ScriptableObjectCollectionTypesAttribute : PropertyAttribute
    {
        public Type[] Types;

        public ScriptableObjectCollectionTypesAttribute(params Type[] types)
        {
            Types = types;
        }
    }
}