using System;
using UnityEngine;

namespace CoreLib
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class CreateScriptableObjectAttribute : PropertyAttribute
    {
        public bool Visible;

        public CreateScriptableObjectAttribute(bool visible)
        {
            Visible = visible;
        }
    }
}