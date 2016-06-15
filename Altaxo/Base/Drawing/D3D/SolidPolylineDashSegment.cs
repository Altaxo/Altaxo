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
	/// <summary>
	/// Contains code to generate triangle geometry for solid polyline dash segments.
	/// This structure needs to be initialized only once per line with <see cref="Initialize(PenX3D)"/>.
	/// It then can be used for each individual dash segment by calling AddGeometry/>.
	/// </summary>
	public struct SolidPolylineDashSegment
	{
		// global Variables, initialized only one per line (not once per dash segment)
		private ICrossSectionOfLine _crossSection;

		private int _crossSectionVertexCount;
		private int _crossSectionNormalCount;
		private ILineCap _dashStartCap;
		private double _dashStartCapBaseInsetAbsolute;
		private ILineCap _dashEndCap;
		private double _dashEndCapBaseInsetAbsolute;
		private PenLineJoin _lineJoin;
		private double _miterLimit;

		// operational variables, i.e. variables evaluated during one call to draw

		/// <summary>The real start of the line, taking into account the cap's inset.</summary>
		private PointD3D _startCapBase;

		private VectorD3D _startCapWest;
		private VectorD3D _startCapNorth;
		private VectorD3D _startCapForwardVector;

		/// <summary>The real end of the line, taking into account the cap's inset.</summary>
		private PointD3D _endCapBase;

		private VectorD3D _endCapWest;
		private VectorD3D _endCapNorth;
		private VectorD3D _endCapForwardVector;

		// local variables, i.e. variables that change with every dash segment

		private object _startCapTemporaryStorageSpace;

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

		/// <summary>
		/// Initialization that is needed only once per straigth line (not once per dash).
		/// </summary>
		/// <param name="pen">The pen that is used to draw the line.</param>
		/// <param name="west">The west vector.</param>
		/// <param name="north">The north vector.</param>
		/// <param name="polylinePoints">The global line to draw.</param>
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
			this._lineJoin = lineJoin;
			this._miterLimit = miterLimit;
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
		}

		private double FindStartIndexOfPolylineWithCapInsetAbsolute(IList<PointD3D> polylinePoints, double capInsetAbsolute, out PointD3D capBase, out VectorD3D capVector)
		{
			var lineStart = polylinePoints[0];

			for (int i = 1; i < polylinePoints.Count; ++i)
			{
				var curr = polylinePoints[i];
				double diff = (curr - lineStart).Length - capInsetAbsolute;

				if (diff == 0) // OK, exactly here
				{
					capBase = curr;
					capVector = (capBase - lineStart).Normalized;
					return i;
				}
				else if (diff > 0) // OK, sowewhere between previous point and here.
				{
					int baseIndex = i - 1;
					var prev = polylinePoints[baseIndex];
					var relIndex = Calc.RootFinding.QuickRootFinding.ByBrentsAlgorithm(
						(r) => (PointD3D.Interpolate(prev, curr, r) - lineStart).Length - capInsetAbsolute,
						0, 1,
						1e-3, 0);

					capBase = PointD3D.Interpolate(prev, curr, relIndex);
					capVector = (capBase - lineStart).Normalized;
					return baseIndex + relIndex;
				}
			}

			// the cap is too big, all the line is inside the cap
			// now search at least for a point which can serve as cap direction
			for (int i = polylinePoints.Count - 1; i > 0; --i)
			{
				if (polylinePoints[i] != lineStart)
				{
					capVector = (polylinePoints[i] - lineStart).Normalized;
					capBase = lineStart + capVector * capInsetAbsolute;
					return double.NaN;
				}
			}

			capVector = VectorD3D.Empty;
			capBase = lineStart;
			return double.NaN;
		}

		private double FindEndIndexOfPolylineWithCapInsetAbsolute(IList<PointD3D> polylinePoints, double capInsetAbsolute, out PointD3D capBase, out VectorD3D capVector)
		{
			var lineEnd = polylinePoints[polylinePoints.Count - 1];
			for (int i = polylinePoints.Count - 2; i >= 0; --i)
			{
				var curr = polylinePoints[i];

				double diff = (curr - lineEnd).Length - capInsetAbsolute;

				if (diff == 0) // OK, exactely here
				{
					capBase = curr;
					capVector = lineEnd - curr;
					return i;
				}
				else if (diff > 0) // OK, sowewhere between previous point and here.
				{
					int baseIndex = i + 1;
					var prev = polylinePoints[baseIndex];
					var relIndex = Calc.RootFinding.QuickRootFinding.ByBrentsAlgorithm(
						(r) => (PointD3D.Interpolate(prev, curr, r) - lineEnd).Length - capInsetAbsolute,
						0, 1,
						1e-3, 0);

					capBase = PointD3D.Interpolate(prev, curr, relIndex);
					capVector = lineEnd - capBase;
					return baseIndex - relIndex;
				}
			}

			// the cap is too big, all the line is inside the cap
			// now search at least for a point which can serve as cap direction
			for (int i = 0; i < polylinePoints.Count; ++i)
			{
				if (polylinePoints[i] != lineEnd)
				{
					capVector = (lineEnd - polylinePoints[i]).Normalized;
					capBase = lineEnd + capVector * capInsetAbsolute;
					return double.NaN;
				}
			}

			capVector = VectorD3D.Empty;
			capBase = lineEnd;
			return double.NaN;
		}

		private IEnumerable<Tuple<PointD3D, VectorD3D, VectorD3D>> GetPolylineWithFractionalStartAndEndIndex(IList<PointD3D> originalPolyline, VectorD3D westVector, VectorD3D northVector, double startIndex, double endIndex)
		{
			var en = Math3D.GetPolylinePointsWithWestAndNorth(originalPolyline, westVector, northVector).GetEnumerator();

			int startIndexInt = (int)Math.Floor(startIndex);
			double startIndexFrac = startIndex - startIndexInt;
			int endIndexInt = (int)Math.Floor(endIndex);
			double endIndexFrac = endIndex - endIndexInt;

			int i;
			for (i = -1; i < startIndexInt; ++i)
				en.MoveNext();

			var currItem = en.Current;

			if (startIndexFrac == 0)
			{
				yield return currItem;
			}
			else
			{
				en.MoveNext();
				++i;
				var nextItem = en.Current;

				var newPoint = PointD3D.Interpolate(currItem.Item1, nextItem.Item1, startIndexFrac);
				yield return new Tuple<PointD3D, VectorD3D, VectorD3D>(newPoint, nextItem.Item2, nextItem.Item3);

				if (startIndexInt == endIndexInt)
				{
					newPoint = PointD3D.Interpolate(currItem.Item1, nextItem.Item1, endIndexFrac);
					yield return new Tuple<PointD3D, VectorD3D, VectorD3D>(newPoint, nextItem.Item2, nextItem.Item3);
					yield break;
				}
				else
				{
					yield return nextItem;
				}
			}

			for (; i < endIndexInt; ++i)
			{
				en.MoveNext();
				yield return en.Current;
			}

			if (endIndexFrac != 0)
			{
				en.MoveNext();
				var nextItem = en.Current;
				var newPoint = PointD3D.Interpolate(currItem.Item1, nextItem.Item1, endIndexFrac);
				yield return new Tuple<PointD3D, VectorD3D, VectorD3D>(newPoint, nextItem.Item2, nextItem.Item3);
			}
		}

		public void AddGeometry(
		Action<PointD3D, VectorD3D> AddPositionAndNormal,
		Action<int, int, int, bool> AddIndices,
		ref int vertexIndexOffset,
		IList<PointD3D> polylinePoints,
		VectorD3D westVector,
		VectorD3D northVector,
		ILineCap overrideStartCap,
		ILineCap overrideEndCap)
		{
			if (null == _normalsTransformedCurrent)
				throw new InvalidProgramException("The structure is not initialized yet. Call Initialize before using it!");

			double startIndexOfPolyline = 0;
			double endIndexOfPolyline = polylinePoints.Count - 1;

			if (null != _dashStartCap && null == overrideStartCap)
			{
				if (_dashStartCapBaseInsetAbsolute < 0)
				{
					startIndexOfPolyline = FindStartIndexOfPolylineWithCapInsetAbsolute(polylinePoints, -_dashStartCapBaseInsetAbsolute, out _startCapBase, out _startCapForwardVector);
				}
			}

			if (null != _dashEndCap && null == overrideEndCap)
			{
				if (_dashEndCapBaseInsetAbsolute < 0)
				{
					endIndexOfPolyline = FindEndIndexOfPolylineWithCapInsetAbsolute(polylinePoints, -_dashEndCapBaseInsetAbsolute, out _endCapBase, out _endCapForwardVector);
				}
			}

			AddGeometry(AddPositionAndNormal,
				AddIndices,
				ref vertexIndexOffset,
				GetPolylineWithFractionalStartAndEndIndex(polylinePoints, westVector, northVector, startIndexOfPolyline, endIndexOfPolyline),
				!double.IsNaN(startIndexOfPolyline) && !double.IsNaN(endIndexOfPolyline),
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
			IEnumerable<Tuple<PointD3D, VectorD3D, VectorD3D>> polylinePoints,
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
				AddGeometryForLineOnly(
					AddPositionAndNormal,
			AddIndices,
			ref vertexIndexOffset,
			 _crossSection,
			 polylinePoints
			);
			}

			// now the start cap
			if (null != resultingStartCap)
			{
				resultingStartCap.AddGeometry(
					AddPositionAndNormal,
					AddIndices,
					ref vertexIndexOffset,
					true,
					_startCapBase,
					_startCapWest,
					_startCapNorth,
					_startCapForwardVector,
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
				_startCapBase,
				_startCapWest,
				_startCapNorth,
				_startCapForwardVector,
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
					_endCapBase,
					_endCapWest,
					_endCapNorth,
					_endCapForwardVector,
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
					_endCapBase,
					_endCapWest,
					_endCapNorth,
					_endCapForwardVector,
					_crossSection,
					null,
					null,
					ref _endCapTemporaryStorageSpace);
			}
		}

		private void AddGeometryForLineOnly(
			Action<PointD3D, VectorD3D> AddPositionAndNormal,
			Action<int, int, int, bool> AddIndices,
			ref int vertexIndexOffset,
			ICrossSectionOfLine _crossSection,
			IEnumerable<Tuple<PointD3D, VectorD3D, VectorD3D>> polylinePoints
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
			VectorD3D currSeg = (currentItem.Item1 - veryFirstItem.Item1).Normalized;

			_startCapBase = veryFirstItem.Item1;
			_startCapWest = veryFirstItem.Item2;
			_startCapNorth = veryFirstItem.Item3;
			_startCapForwardVector = currSeg;

			// Get the matrix for the start plane
			var matrixCurr = Math3D.Get2DProjectionToPlane(veryFirstItem.Item2, veryFirstItem.Item3, veryFirstItem.Item1);

			// add the middle point of the start plane
			AddPositionAndNormal(veryFirstItem.Item1, -currSeg);
			currIndex += 1;

			// add the points of the cross section for the start cap
			// note: normally it is not necessary here to use the normals-count, since in the moment the cap is flat
			// but if the cap is pointy, then we need as many positions as normals
			for (int i = 0; i < crossSectionVertexCount; ++i)
			{
				_positionsTransformedStartCurrent[i] = tp = matrixCurr.Transform(_crossSection.Vertices(i));
				AddPositionAndNormal(tp, -currSeg);

				AddIndices(
				vertexIndexOffset,
				vertexIndexOffset + 1 + i,
				vertexIndexOffset + 1 + (1 + i) % crossSectionVertexCount,
				true);
			}
			currIndex += crossSectionVertexCount;
			// Start cap is done.

			// ************************** Position and normals for the very first segment ********************************

			// now the positions and normals for the start of the first segment
			for (int i = 0, j = 0; i < crossSectionVertexCount; ++i, ++j)
			{
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
				VectorD3D nextSeg = (nextItem.Item1 - currentItem.Item1).Normalized;

				// ******************** calculate normals for the start of the next segment ********************************
				var matrixNext = Math3D.Get2DProjectionToPlane(nextItem.Item2, nextItem.Item3, currentItem.Item1);
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

				VectorD3D symmetryPlaneNormal = (currSeg + nextSeg).Normalized;
				// now get the matrix for transforming the cross sections positions
				var matrixSymmetryPlane = Math3D.Get2DProjectionToPlaneToPlane(currentItem.Item2, currentItem.Item3, currSeg, currentItem.Item1, symmetryPlaneNormal);

				if (false) // Bevel
				{
					// Calculate bevel plane
					VectorD3D reflectionPlaneNormal = (currSeg - nextSeg).Normalized;
					var dot_w = VectorD3D.DotProduct(currentItem.Item2, reflectionPlaneNormal);
					var dot_n = VectorD3D.DotProduct(currentItem.Item3, reflectionPlaneNormal);
					var det = Calc.RMath.Hypot(dot_w, dot_n);
					dot_w /= det;
					dot_n /= det;
					var crossSectionRotationMatrix = new Matrix2x2(dot_w, dot_n, dot_n, -dot_w); // Matrix that will transform our cross section points in a way so that the edge between the 3D lines are in x direction of the transformed points
					double maxheight = 0;
					for (int i = 0; i < crossSectionVertexCount; ++i)
					{
						_crossSectionRotatedVertices[i] = crossSectionRotationMatrix.Transform((VectorD2D)_crossSection.Vertices(i));
						maxheight = Math.Max(maxheight, _crossSectionRotatedVertices[i].X);
					}
					var alphaBy2 = 0.5 * (Math.PI - Math.Acos(dot_curr_next)); // half of the angle between current segment and next segment
					var heightOfBevelPlane = maxheight * Math.Sin(alphaBy2); // height of the bevel plane above the segment middle lines
					var height = heightOfBevelPlane * Math.Sin(alphaBy2); // height as x-coordinate of the rotated cross section

					bool previousPointIsAboveHeight = _crossSectionRotatedVertices[crossSectionVertexCount - 1].X > height;
					int firstIndexOfBevelVertex = -1;
					for (int i = 0; i < crossSectionVertexCount; ++i)
					{
						bool currentPointIsAboveHeight = _crossSectionRotatedVertices[i].X > height;
						if (currentPointIsAboveHeight && !previousPointIsAboveHeight)
						{
							firstIndexOfBevelVertex = i;
							break;
						}
						previousPointIsAboveHeight = currentPointIsAboveHeight;
					}

					// we need not to take any bevel stuff if firstIndexOfBevelVertex is < 0

					var pointAtBevelPlane = currentItem.Item1 + heightOfBevelPlane * reflectionPlaneNormal;
					Matrix4x3 bevelMatrix1 = Math3D.GetProjectionToPlane(currSeg, pointAtBevelPlane, reflectionPlaneNormal); // Projects a point from the current segment onto the bevel plane
					Matrix4x3 bevelMatrix2 = Math3D.GetProjectionToPlane(nextSeg, pointAtBevelPlane, reflectionPlaneNormal); // projects a point from the next segment onto the bevel plane

					// Calculate the positions of the end of the current segment
					for (int i = 0; i < crossSectionVertexCount; ++i)
					{
						tp = matrixSymmetryPlane.Transform(_crossSection.Vertices(i));
						// decide whether the transformed point is above or below the bevel plane
						if (_crossSectionRotatedVertices[i].X >= height)
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

					// mesh the bevel plane now
					{
						AddPositionAndNormal(pointAtBevelPlane, reflectionPlaneNormal);
						int indexOfMidPoint = currIndex;
						++currIndex;

						int currentFirstIndexOfBevelVertex = firstIndexOfBevelVertex;

						do
						{
							int startSearchForNextBevelVertex = currentFirstIndexOfBevelVertex;
							int pointsInThisMesh = 0;
							int i, icurr = currentFirstIndexOfBevelVertex;
							for (i = currentFirstIndexOfBevelVertex; i < currentFirstIndexOfBevelVertex + crossSectionVertexCount; ++i)
							{
								icurr = (i) % crossSectionVertexCount;
								if (_crossSectionRotatedVertices[icurr].X > height)
								{
									AddPositionAndNormal(_positionsTransformedEndCurrent[icurr], reflectionPlaneNormal);
									++currIndex;
									++pointsInThisMesh;
									if (pointsInThisMesh >= 2)
										AddIndices(currIndex - 1, currIndex - 2, indexOfMidPoint, true);
								}
								else
								{
									break;
								}
							}
							startSearchForNextBevelVertex = icurr;

							int iprev = (i - 1 + crossSectionVertexCount) % crossSectionVertexCount;
							double r = (height - _crossSectionRotatedVertices[iprev].X) / (_crossSectionRotatedVertices[icurr].X - _crossSectionRotatedVertices[iprev].X);
							if (!(0 <= r && r <= 1))
								throw new InvalidProgramException("r should always be >=0 and <=1, so what's going wrong here?");

							var additionalCrossSectionVertex = (1 - r) * _crossSection.Vertices(iprev) + r * _crossSection.Vertices(icurr);
							tp = matrixSymmetryPlane.Transform(additionalCrossSectionVertex);
							//tp = bevelMatrix1.Transform(tp);
							AddPositionAndNormal(tp, reflectionPlaneNormal);
							++currIndex;
							AddIndices(currIndex - 1, currIndex - 2, indexOfMidPoint, true);

							// now back
							for (i = i - 1; i >= currentFirstIndexOfBevelVertex; --i)
							{
								icurr = (i) % crossSectionVertexCount;
								AddPositionAndNormal(_positionsTransformedStartNext[icurr], reflectionPlaneNormal);
								++currIndex;
								AddIndices(currIndex - 1, currIndex - 2, indexOfMidPoint, true);
							}

							iprev = icurr;
							icurr = (i + crossSectionVertexCount) % crossSectionVertexCount;
							r = (height - _crossSectionRotatedVertices[iprev].X) / (_crossSectionRotatedVertices[icurr].X - _crossSectionRotatedVertices[iprev].X);
							if (!(0 <= r && r <= 1))
								throw new InvalidProgramException("r should always be >=0 and <=1, so what's going wrong here?");

							additionalCrossSectionVertex = (1 - r) * _crossSection.Vertices(iprev) + r * _crossSection.Vertices(icurr);
							tp = matrixSymmetryPlane.Transform(additionalCrossSectionVertex);
							AddPositionAndNormal(tp, reflectionPlaneNormal);
							++currIndex;
							AddIndices(currIndex - 1, currIndex - 2, indexOfMidPoint, true);
							// and now close the bevel
							AddIndices(indexOfMidPoint + 1, currIndex - 1, indexOfMidPoint, true);

							// search for the next start of a bevel plane

							previousPointIsAboveHeight = false;
							for (i = startSearchForNextBevelVertex; i < startSearchForNextBevelVertex + crossSectionVertexCount; ++i)
							{
								icurr = i % crossSectionVertexCount;
								bool currentPointIsAboveHeight = _crossSectionRotatedVertices[icurr].X > height;
								if (currentPointIsAboveHeight && !previousPointIsAboveHeight)
								{
									currentFirstIndexOfBevelVertex = icurr;
									break;
								}
								previousPointIsAboveHeight = currentPointIsAboveHeight;
							}
						}
						while (currentFirstIndexOfBevelVertex != firstIndexOfBevelVertex); // mesh all single bevel planes
					}

					// now mesh the side faces
					for (int i = 0, j = 0; i < crossSectionVertexCount; ++i, ++j)
					{
						if (_crossSection.IsVertexSharp(i))
						{ ++j; }

						int inext = (i + 1) % crossSectionVertexCount;
						int jnext = (j + 1) % crossSectionNormalCount;

						if ((_crossSectionRotatedVertices[i].X > height && _crossSectionRotatedVertices[inext].X <= height) ||
							(_crossSectionRotatedVertices[i].X <= height && _crossSectionRotatedVertices[inext].X > height))
						{
							double r = (height - _crossSectionRotatedVertices[i].X) / (_crossSectionRotatedVertices[inext].X - _crossSectionRotatedVertices[i].X);
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
			_endCapBase = currentItem.Item1;
			_endCapWest = currentItem.Item2;
			_endCapNorth = currentItem.Item3;
			_endCapForwardVector = currSeg;

			matrixCurr = Math3D.Get2DProjectionToPlane(currentItem.Item2, currentItem.Item3, currentItem.Item1);
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
	}
}