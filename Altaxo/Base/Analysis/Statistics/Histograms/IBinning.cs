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
