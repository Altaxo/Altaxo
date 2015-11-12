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
		public double ZNear { get; set; }
		public double ZFar { get; set; }

		/// <summary>
		/// Gets or sets the screen offset. The screen offset has to be used only in extraordinary situation, e.g. for shifting to simulate multisampling; or for shifting to center the exported bitmap.
		/// It is not serialized either.
		/// </summary>
		/// <value>
		/// The screen offset (this is a relative value - relative to the dimensions of the screen).
		/// </value>
		public Altaxo.Graph.PointD2D ScreenOffset { get; set; }

		public CameraBase()
		{
			EyePosition = new PointD3D(0, 0, -1500);
			UpVector = new VectorD3D(0, 0, 1);
			ZNear = 150;
			ZFar = 3000;
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
				this.ZNear = from.ZNear;
				this.ZFar = from.ZFar;
				// ScreenOffset is temporary, thus it is _not_ copied here
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

		/// <summary>
		/// Returns the same matrix that the Direct3D function LookAtRH would provide.
		/// </summary>
		/// <value>
		/// The look at RH matrix.
		/// </value>
		public MatrixD3D LookAtRHMatrix
		{
			get
			{
				var zaxis = NormalizedEyeVector;
				var xaxis = VectorD3D.CreateNormalized(VectorD3D.CrossProduct(UpVector, zaxis));
				var yaxis = VectorD3D.CrossProduct(zaxis, xaxis);

				return new MatrixD3D(
					xaxis.X, yaxis.X, zaxis.X,
					xaxis.Y, yaxis.Y, zaxis.Y,
					xaxis.Z, yaxis.Z, zaxis.Z,
					-(xaxis.X * EyePosition.X + xaxis.Y * EyePosition.Y + xaxis.Z * EyePosition.Z), -(yaxis.X * EyePosition.X + yaxis.Y * EyePosition.Y + yaxis.Z * EyePosition.Z), -(zaxis.X * EyePosition.X + zaxis.Y * EyePosition.Y + zaxis.Z * EyePosition.Z)
					);
			}
		}

		/// <summary>
		/// Gets a matrix for a hit point on the screen. The hit point is given in relative coordinates (X and Y component). The screen's aspect ratio is given in the Z component.
		/// The result is a matrix which transforms world coordinates in that way that the hit ray in world coordinates is transformed to x=0 and y=0 and z being the distance to the camera.
		/// </summary>
		/// <param name="relativeScreenPosition">The relative screen position (X and Y component), as well as the screen's aspect ratio (Z component).</param>
		/// <returns>Matrix which transforms world coordinates in that way that the hit ray in world coordinates is transformed to x=0 and y=0 and z being the distance to the camera.</returns>
		public abstract MatrixD3D GetHitRayMatrix(PointD3D relativeScreenPosition);
	}
}