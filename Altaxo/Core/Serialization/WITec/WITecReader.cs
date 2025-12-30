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

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Altaxo.Serialization.WITec
{
  /// <summary>
  /// Reader for WiTec project files (.wip).
  /// </summary>
  /// <remarks>
  /// <para>References:</para>
  /// <para>[1] J.T.Holmi, H.Lipsanen, WITio: A MATLAB data evaluation toolbox to script broader insights into big data from WITec microscopes, SoftwareX 18 (2022), 101009, doi: <see href="https://doi.org/10.1016/j.softx.2022.101009"/></para>
  /// <para>[2] Wit tag format description: <see href="https://gitlab.com/jtholmi/wit_io/-/blob/master/+WITio/+doc/README%20on%20WIT-tag%20format.txt"/></para>
  /// <para>[3] Mathlab implementation: <see href="https://gitlab.com/jtholmi/wit_io"/></para>
  /// <para>[4] Phyton implementation: <see href="https://git.photonicdata.science/py-packages/photonicdata-files-wip"/></para>
  /// </remarks>
  public class WITecReader
  {
    /// <summary>
    /// Gets the text encoding that is used in WITec projects (Windows-1252).
    /// </summary>
    public static Encoding TextEncoding { get; } = Encoding.GetEncoding("Windows-1252");


    /// <summary>
    /// The root node of the parsed WITec tree.
    /// </summary>
    private WITecTreeNode RootNode { get; }

    /// <summary>
    /// The data node (child named "Data") within the root node.
    /// </summary>
    private WITecTreeNode DataNode { get; }

    /// <summary>
    /// The magic string read from the file header (first 8 bytes) used to identify the file type.
    /// </summary>
    private string MagicString { get; }

    /// <summary>
    /// Mapping from numeric identifier to the name of the corresponding Data child node.
    /// Key is the identifier, value is the name of the Data child node.
    /// </summary>
    private Dictionary<int, string> ID_To_Name { get; } = new Dictionary<int, string>();

    /// <summary>
    /// List of extracted spectra (TDGraphClass instances) found in the file.
    /// </summary>
    public List<TDGraphClass> Spectra { get; } = [];


    /// <summary>
    /// Initializes a new instance of the <see cref="WITecReader"/> class and parses the provided stream.
    /// The constructor reads the file header, the node tree and prepares the data dictionary.
    /// </summary>
    /// <param name="stream">The stream to read the WITec project from. The stream must be readable and positioned at the beginning of the file.</param>
    public WITecReader(Stream stream)
    {
      var buffer = new byte[8];
      stream.ReadExactly(buffer, 0, 8); // Skip the first 8 bytes
      MagicString = System.Text.Encoding.ASCII.GetString(buffer);
      var (name, node) = WITecTreeNode.Read(stream);
      RootNode = node;
      DataNode = node.GetChild("Data");
      Create_ID_Dictionary(node.GetChild("Data"));
    }

    /// <summary>
    /// Populates the internal identifier dictionary from the provided data node.
    /// The method iterates over the child nodes of <paramref name="dataNode"/> and extracts the integer ID from the child's "TData" node.
    /// </summary>
    /// <param name="dataNode">The data node whose children are inspected to build the ID dictionary.</param>
    protected void Create_ID_Dictionary(WITecTreeNode dataNode)
    {
      ID_To_Name.Clear();
      foreach (var entry in dataNode.ChildNodes)
      {
        if (entry.Value.ChildNodes.TryGetValue("TData", out var tdataNode) &&
           tdataNode.Data.TryGetValue("ID", out var idobj) &&
           idobj is int idnumber)
        {
          ID_To_Name.Add(idnumber, entry.Key);
        }
      }
    }

    /// <summary>
    /// Builds a data class instance for the node identified by the provided identifier.
    /// The method resolves the name of the data node from the internal ID dictionary and constructs the appropriate <see cref="TDataClass"/> derived instance.
    /// </summary>
    /// <typeparam name="T">The expected return type, which must derive from <see cref="TDataClass"/>.</typeparam>
    /// <param name="id">The identifier of the node to build.</param>
    /// <returns>An instance of <typeparamref name="T"/> representing the node with the requested identifier.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the specified <paramref name="id"/> does not exist in the ID dictionary.</exception>
    /// <exception cref="NotImplementedException">Thrown when the found class name is not implemented in the switch statement.</exception>
    /// <exception cref="InvalidDataException">Thrown when the constructed instance is not of the expected type <typeparamref name="T"/>.</exception>
    public T BuildNodeWithID<T>(int id) where T : TDataClass
    {
      var name = ID_To_Name[id];
      var className = DataNode.GetData<string>(TDataClass.GetClassNameOfName(name));
      var treeNode = DataNode.GetChild(name);

      var result = className switch
      {
        "TDSpectralInterpretation" => new TDSpectralInterpretationClass(treeNode),
        "TDZInterpretation" => new TDZInterpretationClass(treeNode),

        "TDSpaceTransformation" => (TDataClass)new TDSpaceTransformationClass(treeNode, this),
        "TDSpectralTransformation" => new TDSpectralTransformationClass(treeNode, this),
        "TDLinearTransformation" => new TDLinearTransformationClass(treeNode, this),

        "TDGraph" => new TDGraphClass(treeNode, this),

        "TDText" => new TDTextClass(treeNode),

        _ => throw new NotImplementedException($"Node with ID={id} has class name {className}, which is not implemented yet."),
      };

      if (result is not T typedResult)
        throw new InvalidDataException($"The node {name} was expected to be of type {typeof(T)}, but it has type {result.GetType()}");
      return typedResult;
    }


    /// <summary>
    /// Searches the parsed data node for all entries of class "TDGraph" and constructs corresponding <see cref="TDGraphClass"/> instances.
    /// Found instances are appended to the public <see cref="Spectra"/> list.
    /// </summary>
    public void ExtractSpectra()
    {
      // search for all nodes that are TDGraph nodes
      var listOfGraphNodes = new List<WITecTreeNode>();

      foreach (var node in DataNode.ChildNodes.Values)
      {
        var className = DataNode.GetData<string>(TDataClass.GetClassNameOfName(node.Name));
        if (className == "TDGraph")
        {
          var instance = new TDGraphClass(node, this);
          Spectra.Add(instance);
        }
      }
    }
  }
}
