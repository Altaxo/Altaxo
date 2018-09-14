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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Drawing;
using Altaxo.Geometry;

namespace Altaxo.Gui.Drawing.ColorManagement
{
  public class ColorModelCMYK : IColorModel
  {
    public AxoColor GetColorFor1DColorSurfaceFromRelativePosition(double relativePosition)
    {
      return AxoColor.FromAhsb(1, (float)(relativePosition), 1, 1);
    }

    public AxoColor GetColorFor2DColorSurfaceFromRelativePosition(PointD2D relativePosition, AxoColor c)
    {
      return AxoColor.FromAhsb(1, c.GetHue(), (float)(relativePosition.X), (float)(relativePosition.Y));
    }

    public (double position1D, PointD2D position2D) GetRelativePositionsFor1Dand2DColorSurfaceFromColor(AxoColor color)
    {
      var (alpha, hue, saturation, brightness) = color.ToAhsb();
      return (hue, new PointD2D(saturation, brightness));
    }

    public bool IsUsingByteComponents { get { return false; } }

    public double[] GetComponentsForColor(AxoColor color)
    {
      var (a, c, m, y, k) = color.ToAcmyk();

      return new double[] { c, m, y, k };
    }

    public AxoColor GetColorFromComponents(double[] components)
    {
      return AxoColor.FromAcmyk(1, (float)components[0], (float)components[1], (float)components[2], (float)components[3]);
    }

    public string[] GetNamesOfComponents()
    {
      return new string[] { "C", "M", "Y", "K" };
    }
  }
}
