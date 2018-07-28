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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Main
{
  public class DocumentInformation
  {
    private string _documentIdentifier;
    private string _documentNotes;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DocumentInformation), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        DocumentInformation s = (DocumentInformation)obj;

        info.AddValue("Identifier", s.DocumentIdentifier);
        info.AddValue("Notes", s.DocumentNotes);
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        DocumentInformation s = SDeserialize(o, info, parent);
        return s;
      }

      protected virtual DocumentInformation SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        DocumentInformation s = null != o ? (DocumentInformation)o : new DocumentInformation();

        s._documentIdentifier = info.GetString("Identifier");
        s._documentNotes = info.GetString("Notes");

        return s;
      }
    }

    #endregion Serialization

    public string DocumentIdentifier
    {
      get { return null == _documentIdentifier ? string.Empty : _documentIdentifier; }
      set { _documentIdentifier = value; }
    }

    public string DocumentNotes
    {
      get { return _documentNotes; }
      set { _documentNotes = value; }
    }
  }
}
