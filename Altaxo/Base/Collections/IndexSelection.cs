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

#nullable enable
using System;

namespace Altaxo.Collections
{
  /// <summary>
  /// Selection of indices, mimicking the behavior of selection in lists etc, using CTRL key to select single items and SHIFT to select multiple items.
  /// </summary>
  public class IndexSelection : Altaxo.Collections.AscendingIntegerCollection
  {
    protected int _lastSelectedIndex = 0;
    protected bool _useExtendedSelectionBehaviour = true;

    /// <summary>
    /// Gets the last selected index.
    /// </summary>
    /// <value>
    /// The last selection.
    /// </value>
    public int LastSelection
    {
      get
      {
        return Count > 0 ? _lastSelectedIndex : 0;
      }
    }

    /// <summary>
    /// Determines whether the specified  index <paramref name="nIndex"/> is selected.
    /// </summary>
    /// <param name="nIndex">Index.</param>
    /// <returns>
    ///   <c>true</c> if the specified index is selected; otherwise, <c>false</c>.
    /// </returns>
    public bool IsSelected(int nIndex)
    {
      return Contains(nIndex);
    }

    /// <summary>
    /// Selects the specified index <paramref name="nIndex"/>, mimicking the behavior when using CTRL and SHIFT keys.
    /// </summary>
    /// <param name="nIndex">Index to select.</param>
    /// <param name="isShiftKeyActive">Set to true if the shift key is pressed. If the control key is pressed, this parameter is ignored.</param>
    /// <param name="isControlKeyActive">Set to true if the control key is pressed. </param>
    public void Select(int nIndex, bool isShiftKeyActive, bool isControlKeyActive)
    {
      if (isControlKeyActive) // Control pressed
      {
        if (Contains(nIndex))
          Remove(nIndex);
        else
          Add(nIndex);
      }
      else if (isShiftKeyActive)
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
