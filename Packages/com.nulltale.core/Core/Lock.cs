namespace CoreLib
{
    public struct Lock
    {
        private int m_Value;
        public bool On() => ++ m_Value == 1;
        public bool Off() => -- m_Value == 0;

        public static implicit operator bool(Lock l)
        {
            return l.m_Value > 0;
        }
    }
}