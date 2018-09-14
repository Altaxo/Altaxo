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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altaxo.Geometry;

namespace Altaxo.Drawing.D3D
{
  /// <summary>
  /// Contains code to generate triangle geometry for solid straight dash segments.
  /// This structure needs to be initialized only once per line with <see cref="Initialize(PenX3D, VectorD3D, VectorD3D, LineD3D)"/>.
  /// It then can be used for each individual dash segment by calling <see cref="AddGeometry(Action{PointD3D, VectorD3D}, Action{int, int, int, bool}, ref int, LineD3D, ILineCap, ILineCap)"/>.
  /// </summary>
  public struct SolidStraightLineDashSegment
  {
    // global Variables, initialized only one per line (not once per dash segment)
    private ICrossSectionOfLine _crossSection;

    private int _crossSectionVertexCount;
    private int _crossSectionNormalCount;
    private ILineCap _dashStartCap;
    private double _dashStartCapBaseInsetAbsolute;
    private ILineCap _dashEndCap;
    private double _dashEndCapBaseInsetAbsolute;
    private VectorD3D _westVector;
    private VectorD3D _northVector;
    private VectorD3D _forwardVector;
    private VectorD3D[] _lastNormalsTransformed;

    // local variables, i.e. variables that change with every dash segment
    private PointD3D[] _lastPositionsTransformedStart;

    private PointD3D[] _lastPositionsTransformedEnd;

    private object _startCapTemporaryStorageSpace;

    private object _endCapTemporaryStorageSpace;

    /// <summary>
    /// Initialization that is needed only once per straigth line (not once per dash).
    /// </summary>
    /// <param name="pen">The pen that is used to draw the line.</param>
    /// <param name="west">The west vector.</param>
    /// <param name="north">The north vector.</param>
    /// <param name="line">The global line to draw. This argument is needed to extract the line vector, which for a straight line is also the line vector for each individual dash segment.</param>
    public void Initialize(
      PenX3D pen,
      VectorD3D west,
      VectorD3D north,
      LineD3D line)
    {
      Initialize(
        pen.CrossSection,
        pen.Thickness1,
        pen.Thickness2,
        pen.DashStartCap,
        pen.DashEndCap,
        west,
        north,
        line
        );
    }

    public void Initialize(
    ICrossSectionOfLine crossSection,
    double thickness1,
    double thickness2,
    ILineCap startCap,
    ILineCap endCap,
    VectorD3D westVector,
    VectorD3D northVector,
    LineD3D line)
    {
      _crossSection = crossSection;
      _crossSectionVertexCount = crossSection.NumberOfVertices;
      _crossSectionNormalCount = crossSection.NumberOfNormals;
      _dashStartCap = startCap;
      _dashStartCapBaseInsetAbsolute = null == _dashStartCap ? 0 : _dashStartCap.GetAbsoluteBaseInset(thickness1, thickness2);
      _dashEndCap = endCap;
      _dashEndCapBaseInsetAbsolute = null == _dashEndCap ? 0 : _dashEndCap.GetAbsoluteBaseInset(thickness1, thickness2);
      _westVector = westVector;
      _northVector = northVector;
      _forwardVector = line.LineVectorNormalized;
      _lastNormalsTransformed = new VectorD3D[_crossSectionNormalCount];
      _lastPositionsTransformedStart = new PointD3D[_crossSectionVertexCount];
      _lastPositionsTransformedEnd = new PointD3D[_crossSectionVertexCount];

      // Get the matrix for the start plane
      var matrix = Math3D.Get2DProjectionToPlane(westVector, northVector, PointD3D.Empty);

      // note: for a single line segment, the normals need to be calculated only once

      for (int i = 0; i < _lastNormalsTransformed.Length; ++i)
      {
        _lastNormalsTransformed[i] = matrix.Transform(crossSection.Normals(i));
      }
    }

    public void AddGeometry(
    Action<PointD3D, VectorD3D> AddPositionAndNormal,
    Action<int, int, int, bool> AddIndices,
    ref int vertexIndexOffset,
    LineD3D dashSegment,
      ILineCap overrideStartCap,
      ILineCap overrideEndCap)
    {
      if (null == _lastNormalsTransformed)
        throw new InvalidProgramException("The structure is not initialized yet. Call Initialize before using it!");

      PointD3D lineStart = dashSegment.P0;
      PointD3D lineEnd = dashSegment.P1;

      var lineVector = dashSegment.LineVector;
      double lineLength = lineVector.Length;

      if (null != _dashStartCap && null == overrideStartCap)
      {
        if (_dashStartCapBaseInsetAbsolute < 0)
        {
          lineStart += -_dashStartCapBaseInsetAbsolute * _forwardVector;
          lineLength += _dashStartCapBaseInsetAbsolute;
        }
      }

      if (null != _dashEndCap && null == overrideEndCap)
      {
        if (_dashEndCapBaseInsetAbsolute < 0)
        {
          lineEnd += _dashEndCapBaseInsetAbsolute * _forwardVector;
          lineLength += _dashEndCapBaseInsetAbsolute;
        }
      }

      AddGeometry(AddPositionAndNormal,
        AddIndices,
        ref vertexIndexOffset,
        lineStart,
        lineEnd,
        lineLength > 0,
        overrideStartCap,
        overrideEndCap);
    }

    /// <summary>
    /// Adds the triangle geometry. Here, the position of the startcap base and of the endcap base is already calculated and provided in the arguments.
    /// </summary>
    /// <param name="AddPositionAndNormal">The procedure to add a vertex position and normal.</param>
    /// <param name="AddIndices">The procedure to add vertex indices for one triangle.</param>
    /// <param name="vertexIndexOffset">The vertex index offset.</param>
    /// <param name="lineStart">The line start. This is the precalculated base of the start line cap.</param>
    /// <param name="lineEnd">The line end. Here, this is the precalculated base of the end line cap.</param>
    /// <param name="drawLine">If this parameter is true, the line segment between lineStart and lineEnd is drawn. If false, the line segment itself is not drawn, but the start end end caps are drawn.</param>
    /// <param name="overrideStartCap">If not null, this parameter override the start cap that is stored in this class.</param>
    /// <param name="overrideEndCap">If not null, this parameter overrides the end cap that is stored in this class.</param>
    /// <exception cref="System.InvalidProgramException">The structure is not initialized yet. Call Initialize before using it!</exception>
    public void AddGeometry(
      Action<PointD3D, VectorD3D> AddPositionAndNormal,
      Action<int, int, int, bool> AddIndices,
      ref int vertexIndexOffset,
      PointD3D lineStart,
      PointD3D lineEnd,
      bool drawLine,
      ILineCap overrideStartCap,
      ILineCap overrideEndCap
      )
    {
      if (null == _lastNormalsTransformed)
        throw new InvalidProgramException("The structure is not initialized yet. Call Initialize before using it!");

      var resultingStartCap = overrideStartCap ?? _dashStartCap;
      var resultingEndCap = overrideEndCap ?? _dashEndCap;

      // draw the straight line if the remaining line length is >0
      if (drawLine)
      {
        // Get the matrix for the start plane
        var matrix = Math3D.Get2DProjectionToPlane(_westVector, _northVector, lineStart);
        for (int i = 0; i < _crossSectionVertexCount; ++i)
          _lastPositionsTransformedStart[i] = matrix.Transform(_crossSection.Vertices(i));

        matrix = Math3D.Get2DProjectionToPlane(_westVector, _northVector, lineEnd);
        for (int i = 0; i < _crossSectionVertexCount; ++i)
          _lastPositionsTransformedEnd[i] = matrix.Transform(_crossSection.Vertices(i));

        // draw the line segment now
        var currIndex = vertexIndexOffset;
        for (int i = 0, j = 0; i < _crossSectionVertexCount; ++i, ++j)
        {
          if (j == 0)
          {
            AddIndices(currIndex, currIndex + 1, currIndex + 2 * _crossSectionNormalCount - 2, false);
            AddIndices(currIndex + 2 * _crossSectionNormalCount - 2, currIndex + 1, currIndex + 2 * _crossSectionNormalCount - 1, false);
          }
          else
          {
            AddIndices(currIndex, currIndex + 1, currIndex - 2, false);
            AddIndices(currIndex - 2, currIndex + 1, currIndex - 1, false);
          }

          AddPositionAndNormal(_lastPositionsTransformedStart[i], _lastNormalsTransformed[j]);
          AddPositionAndNormal(_lastPositionsTransformedEnd[i], _lastNormalsTransformed[j]);
          currIndex += 2;

          if (_crossSection.IsVertexSharp(i))
          {
            ++j;
            AddPositionAndNormal(_lastPositionsTransformedStart[i], _lastNormalsTransformed[j]);
            AddPositionAndNormal(_lastPositionsTransformedEnd[i], _lastNormalsTransformed[j]);
            currIndex += 2;
          }
        }
        vertexIndexOffset = currIndex;
      }

      // now the start cap
      if (null != resultingStartCap)
      {
        resultingStartCap.AddGeometry(
          AddPositionAndNormal,
          AddIndices,
          ref vertexIndexOffset,
          true,
          lineStart,
          _westVector,
          _northVector,
          _forwardVector,
          _crossSection,
          drawLine ? _lastPositionsTransformedStart : null,
          _lastNormalsTransformed,
          ref _startCapTemporaryStorageSpace);
      }
      else if (drawLine)
      {
        LineCaps.Flat.AddGeometry(
          AddPositionAndNormal,
          AddIndices,
          ref vertexIndexOffset,
          true,
          lineStart,
          _forwardVector,
          _lastPositionsTransformedStart
          );
      }

      if (null != resultingEndCap)
      {
        resultingEndCap.AddGeometry(
        AddPositionAndNormal,
          AddIndices,
          ref vertexIndexOffset,
          false,
          lineEnd,
          _westVector,
          _northVector,
          _forwardVector,
          _crossSection,
          drawLine ? _lastPositionsTransformedEnd : null,
          _lastNormalsTransformed,
          ref _endCapTemporaryStorageSpace);
      }
      else if (drawLine)
      {
        LineCaps.Flat.AddGeometry(
          AddPositionAndNormal,
          AddIndices,
          ref vertexIndexOffset,
          false,
          lineEnd,
          _forwardVector,
          _lastPositionsTransformedEnd
          );
      }
    }
  }
}
