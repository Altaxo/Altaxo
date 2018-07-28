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
  /// Base of the materials. This material supports specular reflections using a modified Phong equation:
  /// kspec = SpecularIntensity*(1+SpecularExponent)*DotProduct[IncidentLight,EmergentLight]. The modification
  /// is the term (1+SpecularExponent), which ensures that the integral over the half sphere is constant when the SpecularExponent changes.
  /// </summary>
  /// <seealso cref="Altaxo.Drawing.D3D.IMaterial" />
  public abstract class MaterialBase : IMaterial
  {
    /// <summary>
    /// A value between 0 and 1. A value of 0 defines a rough surface, a value of 1 a very shiny one.
    /// </summary>
    protected double _smoothness;

    /// <summary>
    /// Value between 0 and 1. A value of 0 indicates a plastic like surface, a value of 1 a metal like surface.
    /// If 0, the reflected specular light has the same color as the incident light (thus as if it is reflected at a white surface). This is the behaviour of plastic surfaces.
    /// If 1, the reflected specular light is multiplied with the material diffuse color. This is the behaviour of metals like gold.
    /// </summary>
    protected double _metalness;

    /// <summary>
    /// The index of refraction. i.e. a value between 1 and infinity.
    /// </summary>
    protected double _indexOfRefraction;

    /// <summary>
    /// Initializes a new instance of the <see cref="MaterialBase"/> class with default values for specular intensity, exponent and mixing coefficient.
    /// </summary>
    public MaterialBase()
    {
      _smoothness = 0.5;
      _metalness = 0.5;
      _indexOfRefraction = 1.5;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MaterialBase"/> class.
    /// </summary>
    /// <param name="smoothness">The smoothness value, see <see cref="Smoothness"/>.</param>
    /// <param name="metalness">The specular mixing coefficient, see explanation here: <see cref="Metalness"/>.</param>
    public MaterialBase(double smoothness, double metalness)
      : this(smoothness, metalness, 1.5)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MaterialBase"/> class.
    /// </summary>
    /// <param name="smoothness">The smoothness value, see <see cref="Smoothness"/>.</param>
    /// <param name="metalness">The specular mixing coefficient, see explanation here: <see cref="Metalness"/>.</param>
    /// <param name="indexOfRefraction">The index of refraction, see <see cref="IndexOfRefraction"/>.</param>
    public MaterialBase(double smoothness, double metalness, double indexOfRefraction)
    {
      VerifySmoothness(smoothness, nameof(smoothness));
      _smoothness = smoothness;

      VerifyMetalness(metalness, nameof(metalness));
      _metalness = metalness;

      VerifyIndexOfRefraction(indexOfRefraction, nameof(indexOfRefraction));
      _indexOfRefraction = indexOfRefraction;
    }

    ///<inheritdoc/>
    public abstract NamedColor Color { get; }

    public abstract IMaterial WithColor(NamedColor color);

    ///<inheritdoc/>
    public abstract bool HasColor { get; }

    ///<inheritdoc/>
    public abstract bool HasTexture { get; }

    ///<inheritdoc/>
    public abstract bool IsVisible { get; }

    ///<inheritdoc/>
    public abstract bool Equals(IMaterial other);

    #region Smothness

    /// <summary>
    /// The smothness value, a value in the range [0,1]. A value of 0 defines a rough diffuse surface, and a value of 1 a very shiny surface.
    /// </summary>
    public double Smoothness
    {
      get
      {
        return _smoothness;
      }
    }

    /// <summary>
    /// Gets a new instance of the material with the smothness set to the provided value.
    /// </summary>
    /// <param name="smothness">The smothness value.</param>
    /// <returns>A new instance of the material with smothness set to the provided value.</returns>
    public MaterialBase WithSmoothness(double smothness)
    {
      if (!(smothness == _smoothness))
      {
        VerifySmoothness(smothness, nameof(smothness));

        var result = (MaterialBase)this.MemberwiseClone();
        result._smoothness = smothness;
        return result;
      }
      else
      {
        return this;
      }
    }

    /// <summary>
    /// Verifies the smothness to be in the range [0,1]
    /// </summary>
    /// <param name="value">The value of the specular exponent.</param>
    /// <param name="valueName">Name of the value.</param>
    /// <exception cref="System.ArgumentOutOfRangeException"></exception>
    protected void VerifySmoothness(double value, string valueName)
    {
      if (!(value >= 0))
        throw new ArgumentOutOfRangeException(string.Format("{0} is expected to be >= 0", valueName));
      if (!(value <= 1))
        throw new ArgumentOutOfRangeException(string.Format("{0} is expected to be <= 1", valueName));
    }

    #endregion Smothness

    #region Metalness

    /// <summary>
    /// Mixing coefficient for specular reflection: value between 0 and 1.
    /// If 0, the reflected specular light is multiplied with the material diffuse color
    /// If 1, the reflected specular light has the same color as the incident light (thus as if it is reflected at a white surface)
    /// </summary>
    public double Metalness
    {
      get
      {
        return _metalness;
      }
    }

    /// <summary>
    /// Gets a new instance of the material with the specular mixing coefficient set to the provided value.
    /// </summary>
    /// <param name="metalness">The specular mixing coefficient.</param>
    /// <returns>A new instance of the material with the specular mixing coefficient set to the provided value.</returns>
    public MaterialBase WithMetalness(double metalness)
    {
      if (!(metalness == _metalness))
      {
        VerifyMetalness(metalness, nameof(metalness));

        var result = (MaterialBase)this.MemberwiseClone();
        result._metalness = metalness;
        return result;
      }
      else
      {
        return this;
      }
    }

    /// <summary>
    /// Verifies the metalness to be in the range [0, 1].
    /// </summary>
    /// <param name="value">The value of the metalness.</param>
    /// <param name="valueName">Name of the value.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// </exception>
    protected void VerifyMetalness(double value, string valueName)
    {
      if (!(value >= 0))
        throw new ArgumentOutOfRangeException(string.Format("{0} is expected to be >= 0", valueName));
      if (!(value <= 1))
        throw new ArgumentOutOfRangeException(string.Format("{0} is expected to be <= 1", valueName));
    }

    #endregion Metalness

    #region IndexOfRefraction

    public double IndexOfRefraction
    {
      get
      {
        return _indexOfRefraction;
      }
    }

    /// <summary>
    /// Gets a new instance of the material with the specular mixing coefficient set to the provided value.
    /// </summary>
    /// <param name="indexOfRefraction">The specular mixing coefficient.</param>
    /// <returns>A new instance of the material with the specular mixing coefficient set to the provided value.</returns>
    public MaterialBase WithIndexOfRefraction(double indexOfRefraction)
    {
      if (!(indexOfRefraction == _indexOfRefraction))
      {
        VerifyIndexOfRefraction(indexOfRefraction, nameof(indexOfRefraction));

        var result = (MaterialBase)this.MemberwiseClone();
        result._indexOfRefraction = indexOfRefraction;
        return result;
      }
      else
      {
        return this;
      }
    }

    /// <summary>
    /// Verifies the specular mixing coefficient to be in the range [0, 1].
    /// </summary>
    /// <param name="value">The value of the mixing coefficient.</param>
    /// <param name="valueName">Name of the value.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// </exception>
    protected void VerifyIndexOfRefraction(double value, string valueName)
    {
      if (!(value >= 1))
        throw new ArgumentOutOfRangeException(string.Format("{0} is expected to be >= 0", valueName));
      if (!(value <= double.MaxValue))
        throw new ArgumentOutOfRangeException(string.Format("{0} is expected to be <= double.MinValue", valueName));
    }

    #endregion IndexOfRefraction

    #region Specular Properties

    /// <summary>
    /// Gets a new instance of this material with all specular properties set to the provided values.
    /// </summary>
    /// <param name="smoothness">The surface smoothness in the range [0, 1].</param>
    /// <param name="metalness">The surface metalness in the range [0, 1].</param>
    /// <param name="indexOfRefraction">The index of refraction in the range [1, Infinity].</param>
    /// <returns>A new instance of this material with all specular properties set to the provided values.</returns>
    public MaterialBase WithSpecularProperties(double smoothness, double metalness, double indexOfRefraction)
    {
      if (!(indexOfRefraction == _indexOfRefraction) ||
          !(smoothness == _smoothness) ||
          !(metalness == _metalness))
      {
        VerifySmoothness(smoothness, nameof(smoothness));
        VerifyMetalness(metalness, nameof(metalness));
        VerifyIndexOfRefraction(indexOfRefraction, nameof(indexOfRefraction));

        var result = (MaterialBase)this.MemberwiseClone();
        result._indexOfRefraction = indexOfRefraction;
        result._smoothness = smoothness;
        result._metalness = metalness;
        return result;
      }
      else
      {
        return this;
      }
    }

    ///<inheritdoc/>
    IMaterial IMaterial.WithSpecularProperties(double smoothness, double metalness, double indexOfRefraction)
    {
      return WithSpecularProperties(smoothness, metalness, indexOfRefraction);
    }

    ///<inheritdoc/>
    public IMaterial WithSpecularPropertiesAs(IMaterial templateMaterial)
    {
      return WithSpecularProperties(templateMaterial.Smoothness, templateMaterial.Metalness, templateMaterial.IndexOfRefraction);
    }

    ///<inheritdoc/>
    public bool HasSameSpecularPropertiesAs(IMaterial anotherMaterial)
    {
      return
        this._indexOfRefraction == anotherMaterial.IndexOfRefraction &&
        this._smoothness == anotherMaterial.Smoothness &&
        this._metalness == anotherMaterial.Metalness;
    }

    #endregion Specular Properties

    #region Phong model

    public double PhongModelDiffuseIntensity
    {
      get
      {
        return (1 - _smoothness * _smoothness * _smoothness) * (1 - _smoothness * _metalness);
      }
    }

    /// <summary>
    /// Gets the specular intensity normalized for phong model. This is the expression SpecularIntensity*(1+SpecularExponent).
    /// This pre-factor in the Phong equation ensures that the total light intensity reflected in all directions of the half sphere will not change when changing the SpecularExponent.
    /// </summary>
    /// <value>
    /// The specular intensity normalized for phong model.
    /// </value>
    public double PhongModelSpecularIntensity
    {
      get
      {
        return (PhongModelSpecularExponent + 1) * (_smoothness * _smoothness * _smoothness);
      }
    }

    public double PhongModelSpecularExponent
    {
      get
      {
        return 3 / (1.001 - _smoothness);
      }
    }

    #endregion Phong model
  }
}
