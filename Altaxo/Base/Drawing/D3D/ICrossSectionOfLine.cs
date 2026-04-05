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

#nullable enable
using Altaxo.Geometry;

namespace Altaxo.Drawing.D3D
{
  /// <summary>
  /// Describes the cross section of a 3D line.
  /// </summary>
  public interface ICrossSectionOfLine : Altaxo.Main.IImmutable
  {
    /// <summary>
    /// Gets the first size parameter of the cross section.
    /// </summary>
    double Size1 { get; }
    /// <summary>
    /// Gets the second size parameter of the cross section.
    /// </summary>
    double Size2 { get; }

    /// <summary>
    /// Returns a copy of the cross section with the specified sizes.
    /// </summary>
    /// <param name="size1">The first size parameter.</param>
    /// <param name="size2">The second size parameter.</param>
    /// <returns>A cross section with the specified sizes.</returns>
    ICrossSectionOfLine WithSize(double size1, double size2);

    /// <summary>
    /// Returns a copy of the cross section with the specified first size.
    /// </summary>
    /// <param name="size1">The first size parameter.</param>
    /// <returns>A cross section with the specified first size.</returns>
    ICrossSectionOfLine WithSize1(double size1);

    /// <summary>
    /// Returns a copy of the cross section with the specified second size.
    /// </summary>
    /// <param name="size2">The second size parameter.</param>
    /// <returns>A cross section with the specified second size.</returns>
    ICrossSectionOfLine WithSize2(double size2);

    /// <summary>
    /// Determines whether the vertex at the specified index is sharp.
    /// </summary>
    /// <param name="idx">The vertex index.</param>
    /// <returns><see langword="true"/> if the vertex is sharp; otherwise, <see langword="false"/>.</returns>
    bool IsVertexSharp(int idx);

    /// <summary>
    /// Returns the normal at the specified index.
    /// </summary>
    /// <param name="i">The normal index.</param>
    /// <returns>The normal at the specified index.</returns>
    VectorD2D Normals(int i);

    /// <summary>
    /// Gets the number of normals.
    /// </summary>
    int NumberOfNormals { get; }
    /// <summary>
    /// Gets the number of vertices.
    /// </summary>
    int NumberOfVertices { get; }

    /// <summary>
    /// Returns the vertex at the specified index.
    /// </summary>
    /// <param name="i">The vertex index.</param>
    /// <returns>The vertex at the specified index.</returns>
    PointD2D Vertices(int i);

    /// <summary>
    /// Gets the maximal distance of the cross section from its center.
    /// </summary>
    /// <returns>The maximal distance from the center.</returns>
    double GetMaximalDistanceFromCenter();
  }
}
