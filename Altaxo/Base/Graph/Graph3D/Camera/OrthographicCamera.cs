﻿#region Copyright

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

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Geometry;

namespace Altaxo.Graph.Graph3D.Camera
{
  public class OrthographicCamera : CameraBase
  {
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
        info.AddValue("UpVector", s._upVector);
        info.AddValue("EyePosition", s._eyePosition);
        info.AddValue("TargetPosition", s._targetPosition);
        info.AddValue("ZNear", s._zNear);
        info.AddValue("ZFar", s._zFar);
        info.AddValue("Width", s._widthAtZNear);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (OrthographicCamera?)o ?? new OrthographicCamera();
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

    public OrthographicCamera()
            : base(new VectorD3D(0, 0, 1), new PointD3D(0, 0, -1500), new PointD3D(0, 0, 0), 150, 3000, 1000)
    {
    }

    public OrthographicCamera(VectorD3D upVector, PointD3D eyePosition, PointD3D targetPosition, double zNear, double zFar, double widthAtZNear)
            : base(upVector, eyePosition, targetPosition, zNear, zFar, widthAtZNear)
    {
    }

    #region Overrides

    /// <summary>
    /// Gets the width of the view field at target distance. For this orthographic camera, it is always the same as <see cref="CameraBase.WidthAtZNear"/>.
    /// </summary>
    /// <value>
    /// The width of the view field at target distance.
    /// </value>
    public override double WidthAtTargetDistance
    {
      get
      {
        return _widthAtZNear;
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

      var result = (OrthographicCamera)MemberwiseClone();
      result._zNear = zNear;
      result._zFar = zFar;
      return result;
    }

    #endregion Overrides

    /// <summary>
    /// Creates a new camera with provided  eyePosition and targetPosition;
    /// </summary>
    /// <param name="eyePosition">The eye position.</param>
    /// <param name="targetPosition">The target position.</param>
    /// <param name="widthAtZNear">The width of the view field at <see cref="CameraBase.ZNear"/>. (Since this is an orthographic camera, the width it constant for all distances, thus this is also the width for <see cref="CameraBase.ZFar"/>).</param>
    /// <returns>New camera with the provided parameters.</returns>
    public OrthographicCamera WithEyeTargetWidth(PointD3D eyePosition, PointD3D targetPosition, double widthAtZNear)
    {
      var result = (OrthographicCamera)MemberwiseClone();
      result._eyePosition = eyePosition;
      result._targetPosition = targetPosition;
      result._widthAtZNear = widthAtZNear;
      return result;
    }

    /// <summary>
    /// Gets the OrthoRH matrix. The OrthoRH matrix transforms the camera coordinates into the view volume coordinates (X=(-1..+1), Y=(-1..+1), Z=(0..1)).
    /// </summary>
    /// <param name="aspectRatio">The aspect ratio of the screen (or whatever the 2D output medium is).</param>
    /// <param name="zNearPlane">The z near plane, i.e. the z camera coordinate of the near end of the view volume.</param>
    /// <param name="zFarPlane">The z far plane, i.e. the z camera coordinate of the far end of the view volume.</param>
    /// <returns>The OrthoRH matrix.</returns>
    public Matrix4x3 GetOrthoRHMatrix(double aspectRatio, double zNearPlane, double zFarPlane)
    {
      return new Matrix4x3(
              2 / _widthAtZNear, 0, 0,
              0, 2 / (_widthAtZNear * aspectRatio), 0,
              0, 0, 1 / (zNearPlane - zFarPlane),
              ScreenOffset.X, ScreenOffset.Y, zNearPlane / (zNearPlane - zFarPlane)
              );
    }

    /// <summary>
    /// Gets the transposed result of LookAtRH matrix multiplied with the OrthoRH matrix.
    /// </summary>
    /// <param name="aspectRatio">The aspect ratio of the screen (or whatever the 2D output medium is).</param>
    /// <returns>The LookAtRH matrix multiplied with the OrthoRH matrix.</returns>
    public override Matrix4x4 GetViewProjectionMatrix(double aspectRatio)
    {
      return GetViewProjectionMatrix(aspectRatio, ZNear, ZFar, ScreenOffset.X, ScreenOffset.Y);
    }

    /// <summary>
    /// Gets the transposed result of LookAtRH matrix multiplied with the OrthoRH matrix.
    /// </summary>
    /// <param name="aspectRatio">The aspect ratio of the screen (or whatever the 2D output medium is).</param>
    /// <param name="zNearPlane">The z near plane, i.e. the z camera coordinate of the near end of the view volume.</param>
    /// <param name="zFarPlane">The z far plane, i.e. the z camera coordinate of the far end of the view volume.</param>
    /// <param name="screenOffsetX">The screen offsets x component.</param>
    /// <param name="screenOffsetY">The screen offsets y component.</param>
    /// <returns>The LookAtRH matrix multiplied with the OrthoRH matrix.</returns>
    public Matrix4x4 GetViewProjectionMatrix(double aspectRatio, double zNearPlane, double zFarPlane, double screenOffsetX, double screenOffsetY)
    {
      double scaleX = 2 / _widthAtZNear;
      double scaleY = 2 / (_widthAtZNear * aspectRatio);
      double scaleZ = 1 / (zNearPlane - zFarPlane);

      var l = LookAtRHMatrix;
      return new Matrix4x4(
               l.M11 * scaleX, l.M12 * scaleY, l.M13 * scaleZ, 0,
               l.M21 * scaleX, l.M22 * scaleY, l.M23 * scaleZ, 0,
               l.M31 * scaleX, l.M32 * scaleY, l.M33 * scaleZ, 0,
               l.M41 * scaleX + screenOffsetX, l.M42 * scaleY + screenOffsetY, (l.M43 + zNearPlane) * scaleZ, 1
              );
    }

    /// <summary>
    /// Gets a matrix for a hit point on the screen. The hit point is given in relative screen coordinates (X and Y component, 0..1). The screen's aspect ratio is given in the Z component.
    /// The result is a matrix which transforms world coordinates in that way that the hit ray in world coordinates is transformed to x=0 and y=0 and z being the distance to the camera.
    /// </summary>
    /// <param name="relativeScreenPosition">The relative screen position (X and Y component), as well as the screen's aspect ratio (Z component).</param>
    /// <returns>
    /// Matrix which transforms world coordinates in that way that the hit ray in world coordinates is transformed to x=0 and y=0 and z being the distance to the camera.
    /// </returns>
    public override Matrix4x4 GetHitRayMatrix(PointD3D relativeScreenPosition)
    {
      double relX = relativeScreenPosition.X * 2 - 1; // relative Screen Position from -1..1
      double relY = relativeScreenPosition.Y * 2 - 1; // relative Screen Position from -1..1
      return GetViewProjectionMatrix(relativeScreenPosition.Z, ZNear, ZFar, -relX, -relY);
    }
  }
}
