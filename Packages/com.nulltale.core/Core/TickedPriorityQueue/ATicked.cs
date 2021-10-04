namespace CoreLib
{
	/// <summary> Abstract implementation of ITicked, implements the properties, leaves OnTicked abstract </summary>
	public abstract class ATicked : ITicked
	{
        /// <summary> Constant default tick length, used to set TickLength in the constructor. </summary>
		public const float k_DefaultTickLength = 0.25f;

        /// <summary> Gets or sets the length of the tick in seconds. </summary>
		public virtual float TickLength { get; set; } = k_DefaultTickLength;

        /// <summary> Raised when the tick length has elapsed. </summary>
        /// <param name="deltaTime"> </param>
        public abstract void OnTicked();
	}
}