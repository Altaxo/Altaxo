#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2023 Dr. Dirk Lellinger
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

namespace Altaxo.Science.Spectroscopy.BaselineEstimation
{
  /// <summary>
  /// Interface for baseline estimation algorithms for simple (1D) spectra.
  /// </summary>
  public interface IBaselineEstimation : ISingleSpectrumPreprocessor
  {
    /// <summary>
    /// Executes the baseline estimation algorithm for the provided spectrum.
    /// </summary>
    /// <param name="xArray">The x-values of the spectrum.</param>
    /// <param name="yArray">The y-values of the spectrum.</param>
    /// <param name="resultingBaseline">The destination to which the estimated baseline is written.</param>
    public abstract void Execute(ReadOnlySpan<double> xArray, ReadOnlySpan<double> yArray, Span<double> resultingBaseline);
  }
}
