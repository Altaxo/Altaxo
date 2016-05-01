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
	public class Triangle : ContourShapedLineCapBase
	{
		private static LineCapContour _triangleContour = LineCapContour.Triangle;

		#region Serialization

		/// <summary>
		/// 2016-04-22 initial version.
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Triangle), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				return new Triangle();
			}
		}

		#endregion Serialization

		public override double BaseInset
		{
			get
			{
				return -0.5;
			}
		}

		/// <inheritdoc />
		public override void AddGeometry(
			Action<PointD3D, VectorD3D> AddPositionAndNormal,
			Action<int, int, int, bool> AddIndices,
			ref int vertexIndexOffset,
			bool isStartCap,
			PointD3D basePoint,
			VectorD3D westVector,
			VectorD3D northVector,
			VectorD3D forwardVectorNormalized,
			ICrossSectionOfLine lineCrossSection,
			PointD3D[] lineCrossSectionPositions,
			VectorD3D[] lineCrossSectionNormals,
			ref object temporaryStorageSpace)
		{
			var crossSectionVertexCount = lineCrossSection.NumberOfVertices;
			var crossSectionNormalCount = lineCrossSection.NumberOfNormals;

			var contourNormal = VectorD2D.CreateNormalized(1, 0.5);
			var contourZScale = Math.Max(lineCrossSection.Size1, lineCrossSection.Size2);

			if (isStartCap)
				forwardVectorNormalized = -forwardVectorNormalized;

			var matrix = Matrix4x3.NewFromBasisVectorsAndLocation(westVector, northVector, forwardVectorNormalized, basePoint);

			PointD3D[] capCrossSectionPositions = null;
			VectorD3D[] capCrossSectionNormals = null;
			var tempSpace = temporaryStorageSpace as PositionAndNormalStorageSpace;
			if (null != tempSpace)
			{
				capCrossSectionPositions = tempSpace?.Positions;
				capCrossSectionNormals = tempSpace?.Normals;
			}
			else
			{
				temporaryStorageSpace = tempSpace = new PositionAndNormalStorageSpace();
				tempSpace.Positions = capCrossSectionPositions = new PointD3D[crossSectionVertexCount];
				tempSpace.Normals = capCrossSectionNormals = new VectorD3D[crossSectionNormalCount];
			}

			for (int i = 0, j = 0; i < crossSectionVertexCount; ++i, ++j)
			{
				var sp = lineCrossSection.Vertices(i);
				var sn = lineCrossSection.Normals(j);
				var normal = GetNormalVector(sp, sn, contourNormal, contourZScale);

				capCrossSectionPositions[i] = matrix.Transform(sp);
				capCrossSectionNormals[j] = matrix.Transform(normal);

				if (lineCrossSection.IsVertexSharp(i))
				{
					++j;
					sn = lineCrossSection.Normals(j);
					normal = GetNormalVector(sp, sn, contourNormal, contourZScale);
					capCrossSectionNormals[j] = matrix.Transform(normal);
				}
			}

			// do we need a flat end on the other side of the cap ?
			if (null == lineCrossSectionPositions) // if lineCrossSectionPositions are null, it means that our cap is not connected to the line and needs a flat end
			{
				// the parameter isStartCap must be negated, because this flat cap is the "counterpart" of our cap to draw
				Flat.AddGeometry(AddPositionAndNormal, AddIndices, ref vertexIndexOffset, !isStartCap, basePoint, forwardVectorNormalized, capCrossSectionPositions);
			}

			var tipPoint = basePoint + forwardVectorNormalized * 0.5 * contourZScale;

			// Begin point and triangle definitions
			VectorD3D normal1, normal2, normalAveraged;
			int currIndex = vertexIndexOffset;
			int normalPlusVertexCount = crossSectionNormalCount + crossSectionVertexCount;

			for (int i = 0, j = 0; i < crossSectionVertexCount; ++i, ++j)
			{
				if (i == 0)
				{
					// now add a triangle of the last three additions
					if (isStartCap)
					{
						AddIndices(
							currIndex, // mid point of the end cap
							currIndex + 1,
							currIndex - 1 + normalPlusVertexCount,
							isStartCap);
					}
					else
					{
						AddIndices(
						currIndex, // mid point of the end cap
						currIndex - 1 + normalPlusVertexCount,
						currIndex + 1,
						isStartCap);
					}

					normal1 = capCrossSectionNormals[crossSectionNormalCount - 1];
				}
				else
				{
					if (isStartCap)
					{
						AddIndices(
							currIndex, // mid point of the end cap
							currIndex + 1,
							currIndex - 1,
							isStartCap);
					}
					else
					{
						AddIndices(
						currIndex, // mid point of the end cap
						currIndex - 1,
						currIndex + 1,
						isStartCap);
					}

					normal1 = capCrossSectionNormals[j - 1];
				}

				normal2 = capCrossSectionNormals[j];
				normalAveraged = (normal1 + normal2).Normalized;
				AddPositionAndNormal(tipPoint, normalAveraged); // store the tip point with the averaged normal
				AddPositionAndNormal(capCrossSectionPositions[i], capCrossSectionNormals[j]);
				currIndex += 2; ;

				if (lineCrossSection.IsVertexSharp(i))
				{
					++j;
					AddPositionAndNormal(capCrossSectionPositions[i], capCrossSectionNormals[j]);
					++currIndex;
				}
			}

			vertexIndexOffset = currIndex;
		}

		public override bool Equals(object obj)
		{
			return obj is Triangle;
		}

		public override int GetHashCode()
		{
			return typeof(Triangle).GetHashCode();
		}
	}
}