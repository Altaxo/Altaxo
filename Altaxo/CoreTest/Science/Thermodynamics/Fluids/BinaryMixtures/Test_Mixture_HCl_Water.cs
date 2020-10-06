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

  /// <summary>
  /// Tests and test data for <see cref="Mixture_HCl_Water"/>.
  /// </summary>
  /// <remarks>
  /// <para>Reference:</para>
  /// <para>The test data was created automatically using calls into the TREND.DLL of the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// </remarks>
  
  public class Test_Mixture_HCl_Water : MixtureTestBase
  {

    public Test_Mixture_HCl_Water()
    {
      _mixture = MixtureOfFluids.FromCASRegistryNumbersAndMoleFractions(new[] { ("7647-01-0", 0.5), ("7732-18-5", 0.5) });

      // TestData for 1 Permille to 999 Permille Molefraction contains:
      // 0. Temperature (Kelvin)
      // 1. Mole density (mol/m³)
      // 2. Pressure (Pa)
      // 3. delta*AlphaR_delta
      // 4. Isochoric heat capacity (J/mol K)
      // 5. Phasetype (1: liquid, 2: gas)
      _testData_001_999 = new (double temperature, double moleDensity, double pressure, double deltaPhiR_delta, double cv, int phase)[]
      {
      ( 300, 0.401105650359603, 999.999999768146, -0.000482177526584926, 25.4005162604455, 2 ),
      ( 300, 55302.8755450906, 1000000.00024829, -0.992750607588435, 74.3225482886681, 1 ),
      ( 300, 55524.3427452515, 9999999.99980696, -0.927795228831055, 73.8045875424619, 1 ),
      ( 350, 0.343707919376635, 999.999999941029, -0.000200318864728802, 25.5912523400935, 2 ),
      ( 350, 3.44333328571836, 9999.99935394989, -0.00201624493488998, 25.882915263641, 2 ),
      ( 350, 54031.2409921883, 1000000.00003534, -0.993639993050245, 69.9986106120329, 1 ),
      ( 350, 54251.0249785157, 10000000.0000262, -0.936657589726018, 69.6903073175333, 1 ),
      ( 400, 0.300715682907895, 999.999999994056, -0.000104744241207382, 25.9539136971449, 2 ),
      ( 400, 3.01000294593494, 9999.99993824728, -0.0010501980405789, 26.0559826916342, 2 ),
      ( 400, 30.3962043485036, 99999.9999987308, -0.0107837764487855, 27.1607666287829, 2 ),
      ( 400, 52015.9918533491, 1000000.00015423, -0.994219389576086, 65.3810357918096, 1 ),
      ( 400, 52268.4051374405, 10000000.0000113, -0.942473051572632, 65.1922144360857, 1 ),
      };

      // TestData for 500 Permille to 500 Permille Molefraction contains:
      // 0. Temperature (Kelvin)
      // 1. Mole density (mol/m³)
      // 2. Pressure (Pa)
      // 3. delta*AlphaR_delta
      // 4. Isochoric heat capacity (J/mol K)
      // 5. Phasetype (1: liquid, 2: gas)
      _testData_500_500 = new (double temperature, double moleDensity, double pressure, double deltaPhiR_delta, double cv, int phase)[]
      {
      ( 300, 0.400971289012224, 999.999999916576, -0.000152694619518107, 23.0604337286677, 2 ),
      ( 350, 0.343665395481995, 999.999999990755, -8.20529227916397E-05, 23.1987212302105, 2 ),
      ( 350, 3.43919725886465, 9999.99990527357, -0.000821497374670658, 23.2433870404052, 2 ),
      ( 400, 0.300697520409697, 999.999999998451, -4.97951207666679E-05, 23.4002001668379, 2 ),
      ( 400, 3.00832430077473, 9999.99998430168, -0.000498227324884544, 23.4211167756263, 2 ),
      ( 400, 30.2196581349893, 99999.9999999994, -0.00501009705333534, 23.6343691228137, 2 ),
      };

      // TestData for 999 Permille to 1 Permille Molefraction contains:
      // 0. Temperature (Kelvin)
      // 1. Mole density (mol/m³)
      // 2. Pressure (Pa)
      // 3. delta*AlphaR_delta
      // 4. Isochoric heat capacity (J/mol K)
      // 5. Phasetype (1: liquid, 2: gas)
      _testData_999_001 = new (double temperature, double moleDensity, double pressure, double deltaPhiR_delta, double cv, int phase)[]
      {
      ( 300, 0.400930942702305, 999.982609717659, -5.75231206227543E-05, 20.8200840130482, 2 ),
      ( 300, 4.01138723806557, 9999.99999991231, -0.000575472500176503, 20.8354890877771, 2 ),
      ( 300, 40.3237803454821, 99999.9990831858, -0.00577803939855588, 20.9907160548865, 2 ),
      ( 300, 426.642721911386, 999999.999992643, -0.0603194220533298, 22.6718944644889, 2 ),
      ( 300, 23325.430743615, 10000000.0000003, -0.828124126019839, 33.548762726188, 1 ),
      ( 350, 0.343647507130803, 999.989270429371, -3.5448278290288E-05, 20.8287836260376, 2 ),
      ( 350, 3.43757204819574, 9999.99999997949, -0.000354551207169514, 20.8355773679372, 2 ),
      ( 350, 34.486026608153, 99999.9997902958, -0.00355199165778483, 20.9038416642764, 2 ),
      ( 350, 356.536911234105, 999999.99999987, -0.0361858352949164, 21.62040104378, 2 ),
      ( 350, 6431.29377275261, 9999999.99993766, -0.465682431198692, 33.086640201811, 2 ),
      ( 400, 0.300687954001133, 999.994293786723, -2.34273909783972E-05, 20.8534489151224, 2 ),
      ( 400, 3.00751372961721, 9999.99999999616, -0.000234290983785215, 20.8568555983172, 2 ),
      ( 400, 30.1387506627082, 99999.9999614319, -0.00234448005913177, 20.8910301660006, 2 ),
      ( 400, 307.94786463709, 999999.999999998, -0.0235980042237974, 21.243646324932, 2 ),
      ( 400, 3983.27862916754, 9999999.9894389, -0.245142161208837, 25.7447595107012, 2 ),
      };
    }

    [Fact]
    public override void CASNumberAttribute_Test()
    {
      base.CASNumberAttribute_Test();
    }

    [Fact]
    public override void Constants_Test()
    {
      base.Constants_Test();
    }

    [Fact]
    public override void DeltaPhiRDelta_001_999_Test()
    {
      base.DeltaPhiRDelta_001_999_Test();
    }

    [Fact]
    public override void DeltaPhiRDelta_500_500_Test()
    {
      base.DeltaPhiRDelta_500_500_Test();
    }

    [Fact]
    public override void DeltaPhiRDelta_999_001_Test()
    {
      base.DeltaPhiRDelta_999_001_Test();
    }
  }
}
