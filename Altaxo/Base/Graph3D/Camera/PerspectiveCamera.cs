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

namespace Altaxo.Graph3D.Camera
{
	public class PerspectiveCamera : CameraBase
	{
		public double Angle { get; set; }

		public PerspectiveCamera()
		{
			Angle = Math.PI / 4;
			EyePosition = new PointD3D(0, 0, -1500);
			UpVector = new VectorD3D(0, 0, 1);
		}

		public PerspectiveCamera(PerspectiveCamera from)
		{
			CopyFrom(from);
		}

		public override bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;

			if (false == base.CopyFrom(obj))
				return false;

			var from = obj as PerspectiveCamera;
			if (null != from)
			{
				this.Angle = from.Angle;
				return true;
			}
			else
			{
				return false;
			}
		}

		public override object Clone()
		{
			return new PerspectiveCamera(this);
		}

		public override MatrixD3D GetHitRayMatrix(PointD3D relativeScreenPosition)
		{
			throw new NotImplementedException();
		}
	}
}