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
  /// Represents a TData class extracted from a WITec project node.
  /// This wraps the underlying <see cref="WITecTreeNode"/> and provides easy access to
  /// commonly used values such as the ID, caption and class name.
  /// </summary>
  public class TDataClass
  {
    /// <summary>
    /// Gets the underlying node for this data class.
    /// </summary>
    public WITecTreeNode Node { get; }

    /// <summary>
    /// Gets the node named "TData" that contains the data values for this class.
    /// </summary>
    public WITecTreeNode TData { get; }

    /// <summary>
    /// Gets the identifier of this data class.
    /// </summary>
    public int ID { get; }

    /// <summary>
    /// Gets the caption of this data class.
    /// </summary>
    public string Caption { get; }

    /// <summary>
    /// Gets the class name associated with this data class.
    /// </summary>
    public string ClassName { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TDataClass"/> class using the provided node.
    /// </summary>
    /// <param name="node">The node that represents this data class in the WITec tree.</param>
    public TDataClass(WITecTreeNode node)
    {
      Node = node;
      ClassName = node.Parent.GetData<string>(GetClassNameOfName(node.Name));
      TData = node.GetChild("TData");
      ID = TData.GetData<int>("ID");
      Caption = TData.GetData<string>("Caption");
    }

    /// <summary>
    /// Gets the class name key corresponding to a data node name.
    /// For example, a node name ending with "Data" will be converted to the key ending with "DataClassName".
    /// </summary>
    /// <param name="name">The node name to convert.</param>
    /// <returns>The corresponding class-name key for the given node name.</returns>
    public static string GetClassNameOfName(string name)
    {
      return name.Replace("Data", "DataClassName");
    }
  }
}
