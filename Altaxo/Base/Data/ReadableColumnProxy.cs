#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
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
      public new class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        info.AddBaseValueEmbedded(obj,typeof(DocNodeProxy)); // serialize the base class
      }

      public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        ReadableColumnProxy s = o!=null ? (ReadableColumnProxy)o : new ReadableColumnProxy();
        info.GetBaseValueEmbedded(s,typeof(DocNodeProxy),parent);         // deserialize the base class

        info.DeserializationFinished += new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(s.EhXmlDeserializationFinished);

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

    private void EhXmlDeserializationFinished(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object documentRoot)
    {
      if(this.Document!=null)
        info.DeserializationFinished -= new Altaxo.Serialization.Xml.XmlDeserializationCallbackEventHandler(this.EhXmlDeserializationFinished);
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
  }
}
