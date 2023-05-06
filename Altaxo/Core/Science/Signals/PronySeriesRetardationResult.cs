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
using System.Collections.Generic;
using System.Linq;
using Complex64 = System.Numerics.Complex;

namespace Altaxo.Science.Signals
{
  /// <summary>
  /// Represents the result of a prony series fit to a relaxation process, for instance a modulus, either
  /// in time or in frequency domain.
  /// </summary>
  public record PronySeriesRetardationResult

  {
    public PronySeriesRetardationResult(IReadOnlyList<double> relaxationTimes, IReadOnlyList<double> pronyCoefficients, IReadOnlyList<double> relaxationDensities, double? flowTerm)
    {
      RetardationTimes = relaxationTimes ?? throw new ArgumentNullException(nameof(relaxationTimes));
      PronyCoefficients = pronyCoefficients ?? throw new ArgumentNullException(nameof(pronyCoefficients));
      RetardationDensities = relaxationDensities ?? throw new ArgumentNullException(nameof(relaxationDensities));

      ComplianceLowFrequency = PronyCoefficients.Sum();

      if (flowTerm.HasValue)
      {
        Fluidity = flowTerm.Value;
        Viscosity = 1 / flowTerm.Value;
      }
      else
      {
        Fluidity = double.NaN;
        Viscosity = double.NaN;
      }
    }

    /// <summary>
    /// Gets the retardation times.
    /// </summary>
    public IReadOnlyList<double> RetardationTimes { get; init; }

    /// <summary>
    /// Gets the prony coefficients, corresponding to the <see cref="RetardationTimes"/>.
    /// </summary>
    public IReadOnlyList<double> PronyCoefficients { get; init; }

    /// <summary>
    /// Gets the relaxation densities, corresponding to the <see cref="RetardationTimes"/>.
    /// </summary>
    public IReadOnlyList<double> RetardationDensities { get; init; }

    /// <summary>
    /// Gets the high frequency compliance.(the last element of the Prony coefficient array).
    /// </summary>
    public double ComplianceHighFrequency { get => PronyCoefficients[0]; }

    /// <summary>
    /// Gets the low frequency compliance.  This is the sum of all prony coefficients.
    /// </summary>
    public double ComplianceLowFrequency { get; }

    /// <summary>
    /// Gets the flow term value, which is fluidity in case of strain retardation.
    /// </summary>
    public double Fluidity { get; }

    /// <summary>
    /// Gets the inverse of the flow term value, i.e. 1 / <see cref="Fluidity"/>.
    /// </summary>
    public double Viscosity { get; }

    /// <summary>
    /// Gets (in the time domain) the y-value in dependence on x.
    /// </summary>
    /// <param name="t">The x-value (time).</param>
    /// <returns>The y-value in the time domain at time x.</returns>
    public double GetTimeDomainYOfTime(double t)
    {
      double sum = 0;
      for (int i = 0; i < PronyCoefficients.Count; ++i)
      {
        var tau = RetardationTimes[i];
        sum += tau == 0 ? PronyCoefficients[i] : PronyCoefficients[i] * (1 - Math.Exp(-t / RetardationTimes[i]));
      }
      return sum;
    }

    /// <summary>
    /// Gets (in the frequency domain) the y-value in dependence on the circular frequency.
    /// </summary>
    /// <param name="w">The circular frequency.</param>
    /// <returns>The y-value in the frequency domain. Note that because it is a compliance, the imaginary part is negative.</returns>
    public Complex64 GetFrequencyDomainYOfOmega(double w)
    {
      Complex64 sum = 0;
      for (int i = 0; i < PronyCoefficients.Count; ++i)
      {
        if (double.IsPositiveInfinity(RetardationTimes[i]))
        {
          sum += PronyCoefficients[i];
        }
        else
        {
          var tauomega = w * RetardationTimes[i];
          sum += PronyCoefficients[i] / (1 - tauomega * Complex64.ImaginaryOne);
        }
      }
      return sum;
    }

    /// <summary>
    /// Gets (in the frequency domain) the y-value in dependence on the frequency.
    /// </summary>
    /// <param name="f">The frequency.</param>
    /// <returns>The y-value in the frequency domain. Note that because it is a compliance, the imaginary part is negative.</returns>
    public Complex64 GetFrequencyDomainYOfFrequency(double f)
    {
      return GetFrequencyDomainYOfOmega(f * 2 * Math.PI);
    }
  }

}
