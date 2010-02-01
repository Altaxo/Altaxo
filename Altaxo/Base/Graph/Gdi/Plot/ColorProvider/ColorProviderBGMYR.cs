using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Altaxo.Graph.Gdi.Plot.ColorProvider
{
	public class ColorProviderBGMYR : ColorProviderBase
	{
		#region Serialization
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ColorProviderBGMYR), 0)]
		class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				ColorProviderBGMYR s = (ColorProviderBGMYR)obj;
				info.AddBaseValueEmbedded(s, typeof(ColorProviderBase));

			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{

				ColorProviderBGMYR s = null != o ? (ColorProviderBGMYR)o : new ColorProviderBGMYR();
				info.GetBaseValueEmbedded(s, typeof(ColorProviderBase), parent);

				return s;
			}
		}

		#endregion


		public override Color GetColor(double relVal)
		{
			if (relVal >= 0 && relVal <= 1)
			{
				int val = (int)(relVal * 255);
				return System.Drawing.Color.FromArgb(val, (val + val) % 255, (255 - val));
			}
			else
				return GetOutOfBoundsColor(relVal);
		}

		public override object Clone()
		{
			var result = new ColorProviderBGMYR();
			result.CopyFrom(this);
			return result;
		}
	}

}
