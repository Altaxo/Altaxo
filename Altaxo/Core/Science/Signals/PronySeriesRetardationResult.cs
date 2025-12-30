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
  /// Represents the result of a Prony series fit to a retardation process (general susceptibility),
  /// for instance a mechanical compliance, either in the time domain or in the frequency domain.
  /// </summary>
  public record PronySeriesRetardationResult
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="PronySeriesRetardationResult"/> record.
    /// </summary>
    /// <param name="retardationTimes">
    /// The retardation times.
    /// The list may include <c>0</c> if an intercept/high-frequency term is present.
    /// </param>
    /// <param name="pronyCoefficients">The Prony coefficients corresponding to <paramref name="retardationTimes"/>.</param>
    /// <param name="relaxationDensities">The retardation density values corresponding to <paramref name="retardationTimes"/>.</param>
    /// <param name="flowTerm">
    /// The flow term (fluidity/conductivity). If <see langword="null"/>, no flow term was fitted.
    /// </param>
    /// <param name="isFlowTermFromDielectricSpectrum">
    /// If set to <c>true</c>, the Prony series is the result of a fit to a dielectric spectrum.
    /// In this case, the electrical conductivity is calculated using the vacuum permittivity as a factor.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="retardationTimes"/>, <paramref name="pronyCoefficients"/>, or <paramref name="relaxationDensities"/> is <see langword="null"/>.
    /// </exception>
    public PronySeriesRetardationResult(IReadOnlyList<double> retardationTimes, IReadOnlyList<double> pronyCoefficients, IReadOnlyList<double> relaxationDensities, double? flowTerm, bool isFlowTermFromDielectricSpectrum)
    {
      RetardationTimes = retardationTimes ?? throw new ArgumentNullException(nameof(retardationTimes));
      PronyCoefficients = pronyCoefficients ?? throw new ArgumentNullException(nameof(pronyCoefficients));
      RetardationDensities = relaxationDensities ?? throw new ArgumentNullException(nameof(relaxationDensities));

      SusceptibilityLowFrequency = PronyCoefficients.Sum();

      if (flowTerm.HasValue)
      {
        Fluidity = flowTerm.Value;
        Viscosity = 1 / flowTerm.Value;
        SpecificElectricalConductivity = isFlowTermFromDielectricSpectrum ? flowTerm.Value * SIConstants.VACUUM_PERMITTIVITY : double.NaN; ;
        SpecificElectricalResistivity = isFlowTermFromDielectricSpectrum ? 1 / (flowTerm.Value * SIConstants.VACUUM_PERMITTIVITY) : double.NaN;
      }
      else
      {
        Fluidity = double.NaN;
        Viscosity = double.NaN;
        SpecificElectricalConductivity = double.NaN;
        SpecificElectricalResistivity = double.NaN;
      }
    }

    /// <summary>
    /// Gets the retardation times.
    /// </summary>
    public IReadOnlyList<double> RetardationTimes { get; init; }

    /// <summary>
    /// Gets the Prony coefficients, corresponding to the <see cref="RetardationTimes"/>.
    /// </summary>
    public IReadOnlyList<double> PronyCoefficients { get; init; }

    /// <summary>
    /// Gets the retardation densities, corresponding to the <see cref="RetardationTimes"/>.
    /// </summary>
    public IReadOnlyList<double> RetardationDensities { get; init; }

    /// <summary>
    /// Gets the high-frequency susceptibility (the first element of the Prony coefficient array).
    /// </summary>
    public double SusceptibilityHighFrequency { get => PronyCoefficients[0]; }

    /// <summary>
    /// Gets the low-frequency susceptibility. This is the sum of all Prony coefficients.
    /// </summary>
    public double SusceptibilityLowFrequency { get; }

    /// <summary>
    /// Gets the flow term value, which is fluidity in case of strain retardation.
    /// </summary>
    public double Fluidity { get; }

    /// <summary>
    /// Gets the specific electrical conductivity (if the fitted spectrum was a dielectric spectrum of relative permittivity).
    /// The unit is S/m.
    /// </summary>
    public double SpecificElectricalConductivity { get; }

    /// <summary>
    /// Gets the inverse of the flow term value, i.e. <c>1 /</c> <see cref="Fluidity"/>.
    /// </summary>
    public double Viscosity { get; }

    /// <summary>
    /// Gets the specific electrical resistivity (if the fitted spectrum was a dielectric spectrum of relative permittivity).
    /// The unit is Ohm·m.
    /// </summary>
    public double SpecificElectricalResistivity { get; }

    /// <summary>
    /// Gets (in the time domain) the y-value as a function of time.
    /// </summary>
    /// <param name="t">The time value.</param>
    /// <returns>The y-value in the time domain at time <paramref name="t"/>.</returns>
    public double GetTimeDomainYOfTime(double t)
    {
      double sum = 0;
      for (int i = 0; i < PronyCoefficients.Count; ++i)
      {
        var tau = RetardationTimes[i];
        sum += tau == 0 ? PronyCoefficients[i] : PronyCoefficients[i] * (1 - Math.Exp(-t / RetardationTimes[i]));
      }
      sum += Fluidity * t;
      return sum;
    }

    /// <summary>
    /// Gets (in the frequency domain) the susceptibility as a function of the circular frequency.
    /// </summary>
    /// <param name="w">The circular frequency.</param>
    /// <returns>
    /// The susceptibility in the frequency domain.
    /// Note that because it is a susceptibility, the imaginary part is negative.
    /// </returns>
    public Complex64 GetFrequencyDomainYOfOmega(double w)
    {
      Complex64 sum = 0;
      for (int i = 0; i < PronyCoefficients.Count; ++i)
      {
        var tauomega = w * RetardationTimes[i];
        sum += PronyCoefficients[i] / (1 + tauomega * Complex64.ImaginaryOne);
      }
      if (Fluidity != 0)
      {
        sum -= Complex64.ImaginaryOne * (Fluidity / w);
      }
      return sum;
    }

    /// <summary>
    /// Gets (in the frequency domain) the susceptibility as a function of the frequency.
    /// </summary>
    /// <param name="f">The frequency.</param>
    /// <returns>
    /// The susceptibility in the frequency domain.
    /// Note that because it is a general susceptibility, the imaginary part is negative.
    /// </returns>
    public Complex64 GetFrequencyDomainYOfFrequency(double f)
    {
      return GetFrequencyDomainYOfOmega(f * 2 * Math.PI);
    }
  }

}
