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

namespace Altaxo.Serialization.WITec
{

  /// <summary>
  /// Represents a space transformation defined in a WITec "TDSpaceTransformation" node.
  /// This class currently only exposes the underlying node; transformation logic, if any, is implemented in derived classes.
  /// </summary>
  public class TDSpaceTransformationClass : TDTransformationClass
  {
    /// <summary>
    /// Backing node for the "TDSpaceTransformation" child node.
    /// </summary>
    private WITecTreeNode _tdSpaceTransformation;

    /// <summary>
    /// Initializes a new instance of the <see cref="TDSpaceTransformationClass"/> class.
    /// </summary>
    /// <param name="node">The node representing this transformation in the WITec tree.</param>
    /// <param name="reader">The reader used to resolve referenced nodes if necessary.</param>
    public TDSpaceTransformationClass(WITecTreeNode node, WITecReader reader) : base(node, reader)
    {
      _tdSpaceTransformation = node.GetChild("TDSpaceTransformation");
    }
  }


}
