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

using Altaxo.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Drawing.D3D
{
	public abstract class SolidLineCap
	{
	}

	public class CrossSectionShapedLineCap : SolidLineCap
	{
		public void Add(ICrossSectionOfLine crossSection, LineCapContour contour, VectorD3D e, VectorD3D n, PointD3D start,
			Action<PointD3D, VectorD3D> AddPositionAndNormal,
			Action<int, int, int> AddIndices,
			ref int startIndex)
		{
			var crossSectionVertices = crossSection.Vertices;
			var crossSectionNormals = crossSection.Normals;
			var crossSectionVertexIsSharp = crossSection.IsVertexSharp;
			var crossSectionVertexCount = crossSection.NumberOfVertices;
			var crossSectionNormalCount = crossSection.NumberOfNormals;

			double maxRadius = crossSection.GetMaximalDistanceFromCenter();

			PointD3D tp; // transformed position
			VectorD3D tn; // transformed normal

			PointD3D[] lastPositionsTransformed = new PointD3D[crossSectionVertexCount];
			VectorD3D[] lastNormalsTransformed = new VectorD3D[crossSectionNormalCount];

			VectorD3D currSeg = VectorD3D.CrossProduct(n, e);
			currSeg.Normalize();

			int currIndex = startIndex;

			var vectorTransform = new Matrix4x3(e.X, e.Y, e.Z, n.X, n.Y, n.Z, currSeg.X, currSeg.Y, currSeg.Z, 0, 0, 0); // used to transform the normal vectors into 3D-Space

			for (int contourVertexIdx = 0, contourNormalIdx = 0; contourVertexIdx < contour.NumberOfVertices; ++contourVertexIdx, ++contourNormalIdx)
			{
				// add the points of the cross section for the start cap
				// note: normally it is not necessary here to use the normals-count, since in the moment the cap is flat
				// but if the cap is pointy, then we need as many positions as normals
				var matrix = Math3D.Get2DProjectionToPlane(e, n, start + contour.Vertices[contourVertexIdx].Y * maxRadius * currSeg);

				for (int i = 0, j = 0; i < crossSectionVertexCount; ++i, ++j)
				{
					PointD3D sp = crossSectionVertices[i];
					lastPositionsTransformed[i] = tp = matrix.Transform(new PointD3D(crossSectionVertices[i].X * contour.Vertices[contourVertexIdx].X, crossSectionVertices[i].Y * contour.Vertices[contourVertexIdx].X, 0));

					// Calculation of the normal: that's complicated and has to include the first normal of the contour and
					VectorD3D s = crossSection.Normals[j]; // current section-normal (only x and y component considered)
					VectorD3D c = contour.Normals[contourNormalIdx]; // current contour-normal (only x and y component considered)
					tn = GetNormalVector(maxRadius, sp, s, c);
					tn = vectorTransform.Transform(tn);

					AddPositionAndNormal(tp, tn);

					if (crossSectionVertexIsSharp[i]) // if neccessary, add the second crossSection normals
					{
						++j;
						s = crossSection.Normals[j];
						tn = GetNormalVector(maxRadius, sp, s, c);
						tn = vectorTransform.Transform(tn);

						AddPositionAndNormal(tp, tn);
					}

					// now make the triangles to the previous contour edges
					if (contourVertexIdx > 0)
					{
						AddIndices(
						currIndex - crossSectionNormalCount + j,
						currIndex + j,
						currIndex + (1 + j) % crossSectionNormalCount);

						AddIndices(
						currIndex - crossSectionNormalCount + j,
						currIndex + (1 + j) % crossSectionNormalCount,
						currIndex - crossSectionNormalCount + (1 + j) % crossSectionNormalCount);
					}
				}

				currIndex += crossSectionNormalCount;

				if (contour.IsVertexSharp[contourVertexIdx]) // now, if neccessary, add the second contour normals
				{
					++contourNormalIdx;

					for (int i = 0, j = 0; i < crossSectionVertexCount; ++i, ++j)
					{
						// Calculation of the normal: that's complicated and has to include the first normal of the contour and
						PointD3D sp = crossSectionVertices[i];
						VectorD3D s = crossSection.Normals[j];
						VectorD3D c = contour.Normals[contourNormalIdx];
						tn = GetNormalVector(maxRadius, sp, s, c);
						tn = vectorTransform.Transform(tn);

						AddPositionAndNormal(lastPositionsTransformed[i], tn);

						if (crossSectionVertexIsSharp[i]) // add if neccessary the second crossSection Normals
						{
							++j;
							s = crossSection.Normals[j];
							tn = GetNormalVector(maxRadius, sp, s, c);
							tn = vectorTransform.Transform(tn);
							AddPositionAndNormal(lastPositionsTransformed[i], tn);
						}
					}

					currIndex += crossSectionNormalCount;
				}
			}

			startIndex = currIndex;
		}

		/// <summary>
		/// Calculates the normal to a extruded contour. Here the extrusion direction is fixed to z-direction.
		/// </summary>
		/// <param name="maxRadius">Maximum radius of the cross section (farest distance of a cross section point from the origin).</param>
		/// <param name="pp">Original cross section point coordinates.</param>
		/// <param name="s">Original cross section normal.</param>
		/// <param name="c">Original contour normal.</param>
		/// <returns>The normal of the extruded contour (x-y is the cross section plane, z the extrusion direction). This vector has then to be transformed into the 3D-space of the body.</returns>
		private static VectorD3D GetNormalVector(double maxRadius, PointD3D pp, VectorD3D s, VectorD3D c)
		{
			VectorD3D tn;
			double r = pp.X * pp.X + pp.Y * pp.Y;

			if (0 == c.Y || 0 == r) // the contour doesn't change at this point, thus we can use the original cross section normal.
			{
				tn = s;
			}
			else if (0 == c.X) // the contour makes a sharp bend towards the origin. Thus the normal direction is the z-direction, and the sign of z is the sign of c.Y.
			{
				tn = new VectorD3D(0, 0, c.Y);
			}
			else // non-degenerate case
			{
				VectorD3D a = new VectorD3D(maxRadius * c.X, 0, pp.X * c.Y);
				VectorD3D b = new VectorD3D(0, maxRadius * c.X, pp.Y * c.Y);
				a.Normalize();
				b.Normalize();
				tn = new VectorD3D(s.X * a.X, s.Y * b.Y, s.X * a.Z + s.Y * b.Z);
				tn.Normalize();
			}
			return tn;
		}

		/// <summary>
		/// Calculates the normal to a extruded contour. Here the extrusion direction is fixed to z-direction.
		/// </summary>
		/// <param name="maxRadius">Maximum radius of the cross section (farest distance of a cross section point from the origin).</param>
		/// <param name="pp">Original cross section point coordinates.</param>
		/// <param name="s">Original cross section normal.</param>
		/// <param name="c">Original contour normal.</param>
		/// <returns>The normal of the extruded contour (x-y is the cross section plane, z the extrusion direction). This vector has then to be transformed into the 3D-space of the body.</returns>
		private static VectorD3D GetNormalVectorOll(double maxRadius, PointD3D pp, VectorD3D s, VectorD3D c)
		{
			VectorD3D tn;
			double r = pp.X * pp.X + pp.Y * pp.Y;

			if (0 == c.Y || 0 == r) // the contour doesn't change at this point, thus we can use the original cross section normal.
			{
				tn = s;
			}
			else if (0 == c.X) // the contour makes a sharp bend towards the origin. Thus the normal direction is the z-direction, and the sign of z is the sign of c.Y.
			{
				tn = new VectorD3D(0, 0, c.Y);
			}
			else // non-degenerate case
			{
				VectorD3D v = new VectorD3D(-c.X * c.Y * maxRadius * pp.X, -c.X * c.Y * maxRadius * pp.Y, c.X * c.X * maxRadius * maxRadius);
				v.Normalize();

				double invr = (s.X * pp.X + s.Y * pp.Y) / (c.X * c.X * maxRadius * maxRadius + c.Y * c.Y * r);
				tn = new VectorD3D(s.X - c.Y * c.Y * pp.X * invr, s.Y - c.Y * c.Y * pp.Y * invr, c.X * c.Y * maxRadius * invr);

				double dp = VectorD3D.DotProduct(tn, v);
			}
			return tn;
		}
	}
}