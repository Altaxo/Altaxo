using System;

namespace Altaxo.Collections
{
	/// <summary>
	/// The interface of a sorted collection of integers, sorted so that the smallest integers come first.
	/// </summary>
	public interface IAscendingIntegerCollection
	{
		/// <summary>
		/// Number of integers stored in this collection.
		/// </summary>
		int Count { get; }

		/// <summary>
		/// The integer value stored at position <code>i</code>.
		/// </summary>
		int this[int i] { get; }

		/// <summary>
		/// Returns true, if the integer <code>nValue</code> is contained in this collection.
		/// </summary>
		/// <param name="nValue">The integer value to test for membership.</param>
		/// <returns>True if the integer value is member of the collection.</returns>
		bool Contains(int nValue);
		


		/// <summary>
		/// Get the next range (i.e. a contiguous range of integers) in ascending order.
		/// </summary>
		/// <param name="currentposition">The current position into this collection. Use 0 for the first time. On return, this is the next position.</param>
		/// <param name="rangestart">Returns the starting index of the contiguous range.</param>
		/// <param name="rangecount">Returns the width of the range.</param>
		/// <returns>True if the returned data are valid, false if there is no more data.</returns>
		/// <remarks>You can use this function in a while loop:
		/// <code>
		/// int rangestart, rangecount;
		/// int currentPosition=0;
		/// while(GetNextRangeAscending(ref currentPosition, out rangestart, out rangecount))
		///		{
		///		// do your things here
		///		}
		/// </code></remarks>
		bool GetNextRangeAscending(ref int currentposition, ref int rangestart, ref int rangecount);


		/// <summary>
		/// Get the next range (i.e. a contiguous range of integers) in descending order.
		/// </summary>
		/// <param name="currentposition">The current position into this collection. Use Count-1 for the first time. On return, this is the next position.</param>
		/// <param name="rangestart">Returns the starting index of the contiguous range.</param>
		/// <param name="rangecount">Returns the width of the range.</param>
		/// <returns>True if the range data are valid, false if there is no more data. Used as end-of-loop indicator.</returns>
		/// <remarks>You can use this function in a while loop:
		/// <code>
		/// int rangestart, rangecount;
		/// int currentPosition=selection.Count-1;
		/// while(selection.GetNextRangeAscending(currentPosition,out rangestart, out rangecount))
		///		{
		///		// do your things here
		///		}
		/// </code></remarks>
		bool GetNextRangeDescending(ref int currentposition, ref int rangestart, ref int rangecount);
	}

}
