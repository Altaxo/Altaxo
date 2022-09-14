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

using System;
using System.Collections.Generic;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Science.Spectroscopy;
using Altaxo.Science.Spectroscopy.EnsembleMeanScale;

namespace Altaxo.Calc.Regression.Multivariate
{
  /// <summary>
  /// Contains the basic data that where obtained during preprocessing.
  /// </summary>
  public interface IMultivariatePreprocessingModel
  {
    int[] SpectralRegions
    {
      get;
    }

    System.Collections.Generic.IReadOnlyList<double> XOfX
    {
      get;
    }

    IReadOnlyList<double> XMean
    {
      get;
    }

    IReadOnlyList<double> XScale
    {
      get;
    }

    IReadOnlyList<double> YMean
    {
      get;
    }

    IReadOnlyList<double> YScale
    {
      get;
    }

    ISingleSpectrumPreprocessor PreprocessSingleSpectrum
    {
      get;
    }

    IEnsembleMeanScalePreprocessor PreprocessEnsembleOfSpectra { get; }
  }

  public class MultivariatePreprocessingModel : IMultivariatePreprocessingModel
  {
#nullable disable
    ISingleSpectrumPreprocessor _preprocessSingleSpectrum;
    IEnsembleMeanScalePreprocessor _preprocessEnsembleOfSpectra;

    private int[] _spectralRegions;
    private System.Collections.Generic.IReadOnlyList<double> _xOfX;
    private IReadOnlyList<double> _xMean;
    private IReadOnlyList<double> _xScale;
    private IReadOnlyList<double> _yMean;
    private IReadOnlyList<double> _yScale;
#nullable enable

    public ISingleSpectrumPreprocessor PreprocessSingleSpectrum
    {
      get { return _preprocessSingleSpectrum; }
      set { _preprocessSingleSpectrum = value; }
    }

    public IEnsembleMeanScalePreprocessor PreprocessEnsembleOfSpectra
    {
      get { return _preprocessEnsembleOfSpectra; }
      set { _preprocessEnsembleOfSpectra = value; }
    }

    public int[] SpectralRegions
    {
      get { return _spectralRegions; }
      set { _spectralRegions = value; }
    }

    public System.Collections.Generic.IReadOnlyList<double> XOfX
    {
      get { return _xOfX; }
      set { _xOfX = value; }
    }

    public IReadOnlyList<double> XMean
    {
      get { return _xMean; }
      set { _xMean = value; }
    }

    public IReadOnlyList<double> XScale
    {
      get { return _xScale; }
      set { _xScale = value; }
    }

    public IReadOnlyList<double> YMean
    {
      get { return _yMean; }
      set { _yMean = value; }
    }

    public IReadOnlyList<double> YScale
    {
      get { return _yScale; }
      set { _yScale = value; }
    }
  }
}
