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
//    along with this program; if not, write to the Free Software
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

namespace Altaxo.Gui.Drawing.ColorManagement
{
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

			var br = baseColor.ScR;
			var bg = baseColor.ScG;
			var bb = baseColor.ScB;

			double hw, h, w;
			if (br != bg)
			{
				hw = (r - g) / (br - bg);
				w = 1 - r + br * hw;
				h = hw / w;
			}
			else if (br != bb)
			{
				hw = (r - b) / (br - bb);
				w = 1 - r + br * hw;
				h = hw / w;
			}
			else if (bg != bb)
			{
				hw = (g - b) / (bg - bb);
				w = 1 - r + bg * hw;
				h = hw / w;
			}
			else
			{
				throw new NotImplementedException();
			}

			return new Tuple<PointD2D, double>(new PointD2D(1 - w, 1 - h), ahsb.Item2);
		}
	}
}