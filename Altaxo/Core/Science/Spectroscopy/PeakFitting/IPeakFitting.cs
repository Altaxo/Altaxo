#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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


using System.Collections.Generic;
using System.Threading;

namespace Altaxo.Science.Spectroscopy.PeakFitting
{
  /// <summary>
  /// Interface to peak searching methods.
  /// </summary>
  public interface IPeakFitting
  {
    /// <summary>
    /// Executes the normalization algorithm.
    /// </summary>
    /// <param name="xArray">The array of x-values.</param>
    /// <param name="yArray">The array of y-values.</param>
    /// <param name="peakDescriptions">Description of the peaks (output of peak searching algorithms, see <see cref="PeakSearching.IPeakSearching"/>).</param>
    /// <param name="cancellationToken">Token used to cancel this task.</param>
    /// <returns>The results of the peak fitting. For each spectral regions, a tuple of the peak descriptions in that range, together
    /// with the start and end index (exclusive) of that range is returned. Please note that the peak descriptions
    /// contain position indices that are relative to the corresponding range (thus not to the underlying spectral array).</returns>
    IReadOnlyList<(IReadOnlyList<PeakDescription> PeakDescriptions, int StartOfRegion, int EndOfRegion)> Execute(double[] xArray, double[] yArray, IReadOnlyList<(IReadOnlyList<PeakSearching.PeakDescription> PeakDescriptions, int StartOfRegion, int EndOfRegion)> peakDescriptions, CancellationToken cancellationToken);
  }
}
