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

			VectorD3D v = _linePoints[1] - _linePoints[0];

			if (VectorD3D.AngleBetweenInRadians(v, _xVector) > minAngle)
				return _xVector;
			if (VectorD3D.AngleBetweenInRadians(v, _yVector) > minAngle)
				return _yVector;
			else
				return _zVector;
		}

		public void Add(Action<PointD3D> AddPosition, Action<int, int, int> AddIndices, int startIndex)
		{
			if (_linePoints.Count < 2)
				throw new ArgumentOutOfRangeException("linePoints.Count<2");

			VectorD3D currSeg = _linePoints[1] - _linePoints[0];
			currSeg.Normalize();

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
				tp = matrix.Transform(_crossSection.Vertices[i]);
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
				VectorD3D nextSeg = _linePoints[mSeg + 1] - _linePoints[mSeg];
				nextSeg.Normalize();

				VectorD3D midPlaneNormal = 0.5 * (currSeg + nextSeg);

				// now get the matrix
				matrix = Math3D.Get2DProjectionToPlaneToPlane(e, n, currSeg, _linePoints[mSeg], midPlaneNormal);

				for (int i = 0; i < crossSectionCount; ++i)
				{
					tp = matrix.Transform(_crossSection.Vertices[i]);
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
				tp = matrix.Transform(_crossSection.Vertices[i]);
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

		public void AddWithNormals(Action<PointD3D, VectorD3D> AddPositionAndNormal, Action<int, int, int> AddIndices, ref int startIndex)
		{
			if (_linePoints.Count < 2)
				throw new ArgumentOutOfRangeException("linePoints.Count<2");

			var crossSectionVertices = _crossSection.Vertices;
			var crossSectionNormals = _crossSection.Normals;
			var crossSectionVertexIsSharp = _crossSection.IsVertexSharp;
			var crossSectionVertexCount = _crossSection.NumberOfVertices;
			var crossSectionNormalCount = _crossSection.NumberOfNormals;

			PointD3D tp; // transformed position
			VectorD3D tn; // transformed normal

			PointD3D[] lastPositionsTransformed = new PointD3D[crossSectionVertexCount];
			VectorD3D[] lastNormalsTransformed = new VectorD3D[crossSectionNormalCount];

			VectorD3D currSeg = _linePoints[1] - _linePoints[0];
			currSeg.Normalize();

			VectorD3D e = FindStartEastVector();
			VectorD3D n = VectorD3D.CrossProduct(e, currSeg);

			int currIndex = startIndex;

			// Get the matrix for the start plane
			var matrix = Math3D.Get2DProjectionToPlane(e, n, _linePoints[0]);

			// add the middle point of the start plane
			AddPositionAndNormal(_linePoints[0], -currSeg);
			currIndex += 1;

			// add the points of the cross section for the start cap
			// note: normally it is not necessary here to use the normals-count, since in the moment the cap is flat
			// but if the cap is pointy, then we need as many positions as normals
			for (int i = 0, j = 0; i < crossSectionVertexCount; ++i, ++j)
			{
				lastPositionsTransformed[i] = tp = matrix.Transform(crossSectionVertices[i]);
				AddPositionAndNormal(tp, -currSeg);

				if (crossSectionVertexIsSharp[i])
				{
					++j;
					AddPositionAndNormal(tp, -currSeg);
				}

				AddIndices(
				startIndex,
				startIndex + 1 + j,
				startIndex + 1 + (1 + j) % crossSectionNormalCount);
			}

			currIndex += crossSectionNormalCount;

			// now the positions and normals for the start of the first segment
			for (int i = 0, j = 0; i < crossSectionVertexCount; ++i, ++j)
			{
				lastNormalsTransformed[j] = tn = matrix.Transform(crossSectionNormals[j]);
				AddPositionAndNormal(lastPositionsTransformed[i], tn);

				if (crossSectionVertexIsSharp[i])
				{
					++j;
					lastNormalsTransformed[j] = tn = matrix.Transform(crossSectionNormals[j]);
					AddPositionAndNormal(lastPositionsTransformed[i], tn);
				}
			}

			currIndex += crossSectionNormalCount;

			for (int mSeg = 1; mSeg < _linePoints.Count - 1; ++mSeg)
			{
				VectorD3D nextSeg = _linePoints[mSeg + 1] - _linePoints[mSeg];
				nextSeg.Normalize();

				VectorD3D midPlaneNormal = 0.5 * (currSeg + nextSeg);

				// now get the matrix for transforming the cross sections positions
				matrix = Math3D.Get2DProjectionToPlaneToPlane(e, n, currSeg, _linePoints[mSeg], midPlaneNormal);

				// add positions and normals of the join and make the triangles from the last line join to this line join
				for (int i = 0, j = 0; i < crossSectionVertexCount; ++i, ++j)
				{
					lastPositionsTransformed[i] = tp = matrix.Transform(crossSectionVertices[i]);
					AddPositionAndNormal(tp, lastNormalsTransformed[j]);

					if (crossSectionVertexIsSharp[i])
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
				var normalTransformation = Math3D.Get2DProjectionToPlane(e, n, _linePoints[mSeg]);
				for (int i = 0, j = 0; i < crossSectionVertexCount; ++i, ++j)
				{
					lastNormalsTransformed[j] = tn = normalTransformation.Transform(crossSectionNormals[j]);
					AddPositionAndNormal(lastPositionsTransformed[i], tn);

					if (crossSectionVertexIsSharp[i])
					{
						++j;
						lastNormalsTransformed[j] = tn = matrix.Transform(crossSectionNormals[j]);
						AddPositionAndNormal(lastPositionsTransformed[i], tn);
					}
				}
				currIndex += crossSectionNormalCount;
			}

			// now add the positions and normals for the end of the last segment and the triangles of the last segment
			matrix = Math3D.Get2DProjectionToPlane(e, n, _linePoints[_linePoints.Count - 1]);
			for (int i = 0, j = 0; i < crossSectionVertexCount; ++i, ++j)
			{
				lastPositionsTransformed[i] = tp = matrix.Transform(crossSectionVertices[i]);
				AddPositionAndNormal(tp, lastNormalsTransformed[j]);

				if (crossSectionVertexIsSharp[i])
				{
					++j;
					lastNormalsTransformed[j] = tn = matrix.Transform(crossSectionNormals[j]);
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
				if (crossSectionVertexIsSharp[i])
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

			// add the middle point of the end cap and the normal of the end cap
			AddPositionAndNormal(_linePoints[_linePoints.Count - 1], currSeg);

			++currIndex;

			_endEastVector = e;
			_endNorthVector = n;
			_endAdvanceVector = currSeg;
			startIndex = currIndex;
		}
	}
}