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
  using Material;

  /// <summary>
  /// Factory and helper methods for commonly used 3D materials.
  /// </summary>
  public class Materials
  {
    /// <summary>
    /// Shared material instance that has neither color nor texture.
    /// </summary>
    public static MaterialWithoutColorOrTexture _materialWithoutColorOrTexture = new MaterialWithoutColorOrTexture();

    /// <summary>
    /// Gets the invisible material instance.
    /// </summary>
    /// <returns>The invisible material.</returns>
    public static IMaterial GetNoMaterial()
    {
      return MaterialInvisible.Instance;
    }

    /// <summary>
    /// Creates a solid material with a uniform color.
    /// </summary>
    /// <param name="color">The material color.</param>
    /// <returns>A material with the specified uniform color.</returns>
    public static IMaterial GetSolidMaterial(NamedColor color)
    {
      return new MaterialWithUniformColor(color);
    }

    /// <summary>
    /// Creates a material equivalent to the specified material but with a different color.
    /// </summary>
    /// <param name="material">The material to copy.</param>
    /// <param name="newColor">The replacement color.</param>
    /// <returns>A material with the updated color.</returns>
    public static IMaterial GetMaterialWithNewColor(IMaterial material, NamedColor newColor)
    {
      return material.WithColor(newColor);
    }

    /// <summary>
    /// Gets the shared material instance that has no color and no texture.
    /// </summary>
    /// <returns>The shared colorless, textureless material.</returns>
    public static IMaterial GetSolidMaterialWithoutColorOrTexture()
    {
      return _materialWithoutColorOrTexture;
    }
  }
}
