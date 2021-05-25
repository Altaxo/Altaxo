#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2020 Dr. Dirk Lellinger
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

#endregion Copyrightusing System;

#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Drawing
{
  /// <summary>
  /// Holds some standard brushes.
  /// </summary>
  public static class BrushesX
  {
    /// <summary>
    /// Gets a white solid brush.
    /// </summary>
    public static BrushX White { get; } = new BrushX(NamedColors.White);

    /// <summary>
    /// Gets a black solid brush.
    /// </summary>
    public static BrushX Black { get; } = new BrushX(NamedColors.Black);

    /// <summary>
    /// Gets a transparent brush (a brush that is not visible).
    /// </summary>
    /// <value>
    /// The transparent.
    /// </value>
    public static BrushX Transparent { get; } = new BrushX(NamedColors.Transparent);
  }
}
