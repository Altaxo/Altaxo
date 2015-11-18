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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Drawing.D3D
{
	using Geometry;

	public class CrossSectionRectangular : CrossSectionOfLine
	{
		#region Serialization

		/// <summary>
		/// 2015-11-18 initial version.
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(CrossSectionRectangular), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (CrossSectionRectangular)obj;

				info.AddValue("Size1", s._size1);
				info.AddValue("Size2", s._size2);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				double size1 = info.GetDouble("Size1");
				double size2 = info.GetDouble("Size2");
				return new CrossSectionRectangular(size1, size2);
			}
		}

		#endregion Serialization

		public CrossSectionRectangular(double width, double height)
		{
			_size1 = width;
			_size2 = height;
			double w2 = width / 2;
			double h2 = height / 2;
			_vertices = new PointD3D[4];
			_vertices[0] = new PointD3D(w2, -h2, 0);
			_vertices[1] = new PointD3D(w2, h2, 0);
			_vertices[2] = new PointD3D(-w2, h2, 0);
			_vertices[3] = new PointD3D(-w2, -h2, 0);

			_isVertexSharp = new bool[4];
			_isVertexSharp[0] = true;
			_isVertexSharp[1] = true;
			_isVertexSharp[2] = true;
			_isVertexSharp[3] = true;

			_normals = new VectorD3D[8];
			_normals[0] = new VectorD3D(0, -1, 0);
			_normals[1] = new VectorD3D(1, 0, 0);
			_normals[2] = new VectorD3D(1, 0, 0);
			_normals[3] = new VectorD3D(0, 1, 0);
			_normals[4] = new VectorD3D(0, 1, 0);
			_normals[5] = new VectorD3D(-1, 0, 0);
			_normals[6] = new VectorD3D(-1, 0, 0);
			_normals[7] = new VectorD3D(0, -1, 0);
		}

		public override ICrossSectionOfLine WithSize(double size1, double size2)
		{
			if (_size1 == size1 && _size2 == size2)
				return this;
			else
				return new CrossSectionRectangular(size1, size2);
		}

		public override ICrossSectionOfLine WithSize1(double size1)
		{
			if (_size1 == size1)
				return this;
			else
				return new CrossSectionRectangular(size1, _size2);
		}

		public override ICrossSectionOfLine WithSize2(double size2)
		{
			if (_size2 == size2)
				return this;
			else
				return new CrossSectionRectangular(_size1, size2);
		}
	}
}