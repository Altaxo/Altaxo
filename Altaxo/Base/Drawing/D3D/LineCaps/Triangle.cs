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
		private class TriangleContour : ILineCapContour
		{
			public int NumberOfNormals
			{
				get
				{
					return 2;
				}
			}

			public int NumberOfVertices
			{
				get
				{
					return 2;
				}
			}

			public bool IsVertexSharp(int idx)
			{
				return true;
			}

			public VectorD2D Normals(int idx)
			{
				return VectorD2D.CreateNormalized(1, 0.5);
			}

			public PointD2D Vertices(int idx)
			{
				switch (idx)
				{
					case 0:
						return new PointD2D(0, 1);

					case 1:
						return new PointD2D(0.5, 0);

					default:
						throw new IndexOutOfRangeException();
				}
			}
		}

		#region Serialization

		/// <summary>
		/// 2016-05-02 initial version.
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

		public override void AddGeometry(Action<PointD3D, VectorD3D> AddPositionAndNormal, Action<int, int, int, bool> AddIndices, ref int vertexIndexOffset, bool isStartCap, PointD3D basePoint, VectorD3D eastVector, VectorD3D northVector, VectorD3D forwardVectorNormalized, ICrossSectionOfLine lineCrossSection, PointD3D[] baseCrossSectionPositions, VectorD3D[] baseCrossSectionNormals, ref object temporaryStorageSpace)
		{
			Add(
				AddPositionAndNormal,
				AddIndices,
				ref vertexIndexOffset,
				isStartCap,
				basePoint,
				eastVector,
				northVector,
				forwardVectorNormalized,
				lineCrossSection,
				baseCrossSectionPositions,
				baseCrossSectionNormals,
				ref temporaryStorageSpace,
				new TriangleContour());
		}
	}
}