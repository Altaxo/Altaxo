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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altaxo.Drawing;
using Altaxo.Geometry;

namespace Altaxo.Graph.Graph3D.Lighting
{
  /// <summary>
  /// Represents directional lighting. All light rays are parallel to each other.
  /// </summary>
  public class DirectionalLight : IDiscreteLight
  {
    private bool _isAffixedToCamera;
    private double _lightAmplitude;
    private NamedColor _color;
    private VectorD3D _directionToLight;

    #region Serialization

    /// <summary>
    /// Deserialization constructor.
    /// </summary>
    /// <param name="info">Not used.</param>
    private DirectionalLight(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    {
    }

    /// <summary>
    /// 2016-01-24 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DirectionalLight), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (DirectionalLight)obj;
        info.AddValue("IsAffixedToCamera", s._isAffixedToCamera);
        info.AddValue("LightAmplitude", s._lightAmplitude);
        info.AddValue("Color", s._color);
        info.AddValue("DirectionToLight", s._directionToLight);
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        var s = (DirectionalLight)o ?? new DirectionalLight(info);
        s._isAffixedToCamera = info.GetBoolean("IsAffixedToCamera");
        s._lightAmplitude = info.GetDouble("LightAmplitude");
        s._color = (NamedColor)info.GetValue("Color", s);
        s._directionToLight = (VectorD3D)info.GetValue("DirectionToLight", s);
        return s;
      }
    }

    #endregion Serialization

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="DirectionalLight"/> class with default values.
    /// </summary>
    public DirectionalLight()
    {
      _lightAmplitude = 1;
      _color = NamedColors.White;
      _directionToLight = new VectorD3D(-Math.Sqrt(0.25), -Math.Sqrt(0.25), Math.Sqrt(0.5));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DirectionalLight"/> class.
    /// </summary>
    /// <param name="lightAmplitude">The light amplitude.</param>
    /// <param name="color">The color of light.</param>
    /// <param name="directionToLight">The direction from the scene to the light.</param>
    /// <param name="isAffixedToCamera">Value indicating whether the light source is affixed to the camera coordinate system or the world coordinate system.</param>
    public DirectionalLight(double lightAmplitude, NamedColor color, VectorD3D directionToLight, bool isAffixedToCamera)
    {
      _isAffixedToCamera = isAffixedToCamera;

      VerifyLightAmplitude(lightAmplitude, nameof(lightAmplitude));
      _lightAmplitude = lightAmplitude;

      _color = color;

      var len = VerifyDirection(directionToLight, nameof(directionToLight));
      _directionToLight = directionToLight / len;
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
    /// Gets a new instance of <see cref="DirectionalLight"/> with the provided value for <see cref="IsAffixedToCamera"/>.
    /// </summary>
    /// <param name="isAffixedToCamera">The new value for <see cref="IsAffixedToCamera"/>.</param>
    /// <returns>New instance of <see cref="DirectionalLight"/> with the provided value for <see cref="IsAffixedToCamera"/></returns>
    public DirectionalLight WithValueAffixedToCamera(bool isAffixedToCamera)
    {
      if (!(isAffixedToCamera == _isAffixedToCamera))
      {
        var result = (DirectionalLight)MemberwiseClone();
        result._isAffixedToCamera = isAffixedToCamera;
        return result;
      }
      else
      {
        return this;
      }
    }

    #endregion IsAffixedToCamera

    #region LightAmplitude

    /// <summary>
    /// Gets the light amplitude. The default value is 1. This value is multiplied with the light <see cref="Color"/> to get the effective light's color.
    /// </summary>
    /// <value>
    /// The light amplitude.
    /// </value>
    public double LightAmplitude { get { return _lightAmplitude; } }

    /// <summary>
    /// Gets a new instance of <see cref="DirectionalLight"/> with the provided value for <see cref="LightAmplitude"/>.
    /// </summary>
    /// <param name="lightAmplitude">The new value for <see cref="LightAmplitude"/>.</param>
    /// <returns>New instance of <see cref="DirectionalLight"/> with the provided value for <see cref="LightAmplitude"/></returns>
    public DirectionalLight WithLightAmplitude(double lightAmplitude)
    {
      if (!(lightAmplitude == _lightAmplitude))
      {
        VerifyLightAmplitude(lightAmplitude, nameof(lightAmplitude));

        var result = (DirectionalLight)MemberwiseClone();
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

    #endregion LightAmplitude

    #region Color

    /// <summary>
    /// Gets the color of the light.
    /// </summary>
    public NamedColor Color { get { return _color; } }

    /// <summary>
    /// Gets a new instance of <see cref="DirectionalLight"/> with the provided value for <see cref="Color"/>.
    /// </summary>
    /// <param name="color">The new value for <see cref="Color"/>.</param>
    /// <returns>New instance of <see cref="DirectionalLight"/> with the provided value for <see cref="Color"/></returns>
    public DirectionalLight WithColor(NamedColor color)
    {
      if (!(color == _color))
      {
        var result = (DirectionalLight)MemberwiseClone();
        result._color = color;
        return result;
      }
      else
      {
        return this;
      }
    }

    #endregion Color

    #region Direction

    /// <summary>
    /// Gets the direction from the scene to the light.
    /// </summary>
    public VectorD3D DirectionToLight { get { return _directionToLight; } }

    /// <summary>
    /// Gets a new instance of <see cref="DirectionalLight"/> with the provided value for <see cref="DirectionToLight"/>.
    /// </summary>
    /// <param name="directionToLight">The new value for <see cref="DirectionToLight"/>.</param>
    /// <returns>New instance of <see cref="DirectionalLight"/> with the provided value for <see cref="DirectionToLight"/></returns>
    public DirectionalLight WithDirectionToLight(VectorD3D directionToLight)
    {
      if (!(directionToLight == _directionToLight))
      {
        var len = VerifyDirection(directionToLight, nameof(directionToLight));

        var result = (DirectionalLight)MemberwiseClone();
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
  }
}
