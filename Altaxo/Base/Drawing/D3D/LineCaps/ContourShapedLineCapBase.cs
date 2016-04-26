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

namespace Altaxo.Drawing.D3D.LineCaps
{
	public abstract class ContourShapedLineCapBase : ILineCap
	{
		public abstract double BaseInset { get; }

		public virtual double MinimumRelativeSize
		{
			get
			{
				return 1;
			}
		}

		public virtual double MinimumAbsoluteSizePt
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
				return this.GetType().FullName;
			}
		}

		public virtual ILineCap WithMinimumAbsoluteAndRelativeSize(double absoluteSizePt, double relativeSize)
		{
			return this;
		}

		public virtual double GetAbsoluteBaseInset(double thickness1, double thickness2)
		{
			return BaseInset * Math.Max(thickness1, thickness2);
		}

		public abstract void AddGeometry(Action<PointD3D, VectorD3D> AddPositionAndNormal, Action<int, int, int> AddIndices, ref int vertexIndexOffset, bool isStartCap, PointD3D basePoint, VectorD3D forwardVectorNormalized, VectorD3D eastVector, VectorD3D northVector, ICrossSectionOfLine lineCrossSection, PointD3D[] baseCrossSectionPositions, VectorD3D[] baseCrossSectionNormals, ref object temporaryStorageSpace);

		public static void Add(
			Action<PointD3D, VectorD3D> AddPositionAndNormal,
			Action<int, int, int> AddIndices,
			ref int vertexIndexOffset,
			bool isStartCap,
			PointD3D lineReferencePoint,
			VectorD3D forwardVector,
			VectorD3D eastVector,
			VectorD3D northVector,
			ICrossSectionOfLine crossSection,
			PointD3D[] crossSectionPositions, VectorD3D[] crossSectionNormals,
			LineCapContour capContour
			)
		{
			var crossSectionVertexCount = crossSection.NumberOfVertices;
			var crossSectionNormalCount = crossSection.NumberOfNormals;

			double maxRadius = crossSection.GetMaximalDistanceFromCenter();

			PointD3D tp; // transformed position
			VectorD3D tn; // transformed normal

			PointD3D[] lastPositionsTransformed = new PointD3D[crossSectionVertexCount];
			VectorD3D[] lastNormalsTransformed = new VectorD3D[crossSectionNormalCount];

			int currIndex = vertexIndexOffset;

			var vectorTransform = new Matrix4x3(eastVector.X, eastVector.Y, eastVector.Z, northVector.X, northVector.Y, northVector.Z, forwardVector.X, forwardVector.Y, forwardVector.Z, 0, 0, 0); // used to transform the normal vectors into 3D-Space

			for (int contourVertexIdx = 0, contourNormalIdx = 0; contourVertexIdx < capContour.NumberOfVertices; ++contourVertexIdx, ++contourNormalIdx)
			{
				// add the points of the cross section for the start cap
				// note: normally it is not necessary here to use the normals-count, since in the moment the cap is flat
				// but if the cap is pointy, then we need as many positions as normals
				var matrix = Math3D.Get2DProjectionToPlane(eastVector, northVector, lineReferencePoint + capContour.Vertices[contourVertexIdx].Y * maxRadius * forwardVector);

				for (int i = 0, j = 0; i < crossSectionVertexCount; ++i, ++j)
				{
					PointD2D sp = crossSection.Vertices(i);
					lastPositionsTransformed[i] = tp = matrix.Transform(new PointD3D(sp.X * capContour.Vertices[contourVertexIdx].X, sp.Y * capContour.Vertices[contourVertexIdx].X, 0));

					// Calculation of the normal: that's complicated and has to include the first normal of the contour and
					VectorD2D s = crossSection.Normals(j); // current section-normal (only x and y component considered)
					VectorD2D c = capContour.Normals[contourNormalIdx]; // current contour-normal (only x and y component considered)
					tn = GetNormalVector(maxRadius, sp, s, c);
					tn = vectorTransform.Transform(tn);

					AddPositionAndNormal(tp, tn);

					if (crossSection.IsVertexSharp(i)) // if neccessary, add the second crossSection normals
					{
						++j;
						s = crossSection.Normals(j);
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

				if (capContour.IsVertexSharp[contourVertexIdx] && contourVertexIdx != 0 && contourVertexIdx != (capContour.NumberOfVertices - 1)) // now, if neccessary, add the second contour normals
				{
					++contourNormalIdx;

					for (int i = 0, j = 0; i < crossSectionVertexCount; ++i, ++j)
					{
						// Calculation of the normal: that's complicated and has to include the first normal of the contour and
						PointD2D sp = crossSection.Vertices(i);
						VectorD2D s = crossSection.Normals(j);
						VectorD2D c = capContour.Normals[contourNormalIdx];
						tn = GetNormalVector(maxRadius, sp, s, c);
						tn = vectorTransform.Transform(tn);

						AddPositionAndNormal(lastPositionsTransformed[i], tn);

						if (crossSection.IsVertexSharp(i)) // add if neccessary the second crossSection Normals
						{
							++j;
							s = crossSection.Normals(j);
							tn = GetNormalVector(maxRadius, sp, s, c);
							tn = vectorTransform.Transform(tn);
							AddPositionAndNormal(lastPositionsTransformed[i], tn);
						}
					}

					currIndex += crossSectionNormalCount;
				}
			}

			vertexIndexOffset = currIndex;
		}

		/// <summary>
		/// Calculates the normal to a extruded contour. Here the extrusion direction is fixed to z-direction.
		/// </summary>
		/// <param name="maxRadius">Maximum radius of the cross section (farest distance of a cross section point from the origin).</param>
		/// <param name="crossSectionPoint">Original cross section point coordinates.</param>
		/// <param name="crossSectionNormal">Original cross section normal.</param>
		/// <param name="contourNormal">Original contour normal.</param>
		/// <returns>The normal of the extruded contour (x-y is the cross section plane, z the extrusion direction). This vector has then to be transformed into the 3D-space of the body.</returns>
		private static VectorD3D GetNormalVector(double maxRadius, PointD2D crossSectionPoint, VectorD2D crossSectionNormal, VectorD2D contourNormal)
		{
			VectorD3D tn;
			double r = crossSectionPoint.X * crossSectionPoint.X + crossSectionPoint.Y * crossSectionPoint.Y;

			if (0 == contourNormal.Y || 0 == r) // the contour doesn't change at this point, thus we can use the original cross section normal.
			{
				tn = new VectorD3D(crossSectionNormal.X, crossSectionNormal.Y, 0);
			}
			else if (0 == contourNormal.X) // the contour makes a sharp bend towards the origin. Thus the normal direction is the z-direction, and the sign of z is the sign of c.Y.
			{
				tn = new VectorD3D(0, 0, contourNormal.Y);
			}
			else // non-degenerate case
			{
				VectorD3D a = new VectorD3D(maxRadius * contourNormal.X, 0, crossSectionPoint.X * contourNormal.Y).Normalized;
				VectorD3D b = new VectorD3D(0, maxRadius * contourNormal.X, crossSectionPoint.Y * contourNormal.Y).Normalized;
				tn = new VectorD3D(crossSectionNormal.X * a.X, crossSectionNormal.Y * b.Y, crossSectionNormal.X * a.Z + crossSectionNormal.Y * b.Z).Normalized;
			}
			return tn;
		}

		/// <summary>
		/// Calculates the normal to a extruded contour. Here the extrusion direction is fixed to z-direction.
		/// </summary>
		/// <param name="crossSectionPoint">Original cross section point coordinates.</param>
		/// <param name="crossSectionNormal">Original cross section normal.</param>
		/// <param name="contourNormal">Original contour normal.</param>
		/// <param name="contourZScale">Factor that is multiplied with the x-coordinate of the contour point to return the z-coordinate of the resulting contour.</param>
		/// <returns>The normal of the extruded contour (x-y is the cross section plane, z the extrusion direction). This vector has then to be transformed into the 3D-space of the body.</returns>
		protected static VectorD3D GetNormalVector(PointD2D crossSectionPoint, VectorD2D crossSectionNormal, VectorD2D contourNormal, double contourZScale)
		{
			return VectorD3D.CreateNormalized
				(
				 contourNormal.Y * crossSectionNormal.X * contourZScale,
				 contourNormal.Y * crossSectionNormal.Y * contourZScale,
				 contourNormal.X * (crossSectionNormal.X * crossSectionPoint.X + crossSectionNormal.Y * crossSectionPoint.Y)
				);
		}

		protected class PositionAndNormalStorageSpace
		{
			public PointD3D[] Positions;
			public VectorD3D[] Normals;
		}
	}
}