using System;

namespace CoreLib
{
    [Serializable]
    public enum LogicOperation
    {
        And,
        Or,
        Equal,
        NotEqual,
    }

    public static class CLogicOperationExtentions
    {
        public static bool Check(this LogicOperation operation, bool a, bool b)
        {
            switch (operation)
            {
                case LogicOperation.And:
                    return a && b;
                case LogicOperation.Or:
                    return a || b;
                case LogicOperation.Equal:
                    return a == b;
                case LogicOperation.NotEqual:
                    return a != b;
                default:
                    throw new ArgumentOutOfRangeException(nameof(operation), operation, null);
            }
        }
    }
}