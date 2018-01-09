using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Units
{
	[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(UnitRatioComposite), 0)]
	public class SerializationSurrogate0_UnitRatioComposite : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
	{
		public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
		{
			var s = (UnitRatioComposite)obj;

			info.AddValue("NominatorPrefix", s.NominatorPrefix);
			info.AddValue("NominatorUnit", s.NominatorUnit);
			info.AddValue("DenominatorPrefix", s.DenominatorPrefix);
			info.AddValue("DenominatorUnit", s.DenominatorUnit);
		}

		public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
		{
			var nomPrefix = (SIPrefix)info.GetValue("NominatorPrefix", null);
			var nomUnit = (IUnit)info.GetValue("NominatorUnit", null);
			var denomPrefix = (SIPrefix)info.GetValue("DenominatorPrefix", null);
			var denomUnit = (IUnit)info.GetValue("DenominatorUnit", null);

			return new UnitRatioComposite(nomPrefix, nomUnit, denomPrefix, denomUnit);
		}
	}
}