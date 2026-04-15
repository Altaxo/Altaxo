#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2013 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 3 of the License, or
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

namespace Altaxo.Collections
{
  /// <summary>
  /// Determines the behavior when an item is inserted in the PartitionableList.
  /// </summary>
  public enum PartitionableListAddBehavior
  {
    /// <summary>If there are items in the partial view, the new item is added to the parent list immediately after the last item of the partial view. If the partial view is empty, the new item is added to the parent list as the last item.</summary>
    KeepTogether_AddLastIfEmpty,
    /// <summary>If there are items in the partial view, the new item is added to the parent list immediately after the last item of the partial view. If the partial view is empty, the new item is inserted in the parent list at index 0.</summary>
    KeepTogether_AddFirstIfEmpty,
    /// <summary>If there are items in the partial view, the new item is added to the parent list as the last item. If the partial view is empty, the new item is added to the parent list as the last item.</summary>
    AddLast_AddLastIfEmpty,
    /// <summary>If there are items in the partial view, the new item is added to the parent list as the last item. If the partial view is empty, the new item is inserted in the parent list at index 0.</summary>
    AddLast_AddFirstIfEmpty
  }
}
