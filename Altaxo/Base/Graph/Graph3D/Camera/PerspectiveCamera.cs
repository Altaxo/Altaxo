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
		protected double _scale;

		public double Scale { get { return _scale; } }

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
				info.AddValue("Scale", s._scale);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = (PerspectiveCamera)o ?? new PerspectiveCamera();
				s._upVector = (VectorD3D)info.GetValue("UpVector", s);
				s._eyePosition = (PointD3D)info.GetValue("EyePosition", s);
				s._targetPosition = (PointD3D)info.GetValue("TargetPosition", s);
				s._zNear = info.GetDouble("ZNear");
				s._zFar = info.GetDouble("ZFar");
				s._scale = info.GetDouble("Scale");
				return s;
			}
		}

		#endregion Serialization

		public PerspectiveCamera WithScale(double scale)
		{
			if (_scale == scale)
				return this;

			var result = (PerspectiveCamera)this.MemberwiseClone();
			result._scale = scale;
			return result;
		}

		/// <summary>
		/// Creates a new camera with provided  eyePosition and targetPosition;
		/// </summary>
		/// <param name="eyePosition">The eye position.</param>
		/// <param name="targetPosition">The target position.</param>
		/// <param name="scale">The scale.</param>
		/// <returns>New camera with the provided parameters.</returns>
		public PerspectiveCamera WithEyeTargetScale(PointD3D eyePosition, PointD3D targetPosition, double scale)
		{
			var result = (PerspectiveCamera)this.MemberwiseClone();
			result._eyePosition = eyePosition;
			result._targetPosition = targetPosition;
			result._scale = scale;
			return result;
		}

		/// <summary>
		/// Gets the LookAtRH matrix multiplied with the OrthoRH (see <see cref="GetPerspectiveRHMatrix(double, double, double)"/>) matrix.
		/// </summary>
		/// <param name="aspectRatio">The aspect ratio of the screen (or whatever the 2D output medium is).</param>
		/// <returns>The LookAtRH matrix multiplied with the OrthoRH matrix.</returns>
		public MatrixD3D GetLookAtRHTimesPerspectiveRHMatrix(double aspectRatio)
		{
			return GetLookAtRHTimesPerspectiveRHMatrix(aspectRatio, ZNear, ZFar);
		}

		/// <summary>
		/// Gets the LookAtRH matrix multiplied with the OrthoRH (see <see cref="GetPerspectiveRHMatrix(double, double, double)"/>) matrix.
		/// </summary>
		/// <param name="aspectRatio">The aspect ratio of the screen (or whatever the 2D output medium is).</param>
		/// <param name="zN">The z near plane, i.e. the z camera coordinate of the near end of the view volume.</param>
		/// <param name="zF">The z far plane, i.e. the z camera coordinate of the far end of the view volume.</param>
		/// <returns>The LookAtRH matrix multiplied with the OrthoRH matrix.</returns>
		public MatrixD3D GetLookAtRHTimesPerspectiveRHMatrix(double aspectRatio, double zN, double zF)
		{
			var m = LookAtRHMatrix;
			double scaleX = _scale;
			double scaleY = _scale * aspectRatio;
			double zN_zF = zN - zF;
			return new MatrixD3D(
				2 * m.M11 * zN / scaleX, 2 * m.M12 * zN / scaleY, m.M13 * zF / zN_zF,
				2 * m.M21 * zN / scaleX, 2 * m.M22 * zN / scaleY, m.M23 * zF / zN_zF,
				2 * m.M31 * zN / scaleX, 2 * m.M32 * zN / scaleY, m.M33 * zF / zN_zF,
				2 * m.M41 * zN / scaleX, 2 * m.M42 * zN / scaleY, m.M43 * zF / zN_zF
				);
		}

		public override MatrixD3D GetHitRayMatrix(PointD3D relativeScreenPosition)
		{
			throw new NotImplementedException();
		}
	}
}