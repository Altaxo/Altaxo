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

namespace Altaxo.Gui.Graph.Gdi.Viewing
{

  /// <summary>
  /// Interface for the four-points on a plot curve tool, for instance the step evaluation tool or the peak evaluation tool.
  /// </summary>
  public interface IToolFourPointsOnCurve : IToolTwoPointsOnCurve
  {
    /// <summary>
    /// Gets the inner left point (both plot index and the row index of the original data column).
    /// The values are double because the pointer can be located inbetween two points.
    /// </summary>
    public (double PlotIndex, double RowIndex) InnerLeftPoint { get; }

    /// <summary>
    /// Gets the inner right point (both plot index and the row index of the original data column).
    /// The values are double because the pointer can be located inbetween two points.
    /// </summary>
    public (double PlotIndex, double RowIndex) InnerRightPoint { get; }
  }
}
