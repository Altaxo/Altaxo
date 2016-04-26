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

		private VectorD3D _endEastVector;
		private VectorD3D _endNorthVector;
		private VectorD3D _endAdvanceVector;

		public VectorD3D EndEastVector { get { return _endEastVector; } }
		public VectorD3D EndNorthVector { get { return _endNorthVector; } }
		public VectorD3D EndAdvanceVector { get { return _endAdvanceVector; } }

		public SolidPolyline(ICrossSectionOfLine cross, IList<PointD3D> linePoints)
		{
			_crossSection = cross;

			this._linePoints = linePoints;
		}

		private VectorD3D FindStartEastVector()
		{
			const double minAngle = 1E-4;
			const double maxAngle = Math.PI - minAngle;

			VectorD3D v = _linePoints[1] - _linePoints[0];

			double angle;
			angle = VectorD3D.AngleBetweenInRadians(v, _xVector);
			if (angle > minAngle && angle < maxAngle)
				return _xVector;
			angle = VectorD3D.AngleBetweenInRadians(v, _yVector);
			if (angle > minAngle && angle < maxAngle)
				return _yVector;
			angle = VectorD3D.AngleBetweenInRadians(v, _yVector);
			if (angle > minAngle && angle < maxAngle)
				return _zVector;

			// if this was still not successfull then use y if x.y is not null, or x if

			if (VectorD3D.DotProduct(v, _yVector) != 0)
				return _yVector;
			else if (VectorD3D.DotProduct(v, _xVector) != 0)
				return _xVector;
			else
				return _zVector;
		}

		public void Add(Action<PointD3D> AddPosition, Action<int, int, int> AddIndices, int startIndex)
		{
			if (_linePoints.Count < 2)
				throw new ArgumentOutOfRangeException("linePoints.Count<2");

			VectorD3D currSeg = (_linePoints[1] - _linePoints[0]).Normalized;

			VectorD3D e = FindStartEastVector();
			VectorD3D n = VectorD3D.CrossProduct(e, currSeg);

			int currIndex = startIndex;
			int crossSectionCount = _crossSection.NumberOfVertices;

			// Get the matrix for the start plane
			var matrix = Math3D.Get2DProjectionToPlane(e, n, _linePoints[0]);

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
				matrix = Math3D.Get2DProjectionToPlaneToPlane(e, n, currSeg, _linePoints[mSeg], midPlaneNormal);

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
				e = Math3D.GetMirroredVectorAtPlane(e, midPlaneNormal);
				e = Math3D.GetOrthonormalVectorToVector(e, currSeg); // make the north vector orthogonal (it should be already, but this corrects small deviations)
				n = VectorD3D.CrossProduct(e, currSeg);
			}

			// now add the last segment
			matrix = Math3D.Get2DProjectionToPlane(e, n, _linePoints[_linePoints.Count - 1]);
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

			_endEastVector = e;
			_endNorthVector = n;
		}

		public static void AddWithNormals(
			Action<PointD3D, VectorD3D> AddPositionAndNormal, Action<int, int, int> AddIndices, ref int vertexIndexOffset,
			PenX3D pen,
			IList<PointD3D> polylinePoints
			)
		{
			var dashPattern = pen.DashPattern;

			if (null == dashPattern) // Solid line
			{
				var eastnorth = Math3D.GetEastNorthVectorAtStart(polylinePoints);
				AddWithNormals(AddPositionAndNormal, AddIndices, ref vertexIndexOffset, pen.CrossSection, polylinePoints, eastnorth.Item1, eastnorth.Item2);
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
			Action<PointD3D, VectorD3D> AddPositionAndNormal, Action<int, int, int> AddIndices, ref int vertexIndexOffset,
			ICrossSectionOfLine _crossSection,
			IList<PointD3D> polylinePoints,
			VectorD3D startEastVector,
			VectorD3D startNorthVector
			)
		{
			if (polylinePoints.Count < 2)
				throw new ArgumentOutOfRangeException("linePoints.Count<2");

			var crossSectionVertexCount = _crossSection.NumberOfVertices;
			var crossSectionNormalCount = _crossSection.NumberOfNormals;

			PointD3D tp; // transformed position
			VectorD3D tn; // transformed normal

			PointD3D[] lastPositionsTransformed = new PointD3D[crossSectionVertexCount];
			VectorD3D[] lastNormalsTransformed = new VectorD3D[crossSectionNormalCount];

			VectorD3D currSeg = (polylinePoints[1] - polylinePoints[0]).Normalized;
			VectorD3D e = startEastVector;
			VectorD3D n = startNorthVector;

			int currIndex = vertexIndexOffset;

			// Get the matrix for the start plane
			var matrix = Math3D.Get2DProjectionToPlane(e, n, polylinePoints[0]);

			// add the middle point of the start plane
			AddPositionAndNormal(polylinePoints[0], -currSeg);
			currIndex += 1;

			// add the points of the cross section for the start cap
			// note: normally it is not necessary here to use the normals-count, since in the moment the cap is flat
			// but if the cap is pointy, then we need as many positions as normals
			for (int i = 0, j = 0; i < crossSectionVertexCount; ++i, ++j)
			{
				lastPositionsTransformed[i] = tp = matrix.Transform(_crossSection.Vertices(i));
				AddPositionAndNormal(tp, -currSeg);

				if (_crossSection.IsVertexSharp(i))
				{
					++j;
					AddPositionAndNormal(tp, -currSeg);
				}

				AddIndices(
				vertexIndexOffset,
				vertexIndexOffset + 1 + j,
				vertexIndexOffset + 1 + (1 + j) % crossSectionNormalCount);
			}

			currIndex += crossSectionNormalCount;

			// now the positions and normals for the start of the first segment
			for (int i = 0, j = 0; i < crossSectionVertexCount; ++i, ++j)
			{
				lastNormalsTransformed[j] = tn = matrix.Transform(_crossSection.Normals(j));
				AddPositionAndNormal(lastPositionsTransformed[i], tn);

				if (_crossSection.IsVertexSharp(i))
				{
					++j;
					lastNormalsTransformed[j] = tn = matrix.Transform(_crossSection.Normals(j));
					AddPositionAndNormal(lastPositionsTransformed[i], tn);
				}
			}

			currIndex += crossSectionNormalCount;

			for (int mSeg = 1; mSeg < polylinePoints.Count - 1; ++mSeg)
			{
				VectorD3D nextSeg = (polylinePoints[mSeg + 1] - polylinePoints[mSeg]).Normalized;

				VectorD3D midPlaneNormal = 0.5 * (currSeg + nextSeg);

				// now get the matrix for transforming the cross sections positions
				matrix = Math3D.Get2DProjectionToPlaneToPlane(e, n, currSeg, polylinePoints[mSeg], midPlaneNormal);

				// add positions and normals of the join and make the triangles from the last line join to this line join
				for (int i = 0, j = 0; i < crossSectionVertexCount; ++i, ++j)
				{
					lastPositionsTransformed[i] = tp = matrix.Transform(_crossSection.Vertices(i));
					AddPositionAndNormal(tp, lastNormalsTransformed[j]);

					if (_crossSection.IsVertexSharp(i))
					{
						++j;
						AddPositionAndNormal(tp, lastNormalsTransformed[j]);
					}

					AddIndices(
					currIndex - crossSectionNormalCount + j,
					currIndex + j,
					currIndex + (1 + j) % crossSectionNormalCount);

					AddIndices(
					currIndex - crossSectionNormalCount + j,
					currIndex + (1 + j) % crossSectionNormalCount,
					currIndex - crossSectionNormalCount + (1 + j) % crossSectionNormalCount);
				}
				currIndex += crossSectionNormalCount;

				// mirror the north vector on the midPlane
				currSeg = nextSeg;
				e = Math3D.GetMirroredVectorAtPlane(e, midPlaneNormal);
				e = Math3D.GetOrthonormalVectorToVector(e, currSeg); // make the north vector orthogonal (it should be already, but this corrects small deviations)
				n = VectorD3D.CrossProduct(e, currSeg);

				// now add the positions and the normals for start of the next segment
				var normalTransformation = Math3D.Get2DProjectionToPlane(e, n, polylinePoints[mSeg]);
				for (int i = 0, j = 0; i < crossSectionVertexCount; ++i, ++j)
				{
					lastNormalsTransformed[j] = tn = normalTransformation.Transform(_crossSection.Normals(j));
					AddPositionAndNormal(lastPositionsTransformed[i], tn);

					if (_crossSection.IsVertexSharp(i))
					{
						++j;
						lastNormalsTransformed[j] = tn = matrix.Transform(_crossSection.Normals(j));
						AddPositionAndNormal(lastPositionsTransformed[i], tn);
					}
				}
				currIndex += crossSectionNormalCount;
			}

			// now add the positions and normals for the end of the last segment and the triangles of the last segment
			matrix = Math3D.Get2DProjectionToPlane(e, n, polylinePoints[polylinePoints.Count - 1]);
			for (int i = 0, j = 0; i < crossSectionVertexCount; ++i, ++j)
			{
				lastPositionsTransformed[i] = tp = matrix.Transform(_crossSection.Vertices(i));
				AddPositionAndNormal(tp, lastNormalsTransformed[j]);

				if (_crossSection.IsVertexSharp(i))
				{
					++j;
					lastNormalsTransformed[j] = tn = matrix.Transform(_crossSection.Normals(j));
					AddPositionAndNormal(tp, tn);
				}

				AddIndices(
				currIndex - crossSectionNormalCount + j,
				currIndex + j,
				currIndex + (1 + j) % crossSectionNormalCount);

				AddIndices(
				currIndex - crossSectionNormalCount + j,
				currIndex + (1 + j) % crossSectionNormalCount,
				currIndex - crossSectionNormalCount + (1 + j) % crossSectionNormalCount);
			}
			currIndex += crossSectionNormalCount;

			// and now the end cap

			for (int i = 0, j = 0; i < crossSectionVertexCount; ++i, ++j)
			{
				AddPositionAndNormal(lastPositionsTransformed[i], currSeg);
				if (_crossSection.IsVertexSharp(i))
				{
					++j;
					AddPositionAndNormal(lastPositionsTransformed[i], currSeg);
				}

				// note that the triange index of the midpoint refers to the midpoint that is added only after this loop and thus it not existing now
				AddIndices(
				currIndex + j,
				currIndex + crossSectionNormalCount, // mid point of the end cap
				currIndex + (1 + j) % crossSectionNormalCount);
			}

			currIndex += crossSectionNormalCount;

			// add the middle point of the end cap and the normal of the end cap
			AddPositionAndNormal(polylinePoints[polylinePoints.Count - 1], currSeg);

			++currIndex;

			//_endEastVector = e;
			//_endNorthVector = n;
			//_endAdvanceVector = currSeg;
			vertexIndexOffset = currIndex;
		}
	}
}