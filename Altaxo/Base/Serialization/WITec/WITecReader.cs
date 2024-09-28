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


    private WITecTreeNode RootNode { get; }
    private WITecTreeNode DataNode { get; }

    private string MagicString { get; }

    private Dictionary<int, string> ID_To_Name { get; } = new Dictionary<int, string>();

    public List<TDGraphClass> Spectra { get; } = [];


    public WITecReader(Stream stream)
    {
      var buffer = new byte[8];
      stream.ForcedRead(buffer, 0, 8); // Skip the first 8 bytes
      MagicString = System.Text.Encoding.ASCII.GetString(buffer);
      var (name, node) = WITecTreeNode.Read(stream);
      RootNode = node;
      DataNode = node.GetChild("Data");
      Create_ID_Dictionary(node.GetChild("Data"));
    }

    /// <summary>
    /// Gets the identifier dictionary.
    /// Key is the indentifier, value is the name of the Data child node.
    /// </summary>
    /// <param name="dataNode">The data node.</param>
    /// <returns></returns>
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
