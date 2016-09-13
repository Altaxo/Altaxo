#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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
using Altaxo.Drawing.D3D;
using Altaxo.Geometry;
using Altaxo.Graph.Graph3D.GraphicsContext;
using Altaxo.Graph.Graph3D.Plot.Data;
using Altaxo.Graph.Plot.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Graph3D.Plot.Styles.LineConnectionStyles
{
	/// <summary>
	/// Represents a symbol shape for a 3D scatter plot. Instances of this class have to be immutable.
	/// This base class implements Equals and GetHashCode.
	/// </summary>
	/// <seealso cref="Altaxo.Main.IImmutable" />
	public abstract class LineConnectionStyleBase : ILineConnectionStyle
	{
		/// <summary>
		/// Template to make a line draw.
		/// </summary>
		/// <param name="g">Graphics context.</param>
		/// <param name="pdata">The plot data. Don't use the Range property of the pdata, since it is overriden by the next argument.</param>
		/// <param name="range">The plot range to use.</param>
		/// <param name="layer">Graphics layer.</param>
		/// <param name="pen">The pen to draw the line.</param>
		/// <param name="symbolGap">The size of the symbol gap. Argument is the original index of the data. The return value is the absolute symbol gap at this index.
		/// This function is null if no symbol gap is required.</param>
		/// <param name="connectCircular">If true, the end of the line is connected with the start of the line.</param>
		public abstract void Paint(IGraphicsContext3D g, Processed3DPlotData pdata, PlotRange range, IPlotArea layer, PenX3D pen, Func<int, double> symbolGap, bool connectCircular);

		public override int GetHashCode()
		{
			return this.GetType().GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return this.GetType() == obj?.GetType();
		}
	}
}