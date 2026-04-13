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

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Drawing.D3D.Material
{
  /// <summary>
  /// Material that has specular properties but no intrinsic color or texture.
  /// </summary>
  public class MaterialWithoutColorOrTexture : MaterialBase
  {
    #region Serialization

    /// <summary>
    /// 2015-11-18 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(MaterialWithoutColorOrTexture), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object o, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (MaterialWithoutColorOrTexture)o;

        info.AddValue("Smoothness", s._smoothness);
        info.AddValue("Metalness", s._metalness);
        info.AddValue("IndexOfRefraction", s._indexOfRefraction);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        double smoothness = info.GetDouble("Smoothness");
        double metalness = info.GetDouble("Metalness");
        double indexOfRefraction = info.GetDouble("IndexOfRefraction");

        return new MaterialWithoutColorOrTexture(smoothness, metalness, indexOfRefraction);
      }
    }

    #endregion Serialization

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="MaterialWithoutColorOrTexture"/> class with default specular properties.
    /// </summary>
    public MaterialWithoutColorOrTexture()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MaterialWithoutColorOrTexture"/> class.
    /// </summary>
    /// <param name="smoothness">The surface smoothness.</param>
    /// <param name="metalness">The surface metalness.</param>
    /// <param name="indexOfRefraction">The index of refraction.</param>
    public MaterialWithoutColorOrTexture(double smoothness, double metalness, double indexOfRefraction)
      : base(smoothness, metalness, indexOfRefraction)
    {
    }

    #endregion Constructors

    #region Color

    /// <inheritdoc/>
    public override NamedColor Color
    {
      get
      {
        return NamedColors.Black;
      }
    }

    /// <inheritdoc/>
    public override IMaterial WithColor(NamedColor color)
    {
      return this;
    }

    #endregion Color

    #region Infrastructure

    /// <inheritdoc/>
    public override bool HasColor
    {
      get
      {
        return false;
      }
    }

    /// <inheritdoc/>
    public override bool HasTexture
    {
      get
      {
        return false;
      }
    }

    /// <inheritdoc/>
    public override bool IsVisible
    {
      get
      {
        return true;
      }
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
      // this material is considered to be equal to another material, if this material has exactly
      var other = obj as MaterialWithoutColorOrTexture;
      if (other is not null)
      {
        return

          _smoothness == other._smoothness &&
          _metalness == other._metalness &&
          _indexOfRefraction == other._indexOfRefraction;
      }

      return false;
    }

    /// <inheritdoc/>
    public override bool Equals(IMaterial? obj)
    {
      // this material is considered to be equal to another material, if this material has exactly
      var other = obj as MaterialWithoutColorOrTexture;
      if (other is not null)
      {
        return
          _smoothness == other._smoothness &&
          _metalness == other._metalness &&
          _indexOfRefraction == other._indexOfRefraction;
      }

      return false;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
      return 3 * _smoothness.GetHashCode() + 7 * _metalness.GetHashCode() + 13 * _indexOfRefraction.GetHashCode();
    }

    #endregion Infrastructure
  }
}
