/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

using System;

namespace Altaxo.Worksheet
{
	/// <summary>
	/// The interface of a sorted collection of integers, sorted so that the smallest integers come first.
	/// </summary>
	public interface IAscendingIntegerCollection
	{
		/// <summary>
		/// Number of values stored in this collection.
		/// </summary>
		int Count { get; }

		/// <summary>
		/// Returns the integer value stored at position <code>i</code>.
		/// </summary>
		int this[int i] {get; }

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
		bool GetNextRangeAscending(ref int currentposition, out int rangestart, out int rangecount);


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
		bool GetNextRangeDescending(ref int currentposition, out int rangestart, out int rangecount);
	}

	/// <summary>
	/// This class represents a simple integer range specified by start and count, that can be used as a lightweight substitute for a <see>IndexSelection</see> if 
	/// the selection is contiguous.
	/// </summary>
	public class IntegerRange : IAscendingIntegerCollection
	{
		int _start;
		int _count;

		/// <summary>
		/// Constructs the range by giving a start and the width.
		/// </summary>
		/// <param name="start">The range start.</param>
		/// <param name="count">The range width, i.e the range is from start until (including) start+count-1.</param>
		public IntegerRange(int start, int count)
		{
			_start = start;
			_count = count;
		}

		/// <summary>
		/// The width of the range.
		/// </summary>
		public int Count 
		{
			get 
			{
				return _count;
			}
		}

		/// <summary>
		/// Returns the i-th number of the range, starting from the start of the range.
		/// </summary>
		public int this[int i]
		{
			get { return _start + i; }
		}


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
		public bool GetNextRangeAscending(ref int currentposition, out int rangestart, out int rangecount)
		{
			if(currentposition<0 || currentposition>=Count)
			{
				rangestart=0;
				rangecount=0;
				return false;
			}
			else
			{
				rangestart = _start + currentposition;
				rangecount = _start + _count - rangestart;
				currentposition = _count;
				return true;
			}
		}

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
		public bool GetNextRangeDescending(ref int currentposition, out int rangestart, out int rangecount)
		{
			if(currentposition<0 || currentposition>=Count)
			{
				rangestart=0;
				rangecount=0;
				return false;
			}
			else
			{
				rangestart = _start;
				rangecount = currentposition+1;
				currentposition = -1;
				return true;
			}
		}

	}


	/// <summary>
	/// Summary description for IndexSelection.
	/// </summary>
	public class IndexSelection : System.Collections.SortedList, IAscendingIntegerCollection
	{
		protected int lastSelectedIndex=0;
		protected bool m_ExtendedSelectionBehaviour=true;


		public IndexSelection()
		{
		}

		public int LastSelection
		{
			get
			{
				return this.Count>0 ? lastSelectedIndex : 0;
			}
		}

		public int this[int i]
		{
			get { return (int)base.GetKey(i); }
		}


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
		public bool GetNextRangeAscending(ref int currentposition, out int rangestart, out int rangecount)
		{
			if(currentposition<0 || currentposition>=Count)
			{
				rangestart=0;
				rangecount=0;
				return false;
			}
			rangestart = this[currentposition];
			int previous = rangestart;
			rangecount=1;
			for(currentposition=currentposition+1;currentposition<Count;currentposition++)
			{
				if(this[currentposition]==(previous+1))
				{
					previous++;
					rangecount++;
				}
				else
				{
					break;
				}
			}

			return true;
		}

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
		public bool GetNextRangeDescending(ref int currentposition, out int rangestart, out int rangecount)
		{
			if(currentposition<0 || currentposition>=Count)
			{
				rangestart=0;
				rangecount=0;
				return false;
			}

			rangestart = this[currentposition];
			rangecount=1;
			for(currentposition=currentposition-1;currentposition>=0;currentposition--)
			{
				if(this[currentposition]==(rangestart-1))
				{
					rangestart--;
					rangecount++;
				}
				else
				{
					break;
				}
			}

			return true;
		}


		public bool IsSelected(int nIndex)
		{
			return this.ContainsKey(nIndex);
		}

		/// <summary>
		/// Returns the selected indizes as an integer array.
		/// </summary>
		/// <returns>An array of integers containing the selected indizes. If nothing is selected, an array of length 0 is returned.</returns>
		public int[] GetSelectedIndizes()
		{
			int[] arr = new int[this.Count];
			for(int i=0;i<this.Count;i++)
				arr[i] = (int)base.GetKey(i);

			return arr;
		}

		public void Select(int nIndex, bool bShiftKey, bool bControlKey)
		{
			if(bControlKey) // Control pressed
			{
				if(this.ContainsKey(nIndex))
					this.Remove(nIndex);
				else
					this.Add(nIndex,null);
			}
			else if(bShiftKey)
			{
				if(0==this.Count)
					lastSelectedIndex=0;

				if(!m_ExtendedSelectionBehaviour && 0!=this.Count) // standard behaviour : clear the selection list before selecting the new range
				{
					// if standard behaviour, clear the list before selecting the new area
					// but keep lastSelectedIndex !
					this.Clear();
				}


				int beg, end;
				if(nIndex>=LastSelection)
				{	beg = lastSelectedIndex; end = nIndex;  }
				else 
				{	beg=nIndex; end=lastSelectedIndex;	}

				// select all from lastSelectionIndex to here
				for(int i=beg;i<=end;i++)
				{
					if(!this.ContainsKey(i))
						this.Add(i,null);
				}

			}
			else // no modifier key 
			{
				// Clear the selection, if the user clicked again on a single selection
				if(this.Count==1 && this.ContainsKey(nIndex))
				{
					this.Clear();
				}
				else
				{
					this.Clear();
					this.Add(nIndex,null);
				}
			}
			lastSelectedIndex = nIndex;
		}
	}
}
