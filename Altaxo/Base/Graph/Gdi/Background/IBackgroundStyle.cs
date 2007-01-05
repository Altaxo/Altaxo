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
using System.Drawing;

namespace Altaxo.Graph.Gdi.Background
{
  /// <summary>
  /// Provides a background around a rectangular spaced area.
  /// </summary>
  public interface IBackgroundStyle : ICloneable
  {
    /// <summary>
    /// Measures the outer size of the item.
    /// </summary>
    /// <param name="g">Graphics context.</param>
    /// <param name="innerArea">Inner area of the item.</param>
    /// <returns>The rectangle that encloses the item including the background.</returns>
    RectangleF MeasureItem(Graphics g, RectangleF innerArea);


    /// <summary>
    /// Draws the background.
    /// </summary>
    /// <param name="g">Graphics context.</param>
    /// <param name="innerArea">The inner area of the item.</param>
    void Draw(Graphics g, RectangleF innerArea);

    /// <summary>
    /// True if the classes color property can be set/reset;
    /// </summary>
    bool SupportsBrush { get; }

    /// <summary>
    /// Get/sets the color.
    /// </summary>
    BrushX Brush { get; set; }
  }
}
