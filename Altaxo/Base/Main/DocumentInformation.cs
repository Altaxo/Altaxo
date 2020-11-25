#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
using System.Linq;
using System.Text;

namespace Altaxo.Main
{
  public class DocumentInformation
  {
    private string _documentIdentifier = string.Empty;
    private string _documentNotes = string.Empty;
    private Version _versionCreatedWith = new Version(0, 0, 0, 0);



    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DocumentInformation), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (DocumentInformation)obj;

        info.AddValue("Identifier", s.DocumentIdentifier);
        info.AddValue("Notes", s.DocumentNotes);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        DocumentInformation s = SDeserialize(o, info, parent);
        return s;
      }

      protected virtual DocumentInformation SDeserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        DocumentInformation s = (DocumentInformation?)o ?? new DocumentInformation();

        s._documentIdentifier = info.GetString("Identifier");
        s._documentNotes = info.GetString("Notes");


        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DocumentInformation), 1)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (DocumentInformation)obj;

        info.AddValue("Identifier", s.DocumentIdentifier);
        info.AddValue("Notes", s.DocumentNotes);

        var version = Version.Parse(RevisionClass.FullVersion);
        info.AddValue("AltaxoVersion", version.ToString());
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        DocumentInformation s = SDeserialize(o, info, parent);
        return s;
      }

      protected virtual DocumentInformation SDeserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        DocumentInformation s = (DocumentInformation?)o ?? new DocumentInformation();

        s._documentIdentifier = info.GetString("Identifier");
        s._documentNotes = info.GetString("Notes");
        s._versionCreatedWith = Version.Parse(info.GetString("AltaxoVersion"));

        return s;
      }
    }

    #endregion Serialization


    public string DocumentIdentifier
    {
      get { return _documentIdentifier ?? string.Empty; }
      set { _documentIdentifier = value ?? string.Empty; }
    }

    public string DocumentNotes
    {
      get { return _documentNotes; }
      set { _documentNotes = value ?? string.Empty; }
    }

    /// <summary>
    /// Gets the Altaxo version this document was created with.
    /// </summary>
    /// <value>
    /// The Altaxo version this document was created with.
    /// </value>
    public Version AltaxoVersionCreatedWith
    {
      get
      {
        return _versionCreatedWith;
      }
    }


  }
}
