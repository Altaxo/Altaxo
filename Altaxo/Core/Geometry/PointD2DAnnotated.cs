#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Geometry
{
  /// <summary>
  /// Represents a point in 2D space, identified by an <see cref="System.Int32"/> identifier.
  /// </summary>
  public struct PointD2DAnnotated
  {
    /// <summary>Gets the identifier of this point.</summary>
    public int ID { get; private set; }
    /// <summary>Gets the x position of the point.</summary>
    public double X { get; private set; }
    /// <summary>Gets the y position of the point.</summary>
    public double Y { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PointD2DAnnotated"/> struct.
    /// </summary>
    /// <param name="x">The x position of the point.</param>
    /// <param name="y">The y position of the point.</param>
    /// <param name="id">The identifier of the point.</param>
    public PointD2DAnnotated(double x, double y, int id)
    {
      X = x;
      Y = y;
      ID = id;
    }

    /// <summary>
    /// Deconstructs this instance into its components.
    /// </summary>
    /// <param name="x">The x position of the point.</param>
    /// <param name="y">The y position of the point.</param>
    /// <param name="id">The identifier of the point.</param>
    public void Deconstruct(out double x, out double y, out int id)
    {
      x = X;
      y = Y;
      id = ID;
    }

    /// <summary>
    /// Gets the underlying 2D point.
    /// </summary>
    public PointD2D Point { get { return new PointD2D(X, Y); } }

    /// <summary>
    /// Performs an implicit conversion from <see cref="PointD2DAnnotated"/> to <see cref="PointD2D"/>.
    /// </summary>
    /// <param name="annotatedPoint">The annotated point.</param>
    /// <returns>The underlying 2D point.</returns>
    public static implicit operator PointD2D(PointD2DAnnotated annotatedPoint)
    {
      return annotatedPoint.Point;
    }
  }
}
