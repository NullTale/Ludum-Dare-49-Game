
namespace CoreLib
{
	/// <summary> Interface used for all items which need to be ticked </summary>
	public interface ITicked
	{
        /// <summary> Gets or sets the length of the tick in seconds. </summary>
		/// <value> The length of the tick (Seconds). </value>
		float TickLength { get; }

        /// <summary> Raised when the tick length has elapsed. </summary>
        /// <param name="deltaTime"> </param>
        void OnTicked();
	}

	public interface ITickedDelta
    {
        IRefGet<float>  TickedDelta { set; }
    }
}