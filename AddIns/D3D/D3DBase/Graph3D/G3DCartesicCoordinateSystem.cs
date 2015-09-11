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

using Altaxo.Graph;
using Altaxo.Graph3D.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Graph3D
{
	public class G3DCartesicCoordinateSystem : G3DCoordinateSystem
	{
		public override bool Is3D
		{
			get
			{
				return true;
			}
		}

		public override bool IsAffine
		{
			get
			{
				return true;
			}
		}

		public override bool IsOrthogonal
		{
			get
			{
				return true;
			}
		}

		public override object Clone()
		{
			throw new NotImplementedException();
		}

		public override string GetAxisSideName(CSLineID id, CSAxisSide side)
		{
			throw new NotImplementedException();
		}

		public override ISweepPath3D GetIsoline(Logical3D r0, Logical3D r1)
		{
			PointD3D pt0, pt1;
			LogicalToLayerCoordinates(r0, out pt0);
			LogicalToLayerCoordinates(r1, out pt1);
			return new Primitives.StraightLineSweepPath3D(pt0, pt1);
		}

		public override bool LayerToLogicalCoordinates(PointD3D location, out Logical3D r)
		{
			r = new Logical3D(location.X / _layerSize.X, location.Y / _layerSize.Y, location.Z / _layerSize.Z);
			return true;
		}

		public override bool LogicalToLayerCoordinates(Logical3D r, out PointD3D location)
		{
			location = new PointD3D(r.RX * _layerSize.X, r.RY * _layerSize.Y, r.RZ * _layerSize.Z);
			return true;
		}

		public override bool LogicalToLayerCoordinatesAndDirection(Logical3D r0, Logical3D r1, double t, out PointD3D position, out VectorD3D direction)
		{
			PointD3D pt0, pt1, ptt;
			LogicalToLayerCoordinates(r0, out pt0);
			LogicalToLayerCoordinates(r1, out pt1);
			LogicalToLayerCoordinates(r0.InterpolateTo(r1, t), out position);
			direction = (pt1 - pt0);
			direction.Normalize();
			return true;
		}

		protected override void UpdateAxisInfo()
		{
			throw new NotImplementedException();
		}
	}
}