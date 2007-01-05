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
  /// Interface for an label item that is ready to draw and was already measured.
  /// </summary>
  public interface IMeasuredLabelItem
  {
    /// <summary>
    /// Size of the enclosing rectangle of the label item.
    /// </summary>
    SizeF Size { get; }

    /// <summary>
    /// Draws the label to a specified point.
    /// </summary>
    /// <param name="g">Graphics context.</param>
    /// <param name="brush">The brush to use for the drawing.</param>
    /// <param name="point">The point where to draw the item.</param>
    void Draw(Graphics g, BrushX brush, PointF point);
  }

 
}
