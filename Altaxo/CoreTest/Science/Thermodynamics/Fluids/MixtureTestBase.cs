#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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
using System.Text;
using System.Threading.Tasks;
using Altaxo.Science.Thermodynamics.Fluids;
using Xunit;

namespace Altaxo.Science.Thermodynamics.Fluids
{
  public class MixtureTestBase : TestBase
  {
    protected MixtureOfFluids _mixture;

    /// <summary>
    /// TestData for 1 Permille to 999 Permille Molefraction contains
    ///  0. Temperature (Kelvin)\r\n" +
    ///  1. Mole density (mol/m³)\r\n" +
    ///  2. Pressure (Pa)\r\n" +
    ///  3. delta*AlphaR_delta\r\n" +
    ///  4. Isochoric heat capacity (J/mol K)\r\n" +
    ///  5. Phasetype (1: liquid, 2: gas)\r\n" +
    /// </summary>
    public (double temperature, double moleDensity, double pressure, double deltaPhiR_delta, double cv, int phase)[] _testData_001_999;

    /// <summary>
    /// TestData for 500 Permille to 500 Permille Molefraction contains
    ///  0. Temperature (Kelvin)\r\n" +
    ///  1. Mole density (mol/m³)\r\n" +
    ///  2. Pressure (Pa)\r\n" +
    ///  3. delta*AlphaR_delta\r\n" +
    ///  4. Isochoric heat capacity (J/mol K)\r\n" +
    ///  5. Phasetype (1: liquid, 2: gas)\r\n" +
    /// </summary>
    public (double temperature, double moleDensity, double pressure, double deltaPhiR_delta, double cv, int phase)[] _testData_500_500;

    /// <summary>
    /// TestData for 999 Permille to 1 Permille Molefraction contains
    ///  0. Temperature (Kelvin)\r\n" +
    ///  1. Mole density (mol/m³)\r\n" +
    ///  2. Pressure (Pa)\r\n" +
    ///  3. delta*AlphaR_delta\r\n" +
    ///  4. Isochoric heat capacity (J/mol K)\r\n" +
    ///  5. Phasetype (1: liquid, 2: gas)\r\n" +
    /// </summary>
    public (double temperature, double moleDensity, double pressure, double deltaPhiR_delta, double cv, int phase)[] _testData_999_001;

    public virtual void CASNumberAttribute_Test()
    {
    }

    public virtual void Constants_Test()
    {
    }

    public virtual void DeltaPhiRDelta_001_999_Test()
    {
      var mixture = _mixture.WithMoleFractions(new[] { 0.001, 0.999 });
      DeltaPhiRDelta_Test(mixture, _testData_001_999);
    }

    public virtual void DeltaPhiRDelta_500_500_Test()
    {
      var mixture = _mixture.WithMoleFractions(new[] { 0.5, 0.5 });
      DeltaPhiRDelta_Test(mixture, _testData_500_500);
    }

    public virtual void DeltaPhiRDelta_999_001_Test()
    {
      var mixture = _mixture.WithMoleFractions(new[] { 0.999, 0.001 });
      DeltaPhiRDelta_Test(mixture, _testData_999_001);
    }

    public void DeltaPhiRDelta_Test(MixtureOfFluids mixture, (double temperature, double moleDensity, double pressure, double deltaPhiR_delta, double cv, int phase)[] testData)
    {
      double relativeDeviation;
      double maxDeviationdeltaPhiR_delta = 0;

      foreach (var (temperature, moleDensity, pressure, deltaPhiR_delta, cv, phase) in testData)
      {
        var delta = mixture.GetDeltaFromMoleDensity(moleDensity);
        var tau = mixture.GetTauFromTemperature(temperature);

        double pressure_here = mixture.Pressure_FromMoleDensityAndTemperature(moleDensity, temperature);
        double pressureDeviation = GetRelativeErrorBetween(pressure, pressure_here);
        AssertEx.Equal(pressure, pressure_here, GetAllowedError(pressure, 1E-4, 0), $"Pressure at T={temperature} K and rho={moleDensity} mol/m³:");

        double deltaPhiR_delta_here = delta * mixture.PhiR_delta_OfReducedVariables(delta, tau);
        relativeDeviation = GetRelativeErrorBetween(deltaPhiR_delta, deltaPhiR_delta_here);
        maxDeviationdeltaPhiR_delta = Math.Max(maxDeviationdeltaPhiR_delta, relativeDeviation);
        AssertEx.Equal(deltaPhiR_delta, deltaPhiR_delta_here, GetAllowedError(deltaPhiR_delta, 1E-4, 0), $"Delta_PhiR_delta at T={temperature} K and rho={moleDensity} mol/m³:");

        double cv_here = mixture.MoleSpecificIsochoricHeatCapacity_FromMoleDensityAndTemperature(moleDensity, temperature);

        AssertEx.Equal(cv, cv_here, GetAllowedError(cv, 1E-4, 1E-2), $"Isochoric heat capacity at T={temperature} K and rho={moleDensity} mol/m³:");
      }
    }
  }
}
