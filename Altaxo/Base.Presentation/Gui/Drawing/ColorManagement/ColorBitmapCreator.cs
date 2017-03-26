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

using Altaxo.Drawing;
using Altaxo.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Altaxo.Gui.Drawing.ColorManagement
{
	public static class ColorBitmapCreator
	{
		public static BitmapSource GetBitmap(Func<PointD2D, AxoColor> colorFunction)
		{
			const int width = 32;
			const int height = 32;

			WriteableBitmap wbitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
			byte[] pixels = new byte[height * width * 4];

			for (int row = 0; row < height; ++row)
			{
				var h = (height - 1 - row) / (float)(height - 1); // h is 0 on the lower edge, 1 on the upper edge
				for (int col = 0; col < width; ++col)
				{
					var w = (col) / (float)(width - 1); // w is 0 on the left edge, 1 on the right edge
					var cc = colorFunction(new PointD2D(w, h));

					int idx = (row * width + col) * 4;
					pixels[idx + 0] = cc.B;  // B
					pixels[idx + 1] = cc.G; // G
					pixels[idx + 2] = cc.R; // R
					pixels[idx + 3] = 255;  // A
				}
			}

			// Update writeable bitmap with the colorArray to the image.
			Int32Rect rect = new Int32Rect(0, 0, width, height);
			int stride = 4 * width;
			wbitmap.WritePixels(rect, pixels, stride, 0);

			return wbitmap;
		}
	}
}