#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

using System;
using Altaxo.Main;

namespace Altaxo.Data
{


 
  /// <summary>
  /// Summary description for DataColumnPlaceHolder.
  /// </summary>
  public class ReadableColumnProxy : DocNodeProxy
  {
    #region Serialization
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ReadableColumnProxy),0)]
      class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        info.AddBaseValueEmbedded(obj,typeof(DocNodeProxy)); // serialize the base class
      }

      public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        ReadableColumnProxy s = o!=null ? (ReadableColumnProxy)o : new ReadableColumnProxy();
        info.GetBaseValueEmbedded(s,typeof(DocNodeProxy),parent);         // deserialize the base class

        return s;
      }
    }
    #endregion

    public ReadableColumnProxy(IReadableColumn column)
      : base(column)
    {
    }

    /// <summary>
    /// For deserialization purposes only.
    /// </summary>
    protected ReadableColumnProxy()
    {
    }

    /// <summary>
    /// Cloning constructor.
    /// </summary>
    /// <param name="from">Object to clone from.</param>
    public ReadableColumnProxy(ReadableColumnProxy from)
      : base(from)
    {
    }

    protected override bool IsValidDocument(object obj)
    {
      return (obj is IReadableColumn) || obj==null;
    }
   
    public IReadableColumn Document
    {
      get
      {
        return (IReadableColumn)base.DocumentObject;        
      }
    }

    public override object Clone()
    {
      return new ReadableColumnProxy(this);
    }

    public string GetName(int level)
    {
      IReadableColumn col = this.Document; // this may have the side effect that the object is tried to resolve, is this o.k.?
      if (col is Data.DataColumn)
      {
        Altaxo.Data.DataTable table = Altaxo.Data.DataTable.GetParentDataTableOf((DataColumn)col);
        string tablename = table == null ? string.Empty : table.Name + "\\";
        string collectionname = table == null ? string.Empty : (table.PropertyColumns.ContainsColumn((DataColumn)col) ? "PropCols\\" : "DataCols\\");
        if (level <= 0)
          return ((DataColumn)col).Name;
        else if (level == 1)
          return tablename + ((DataColumn)col).Name;
        else
          return tablename + collectionname + ((DataColumn)col).Name;
      }
      else if(base._docNodePath != null)
      {
        string path =  _docNodePath.ToString();
        int idx = 0;
        if (level <= 0)
        {
          idx = path.LastIndexOf('/');
          if (idx < 0)
            idx = 0;
          else
            idx++;
        }

        return path.Substring(idx);
      }
      else if (col != null)
      {
        return col.ToString();
      }
      else
      {
        return string.Empty;
      }
    }
  }
}
