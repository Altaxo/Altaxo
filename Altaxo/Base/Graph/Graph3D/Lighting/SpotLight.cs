#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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

using Altaxo.Drawing;
using Altaxo.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Graph3D.Lighting
{
  /// <summary>
  /// Represents directional lighting. All light rays are parallel to each other.
  /// </summary>
  public class SpotLight : IDiscreteLight
  {
    private bool _isAffixedToCamera;
    private double _lightAmplitude;
    private NamedColor _color;
    private PointD3D _position;
    private VectorD3D _directionToLight;
    private double _range;
    private double _outerConeAngle;
    private double _innerConeAngle;

    #region Serialization

    /// <summary>
    /// Deserialization constructor.
    /// </summary>
    /// <param name="info">Not used.</param>
    private SpotLight(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    {
    }

    /// <summary>
    /// 2016-01-24 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(SpotLight), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (SpotLight)obj;
        info.AddValue("IsAffixedToCamera", s._isAffixedToCamera);
        info.AddValue("LightAmplitude", s._lightAmplitude);
        info.AddValue("Color", s._color);
        info.AddValue("Position", s._position);
        info.AddValue("DirectionToLight", s._directionToLight);
        info.AddValue("Range", s._range);
        info.AddValue("OuterConeAngle", s._outerConeAngle);
        info.AddValue("InnerConeAngle", s._innerConeAngle);
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        var s = (SpotLight)o ?? new SpotLight(info);
        s._isAffixedToCamera = info.GetBoolean("IsAffixedToCamera");
        s._lightAmplitude = info.GetDouble("LightAmplitude");
        s._color = (NamedColor)info.GetValue("Color", s);
        s._position = (PointD3D)info.GetValue("Position", s);
        s._directionToLight = (VectorD3D)info.GetValue("DirectionToLight", s);
        s._range = info.GetDouble("Range");
        s._outerConeAngle = info.GetDouble("OuterConeAngle");
        s._innerConeAngle = info.GetDouble("InnerConeAngle");
        return s;
      }
    }

    #endregion Serialization

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="SpotLight"/> class with default values.
    /// </summary>
    public SpotLight()
    {
      _lightAmplitude = 1;
      _color = NamedColors.White;
      _range = 1;
      _directionToLight = new VectorD3D(0, 0, -1);
      _outerConeAngle = Math.PI / 2;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SpotLight"/> class.
    /// </summary>
    /// <param name="lightAmplitude">The light amplitude.</param>
    /// <param name="color">The color of light.</param>
    /// <param name="position">The position of the light.</param>
    /// <param name="directionToLight">The direction from the scene to the light.</param>
    /// <param name="range">The range of the light.</param>
    /// <param name="outerConeAngle">The outer cone angle in radians.</param>
    /// <param name="innerConeAngle">The inner cone angle in radians.</param>
    /// <param name="isAffixedToCamera">Value indicating whether the light source is affixed to the camera coordinate system or the world coordinate system.</param>
    public SpotLight(double lightAmplitude, NamedColor color, PointD3D position, VectorD3D directionToLight, double range, double outerConeAngle, double innerConeAngle, bool isAffixedToCamera)
    {
      _isAffixedToCamera = isAffixedToCamera;

      VerifyLightAmplitude(lightAmplitude, nameof(lightAmplitude));
      _lightAmplitude = lightAmplitude;

      _color = color;

      VerifyPosition(position, nameof(position));
      _position = position;

      var dlen = VerifyDirection(directionToLight, nameof(directionToLight));
      _directionToLight = directionToLight / dlen;

      VerifyRange(range, nameof(range));
      _range = range;

      VerifyAngle(outerConeAngle, nameof(outerConeAngle));
      _outerConeAngle = outerConeAngle;

      VerifyAngle(innerConeAngle, nameof(innerConeAngle));
      _innerConeAngle = innerConeAngle;
    }

    #endregion Constructors

    #region IsAffixedToCamera

    /// <summary>
    /// Gets a value indicating whether this light source is affixed to the camera coordinate system or to the world coordinate system.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is affixed to the camera coordinate system; <c>false</c> if this instance is affixed to the world coordinate system.
    /// </value>
    public bool IsAffixedToCamera { get { return _isAffixedToCamera; } }

    /// <summary>
    /// Gets a new instance of <see cref="SpotLight"/> with the provided value for <see cref="IsAffixedToCamera"/>.
    /// </summary>
    /// <param name="isAffixedToCamera">The new value for <see cref="IsAffixedToCamera"/>.</param>
    /// <returns>New instance of <see cref="SpotLight"/> with the provided value for <see cref="IsAffixedToCamera"/></returns>
    public SpotLight WithValueAffixedToCamera(bool isAffixedToCamera)
    {
      if (!(isAffixedToCamera == _isAffixedToCamera))
      {
        var result = (SpotLight)this.MemberwiseClone();
        result._isAffixedToCamera = isAffixedToCamera;
        return result;
      }
      else
      {
        return this;
      }
    }

    #endregion IsAffixedToCamera

    #region LightAmpltitude

    /// <summary>
    /// Gets the light amplitude. The default value is 1. This value is multiplied with the light <see cref="Color"/> to get the effective light's color.
    /// </summary>
    /// <value>
    /// The light amplitude.
    /// </value>
    public double LightAmplitude { get { return _lightAmplitude; } }

    /// <summary>
    /// Gets a new instance of <see cref="SpotLight"/> with the provided value for <see cref="LightAmplitude"/>.
    /// </summary>
    /// <param name="lightAmplitude">The new value for <see cref="LightAmplitude"/>.</param>
    /// <returns>New instance of <see cref="SpotLight"/> with the provided value for <see cref="LightAmplitude"/></returns>
    public SpotLight WithLightAmplitude(double lightAmplitude)
    {
      if (!(lightAmplitude == _lightAmplitude))
      {
        VerifyLightAmplitude(lightAmplitude, nameof(lightAmplitude));

        var result = (SpotLight)this.MemberwiseClone();
        result._lightAmplitude = lightAmplitude;
        return result;
      }
      else
      {
        return this;
      }
    }

    /// <summary>
    /// Verifies the light amplitude.
    /// </summary>
    /// <param name="value">The light amplitude value.</param>
    /// <param name="valueName">The paramter name.</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private void VerifyLightAmplitude(double value, string valueName)
    {
      if (!(value >= 0))
        throw new ArgumentOutOfRangeException(string.Format("{0} is expected to be >= 0", valueName));
    }

    #endregion LightAmpltitude

    #region Color

    /// <summary>
    /// Gets the color of the light.
    /// </summary>
    public NamedColor Color { get { return _color; } }

    /// <summary>
    /// Gets a new instance of <see cref="SpotLight"/> with the provided value for <see cref="Color"/>.
    /// </summary>
    /// <param name="color">The new value for <see cref="Color"/>.</param>
    /// <returns>New instance of <see cref="SpotLight"/> with the provided value for <see cref="Color"/></returns>
    public SpotLight WithColor(NamedColor color)
    {
      if (!(color == _color))
      {
        var result = (SpotLight)this.MemberwiseClone();
        result._color = color;
        return result;
      }
      else
      {
        return this;
      }
    }

    #endregion Color

    #region Position

    /// <summary>
    /// Gets the direction from the light to the scene
    /// </summary>
    public PointD3D Position { get { return _position; } }

    /// <summary>
    /// Gets a new instance of <see cref="SpotLight"/> with the provided value for <see cref="Position"/>.
    /// </summary>
    /// <param name="position">The new value for <see cref="Position"/>.</param>
    /// <returns>New instance of <see cref="SpotLight"/> with the provided value for <see cref="Position"/></returns>
    public SpotLight WithPosition(PointD3D position)
    {
      if (!(position == _position))
      {
        VerifyPosition(position, nameof(position));

        var result = (SpotLight)this.MemberwiseClone();
        result._position = position;
        return result;
      }
      else
      {
        return this;
      }
    }

    /// <summary>
    /// Verifies the validity of the position value.
    /// </summary>
    /// <param name="value">The position of the light.</param>
    /// <param name="valueName">The name of the parameter.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// </exception>
    private void VerifyPosition(PointD3D value, string valueName)
    {
      if (value.IsNaN)
        throw new ArgumentOutOfRangeException(valueName + " is a struct with invalid elements");
    }

    #endregion Position

    #region Direction

    /// <summary>
    /// Gets the direction from the scene to the light.
    /// </summary>
    public VectorD3D DirectionToLight { get { return _directionToLight; } }

    /// <summary>
    /// Gets a new instance of <see cref="SpotLight"/> with the provided value for <see cref="DirectionToLight"/>.
    /// </summary>
    /// <param name="directionToLight">The new value for <see cref="DirectionToLight"/>.</param>
    /// <returns>New instance of <see cref="SpotLight"/> with the provided value for <see cref="DirectionToLight"/></returns>
    public SpotLight WithDirectionToLight(VectorD3D directionToLight)
    {
      if (!(directionToLight == _directionToLight))
      {
        var len = VerifyDirection(directionToLight, nameof(directionToLight));

        var result = (SpotLight)this.MemberwiseClone();
        result._directionToLight = directionToLight / len;
        return result;
      }
      else
      {
        return this;
      }
    }

    /// <summary>
    /// Verifies the validity of the vector and returns the vector length.
    /// </summary>
    /// <param name="value">The direction from light to scene.</param>
    /// <param name="valueName">The name of the parameter.</param>
    /// <returns>Vector length.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// </exception>
    private double VerifyDirection(VectorD3D value, string valueName)
    {
      if (value == VectorD3D.Empty)
        throw new ArgumentOutOfRangeException(valueName + " must not be an empty vector");
      var len = value.Length;
      if (!(len >= 0))
        throw new ArgumentOutOfRangeException(valueName + " is a vector with invalid elements");

      return len;
    }

    #endregion Direction

    #region Range

    /// <summary>
    /// Gets the range of this light source (in world coordinate units).
    /// </summary>
    /// <value>
    /// The range of this light source.
    /// </value>
    public double Range { get { return _range; } }

    /// <summary>
    /// Gets a new instance of <see cref="SpotLight"/> with the provided value for <see cref="Range"/>.
    /// </summary>
    /// <param name="range">The new value for <see cref="Range"/>.</param>
    /// <returns>New instance of <see cref="SpotLight"/> with the provided value for <see cref="Range"/></returns>
    public SpotLight WithRange(double range)
    {
      if (!(range == _range))
      {
        VerifyRange(range, nameof(range));

        var result = (SpotLight)this.MemberwiseClone();
        result._range = range;
        return result;
      }
      else
      {
        return this;
      }
    }

    /// <summary>
    /// Verifies the validity of the range value.
    /// </summary>
    /// <param name="value">The light range.</param>
    /// <param name="valueName">The name of the parameter.</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private void VerifyRange(double value, string valueName)
    {
      if (!(value > 0))
        throw new ArgumentOutOfRangeException(string.Format("{0} is expected to be > 0", valueName));
    }

    #endregion Range

    #region OuterConeAngle

    /// <summary>
    /// Gets the outer cone angle (in radians).
    /// </summary>
    /// <value>
    /// The outer cone angle in radians.
    /// </value>
    public double OuterConeAngle { get { return _outerConeAngle; } }

    /// <summary>
    /// Gets a new instance of <see cref="SpotLight"/> with the provided value for <see cref="OuterConeAngle"/>.
    /// </summary>
    /// <param name="outerAngle">The new value for <see cref="OuterConeAngle"/> in radians.</param>
    /// <returns>New instance of <see cref="SpotLight"/> with the provided value for <see cref="OuterConeAngle"/></returns>
    public SpotLight WithOuterConeAngle(double outerAngle)
    {
      if (!(outerAngle == _outerConeAngle))
      {
        VerifyAngle(outerAngle, nameof(outerAngle));

        var result = (SpotLight)this.MemberwiseClone();
        result._outerConeAngle = outerAngle;
        return result;
      }
      else
      {
        return this;
      }
    }

    /// <summary>
    /// Verifies the value of the cone angle.
    /// </summary>
    /// <param name="value">The cone angle in radians.</param>
    /// <param name="valueName">The name of the parameter.</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private void VerifyAngle(double value, string valueName)
    {
      if (!(value >= 0))
        throw new ArgumentOutOfRangeException(string.Format("{0} is expected to be >= 0", valueName));
    }

    #endregion OuterConeAngle

    #region InnerConeAngle

    /// <summary>
    /// Gets the inner cone angle (in radians).
    /// </summary>
    /// <value>
    /// The inner cone angle in radians.
    /// </value>
    public double InnerConeAngle { get { return _innerConeAngle; } }

    /// <summary>
    /// Gets a new instance of <see cref="SpotLight"/> with the provided value for <see cref="InnerConeAngle"/>.
    /// </summary>
    /// <param name="innerAngle">The new value for <see cref="InnerConeAngle"/> in radians.</param>
    /// <returns>New instance of <see cref="SpotLight"/> with the provided value for <see cref="InnerConeAngle"/></returns>
    public SpotLight WithInnerConeAngle(double innerAngle)
    {
      if (!(innerAngle == _innerConeAngle))
      {
        VerifyAngle(innerAngle, nameof(innerAngle));

        var result = (SpotLight)this.MemberwiseClone();
        result._innerConeAngle = innerAngle;
        return result;
      }
      else
      {
        return this;
      }
    }

    #endregion InnerConeAngle
  }
}
