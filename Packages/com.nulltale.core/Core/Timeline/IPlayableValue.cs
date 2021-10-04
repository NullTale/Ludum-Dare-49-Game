using System;

namespace CoreLib
{
    internal interface IPlayableValue
    {
        internal void Lock();
        internal void Set(float curveValue, float weight);
        internal void UnLock();
    }
}