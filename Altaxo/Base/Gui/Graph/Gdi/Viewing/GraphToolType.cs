﻿#region Copyright

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

#nullable disable
namespace Altaxo.Gui.Graph.Gdi.Viewing
{
  public enum GraphToolType
  {
    None,
    ObjectPointer,
    ArrowLineDrawing,
    SingleLineDrawing,
    RectangleDrawing,
    CurlyBraceDrawing,
    EllipseDrawing,
    TextDrawing,
    ReadPlotItemData,
    ReadXYCoordinates,
    ZoomAxes,
    RegularPolygonDrawing,
    OpenCardinalSplineDrawing,
    ClosedCardinalSplineDrawing,

    /// <summary>Edits the grid of the current layer, or if it has no childs, the grid of the parent layer.</summary>
    EditGrid,

    /// <summary>Four points on a curve to evaluate areas, steps, etc,</summary>
    FourPointsOnCurve,

    FourPointStepEvaluation,

    FourPointPeakEvaluation,
  }
}
