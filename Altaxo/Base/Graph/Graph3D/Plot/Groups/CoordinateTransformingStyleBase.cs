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

using Altaxo.Graph.Scales.Boundaries;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Graph.Graph3D.Plot.Groups
{
	using GraphicsContext;

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

		public static void MergeZBoundsInto(IPhysicalBoundaries pb, PlotItemCollection coll)
		{
			foreach (IGPlotItem pi in coll)
			{
				if (pi is IZBoundsHolder)
				{
					IZBoundsHolder plotItem = (IZBoundsHolder)pi;
					plotItem.MergeZBoundsInto(pb);
				}
			}
		}

		public static void Paint(IGraphicContext3D g, Altaxo.Graph.IPaintContext paintContext, IPlotArea layer, PlotItemCollection coll)
		{
			for (int i = coll.Count - 1; i >= 0; --i)
			{
				coll[i].Paint(g, paintContext, layer, i == coll.Count - 1 ? null : coll[i + 1], i == 0 ? null : coll[i - 1]);
			}
		}
	}
}