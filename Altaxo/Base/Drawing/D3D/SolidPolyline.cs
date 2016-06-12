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
	public class SolidPolyline
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

		public SolidPolyline(ICrossSectionOfLine cross, IList<PointD3D> linePoints)
		{
			_crossSection = cross;

			this._linePoints = linePoints;
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
				w = Math3D.GetMirroredVectorAtPlane(w, midPlaneNormal);
				w = Math3D.GetOrthonormalVectorToVector(w, currSeg); // make the north vector orthogonal (it should be already, but this corrects small deviations)
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

		public static void AddWithNormals(
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
			double maxThickness = 10;

			var crossSectionVertexCount = _crossSection.NumberOfVertices;
			var crossSectionNormalCount = _crossSection.NumberOfNormals;

			PointD3D tp; // transformed position
			VectorD3D tn; // transformed normal

			PointD3D[] lastPositionsTransformed = new PointD3D[crossSectionVertexCount];
			PointD3D[] lastPositionsTransformedEnd = new PointD3D[crossSectionVertexCount];
			PointD3D[] lastPositionsTransformedNext = new PointD3D[crossSectionVertexCount];
			bool[] isPositionOnBevel = new bool[crossSectionVertexCount];
			VectorD3D[] lastNormalsTransformed = new VectorD3D[crossSectionNormalCount];
			VectorD3D[] lastNormalsTransformedNext = new VectorD3D[crossSectionNormalCount];

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
			var matrix = Math3D.Get2DProjectionToPlane(westVector, northVector, previousPolylinePoint);

			// add the middle point of the start plane
			AddPositionAndNormal(previousPolylinePoint, -currSeg);
			currIndex += 1;

			// add the points of the cross section for the start cap
			// note: normally it is not necessary here to use the normals-count, since in the moment the cap is flat
			// but if the cap is pointy, then we need as many positions as normals
			for (int i = 0; i < crossSectionVertexCount; ++i)
			{
				lastPositionsTransformed[i] = tp = matrix.Transform(_crossSection.Vertices(i));
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
				lastNormalsTransformed[j] = matrix.Transform(_crossSection.Normals(j));

				if (_crossSection.IsVertexSharp(i))
				{
					++j;
					lastNormalsTransformed[j] = matrix.Transform(_crossSection.Normals(j));
				}
			}

			// ************************** For all polyline segments ****************************************************
			while (polylineEnumerator.MoveNext())
			{
				var nextItem = polylineEnumerator.Current; // this is already the item for the end of the next segment
				VectorD3D nextSeg = (nextItem.Item1 - currentItem.Item1).Normalized;

				// ******************** calculate normals for the start of the next segment ********************************
				matrix = Math3D.Get2DProjectionToPlane(nextItem.Item2, nextItem.Item3, currentItem.Item1);
				for (int i = 0, j = 0; i < crossSectionVertexCount; ++i, ++j)
				{
					lastNormalsTransformedNext[j] = matrix.Transform(_crossSection.Normals(j));

					if (_crossSection.IsVertexSharp(i))
					{
						++j;
						lastNormalsTransformedNext[j] = matrix.Transform(_crossSection.Normals(j));
					}
				}

				VectorD3D midPlaneNormal = (currSeg + nextSeg).Normalized;
				// now get the matrix for transforming the cross sections positions
				matrix = Math3D.Get2DProjectionToPlaneToPlane(currentItem.Item2, currentItem.Item3, currSeg, currentItem.Item1, midPlaneNormal);

				if (true) // Bevel
				{
					// Calculate bevel plane
					VectorD3D reflectionPlaneNormal = (currSeg - nextSeg).Normalized;
					var dot = VectorD3D.DotProduct(currSeg, nextSeg);
					var height = 0.5 * maxThickness * Math.Sin(Math.Acos(dot));
					var pointAtBevelPlane = currentItem.Item1 + height * reflectionPlaneNormal;
					Matrix4x3 bevelMatrix1 = Math3D.GetProjectionToPlane(currSeg, pointAtBevelPlane, reflectionPlaneNormal);
					Matrix4x3 bevelMatrix2 = Math3D.GetProjectionToPlane(nextSeg, pointAtBevelPlane, reflectionPlaneNormal);

					int firstBevelIndex = int.MinValue;
					int lastBevelIndex = int.MinValue;
					// Calculate the positions of the end of the current segment
					for (int i = 0; i < crossSectionVertexCount; ++i)
					{
						tp = matrix.Transform(_crossSection.Vertices(i));
						// decide whether the transformed point is above or below the bevel plane
						if (Math3D.GetDistancePointToPlane(tp, pointAtBevelPlane, reflectionPlaneNormal) > 0)
						{
							// then the point is above the bevel plane; we need to project it to the bevel plane
							lastPositionsTransformedEnd[i] = bevelMatrix1.Transform(tp);
							lastPositionsTransformedNext[i] = bevelMatrix2.Transform(tp);
							isPositionOnBevel[i] = true;
							if (firstBevelIndex == int.MinValue)
								firstBevelIndex = i;
							lastBevelIndex = i;
						}
						else
						{
							lastPositionsTransformedEnd[i] = lastPositionsTransformedNext[i] = tp;
							isPositionOnBevel[i] = false;
						}
					}

					// mesh the bevel plane now
					{
						AddPositionAndNormal(pointAtBevelPlane, reflectionPlaneNormal);
						int indexOfMidPoint = currIndex;
						++currIndex;
						int i;
						// i is now the first point that is located on the bevel plane
						AddPositionAndNormal(lastPositionsTransformedEnd[firstBevelIndex], reflectionPlaneNormal);
						++currIndex;
						for (i = firstBevelIndex + 1; i <= lastBevelIndex; ++i)
						{
							if (isPositionOnBevel[i])
							{
								AddPositionAndNormal(lastPositionsTransformedEnd[i], reflectionPlaneNormal);
								AddIndices(currIndex, currIndex - 1, indexOfMidPoint, true);
								++currIndex;
							}
							else
							{
							}
						}

						AddPositionAndNormal(lastPositionsTransformedNext[lastBevelIndex], reflectionPlaneNormal);  // For bevel plane: Point on the other side of the symmetry plane
						++currIndex;
						AddIndices(currIndex - 1, currIndex - 2, indexOfMidPoint, true); // bevel - midpoint - bevel (other side)

						AddPositionAndNormal(lastPositionsTransformedEnd[lastBevelIndex], lastNormalsTransformed[lastBevelIndex]);  // last Point on the bevel plane, but with the normal of the line
						++currIndex;
						AddPositionAndNormal(lastPositionsTransformedEnd[lastBevelIndex + 1], lastNormalsTransformed[lastBevelIndex + 1]); // Point that is not on the bevel plane
						++currIndex;
						AddPositionAndNormal(lastPositionsTransformedNext[lastBevelIndex], lastNormalsTransformedNext[lastBevelIndex]);  // TODO normals should be from next line Point on the other side of the symmetry plane
						++currIndex;
						AddIndices(currIndex - 1, currIndex - 2, currIndex - 3, true); // bevel - crotch -bevel (other side)

						// now back to the original index
						AddPositionAndNormal(lastPositionsTransformedNext[lastBevelIndex], reflectionPlaneNormal);  // Point on the other side of the symmetry plane. Note this point was added 3 points before, but is included here again to simplify code
						++currIndex;
						for (i = lastBevelIndex - 1; i >= firstBevelIndex; --i)
						{
							if (isPositionOnBevel[i])
							{
								AddPositionAndNormal(lastPositionsTransformedNext[i], reflectionPlaneNormal);
								++currIndex;
								AddIndices(currIndex - 1, currIndex - 2, indexOfMidPoint, true);
							}
						}

						AddPositionAndNormal(lastPositionsTransformedEnd[firstBevelIndex], reflectionPlaneNormal);  // Point on the other side of the symmetry plane
						++currIndex;
						AddIndices(currIndex - 1, currIndex - 2, indexOfMidPoint, true); // bevel - midpoint - bevel (other side)

						AddPositionAndNormal(lastPositionsTransformedNext[firstBevelIndex], lastNormalsTransformedNext[firstBevelIndex]);  // last Point on the bevel plane, but with the normal of the line
						++currIndex;
						AddPositionAndNormal(lastPositionsTransformedNext[i], lastNormalsTransformed[i]); // Point that is not on the bevel plane
						++currIndex;
						AddPositionAndNormal(lastPositionsTransformedEnd[firstBevelIndex], lastNormalsTransformed[firstBevelIndex]);  // TODO normals should be from next line Point on the other side of the symmetry plane
						++currIndex;
						AddIndices(currIndex - 1, currIndex - 2, currIndex - 3, true); // bevel - crotch -bevel (other side)
					}
				}
				else // without bevel or miter
				{
					// Calculate the positions of the end of the current segment
					for (int i = 0; i < crossSectionVertexCount; ++i)
					{
						lastPositionsTransformedEnd[i] = lastPositionsTransformedNext[i] = matrix.Transform(_crossSection.Vertices(i));
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

					AddPositionAndNormal(lastPositionsTransformed[i], lastNormalsTransformed[j]);
					AddPositionAndNormal(lastPositionsTransformedEnd[i], lastNormalsTransformed[j]);
					currIndex += 2;

					if (_crossSection.IsVertexSharp(i))
					{
						++j;
						AddPositionAndNormal(lastPositionsTransformed[i], lastNormalsTransformed[j]);
						AddPositionAndNormal(lastPositionsTransformedEnd[i], lastNormalsTransformed[j]);
						currIndex += 2;
					}
				}
				// previous segment is done now

				// switch lastPositionsTransformed - the positions of the end of the previous segment are then the positions of the start of the new segment
				var h = lastPositionsTransformed;
				lastPositionsTransformed = lastPositionsTransformedNext;
				lastPositionsTransformedEnd = h;
				// switch normals
				var v = lastNormalsTransformed;
				lastNormalsTransformed = lastNormalsTransformedNext;
				lastNormalsTransformedNext = v;

				currentItem = nextItem;
				currSeg = nextSeg;
			} // for all segments - except the last one

			// *************************** very last segment ***********************************************

			// now add the positions and normals for the end of the last segment and the triangles of the last segment
			matrix = Math3D.Get2DProjectionToPlane(currentItem.Item2, currentItem.Item3, currentItem.Item1);
			for (int i = 0, j = 0; i < crossSectionVertexCount; ++i, ++j)
			{
				lastPositionsTransformedEnd[i] = tp = matrix.Transform(_crossSection.Vertices(i));
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

				AddPositionAndNormal(lastPositionsTransformed[i], lastNormalsTransformed[j]);
				AddPositionAndNormal(lastPositionsTransformedEnd[i], lastNormalsTransformed[j]);
				currIndex += 2;

				if (_crossSection.IsVertexSharp(i))
				{
					++j;
					AddPositionAndNormal(lastPositionsTransformed[i], lastNormalsTransformed[j]);
					AddPositionAndNormal(lastPositionsTransformedEnd[i], lastNormalsTransformed[j]);
					currIndex += 2;
				}
			}
			// end line segment is done now

			// and now the end cap

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

			vertexIndexOffset = currIndex;
		}
	}
}