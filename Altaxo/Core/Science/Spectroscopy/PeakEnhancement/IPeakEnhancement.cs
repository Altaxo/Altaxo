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
using Altaxo.Science.Spectroscopy.PeakSearching;

namespace Altaxo.Science.Spectroscopy.PeakEnhancement
{
  /// <summary>
  /// Interface to peak enhancement methods. These methods will enhance peaks: if two peaks are close together, or one peak is hidden in the shoulder of another peak, it tries
  /// to separate those peaks. Most properties of the peaks are not kept: the height, width, shape of the peak may differ from the original peak, but at least the position should be that
  /// of the original peak.
  /// </summary>
  public interface IPeakEnhancement : ISingleSpectrumPreprocessor
  {
    /// <summary>
    /// Adjusts the parameters of this peak enhancement method by using the spectrum, and the result of a regular peak search over the spectrum.
    /// </summary>
    /// <param name="subX">The x-values of the spectrum.</param>
    /// <param name="subY">The y-values of the spectrum.</param>
    /// <param name="resultRegular">The result of a peak search over the spectrum.</param>
    /// <returns>An instance of the peak search with adjusted parameters. If nothing is changed, the same instance can be returned.</returns>
    IPeakEnhancement WithAdjustedParameters(double[] subX, double[] subY, List<PeakDescription> resultRegular);
  }
}
