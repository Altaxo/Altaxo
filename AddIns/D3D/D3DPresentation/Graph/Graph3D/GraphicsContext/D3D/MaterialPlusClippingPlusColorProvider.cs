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
using System.Threading.Tasks;
using Altaxo.Drawing.D3D;
using Altaxo.Geometry;

namespace Altaxo.Graph.Graph3D.GraphicsContext.D3D
{
  /// <summary>
  /// Combines a material with one or more clip planes.
  /// </summary>
  public class MaterialPlusClippingPlusColorProviderKey : MaterialPlusClippingKey
  {
    public Gdi.Plot.IColorProvider ColorProvider { get; private set; }

    public MaterialPlusClippingPlusColorProviderKey(IMaterial material, PlaneD3D[] clipPlanes, Gdi.Plot.IColorProvider colorProvider)
      : base(material, clipPlanes)
    {
      if (null == colorProvider)
        throw new ArgumentNullException(nameof(colorProvider));

      ColorProvider = colorProvider;
    }

    public override bool Equals(object obj)
    {
      var from = obj as MaterialPlusClippingPlusColorProviderKey;
      if (null == from)
        return false;

      return base.Equals(from) && ColorProvider.Equals(from.ColorProvider);
    }

    public override int GetHashCode()
    {
      return base.GetHashCode() + 73 * ColorProvider.GetHashCode();
    }
  }
}
