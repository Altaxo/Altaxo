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

#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Graph.Graph3D.Viewing
{
  /// <summary>
  /// Specifies the active tool in a 3D graph view.
  /// </summary>
  public enum GraphToolType
  {
    /// <summary>No tool is active.</summary>
    None,
    /// <summary>Selects and manipulates objects.</summary>
    ObjectPointer,
    /// <summary>Draws an arrow line.</summary>
    ArrowLineDrawing,
    /// <summary>Draws a single line.</summary>
    SingleLineDrawing,
    /// <summary>Draws a rectangle.</summary>
    RectangleDrawing,
    /// <summary>Draws a curly brace.</summary>
    CurlyBraceDrawing,
    /// <summary>Draws an ellipse.</summary>
    EllipseDrawing,
    /// <summary>Draws a text object.</summary>
    TextDrawing,
    /// <summary>Reads plot-item data.</summary>
    ReadPlotItemData,
    /// <summary>Reads XY coordinates.</summary>
    ReadXYCoordinates,
    /// <summary>Zooms axes.</summary>
    ZoomAxes,
    /// <summary>Draws a regular polygon.</summary>
    RegularPolygonDrawing,
    /// <summary>Draws an open cardinal spline.</summary>
    OpenCardinalSplineDrawing,
    /// <summary>Draws a closed cardinal spline.</summary>
    ClosedCardinalSplineDrawing,

    /// <summary>Edits the grid of the current layer, or if it has no childs, the grid of the parent layer.</summary>
    EditGrid
  }
}
