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
using System.Drawing.Drawing2D;
using Altaxo.Serialization;
using Altaxo.Graph.Scales;
using Altaxo.Data;

namespace Altaxo.Graph.Gdi.LabelFormatting
{
  

  /// <summary>
  /// Procedures to format an item of the <see cref="Altaxo.Data.AltaxoVariant" /> class.
  /// </summary>
  public interface ILabelFormatting : ICloneable
  {

    /// <summary>
    /// Measures the item, i.e. returns the size of the item.
    /// </summary>
    /// <param name="g">Graphics context.</param>
    /// <param name="font">The font that is used to draw the item.</param>
    /// <param name="strfmt">String format used to draw the item.</param>
    /// <param name="mtick">The item to draw.</param>
    /// <param name="morg">The location the item will be drawn.</param>
    /// <returns>The size of the item if it would be drawn.</returns>
    SizeF MeasureItem(Graphics g, System.Drawing.Font font, System.Drawing.StringFormat strfmt, Data.AltaxoVariant mtick, PointF morg);
    
    /// <summary>
    /// Draws the item to a specified location.
    /// </summary>
    /// <param name="g">Graphics context.</param>
    /// <param name="brush">Brush used to draw the item.</param>
    /// <param name="font">Font used to draw the item.</param>
    /// <param name="strfmt">String format.</param>
    /// <param name="item">The item to draw.</param>
    /// <param name="morg">The location where the item is drawn to.</param>
    void DrawItem(Graphics g, BrushX brush, System.Drawing.Font font, System.Drawing.StringFormat strfmt, AltaxoVariant item, PointF morg);

    /// <summary>
    /// Measured a couple of items and prepares them for being drawn.
    /// </summary>
    /// <param name="g">Graphics context.</param>
    /// <param name="font">Font used.</param>
    /// <param name="strfmt">String format used.</param>
    /// <param name="items">Array of items to be drawn.</param>
    /// <returns>An array of <see cref="IMeasuredLabelItem" /> that can be used to determine the size of each item and to draw it.</returns>
    IMeasuredLabelItem[] GetMeasuredItems(Graphics g, System.Drawing.Font font, System.Drawing.StringFormat strfmt, AltaxoVariant[] items);
  }
}
