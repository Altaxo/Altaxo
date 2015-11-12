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
using System.Threading.Tasks;

namespace Altaxo.Geometry
{
	/// <summary>
	/// RectangleD describes a rectangle in 3D space.
	/// </summary>
	[Serializable]
	public struct RectangleD3D
	{
		private double _x, _y, _z, _sizeX, _sizeY, _sizeZ;

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(RectangleD3D), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				RectangleD3D s = (RectangleD3D)obj;
				info.AddValue("X", s._x);
				info.AddValue("Y", s._y);
				info.AddValue("Z", s._z);
				info.AddValue("SizeX", s._sizeX);
				info.AddValue("SizeY", s._sizeY);
				info.AddValue("SizeZ", s._sizeZ);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				RectangleD3D s = null != o ? (RectangleD3D)o : new RectangleD3D();
				s._x = info.GetDouble("X");
				s._y = info.GetDouble("Y");
				s._z = info.GetDouble("Z");
				s._sizeX = info.GetDouble("SizeX");
				s._sizeY = info.GetDouble("SizeY");
				s._sizeZ = info.GetDouble("SizeZ");

				return s;
			}
		}

		#endregion Serialization

		public double X
		{
			get { return _x; }
			set { _x = value; }
		}

		public double XPlusSizeX
		{
			get { return _x + _sizeX; }
		}

		public double Y
		{
			get { return _y; }
			set { _y = value; }
		}

		public double YPlusSizeY
		{
			get { return _y + _sizeY; }
		}

		public double Z
		{
			get { return _z; }
			set { _z = value; }
		}

		public double ZPlusSizeZ
		{
			get { return _z + _sizeZ; }
		}

		public double SizeX
		{
			get { return _sizeX; }
			set { _sizeX = value; }
		}

		public double SizeY
		{
			get { return _sizeY; }
			set { _sizeY = value; }
		}

		public double SizeZ
		{
			get { return _sizeZ; }
			set { _sizeZ = value; }
		}

		public RectangleD3D(double x, double y, double z, double width, double height, double sizeZ)
			: this()
		{
			_x = x;
			_y = y;
			_z = z;
			_sizeX = width;
			_sizeY = height;
			_sizeZ = sizeZ;
		}

		public RectangleD3D(PointD3D position, VectorD3D size)
		{
			_x = position.X;
			_y = position.Y;
			_z = position.Z;
			_sizeX = size.X;
			_sizeY = size.Y;
			_sizeZ = size.Z;
		}

		public static RectangleD3D Empty
		{
			get
			{
				return new RectangleD3D();
			}
		}

		public bool IsEmpty
		{
			get
			{
				return 0 == _sizeX && 0 == _sizeY && 0 == _sizeZ;
			}
		}

		public static bool operator ==(RectangleD3D p, RectangleD3D q)
		{
			return p._x == q._x && p._y == q._y && p._z == q._z && p._sizeX == q._sizeX && p._sizeY == q._sizeY && p._sizeZ == q._sizeZ;
		}

		public static bool operator !=(RectangleD3D p, RectangleD3D q)
		{
			return !(p._x == q._x && p._y == q._y && p._z == q._z && p._sizeX == q._sizeX && p._sizeY == q._sizeY && p._sizeZ == q._sizeZ);
		}

		public override int GetHashCode()
		{
			return _x.GetHashCode() + _y.GetHashCode() + _z.GetHashCode() + _sizeX.GetHashCode() + _sizeY.GetHashCode() + _sizeZ.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj is RectangleD3D)
			{
				var q = (RectangleD3D)obj;
				return _x == q._x && _y == q._y && _z == q._z && _sizeX == q._sizeX && _sizeY == q._sizeY && _sizeZ == q._sizeZ;
			}
			else
			{
				return false;
			}
		}

		public override string ToString()
		{
			return string.Format("X={0}; Y={1}; Z={2}; SX={3}; SY={4}; SZ={5}", _x, _y, _z, _sizeX, _sizeY, _sizeZ);
		}

		public PointD3D Location
		{
			get
			{
				return new PointD3D(_x, _y, _z);
			}
			set
			{
				_x = value.X;
				_y = value.Y;
				_z = value.Z;
			}
		}

		public VectorD3D Size
		{
			get
			{
				return new VectorD3D(_sizeX, _sizeY, _sizeZ);
			}
			set
			{
				_sizeX = value.X;
				_sizeY = value.Y;
				_sizeZ = value.Z;
			}
		}

		public bool Contains(PointD3D p)
		{
			return p.X >= X && p.Y >= Y && p.Z >= Z && p.X <= (_x + _sizeX) && p.Y <= (_y + _sizeY) && p.Z <= (_z + _sizeZ);
		}

		/// <summary>Inflates the rectangle by the specified values <paramref name="x"/> and <paramref name="y"/>.</summary>
		/// <param name="x">The value used to inflate the rectangle in x-direction.</param>
		/// <param name="y">The value used to inflate the rectangle in y-direction.</param>
		public void Inflate(double x, double y, double z)
		{
			this._x -= x;
			this._sizeX += x + x;
			this._y -= y;
			this._sizeY += y + y;
			this._z -= z;
			this._sizeZ += z + z;
		}

		/// <summary>Changes the location of the rectangle by the specified values for <paramref name="x"/> and <paramref name="y"/>.</summary>
		/// <param name="x">The x offset.</param>
		/// <param name="y">The y offset.</param>
		public void Offset(double x, double y, double z)
		{
			_x += x;
			_y += y;
			_z += z;
		}

		/// <summary>
		/// Expands this rectangle, so that it contains the point p.
		/// </summary>
		/// <param name="p">The point that should be contained in this rectangle.</param>
		/// <returns>The new rectangle that now contains the point p.</returns>
		public void ExpandToInclude(PointD3D p)
		{
			if (!(Contains(p)))
			{
				if (p.X < _x)
				{
					_sizeX += _x - p.X;
					_x = p.X;
				}
				else if (p.X > (_x + _sizeX))
				{
					_sizeX = p.X - _x;
				}

				if (p.Y < _y)
				{
					_sizeY += _y - p.Y;
					_y = p.Y;
				}
				else if (p.Y > (_y + _sizeY))
				{
					_sizeY = p.Y - _y;
				}

				if (p.Z < _z)
				{
					_sizeZ += _z - p.Z;
					_z = p.Z;
				}
				else if (p.Z > (_z + _sizeZ))
				{
					_sizeZ = p.Z - _z;
				}
			}
		}

		/// <summary>
		/// Creates a rectangle that includes all the provided points.
		/// </summary>
		/// <param name="points">The points that the rectangle should include.</param>
		/// <returns>The rectangle that includes all the provided points.</returns>
		/// <exception cref="System.ArgumentException">Enumeration is empty!</exception>
		public static RectangleD3D NewRectangleIncludingAllPoints(IEnumerable<PointD3D> points)
		{
			var en = points.GetEnumerator();
			if (!en.MoveNext())
				throw new ArgumentException("Enumeration is empty!", nameof(points));

			var result = new RectangleD3D(en.Current, VectorD3D.Empty);

			while (en.MoveNext())
			{
				result.ExpandToInclude(en.Current);
			}

			return result;
		}

		/// <summary>
		/// Gets the vertices in binary order (x: 0th digit, y: 1st digit, z: 2nd digit).
		/// </summary>
		/// <value>
		/// The vertices.
		/// </value>
		public IEnumerable<PointD3D> Vertices
		{
			get
			{
				yield return new PointD3D(_x, _y, _z);
				yield return new PointD3D(_x + _sizeX, _y, _z);
				yield return new PointD3D(_x, _y + _sizeY, _z);
				yield return new PointD3D(_x + _sizeX, _y + _sizeY, _z);
				yield return new PointD3D(_x, _y, _z + _sizeZ);
				yield return new PointD3D(_x + _sizeX, _y, _z + _sizeZ);
				yield return new PointD3D(_x, _y + _sizeY, _z + _sizeZ);
				yield return new PointD3D(_x + _sizeX, _y + _sizeY, _z + _sizeZ);
			}
		}

		/// <summary>
		/// Gets the triangle indices of all faces using the vertices returned by <see cref="Vertices"/>.
		/// The order is front, back, top, bottom, left, right.
		/// </summary>
		/// <value>
		/// The triangle indices.
		/// </value>
		public IEnumerable<Tuple<int, int, int>> TriangleIndices
		{
			get
			{
				// Front
				yield return new Tuple<int, int, int>(0, 2, 3);
				yield return new Tuple<int, int, int>(0, 3, 1);
				// Back
				yield return new Tuple<int, int, int>(4, 7, 6);
				yield return new Tuple<int, int, int>(4, 5, 7);
				// Top
				yield return new Tuple<int, int, int>(2, 6, 7);
				yield return new Tuple<int, int, int>(2, 7, 3);
				// Bottom
				yield return new Tuple<int, int, int>(0, 5, 4);
				yield return new Tuple<int, int, int>(0, 1, 5);
				// Left
				yield return new Tuple<int, int, int>(0, 4, 6);
				yield return new Tuple<int, int, int>(0, 6, 2);
				// Right
				yield return new Tuple<int, int, int>(1, 7, 5);
				yield return new Tuple<int, int, int>(1, 3, 7);
			}
		}

		public void ExpandToInclude(RectangleD3D rect)
		{
			foreach (PointD3D p in rect.Vertices)
				ExpandToInclude(p);
		}

		/// <summary>Gets a rectangle that includes the smallest circle around this rectangle.</summary>
		/// <value>A rectangle that includes the smallest circle around this rectangle.</value>
		public RectangleD3D OuterCircleBoundingBox
		{
			get
			{
				double d = Calc.BasicFunctions.hypot(_sizeX, _sizeY, _sizeZ);
				return new RectangleD3D(_x + 0.5 * (_sizeX - d), _y + 0.5 * (_sizeY - d), _z + 0.5 * (_sizeZ - d), d, d, d);
			}
		}
	}
}