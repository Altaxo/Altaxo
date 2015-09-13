#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2015 Dr. Dirk Lellinger
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
using System.Threading.Tasks;

namespace Altaxo.Graph3D
{
	public class FontManager3D
	{
		private static Bitmap _bmp = new Bitmap(16, 16);
		private static Graphics _graphics;

		static FontManager3D()
		{
			_bmp = new Bitmap(16, 16);
			_graphics = Graphics.FromImage(_bmp);
		}

		public static VectorD3D MeasureString(string text, FontX3D font, StringFormat format)
		{
			var size = _graphics.MeasureString(text, Altaxo.Graph.Gdi.GdiFontManager.ToGdi(font.Font), new PointF(0, 0), format);
			return new VectorD3D(size.Width, size.Height, font.Depth);
		}

		public static FontInfo GetFontInformation(FontX3D font)
		{
			// get some properties of the font
			var gdiFont = Altaxo.Graph.Gdi.GdiFontManager.ToGdi(font.Font);
			double size = gdiFont.Size;
			double cyLineSpace = gdiFont.GetHeight(_graphics); // space between two lines
			int iCellSpace = gdiFont.FontFamily.GetLineSpacing(gdiFont.Style);
			int iCellAscent = gdiFont.FontFamily.GetCellAscent(gdiFont.Style);
			int iCellDescent = gdiFont.FontFamily.GetCellDescent(gdiFont.Style);
			double cyAscent = cyLineSpace * iCellAscent / iCellSpace;
			double cyDescent = cyLineSpace * iCellDescent / iCellSpace;

			return new FontInfo(cyLineSpace, cyAscent, cyDescent, size);
		}
	}
}