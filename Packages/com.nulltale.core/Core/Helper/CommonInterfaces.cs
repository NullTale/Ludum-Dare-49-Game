namespace CoreLib
{
    public interface IParameter
    {
    }

    public interface IParameter<T> : IParameter
    {
        T Value { get; set; }
    }
}