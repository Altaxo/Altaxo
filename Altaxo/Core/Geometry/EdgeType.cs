#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2025 Dr. Dirk Lellinger
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

namespace Altaxo.Geometry
{
  /// <summary>
  /// Designates the 4 edges of a rectangular area.
  /// </summary>
  [Serializable]
  public enum EdgeType
  {
    /// <summary>
    /// The left edge of the rectangular area.
    /// </summary>
    Left = 0,
    /// <summary>
    /// The bottom edge of the rectangular area.
    /// </summary>
    Bottom = 1,
    /// <summary>
    /// The right edge of the rectangular area.
    /// </summary>
    Right = 2,
    /// <summary>
    /// The top edge of the rectangular area.
    /// </summary>
    Top = 3
  }
}
