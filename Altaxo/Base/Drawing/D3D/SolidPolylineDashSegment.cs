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

using Altaxo.Drawing.D3D.CrossSections;
using Altaxo.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Drawing.D3D
{
  /// <summary>
  /// Contains code to generate triangle geometry for solid polyline dash segments.
  /// This structure needs to be initialized only once per line with <see cref="Initialize(PenX3D)"/>.
  /// It then can be used for each individual dash segment by calling AddGeometry/>.
  /// </summary>
  public struct SolidPolylineDashSegment
  {
    private static readonly double Cos1Degree = Math.Cos(1.0 * Math.PI / 180);
    private static readonly double Cos01Degree = Math.Cos(0.1 * Math.PI / 180);

    #region global Variables, initialized only once per line (not once per dash segment)

    /// <summary>The _cross section of the pen.</summary>
    private ICrossSectionOfLine _crossSection;

    /// <summary>The number of vertices of the cross section of the pen.</summary>
    private int _crossSectionVertexCount;

    /// <summary>The number of normals of the cross section of the pen.</summary>
    private int _crossSectionNormalCount;

    /// <summary>The dash start cap of the pen. In some calls, it can be overridden by the line start cap of the pen.</summary>
    private ILineCap _dashStartCap;

    /// <summary>The absolute base inset of the dash start cap of the pen. Here, absolute means absolute units, not relative units.</summary>
    private double _dashStartCapBaseInsetAbsolute;

    /// <summary>The dash end cap of the pen. In some calls, it can be overridden by the line end cap of the pen.</summary>
    private ILineCap _dashEndCap;

    /// <summary>The absolute base inset of the dash end cap of the pen. Here, absolute means absolute units, not relative units.</summary>
    private double _dashEndCapBaseInsetAbsolute;

    /// <summary>The LineJoin property of the pen.</summary>
    private PenLineJoin _lineJoin;

    /// <summary>The miter limit of the pen. Applies only if lineJoin is Miter.</summary>
    private double _miterLimit;

    /// <summary>If the line join is miter, and the DotProduct of current and next segment is above this value, there is no need to clip the line join.</summary>
    private double _miterLimitDotThreshold;

    /// <summary>The maximal distance of any vertex of the cross section from the center of the cross section.</summary>
    private double _crossSectionMaximalDistanceFromCenter;

    /// <summary>
    /// A dictionary to mesh parts of the cross section. The key is a tuple, consisting of the first vertex index of the cross section, and the last vertex index of the cross section that should be meshed.
    /// The value is an array with the triangle indices of the mesh (each 3 consecutive items form one triangle of the cross section part).
    /// </summary>
    private Dictionary<Tuple<int, int>, int[]> _crossSectionPartsTriangleIndices;

    #endregion global Variables, initialized only once per line (not once per dash segment)

    #region operational variables, i.e. variables evaluated during every call to draw

    /// <summary>If true, the position and forward vector of the start cap is already calculated and stored in <see cref="_startCapCOS"/>
    /// In cases in which <see cref="PolylinePointD3D"/>s are provided, this means that the west and north vectors were already calculated, too.</summary>
    private bool _startCapForwardAndPositionProvided;

    /// <summary>The index (can be fractional) of the polyline at the start cap base.</summary>
    private double _polylineIndexAtStartCapBase;

    /// <summary>Is a small segment at the start of the polyline neccessary, that has the direction of the start cap? It is not neccessary if the direction of the start cap is the same as the direction of the first polyline segment.</summary>
    private bool _startCapNeedsJoiningSegment;

    /// <summary>Holds position and orientation of the start cap.</summary>
    private PolylinePointD3DAsClass _startCapCOS;

    /// <summary>If true, the position and forward vector of the end cap is already calculated and stored in <see cref="_endCapCOS"/>
    /// In cases in which <see cref="PolylinePointD3D"/>s are provided, this means that the west and north vectors were already calculated, too.</summary>
    private bool _endCapForwardAndPositionProvided;

    /// <summary>Is a small segment at the end of the polyline neccessary, that has the direction of the end cap? It is not neccessary if the direction of the end cap is the same as the direction of the last polyline segment.</summary>
    private bool _endCapNeedsJoiningSegment;

    /// <summary>Holds position and orientation of the end cap.</summary>
    private PolylinePointD3DAsClass _endCapCOS;

    /// <summary>The index (can be fractional) of the polyline at the start cap base.</summary>
    private double _polylineIndexAtEndCapBase;

    #endregion operational variables, i.e. variables evaluated during every call to draw

    #region local variables, i.e. variables that change with every dash segment

    /// <summary>Temporary storage needed by the start cap drawing routine.</summary>
    private object _startCapTemporaryStorageSpace;

    /// <summary>Temporary storage needed by the end cap drawing routine.</summary>
    private object _endCapTemporaryStorageSpace;

    /// <summary>Transformed positions for the start of the current segment.</summary>
    private PointD3D[] _positionsTransformedStartCurrent;

    /// <summary>Transformed positions for the end of the current segment.</summary>
    private PointD3D[] _positionsTransformedEndCurrent;

    /// <summary>Transformed positions for the start of the next segment.</summary>
    private PointD3D[] _positionsTransformedStartNext;

    /// <summary>Transformed normals for the current segment.</summary>
    private VectorD3D[] _normalsTransformedCurrent;

    /// <summary>Transformed normals for the next segment.</summary>
    private VectorD3D[] _normalsTransformedNext;

    /// <summary>Cross section positions rotated in such a way that the point of the current joint is in x-direction of the rotated vertices.</summary>
    private VectorD2D[] _crossSectionRotatedVertices;

    #endregion local variables, i.e. variables that change with every dash segment

    /// <summary>
    /// Initialization that is needed only once per straigth line (not once per dash).
    /// </summary>
    /// <param name="pen">The pen that is used to draw the line.</param>
    public void Initialize(
      PenX3D pen
      )
    {
      Initialize(
        pen.CrossSection,
        pen.Thickness1,
        pen.Thickness2,
        pen.LineJoin,
        pen.MiterLimit,
        pen.DashStartCap,
        pen.DashEndCap
        );
    }

    /// <summary>
    /// Initialization that is needed only once per straigth line (not once per dash).
    /// </summary>
    /// <param name="crossSection">The cross section of the pen that is used to draw the line.</param>
    /// <param name="thickness1">Thickness1 of the pen.</param>
    /// <param name="thickness2">Thickness2 of the pen.</param>
    /// <param name="lineJoin">The LineJoin property of the pen.</param>
    /// <param name="miterLimit">The MiterLimit property of the pen.</param>
    /// <param name="startCap">The start cap to be used for this polyline segment.</param>
    /// <param name="endCap">The end cap to be used for this polyline segment.</param>
    public void Initialize(
    ICrossSectionOfLine crossSection,
    double thickness1,
    double thickness2,
    PenLineJoin lineJoin,
    double miterLimit,
    ILineCap startCap,
    ILineCap endCap)
    {
      this._crossSection = crossSection;
      this._crossSectionVertexCount = crossSection.NumberOfVertices;
      this._crossSectionNormalCount = crossSection.NumberOfNormals;
      this._crossSectionMaximalDistanceFromCenter = _crossSection.GetMaximalDistanceFromCenter();
      this._crossSectionPartsTriangleIndices = new Dictionary<Tuple<int, int>, int[]>();
      this._lineJoin = lineJoin;
      this._miterLimit = miterLimit;
      this._miterLimitDotThreshold = Math.Cos(Math.PI - 2 * Math.Asin(1 / miterLimit));

      this._dashStartCap = startCap;
      this._dashStartCapBaseInsetAbsolute = null == _dashStartCap ? 0 : _dashStartCap.GetAbsoluteBaseInset(thickness1, thickness2);
      this._dashEndCap = endCap;
      this._dashEndCapBaseInsetAbsolute = null == _dashEndCap ? 0 : _dashEndCap.GetAbsoluteBaseInset(thickness1, thickness2);

      _positionsTransformedStartCurrent = new PointD3D[_crossSectionVertexCount];
      _positionsTransformedEndCurrent = new PointD3D[_crossSectionVertexCount];
      _positionsTransformedStartNext = new PointD3D[_crossSectionVertexCount];
      _normalsTransformedCurrent = new VectorD3D[_crossSectionNormalCount];
      _normalsTransformedNext = new VectorD3D[_crossSectionNormalCount];
      _crossSectionRotatedVertices = new VectorD2D[_crossSectionVertexCount];
      _startCapCOS = new PolylinePointD3DAsClass();
      _endCapCOS = new PolylinePointD3DAsClass();
    }

    /// <summary>
    /// Adds the triangle geometry for a polyline segment with start and end cap.
    /// </summary>
    /// <param name="AddPositionAndNormal">The procedure to add a vertex position and normal.</param>
    /// <param name="AddIndices">The procedure to add vertex indices for one triangle.</param>
    /// <param name="vertexIndexOffset">The vertex index offset.</param>
    /// <param name="polylinePoints">The points of the original polyline to draw (not shortened to account for start and end cap). Here, the polyline points are already amended with orientation vectors.</param>
    /// <param name="overrideStartCap">If not null, this parameter override the start cap that is stored in this class.</param>
    /// <param name="overrideEndCap">If not null, this parameter overrides the end cap that is stored in this class.</param>
    public void AddGeometry(
  Action<PointD3D, VectorD3D> AddPositionAndNormal,
  Action<int, int, int, bool> AddIndices,
  ref int vertexIndexOffset,
  IList<PolylinePointD3D> polylinePoints,
  ILineCap overrideStartCap,
  ILineCap overrideEndCap)
    {
      if (null == _normalsTransformedCurrent)
        throw new InvalidProgramException("The structure is not initialized yet. Call Initialize before using it!");

      _polylineIndexAtStartCapBase = 0;
      _startCapForwardAndPositionProvided = false;
      _startCapNeedsJoiningSegment = false;

      _polylineIndexAtEndCapBase = polylinePoints.Count - 1;
      _endCapForwardAndPositionProvided = false;
      _endCapNeedsJoiningSegment = false;

      if (null != _dashStartCap && null == overrideStartCap)
      {
        if (_dashStartCapBaseInsetAbsolute < 0)
        {
          _polylineIndexAtStartCapBase = PolylineMath3D.GetFractionalStartIndexOfPolylineWithCapInsetAbsolute(
            polylinePoints,
            -_dashStartCapBaseInsetAbsolute,
            out _startCapForwardAndPositionProvided,
            out _startCapNeedsJoiningSegment,
            _startCapCOS);
        }
      }

      if (null != _dashEndCap && null == overrideEndCap)
      {
        if (_dashEndCapBaseInsetAbsolute < 0)
        {
          _polylineIndexAtEndCapBase = PolylineMath3D.GetFractionalEndIndexOfPolylineWithCapInsetAbsolute(
            polylinePoints,
            -_dashEndCapBaseInsetAbsolute,
            out _endCapForwardAndPositionProvided,
            out _endCapNeedsJoiningSegment,
            _endCapCOS);
        }
      }

      bool isLineDrawn = _polylineIndexAtEndCapBase > _polylineIndexAtStartCapBase && !double.IsNaN(_polylineIndexAtStartCapBase) && !double.IsNaN(_polylineIndexAtEndCapBase);

      if (!isLineDrawn) // if no line is drawn, then we must provide the start cap and end cap at least with the west and north vectors
      {
        // note because here we have used IList<PolylinePointD3D>, if _startCapForwardAndPositionProvided is true, the startCapCOS is already provided with west and north
        if (!_startCapForwardAndPositionProvided)
        {
          _startCapCOS.WestVector = polylinePoints[0].WestVector;
          _startCapCOS.NorthVector = polylinePoints[0].NorthVector;
          PolylineMath3D.GetWestAndNorthVectorsForPreviousSegment(_startCapCOS.ForwardVector, polylinePoints[0].ForwardVector, ref _startCapCOS.WestVector, ref _startCapCOS.NorthVector);
        }

        // note because here we have used IList<PolylinePointD3D>, if _endCapForwardAndPositionProvided is true, the endCapCOS is already provided with west and north
        if (!_endCapForwardAndPositionProvided)
        {
          _endCapCOS.WestVector = polylinePoints[polylinePoints.Count - 1].WestVector;
          _endCapCOS.NorthVector = polylinePoints[polylinePoints.Count - 1].NorthVector;
          PolylineMath3D.GetWestAndNorthVectorsForNextSegment(_endCapCOS.ForwardVector, polylinePoints[polylinePoints.Count - 1].ForwardVector, ref _endCapCOS.WestVector, ref _endCapCOS.NorthVector);
        }
      }

      AddGeometry(AddPositionAndNormal,
        AddIndices,
        ref vertexIndexOffset,
        isLineDrawn ? PolylineMath3D.GetPolylineWithFractionalStartAndEndIndex(polylinePoints, _polylineIndexAtStartCapBase, _polylineIndexAtEndCapBase, _startCapForwardAndPositionProvided, _startCapNeedsJoiningSegment, _startCapCOS, _endCapForwardAndPositionProvided, _endCapNeedsJoiningSegment, _endCapCOS) : PolylineMath3D.GetEmptyPolyline(),
        isLineDrawn,
        overrideStartCap,
        overrideEndCap);
    }

    /// <summary>
    /// Adds the triangle geometry for a polyline segment with start and end cap.
    /// </summary>
    /// <param name="AddPositionAndNormal">The procedure to add a vertex position and normal.</param>
    /// <param name="AddIndices">The procedure to add vertex indices for one triangle.</param>
    /// <param name="vertexIndexOffset">The vertex index offset.</param>
    /// <param name="polylinePoints">The points of the original polyline to draw (not shortened to account for start and end cap).</param>
    /// <param name="westVector">West vector at the start of the original polyline.</param>
    /// <param name="northVector">North vector at the start of the original polyline.</param>
    /// <param name="forwardVector">Forward vector at the start of the original polyline.</param>
    /// <param name="overrideStartCap">If not null, this parameter override the start cap that is stored in this class.</param>
    /// <param name="overrideEndCap">If not null, this parameter overrides the end cap that is stored in this class.</param>
    public void AddGeometry(
    Action<PointD3D, VectorD3D> AddPositionAndNormal,
    Action<int, int, int, bool> AddIndices,
    ref int vertexIndexOffset,
    IList<PointD3D> polylinePoints,
    VectorD3D westVector,
    VectorD3D northVector,
    VectorD3D forwardVector,
    ILineCap overrideStartCap,
    ILineCap overrideEndCap)
    {
      if (null == _normalsTransformedCurrent)
        throw new InvalidProgramException("The structure is not initialized yet. Call Initialize before using it!");

      _polylineIndexAtStartCapBase = 0;
      _startCapForwardAndPositionProvided = false;
      _startCapNeedsJoiningSegment = false;

      _polylineIndexAtEndCapBase = polylinePoints.Count - 1;
      _endCapForwardAndPositionProvided = false;
      _endCapNeedsJoiningSegment = false;

      if (null != _dashStartCap && null == overrideStartCap)
      {
        if (_dashStartCapBaseInsetAbsolute < 0)
        {
          _polylineIndexAtStartCapBase = PolylineMath3D.GetFractionalStartIndexOfPolylineWithCapInsetAbsolute(
            polylinePoints,
            -_dashStartCapBaseInsetAbsolute,
            out _startCapForwardAndPositionProvided,
            out _startCapNeedsJoiningSegment,
            _startCapCOS);
        }
      }

      if (null != _dashEndCap && null == overrideEndCap)
      {
        if (_dashEndCapBaseInsetAbsolute < 0)
        {
          _polylineIndexAtEndCapBase = PolylineMath3D.GetFractionalEndIndexOfPolylineWithCapInsetAbsolute(
            polylinePoints,
            -_dashEndCapBaseInsetAbsolute,
            out _endCapForwardAndPositionProvided,
            out _endCapNeedsJoiningSegment,
            _endCapCOS);
        }
      }

      bool isLineDrawn = _polylineIndexAtEndCapBase > _polylineIndexAtStartCapBase && !double.IsNaN(_polylineIndexAtStartCapBase) && !double.IsNaN(_polylineIndexAtEndCapBase);

      if (!isLineDrawn) // if no line is drawn, then we must provide the start cap and end cap at least with the west and north vectors
      {
        _startCapCOS.WestVector = westVector;
        _startCapCOS.NorthVector = northVector;
        PolylineMath3D.GetWestAndNorthVectorsForNextSegment(_startCapCOS.ForwardVector, forwardVector, ref _startCapCOS.WestVector, ref _startCapCOS.NorthVector);

        _endCapCOS.WestVector = _startCapCOS.WestVector;
        _endCapCOS.NorthVector = _startCapCOS.NorthVector;
        PolylineMath3D.GetWestAndNorthVectorsForNextSegment(_endCapCOS.ForwardVector, _startCapCOS.ForwardVector, ref _endCapCOS.WestVector, ref _endCapCOS.NorthVector);
      }

      AddGeometry(AddPositionAndNormal,
        AddIndices,
        ref vertexIndexOffset,
        isLineDrawn ? PolylineMath3D.GetPolylineWithFractionalStartAndEndIndex(polylinePoints, westVector, northVector, forwardVector, _polylineIndexAtStartCapBase, _polylineIndexAtEndCapBase, _startCapForwardAndPositionProvided, _startCapNeedsJoiningSegment, _startCapCOS, _endCapForwardAndPositionProvided, _endCapNeedsJoiningSegment, _endCapCOS) : PolylineMath3D.GetEmptyPolyline(),
        isLineDrawn,
        overrideStartCap,
        overrideEndCap);
    }

    /// <summary>
    /// Adds the triangle geometry. Here, the position of the startcap base and of the endcap base is already calculated and provided in the arguments.
    /// </summary>
    /// <param name="AddPositionAndNormal">The procedure to add a vertex position and normal.</param>
    /// <param name="AddIndices">The procedure to add vertex indices for one triangle.</param>
    /// <param name="vertexIndexOffset">The vertex index offset.</param>
    /// <param name="polylinePoints">The points of the polyline to draw. This is not the original polyline segment, but the polyline segment shortened to account for the start and end cap.</param>
    /// <param name="drawLine">If this parameter is true, the line segment between lineStart and lineEnd is drawn. If false, the line segment itself is not drawn, but the start end end caps are drawn.</param>
    /// <param name="overrideStartCap">If not null, this parameter override the start cap that is stored in this class.</param>
    /// <param name="overrideEndCap">If not null, this parameter overrides the end cap that is stored in this class.</param>
    /// <exception cref="System.InvalidProgramException">The structure is not initialized yet. Call Initialize before using it!</exception>
    public void AddGeometry(
      Action<PointD3D, VectorD3D> AddPositionAndNormal,
      Action<int, int, int, bool> AddIndices,
      ref int vertexIndexOffset,
      IEnumerable<PolylinePointD3D> polylinePoints,
      bool drawLine,
      ILineCap overrideStartCap,
      ILineCap overrideEndCap
      )
    {
      if (null == _normalsTransformedCurrent)
        throw new InvalidProgramException("The structure is not initialized yet. Call Initialize before using it!");

      var resultingStartCap = overrideStartCap ?? _dashStartCap;
      var resultingEndCap = overrideEndCap ?? _dashEndCap;

      // draw the straight line if the remaining line length is >0
      if (drawLine)
      {
        AddGeometryForLineOnly(AddPositionAndNormal, AddIndices, ref vertexIndexOffset, polylinePoints);
      }

      // now the start cap
      if (null != resultingStartCap)
      {
        resultingStartCap.AddGeometry(
          AddPositionAndNormal,
          AddIndices,
          ref vertexIndexOffset,
          true,
          _startCapCOS.Position,
          _startCapCOS.WestVector,
          _startCapCOS.NorthVector,
          _startCapCOS.ForwardVector,
          _crossSection,
          null,
          null,
          ref _startCapTemporaryStorageSpace);
      }
      else if (drawLine)
      {
        /*
				LineCaps.Flat.AddGeometry(
					AddPositionAndNormal,
					AddIndices,
					ref vertexIndexOffset,
					true,
					_startCapBase,
					_startCapForwardVector,
					null
					);
				*/

        LineCaps.Flat.Instance.AddGeometry(
        AddPositionAndNormal,
        AddIndices,
        ref vertexIndexOffset,
        true,
        _startCapCOS.Position,
        _startCapCOS.WestVector,
        _startCapCOS.NorthVector,
        _startCapCOS.ForwardVector,
        _crossSection,
        null,
        null,
        ref _startCapTemporaryStorageSpace);
      }

      if (null != resultingEndCap)
      {
        resultingEndCap.AddGeometry(
        AddPositionAndNormal,
          AddIndices,
          ref vertexIndexOffset,
          false,
          _endCapCOS.Position,
          _endCapCOS.WestVector,
          _endCapCOS.NorthVector,
          _endCapCOS.ForwardVector,
          _crossSection,
          null,
          null,
          ref _endCapTemporaryStorageSpace);
      }
      else if (drawLine)
      {
        /*
				LineCaps.Flat.AddGeometry(
					AddPositionAndNormal,
					AddIndices,
					ref vertexIndexOffset,
					false,
					_endCapBase,
					_endCapForwardVector,
					null
					);
				*/

        LineCaps.Flat.Instance.AddGeometry(
          AddPositionAndNormal,
          AddIndices,
          ref vertexIndexOffset,
          false,
          _endCapCOS.Position,
          _endCapCOS.WestVector,
          _endCapCOS.NorthVector,
          _endCapCOS.ForwardVector,
          _crossSection,
          null,
          null,
          ref _endCapTemporaryStorageSpace);
      }
    }

    /// <summary>
    /// Adds the geometry for the polyline only (i.e. it adds no geometry for the start and end caps).
    /// </summary>
    /// <param name="AddPositionAndNormal">The procedure to add position and normal of one triangle vertex.</param>
    /// <param name="AddIndices">The prodedure to add the indices for one triangle.</param>
    /// <param name="vertexIndexOffset">The vertex index offset at the start of this procedure.</param>
    /// <param name="polylinePoints">The polyline points.</param>
    private void AddGeometryForLineOnly(
      Action<PointD3D, VectorD3D> AddPositionAndNormal,
      Action<int, int, int, bool> AddIndices,
      ref int vertexIndexOffset,
      IEnumerable<PolylinePointD3D> polylinePoints
      )
    {
      var crossSectionVertexCount = _crossSection.NumberOfVertices;
      var crossSectionNormalCount = _crossSection.NumberOfNormals;

      PointD3D tp; // transformed position
      VectorD3D tn; // transformed normal

      int currIndex = vertexIndexOffset;

      var polylineEnumerator = polylinePoints.GetEnumerator();
      if (!polylineEnumerator.MoveNext())
        return; // there is nothing to draw here, because no points are in this line

      var veryFirstItem = polylineEnumerator.Current;

      if (!polylineEnumerator.MoveNext())
        return; // there is nothing to draw here, because there is only one point in this line

      var currentItem = polylineEnumerator.Current;
      VectorD3D currSeg = veryFirstItem.ForwardVector;

      // Get the matrix for the start plane
      var matrixCurr = Math3D.Get2DProjectionToPlane(veryFirstItem.WestVector, veryFirstItem.NorthVector, veryFirstItem.Position);

      // ************************** Position and normals for the very first segment ********************************

      // now the positions and normals for the start of the first segment
      for (int i = 0, j = 0; i < crossSectionVertexCount; ++i, ++j)
      {
        _positionsTransformedStartCurrent[i] = matrixCurr.Transform(_crossSection.Vertices(i));
        _normalsTransformedCurrent[j] = matrixCurr.Transform(_crossSection.Normals(j));

        if (_crossSection.IsVertexSharp(i))
        {
          ++j;
          _normalsTransformedCurrent[j] = matrixCurr.Transform(_crossSection.Normals(j));
        }
      }

      // ************************** For all polyline segments ****************************************************
      while (polylineEnumerator.MoveNext())
      {
        var nextItem = polylineEnumerator.Current; // this is already the item for the end of the next segment
        VectorD3D nextSeg = nextItem.ForwardVector;

        // ******************** calculate normals for the start of the next segment ********************************
        var matrixNext = Math3D.Get2DProjectionToPlane(nextItem.WestVector, nextItem.NorthVector, currentItem.Position);
        for (int i = 0, j = 0; i < crossSectionVertexCount; ++i, ++j)
        {
          _normalsTransformedNext[j] = matrixNext.Transform(_crossSection.Normals(j));

          if (_crossSection.IsVertexSharp(i))
          {
            ++j;
            _normalsTransformedNext[j] = matrixNext.Transform(_crossSection.Normals(j));
          }
        }

        var dot_curr_next = VectorD3D.DotProduct(currSeg, nextSeg);

        // now we have three cases
        // 1st) dot_curr_next is very close to 1, the symmetry plane can be evaluated, but we don't need bevel or miter
        // 2nd) dot_curr_next is very close to -1, the symmetry place can not be evaluated, but the reflection plane can, we can set an end cap here
        // 3rd) normal case both symmetry plane and reflection plane can be evaluated.

        if (!(-Cos01Degree < dot_curr_next)) // next segment is almost 180° to current segment
        {
          // we can not use the symmetry plane here, because it would be close to 90° to the segment directions
          // instead we use the reflection plane
          VectorD3D reflectionPlaneNormal = (currSeg - nextSeg).Normalized;

          PointD3D reflectionPoint = currentItem.Position;

          // now get the matrix for transforming the cross sections positions
          var matrixReflectionPlane = Math3D.Get2DProjectionToPlaneToPlane(currentItem.WestVector, currentItem.NorthVector, currSeg, reflectionPoint, reflectionPlaneNormal);
          // Calculate the positions of the end of the current segment
          for (int i = 0; i < crossSectionVertexCount; ++i)
          {
            _positionsTransformedEndCurrent[i] = _positionsTransformedStartNext[i] = matrixReflectionPlane.Transform(_crossSection.Vertices(i));
          }
          // we put an end cap hereabove
          LineCaps.Flat.AddGeometry(
              AddPositionAndNormal,
              AddIndices,
              ref vertexIndexOffset,
              false,
              reflectionPoint,
              reflectionPlaneNormal,
            _positionsTransformedEndCurrent);
        }
        else // not close to 180°, thus the symmetry plane can be evaluated
        {
          VectorD3D symmetryPlaneNormal = (currSeg + nextSeg).Normalized;
          // now get the matrix for transforming the cross sections positions
          var matrixSymmetryPlane = Math3D.Get2DProjectionToPlaneToPlane(currentItem.WestVector, currentItem.NorthVector, currSeg, currentItem.Position, symmetryPlaneNormal);

          // normal case: consider bevel or miter only if angle > 1 degree, and not close to 180
          // dot_curr_next<=_miterLimitDotThreshold is a criterion that avoids unneccessary computations
          if (dot_curr_next < Cos1Degree && (_lineJoin == PenLineJoin.Bevel || dot_curr_next <= _miterLimitDotThreshold))
          {
            //reflection plane is the plane at which the line seems to be reflected.
            VectorD3D reflectionPlaneNormal = (currSeg - nextSeg).Normalized;

            // For further calculations it is neccessary to rotate the 2D crossection points in such a way
            // that the reflection direction points in the x-direction in the rotated coordinate system.
            // By this it is possible which cross section points are "above" the bevel plane
            // (namely all cross section points with an x-coordinate above a threshold) and which are below.
            // By this it is even possible to determine the exact location where the cross section crosses the
            // bevel plane. This is done by interpolation between two cross section points and determining, at which
            // position the x-coordinate of the line crosses the threshold.

            // To determine the rotation matrix for the cross section points, we need the direction of the reflectionPlaneNormal
            // with respect to the west and north vectors, like so:
            var dot_w = VectorD3D.DotProduct(currentItem.WestVector, reflectionPlaneNormal);
            var dot_n = VectorD3D.DotProduct(currentItem.NorthVector, reflectionPlaneNormal);
            var det = Calc.RMath.Hypot(dot_w, dot_n);
            dot_w /= det;
            dot_n /= det;
            var crossSectionRotationMatrix = new Matrix2x2(dot_w, dot_n, dot_n, -dot_w); // Matrix that will transform our cross section points in a way so that the edge between the 3D lines are in x direction of the transformed points

            // determine maxheight as the maximum of the x-coordinate of the rotated cross section vertices
            double maxheight = 0;
            for (int i = 0; i < crossSectionVertexCount; ++i)
            {
              _crossSectionRotatedVertices[i] = crossSectionRotationMatrix.Transform((VectorD2D)_crossSection.Vertices(i));
              maxheight = Math.Max(maxheight, _crossSectionRotatedVertices[i].X);
            }

            // alphaBy2 is the half angle between the current segment and the next segment
            var alphaBy2 = 0.5 * (Math.PI - Math.Acos(dot_curr_next));
            double heightOfBevelPlane = 0;// height of the bevel plane above the segment middle lines

            switch (_lineJoin)
            {
              case PenLineJoin.Bevel:
                heightOfBevelPlane = maxheight * Math.Sin(alphaBy2); // height of the bevel plane above the segment middle lines
                break;

              case PenLineJoin.Miter:
                heightOfBevelPlane = _miterLimit * maxheight; // height of the bevel plane above the segment middle lines
                break;

              default:
                throw new NotImplementedException();
            }
            // crossSectionDistanceThreshold: if the x-coordinate of the rotated cross section vertices is above this threshold, it needs to be clipped to the bevel plane
            var crossSectionDistanceThreshold = heightOfBevelPlane * Math.Sin(alphaBy2); // height as x-coordinate of the rotated cross section

            bool previousPointIsAboveHeight = _crossSectionRotatedVertices[crossSectionVertexCount - 1].X > crossSectionDistanceThreshold;
            int firstIndexOfBevelVertex = -1;
            for (int i = 0; i < crossSectionVertexCount; ++i)
            {
              bool currentPointIsAboveHeight = _crossSectionRotatedVertices[i].X > crossSectionDistanceThreshold;
              if (currentPointIsAboveHeight && !previousPointIsAboveHeight)
              {
                firstIndexOfBevelVertex = i;
                break;
              }
              previousPointIsAboveHeight = currentPointIsAboveHeight;
            }

            // we need not to take any bevel stuff if firstIndexOfBevelVertex is < 0

            var pointAtBevelPlane = currentItem.Position + heightOfBevelPlane * reflectionPlaneNormal;
            Matrix4x3 bevelMatrix1 = Math3D.GetProjectionToPlane(currSeg, pointAtBevelPlane, reflectionPlaneNormal); // Projects a point from the current segment onto the bevel plane
            Matrix4x3 bevelMatrix2 = Math3D.GetProjectionToPlane(nextSeg, pointAtBevelPlane, reflectionPlaneNormal); // projects a point from the next segment onto the bevel plane

            // Calculate the positions of the end of the current segment
            for (int i = 0; i < crossSectionVertexCount; ++i)
            {
              tp = matrixSymmetryPlane.Transform(_crossSection.Vertices(i));
              // decide whether the transformed point is above or below the bevel plane
              if (_crossSectionRotatedVertices[i].X >= crossSectionDistanceThreshold)
              {
                // then the point is above the bevel plane; we need to project it to the bevel plane
                _positionsTransformedEndCurrent[i] = bevelMatrix1.Transform(tp);
                _positionsTransformedStartNext[i] = bevelMatrix2.Transform(tp);
              }
              else
              {
                _positionsTransformedEndCurrent[i] = _positionsTransformedStartNext[i] = tp;
              }
            }

            #region Bevel plane meshing

            // mesh the bevel plane now
            if (firstIndexOfBevelVertex >= 0)
            {
              // find first index of the polygon on the bevel plane (the first index is not on the plane, then the next indices are on the plane, and the last index is not on the plane)
              int firstPolygonIndex = (firstIndexOfBevelVertex - 1 + crossSectionVertexCount) % crossSectionVertexCount;
              do // for every subsection of the cross section which is on the bevel plane, i.e. for which is the X compoment of the  _crossSectionRotatedVertices >=  crossSectionDistanceThreshold
              {
                // find the last index of the polygon on the bevel plane (the first index is not on the plane, then the next indices are on the plane, and the last index is not on the plane)
                int lastPolygonIndex;
                for (lastPolygonIndex = firstPolygonIndex + 1; lastPolygonIndex < (firstPolygonIndex + crossSectionVertexCount); lastPolygonIndex++)
                {
                  if (!(_crossSectionRotatedVertices[lastPolygonIndex % crossSectionVertexCount].X >= crossSectionDistanceThreshold))
                    break;
                }

                // mesh the polygon. The trick here is the following: the real polygon on the bevel plane is the original cross section, but cut by a line (which represents in rotated coordinates _crossSectionRotatedVertices.X is equal to  crossSectionDistanceThreshold)
                // thus every polygon on the bevel plane would be unique (depending on the orientation and angle of the line segments)
                // thus in principle every polygon must be meshed separately, but meshing is compute intensive
                // Solution: the meshing triangle indices are the same, whether you would use the exact polygon cut by the line, or if you would use the points of the original cross section next to the cut points.
                // thus we store for every polygon formed as a part of the cross section, defined by the first vertex index, and the last vertex index,
                // the triangle indices that form the polygon mesh
                int[] indexedTriangles;
                if (!_crossSectionPartsTriangleIndices.TryGetValue(new Tuple<int, int>(firstPolygonIndex, lastPolygonIndex), out indexedTriangles))
                {
                  var polygon = PolygonClosedD2D.FromPoints(CrossSectionOfLine.GetVerticesFromToIncluding(_crossSection, firstPolygonIndex, lastPolygonIndex)); // polygon formed from a part of the cross section from firstPolygonIndex to (and including) lastPolygonIndex
                  indexedTriangles = Triangulate(polygon); // calculate the triangle indices that form the mesh of this polygon
                  _crossSectionPartsTriangleIndices.Add(new Tuple<int, int>(firstPolygonIndex, lastPolygonIndex), indexedTriangles); // cache those indices for later use
                }

                // now add and calculate the points on the bevel plane
                // we do this simultaneously for the points of the end of the current segment and the points of the start of the next segment
                for (int i = firstPolygonIndex; i <= lastPolygonIndex; ++i)
                {
                  int icurr = i % crossSectionVertexCount;

                  PointD2D crossSectionPoint;
                  if (i == firstPolygonIndex) // first index, i.e. this index is not on the bevel plane, but next index is on the bevel plane
                  {
                    int inext = (i + 1) % crossSectionVertexCount;
                    double r = (crossSectionDistanceThreshold - _crossSectionRotatedVertices[icurr].X) / (_crossSectionRotatedVertices[inext].X - _crossSectionRotatedVertices[icurr].X);
                    if (!(0 <= r && r <= 1))
                      throw new InvalidProgramException("r should always be >=0 and <=1, so what's going wrong here?");
                    crossSectionPoint = (1 - r) * _crossSection.Vertices(icurr) + r * _crossSection.Vertices(inext); // this is a point on the crossSection that exactly lies on the bevel plane
                  }
                  else if (i == lastPolygonIndex) // last index, i.e. this index is not on the bevel plane, but the previous index was still on the bevel plane
                  {
                    int iprev = (i - 1 + crossSectionVertexCount) % crossSectionVertexCount;
                    double r = (crossSectionDistanceThreshold - _crossSectionRotatedVertices[iprev].X) / (_crossSectionRotatedVertices[icurr].X - _crossSectionRotatedVertices[iprev].X);
                    if (!(0 <= r && r <= 1))
                      throw new InvalidProgramException("r should always be >=0 and <=1, so what's going wrong here?");
                    crossSectionPoint = (1 - r) * _crossSection.Vertices(iprev) + r * _crossSection.Vertices(icurr); // this is a point on the crossSection that exactly lies on the bevel plane
                  }
                  else
                  {
                    crossSectionPoint = _crossSection.Vertices(icurr); // index in the
                  }

                  tp = matrixSymmetryPlane.Transform(crossSectionPoint);
                  // then the point is above the bevel plane; we need to project it to the bevel plane
                  var positionsTransformedEndCurrent = bevelMatrix1.Transform(tp);
                  var positionsTransformedStartNext = bevelMatrix2.Transform(tp);
                  AddPositionAndNormal(positionsTransformedEndCurrent, reflectionPlaneNormal); // points cut from the end of the current line segment lying on the bevel plane
                  AddPositionAndNormal(positionsTransformedStartNext, reflectionPlaneNormal); // points cut from the start of the next line segment lying on the bevel plane
                }

                // now add the triangle indices that form the triangle mesh on the bevel plane
                // again, this is done simultaneously for the points of the end of the current segment and the start of the next segment
                for (int i = 0; i < indexedTriangles.Length; i += 3)
                {
                  AddIndices(currIndex + 2 * indexedTriangles[i], currIndex + 2 * indexedTriangles[i + 1], currIndex + 2 * indexedTriangles[i + 2], false); // indices for the points cut from the end of the current line segment lying on the bevel plane
                  AddIndices(currIndex + 1 + 2 * indexedTriangles[i], currIndex + 1 + 2 * indexedTriangles[i + 1], currIndex + 1 + 2 * indexedTriangles[i + 2], true);  // indices for the points cut from the start of the next line segment lying on the bevel plane
                }

                currIndex += 2 * (lastPolygonIndex + 1 - firstPolygonIndex); // we have added this number of points for the bevel plane

                // now find the next first polygon index that is located on the bevel plane (strictly spoken, we are searching for the point before this point)
                for (firstPolygonIndex = lastPolygonIndex; firstPolygonIndex < crossSectionVertexCount; ++firstPolygonIndex)
                {
                  if (_crossSectionRotatedVertices[(firstPolygonIndex + 1) % crossSectionVertexCount].X >= crossSectionDistanceThreshold)
                    break;
                }
              } while ((firstPolygonIndex + 1) < crossSectionVertexCount); // for each subsection of the cross section
            } // end of bevel plane meshing

            #endregion Bevel plane meshing

            // now mesh the side faces
            for (int i = 0, j = 0; i < crossSectionVertexCount; ++i, ++j)
            {
              if (_crossSection.IsVertexSharp(i))
              { ++j; }

              int inext = (i + 1) % crossSectionVertexCount;
              int jnext = (j + 1) % crossSectionNormalCount;

              if ((_crossSectionRotatedVertices[i].X > crossSectionDistanceThreshold && _crossSectionRotatedVertices[inext].X <= crossSectionDistanceThreshold) ||
                (_crossSectionRotatedVertices[i].X <= crossSectionDistanceThreshold && _crossSectionRotatedVertices[inext].X > crossSectionDistanceThreshold))
              {
                double r = (crossSectionDistanceThreshold - _crossSectionRotatedVertices[i].X) / (_crossSectionRotatedVertices[inext].X - _crossSectionRotatedVertices[i].X);
                if (!(0 <= r && r <= 1))
                  throw new InvalidProgramException("r should always be >=0 and <=1, so what's going wrong here?");

                var additionalCrossSectionVertex = (1 - r) * _crossSection.Vertices(i) + r * _crossSection.Vertices(inext);
                var additionalCrossSectionNormal = ((1 - r) * _crossSection.Normals(j) + r * _crossSection.Normals(jnext)).Normalized;

                tp = matrixSymmetryPlane.Transform(additionalCrossSectionVertex);
                tn = matrixCurr.Transform(additionalCrossSectionNormal);

                AddPositionAndNormal(tp, tn);
                ++currIndex;
                AddPositionAndNormal(_positionsTransformedEndCurrent[i], _normalsTransformedCurrent[j]);
                ++currIndex;
                AddPositionAndNormal(_positionsTransformedEndCurrent[inext], _normalsTransformedCurrent[jnext]);
                ++currIndex;
                AddIndices(currIndex - 1, currIndex - 2, currIndex - 3, true);

                tn = matrixNext.Transform(additionalCrossSectionNormal);
                AddPositionAndNormal(tp, tn);
                ++currIndex;
                AddPositionAndNormal(_positionsTransformedStartNext[i], _normalsTransformedNext[j]);
                ++currIndex;
                AddPositionAndNormal(_positionsTransformedStartNext[inext], _normalsTransformedNext[jnext]);
                ++currIndex;
                AddIndices(currIndex - 1, currIndex - 2, currIndex - 3, false);
              }
            }
          }
          else // without bevel or miter
          {
            // Calculate the positions of the end of the current segment
            for (int i = 0; i < crossSectionVertexCount; ++i)
            {
              _positionsTransformedEndCurrent[i] = _positionsTransformedStartNext[i] = matrixSymmetryPlane.Transform(_crossSection.Vertices(i));
            }
          }
        }

        // draw the segment from the previous point to the current point
        for (int i = 0, j = 0; i < crossSectionVertexCount; ++i, ++j)
        {
          if (j == 0)
          {
            AddIndices(currIndex, currIndex + 1, currIndex + 2 * crossSectionNormalCount - 2, false);
            AddIndices(currIndex + 2 * crossSectionNormalCount - 2, currIndex + 1, currIndex + 2 * crossSectionNormalCount - 1, false);
          }
          else
          {
            AddIndices(currIndex, currIndex + 1, currIndex - 2, false);
            AddIndices(currIndex - 2, currIndex + 1, currIndex - 1, false);
          }

          AddPositionAndNormal(_positionsTransformedStartCurrent[i], _normalsTransformedCurrent[j]);
          AddPositionAndNormal(_positionsTransformedEndCurrent[i], _normalsTransformedCurrent[j]);
          currIndex += 2;

          if (_crossSection.IsVertexSharp(i))
          {
            ++j;
            AddPositionAndNormal(_positionsTransformedStartCurrent[i], _normalsTransformedCurrent[j]);
            AddPositionAndNormal(_positionsTransformedEndCurrent[i], _normalsTransformedCurrent[j]);
            currIndex += 2;
          }
        }
        // previous segment is done now

        // switch lastPositionsTransformed - the positions of the end of the previous segment are then the positions of the start of the new segment
        var h = _positionsTransformedStartCurrent;
        _positionsTransformedStartCurrent = _positionsTransformedStartNext;
        _positionsTransformedStartNext = h;

        // switch normals
        var v = _normalsTransformedCurrent;
        _normalsTransformedCurrent = _normalsTransformedNext;
        _normalsTransformedNext = v;

        currentItem = nextItem;
        currSeg = nextSeg;
        matrixCurr = matrixNext;
      } // for all segments - except the last one

      // *************************** very last segment ***********************************************

      // now add the positions and normals for the end of the last segment and the triangles of the last segment

      matrixCurr = Math3D.Get2DProjectionToPlane(currentItem.WestVector, currentItem.NorthVector, currentItem.Position);
      for (int i = 0, j = 0; i < crossSectionVertexCount; ++i, ++j)
      {
        _positionsTransformedEndCurrent[i] = tp = matrixCurr.Transform(_crossSection.Vertices(i));
      }

      // draw the end line segment now
      for (int i = 0, j = 0; i < crossSectionVertexCount; ++i, ++j)
      {
        if (j == 0)
        {
          AddIndices(currIndex, currIndex + 1, currIndex + 2 * crossSectionNormalCount - 2, false);
          AddIndices(currIndex + 2 * crossSectionNormalCount - 2, currIndex + 1, currIndex + 2 * crossSectionNormalCount - 1, false);
        }
        else
        {
          AddIndices(currIndex, currIndex + 1, currIndex - 2, false);
          AddIndices(currIndex - 2, currIndex + 1, currIndex - 1, false);
        }

        AddPositionAndNormal(_positionsTransformedStartCurrent[i], _normalsTransformedCurrent[j]);
        AddPositionAndNormal(_positionsTransformedEndCurrent[i], _normalsTransformedCurrent[j]);
        currIndex += 2;

        if (_crossSection.IsVertexSharp(i))
        {
          ++j;
          AddPositionAndNormal(_positionsTransformedStartCurrent[i], _normalsTransformedCurrent[j]);
          AddPositionAndNormal(_positionsTransformedEndCurrent[i], _normalsTransformedCurrent[j]);
          currIndex += 2;
        }
      }
      // end line segment is done now

      vertexIndexOffset = currIndex;
    }

    /// <summary>
    /// Triangulates the specified polygons. The result are indexed triangles.
    /// </summary>
    /// <param name="polygon">The polygon to triangulate.</param>
    /// <returns>Instance of the <see cref="IndexedTriangles"/> class, which holds the triangle vertices as well as the indices.</returns>
    private static int[] Triangulate(PolygonClosedD2D polygon)
    {
      var triangles = GetTriangles(polygon);
      var pointList = new List<PointD2D>();
      var pointToIndex = new Dictionary<PointD2D, int>();

      for (int i = 0; i < polygon.Points.Length; ++i)
        pointToIndex.Add(polygon.Points[i], i);

      var indexList = new List<int>();

      foreach (var triangle in triangles.Triangles)
      {
        var p0 = new PointD2D(triangle.Points[0].X, triangle.Points[0].Y);
        var p1 = new PointD2D(triangle.Points[1].X, triangle.Points[1].Y);
        var p2 = new PointD2D(triangle.Points[2].X, triangle.Points[2].Y);

        int i0, i1, i2;

        if (!pointToIndex.TryGetValue(p0, out i0))
        {
          throw new InvalidOperationException("Should work except when our cross section was implified by pol2Tri");
        }
        if (!pointToIndex.TryGetValue(p1, out i1))
        {
          throw new InvalidOperationException("Should work except when our cross section was implified by pol2Tri");
        }
        if (!pointToIndex.TryGetValue(p2, out i2))
        {
          throw new InvalidOperationException("Should work except when our cross section was implified by pol2Tri");
        }

        indexList.Add(i0);
        indexList.Add(i1);
        indexList.Add(i2);
      }

      return indexList.ToArray();
    }

    public static Poly2Tri.Polygon GetTriangles(PolygonClosedD2D polygons)
    {
      var mainPolygon = new Poly2Tri.Polygon(polygons.Points.Select(pt => new Poly2Tri.PolygonPoint(pt.X, pt.Y)));
      Poly2Tri.P2T.Triangulate(mainPolygon);
      return mainPolygon;
    }
  }
}
