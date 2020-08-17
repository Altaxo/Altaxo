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

  public class Materials
  {
    public static MaterialWithoutColorOrTexture _materialWithoutColorOrTexture = new MaterialWithoutColorOrTexture();

    public static IMaterial GetNoMaterial()
    {
      return MaterialInvisible.Instance;
    }

    public static IMaterial GetSolidMaterial(NamedColor color)
    {
      return new MaterialWithUniformColor(color);
    }

    public static IMaterial GetMaterialWithNewColor(IMaterial material, NamedColor newColor)
    {
      return material.WithColor(newColor);
    }

    public static IMaterial GetSolidMaterialWithoutColorOrTexture()
    {
      return _materialWithoutColorOrTexture;
    }
  }
}
