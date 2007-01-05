#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

using System;
using System.Collections.Generic;
using System.Text;

using Altaxo.Graph.Scales.Boundaries;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Plot;


namespace Altaxo.Graph.Gdi.Plot.Groups
{
  public class CoordinateTransformingStyleBase
  {
    public static void MergeXBoundsInto(IPhysicalBoundaries pb, PlotItemCollection coll)
    {
      foreach (IGPlotItem pi in coll)
      {
        if (pi is IXBoundsHolder)
        {
          IXBoundsHolder plotItem = (IXBoundsHolder)pi;
          plotItem.MergeXBoundsInto(pb);
        }
      }
    }

    public static void MergeYBoundsInto(IPhysicalBoundaries pb, PlotItemCollection coll)
    {
      foreach (IGPlotItem pi in coll)
      {
        if (pi is IYBoundsHolder)
        {
          IYBoundsHolder plotItem = (IYBoundsHolder)pi;
          plotItem.MergeYBoundsInto(pb);
        }
      }
    }

    public static void Paint(System.Drawing.Graphics g, IPlotArea layer, PlotItemCollection coll)
    {
      foreach (IGPlotItem pi in coll)
        pi.Paint(g, layer);
    }


  }
}
