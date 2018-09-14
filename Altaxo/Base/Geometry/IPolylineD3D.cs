#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2015 Dr. Dirk Lellinger
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Geometry;

namespace Altaxo.Geometry
{
  /// <summary>
  /// Represents a polyline, i.e. a line consisting of multiple line segments. This class contains additional information whether the joints between the line segments
  /// are sharp or soft.
  /// </summary>
  public interface IPolylineD3D
  {
    PointD3D GetPoint(int idx);

    /// <summary>
    /// Gets the number of points.
    /// </summary>
    /// <value>
    /// Number of points.
    /// </value>
    int Count { get; }

    /// <summary>
    /// Gets the points of this polyline. No information is contained here whether the joints are sharp or soft.
    /// </summary>
    /// <value>
    /// The points that make out the polyline.
    /// </value>
    IList<PointD3D> Points { get; } // TODO change this to IReadonlyList

    bool IsTransitionFromIdxToNextIdxSharp(int idx);

    /// <summary>
    /// Gets the total line length of the polyline.
    /// </summary>
    /// <value>
    /// The total length of the polyline.
    /// </value>
    double TotalLineLength { get; }

    /// <summary>
    /// Returns a new, shortened polyline. If the shortened line would have zero or negative length, <c>null</c> is returned.
    /// </summary>
    /// <param name="marginAtStart">The margin at start. Either an absolute value, or relative to the total length of the polyline.</param>
    /// <param name="marginAtEnd">The margin at end. Either an absolute value, or relative to the total length of the polyline.</param>
    /// <returns>A new, shortened polyline. If the shortened line would have zero or negative length, <c>null</c> is returned.</returns>
    IPolylineD3D ShortenedBy(RADouble marginAtStart, RADouble marginAtEnd);
  }
}
