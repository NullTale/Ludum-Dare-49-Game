using System;

namespace CoreLib
{
    [Serializable]
    public enum ConditionBool
    {
        True,
        False,
        AlwaysTrue,
        AlwaysFalse,
    }

    public static class ConditionBoolExtentions
    {
        public static bool Check(this ConditionBool condition, bool value)
        {
            switch (condition)
            {
                case ConditionBool.True:
                    return value == true;
                case ConditionBool.False:
                    return value == false;
                case ConditionBool.AlwaysTrue:
                    return true;
                case ConditionBool.AlwaysFalse:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException(nameof(condition), condition, null);
            }
        }
    }
}