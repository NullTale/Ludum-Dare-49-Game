using System;
using UnityEngine;

namespace CoreLib
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ScriptableObjectNested : PropertyAttribute
    {
    }
}