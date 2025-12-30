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
  /// Interpretation class for Z-axis units used in WITec data.
  /// Reads the unit name from the underlying node and exposes it via <see cref="UnitName"/>.
  /// </summary>
  public class TDZInterpretationClass : TDInterpretationClass
  {
    /// <summary>
    /// Backing node for the "TDZInterpretation" child node.
    /// </summary>
    private WITecTreeNode _tdzInterpretation;

    /// <summary>
    /// Gets the name of the unit used for Z values as read from the node.
    /// </summary>
    public string UnitName { get; }


    /// <summary>
    /// Initializes a new instance of the <see cref="TDZInterpretationClass"/> class using the provided node.
    /// </summary>
    /// <param name="node">The node representing this Z-interpretation in the WITec tree.</param>
    public TDZInterpretationClass(WITecTreeNode node) : base(node)
    {
      _tdzInterpretation = node.GetChild("TDZInterpretation");
      UnitName = _tdzInterpretation.GetData<string>("UnitName");
    }
  }
}

