using System.Collections.Generic;
namespace CoreLib
{
	/// <summary>
	/// Priority comparer for <see cref="TickedQueueItem"/>.
	/// Only used internally.
	/// </summary>
	internal sealed class TickedQueueItemComparer : IComparer<TickedQueueItem>
	{
		/// <summary>
		/// <see cref="System.Collections.Generic.IComparer{TickedQueueItem}"/> implementation for sorting <see cref="TickedQueueItem"/> instances by Priority, then TickLength for equal priorities.
		/// </summary>
		/// <param name='a'>
		/// First item.
		/// </param>
		/// <param name='b'>
		/// Second Item.
		/// </param>
		public int Compare(TickedQueueItem a, TickedQueueItem b)
		{
			return DefaultCompare(a,b);
		}
		
		/// <summary>
		/// <see cref="System.Collections.Generic.IComparer{TickedQueueItem}"/> implementation for sorting <see cref="TickedQueueItem"/> instances by Priority, then TickLength for equal priorities.
		/// </summary>
		/// <param name='a'>
		/// First item.
		/// </param>
		/// <param name='b'>
		/// Second Item.
		/// </param>
		/// <remarks>
        /// For items with equal times and priorities, we will state that the
        /// first item is always lower than the second, so that the second is
        /// always placed later in the queue.  This means that DefaultCompare
        /// is non-commutative for these items.
		/// </remarks>
		public static int DefaultCompare(TickedQueueItem a, TickedQueueItem b)
		{
            var result = a.NextTickTime.CompareTo(b.NextTickTime);
            if (result == 0) result = -1;
		    return result;
		}
	}
}

