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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Graph.Plot.Data
{
	/// <summary>
	/// Interface to a plot range. A plot range designates a range of indices in the array of already converted plot points (converted to layer coordinates), together
	/// with information about the original data rows that correspond to the data.
	/// </summary>
	public interface IPlotRange
	{
		/// <summary>
		/// Number of points in this plot range.
		/// </summary>
		int Length { get; }

		/// <summary>
		/// First index of a contiguous plot range in the plot point array (i.e. in the array of processed plot point data, <b>not</b> in the original data column).
		/// To calculate from which row index in the original data column this comes from, add to this value <see cref="OffsetToOriginal"/>.
		/// </summary>
		int LowerBound { get; }

		/// <summary>
		/// Last index + 1 of a contiguous plot range in the plot point array (i.e. in the array of processed plot point data, <b>not</b> in the original data column).
		/// To calculate from which row index in the original data column this comes from, add to this value <see cref="OffsetToOriginal"/>.
		/// </summary>
		int UpperBound { get; }

		/// <summary>
		/// Shortens the upper boundary by the number of points provided in the parameter.
		/// </summary>
		/// <param name="count">The number of points to shorten the upper bound. Must be greater than or equal to 0.</param>
		/// <returns>A new plot range where the upper boundary is shortened.</returns>
		IPlotRange WithUpperBoundShortenedBy(int count);

		/// <summary>
		/// Row index of the first point of this plot range in the original data column.
		/// </summary>
		int OriginalFirstPoint { get; }

		/// <summary>
		/// Row index of the last point of this plot range in the original data column.
		/// </summary>
		int OriginalLastPoint { get; }
	}
}