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

#nullable enable
namespace Altaxo.Drawing.D3D
{
  using Altaxo.Geometry;

  /// <summary>
  /// Represents a collection of 2D vertices together with an index array describing triangles.
  /// Each consecutive group of three indices in <see cref="TriangleIndices"/> defines one triangle
  /// referencing elements in <see cref="Vertices"/>.
  /// </summary>
  public class IndexedTriangles
  {
    /// <summary>
    /// Gets the array of 2D vertex positions used by the triangles.
    /// </summary>
    public PointD2D[] Vertices { get; private set; }

    /// <summary>
    /// Gets the index array. Each consecutive group of three indices defines a triangle.
    /// Indices refer to entries in <see cref="Vertices"/>.
    /// </summary>
    public int[] TriangleIndices { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="IndexedTriangles"/> class.
    /// </summary>
    /// <param name="vertices">The vertex positions.</param>
    /// <param name="triangleIndices">The triangle index array (groups of three indices).</param>
    public IndexedTriangles(PointD2D[] vertices, int[] triangleIndices)
    {
      Vertices = vertices;
      TriangleIndices = triangleIndices;
    }
  }
}
