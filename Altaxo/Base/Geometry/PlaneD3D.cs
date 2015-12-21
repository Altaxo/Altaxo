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

namespace Altaxo.Geometry
{
	/// <summary>
	/// Represents a plane in 3d space. For the definition how it works as a clip plane see the example below.
	/// </summary>
	/// <example>
	/// Lets assume the vector (x,y,z) of the clip plane is (1,0,0), i.e. points in x-direction, and the w component is -200. Then everything
	/// that is left of the point x=+200 is clipped (i.e. every point with x&lt;+200), and thus everything right (with x&gt;+200) is visible.
	/// </example>
	public struct PlaneD3D : IEqualityComparer<PlaneD3D>
	{
		public double X { get; private set; }
		public double Y { get; private set; }
		public double Z { get; private set; }
		public double W { get; private set; }

		public PlaneD3D(double x, double y, double z, double distance)
		{
			X = x;
			Y = y;
			Z = z;
			W = distance;
		}

		public override int GetHashCode()
		{
			return 7 * X.GetHashCode() + 11 * Y.GetHashCode() + 17 * Z.GetHashCode() + 31 * W.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (!(obj is PlaneD3D))
				return false;

			var b = (PlaneD3D)obj;

			return X == b.X && Y == b.Y && Z == b.Z && W == b.W;
		}

		public bool Equals(PlaneD3D a, PlaneD3D b)
		{
			return a.X == b.X && a.Y == b.Y && a.Z == b.Z && a.W == b.W;
		}

		public int GetHashCode(PlaneD3D obj)
		{
			return obj.GetHashCode();
		}
	}
}