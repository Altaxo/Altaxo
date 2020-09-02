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
using Altaxo.Geometry;

namespace Altaxo.Drawing.D3D.LineCaps
{
  public class Flat : ILineCap
  {
    private static Flat _instance = new Flat();

    /// <summary>
    /// Gets an instance of this class.
    /// </summary>
    /// <value>
    /// The instance.
    /// </value>
    public static Flat Instance { get { return _instance; } }

    /// <inheritdoc />
    public double BaseInset
    {
      get
      {
        return 0;
      }
    }

    public double MinimumRelativeSize
    {
      get
      {
        return 1;
      }
    }

    public double MinimumAbsoluteSizePt
    {
      get
      {
        return 0;
      }
    }

    public virtual string Name
    {
      get
      {
        return GetType().FullName ?? GetType().Name;
      }
    }

    /// <inheritdoc />
    public double GetAbsoluteBaseInset(double thickness1, double thickness2)
    {
      return 0;
    }

    /// <inheritdoc />
    public void AddGeometry(
      Action<PointD3D, VectorD3D> AddPositionAndNormal,
      Action<int, int, int, bool> AddIndices,
      ref int vertexIndexOffset,
      bool isStartCap,
      PointD3D basePoint,
      VectorD3D eastVector,
      VectorD3D northVector,
      VectorD3D forwardVectorNormalized,
      ICrossSectionOfLine lineCrossSection,
      PointD3D[]? baseCrossSectionPositions,
      VectorD3D[]? baseCrossSectionNormals,
      ref object? temporaryStorageSpace)
    {
      var crossSectionVertexCount = lineCrossSection.NumberOfVertices;
      var crossSectionNormalCount = lineCrossSection.NumberOfNormals;

      var capCrossSectionPositions = baseCrossSectionPositions ?? (PointD3D[]?)temporaryStorageSpace ?? (PointD3D[])(temporaryStorageSpace = new PointD3D[crossSectionVertexCount]);

      if (baseCrossSectionPositions is null) // if null the positions were not provided
      {
        var matrix = Math3D.Get2DProjectionToPlane(eastVector, northVector, basePoint);
        for (int i = 0, j = 0; i < crossSectionVertexCount; ++i, ++j)
        {
          capCrossSectionPositions[i] = matrix.Transform(lineCrossSection.Vertices(i));
        }
      }

      AddGeometry(AddPositionAndNormal, AddIndices, ref vertexIndexOffset, isStartCap, basePoint, forwardVectorNormalized, capCrossSectionPositions);
    }

    /// <summary>
    /// Adds the triangle geometry for this cap.
    /// </summary>
    /// <param name="AddPositionAndNormal">The procedure to add a vertex position and normal.</param>
    /// <param name="AddIndices">The procedure to add vertex indices for one triangle.</param>
    /// <param name="vertexIndexOffset">The vertex index offset. Must be actualized during this call.</param>
    /// <param name="isStartCap">If set to <c>true</c>, a start cap is drawn; otherwise, an end cap is drawn.</param>
    /// <param name="basePoint">The base point of the cap.</param>
    /// <param name="forwardVectorNormalized">The forward vector of the line or line segment. Must be normalized.</param>
    /// <param name="baseCrossSectionPositions">The base cross section positions.</param>
    public static void AddGeometry(
      Action<PointD3D, VectorD3D> AddPositionAndNormal,
      Action<int, int, int, bool> AddIndices,
      ref int vertexIndexOffset,
      bool isStartCap,
      PointD3D basePoint,
      VectorD3D forwardVectorNormalized,
      PointD3D[] baseCrossSectionPositions)
    {
      if (baseCrossSectionPositions is null)
        throw new ArgumentNullException(nameof(baseCrossSectionPositions));
      // and now the cap

      if (isStartCap)
        forwardVectorNormalized = -forwardVectorNormalized;

      int currIndex = vertexIndexOffset;
      int crossSectionPositionCount = baseCrossSectionPositions.Length;

      // Add the midpoint
      // add the middle point of the end cap and the normal of the end cap
      AddPositionAndNormal(basePoint, forwardVectorNormalized);
      ++currIndex;

      for (int i = 0; i < crossSectionPositionCount; ++i)
      {
        AddPositionAndNormal(baseCrossSectionPositions[i], forwardVectorNormalized);

        AddIndices(
        currIndex - 1, // mid point of the end cap
        currIndex + i,
        currIndex + (1 + i) % crossSectionPositionCount,
        isStartCap);
      }

      currIndex += crossSectionPositionCount;
      vertexIndexOffset = currIndex;
    }

    /// <summary>
    /// Adds the triangle geometry for this cap.
    /// </summary>
    /// <param name="AddPositionAndNormal">The procedure to add a vertex position and normal.</param>
    /// <param name="AddIndices">The procedure to add vertex indices for one triangle.</param>
    /// <param name="vertexIndexOffset">The vertex index offset. Must be actualized during this call.</param>
    /// <param name="isStartCap">If set to <c>true</c>, a start cap is drawn; otherwise, an end cap is drawn.</param>
    /// <param name="basePoint">The location of the middle point of the line at the cap's location.</param>
    /// <param name="westVector">The west vector for orientation of the cross section.</param>
    /// <param name="northVector">The north vector for orientation of the cross section.</param>
    /// <param name="forwardVector">The forward vector for orientation of the cross section.</param>
    /// <param name="crossSection">The cross section of the line.</param>
    public static void AddGeometry(
      Action<PointD3D, VectorD3D> AddPositionAndNormal,
      Action<int, int, int, bool> AddIndices,
      ref int vertexIndexOffset,
      bool isStartCap,
      PointD3D basePoint,
      VectorD3D westVector,
      VectorD3D northVector,
      VectorD3D forwardVector,
      ICrossSectionOfLine crossSection
      )
    {
      if (isStartCap)
        forwardVector = -forwardVector;

      var matrix = Matrix4x3.NewFromBasisVectorsAndLocation(westVector, northVector, forwardVector, basePoint);

      int currIndex = vertexIndexOffset;
      int crossSectionPositionCount = crossSection.NumberOfVertices;

      // Add the midpoint
      // add the middle point of the end cap and the normal of the end cap
      AddPositionAndNormal(basePoint, forwardVector);
      ++currIndex;

      for (int i = 0; i < crossSectionPositionCount; ++i)
      {
        var sp = matrix.Transform(crossSection.Vertices(i));
        AddPositionAndNormal(sp, forwardVector);

        AddIndices(
        currIndex - 1, // mid point of the end cap
        currIndex + i,
        currIndex + (1 + i) % crossSectionPositionCount,
        isStartCap);
      }

      currIndex += crossSectionPositionCount;
      vertexIndexOffset = currIndex;
    }

    public ILineCap WithMinimumAbsoluteAndRelativeSize(double absoluteSizePt, double relativeSize)
    {
      return _instance;
    }

    public override bool Equals(object? obj)
    {
      return obj is Flat;
    }

    public override int GetHashCode()
    {
      return typeof(Flat).GetHashCode();
    }
  }
}
