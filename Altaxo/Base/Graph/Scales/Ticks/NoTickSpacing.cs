using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Scales.Ticks
{

	/// <summary>
	/// Class that return no ticks at all.
	/// </summary>
	public class NoTickSpacing : TickSpacing
	{
		public override object Clone()
		{
			return new NoTickSpacing();
		}

		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(NoTickSpacing), 0)]
		class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				NoTickSpacing s = (NoTickSpacing)obj;

				
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				NoTickSpacing s = SDeserialize(o, info, parent);
				return s;
			}


			protected virtual NoTickSpacing SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				NoTickSpacing s = null != o ? (NoTickSpacing)o : new NoTickSpacing();

				return s;
			}
		}
		#endregion


		public override bool PreProcessScaleBoundaries(ref Altaxo.Data.AltaxoVariant org, ref Altaxo.Data.AltaxoVariant end, bool isOrgExtendable, bool isEndExtendable)
		{
			return false;
		}

		public override void FinalProcessScaleBoundaries(Altaxo.Data.AltaxoVariant org, Altaxo.Data.AltaxoVariant end, Scale scale)
		{
			
		}

		public override Altaxo.Data.AltaxoVariant[] GetMajorTicksAsVariant()
		{
			return new Altaxo.Data.AltaxoVariant[0];
		}

		public override Altaxo.Data.AltaxoVariant[] GetMinorTicksAsVariant()
		{
			return new Altaxo.Data.AltaxoVariant[0];
		}
	}
}
