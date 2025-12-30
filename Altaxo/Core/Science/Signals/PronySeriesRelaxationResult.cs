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
  /// Represents the result of a Prony series fit to a relaxation process (for instance, a modulus), either
  /// in the time domain or in the frequency domain.
  /// </summary>
  public record PronySeriesRelaxationResult
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="PronySeriesRelaxationResult"/> record.
    /// </summary>
    /// <param name="relaxationTimes">
    /// The relaxation times (tau values). The list may include <see cref="double.PositiveInfinity"/> if an intercept/static term is present.
    /// </param>
    /// <param name="pronyCoefficients">The Prony coefficients corresponding to <paramref name="relaxationTimes"/>.</param>
    /// <param name="relaxationDensities">The relaxation density values corresponding to <paramref name="relaxationTimes"/>.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="relaxationTimes"/>, <paramref name="pronyCoefficients"/>, or <paramref name="relaxationDensities"/> is <see langword="null"/>.
    /// </exception>
    public PronySeriesRelaxationResult(IReadOnlyList<double> relaxationTimes, IReadOnlyList<double> pronyCoefficients, IReadOnlyList<double> relaxationDensities)
    {
      RelaxationTimes = relaxationTimes ?? throw new ArgumentNullException(nameof(relaxationTimes));
      PronyCoefficients = pronyCoefficients ?? throw new ArgumentNullException(nameof(pronyCoefficients));
      RelaxationDensities = relaxationDensities ?? throw new ArgumentNullException(nameof(relaxationDensities));


      // ANSYS and maybe other simulation programs request relativeCoefficients:
      // in a way that the sum of relativeCoefficients is equal to 1 - E_u/E_o,
      // in which E_u is the lower frequency modulus and E_o is the high frequency modulus
      // the low frequency modulus is the modulus at the highest time, thus the last element of the PronyCoefficient array
      // the high frequency modulus is the sum of all coefficients:
      ModulusHighFrequency = PronyCoefficients.Sum();

      var targetSum = 1 - ModulusLowFrequency / ModulusHighFrequency;
      double coeffSum = 0;
      int N = 0;
      for (int i = 0; i < RelaxationTimes.Count; i++)
      {
        if (!double.IsInfinity(RelaxationTimes[i]))
        {
          coeffSum += PronyCoefficients[i]; // this is the sum of Prony coefficients, but without the static modulus
          ++N;
        }
      }
      var relativeCoefficients = new double[N];
      for (int i = 0; i < N; ++i)
      {
        relativeCoefficients[i] = PronyCoefficients[i] * targetSum / coeffSum;
      }
      RelativeRelaxationCoefficients = relativeCoefficients;
    }

    /// <summary>
    /// Gets the relaxation times.
    /// </summary>
    public IReadOnlyList<double> RelaxationTimes { get; init; }

    /// <summary>
    /// Gets the Prony coefficients, corresponding to the <see cref="RelaxationTimes"/>.
    /// </summary>
    public IReadOnlyList<double> PronyCoefficients { get; init; }

    /// <summary>
    /// Gets the relaxation densities, corresponding to the <see cref="RelaxationTimes"/>.
    /// </summary>
    public IReadOnlyList<double> RelaxationDensities { get; init; }

    /// <summary>
    /// Gets the relative relaxation coefficients. See remarks for details.
    /// </summary>
    /// <remarks>
    /// ANSYS (and possibly other simulation programs) request relative coefficients
    /// such that the sum of the relative coefficients equals <c>1 - E_u/E_0</c>,
    /// where <c>E_u</c> is the low-frequency modulus and <c>E_0</c> is the high-frequency modulus.
    ///
    /// The low-frequency modulus is the modulus at the highest time, thus the last element of the <see cref="PronyCoefficients"/> array,
    /// whereas the high-frequency modulus is the sum of all elements in <see cref="PronyCoefficients"/>.
    /// </remarks>
    public IReadOnlyList<double> RelativeRelaxationCoefficients { get; init; }

    /// <summary>
    /// Gets the high-frequency modulus. This is the sum of all Prony coefficients.
    /// </summary>
    public double ModulusHighFrequency { get; init; }

    /// <summary>
    /// Gets the low-frequency modulus (the last element of the Prony coefficient array).
    /// </summary>
    public double ModulusLowFrequency { get => PronyCoefficients[^1]; }

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
        sum += PronyCoefficients[i] * Math.Exp(-t / RelaxationTimes[i]);
      }
      return sum;
    }

    /// <summary>
    /// Gets (in the frequency domain) the modulus as a function of the circular frequency.
    /// </summary>
    /// <param name="w">The circular frequency.</param>
    /// <returns>The modulus in the frequency domain.</returns>
    public Complex64 GetFrequencyDomainYOfOmega(double w)
    {
      Complex64 sum = 0;
      for (int i = 0; i < PronyCoefficients.Count; ++i)
      {
        if (double.IsPositiveInfinity(RelaxationTimes[i]))
        {
          sum += PronyCoefficients[i];
        }
        else
        {
          var tauomega = w * RelaxationTimes[i];
          sum += PronyCoefficients[i] * tauomega / (tauomega - Complex64.ImaginaryOne);
        }
      }
      return sum;
    }

    /// <summary>
    /// Gets (in the frequency domain) the modulus as a function of the frequency.
    /// </summary>
    /// <param name="f">The frequency.</param>
    /// <returns>The modulus in the frequency domain.</returns>
    public Complex64 GetFrequencyDomainYOfFrequency(double f)
    {
      return GetFrequencyDomainYOfOmega(f * 2 * Math.PI);
    }
  }

}
