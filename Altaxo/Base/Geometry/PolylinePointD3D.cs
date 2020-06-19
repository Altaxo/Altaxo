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

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Geometry
{
  /// <summary>
  /// Represents a polyline point with forward, west, and north vector, and its position.
  /// This is equivalent to a <see cref="Matrix4x3"/>, but without the overhead for calculating the determinant.
  /// </summary>
  public struct PolylinePointD3D
  {
    public VectorD3D ForwardVector;
    public VectorD3D WestVector;
    public VectorD3D NorthVector;
    public PointD3D Position;

    public PolylinePointD3D(VectorD3D forwardVector, VectorD3D westVector, VectorD3D northVector, PointD3D position)
    {
      Position = position;
      WestVector = westVector;
      NorthVector = northVector;
      ForwardVector = forwardVector;
    }

    public PolylinePointD3D WithPosition(PointD3D position)
    {
      var result = this;
      result.Position = position;
      return result;
    }
  }

  /// <summary>
  /// Represents a polyline point with forward, west, and north vector, and its position.
  /// This is equivalent to a <see cref="Matrix4x3"/>, but without the overhead for calculating the determinant.
  /// </summary>
  public class PolylinePointD3DAsClass
  {
    public VectorD3D ForwardVector;
    public VectorD3D WestVector;
    public VectorD3D NorthVector;
    public PointD3D Position;

    public PolylinePointD3DAsClass()
    {
    }

    public PolylinePointD3DAsClass(VectorD3D forwardVector, VectorD3D westVector, VectorD3D northVector, PointD3D position)
    {
      Position = position;
      WestVector = westVector;
      NorthVector = northVector;
      ForwardVector = forwardVector;
    }
  }
}
