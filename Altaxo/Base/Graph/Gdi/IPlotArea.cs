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
using Altaxo.Graph.Scales;

namespace Altaxo.Graph.Gdi
{
  /// <summary>
  /// Interface used for all plot items and styles to get information for plotting their data.
  /// </summary>
  public interface IPlotArea
  {

    /// <summary>
    /// Returns true when this is a 3D area, i.e. it utilizes 3 Scales and a 3D Coordinate system.
    /// </summary>
    bool Is3D { get; }

    /// <summary>
    /// Gets the axis of the independent variable.
    /// </summary>
    Scale XAxis { get; }
    
    /// <summary>
    /// Gets the axis of the dependent variable.
    /// </summary>
    Scale YAxis { get; }


    G2DCoordinateSystem CoordinateSystem { get; }

  
    /// <summary>
    /// Returns the size of the rectangular layer area.
    /// </summary>
    SizeF Size { get; }

    Logical3D GetLogical3D(I3DPhysicalVariantAccessor acc, int idx);

    /// <summary>
    /// Returns a list of the used axis style ids for this layer.
    /// </summary>
    System.Collections.Generic.IEnumerable<CSLineID> AxisStyleIDs { get; }

    /// <summary>
    /// Updates the logical value of a plane id in case it uses a physical value.
    /// </summary>
    /// <param name="id">The plane identifier</param>
    void UpdateCSPlaneID(CSPlaneID id);


  }
}
