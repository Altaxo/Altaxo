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
  /// Immutable class that represents hemispheric ambient lighting. The color of a face is determined by its normal. If the normal points in the direction of the member <see cref="DirectionBelowToAbove"/>, the color is <see cref="ColorAbove"/>.
  /// If the normal points in the opposite direction, the color is <see cref="ColorBelow"/>. The color for all other directions is an interpolated value between <see cref="ColorBelow"/> and <see cref="ColorAbove"/>.
  /// </summary>
  /// <remarks>
  /// The interpolation value r (0..1) between ColorBelow and ColorAbove is determined by r = 0.5*(1+dotproduct(facenormal, directionBelowToAbove)).
  /// </remarks>
  public class HemisphericAmbientLight : Main.IImmutable
  {
    private bool _isAffixedToCamera;
    private double _lightAmplitude;
    private NamedColor _colorBelow;
    private NamedColor _colorAbove;
    private VectorD3D _directionBelowToAbove;

    #region Serialization

    /// <summary>
    /// Deserialization constructor.
    /// </summary>
    /// <param name="info">Not used.</param>
    private HemisphericAmbientLight(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    {
    }

    /// <summary>
    /// 2016-01-24 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(HemisphericAmbientLight), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (HemisphericAmbientLight)obj;
        info.AddValue("IsAffixedToCamera", s._isAffixedToCamera);
        info.AddValue("LightAmplitude", s._lightAmplitude);
        info.AddValue("ColorBelow", s._colorBelow);
        info.AddValue("ColorAbove", s._colorAbove);
        info.AddValue("DirectionBelowToAbove", s._directionBelowToAbove);
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        var s = (HemisphericAmbientLight)o ?? new HemisphericAmbientLight(info);
        s._isAffixedToCamera = info.GetBoolean("IsAffixedToCamera");
        s._lightAmplitude = info.GetDouble("LightAmplitude");
        s._colorBelow = (NamedColor)info.GetValue("ColorBelow", s);
        s._colorAbove = (NamedColor)info.GetValue("ColorAbove", s);
        s._directionBelowToAbove = (VectorD3D)info.GetValue("DirectionBelowToAbove", s);
        return s;
      }
    }

    #endregion Serialization

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="HemisphericAmbientLight"/> class with default values.
    /// </summary>
    public HemisphericAmbientLight()
    {
      _colorBelow = NamedColors.White;
      _colorAbove = NamedColors.White;
      _lightAmplitude = 1;
      _directionBelowToAbove = new VectorD3D(0, 0, 1);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HemisphericAmbientLight"/> class.
    /// </summary>
    /// <param name="lightAmplitude">The light amplitude.</param>
    /// <param name="colorBelow">The color below.</param>
    /// <param name="colorAbove">The color above.</param>
    /// <param name="directionBelowToAbove">The direction from "below" to "above".</param>
    /// <param name="isAffixedToCamera">Value indicating whether the light source is affixed to the camera coordinate system or the world coordinate system.</param>
    public HemisphericAmbientLight(double lightAmplitude, NamedColor colorBelow, NamedColor colorAbove, VectorD3D directionBelowToAbove, bool isAffixedToCamera)
    {
      VerifyLightAmplitude(lightAmplitude, nameof(lightAmplitude));
      _lightAmplitude = lightAmplitude;

      var len = VerifyDirectionBelowToAbove(directionBelowToAbove, nameof(directionBelowToAbove));
      _directionBelowToAbove = directionBelowToAbove / len;

      _colorBelow = colorBelow;
      _colorAbove = colorAbove;

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
    /// Gets a new instance of <see cref="HemisphericAmbientLight"/> with the provided value for <see cref="IsAffixedToCamera"/>.
    /// </summary>
    /// <param name="isAffixedToCamera">The new value for <see cref="IsAffixedToCamera"/>.</param>
    /// <returns>New instance of <see cref="HemisphericAmbientLight"/> with the provided value for <see cref="IsAffixedToCamera"/></returns>
    public HemisphericAmbientLight WithValueAffixedToCamera(bool isAffixedToCamera)
    {
      if (!(isAffixedToCamera == _isAffixedToCamera))
      {
        var result = (HemisphericAmbientLight)MemberwiseClone();
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
    /// Gets the light amplitude. The default value is 1. This value is multiplied with the light colors (<see cref="ColorBelow"/> and <see cref="ColorAbove"/>) to get the effective light's color.
    /// </summary>
    /// <value>
    /// The light amplitude.
    /// </value>
    public double LightAmplitude { get { return _lightAmplitude; } }

    /// <summary>
    /// Gets a new instance of <see cref="HemisphericAmbientLight"/> with the provided value for <see cref="LightAmplitude"/>.
    /// </summary>
    /// <param name="lightAmplitude">The new value for <see cref="LightAmplitude"/>.</param>
    /// <returns>New instance of <see cref="HemisphericAmbientLight"/> with the provided value for <see cref="LightAmplitude"/></returns>
    public HemisphericAmbientLight WithLightAmplitude(double lightAmplitude)
    {
      if (!(lightAmplitude == _lightAmplitude))
      {
        VerifyLightAmplitude(lightAmplitude, nameof(lightAmplitude));

        var result = (HemisphericAmbientLight)MemberwiseClone();
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

    #region ColorBelow

    /// <summary>
    /// Gets the color of the face if its normal vector points in the direction opposite to <see cref="DirectionBelowToAbove"/>.
    /// </summary>
    public NamedColor ColorBelow { get { return _colorBelow; } }

    /// <summary>
    /// Gets a new instance of <see cref="HemisphericAmbientLight"/> with the provided value for <see cref="ColorBelow"/>.
    /// </summary>
    /// <param name="colorBelow">The new value for <see cref="ColorBelow"/>.</param>
    /// <returns>New instance of <see cref="HemisphericAmbientLight"/> with the provided value for <see cref="ColorBelow"/></returns>
    public HemisphericAmbientLight WithColorBelow(NamedColor colorBelow)
    {
      if (!(colorBelow == _colorBelow))
      {
        var result = (HemisphericAmbientLight)MemberwiseClone();
        result._colorBelow = colorBelow;
        return result;
      }
      else
      {
        return this;
      }
    }

    #endregion ColorBelow

    #region ColorAbove

    /// <summary>
    /// Gets the color of the face if its normal vector points in the same direction as <see cref="DirectionBelowToAbove"/>.
    /// </summary>
    public NamedColor ColorAbove { get { return _colorAbove; } }

    /// <summary>
    /// Gets a new instance of <see cref="HemisphericAmbientLight"/> with the provided value for <see cref="ColorAbove"/>.
    /// </summary>
    /// <param name="colorAbove">The new value for <see cref="ColorAbove"/>.</param>
    /// <returns>New instance of <see cref="HemisphericAmbientLight"/> with the provided value for <see cref="ColorAbove"/></returns>
    public HemisphericAmbientLight WithColorAbove(NamedColor colorAbove)
    {
      if (!(colorAbove == _colorAbove))
      {
        var result = (HemisphericAmbientLight)MemberwiseClone();
        result._colorAbove = colorAbove;
        return result;
      }
      else
      {
        return this;
      }
    }

    #endregion ColorAbove

    #region DirectionBelowToAbove

    /// <summary>
    /// Gets the direction from "below" to "above".
    /// </summary>
    public VectorD3D DirectionBelowToAbove { get { return _directionBelowToAbove; } }

    /// <summary>
    /// Gets a new instance of <see cref="HemisphericAmbientLight"/> with the provided value for <see cref="DirectionBelowToAbove"/>.
    /// </summary>
    /// <param name="directionBelowToAbove">The new value for <see cref="DirectionBelowToAbove"/>.</param>
    /// <returns>New instance of <see cref="HemisphericAmbientLight"/> with the provided value for <see cref="DirectionBelowToAbove"/></returns>
    public HemisphericAmbientLight WithDirectionBelowToAbove(VectorD3D directionBelowToAbove)
    {
      if (!(directionBelowToAbove == _directionBelowToAbove))
      {
        var len = VerifyDirectionBelowToAbove(directionBelowToAbove, nameof(directionBelowToAbove));

        var result = (HemisphericAmbientLight)MemberwiseClone();
        result._directionBelowToAbove = directionBelowToAbove / len;
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
    /// <param name="directionBelowToAbove">The direction below to above.</param>
    /// <param name="valueName">The name of the parameter.</param>
    /// <returns>Vector length.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// </exception>
    private double VerifyDirectionBelowToAbove(VectorD3D directionBelowToAbove, string valueName)
    {
      if (directionBelowToAbove == VectorD3D.Empty)
        throw new ArgumentOutOfRangeException(valueName + " must not be an empty vector");
      var len = directionBelowToAbove.Length;
      if (!(len >= 0))
        throw new ArgumentOutOfRangeException(valueName + " is a vector with invalid elements");

      return len;
    }

    #endregion DirectionBelowToAbove
  }
}
