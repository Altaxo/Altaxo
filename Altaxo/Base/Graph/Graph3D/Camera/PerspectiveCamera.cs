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

        public override double Scale { get { var distance = (this.TargetPosition - this.EyePosition).Length; return _scale * distance / _zNear; } }

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

        public PerspectiveCamera()
                : base(new VectorD3D(0, 0, 1), new PointD3D(0, 0, -1500), new PointD3D(0, 0, 0), 150, 3000)
        {
            _scale = 1000 * _zNear;
        }

        public PerspectiveCamera(VectorD3D upVector, PointD3D eyePosition, PointD3D targetPosition, double zNear, double zFar, double scaleByZNear)
                : base(upVector, eyePosition, targetPosition, zNear, zFar)
        {
            var distance = (this.TargetPosition - this.EyePosition).Length;
            this._scale = scaleByZNear * zNear / distance;
        }

        public PerspectiveCamera WithScale(double scale)
        {
            if (_scale == scale)
                return this;

            var result = (PerspectiveCamera)this.MemberwiseClone();
            result._scale = scale;
            return result;
        }

        public PerspectiveCamera WithScaleByZNear(double scaleByZNear)
        {
            if (_scale / _zNear == scaleByZNear)
                return this;

            var result = (PerspectiveCamera)this.MemberwiseClone();
            result._scale = scaleByZNear * _zNear;
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
        public Matrix4x3 GetLookAtRHTimesPerspectiveRHMatrix(double aspectRatio)
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
        public Matrix4x3 GetLookAtRHTimesPerspectiveRHMatrix(double aspectRatio, double zN, double zF)
        {
            var m = LookAtRHMatrix;
            double scaleX = _scale;
            double scaleY = _scale * aspectRatio;
            double zN_zF = zN - zF;
            return new Matrix4x3(
                    2 * m.M11 * zN / scaleX, 2 * m.M12 * zN / scaleY, m.M13 * zF / zN_zF,
                    2 * m.M21 * zN / scaleX, 2 * m.M22 * zN / scaleY, m.M23 * zF / zN_zF,
                    2 * m.M31 * zN / scaleX, 2 * m.M32 * zN / scaleY, m.M33 * zF / zN_zF,
                    2 * m.M41 * zN / scaleX, 2 * m.M42 * zN / scaleY, m.M43 * zF / zN_zF
                    );
        }

        /// <summary>
        /// Gets the transposed result of LookAtRH matrix multiplied with the OrthoRH matrix.
        /// </summary>
        /// <param name="aspectRatio">The aspect ratio of the screen (or whatever the 2D output medium is).</param>
        /// <returns>The LookAtRH matrix multiplied with the OrthoRH matrix.</returns>
        public override Matrix4x4 GetViewProjectionMatrix(double aspectRatio)
        {
            return GetViewProjectionMatrix(aspectRatio, ZNear, ZFar);
        }

        /// <summary>
        /// Gets the transposed result of LookAtRH matrix multiplied with the OrthoRH matrix.
        /// </summary>
        /// <param name="aspectRatio">The aspect ratio of the screen (or whatever the 2D output medium is).</param>
        /// <param name="zNearPlane">The z near plane, i.e. the z camera coordinate of the near end of the view volume.</param>
        /// <param name="zFarPlane">The z far plane, i.e. the z camera coordinate of the far end of the view volume.</param>
        /// <returns>The LookAtRH matrix multiplied with the OrthoRH matrix.</returns>
        public Matrix4x4 GetViewProjectionMatrix(double aspectRatio, double zNearPlane, double zFarPlane)
        {
            double scaleX = 2 * zNearPlane / _scale;
            double scaleY = 2 * zNearPlane / (_scale * aspectRatio);
            double scaleZ = zFarPlane / (zNearPlane - zFarPlane);

            var l = LookAtRHMatrix;
            return new Matrix4x4(
                     l.M11 * scaleX, l.M12 * scaleY, l.M13 * scaleZ, -l.M13,
                     l.M21 * scaleX, l.M22 * scaleY, l.M23 * scaleZ, -l.M23,
                     l.M31 * scaleX, l.M32 * scaleY, l.M33 * scaleZ, -l.M33,
                     l.M41 * scaleX, l.M42 * scaleY, (l.M43 + zNearPlane) * scaleZ, -l.M43
                    );
        }

        public override Matrix4x3 GetHitRayMatrix(PointD3D relativeScreenPosition)
        {
            throw new NotImplementedException();
        }
    }
}