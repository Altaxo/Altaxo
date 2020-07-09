#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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
using Altaxo.Main;

namespace Altaxo.Data
{
  /// <summary>
  /// Summary description for DataColumnPlaceHolder.
  /// </summary>
  [Serializable]
  public class DataTableProxy : DocNodeProxy
  {
    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Data.DataTableProxy", 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new InvalidOperationException("Serialization of old version");
        /*
                info.AddBaseValueEmbedded(obj, obj.GetType().BaseType); // serialize the base class
                */
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (DataTableProxy?)o ?? new DataTableProxy(info);
#pragma warning disable CS0618 // Type or member is obsolete
        var baseobj = info.GetBaseValueEmbeddedOrNull(s, "AltaxoBase,Altaxo.Main.DocNodeProxy,0", parent);         // deserialize the base class
#pragma warning restore CS0618 // Type or member is obsolete

        if (!object.ReferenceEquals(s, baseobj))
        {
          throw new InvalidProgramException($"What should be returned here? S: {s}, baseobj: {baseobj}");
          // return null;
        }

        if (!(null != s.InternalDocumentPath))
          throw new InvalidOperationException();
        return s;
      }
    }

    /// <summary>
    /// 2014-12-26 From here on it is ensured that document path has always a value
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DataTableProxy), 1)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        info.AddBaseValueEmbedded(obj, obj.GetType().BaseType!); // serialize the base class
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (DataTableProxy?)o ?? new DataTableProxy(info);
        info.GetBaseValueEmbedded(s, s.GetType().BaseType!, parent);         // deserialize the base class

        if (!(null != s.InternalDocumentPath))
          throw new InvalidOperationException();

        return s;
      }
    }

    #endregion Serialization

    public DataTableProxy(DataTable table)
      : base(table)
    {
    }

    /// <summary>
    /// For deserialization purposes only.
    /// </summary>
    protected DataTableProxy(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
      : base(info)
    {
    }

    /// <summary>
    /// Cloning constructor.
    /// </summary>
    /// <param name="from">Object to clone from.</param>
    public DataTableProxy(DataTableProxy from)
      : base(from)
    {
    }

    protected override bool IsValidDocument(object obj)
    {
      return (obj is DataTable) || obj == null;
    }

    public DataTable? Document
    {
      get
      {
        return (DataTable?)base.DocumentObject();
      }
    }

    public override object Clone()
    {
      return new DataTableProxy(this);
    }

    public string GetName(int level)
    {
      var table = Document; // this may have the side effect that the object is tried to resolve, is this o.k.?
      if (null != table)
      {
        return table.Name;
      }
      else
      {
        string path = InternalDocumentPath.ToString();
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
    }
  }
}
