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
using Altaxo.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Gdi.Plot.Styles
{
	/// <summary>
	/// Represents a symbol shape for a 3D scatter plot. Instances of this class have to be immutable. They still need to be cloneable,
	/// because in a list of scatter symbols we need unique instances.
	/// </summary>
	/// <seealso cref="Altaxo.Main.IImmutable" />
	public interface IScatterSymbol : Main.IImmutable, ICloneable
	{
		ScatterSymbols.IScatterSymbolFrame Frame { get; }
		ScatterSymbols.IScatterSymbolInset Inset { get; }

		NamedColor FillColor { get; }

		void CalculatePolygons(
		out List<List<ClipperLib.IntPoint>> framePolygon,
		out List<List<ClipperLib.IntPoint>> insetPolygon,
		out List<List<ClipperLib.IntPoint>> fillPolygon);
	}
}