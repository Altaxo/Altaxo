#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

#endregion Copyright

using System;

namespace Altaxo.Collections
{
  /// <summary>
  /// Summary description for IndexSelection.
  /// </summary>
  public class IndexSelection : Altaxo.Collections.AscendingIntegerCollection
  {
    protected int _lastSelectedIndex = 0;
    protected bool _useExtendedSelectionBehaviour = true;

    public int LastSelection
    {
      get
      {
        return Count > 0 ? _lastSelectedIndex : 0;
      }
    }

    public bool IsSelected(int nIndex)
    {
      return Contains(nIndex);
    }

    public void Select(int nIndex, bool bShiftKey, bool bControlKey)
    {
      if (bControlKey) // Control pressed
      {
        if (Contains(nIndex))
          Remove(nIndex);
        else
          Add(nIndex);
      }
      else if (bShiftKey)
      {
        if (0 == Count)
          _lastSelectedIndex = 0;

        if (!_useExtendedSelectionBehaviour && 0 != Count) // standard behaviour : clear the selection list before selecting the new range
        {
          // if standard behaviour, clear the list before selecting the new area
          // but keep lastSelectedIndex !
          Clear();
        }

        int beg, end;
        if (nIndex >= LastSelection)
        { beg = _lastSelectedIndex; end = nIndex; }
        else
        { beg = nIndex; end = _lastSelectedIndex; }

        // select all from lastSelectionIndex to here
        for (int i = beg; i <= end; i++)
        {
          if (!Contains(i))
            Add(i);
        }
      }
      else // no modifier key
      {
        // Clear the selection, if the user clicked again on a single selection
        if (Count == 1 && Contains(nIndex))
        {
          Clear();
        }
        else
        {
          Clear();
          Add(nIndex);
        }
      }
      _lastSelectedIndex = nIndex;
    }
  }
}
