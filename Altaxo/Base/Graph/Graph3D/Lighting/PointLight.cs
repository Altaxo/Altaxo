﻿#region Copyright

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

#nullable enable
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
  public class PointLight : IDiscreteLight
  {
    private bool _isAffixedToCamera;
    private double _lightAmplitude;
    private NamedColor _color;
    private PointD3D _position;
    private double _range;

    #region Serialization

    /// <summary>
    /// Deserialization constructor.
    /// </summary>
    /// <param name="info">Not used.</param>
    private PointLight(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    {
    }

    /// <summary>
    /// 2016-01-24 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PointLight), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PointLight)obj;
        info.AddValue("IsAffixedToCamera", s._isAffixedToCamera);
        info.AddValue("LightAmplitude", s._lightAmplitude);
        info.AddValue("Color", s._color);
        info.AddValue("Position", s._position);
        info.AddValue("Range", s._range);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (PointLight?)o ?? new PointLight(info);
        s._isAffixedToCamera = info.GetBoolean("IsAffixedToCamera");
        s._lightAmplitude = info.GetDouble("LightAmplitude");
        s._color = (NamedColor)info.GetValue("Color", s);
        s._position = (PointD3D)info.GetValue("Position", s);
        s._range = info.GetDouble("Range");
        return s;
      }
    }

    #endregion Serialization

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="PointLight"/> class with default values.
    /// </summary>
    public PointLight()
    {
      _lightAmplitude = 1;
      _color = NamedColors.White;
      _range = 1;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PointLight"/> class.
    /// </summary>
    /// <param name="lightAmplitude">The light amplitude.</param>
    /// <param name="color">The color of light.</param>
    /// <param name="position">The position of the light.</param>
    /// <param name="range">The range of the light.</param>
    /// <param name="isAffixedToCamera">Value indicating whether the light source is affixed to the camera coordinate system or the world coordinate system.</param>
    public PointLight(double lightAmplitude, NamedColor color, PointD3D position, double range, bool isAffixedToCamera)
    {
      VerifyLightAmplitude(lightAmplitude, nameof(lightAmplitude));
      _lightAmplitude = lightAmplitude;

      VerifyPosition(position, nameof(position));
      _position = position;

      VerifyRange(range, nameof(range));
      _range = range;

      _color = color;
      _isAffixedToCamera = isAffixedToCamera;
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
    /// Gets a new instance of <see cref="PointLight"/> with the provided value for <see cref="IsAffixedToCamera"/>.
    /// </summary>
    /// <param name="isAffixedToCamera">The new value for <see cref="IsAffixedToCamera"/>.</param>
    /// <returns>New instance of <see cref="PointLight"/> with the provided value for <see cref="IsAffixedToCamera"/></returns>
    public PointLight WithValueAffixedToCamera(bool isAffixedToCamera)
    {
      if (!(isAffixedToCamera == _isAffixedToCamera))
      {
        var result = (PointLight)MemberwiseClone();
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
    /// Gets a new instance of <see cref="PointLight"/> with the provided value for <see cref="LightAmplitude"/>.
    /// </summary>
    /// <param name="lightAmplitude">The new value for <see cref="LightAmplitude"/>.</param>
    /// <returns>New instance of <see cref="PointLight"/> with the provided value for <see cref="LightAmplitude"/></returns>
    public PointLight WithLightAmplitude(double lightAmplitude)
    {
      if (!(lightAmplitude == _lightAmplitude))
      {
        VerifyLightAmplitude(lightAmplitude, nameof(lightAmplitude));

        var result = (PointLight)MemberwiseClone();
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
    /// Gets a new instance of <see cref="PointLight"/> with the provided value for <see cref="Color"/>.
    /// </summary>
    /// <param name="color">The new value for <see cref="Color"/>.</param>
    /// <returns>New instance of <see cref="PointLight"/> with the provided value for <see cref="Color"/></returns>
    public PointLight WithColor(NamedColor color)
    {
      if (!(color == _color))
      {
        var result = (PointLight)MemberwiseClone();
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
    /// Gets a new instance of <see cref="PointLight"/> with the provided value for <see cref="Position"/>.
    /// </summary>
    /// <param name="position">The new value for <see cref="Position"/>.</param>
    /// <returns>New instance of <see cref="PointLight"/> with the provided value for <see cref="Position"/></returns>
    public PointLight WithPosition(PointD3D position)
    {
      if (!(position == _position))
      {
        VerifyPosition(position, nameof(position));

        var result = (PointLight)MemberwiseClone();
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

    #region Range

    /// <summary>
    /// Gets the range of this light source (in world coordinate units).
    /// </summary>
    /// <value>
    /// The range of this light source.
    /// </value>
    public double Range { get { return _range; } }

    /// <summary>
    /// Gets a new instance of <see cref="PointLight"/> with the provided value for <see cref="Range"/>.
    /// </summary>
    /// <param name="range">The new value for <see cref="Range"/>.</param>
    /// <returns>New instance of <see cref="PointLight"/> with the provided value for <see cref="Range"/></returns>
    public PointLight WithRange(double range)
    {
      if (!(range == _range))
      {
        VerifyRange(range, nameof(range));

        var result = (PointLight)MemberwiseClone();
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
  }
}
