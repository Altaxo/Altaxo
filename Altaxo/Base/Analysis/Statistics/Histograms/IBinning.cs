#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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

namespace Altaxo.Analysis.Statistics.Histograms
{
  /// <summary>
  /// Interface to a binning (for instance <see cref="LinearBinning"/> or <see cref="LogarithmicBinning"/>.
  /// </summary>
  public interface IBinning : ICloneable
  {
    /// <summary>
    /// Gets the bins.
    /// </summary>
    /// <value>
    /// The bins.
    /// </value>
    IReadOnlyList<Bin> Bins { get; }

    /// <summary>
    /// Calculates the bin positions, the width of the bins and the number of bins from a sorted list containing the data ensemble.
    /// This does not calculate the bins itself. To do this, use <see cref="CalculateBinsFromSortedList"/>
    /// </summary>
    /// <param name="sortedList">The sorted list.</param>
    void CalculateBinPositionsFromSortedList(IReadOnlyList<double> sortedList);

    /// <summary>
    /// Calculates the bins from sorted list containing the data ensemble.
    /// </summary>
    /// <param name="sortedListOfData">The sorted list of data.</param>
    void CalculateBinsFromSortedList(IReadOnlyList<double> sortedListOfData);
  }
}
