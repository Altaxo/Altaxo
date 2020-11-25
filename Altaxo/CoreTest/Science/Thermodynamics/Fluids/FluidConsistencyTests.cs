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
using Xunit;

namespace Altaxo.Science.Thermodynamics.Fluids
{

  public class FluidConsistencyTests
  {
    /// <summary>
    /// Tests the consistence between the presence/absence of a departure function
    /// and the value of F of the binary mixture.
    /// </summary>
    [Fact]
    public void TestLowerTemperatureLimit()
    {
      return; // Test not active currently

      var assembly = typeof(MixtureOfFluids).Assembly;

      foreach (var type in assembly.GetTypes().Where(t => typeof(HelmholtzEquationOfStateOfPureFluidsBySpanEtAl).IsAssignableFrom(t) && !t.IsAbstract))
      {
        var m = type.GetProperty("Instance").GetGetMethod();
        var fluid = (HelmholtzEquationOfStateOfPureFluidsBySpanEtAl)m.Invoke(null, new object[0]);

        var minTemperature = Math.Max(fluid.TriplePointTemperature, fluid.LowerTemperatureLimit);
        var maxTemperature = Math.Min(fluid.CriticalPointTemperature, fluid.UpperTemperatureLimit);

        if (minTemperature < maxTemperature) // if there is a range on the liquid vapor line
        {
          double middleTemperature = (minTemperature + maxTemperature) / 2;

          var (liquidMoleDensity, vaporMoleDensity, pressure) = fluid.SaturatedLiquidAndVaporMoleDensitiesAndPressure_FromTemperature(middleTemperature, 1E-12);
          AssertEx.Greater(liquidMoleDensity, 0);
          AssertEx.Greater(vaporMoleDensity, 0);
          AssertEx.Greater(pressure, 0);

          // Try to go down to triple point temperature

          for (double temperature = middleTemperature; ; temperature -= 1.0 / 16)
          {
            temperature = Math.Max(fluid.TriplePointTemperature, temperature);

            (liquidMoleDensity, vaporMoleDensity, pressure) = fluid.SaturatedLiquidAndVaporMoleDensitiesAndPressure_FromTemperature(temperature, liquidMoleDensity, vaporMoleDensity, 1E-12);

            AssertEx.Greater(liquidMoleDensity, 0);
            AssertEx.Greater(vaporMoleDensity, 0);
            AssertEx.Greater(pressure, 0);

            if (temperature == fluid.TriplePointTemperature)
            {
              AssertEx.Equal(fluid.TriplePointPressure, pressure, fluid.TriplePointPressure * 1E-2);
              break;
            }

            if (temperature == minTemperature)
            {
              break;
            }
          }
        }
      }
    }
  }
}
