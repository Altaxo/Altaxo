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
				// deselect all and then select from lastSelectedColumn to this
				// this is the behaviour of the windows explorer
				if(0==this.Count)
					lastSelectedIndex=0;
				else
					this.Clear();
				int step = lastSelectedIndex<=nIndex ? 1 : -1;
				for(int i=lastSelectedIndex;i<=nIndex;i+=step)
					this.Add(i,null);
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
