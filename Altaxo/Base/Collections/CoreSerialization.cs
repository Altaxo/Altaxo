using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Collections
{
  internal class CoreSerialization
  {
    #region Serialization of ContiguousIntegerRange
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase","Altaxo.Collections.IntegerRangeAsCollection", 0)]
    class XmlSerializationSurrogate00 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new NotImplementedException("Should not serialize deprecated type");
        /*
        IntegerRangeAsCollection s = (IntegerRangeAsCollection)obj;
        info.AddValue("Start", s._start);
        info.AddValue("Count", s._count);
        */
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        var start = info.GetInt32("Start");
        var count = info.GetInt32("Count");
        return new ContiguousIntegerRange(start, count);
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ContiguousIntegerRange), 0)]
    class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ContiguousIntegerRange)obj;
        info.AddValue("Start", s.Start);
        info.AddValue("Count", s.Count);

      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        var start = info.GetInt32("Start");
        var count = info.GetInt32("Count");
        return new ContiguousIntegerRange(start,count);
      }
    }

    #endregion

  }
}
