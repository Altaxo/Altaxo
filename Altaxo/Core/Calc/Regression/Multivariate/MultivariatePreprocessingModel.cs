#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
using Altaxo.Science.Spectroscopy;
using Altaxo.Science.Spectroscopy.EnsembleProcessing;

namespace Altaxo.Calc.Regression.Multivariate
{
  /// <summary>
  /// Contains the basic data that were obtained during preprocessing.
  /// </summary>
  public interface IMultivariatePreprocessingModel
  {
    /// <summary>
    /// Gets the spectral regions used in preprocessing.
    /// </summary>
    int[] SpectralRegions
    {
      get;
    }

    /// <summary>
    /// Gets the <c>X</c> axis values corresponding to the columns of the (unprocessed) spectral matrix.
    /// </summary>
    System.Collections.Generic.IReadOnlyList<double> XOfX
    {
      get;
    }

    /// <summary>
    /// Gets the mean values of <c>X</c> used for centering during preprocessing.
    /// </summary>
    IReadOnlyList<double> XMean
    {
      get;
    }

    /// <summary>
    /// Gets the scaling factors of <c>X</c> used during preprocessing.
    /// </summary>
    IReadOnlyList<double> XScale
    {
      get;
    }

    /// <summary>
    /// Gets the mean values of <c>Y</c> used for centering during preprocessing.
    /// </summary>
    IReadOnlyList<double> YMean
    {
      get;
    }

    /// <summary>
    /// Gets the scaling factors of <c>Y</c> used during preprocessing.
    /// </summary>
    IReadOnlyList<double> YScale
    {
      get;
    }

    /// <summary>
    /// Gets the preprocessing operation applied to a single spectrum.
    /// </summary>
    ISingleSpectrumPreprocessor PreprocessSingleSpectrum
    {
      get;
    }

    /// <summary>
    /// Gets the preprocessing operation applied to an ensemble of spectra (mean/scale preprocessing).
    /// </summary>
    IEnsembleMeanScalePreprocessor PreprocessEnsembleOfSpectra { get; }
  }

  /// <summary>
  /// Default implementation of <see cref="IMultivariatePreprocessingModel"/>.
  /// </summary>
  public class MultivariatePreprocessingModel : IMultivariatePreprocessingModel
  {
#nullable disable
    /// <summary>
    /// Backing field for <see cref="PreprocessSingleSpectrum"/>.
    /// </summary>
    ISingleSpectrumPreprocessor _preprocessSingleSpectrum;

    /// <summary>
    /// Backing field for <see cref="PreprocessEnsembleOfSpectra"/>.
    /// </summary>
    IEnsembleMeanScalePreprocessor _preprocessEnsembleOfSpectra;

    private int[] _spectralRegions;
    private System.Collections.Generic.IReadOnlyList<double> _xOfX;
    private IReadOnlyList<double> _xMean;
    private IReadOnlyList<double> _xScale;
    private IReadOnlyList<double> _yMean;
    private IReadOnlyList<double> _yScale;
#nullable enable

    /// <inheritdoc/>
    public ISingleSpectrumPreprocessor PreprocessSingleSpectrum
    {
      get { return _preprocessSingleSpectrum; }
      set { _preprocessSingleSpectrum = value; }
    }

    /// <inheritdoc/>
    public IEnsembleMeanScalePreprocessor PreprocessEnsembleOfSpectra
    {
      get { return _preprocessEnsembleOfSpectra; }
      set { _preprocessEnsembleOfSpectra = value; }
    }

    /// <inheritdoc/>
    public int[] SpectralRegions
    {
      get { return _spectralRegions; }
      set { _spectralRegions = value; }
    }

    /// <inheritdoc/>
    public System.Collections.Generic.IReadOnlyList<double> XOfX
    {
      get { return _xOfX; }
      set { _xOfX = value; }
    }

    /// <inheritdoc/>
    public IReadOnlyList<double> XMean
    {
      get { return _xMean; }
      set { _xMean = value; }
    }

    /// <inheritdoc/>
    public IReadOnlyList<double> XScale
    {
      get { return _xScale; }
      set { _xScale = value; }
    }

    /// <inheritdoc/>
    public IReadOnlyList<double> YMean
    {
      get { return _yMean; }
      set { _yMean = value; }
    }

    /// <inheritdoc/>
    public IReadOnlyList<double> YScale
    {
      get { return _yScale; }
      set { _yScale = value; }
    }
  }
}
