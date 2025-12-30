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
  /// Represents an interpretation of measurement units for a WITec data class.
  /// This class extracts the unit index and maps it to a human-readable description and shortcut.
  /// </summary>
  public class TDInterpretationClass : TDataClass
  {
    /// <summary>
    /// Backing node for the "TDInterpretation" child node of the data class.
    /// </summary>
    protected WITecTreeNode _tdInterpretation;

    /// <summary>
    /// Gets the index identifying the selected unit.
    /// </summary>
    public int UnitIndex { get; }

    /// <summary>
    /// Gets the textual description of the unit corresponding to <see cref="UnitIndex"/>.
    /// </summary>
    public string UnitDescription { get; }

    /// <summary>
    /// Gets the short notation or shortcut for the unit corresponding to <see cref="UnitIndex"/>.
    /// </summary>
    public string UnitShortCut { get; }

    /// <summary>
    /// Returns the unit description and shortcut for the provided <paramref name="unitIndex"/>.
    /// Derived classes should override this to provide mapping between unit indices and their textual representations.
    /// </summary>
    /// <param name="unitIndex">The unit index to look up.</param>
    /// <returns>A tuple containing the description and the short cut for the unit.</returns>
    public virtual (string Description, string shortCut) GetUnit(int unitIndex)
    {
      return ("", "");
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TDInterpretationClass"/> using the provided node.
    /// The constructor reads the unit index and resolves the corresponding description and shortcut using <see cref="GetUnit(int)"/>.
    /// </summary>
    /// <param name="node">The node representing this interpretation in the WITec tree.</param>
    public TDInterpretationClass(WITecTreeNode node)
      : base(node)
    {
      _tdInterpretation = node.GetChild("TDInterpretation");
      UnitIndex = _tdInterpretation.GetData<int>("UnitIndex");
      (UnitDescription, UnitShortCut) = GetUnit(UnitIndex);

    }
  }
}

