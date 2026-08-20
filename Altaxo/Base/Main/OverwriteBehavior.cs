#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
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

namespace Altaxo.Main
{
  /// <summary>
  /// Specifies the action to take when a item already exists in the target location during a copy or move operation.
  /// </summary>
  public enum OverwriteBehavior
  {
    /// <summary>
    /// Specifies that the existing item should be kept and the new item should not be copied or moved. This is the default behavior.
    /// </summary>
    Skip = 0,

    /// <summary>
    /// Specifies that the existing item should be overwritten with the new item, regardless of type or other conditions.
    /// </summary>
    Overwrite = 1,

    /// <summary>
    /// Specifies that the new item should be renamed if an item with the same name already exists in the target location.
    /// </summary>
    Rename = 2,
  }
}
