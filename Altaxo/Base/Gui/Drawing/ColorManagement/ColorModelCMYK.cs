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
	public class ColorModelCMYK : IColorModel
	{
		public AxoColor GetColorFor1DColorSurfaceFromRelativePosition(double relativePosition)
		{
			return AxoColor.FromAHSB(255, (float)(relativePosition), 1, 1);
		}

		public AxoColor GetColorFor2DColorSurfaceFromRelativePosition(PointD2D relativePosition, AxoColor c)
		{
			return AxoColor.FromAHSB(255, c.GetHue(), (float)(relativePosition.X), (float)(relativePosition.Y));
		}

		public (double position1D, PointD2D position2D) GetRelativePositionsFor1Dand2DColorSurfaceFromColor(AxoColor color)
		{
			var (alpha, hue, saturation, brightness) = color.ToAHSB();
			return (hue, new PointD2D(saturation, brightness));
		}

		public int[] GetComponentsForColor(AxoColor color)
		{
			var (a, c, m, y, k) = color.ToACMYK();

			return new int[] { AxoColor.NormFloatToByte(c), AxoColor.NormFloatToByte(m), AxoColor.NormFloatToByte(y), AxoColor.NormFloatToByte(k) };
		}

		public AxoColor GetColorFromComponents(int[] components)
		{
			return AxoColor.FromACMYK(1, AxoColor.ByteToNormFloat((byte)components[0]), AxoColor.ByteToNormFloat((byte)components[1]), AxoColor.ByteToNormFloat((byte)components[2]), AxoColor.ByteToNormFloat((byte)components[3]));
		}

		public string[] GetNamesOfComponents()
		{
			return new string[] { "C", "M", "Y", "K" };
		}
	}
}