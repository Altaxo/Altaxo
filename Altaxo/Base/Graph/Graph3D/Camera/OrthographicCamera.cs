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
	public class OrthographicCamera : CameraBase
	{
		public double Scale { get; set; }

		#region Serialization

		/// <summary>
		/// 2015-11-14 initial version.
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(OrthographicCamera), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (OrthographicCamera)obj;
				info.AddBaseValueEmbedded(s, s.GetType().BaseType);
				info.AddValue("Scale", s.Scale);
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = (OrthographicCamera)o ?? new OrthographicCamera();
				info.GetBaseValueEmbedded(s, s.GetType().BaseType, parent);
				s.Scale = info.GetDouble("Scale");
				return s;
			}
		}

		#endregion Serialization

		public OrthographicCamera()
		{
			Scale = 1;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OrthographicCamera"/> class.
		/// </summary>
		/// <param name="from">Camera to copy the data from.</param>
		public OrthographicCamera(OrthographicCamera from)
		{
			CopyFrom(from);
		}

		/// <summary>
		/// Makes a copy of the provided instance.
		/// </summary>
		/// <param name="obj">The object to copy from.</param>
		/// <returns>A copy of the provided instance.</returns>
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

		/// <summary>
		/// Clones this instance.
		/// </summary>
		/// <returns>Clone of this instance.</returns>
		public override object Clone()
		{
			return new OrthographicCamera(this);
		}

		/// <summary>
		/// Gets a matrix for a hit point on the screen. The hit point is given in relative screen coordinates (X and Y component, 0..1). The screen's aspect ratio is given in the Z component.
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

		/// <summary>
		/// Gets the OrthoRH matrix. The OrthoRH matrix transforms the camera coordinates into the view volume coordinates (X=(-1..+1), Y=(-1..+1), Z=(0..1)).
		/// </summary>
		/// <param name="aspectRatio">The aspect ratio of the screen (or whatever the 2D output medium is).</param>
		/// <param name="zNearPlane">The z near plane, i.e. the z camera coordinate of the near end of the view volume.</param>
		/// <param name="zFarPlane">The z far plane, i.e. the z camera coordinate of the far end of the view volume.</param>
		/// <returns>The OrthoRH matrix.</returns>
		public MatrixD3D GetOrthoRHMatrix(double aspectRatio, double zNearPlane, double zFarPlane)
		{
			return new MatrixD3D(
				2 / Scale, 0, 0,
				0, 2 / (Scale * aspectRatio), 0,
				0, 0, 1 / (zNearPlane - zFarPlane),
				ScreenOffset.X, ScreenOffset.Y, zNearPlane / (zNearPlane - zFarPlane)
				);
		}

		/// <summary>
		/// Gets the LookAtRH matrix multiplied with the OrthoRH (see <see cref="GetOrthoRHMatrix(double, double, double)"/>) matrix.
		/// </summary>
		/// <param name="aspectRatio">The aspect ratio of the screen (or whatever the 2D output medium is).</param>
		/// <returns>The LookAtRH matrix multiplied with the OrthoRH matrix.</returns>
		public MatrixD3D GetLookAtRHTimesOrthoRHMatrix(double aspectRatio)
		{
			return GetLookAtRHTimesOrthoRHMatrix(aspectRatio, ZNear, ZFar);
		}

		/// <summary>
		/// Gets the LookAtRH matrix multiplied with the OrthoRH (see <see cref="GetOrthoRHMatrix(double, double, double)"/>) matrix.
		/// </summary>
		/// <param name="aspectRatio">The aspect ratio of the screen (or whatever the 2D output medium is).</param>
		/// <param name="zNearPlane">The z near plane, i.e. the z camera coordinate of the near end of the view volume.</param>
		/// <param name="zFarPlane">The z far plane, i.e. the z camera coordinate of the far end of the view volume.</param>
		/// <returns>The LookAtRH matrix multiplied with the OrthoRH matrix.</returns>
		public MatrixD3D GetLookAtRHTimesOrthoRHMatrix(double aspectRatio, double zNearPlane, double zFarPlane)
		{
			var result = LookAtRHMatrix;
			result.AppendTransform(new MatrixD3D(
				2 / Scale, 0, 0,
				0, 2 / (Scale * aspectRatio), 0,
				0, 0, 1 / (zNearPlane - zFarPlane),
				ScreenOffset.X, ScreenOffset.Y, zNearPlane / (zNearPlane - zFarPlane)
				));

			return result;
		}
	}
}