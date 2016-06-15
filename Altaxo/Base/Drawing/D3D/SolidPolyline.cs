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

using Altaxo.Drawing.D3D;
using Altaxo.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Drawing.D3D
{
	/// <summary>
	/// Represents the solid geometry of a polyline in 3D space.
	/// </summary>
	public struct SolidPolyline
	{
		private static readonly VectorD3D _xVector = new VectorD3D(1, 0, 0);
		private static readonly VectorD3D _yVector = new VectorD3D(0, 1, 0);
		private static readonly VectorD3D _zVector = new VectorD3D(0, 0, 1);

		private ICrossSectionOfLine _crossSection;
		private IList<PointD3D> _linePoints;

		private VectorD3D _endWestVector;
		private VectorD3D _endNorthVector;
		private VectorD3D _endAdvanceVector;

		public VectorD3D EndWestVector { get { return _endWestVector; } }
		public VectorD3D EndNorthVector { get { return _endNorthVector; } }
		public VectorD3D EndAdvanceVector { get { return _endAdvanceVector; } }

		private SolidPolylineDashSegment _dashSegment;

		public void AddWithNormals(
		Action<PointD3D, VectorD3D> AddPositionAndNormal,
		Action<int, int, int, bool> AddIndices,
		ref int vertexIndexOffset,
		PenX3D pen,
		IList<PointD3D> polylinePoints
		)
		{
			if (pen.DashPattern == null)
			{
				// draw without a dash pattern - we consider the whole line as one dash segment, but instead of dash caps, with line caps
				_dashSegment.Initialize(pen.CrossSection, pen.Thickness1, pen.Thickness2, pen.LineJoin, pen.MiterLimit, pen.LineStartCap, pen.LineEndCap);
				var westNorth = Math3D.GetWestNorthVectorAtStart(polylinePoints);
				_dashSegment.AddGeometry(AddPositionAndNormal, AddIndices, ref vertexIndexOffset, polylinePoints, westNorth.Item1, westNorth.Item2, null, null);
			}
		}

		public void Add(Action<PointD3D> AddPosition, Action<int, int, int> AddIndices, int startIndex)
		{
			if (_linePoints.Count < 2)
				throw new ArgumentOutOfRangeException("linePoints.Count<2");

			VectorD3D currSeg = (_linePoints[1] - _linePoints[0]).Normalized;

			var westNorth = Math3D.GetWestNorthVectors(currSeg);

			VectorD3D w = westNorth.Item1;
			VectorD3D n = westNorth.Item2;

			int currIndex = startIndex;
			int crossSectionCount = _crossSection.NumberOfVertices;

			// Get the matrix for the start plane
			var matrix = Math3D.Get2DProjectionToPlane(w, n, _linePoints[0]);

			var tp = matrix.Transform(new PointD3D(0, 0, 0));
			AddPosition(tp);

			currIndex += 1;

			for (int i = 0; i < crossSectionCount; ++i)
			{
				tp = matrix.Transform(_crossSection.Vertices(i));
				AddPosition(tp);

				AddIndices(
				startIndex,
				startIndex + 1 + i,
				startIndex + 1 + (1 + i) % crossSectionCount
				);
			}

			currIndex += crossSectionCount;

			for (int mSeg = 1; mSeg < _linePoints.Count - 1; ++mSeg)
			{
				VectorD3D nextSeg = (_linePoints[mSeg + 1] - _linePoints[mSeg]).Normalized;

				VectorD3D midPlaneNormal = 0.5 * (currSeg + nextSeg);

				// now get the matrix
				matrix = Math3D.Get2DProjectionToPlaneToPlane(w, n, currSeg, _linePoints[mSeg], midPlaneNormal);

				for (int i = 0; i < crossSectionCount; ++i)
				{
					tp = matrix.Transform(_crossSection.Vertices(i));
					AddPosition(tp);

					AddIndices(
					currIndex - crossSectionCount + i,
					currIndex + i,
					currIndex + (1 + i) % crossSectionCount
					);

					AddIndices(
					currIndex - crossSectionCount + i,
					currIndex + (1 + i) % crossSectionCount,
					currIndex - crossSectionCount + (1 + i) % crossSectionCount
					);
				}
				currIndex += crossSectionCount;

				// mirror the north vector on the midPlane
				currSeg = nextSeg;
				w = Math3D.GetVectorSymmetricalToVector(w, midPlaneNormal);
				w = Math3D.GetNormalizedVectorOrthogonalToVector(w, currSeg); // make the north vector orthogonal (it should be already, but this corrects small deviations)
				n = VectorD3D.CrossProduct(w, currSeg);
			}

			// now add the last segment
			matrix = Math3D.Get2DProjectionToPlane(w, n, _linePoints[_linePoints.Count - 1]);
			for (int i = 0; i < crossSectionCount; ++i)
			{
				tp = matrix.Transform(_crossSection.Vertices(i));
				AddPosition(tp);

				AddIndices(
				currIndex - crossSectionCount + i,
				currIndex + i,
				currIndex + (1 + i) % crossSectionCount
				);

				AddIndices(
				currIndex - crossSectionCount + i,
				currIndex + (1 + i) % crossSectionCount,
				currIndex - crossSectionCount + (1 + i) % crossSectionCount
				);
			}
			currIndex += crossSectionCount;

			// and now the end cap
			AddPosition(_linePoints[_linePoints.Count - 1]);

			for (int i = 0; i < crossSectionCount; ++i)
			{
				AddIndices(
				currIndex - crossSectionCount + i,
				currIndex,
				currIndex - crossSectionCount + (1 + i) % crossSectionCount);
			}

			_endWestVector = w;
			_endNorthVector = n;
			_endAdvanceVector = currSeg.Normalized;
		}

		public static void AddWithNormalsOld(
			Action<PointD3D, VectorD3D> AddPositionAndNormal,
			Action<int, int, int, bool> AddIndices,
			ref int vertexIndexOffset,
			PenX3D pen,
			IList<PointD3D> polylinePoints
			)
		{
			var dashPattern = pen.DashPattern;

			if (null == dashPattern) // Solid line
			{
				var westNorth = Math3D.GetWestNorthVectorAtStart(polylinePoints);
				AddWithNormals(AddPositionAndNormal, AddIndices, ref vertexIndexOffset, pen.CrossSection, polylinePoints, westNorth.Item1, westNorth.Item2);
			}
			else // line with dash
			{
				double unitLength = Math.Max(pen.Thickness1, pen.Thickness2);
				foreach (var polyline in Math3D.DissectPolylineWithDashPattern(polylinePoints, dashPattern, unitLength))
				{
					AddWithNormals(AddPositionAndNormal, AddIndices, ref vertexIndexOffset, pen.CrossSection, polyline.Item1, polyline.Item2, polyline.Item3);
				}
			}
		}

		public static void AddWithNormals(
			Action<PointD3D, VectorD3D> AddPositionAndNormal,
			Action<int, int, int, bool> AddIndices,
			ref int vertexIndexOffset,
			ICrossSectionOfLine _crossSection,
			IEnumerable<PointD3D> polylinePoints,
			VectorD3D startWestVector,
			VectorD3D startNorthVector
			)
		{
			var crossSectionVertexCount = _crossSection.NumberOfVertices;
			var crossSectionNormalCount = _crossSection.NumberOfNormals;

			PointD3D tp; // transformed position
			VectorD3D tn; // transformed normal

			PointD3D[] lastPositionsTransformedBegCurr = new PointD3D[crossSectionVertexCount];
			PointD3D[] lastPositionsTransformedEndCurr = new PointD3D[crossSectionVertexCount];
			PointD3D[] lastPositionsTransformedBegNext = new PointD3D[crossSectionVertexCount];
			VectorD3D[] lastNormalsTransformedCurr = new VectorD3D[crossSectionNormalCount];
			VectorD3D[] lastNormalsTransformedNext = new VectorD3D[crossSectionNormalCount];
			VectorD2D[] crossSectionRotatedVertices = new VectorD2D[crossSectionVertexCount];

			int currIndex = vertexIndexOffset;

			var polylineEnumerator = Math3D.GetPolylinePointsWithWestAndNorth(polylinePoints, startWestVector, startNorthVector).GetEnumerator();
			if (!polylineEnumerator.MoveNext())
				return; // there is nothing to draw here, because no points are in this line

			var pitem = polylineEnumerator.Current;
			var previousPolylinePoint = pitem.Item1;
			var westVector = pitem.Item2;
			var northVector = pitem.Item3;

			if (!polylineEnumerator.MoveNext())
				return; // there is nothing to draw here, because there is only one point in this line

			var currentItem = polylineEnumerator.Current;
			VectorD3D currSeg = (currentItem.Item1 - previousPolylinePoint).Normalized;

			// Get the matrix for the start plane
			var matrixCurr = Math3D.Get2DProjectionToPlane(westVector, northVector, previousPolylinePoint);

			// add the middle point of the start plane
			AddPositionAndNormal(previousPolylinePoint, -currSeg);
			currIndex += 1;

			// add the points of the cross section for the start cap
			// note: normally it is not necessary here to use the normals-count, since in the moment the cap is flat
			// but if the cap is pointy, then we need as many positions as normals
			for (int i = 0; i < crossSectionVertexCount; ++i)
			{
				lastPositionsTransformedBegCurr[i] = tp = matrixCurr.Transform(_crossSection.Vertices(i));
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
				lastNormalsTransformedCurr[j] = matrixCurr.Transform(_crossSection.Normals(j));

				if (_crossSection.IsVertexSharp(i))
				{
					++j;
					lastNormalsTransformedCurr[j] = matrixCurr.Transform(_crossSection.Normals(j));
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
					lastNormalsTransformedNext[j] = matrixNext.Transform(_crossSection.Normals(j));

					if (_crossSection.IsVertexSharp(i))
					{
						++j;
						lastNormalsTransformedNext[j] = matrixNext.Transform(_crossSection.Normals(j));
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

				if (true) // Bevel
				{
					// Calculate bevel plane
					VectorD3D reflectionPlaneNormal = (currSeg - nextSeg).Normalized;
					var dot_w = VectorD3D.DotProduct(westVector, reflectionPlaneNormal);
					var dot_n = VectorD3D.DotProduct(northVector, reflectionPlaneNormal);
					var det = Calc.RMath.Hypot(dot_w, dot_n);
					dot_w /= det;
					dot_n /= det;
					var crossSectionRotationMatrix = new Matrix2x2(dot_w, dot_n, dot_n, -dot_w); // Matrix that will transform our cross section points in a way so that the edge between the 3D lines are in x direction of the transformed points
					double maxheight = 0;
					for (int i = 0; i < crossSectionVertexCount; ++i)
					{
						crossSectionRotatedVertices[i] = crossSectionRotationMatrix.Transform((VectorD2D)_crossSection.Vertices(i));
						maxheight = Math.Max(maxheight, crossSectionRotatedVertices[i].X);
					}
					var alphaBy2 = 0.5 * (Math.PI - Math.Acos(dot_curr_next)); // half of the angle between current segment and next segment
					var heightOfBevelPlane = maxheight * Math.Sin(alphaBy2); // height of the bevel plane above the segment middle lines
					var height = heightOfBevelPlane * Math.Sin(alphaBy2); // height as x-coordinate of the rotated cross section

					bool previousPointIsAboveHeight = crossSectionRotatedVertices[crossSectionVertexCount - 1].X > height;
					int firstIndexOfBevelVertex = -1;
					for (int i = 0; i < crossSectionVertexCount; ++i)
					{
						bool currentPointIsAboveHeight = crossSectionRotatedVertices[i].X > height;
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
						if (crossSectionRotatedVertices[i].X >= height)
						{
							// then the point is above the bevel plane; we need to project it to the bevel plane
							lastPositionsTransformedEndCurr[i] = bevelMatrix1.Transform(tp);
							lastPositionsTransformedBegNext[i] = bevelMatrix2.Transform(tp);
						}
						else
						{
							lastPositionsTransformedEndCurr[i] = lastPositionsTransformedBegNext[i] = tp;
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
								if (crossSectionRotatedVertices[icurr].X > height)
								{
									AddPositionAndNormal(lastPositionsTransformedEndCurr[icurr], reflectionPlaneNormal);
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
							double r = (height - crossSectionRotatedVertices[iprev].X) / (crossSectionRotatedVertices[icurr].X - crossSectionRotatedVertices[iprev].X);
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
								AddPositionAndNormal(lastPositionsTransformedBegNext[icurr], reflectionPlaneNormal);
								++currIndex;
								AddIndices(currIndex - 1, currIndex - 2, indexOfMidPoint, true);
							}

							iprev = icurr;
							icurr = (i + crossSectionVertexCount) % crossSectionVertexCount;
							r = (height - crossSectionRotatedVertices[iprev].X) / (crossSectionRotatedVertices[icurr].X - crossSectionRotatedVertices[iprev].X);
							if (!(0 <= r && r <= 1))
								throw new InvalidProgramException("r should always be >=0 and <=1, so what's going wrong here?");

							additionalCrossSectionVertex = (1 - r) * _crossSection.Vertices(iprev) + r * _crossSection.Vertices(icurr);
							tp = matrixSymmetryPlane.Transform(additionalCrossSectionVertex);
							//tp = bevelMatrix1.Transform(tp);
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
								bool currentPointIsAboveHeight = crossSectionRotatedVertices[icurr].X > height;
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

						if ((crossSectionRotatedVertices[i].X > height && crossSectionRotatedVertices[inext].X <= height) ||
							(crossSectionRotatedVertices[i].X <= height && crossSectionRotatedVertices[inext].X > height))
						{
							double r = (height - crossSectionRotatedVertices[i].X) / (crossSectionRotatedVertices[inext].X - crossSectionRotatedVertices[i].X);
							if (!(0 <= r && r <= 1))
								throw new InvalidProgramException("r should always be >=0 and <=1, so what's going wrong here?");

							var additionalCrossSectionVertex = (1 - r) * _crossSection.Vertices(i) + r * _crossSection.Vertices(inext);
							var additionalCrossSectionNormal = ((1 - r) * _crossSection.Normals(j) + r * _crossSection.Normals(jnext)).Normalized;

							tp = matrixSymmetryPlane.Transform(additionalCrossSectionVertex);
							tn = matrixCurr.Transform(additionalCrossSectionNormal);

							AddPositionAndNormal(tp, tn);
							++currIndex;
							AddPositionAndNormal(lastPositionsTransformedEndCurr[i], lastNormalsTransformedCurr[j]);
							++currIndex;
							AddPositionAndNormal(lastPositionsTransformedEndCurr[inext], lastNormalsTransformedCurr[jnext]);
							++currIndex;
							AddIndices(currIndex - 1, currIndex - 2, currIndex - 3, true);

							tn = matrixNext.Transform(additionalCrossSectionNormal);
							AddPositionAndNormal(tp, tn);
							++currIndex;
							AddPositionAndNormal(lastPositionsTransformedBegNext[i], lastNormalsTransformedNext[j]);
							++currIndex;
							AddPositionAndNormal(lastPositionsTransformedBegNext[inext], lastNormalsTransformedNext[jnext]);
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
						lastPositionsTransformedEndCurr[i] = lastPositionsTransformedBegNext[i] = matrixSymmetryPlane.Transform(_crossSection.Vertices(i));
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

					AddPositionAndNormal(lastPositionsTransformedBegCurr[i], lastNormalsTransformedCurr[j]);
					AddPositionAndNormal(lastPositionsTransformedEndCurr[i], lastNormalsTransformedCurr[j]);
					currIndex += 2;

					if (_crossSection.IsVertexSharp(i))
					{
						++j;
						AddPositionAndNormal(lastPositionsTransformedBegCurr[i], lastNormalsTransformedCurr[j]);
						AddPositionAndNormal(lastPositionsTransformedEndCurr[i], lastNormalsTransformedCurr[j]);
						currIndex += 2;
					}
				}
				// previous segment is done now

				// switch lastPositionsTransformed - the positions of the end of the previous segment are then the positions of the start of the new segment
				var h = lastPositionsTransformedBegCurr;
				lastPositionsTransformedBegCurr = lastPositionsTransformedBegNext;
				lastPositionsTransformedBegNext = h;

				// switch normals
				var v = lastNormalsTransformedCurr;
				lastNormalsTransformedCurr = lastNormalsTransformedNext;
				lastNormalsTransformedNext = v;

				currentItem = nextItem;
				currSeg = nextSeg;
				matrixCurr = matrixNext;
			} // for all segments - except the last one

			// *************************** very last segment ***********************************************

			// now add the positions and normals for the end of the last segment and the triangles of the last segment
			matrixCurr = Math3D.Get2DProjectionToPlane(currentItem.Item2, currentItem.Item3, currentItem.Item1);
			for (int i = 0, j = 0; i < crossSectionVertexCount; ++i, ++j)
			{
				lastPositionsTransformedEndCurr[i] = tp = matrixCurr.Transform(_crossSection.Vertices(i));
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

				AddPositionAndNormal(lastPositionsTransformedBegCurr[i], lastNormalsTransformedCurr[j]);
				AddPositionAndNormal(lastPositionsTransformedEndCurr[i], lastNormalsTransformedCurr[j]);
				currIndex += 2;

				if (_crossSection.IsVertexSharp(i))
				{
					++j;
					AddPositionAndNormal(lastPositionsTransformedBegCurr[i], lastNormalsTransformedCurr[j]);
					AddPositionAndNormal(lastPositionsTransformedEndCurr[i], lastNormalsTransformedCurr[j]);
					currIndex += 2;
				}
			}
			// end line segment is done now

			// and now the end cap
			/*
			for (int i = 0, j = 0; i < crossSectionVertexCount; ++i, ++j)
			{
				AddPositionAndNormal(lastPositionsTransformed[i], currSeg);
				if (_crossSection.IsVertexSharp(i))
				{
					++j;
					AddPositionAndNormal(lastPositionsTransformed[i], currSeg);
				}

				// note that the triangle index of the midpoint refers to the midpoint that is added only after this loop and thus it not existing now
				AddIndices(
				currIndex + j,
				currIndex + crossSectionNormalCount, // mid point of the end cap
				currIndex + (1 + j) % crossSectionNormalCount,
				false);
			}

			currIndex += crossSectionNormalCount;

			// add the middle point of the end cap and the normal of the end cap
			AddPositionAndNormal(currentItem.Item1, currSeg);

			++currIndex;
			*/

			vertexIndexOffset = currIndex;
		}
	}
}