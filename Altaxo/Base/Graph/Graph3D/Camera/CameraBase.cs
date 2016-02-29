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

namespace Altaxo.Graph.Graph3D.Camera
{
	/// <summary>
	/// Represents the camera. Classes derived from here are meant to be immutable.
	/// </summary>
	/// <seealso cref="Altaxo.Main.IImmutable" />
	public abstract class CameraBase : Main.IImmutable
	{
		protected VectorD3D _upVector;
		protected PointD3D _eyePosition;
		protected PointD3D _targetPosition;
		protected double _zNear;
		protected double _zFar;
		protected double _widthAtZNear;

		/// <summary>
		/// Gets or sets the screen offset. The screen offset has to be used only in extraordinary situation,
		/// e.g. for shifting to simulate multisampling; or for shifting to center the exported bitmap.
		/// This is a relative value - relative to the dimensions of the screen.
		/// It is not serialized either.
		/// </summary>
		protected PointD2D _screenOffset;

		#region WidthAtZNear

		/// <summary>
		/// Gets the width of the view field at <see cref="ZNear"/> distance.
		/// </summary>
		/// <value>
		/// The width of the view field at <see cref="ZNear"/> distance.
		/// </value>
		public double WidthAtZNear
		{
			get
			{
				return _widthAtZNear;
			}
		}

		/// <summary>
		/// Gets a new instance of this camera, with <see cref="WidthAtZNear"/> set to the provided argument.
		/// </summary>
		/// <param name="widthAtZNear">The width of the view field at <see cref="ZNear"/> distance.</param>
		/// <returns>A new instance of this camera, with <see cref="WidthAtZNear"/> set to the provided argument <paramref name="widthAtZNear"/>.</returns>
		public CameraBase WithWidthAtZNear(double widthAtZNear)
		{
			if (_widthAtZNear == widthAtZNear)
				return this;

			var result = (CameraBase)this.MemberwiseClone();
			result._widthAtZNear = widthAtZNear;
			return result;
		}

		#endregion WidthAtZNear

		/// <summary>
		/// Gets the camera up vector.
		/// </summary>
		public VectorD3D UpVector { get { return _upVector; } }

		/// <summary>
		/// Gets the camera position, the so-called eye position.
		/// </summary>
		public PointD3D EyePosition { get { return _eyePosition; } }

		/// <summary>
		/// Gets the position the camera is looking at.
		/// </summary>
		public PointD3D TargetPosition { get { return _targetPosition; } }

		/// <summary>
		/// Gets the minimum distance the camera is 'seeing' something. Objects closer than this distance (from the camera) will not be visible.
		/// </summary>
		public double ZNear { get { return _zNear; } }

		/// <summary>
		/// Gets the maximum distance the camera is 'seeing' something. Objects farther away than this distance (from the camera) will not be visible.
		/// </summary>
		public double ZFar { get { return _zFar; } }

		/// <summary>
		/// Gets the width of the view field at target distance.
		/// </summary>
		/// <value>
		/// The width of the view field at target distance.
		/// </value>
		public abstract double WidthAtTargetDistance { get; }

		/// <summary>
		/// Gets a new instance of the camera with  <see cref="ZNear"/> and <see cref="ZFar"/> set to the provided values. The <see cref="WidthAtTargetDistance"/> is adjusted so that the view angle of the camera is not changed.
		/// </summary>
		/// <param name="zNear">The zNear distance.</param>
		/// <param name="zFar">The zFar distance.</param>
		/// <returns>A new instance of the camera with  <see cref="ZNear"/> and <see cref="ZFar"/> set to the provided values. The <see cref="WidthAtTargetDistance"/> is adjusted so that the view angle of the camera is not changed.</returns>
		public abstract CameraBase WithZNearZFarWithoutChangingViewAngle(double zNear, double zFar);

		/// <summary>
		/// Gets the screen offset. The screen offset has to be used only in extraordinary situation, e.g. for shifting to simulate multisampling; or for shifting to center the exported bitmap.
		/// It is not serialized either.
		/// </summary>
		/// <value>
		/// The screen offset (this is a relative value - relative to the dimensions of the screen).
		/// </value>
		public PointD2D ScreenOffset { get { return _screenOffset; } }

		/// <summary>
		/// Gets a new instance with <see cref="ScreenOffset"/> set to the provided value. The screen offset has to be used only in extraordinary situation, e.g. for shifting to simulate multisampling; or for shifting to center the exported bitmap.
		/// It is not serialized either.
		/// </summary>
		/// <param name="screenOffset">The screen offset.</param>
		/// <returns>New instance of this class with <see cref="ScreenOffset"/> set to the provided value.</returns>
		public CameraBase WithScreenOffset(PointD2D screenOffset)
		{
			if (screenOffset != _screenOffset)
			{
				var result = (CameraBase)this.MemberwiseClone();
				result._screenOffset = screenOffset;
				return result;
			}
			else
			{
				return this;
			}
		}

		protected CameraBase(VectorD3D upVector, PointD3D eyePosition, PointD3D targetPosition, double zNear, double zFar, double widthAtZNear)
		{
			this._upVector = upVector;
			this._eyePosition = eyePosition;
			this._targetPosition = targetPosition;
			this._zNear = zNear;
			this._zFar = zFar;
			this._widthAtZNear = widthAtZNear;
		}

		/// <summary>
		/// Creates a new camera with provided upVector, eyePosition, targetPosition, znear and  zfar distance..
		/// </summary>
		/// <param name="upVector">Up vector.</param>
		/// <param name="eyePosition">The eye position.</param>
		/// <param name="targetPosition">The target position.</param>
		/// <param name="zNear">The z near distance.</param>
		/// <param name="zFar">The z far distance.</param>
		/// <returns>New camera with the provided parameters.</returns>
		public CameraBase WithUpEyeTargetZNearZFar(VectorD3D upVector, PointD3D eyePosition, PointD3D targetPosition, double zNear, double zFar)
		{
			var result = (CameraBase)this.MemberwiseClone();
			result._upVector = upVector;
			result._eyePosition = eyePosition;
			result._targetPosition = targetPosition;
			result._zNear = zNear;
			result._zFar = zFar;
			return result;
		}

		/// <summary>
		/// Creates a new camera with provided upVector, eyePosition, targetPosition.
		/// </summary>
		/// <param name="upVector">Up vector.</param>
		/// <param name="eyePosition">The eye position.</param>
		/// <param name="targetPosition">The target position.</param>
		/// <returns>New camera with the provided parameters.</returns>
		public CameraBase WithUpEyeTarget(VectorD3D upVector, PointD3D eyePosition, PointD3D targetPosition)
		{
			var result = (CameraBase)this.MemberwiseClone();
			result._upVector = upVector;
			result._eyePosition = eyePosition;
			result._targetPosition = targetPosition;
			return result;
		}

		/// <summary>
		/// Creates a new camera with provided upVector and eyePosition.
		/// </summary>
		/// <param name="upVector">Up vector.</param>
		/// <param name="eyePosition">The eye position.</param>
		/// <returns>New camera with the provided parameters.</returns>
		public CameraBase WithUpEye(VectorD3D upVector, PointD3D eyePosition)
		{
			var result = (CameraBase)this.MemberwiseClone();
			result._upVector = upVector;
			result._eyePosition = eyePosition;
			return result;
		}

		/// <summary>
		/// Creates a new camera with provided  eyePosition and targetPosition;
		/// </summary>
		/// <param name="eyePosition">The eye position.</param>
		/// <param name="targetPosition">The target position.</param>
		/// <returns>New camera with the provided parameters.</returns>
		public CameraBase WithEyeTarget(PointD3D eyePosition, PointD3D targetPosition)
		{
			var result = (CameraBase)this.MemberwiseClone();
			result._eyePosition = eyePosition;
			result._targetPosition = targetPosition;
			return result;
		}

		/// <summary>
		/// Gets the eye vector, i.e. the vector pointing from target to the camera eye.
		/// </summary>
		/// <value>
		/// The eye vector.
		/// </value>
		public VectorD3D TargetToEyeVector
		{
			get
			{
				return (EyePosition - TargetPosition);
			}
		}

		/// <summary>
		/// Gets the distance of the camera from the target position.
		/// </summary>
		/// <value>
		/// The distance of the camera from the target position.
		/// </value>
		public double Distance
		{
			get
			{
				return (EyePosition - TargetPosition).Length;
			}
		}

		/// <summary>
		/// Gets a new camera with the distance between eye and target as provided in the argument <paramref name="distance"/>.
		/// The new distance is set by changing the eye position of the camera.
		/// </summary>
		/// <param name="distance">The new distance between eye and target.</param>
		/// <returns>A new camera with the distance between eye and target as provided in the argument <paramref name="distance"/>.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">distance has to be > 0</exception>
		public CameraBase WithDistanceByChangingEyePosition(double distance)
		{
			if (!(distance > 0))
				throw new ArgumentOutOfRangeException("distance has to be > 0");

			var diff = EyePosition - TargetPosition;

			var oldDistance = diff.Length;

			if (diff.Length == distance)
				return this;

			var result = (CameraBase)this.MemberwiseClone();

			result._eyePosition = TargetPosition + diff * (distance / oldDistance);

			return result;
		}

		/// <summary>
		/// Gets the normalized eye vector, i.e. the vector pointing from target to the camera eye.
		/// </summary>
		/// <value>
		/// The normalized eye vector.
		/// </value>
		public VectorD3D TargetToEyeVectorNormalized
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
		public VectorD3D UpVectorPerpendicularToEyeVectorNormalized
		{
			get
			{
				return Math3D.GetOrthonormalVectorToVector(UpVector, EyePosition - TargetPosition);
			}
		}

		/// <summary>
		/// Gets the normalized right vector, that is made perpendicular to the eye vector.
		/// </summary>
		/// <value>
		/// The normalized right vector perpendicular to eye vector.
		/// </value>
		public VectorD3D RightVectorPerpendicularToEyeVectorNormalized
		{
			get
			{
				var up = UpVectorPerpendicularToEyeVectorNormalized;
				var eye = TargetToEyeVectorNormalized;
				return VectorD3D.CrossProduct(up, eye);
			}
		}

		/// <summary>
		/// Returns the same matrix that the Direct3D function LookAtRH would provide.
		/// </summary>
		/// <value>
		/// The look at RH matrix.
		/// </value>
		public Matrix4x3 LookAtRHMatrix
		{
			get
			{
				var zaxis = TargetToEyeVectorNormalized;
				var xaxis = VectorD3D.CreateNormalized(VectorD3D.CrossProduct(UpVector, zaxis));
				var yaxis = VectorD3D.CrossProduct(zaxis, xaxis);

				return new Matrix4x3(
								xaxis.X, yaxis.X, zaxis.X,
								xaxis.Y, yaxis.Y, zaxis.Y,
								xaxis.Z, yaxis.Z, zaxis.Z,
								-(xaxis.X * EyePosition.X + xaxis.Y * EyePosition.Y + xaxis.Z * EyePosition.Z), -(yaxis.X * EyePosition.X + yaxis.Y * EyePosition.Y + yaxis.Z * EyePosition.Z), -(zaxis.X * EyePosition.X + zaxis.Y * EyePosition.Y + zaxis.Z * EyePosition.Z)
								);
			}
		}

		/// <summary>
		/// Creates a new camera which has the LookAtRH matrix as provided in the argument. Up and eye vector as well as eye position are calculated from the provided matrix, the target position is in such a way calculated that the distance is kept constant.
		/// </summary>
		/// <param name="l">The LookAtRH matrix. This matrix must have a determinant of 1, and each of the vectors {M11, M21, M31}, {M12, M22, M32}, {M13, M23, M33} must have a length of 1.</param>
		/// <returns>A new camera which has the LookAtRH matrix as provided in the argument. Up and eye vector as well as eye position are calculated from the provided matrix, the target position is in such a way calculated that the distance is kept constant.</returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public CameraBase WithLookAtRHMatrix(Matrix4x3 l)
		{
			return WithLookAtRHMatrix(l, this.Distance);
		}

		/// <summary>
		/// Creates a new camera which has the LookAtRH matrix as provided in the argument. Up and eye vector as well as eye position are calculated from the provided matrix, the target position is calculated from the eye vector and the provided <paramref name="newDistance"/> value.
		/// </summary>
		/// <param name="l">The LookAtRH matrix. This matrix must have a determinant of 1, and each of the vectors {M11, M21, M31}, {M12, M22, M32}, {M13, M23, M33} must have a length of 1.</param>
		/// <param name="newDistance">The distance between camera eye and target of the new camera.</param>
		/// <returns>A new camera which has the LookAtRH matrix as provided in the argument. Up and eye vector as well as eye position are calculated from the provided matrix, the target position is calculated from the eye vector and the provided <paramref name="newDistance"/> value.</returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public CameraBase WithLookAtRHMatrix(Matrix4x3 l, double newDistance)
		{
			double determinant = l.Determinant;

			if (!(determinant > 0.9 && determinant < 1.1))
				throw new ArgumentOutOfRangeException(nameof(l) + " seems not to be a LookAtRH matrix because its determinant is not 1");

			// get position
			var eyePos = new PointD3D(
			(l.M23 * l.M32 * l.M41 - l.M22 * l.M33 * l.M41 - l.M23 * l.M31 * l.M42 + l.M21 * l.M33 * l.M42 + l.M22 * l.M31 * l.M43 - l.M21 * l.M32 * l.M43) / determinant,
			(-(l.M13 * l.M32 * l.M41) + l.M12 * l.M33 * l.M41 + l.M13 * l.M31 * l.M42 - l.M11 * l.M33 * l.M42 - l.M12 * l.M31 * l.M43 + l.M11 * l.M32 * l.M43) / determinant,
			(l.M13 * l.M22 * l.M41 - l.M12 * l.M23 * l.M41 - l.M13 * l.M21 * l.M42 + l.M11 * l.M23 * l.M42 + l.M12 * l.M21 * l.M43 - l.M11 * l.M22 * l.M43) / determinant
			);

			var upVector = new VectorD3D(l.M12, l.M22, l.M32);
			var eyeVector = new VectorD3D(l.M13, l.M23, l.M33);

			return WithUpEyeTarget(upVector, eyePos, eyePos - eyeVector * newDistance);
		}

		/// <summary>
		/// Gets the inverse LookAtRH matrix. This matrix transforms a point from the camera coordinate system to the world coordinate system, i.e. the point (0,0,0) is transformed to (CameraPosition.X, CameraPosition.Y, CameraPosition.Z).
		/// </summary>
		/// <value>
		/// The inverse LookAtRH matrix.
		/// </value>
		public Matrix4x3 InverseLookAtRHMatrix
		{
			get
			{
				var zaxis = TargetToEyeVectorNormalized; // eye
				var xaxis = VectorD3D.CreateNormalized(VectorD3D.CrossProduct(UpVector, zaxis));
				var yaxis = VectorD3D.CrossProduct(zaxis, xaxis); // upvector

				return new Matrix4x3(
								xaxis.X, xaxis.Y, xaxis.Z,
								yaxis.X, yaxis.Y, yaxis.Z,
								zaxis.X, zaxis.Y, zaxis.Z,
								EyePosition.X, EyePosition.Y, EyePosition.Z
								);
			}
		}

		/// <summary>
		/// Gets a matrix for a hit point on the screen. The hit point is given in relative coordinates (X and Y component). The screen's aspect ratio is given in the Z component.
		/// The result is a matrix which transforms world coordinates in that way that the hit ray in world coordinates is transformed to x=0 and y=0 and z being the distance to the camera.
		/// </summary>
		/// <param name="relativeScreenPosition">The relative screen position (X and Y component), as well as the screen's aspect ratio (Z component).</param>
		/// <returns>Matrix which transforms world coordinates in that way that the hit ray in world coordinates is transformed to x=0 and y=0 and z being the distance to the camera.</returns>
		public abstract Matrix4x4 GetHitRayMatrix(PointD3D relativeScreenPosition);

		/// <summary>
		/// Gets the result of LookAtRH matrix multiplied with the ViewRH matrix (ViewRH is either OrthoRH or PerspectiveRH, depending on the camera type).
		/// </summary>
		/// <param name="aspectRatio">The aspect ratio of the screen (or whatever the 2D output medium is).</param>
		/// <returns>The LookAtRH matrix multiplied with the ViewRH matrix (ViewRH is either OrthoRH or PerspectiveRH, depending on the camera type).</returns>
		public abstract Matrix4x4 GetViewProjectionMatrix(double aspectRatio);
	}
}