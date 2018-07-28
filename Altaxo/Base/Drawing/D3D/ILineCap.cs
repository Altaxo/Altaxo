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

using Altaxo.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Drawing.D3D
{
  public interface ILineCap
  {
    /// <summary>
    /// Gets the absolute base inset using the thickness1 and thickness2 of the pen.
    /// </summary>
    /// <param name="thickness1">The thickness1.</param>
    /// <param name="thickness2">The thickness2.</param>
    /// <returns>The base inset as abolute value. If this value is negative, the line is shortened by this value, and the cap is drawn from the end of the shortened line.
    /// If this value is zero, the cap is drawn starting at the line end. If this value is positive,
    /// the cap is also drawn from the end of the line. In this case the cap itself is reponsible for taking the offset into account.</returns>
    double GetAbsoluteBaseInset(double thickness1, double thickness2);

    double MinimumRelativeSize { get; }

    double MinimumAbsoluteSizePt { get; }

    string Name { get; }

    ILineCap WithMinimumAbsoluteAndRelativeSize(double absoluteSizePt, double relativeSize);

    /// <summary>
    /// Adds the triangle geometry for this cap.
    /// </summary>
    /// <param name="AddPositionAndNormal">The procedure to add a vertex position and normal.</param>
    /// <param name="AddIndices">The procedure to add vertex indices for one triangle.</param>
    /// <param name="vertexIndexOffset">The vertex index offset. Must be actualized during this call.</param>
    /// <param name="isStartCap">If set to <c>true</c>, a start cap is drawn; otherwise, an end cap is drawn.</param>
    /// <param name="basePoint">The base point of the cap.</param>
    /// <param name="westVector">The west vector of the cross section.</param>
    /// <param name="northVector">The north vector of the cross section.</param>
    /// <param name="forwardVectorNormalized">The forward vector of the line or line segment. Must be normalized.</param>
    /// <param name="lineCrossSection">The line cross section.</param>
    /// <param name="baseCrossSectionPositions">The cross section positions at the base of the cap, or null if the line does not end at the cap base.</param>
    /// <param name="baseCrossSectionNormals">The cross section normals at the base of the cap, or null.</param>
    /// <param name="temporaryStorageSpace">Object which represents temporary storage space used by the cap. The storage space can be used again if the returned object is again provided in subsequent calls.</param>
    void AddGeometry(
    Action<PointD3D, VectorD3D> AddPositionAndNormal,
      Action<int, int, int, bool> AddIndices,
      ref int vertexIndexOffset,
      bool isStartCap,
      PointD3D basePoint,
      VectorD3D westVector,
      VectorD3D northVector,
      VectorD3D forwardVectorNormalized,
      ICrossSectionOfLine lineCrossSection,
      PointD3D[] baseCrossSectionPositions,
      VectorD3D[] baseCrossSectionNormals,
      ref object temporaryStorageSpace
    );
  }
}
