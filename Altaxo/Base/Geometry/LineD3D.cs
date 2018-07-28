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

namespace Altaxo.Geometry
{
  /// <summary>
  /// A straight line in 3D space.
  /// </summary>
  public struct LineD3D
  {
    private PointD3D _p0;
    private PointD3D _p1;

    /// <summary>
    /// Initializes a new instance of the <see cref="LineD3D"/> struct.
    /// </summary>
    /// <param name="p0">The starting point of the line.</param>
    /// <param name="p1">The end point of the line.</param>
    public LineD3D(PointD3D p0, PointD3D p1)
    {
      _p0 = p0;
      _p1 = p1;
    }

    /// <summary>
    /// Returns a new line with <see cref="P0"/> set to the provided value.
    /// </summary>
    /// <param name="p0">The value for <see cref="P0"/>.</param>
    /// <returns>A new line with <see cref="P0"/> set to the provided value.</returns>
    public LineD3D WithP0(PointD3D p0)
    {
      return new LineD3D(p0, P1);
    }

    /// <summary>
    /// Returns a new line with <see cref="P1"/> set to the provided value.
    /// </summary>
    /// <param name="p1">The value for <see cref="P1"/>.</param>
    /// <returns>A new line with <see cref="P1"/> set to the provided value.</returns>
    public LineD3D WithP1(PointD3D p1)
    {
      return new LineD3D(P0, p1);
    }

    /// <summary>
    /// Gets the starting point of the line.
    /// </summary>
    /// <value>
    /// The starting point of the line.
    /// </value>
    public PointD3D P0 { get { return _p0; } }

    /// <summary>
    /// Gets the end point of the line.
    /// </summary>
    /// <value>
    /// The end point of the line.
    /// </value>
    public PointD3D P1 { get { return _p1; } }

    /// <summary>
    /// Gets the vector from <see cref="P0"/> to <see cref="P1"/>.
    /// </summary>
    /// <value>
    /// The vector from <see cref="P0"/> to <see cref="P1"/>.
    /// </value>
    public VectorD3D Vector { get { return _p1 - _p0; } }

    /// <summary>
    /// Gets the length of the line.
    /// </summary>
    /// <value>
    /// The length of the line.
    /// </value>
    public double Length { get { return (_p1 - _p0).Length; } }

    public VectorD3D LineVector { get { return _p1 - _p0; } }

    public VectorD3D LineVectorNormalized { get { return VectorD3D.CreateNormalized(_p1.X - _p0.X, _p1.Y - _p0.Y, _p1.Z - _p0.Z); } }

    /// <summary>
    /// Gets the points of the line as enumeration.
    /// </summary>
    /// <returns>Enumeration of the two points of this line.</returns>
    public IEnumerable<PointD3D> GetPoints()
    {
      yield return _p0;
      yield return _p1;
    }

    /// <summary>
    /// Gets the point at this line from a relative value.
    /// </summary>
    /// <param name="relValue">The relative value. If 0, the start point <see cref="P0"/> is returned. If 1, the end point <see cref="P1"/> is returned.</param>
    /// <returns></returns>
    public PointD3D GetPointAtLineFromRelativeValue(double relValue)
    {
      if (relValue == 0)
        return _p0;
      else if (relValue == 1)
        return _p1;
      else
        return new PointD3D(
        (1 - relValue) * _p0.X + (relValue) * _p1.X,
        (1 - relValue) * _p0.Y + (relValue) * _p1.Y,
        (1 - relValue) * _p0.Z + (relValue) * _p1.Z);
    }
  }
}
