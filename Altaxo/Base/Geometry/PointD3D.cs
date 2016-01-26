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
	public struct PointD3D
	{
		public double X;
		public double Y;
		public double Z;

		#region Serialization

		/// <summary>
		/// 2015-11-16 initial version 0.
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PointD3D), 1)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (PointD3D)obj;
				info.AddValue("X", s.X);
				info.AddValue("Y", s.Y);
				info.AddValue("Z", s.Z);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = null != o ? (PointD3D)o : new PointD3D();
				s.X = info.GetDouble("X");
				s.Y = info.GetDouble("Y");
				s.Z = info.GetDouble("Z");
				return s;
			}
		}

		#endregion Serialization

		public PointD3D(double x, double y, double z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public static PointD3D Empty { get { return new PointD3D(); } }

		/// <summary>
		/// Gets a value indicating whether one of the members of this instance is <see cref="double.NaN"/>.
		/// </summary>
		/// <value>
		///   <c>true</c> if one of the members of this instance is <see cref="double.NaN"/>; otherwise, <c>false</c>.
		/// </value>
		public bool IsNaN
		{
			get
			{
				return double.IsNaN(X) || double.IsNaN(Y) || double.IsNaN(Z);
			}
		}

		public static PointD3D operator +(PointD3D a, VectorD3D b)
		{
			return new PointD3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
		}

		public static PointD3D operator +(VectorD3D b, PointD3D a)
		{
			return new PointD3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
		}

		public static VectorD3D operator -(PointD3D a, PointD3D b)
		{
			return new VectorD3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
		}

		public static PointD3D operator -(PointD3D a, VectorD3D b)
		{
			return new PointD3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
		}

		public static bool operator ==(PointD3D a, PointD3D b)
		{
			return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
		}

		public static bool operator !=(PointD3D a, PointD3D b)
		{
			return !(a.X == b.X && a.Y == b.Y && a.Z == b.Z);
		}

		public static explicit operator PointD3D(VectorD3D v)
		{
			return new PointD3D(v.X, v.Y, v.Z);
		}

		public override string ToString()
		{
			return X.ToString() + "; " + Y.ToString() + "; " + Z.ToString();
		}
	}
}