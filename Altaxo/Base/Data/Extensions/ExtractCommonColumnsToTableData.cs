#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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
using System.Collections.Immutable;
using System.Linq;
using Altaxo.Main;

namespace Altaxo.Data
{
  /// <summary>
  /// Process data for <see cref="ExtractCommonColumnsToTableDataSource"/>.
  /// </summary>
  /// <seealso cref="Altaxo.Main.SuspendableDocumentNodeWithEventArgs" />
  /// <seealso cref="System.ICloneable" />
  public class ExtractCommonColumnsToTableData : Main.SuspendableDocumentNodeWithEventArgs, ICloneable
  {
    private List<DataTableProxy> _tables;
    string _xColumnName;
    ImmutableArray<string> _yColumnNames;


    #region Serialization

    /// <summary>
    /// 2022-10-12 Initial version
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ExtractCommonColumnsToTableData), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ExtractCommonColumnsToTableData)obj;

        info.AddArray("DataTableProxies", s.Tables, s.Tables.Count);
        info.AddValue("XColumnName", s.XColumnName);
        info.AddArray("YColumnNames", s.YColumnNames, s.YColumnNames.Length);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {

        var dataTableProxies = info.GetArrayOfValues<DataTableProxy>("DataTableProxies", parent);
        var xColumnName = info.GetString("XColumnName");
        var yColumnNames = info.GetArrayOfStrings("YColumnNames");

        return new ExtractCommonColumnsToTableData(dataTableProxies, xColumnName, yColumnNames.Select(s => s ?? String.Empty).ToImmutableArray());
      }
    }

    #endregion

    public ExtractCommonColumnsToTableData(IEnumerable<DataTableProxy> tables, string xColumnName, ImmutableArray<string> yColumnNames)
    {
      _tables = new List<DataTableProxy>();
      foreach (var t in tables)
      {
        var tt = (DataTableProxy)t.Clone();
        tt.ParentObject = this;
        _tables.Add(tt);
      }
      _xColumnName = xColumnName ?? string.Empty;
      _yColumnNames = yColumnNames;
    }

    public object Clone()
    {
      return new ExtractCommonColumnsToTableData(
         _tables,
        _xColumnName,
        _yColumnNames
        );
    }

    /// <summary>
    /// Gets the source tables for the extraction.
    /// </summary>
    public IReadOnlyList<DataTableProxy> Tables => _tables;

    /// <summary>
    /// Gets the common name of the x column used for extraction.
    /// </summary>
    public string XColumnName => _xColumnName;

    /// <summary>
    /// Get the names of the y columns used for extraction.
    /// </summary>
    public ImmutableArray<string> YColumnNames => _yColumnNames;

   

    protected override IEnumerable<DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (_tables is { } tables)
      {
        for (int i = tables.Count - 1; i >= 0; i--)
          yield return new DocumentNodeAndName(_tables[i], $"Table[{i}]");
      }
    }

    public void VisitDocumentReferences(DocNodeProxyReporter reportProxies)
    {
      for (int i = _tables.Count - 1; i >= 0; i--)
        reportProxies(_tables[i], this, $"Tables[{i}]");
    }
  }
}
