#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2004 Dr. Dirk Lellinger
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

namespace Altaxo.Graph
{
  /// <summary>
  /// Interface used for all plot items and styles to get information for plotting their data.
  /// </summary>
  public interface IPlotArea
  {
    /// <summary>
    /// Returns true if the plot area is orthogonal, i.e. if the x and the y axis are orthogonal to each other.
    /// </summary>
    bool IsOrthogonal { get; }

    /// <summary>
    /// Returns true if the plot coordinates can be calculated as a linear transformation of the physical values.
    /// Returns false if this is for instance a polar diagram. 
    /// </summary>
    bool IsAffine { get; }

    /// <summary>
    /// Gets the axis of the independent variable.
    /// </summary>
    Axis XAxis { get; }
    
    /// <summary>
    /// Gets the axis of the dependent variable.
    /// </summary>
    Axis YAxis { get; }

    /// <summary>
    /// Get a region object, which describes the plotting area. Used to clip the plotting to
    /// the plotting area.
    /// </summary>
    /// <returns>A region object describing the plotting area.</returns>
    Region GetRegion();

    /// <summary>
    /// Converts logical x and y values (between 0 and 1) to the appropriate coordinate values (layer coordinates).
    /// </summary>
    I2DTo2DConverter LogicalToAreaConversion { get; }

    /// <summary>
    /// Converts layer coordinates to logical x and y values (between 0 and 1).
    /// </summary>
    I2DTo2DConverter AreaToLogicalConversion { get; }
  }
}
