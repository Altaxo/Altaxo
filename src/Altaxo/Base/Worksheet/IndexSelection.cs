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
	/// Summary description for IndexSelection.
	/// </summary>
	public class IndexSelection : System.Collections.SortedList 
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
				this.Clear();
				this.Add(nIndex,null);
			}
			lastSelectedIndex = nIndex;
		}
	}
}
