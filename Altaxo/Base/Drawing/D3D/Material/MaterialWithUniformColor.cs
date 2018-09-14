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

namespace Altaxo.Drawing.D3D.Material
{
  /// <summary>
  /// Represents a material which is uniformely colored.
  /// </summary>
  public class MaterialWithUniformColor : MaterialBase
  {
    /// <summary>
    /// The diffuse color of the material.
    /// </summary>
    private NamedColor _color;

    #region Serialization

    /// <summary>
    /// 2015-11-18 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(MaterialWithUniformColor), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (MaterialWithUniformColor)obj;

        info.AddValue("Smoothness", s._smoothness);
        info.AddValue("Metalness", s._metalness);
        info.AddValue("IndexOfRefraction", s._indexOfRefraction);
        info.AddValue("Color", s._color);
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        double smoothness = info.GetDouble("Smoothness");
        double metalness = info.GetDouble("Metalness");
        double indexOfRefraction = info.GetDouble("IndexOfRefraction");
        var color = (NamedColor)info.GetValue("Color", null);

        return new MaterialWithUniformColor(color, smoothness, metalness, indexOfRefraction);
      }
    }

    #endregion Serialization

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="MaterialWithUniformColor" /> class with the provided color and with default specular properties.
    /// </summary>
    /// <param name="color">The material color.</param>
    public MaterialWithUniformColor(NamedColor color)
    {
      _color = color;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MaterialWithUniformColor" /> class with the provided color and specular properties.
    /// </summary>
    /// <param name="color">The material color.</param>
    /// <param name="smoothness">The specular intensity.</param>
    /// <param name="metalness">The specular exponent.</param>
    public MaterialWithUniformColor(NamedColor color, double smoothness, double metalness)
          : base(smoothness, metalness)
    {
      _color = color;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MaterialWithUniformColor" /> class with the provided color and specular properties.
    /// </summary>
    /// <param name="color">The material color.</param>
    /// <param name="smoothness">The specular intensity.</param>
    /// <param name="metalness">The specular exponent.</param>
    /// <param name="indexOfRefraction">The index of refraction.</param>
    public MaterialWithUniformColor(NamedColor color, double smoothness, double metalness, double indexOfRefraction)
          : base(smoothness, metalness, indexOfRefraction)
    {
      _color = color;
    }

    #endregion Constructors

    #region Color

    ///<inheritdoc/>
    public override NamedColor Color
    {
      get
      {
        return _color;
      }
    }

    ///<inheritdoc/>
    public override IMaterial WithColor(NamedColor color)
    {
      if (!(color == _color))
      {
        var result = (MaterialWithUniformColor)MemberwiseClone();
        result._color = color;
        return result;
      }
      else
      {
        return this;
      }
    }

    #endregion Color

    #region Infrastructure

    ///<inheritdoc/>
    public override bool HasColor
    {
      get
      {
        return true;
      }
    }

    ///<inheritdoc/>
    public override bool HasTexture
    {
      get
      {
        return false;
      }
    }

    public override bool IsVisible
    {
      get
      {
        return true;
      }
    }

    ///<inheritdoc/>
    public override bool Equals(object obj)
    {
      // this material is considered to be equal to another material, if this material has exactly
      var other = obj as MaterialWithUniformColor;
      if (null != other)
      {
        return
          _color == other._color &&
          _smoothness == other._smoothness &&
          _metalness == other._metalness &&
          _indexOfRefraction == other._indexOfRefraction;
      }

      return false;
    }

    ///<inheritdoc/>
    public override bool Equals(IMaterial obj)
    {
      // this material is considered to be equal to another material, if this material has exactly
      var other = obj as MaterialWithUniformColor;
      if (null != other)
      {
        return
          _color == other._color &&

          _smoothness == other._smoothness &&
          _metalness == other._metalness &&
          _indexOfRefraction == other._indexOfRefraction;
      }

      return false;
    }

    public override int GetHashCode()
    {
      return _color.GetHashCode() + 3 * _smoothness.GetHashCode() + 7 * _metalness.GetHashCode() + 13 * _indexOfRefraction.GetHashCode();
    }

    #endregion Infrastructure
  }
}
