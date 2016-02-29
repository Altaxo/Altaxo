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
	public class PerspectiveCamera : CameraBase
	{
		#region Serialization

		/// <summary>
		/// 2015-11-14 initial version.
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PerspectiveCamera), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (PerspectiveCamera)obj;
				info.AddValue("UpVector", s._upVector);
				info.AddValue("EyePosition", s._eyePosition);
				info.AddValue("TargetPosition", s._targetPosition);
				info.AddValue("ZNear", s._zNear);
				info.AddValue("ZFar", s._zFar);
				info.AddValue("Width", s._widthAtZNear);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = (PerspectiveCamera)o ?? new PerspectiveCamera();
				s._upVector = (VectorD3D)info.GetValue("UpVector", s);
				s._eyePosition = (PointD3D)info.GetValue("EyePosition", s);
				s._targetPosition = (PointD3D)info.GetValue("TargetPosition", s);
				s._zNear = info.GetDouble("ZNear");
				s._zFar = info.GetDouble("ZFar");
				s._widthAtZNear = info.GetDouble("Width");
				return s;
			}
		}

		#endregion Serialization

		#region Constructors

		public PerspectiveCamera()
						: base(new VectorD3D(0, 0, 1), new PointD3D(0, 0, -1500), new PointD3D(0, 0, 0), 150, 3000, 50)
		{
		}

		public PerspectiveCamera(VectorD3D upVector, PointD3D eyePosition, PointD3D targetPosition, double zNear, double zFar, double widthAtZNear)
						: base(upVector, eyePosition, targetPosition, zNear, zFar, widthAtZNear)
		{
		}

		#endregion Constructors

		#region Overrides

		/// <summary>
		/// Gets the width of the view field at target distance. For this perspective camera, it is <see cref="CameraBase.WidthAtZNear"/> multiplied with the <see cref="CameraBase.Distance"/> divided by <see cref="CameraBase.ZNear"/>.
		/// </summary>
		/// <value>
		/// The width of the view field at target distance.
		/// </value>
		public override double WidthAtTargetDistance
		{
			get
			{
				return _widthAtZNear * Distance / _zNear;
			}
		}

		/// <summary>
		/// Gets a new instance of the camera with  <see cref="CameraBase.ZNear" /> and <see cref="CameraBase.ZFar" /> set to the provided values. The <see cref="WidthAtTargetDistance" /> is adjusted so that the view angle of the camera is not changed.
		/// </summary>
		/// <param name="zNear">The zNear distance.</param>
		/// <param name="zFar">The zFar distance.</param>
		/// <returns>
		/// A new instance of the camera with  <see cref="CameraBase.ZNear" /> and <see cref="CameraBase.ZFar" /> set to the provided values. The <see cref="WidthAtTargetDistance" /> is adjusted so that the view angle of the camera is not changed.
		/// </returns>
		public override CameraBase WithZNearZFarWithoutChangingViewAngle(double zNear, double zFar)
		{
			if (zNear == _zNear && zFar == _zFar)
				return this;

			if (!(zNear > 0))
				throw new ArgumentOutOfRangeException(nameof(zNear) + " has to be > 0 ");
			if (!(zFar > 0))
				throw new ArgumentOutOfRangeException(nameof(zFar) + " has to be > 0 ");
			if (!(zNear < zFar))
				throw new ArgumentOutOfRangeException(nameof(zFar) + " has to be > " + nameof(zNear));

			var result = (PerspectiveCamera)this.MemberwiseClone();

			result._widthAtZNear = _widthAtZNear * zNear / _zNear; // Adjust width so that view angle is kept
			result._zNear = zNear;
			result._zFar = zFar;
			return result;
		}

		#endregion Overrides

		#region Compound setters

		/// <summary>
		/// Creates a new camera with provided  eyePosition and targetPosition;
		/// </summary>
		/// <param name="eyePosition">The eye position.</param>
		/// <param name="targetPosition">The target position.</param>
		/// <param name="widthAtZNear">The width of the view field at <see cref="CameraBase.ZNear"/> distance.</param>
		/// <returns>New camera with the provided parameters.</returns>
		public PerspectiveCamera WithEyeTargetWidth(PointD3D eyePosition, PointD3D targetPosition, double widthAtZNear)
		{
			var result = (PerspectiveCamera)this.MemberwiseClone();
			result._eyePosition = eyePosition;
			result._targetPosition = targetPosition;
			result._widthAtZNear = widthAtZNear;
			return result;
		}

		#endregion Compound setters

		#region Matrix getters

		/// <summary>
		/// Gets the transposed result of LookAtRH matrix multiplied with the OrthoRH matrix.
		/// </summary>
		/// <param name="aspectRatio">The aspect ratio of the screen (or whatever the 2D output medium is).</param>
		/// <returns>The LookAtRH matrix multiplied with the OrthoRH matrix.</returns>
		public override Matrix4x4 GetViewProjectionMatrix(double aspectRatio)
		{
			return GetViewProjectionMatrix(aspectRatio, ZNear, ZFar, _screenOffset.X, _screenOffset.Y);
		}

		/// <summary>
		/// Gets the transposed result of LookAtRH matrix multiplied with the OrthoRH matrix.
		/// </summary>
		/// <param name="aspectRatio">The aspect ratio of the screen (or whatever the 2D output medium is).</param>
		/// <param name="zNearPlane">The z near plane, i.e. the z camera coordinate of the near end of the view volume.</param>
		/// <param name="zFarPlane">The z far plane, i.e. the z camera coordinate of the far end of the view volume.</param>
		/// <param name="screenOffsetX">The x component of the relative screen offset.</param>
		/// <param name="screenOffsetY">The y component of the relative screen offset.</param>
		/// <returns>The LookAtRH matrix multiplied with the OrthoRH matrix.</returns>
		public Matrix4x4 GetViewProjectionMatrix(double aspectRatio, double zNearPlane, double zFarPlane, double screenOffsetX, double screenOffsetY)
		{
			double scaleX = 2 * zNearPlane / _widthAtZNear;
			double scaleY = 2 * zNearPlane / (_widthAtZNear * aspectRatio);
			double scaleZ = zFarPlane / (zNearPlane - zFarPlane);

			var l = LookAtRHMatrix;
			return new Matrix4x4(
							 l.M11 * scaleX - l.M13 * screenOffsetX, l.M12 * scaleY - l.M13 * screenOffsetY, l.M13 * scaleZ, -l.M13,
							 l.M21 * scaleX - l.M23 * screenOffsetX, l.M22 * scaleY - l.M23 * screenOffsetY, l.M23 * scaleZ, -l.M23,
							 l.M31 * scaleX - l.M33 * screenOffsetX, l.M32 * scaleY - l.M33 * screenOffsetY, l.M33 * scaleZ, -l.M33,
							 l.M41 * scaleX - l.M43 * screenOffsetX, l.M42 * scaleY - l.M43 * screenOffsetY, (l.M43 + zNearPlane) * scaleZ, -l.M43
							);
		}

		public override Matrix4x4 GetHitRayMatrix(PointD3D relativeScreenPosition)
		{
			double relX = relativeScreenPosition.X * 2 - 1; // relative Screen Position from -1..1
			double relY = relativeScreenPosition.Y * 2 - 1; // relative Screen Position from -1..1
			return GetViewProjectionMatrix(relativeScreenPosition.Z, ZNear, ZFar, -relX, -relY);
		}

		#endregion Matrix getters

		#region Zoom by getting closer

		/// <summary>
		/// Zooms the view by moving the camera along the straigth line between camera and target position closer to the target,
		/// and then slightly rotate the camera so that the relative screen points rx and ry are matched before and after the zooming (for a point at the plane at camera distance).
		/// The target position itself is kept.
		/// </summary>
		/// <param name="distanceFactor">The factor by which the distance is multiplied to get the new distance. Must be greater than zero. Values less than 1 means zoom in, values greater than one zoom out.</param>
		/// <param name="rx">The x component of the relative screen coordinate of the screen point which should afterwards show the same object (at target distance). Is a value in interval [-1,1].</param>
		/// <param name="ry">The y component of the relative screen coordinate of the screen point which should afterwards show the same object (at target distance). Is a value in interval [-1,1].</param>
		/// <param name="aspectRatio">The aspect ratio of the screen (y/x).</param>
		/// <returns>A new camera with another distance to the target, determined by parameter <paramref name="distanceFactor"/>.</returns>
		public PerspectiveCamera ZoomByGettingCloserToTarget(double distanceFactor, double rx, double ry, double aspectRatio)
		{
			if (rx == 0 && ry == 0)
			{
				var newEyePosition = _targetPosition + (_eyePosition - _targetPosition) * distanceFactor;
				return (PerspectiveCamera)this.WithEyeTarget(newEyePosition, _targetPosition);
			}
			else
			{
				double w = _widthAtZNear;
				double h = _widthAtZNear * aspectRatio;

				var fznq = 4 * _zNear * _zNear;
				var hqryq = h * h * ry * ry;
				var wqrxq = w * w * rx * rx;

				// Calculation of tan[azimuth]
				var term1 = wqrxq + fznq;
				var term2 = hqryq + term1;
				var term3 = distanceFactor * distanceFactor - 1;
				var term4 = fznq * term3;

				var denom = (term1 * (term2 + term4) - (2 * term2 + term4) * wqrxq);
				var nomin = (2 * rx * w * (term2 + term4 - Math.Sqrt(term2 * (term2 + term4 + term3 * wqrxq))) * _zNear);

				var tanAzimuth = nomin / denom;
				var cosAzimuth = 1 / Math.Sqrt(1 + tanAzimuth * tanAzimuth);
				var sinAzimuth = tanAzimuth * cosAzimuth;

				// Calculation of tan[elevation]

				var term6 = fznq * distanceFactor + hqryq * cosAzimuth;
				var term7 = (hqryq + fznq * distanceFactor * distanceFactor) * (fznq + hqryq * cosAzimuth * cosAzimuth) - hqryq * wqrxq * sinAzimuth * sinAzimuth;

				denom = (2 * hqryq * rx * sinAzimuth * term6 * w * _zNear * (-1 + cosAzimuth * distanceFactor) + term6 * Math.Sqrt(term7) * Math.Abs(term6));
				nomin = (h * ry * (-(rx * sinAzimuth * term6 * term6 * w) + 2 * Math.Sqrt(term7) * _zNear * (-1 + cosAzimuth * distanceFactor) * Math.Abs(term6)));

				var tanElevation = nomin / denom;
				var cosElevation = 1 / Math.Sqrt(1 + tanElevation * tanElevation);
				var sinElevation = tanElevation * cosElevation;

				// now rotate our current LookAt with the azimuth and elevation (elevation first, then azimuth)
				double distance = this.Distance;
				Matrix4x3 rotMatrix = new Matrix4x3(
					cosAzimuth, 0, sinAzimuth,
					-sinAzimuth * sinElevation, cosElevation, cosAzimuth * sinElevation,
					-cosElevation * sinAzimuth, -sinElevation, cosAzimuth * cosElevation,
					distance * (distanceFactor - 1) * sinAzimuth * cosElevation, distance * (distanceFactor - 1) * sinElevation, distance * (1 - distanceFactor) * cosAzimuth * cosElevation
					);

				Matrix4x3 newLookAt = this.LookAtRHMatrix.WithAppendedTransformation(rotMatrix);

				return (PerspectiveCamera)this.WithLookAtRHMatrix(newLookAt, Distance * distanceFactor);

				/* the code that will setup the camera manually:
				var newEyeVec = new VectorD3D(newLookAt.M13, newLookAt.M23, newLookAt.M33);
				var newUpVec = new VectorD3D(newLookAt.M12, newLookAt.M22, newLookAt.M32);
				var newDistance = this.DistanceToTarget * distanceFactor;
				var newEyePosition = this.TargetPosition + this.EyeVector * distanceFactor; // we move the camera straigt in target direction, but then turn it
				var newTargetPosition = newEyePosition - newEyeVec * newDistance;
				var newCam = (PerspectiveCamera)this.WithUpEyeTargetZNearZFar(newUpVec, newEyePosition, newTargetPosition, this.ZNear, this.ZFar);
				var controlLookAt = newCam.LookAtRHMatrix;
				return newCam;
				*/
			}
		}

		#endregion Zoom by getting closer

		#region Zoom by moving camera forward

		/// <summary>
		/// Zooms the view by moving the camera forward, i.e. by shifting both eye position and target position by the same amount, thus keeping the camera direction and the up vector constant.
		/// </summary>
		/// <param name="distanceFactor">The factor by which the distance is multiplied to get the moving distance = DistanceToTarget*(1-distanceFactor). Must be greater than zero. Values less than 1 means zoom in, values greater than one zoom out.</param>
		/// <param name="rx">The x component of the relative screen coordinate of the screen point which should afterwards show the same object (at target distance). Is a value in interval [-1,1].</param>
		/// <param name="ry">The y component of the relative screen coordinate of the screen point which should afterwards show the same object (at target distance). Is a value in interval [-1,1].</param>
		/// <param name="aspectRatio">The aspect ratio of the screen (y/x).</param>
		/// <returns>A new camera shofted forward by a difference, determined by parameter <paramref name="distanceFactor"/> and the current camera distance.</returns>
		public PerspectiveCamera ZoomByMovingCameraForward(double distanceFactor, double rx, double ry, double aspectRatio)
		{
			double tanx = rx * _widthAtZNear / (2 * _zNear);
			double tany = ry * _widthAtZNear * aspectRatio / (2 * _zNear);

			double diffDistance = this.Distance * (1 - distanceFactor);

			var eyeVec = this.TargetToEyeVectorNormalized;
			var up = this.UpVectorPerpendicularToEyeVectorNormalized;
			var right = this.RightVectorPerpendicularToEyeVectorNormalized;

			var diffVector = -diffDistance * eyeVec + tanx * diffDistance * right + tany * diffDistance * up;

			return (PerspectiveCamera)this.WithEyeTarget(this.EyePosition + diffVector, this.TargetPosition + diffVector);
		}

		#endregion Zoom by moving camera forward
	}
}