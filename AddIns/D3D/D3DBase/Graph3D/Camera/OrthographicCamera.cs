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

namespace Altaxo.Graph3D.Camera
{
	public class OrthographicCamera : CameraBase
	{
		public double Scale { get; set; }

		public OrthographicCamera()
		{
			Scale = 1;
		}

		public OrthographicCamera(OrthographicCamera from)
		{
			CopyFrom(from);
		}

		public override bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;

			if (false == base.CopyFrom(obj))
				return false;

			var from = obj as OrthographicCamera;
			if (null != from)
			{
				this.Scale = from.Scale;
				return true;
			}
			else
			{
				return false;
			}
		}

		public override object Clone()
		{
			return new OrthographicCamera(this);
		}

		/// <summary>
		/// Gets a matrix for a hit point on the screen. The hit point is given in relative coordinates (X and Y component). The screen's aspect ratio is given in the Z component.
		/// The result is a matrix which transforms world coordinates in that way that the hit ray in world coordinates is transformed to x=0 and y=0 and z being the distance to the camera.
		/// </summary>
		/// <param name="relativeScreenPosition">The relative screen position (X and Y component), as well as the screen's aspect ratio (Z component).</param>
		/// <returns>
		/// Matrix which transforms world coordinates in that way that the hit ray in world coordinates is transformed to x=0 and y=0 and z being the distance to the camera.
		/// </returns>
		public override MatrixD3D GetHitRayMatrix(PointD3D relativeScreenPosition)
		{
			var result = LookAtRHMatrix;
			result.TranslateAppend((0.5 - relativeScreenPosition.X) * Scale, (0.5 - relativeScreenPosition.Y) * Scale * relativeScreenPosition.Z, 0);
			return result;
		}
	}
}