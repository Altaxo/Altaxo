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
		/// Gets the width of the view field at target distance. For this perspective camera, it is <see cref="CameraBase.WidthAtZNear"/> multiplied with the <see cref="CameraBase.DistanceToTarget"/> divided by <see cref="CameraBase.ZNear"/>.
		/// </summary>
		/// <value>
		/// The width of the view field at target distance.
		/// </value>
		public override double WidthAtTargetDistance
		{
			get
			{
				return _widthAtZNear * DistanceToTarget / _zNear;
			}
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
		/// Zooms the view by getting the camera closer to the target position. The target position itself is kept.
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

				var denom = (Math.Sqrt((term1 - term2 - wqrxq) * (term1 - term2 + term4 - wqrxq)) + term1 * distanceFactor - wqrxq * distanceFactor);
				var nomin = (-2 * _zNear * Math.Sqrt(-2 * term1 + term2 * (2 + term3) - 2 * term4 + 2 * wqrxq - 2 * Math.Sqrt((-term1 + term2 + wqrxq) * (-term1 + term2 - term4 + wqrxq)) * distanceFactor));

				var tanAzimuth = nomin / denom;
				var cosAzimuth = 1 / Math.Sqrt(1 + tanAzimuth * tanAzimuth);
				var sinAzimuth = tanAzimuth * cosAzimuth;

				// Calculation of tan[elevation]

				var term6 = fznq * distanceFactor + hqryq * cosAzimuth;
				var term7 = Math.Sqrt((hqryq + fznq * distanceFactor * distanceFactor) * (fznq + hqryq * cosAzimuth * cosAzimuth) - hqryq * wqrxq * sinAzimuth * sinAzimuth);

				denom = ((hqryq - 2 * rx * w * _zNear * sinAzimuth) *
					(hqryq * (-term6 + hqryq * cosAzimuth - 2 * rx * w * _zNear * distanceFactor * sinAzimuth) -
					Math.Abs(hqryq - 2 * rx * w * _zNear * sinAzimuth) *
					Math.Sqrt(hqryq * (fznq + hqryq - term6 * distanceFactor + hqryq * distanceFactor * cosAzimuth) +
					(fznq + hqryq) * wqrxq * sinAzimuth * sinAzimuth)));

				nomin = (h * ry * (-2 * hqryq * hqryq * _zNear * distanceFactor +
					2 * (term6 - hqryq * cosAzimuth) * sinAzimuth *
					(hqryq * rx * w - wqrxq * _zNear * sinAzimuth) +
					Math.Abs(hqryq - 2 * rx * w * _zNear * sinAzimuth) * (2 * _zNear + rx * w * sinAzimuth) *
					Math.Sqrt(hqryq * (fznq + hqryq - term6 * distanceFactor + hqryq * distanceFactor * cosAzimuth) +
					(fznq + hqryq) * wqrxq * sinAzimuth * sinAzimuth)));

				var tanElevation = nomin / denom;
				var cosElevation = 1 / Math.Sqrt(1 + tanElevation * tanElevation);
				var sinElevation = tanElevation * cosElevation;

				// now rotate our current LookAt with the azimuth and elevation (elevation first, then azimuth)
				Matrix4x3 rotMatrix = new Matrix4x3(
					cosAzimuth, -sinAzimuth * sinElevation, sinAzimuth*cosElevation,
					0, cosElevation, sinElevation,
					-sinAzimuth, -cosAzimuth * sinElevation, cosAzimuth * cosElevation,
					0, 0, 0);

				Matrix4x3 newLookAt = this.WithDistanceToTarget(distanceFactor * DistanceToTarget).LookAtRHMatrix.WithPrependedTransformation(rotMatrix);

				var newEyeVec = new VectorD3D(newLookAt.M13, newLookAt.M23, newLookAt.M33);
				var newUpVec = new VectorD3D(newLookAt.M12, newLookAt.M22, newLookAt.M32);
				var newDistance = this.DistanceToTarget * distanceFactor;
				var newEyePosition = this._targetPosition + newEyeVec * newDistance;
				var newCam = (PerspectiveCamera)this.WithUpEye(newUpVec, newEyePosition);
				var controlLookAt = newCam.LookAtRHMatrix;

				return newCam;
			}
		}

		#endregion Zoom by getting closer
	}
}