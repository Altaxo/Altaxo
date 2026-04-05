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
  /// Invisible placeholder material used when no material should be rendered.
  /// </summary>
  public class MaterialInvisible : IMaterial
  {
    /// <summary>
    /// Gets the singleton instance.
    /// </summary>
    public static MaterialInvisible Instance { get; private set; } = new MaterialInvisible();

    #region Serialization

    /// <summary>
    /// 2016-03-29 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(MaterialInvisible), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return MaterialInvisible.Instance;
      }
    }

    #endregion Serialization

    private MaterialInvisible()
    {
    }

    /// <inheritdoc/>
    public NamedColor Color
    {
      get
      {
        return NamedColors.Transparent;
      }
    }

    /// <inheritdoc/>
    public bool HasColor
    {
      get
      {
        return false;
      }
    }

    /// <inheritdoc/>
    public bool HasTexture
    {
      get
      {
        return false;
      }
    }

    /// <inheritdoc/>
    public bool IsVisible
    {
      get
      {
        return false;
      }
    }

    /// <inheritdoc/>
    public double Smoothness
    {
      get
      {
        return 0;
      }
    }

    /// <inheritdoc/>
    public double Metalness
    {
      get
      {
        return 0;
      }
    }

    /// <inheritdoc/>
    public double IndexOfRefraction
    {
      get
      {
        return 1;
      }
    }

    /// <inheritdoc/>
    public double PhongModelSpecularIntensity
    {
      get
      {
        return 0;
      }
    }

    /// <inheritdoc/>
    public double PhongModelDiffuseIntensity
    {
      get
      {
        return 0;
      }
    }

    /// <inheritdoc/>
    public double PhongModelSpecularExponent
    {
      get
      {
        return 1;
      }
    }

    /// <inheritdoc/>
    public bool Equals(IMaterial? other)
    {
      return (other is MaterialInvisible);
    }

    /// <inheritdoc/>
    public IMaterial WithColor(NamedColor color)
    {
      return this;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
      return obj is MaterialInvisible;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
      return 483886 + base.GetHashCode();
    }

    /// <inheritdoc/>
    public IMaterial WithSpecularProperties(double smoothness, double metalness, double indexOfRefraction)
    {
      return this;
    }

    /// <inheritdoc/>
    public IMaterial WithSpecularPropertiesAs(IMaterial templateMaterial)
    {
      return this;
    }

    /// <inheritdoc/>
    public bool HasSameSpecularPropertiesAs(IMaterial anotherMaterial)
    {
      return anotherMaterial is MaterialInvisible;
    }
  }
}
