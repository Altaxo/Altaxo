#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

using System;

namespace Altaxo.Worksheet
{
  


  /// <summary>
  /// Summary description for IndexSelection.
  /// </summary>
  public class IndexSelection : Altaxo.Collections.AscendingIntegerCollection
  {
    protected int lastSelectedIndex=0;
    protected bool m_ExtendedSelectionBehaviour=true;

  
    public int LastSelection
    {
      get
      {
        return this.Count>0 ? lastSelectedIndex : 0;
      }
    }

    public bool IsSelected(int nIndex)
    {
      return Contains(nIndex);
    }

    public void Select(int nIndex, bool bShiftKey, bool bControlKey)
    {
      if(bControlKey) // Control pressed
      {
        if(this.Contains(nIndex))
          this.Remove(nIndex);
        else
          this.Add(nIndex);
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
        { beg = lastSelectedIndex; end = nIndex;  }
        else 
        { beg=nIndex; end=lastSelectedIndex;  }

        // select all from lastSelectionIndex to here
        for(int i=beg;i<=end;i++)
        {
          if(!this.Contains(i))
            this.Add(i);
        }

      }
      else // no modifier key 
      {
        // Clear the selection, if the user clicked again on a single selection
        if(this.Count==1 && this.Contains(nIndex))
        {
          this.Clear();
        }
        else
        {
          this.Clear();
          this.Add(nIndex);
        }
      }
      lastSelectedIndex = nIndex;
    }
  }
}
