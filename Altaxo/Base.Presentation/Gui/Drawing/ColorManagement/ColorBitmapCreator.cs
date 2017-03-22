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
	public interface IColorModel
	{
		AxoColor GetColorFor2DColorSurfaceFromRelativePosition(PointD2D relativePosition, AxoColor baseColor);

		AxoColor GetColorFor1DColorSurfaceFromRelativePosition(double relativePosition);

		Tuple<PointD2D, double> GetRelativePositionsFor2Dand1DColorSurfaceFromColor(AxoColor color);

		string[] GetNamesOfComponents();

		int[] GetComponentsForColor(AxoColor color);

		AxoColor GetColorFromComponents(int[] components);
	}

	public class ColorModelRGB : IColorModel
	{
		public AxoColor GetColorFor1DColorSurfaceFromRelativePosition(double relativePosition)
		{
			return AxoColor.FromAHSB(255, (float)relativePosition, 1, 1);
		}

		public AxoColor GetColorFor2DColorSurfaceFromRelativePosition(PointD2D relativePosition, AxoColor c)
		{
			float h = (float)(1 - relativePosition.Y);
			var w = (float)(1 - relativePosition.X);
			var bc = AxoColor.FromScRgb(1, c.ScR * h, c.ScG * h, c.ScB * h);
			return AxoColor.FromScRgb(1, bc.ScR * w + (1 - w), bc.ScG * w + (1 - w), bc.ScB * w + (1 - w));
		}

		public int[] GetComponentsForColor(AxoColor color)
		{
			return new int[] { color.R, color.G, color.B };
		}

		public AxoColor GetColorFromComponents(int[] components)
		{
			return AxoColor.FromArgb(255, (byte)components[0], (byte)components[1], (byte)components[2]);
		}

		public string[] GetNamesOfComponents()
		{
			return new string[] { "R", "G", "B" };
		}

		public Tuple<PointD2D, double> GetRelativePositionsFor2Dand1DColorSurfaceFromColor(AxoColor color)
		{
			var ahsb = color.ToAHSB();
			var baseColor = AxoColor.FromAHSB(255, ahsb.Item2, 1, 1);

			var r = color.ScR;
			var g = color.ScG;
			var b = color.ScB;

			double hw, h, w;
			if (r != g)
			{
				hw = (r - g) / (baseColor.ScR - baseColor.ScG);
				w = 1 - r + baseColor.ScR * hw;
				h = hw / w;
			}
			else
				throw new NotImplementedException();

			return new Tuple<PointD2D, double>(new PointD2D(1 - w, 1 - h), ahsb.Item2);
		}
	}

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
				var h = (height - 1 - row) / (float)(height - 1);
				for (int col = 0; col < width; ++col)
				{
					int idx = (row * width + col) * 4;
					var w = (col) / (float)(width - 1);

					var cc = colorFunction(new PointD2D(w, h));

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