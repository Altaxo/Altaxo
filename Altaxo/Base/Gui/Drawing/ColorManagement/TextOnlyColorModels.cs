#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2017 Dr. Dirk Lellinger
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
//    along with ctrl program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Drawing;

namespace Altaxo.Gui.Drawing.ColorManagement
{
	public class RGBColorTextModel : ITextOnlyColorModel
	{
		public string[] GetComponentsForColor(AxoColor color, IFormatProvider formatProvider)
		{
			return new string[] { color.R.ToString(formatProvider), color.G.ToString(formatProvider), color.B.ToString(formatProvider) };
		}

		public string[] GetNamesOfComponents()
		{
			return new[] { "R", "G", "B" };
		}
	}

	public class CMYColorTextModel : ITextOnlyColorModel
	{
		public string[] GetComponentsForColor(AxoColor color, IFormatProvider formatProvider)
		{
			return new string[] { (255 - color.R).ToString(formatProvider), (255 - color.G).ToString(formatProvider), (255 - color.B).ToString(formatProvider) };
		}

		public string[] GetNamesOfComponents()
		{
			return new[] { "C", "M", "Y" };
		}
	}

	public class CMYKColorTextModel : ITextOnlyColorModel
	{
		public string[] GetComponentsForColor(AxoColor color, IFormatProvider formatProvider)
		{
			var c = 255 - color.R;
			var m = 255 - color.G;
			var y = 255 - color.B;
			var min = Math.Min(c, Math.Min(m, y));

			var cc = AxoColor.NormFloatToByte((c - min) / (255.0f - min));
			var mm = AxoColor.NormFloatToByte((m - min) / (255.0f - min));
			var yy = AxoColor.NormFloatToByte((y - min) / (255.0f - min));
			var kk = min;

			return new string[] { cc.ToString(formatProvider), mm.ToString(formatProvider), yy.ToString(formatProvider), kk.ToString(formatProvider) };
		}

		public string[] GetNamesOfComponents()
		{
			return new[] { "C", "M", "Y", "K" };
		}
	}

	public class HSBColorTextModel : ITextOnlyColorModel
	{
		public string[] GetComponentsForColor(AxoColor color, IFormatProvider formatProvider)
		{
			var (a, h, s, b) = color.ToAHSB();
			return new string[] { AxoColor.NormFloatToByte(h).ToString(formatProvider), AxoColor.NormFloatToByte(s).ToString(formatProvider), AxoColor.NormFloatToByte(b).ToString(formatProvider) };
		}

		public string[] GetNamesOfComponents()
		{
			return new[] { "H", "S", "B" };
		}
	}

	public class HexColorTextModel : ITextOnlyColorModel
	{
		public string[] GetComponentsForColor(AxoColor color, IFormatProvider formatProvider)
		{
			return new string[] { string.Format(formatProvider, "#{0:X02}{1:X02}{2:X02}", color.R, color.G, color.B) };
		}

		public string[] GetNamesOfComponents()
		{
			return new[] { "Hex" };
		}
	}
}