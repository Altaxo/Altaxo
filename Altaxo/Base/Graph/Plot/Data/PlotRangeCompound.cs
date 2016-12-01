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
	/// Compound of plot ranges that can be treated as a single plot range, because it implements <see cref="IPlotRange"/>.
	/// </summary>
	/// <seealso cref="Altaxo.Graph.Plot.Data.IPlotRange" />
	public class PlotRangeCompound : IPlotRange
	{
		private IPlotRange[] _ranges;

		public PlotRangeCompound(IEnumerable<IPlotRange> ranges)
		{
			if (null == ranges)
				throw new ArgumentNullException(nameof(ranges));

			_ranges = ranges.ToArray();

			if (_ranges.Length == 0)
				throw new ArgumentOutOfRangeException(nameof(ranges), "must not be empty");
		}

		/// <inheritdoc/>
		public int Length
		{
			get
			{
				return _ranges[_ranges.Length - 1].UpperBound - _ranges[0].LowerBound;
			}
		}

		/// <inheritdoc/>
		public int LowerBound
		{
			get
			{
				return _ranges[0].LowerBound;
			}
		}

			int _lastPlotPointIndex;
		int _lastRangeIndex;
		public int GetOriginalRowIndexFromPlotPointIndex(int plotPointIndex)
		{
			if (plotPointIndex >= _lastPlotPointIndex)
			{
				if (plotPointIndex < _ranges[_lastRangeIndex].UpperBound)
				{
					return _ranges[_lastRangeIndex].GetOriginalRowIndexFromPlotPointIndex(plotPointIndex);
				}

				for (int i = _lastRangeIndex + 1; i < _ranges.Length; ++i)
				{
					if (plotPointIndex < _ranges[i].UpperBound)
					{
						_lastRangeIndex = i;
						_lastPlotPointIndex = plotPointIndex;
						return _ranges[i].GetOriginalRowIndexFromPlotPointIndex(plotPointIndex);
					}
				}
			}
			else
			{
				for (int i = 0; i < _ranges.Length; ++i)
				{
					if (plotPointIndex < _ranges[i].UpperBound)
					{
						_lastRangeIndex = i;
						_lastPlotPointIndex = plotPointIndex;
						return _ranges[i].GetOriginalRowIndexFromPlotPointIndex(plotPointIndex);
					}
				}
			}

			throw new ArgumentOutOfRangeException(nameof(plotPointIndex), "is above the range range");

		}

		/// <inheritdoc/>
		public int OriginalFirstPoint
		{
			get
			{
				return _ranges[0].OriginalFirstPoint;
			}
		}

		public int OriginalLastPoint
		{
			get
			{
				return _ranges[_ranges.Length - 1].OriginalLastPoint;
			}
		}

		/// <inheritdoc/>
		public int UpperBound
		{
			get
			{
				return _ranges[_ranges.Length - 1].UpperBound;
			}
		}

		/// <inheritdoc/>
		public IPlotRange WithUpperBoundShortenedBy(int count)
		{
			if (!(count >= 0))
				throw new ArgumentOutOfRangeException(nameof(count), "must be >=0");

			if (0 == count)
				return this;

			int remaining = count;

			int i = _ranges.Length;

			while (remaining > 0 && i > 0)
			{
				--i;
				if (_ranges[i].Length <= remaining)
					remaining -= _ranges[i].Length;
				else
					break;
			}

			if (i == 0)
				return _ranges[0].WithUpperBoundShortenedBy(remaining);
			else
			{
				return new PlotRangeCompound(_ranges.Take(i - 1).Concat(new[] { _ranges[i].WithUpperBoundShortenedBy(remaining) }));
			}
		}
	}
}