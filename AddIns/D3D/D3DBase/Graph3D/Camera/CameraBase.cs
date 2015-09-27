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
	public abstract class CameraBase : Main.ICopyFrom
	{
		public VectorD3D UpVector { get; set; }
		public PointD3D EyePosition { get; set; }
		public PointD3D TargetPosition { get; set; }

		public CameraBase()
		{
			EyePosition = new PointD3D(0, 0, -1500);
			UpVector = new VectorD3D(0, 0, 1);
		}

		public abstract object Clone();

		public virtual bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;

			var from = obj as CameraBase;
			if (null != from)
			{
				this.UpVector = from.UpVector;
				this.EyePosition = from.EyePosition;
				this.TargetPosition = from.TargetPosition;
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Gets the eye vector, i.e. the vector pointing from target to the camera eye.
		/// </summary>
		/// <value>
		/// The eye vector.
		/// </value>
		public VectorD3D EyeVector
		{
			get
			{
				return (EyePosition - TargetPosition);
			}
		}

		/// <summary>
		/// Gets the normalized eye vector, i.e. the vector pointing from target to the camera eye.
		/// </summary>
		/// <value>
		/// The normalized eye vector.
		/// </value>
		public VectorD3D NormalizedEyeVector
		{
			get
			{
				return VectorD3D.CreateNormalized(EyePosition - TargetPosition);
			}
		}

		/// <summary>
		/// Gets the normalized up vector, that is made perpendicular to the eye vector.
		/// </summary>
		/// <value>
		/// The normalized up vector perpendicular to eye vector.
		/// </value>
		public VectorD3D NormalizedUpVectorPerpendicularToEyeVector
		{
			get
			{
				return Math3D.GetOrthonormalVectorToVector(UpVector, EyePosition - TargetPosition);
			}
		}
	}
}