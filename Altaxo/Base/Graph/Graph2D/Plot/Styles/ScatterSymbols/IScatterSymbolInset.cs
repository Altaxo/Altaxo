#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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
using Altaxo.Drawing;
using Clipper2Lib;

namespace Altaxo.Graph.Graph2D.Plot.Styles.ScatterSymbols
{
  /// <summary>
  /// Represents the inset geometry and color of a scatter symbol.
  /// </summary>
  public interface IScatterSymbolInset
  {
    /// <summary>
    /// Gets the inset color.
    /// </summary>
    NamedColor Color { get; }

    /// <summary>
    /// Returns a copy of this inset with the specified color.
    /// </summary>
    /// <param name="color">The inset color to apply.</param>
    /// <returns>An inset instance with the specified color.</returns>
    IScatterSymbolInset WithColor(NamedColor color);

    /// <summary>
    /// Gets a copy of the inset polygon in clipper coordinates.
    /// </summary>
    /// <param name="relativeWidth">The inset width relative to the symbol size.</param>
    /// <returns>The inset polygon in clipper coordinates.</returns>
    Paths64 GetCopyOfClipperPolygon(double relativeWidth);
  }
}
