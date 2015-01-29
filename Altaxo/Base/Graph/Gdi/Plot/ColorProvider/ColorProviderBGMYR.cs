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
using System.Drawing;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Gdi.Plot.ColorProvider
{
	public class ColorProviderBGMYR : ColorProviderBase
	{
		#region Serialization

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ColorProviderBGMYR), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
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

		#endregion Serialization

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