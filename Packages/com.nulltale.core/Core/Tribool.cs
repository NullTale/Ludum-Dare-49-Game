using System;

namespace CoreLib
{
    [Serializable]
    public enum Tribool
    {
        False = -1,
        Unknown = 0,
        True = 1,
    }

    public static class TriboolExtentions
    {
        public static bool AsBool(this Tribool tb)
        {
            return tb == Tribool.True;
        }
        
        public static Tribool AsTribool(this bool b)
        {
            return b ? Tribool.True : Tribool.False;
        }

        public static bool IsTrue(this Tribool tb)
        {
            return tb == Tribool.True;
        }
        
        public static bool IsFalse(this Tribool tb)
        {
            return tb == Tribool.False;
        }

        public static bool Equal(this Tribool tb, bool b)
        {
            switch (tb)
            {
                case Tribool.Unknown:
                    return false;
                case Tribool.False:
                    return b == false;
                case Tribool.True:
                    return b == true;
                default:
                    throw new ArgumentOutOfRangeException(nameof(tb), tb, null);
            }
        }
    }
}