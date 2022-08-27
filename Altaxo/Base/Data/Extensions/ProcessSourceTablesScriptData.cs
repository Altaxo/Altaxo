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
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using Altaxo.Collections;
using Altaxo.Main;
using Altaxo.Scripting;

namespace Altaxo.Data
{
  public class ProcessSourceTablesScriptData : Main.SuspendableDocumentNodeWithEventArgs, ICloneable
  {
    private List<(string Name, DataTableProxy TableProxy)> _tables = new();

    #region Serialization

    #region Version 0

    /// <summary>
    /// 2022-08-27 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ProcessSourceTablesScriptData), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ProcessSourceTablesScriptData)obj;

        info.CreateArray("Tables", s._tables.Count);
        {
          for(int i=0;i<s._tables.Count;++i)
          {
            info.CreateElement("e");
            {
              info.AddValue("Name", s._tables[i].Name);
              info.AddValue("Table", s._tables[i].TableProxy);
            }
            info.CommitElement(); // e
          }

        }
        info.CommitArray();
      }



      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var count = info.OpenArray("Tables");
        var list = new List<(string Name, DataTableProxy DataTable)>(count);
        for(int i=0;i<count;++i)
        {
          info.OpenElement();
          {
            var name = info.GetString("Name");
            var table = info.GetValue<DataTableProxy>("Table", null);
            list.Add((name, table));
          }
          info.CloseElement();
        }
        info.CloseArray(count);

        return new ProcessSourceTablesScriptData(list);
      }
    }

    #endregion Version 0

    #endregion Serialization



    public ProcessSourceTablesScriptData(IEnumerable<(string Name, DataTableProxy TableProxy)> tables)
    {
      _tables = new();
      foreach (var entry in tables)
      {
        var tt = (DataTableProxy)entry.TableProxy.Clone();
        tt.ParentObject = this;
        _tables.Add((entry.Name, tt));
      }
    }

    public object Clone()
    {
      return new ProcessSourceTablesScriptData(_tables);
    }

    /// <summary>
    /// Gets the source tables for the extraction.
    /// </summary>
    public IReadOnlyList<(string Name, DataTableProxy Proxy)> TableProxies => _tables;

    public IReadOnlyListDictionary<string, DataTable> Tables
    {
      get
      {
        var result = new ListDictionary<string, DataTable>();

        foreach (var entry in _tables)
        {
          result.Add(entry.Name, entry.TableProxy.Document);
        }
        return result;
      }
    }


    protected override IEnumerable<DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (_tables is { } tables)
      {
        for (int i = tables.Count - 1; i >= 0; i--)
          yield return new DocumentNodeAndName(_tables[i].TableProxy, $"Table[{i}]");
      }
    }

    public void VisitDocumentReferences(DocNodeProxyReporter reportProxies)
    {
      for (int i = _tables.Count - 1; i >= 0; i--)
        reportProxies(_tables[i].TableProxy, this, $"Tables[{i}]");
    }
  }
}
