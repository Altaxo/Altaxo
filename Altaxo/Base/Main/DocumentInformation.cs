using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Main
{
  public class DocumentInformation
  {
    string _documentIdentifier;
    string _documentNotes;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DocumentInformation), 0)]
    class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
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
    #endregion


    public string DocumentIdentifier
    {
      get { return null==_documentIdentifier ? string.Empty : _documentIdentifier; }
      set { _documentIdentifier = value; }
    }

    public string DocumentNotes
    {
      get { return _documentNotes; }
      set { _documentNotes = value; }
    }
  }
}
