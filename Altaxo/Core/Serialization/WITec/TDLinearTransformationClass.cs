#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2024 Dr. Dirk Lellinger
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

using System.Collections.Generic;
using System.Linq;

namespace Altaxo.Serialization.WITec
{

  public class TDLinearTransformationClass : TDTransformationClass
  {
    private WITecTreeNode _tdLinearTransformation;
    private double _modelOrigin_D;
    private double _worldOrigin_D;
    private double _scale_D;

    public TDLinearTransformationClass(WITecTreeNode node, WITecReader reader) : base(node, reader)
    {
      _tdLinearTransformation = node.GetChild("TDLinearTransformation");

      _modelOrigin_D = _tdLinearTransformation.GetData<double>("ModelOrigin_D");
      _worldOrigin_D = _tdLinearTransformation.GetData<double>("WorldOrigin_D");
      _scale_D = _tdLinearTransformation.GetData<double>("Scale_D");

    }

    public IEnumerable<double> Transform(IEnumerable<double> values)
    {
      return values.Select(value =>
      {
        return (value - _modelOrigin_D) * _scale_D + _worldOrigin_D;
      });
    }
  }
}
