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
using System.Threading.Tasks;
using Altaxo.Drawing;
using Altaxo.Geometry;

namespace Altaxo.Drawing.D3D
{
  /// <summary>
  /// Interface for material used in 3D geometry.
  /// </summary>
  /// <seealso cref="Altaxo.Main.IImmutable" />
  /// <seealso cref="System.IEquatable{T}" />
  public interface IMaterial : Altaxo.Main.IImmutable, IEquatable<IMaterial>
  {
    /// <summary>
    /// Gets the diffuse color of the material.
    /// </summary>
    /// <value>
    /// The color.
    /// </value>
    NamedColor Color { get; }

    /// <summary>
    /// Gets the specular exponent. The higher this value, the more shiny the material (i.e. the smaller the reflex on the surface).
    /// </summary>
    /// <value>
    /// The specular exponent.
    /// </value>
    double Smoothness { get; }

    /// <summary>
    /// Value between 0 and 1.
    /// If 0, the reflected specular light is multiplied with the material diffuse color. This is often the case for metals, e.g. gold.
    /// If 1, the reflected specular light has the same color as the incident light (thus as if it is reflected at a white surface). This is often the case for plastics.
    /// </summary>
    double Metalness { get; }

    /// <summary>
    /// Gets the index of refraction of the material.
    /// </summary>
    /// <value>
    /// The index of refraction.
    /// </value>
    double IndexOfRefraction { get; }

    /// <summary>
    /// Gets a new instance of this material with the specular properties set to the provided values.
    /// </summary>
    /// <param name="smoothness">The surface smoothness in the range [0, 1].</param>
    /// <param name="metalness">The surface metalness in the range [0, 1].</param>
    /// <param name="indexOfRefraction">The index of refraction of the material.</param>
    /// <returns>A new instance of this material with the specular properties set to the provided values.</returns>
    IMaterial WithSpecularProperties(double smoothness, double metalness, double indexOfRefraction);

    /// <summary>
    /// Returns a new material based on this material, but with all specular properties taken from the template material provided in <paramref name="templateMaterial"/>.
    /// </summary>
    /// <param name="templateMaterial">The template material.</param>
    /// <returns>A new material based on this material, but with all specular properties taken from the template material provided in <paramref name="templateMaterial"/>.</returns>
    IMaterial WithSpecularPropertiesAs(IMaterial templateMaterial);

    /// <summary>
    /// Determines whether this material has the same specular properties as the material provided in <paramref name="anotherMaterial"/>.
    /// </summary>
    /// <param name="anotherMaterial">The material to compare the specular properties with.</param>
    /// <returns>True if this material has the same specular properties as the material provided in <paramref name="anotherMaterial"/>; otherwise false.</returns>
    bool HasSameSpecularPropertiesAs(IMaterial anotherMaterial);

    /// <summary>
    /// Gets a value indicating whether this instance has a color by itself.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance has a color by itself; otherwise, when <c>false</c>, the color of the material is provided by other means (e.g. a texture).
    /// </value>
    bool HasColor { get; }

    /// <summary>
    /// Gets a value indicating whether this instance has texture.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance has a texture; otherwise, <c>false</c>.
    /// </value>
    bool HasTexture { get; }

    /// <summary>
    /// Gets a value indicating whether this material is visible at all. There might be one material instance that could be used as a convenient place holder for null, that
    /// is treated as "no material". Drawing contexts should not draw anything if the material returns false for this property.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is visible; otherwise, <c>false</c>.
    /// </value>
    bool IsVisible { get; }

    /// <summary>
    /// Gets a new instance of this material with the color set to the provided values. Material classes that don't support
    /// color should not throw an exception, but simply return the same instance.
    /// </summary>
    /// <param name="color">The color.</param>
    /// <returns>A new instance of this material with the color set to the provided values.</returns>
    IMaterial WithColor(NamedColor color);

    #region Lighing model support

    /// <summary>
    /// Gets the specular intensity normalized for phong model. This is the expression SpecularIntensity*(1+SpecularExponent).
    /// This pre-factor in the Phong equation ensures that the total light intensity reflected in all directions of the half sphere will not change when changing the SpecularExponent.
    /// </summary>
    /// <value>
    /// The specular intensity normalized for phong model.
    /// </value>
    double PhongModelSpecularIntensity { get; }

    /// <summary>
    /// Gets the diffuse intensity normalized for phong model.
    /// </summary>
    /// <value>
    /// The diffuse intensity normalized for phong model.
    /// </value>
    double PhongModelDiffuseIntensity { get; }

    /// <summary>
    /// Gets the specular exponent for the Phong model.
    /// </summary>
    /// <value>
    /// The specular exponent of the Phong model.
    /// </value>
    double PhongModelSpecularExponent { get; }

    #endregion Lighing model support
  }
}
