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
using System.Threading.Tasks;

namespace Altaxo.Drawing.D3D
{
	/// <summary>
	/// Represents the solid geometry of a cube.
	/// </summary>
	public class SolidCube
	{
		public static void Add(double _x, double _y, double _z, double _dx, double _dy, double _dz,
			Action<PointD3D, VectorD3D> AddPositionAndNormal, Action<int, int, int> AddIndices, ref int offset)
		{
			// Front z = _z

			AddPositionAndNormal(new PointD3D(_x, _y, _z), new VectorD3D(0, 0, -1));
			AddPositionAndNormal(new PointD3D(_x, _y + _dy, _z), new VectorD3D(0, 0, -1));
			AddPositionAndNormal(new PointD3D(_x + _dx, _y + _dy, _z), new VectorD3D(0, 0, -1));
			AddPositionAndNormal(new PointD3D(_x + _dx, _y, _z), new VectorD3D(0, 0, -1));

			AddIndices(0, 1, 2);
			AddIndices(0, 2, 3);

			// Back z = z+dz
			AddPositionAndNormal(new PointD3D(_x, _y, _z + _dz), new VectorD3D(0, 0, 1));
			AddPositionAndNormal(new PointD3D(_x + _dx, _y + _dy, _z + _dz), new VectorD3D(0, 0, 1));
			AddPositionAndNormal(new PointD3D(_x, _y + _dy, _z + _dz), new VectorD3D(0, 0, 1));
			AddPositionAndNormal(new PointD3D(_x + _dx, _y, _z + _dz), new VectorD3D(0, 0, 1));

			AddIndices(4, 5, 6);
			AddIndices(4, 7, 5);

			// Top y = y+dy
			AddPositionAndNormal(new PointD3D(_x, _y + _dy, _z), new VectorD3D(0, 1, 0));
			AddPositionAndNormal(new PointD3D(_x, _y + _dy, _z + _dz), new VectorD3D(0, 1, 0));
			AddPositionAndNormal(new PointD3D(_x + _dx, _y + _dy, _z + _dz), new VectorD3D(0, 1, 0));
			AddPositionAndNormal(new PointD3D(_x + _dx, _y + _dy, _z), new VectorD3D(0, 1, 0));

			AddIndices(8, 9, 10);
			AddIndices(8, 10, 11);

			// Bottom y = y
			AddPositionAndNormal(new PointD3D(_x, _y, _z), new VectorD3D(0, -1, 0));
			AddPositionAndNormal(new PointD3D(_x + _dx, _y, _z + _dz), new VectorD3D(0, -1, 0));
			AddPositionAndNormal(new PointD3D(_x, _y, _z + _dz), new VectorD3D(0, -1, 0));
			AddPositionAndNormal(new PointD3D(_x + _dx, _y, _z), new VectorD3D(0, -1, 0));

			AddIndices(12, 13, 14);
			AddIndices(12, 15, 13);

			// Left x = x
			AddPositionAndNormal(new PointD3D(_x, _y, _z), new VectorD3D(-1, 0, 0));
			AddPositionAndNormal(new PointD3D(_x, _y, _z + _dz), new VectorD3D(-1, 0, 0));
			AddPositionAndNormal(new PointD3D(_x, _y + _dy, _z + _dz), new VectorD3D(-1, 0, 0));
			AddPositionAndNormal(new PointD3D(_x, _y + _dy, _z), new VectorD3D(-1, 0, 0));

			AddIndices(16, 17, 18);
			AddIndices(16, 18, 19);

			// Right x = x + dx
			AddPositionAndNormal(new PointD3D(_x + _dx, _y, _z), new VectorD3D(1, 0, 0));
			AddPositionAndNormal(new PointD3D(_x + _dx, _y + _dy, _z + _dz), new VectorD3D(1, 0, 0));
			AddPositionAndNormal(new PointD3D(_x + _dx, _y, _z + _dz), new VectorD3D(1, 0, 0));
			AddPositionAndNormal(new PointD3D(_x + _dx, _y + _dy, _z), new VectorD3D(1, 0, 0));

			AddIndices(20, 21, 22);
			AddIndices(20, 23, 21);
		}
	}
}