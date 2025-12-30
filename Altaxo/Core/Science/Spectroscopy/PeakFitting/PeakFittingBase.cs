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

using System;
using Altaxo.Calc.FitFunctions.Peaks;
using Altaxo.Calc.FitFunctions.Probability;

namespace Altaxo.Science.Spectroscopy.PeakFitting
{
  /// <summary>
  /// Base settings used for peak fitting.
  /// </summary>
  public record class PeakFittingBase
  {
    private IFitFunctionPeak _fitFunction = new VoigtAreaParametrizationNu();

    /// <summary>
    /// Gets/sets the fit function to use.
    /// </summary>
    public IFitFunctionPeak FitFunction
    {
      get { return _fitFunction; }
      init { _fitFunction = value ?? throw new ArgumentNullException(nameof(FitFunction)); }
    }

    /// <summary>
    /// Gets/sets the scaling factor of the fit width.
    /// To calculate the fit width, the scaling factor is multiplied by the full width at half maximum (FWHM) of the peak.
    /// </summary>
    private double _fitWidthScalingFactor = 2;

    /// <summary>
    /// Gets/sets the scaling factor of the fit width.
    /// To calculate the fit width, the scaling factor is multiplied by the full width at half maximum (FWHM) of the peak.
    /// </summary>
    public double FitWidthScalingFactor
    {
      get
      {
        return _fitWidthScalingFactor;
      }
      init
      {
        if (!(value > 0))
          throw new ArgumentOutOfRangeException("Factor has to be > 0", nameof(FitWidthScalingFactor));

        _fitWidthScalingFactor = value;
      }
    }

    private double _minimalFWHMValue;

    /// <summary>
    /// Gets/sets the minimal allowed FWHM value.
    /// </summary>
    public double MinimalFWHMValue
    {
      get
      {
        return _minimalFWHMValue;
      }
      init
      {
        if (!(value >= 0))
          throw new ArgumentOutOfRangeException("Value has to be >= 0", nameof(MinimalFWHMValue));

        _minimalFWHMValue = value;
      }
    }

    /// <summary>
    /// Gets a value indicating whether <see cref="MinimalFWHMValue"/> is specified in x-units.
    /// </summary>
    public bool IsMinimalFWHMValueInXUnits { get; init; } = true;
  }
}
