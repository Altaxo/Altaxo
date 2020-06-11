using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Units.ElectricPotential
{
  [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Volt), 0)]
  public class SerializationSurrogate0_Volt : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
  {
    public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
    {
    }

    public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
    {
      return Volt.Instance;
    }
  }
}
