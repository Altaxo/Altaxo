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


		/// <summary>
		/// Calculates a color from the provided relative value, that is guaranteed to be between 0 and 1
		/// </summary>
		/// <param name="relVal">Value used for color calculation. Guaranteed to be between 0 and 1.</param>
		/// <returns>A color associated with the relative value.</returns>
		protected override Color GetColorFrom0To1Continuously(double relVal)
		{
				int val = (int)(relVal * 255);
				return System.Drawing.Color.FromArgb(val, (val + val) % 255, (255 - val));
		}

		public override object Clone()
		{
			var result = new ColorProviderBGMYR();
			result.CopyFrom(this);
			return result;
		}
	}

}
