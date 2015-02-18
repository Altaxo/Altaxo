using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Scales.Ticks
{
	public class NoNumericTickSpacing : NumericTickSpacing
	{
		public override object Clone()
		{
			return new NoNumericTickSpacing();
		}

		public override bool CopyFrom(object obj)
		{
			return obj is NoNumericTickSpacing;
		}

		#region Serialization

		/// <summary>
		/// 2015-02-11 Initial version
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(NoNumericTickSpacing), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				return (o as NoNumericTickSpacing) ?? new NoNumericTickSpacing();
			}
		}

		#endregion Serialization

		/// <summary>
		/// Decides giving a raw org and end value, whether or not the scale boundaries should be extended to
		/// have more 'nice' values. If the boundaries should be changed, the function return true, and the
		/// org and end argument contain the proposed new scale boundaries.
		/// </summary>
		/// <param name="org">Raw scale org.</param>
		/// <param name="end">Raw scale end.</param>
		/// <param name="isOrgExtendable">True when the org is allowed to be extended.</param>
		/// <param name="isEndExtendable">True when the scale end can be extended.</param>
		/// <returns>
		/// True when org or end are changed. False otherwise.
		/// </returns>
		public override bool PreProcessScaleBoundaries(ref Data.AltaxoVariant org, ref Data.AltaxoVariant end, bool isOrgExtendable, bool isEndExtendable)
		{
			return false;
		}

		public override void FinalProcessScaleBoundaries(Data.AltaxoVariant org, Data.AltaxoVariant end, Scale scale)
		{
		}

		protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
		{
			yield break;
		}
	}
}