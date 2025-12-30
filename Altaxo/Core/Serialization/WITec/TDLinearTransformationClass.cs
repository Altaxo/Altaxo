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

  /// <summary>
  /// Represents a linear transformation defined in a WITec TDLinearTransformation node.
  /// The transformation maps model coordinates to world coordinates using an origin, scale and offset.
  /// </summary>
  public class TDLinearTransformationClass : TDTransformationClass
  {
    /// <summary>
    /// Backing node for the "TDLinearTransformation" child node.
    /// </summary>
    private WITecTreeNode _tdLinearTransformation;

    /// <summary>
    /// The model origin value read from the node (ModelOrigin_D).
    /// </summary>
    private double _modelOrigin_D;

    /// <summary>
    /// The world origin value read from the node (WorldOrigin_D).
    /// </summary>
    private double _worldOrigin_D;

    /// <summary>
    /// The scale factor read from the node (Scale_D).
    /// </summary>
    private double _scale_D;

    /// <summary>
    /// Initializes a new instance of the <see cref="TDLinearTransformationClass"/> class.
    /// The constructor reads transformation parameters from the provided node.
    /// </summary>
    /// <param name="node">The node representing the transformation.</param>
    /// <param name="reader">The reader used to resolve referenced nodes if necessary.</param>
    public TDLinearTransformationClass(WITecTreeNode node, WITecReader reader) : base(node, reader)
    {
      _tdLinearTransformation = node.GetChild("TDLinearTransformation");

      _modelOrigin_D = _tdLinearTransformation.GetData<double>("ModelOrigin_D");
      _worldOrigin_D = _tdLinearTransformation.GetData<double>("WorldOrigin_D");
      _scale_D = _tdLinearTransformation.GetData<double>("Scale_D");

    }

    /// <summary>
    /// Transforms the provided sequence of values by applying the linear mapping
    /// (value - ModelOrigin_D) * Scale_D + WorldOrigin_D to each element.
    /// </summary>
    /// <param name="values">The source values (model coordinates) to transform.</param>
    /// <returns>An enumeration of transformed values (world coordinates) in the same order.</returns>
    public IEnumerable<double> Transform(IEnumerable<double> values)
    {
      return values.Select(value =>
      {
        return (value - _modelOrigin_D) * _scale_D + _worldOrigin_D;
      });
    }
  }
}
