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
  /// Tests and test data for <see cref="Mixture_DEA_H2S"/>.
  /// </summary>
  /// <remarks>
  /// <para>Reference:</para>
  /// <para>The test data was created automatically using calls into the TREND.DLL of the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// </remarks>
  
  public class Test_Mixture_DEA_H2S : MixtureTestBase
  {

    public Test_Mixture_DEA_H2S()
    {
      _mixture = MixtureOfFluids.FromCASRegistryNumbersAndMoleFractions(new[] { ("111-42-2", 0.5), ("7783-06-4", 0.5) });

      // TestData for 1 Permille to 999 Permille Molefraction contains:
      // 0. Temperature (Kelvin)
      // 1. Mole density (mol/m³)
      // 2. Pressure (Pa)
      // 3. delta*AlphaR_delta
      // 4. Isochoric heat capacity (J/mol K)
      // 5. Phasetype (1: liquid, 2: gas)
      _testData_001_999 = new (double temperature, double moleDensity, double pressure, double deltaPhiR_delta, double cv, int phase)[]
      {
      ( 350, 0.343650515401345, 999.999999999735, -4.54025792820096E-05, 26.6325296151425, 2 ),
      ( 350, 3.43791055583078, 9999.99999185423, -0.000454179359336297, 26.64006078186, 2 ),
      ( 350, 18979.550516753, 9999999.99999992, -0.818944651782849, 36.0460765724469, 1 ),
      ( 400, 0.300689459281289, 999.999999999945, -2.96338868435939E-05, 27.4157175134346, 2 ),
      ( 400, 3.00769695192494, 9999.99999945125, -0.000296394583744799, 27.4199933032407, 2 ),
      ( 400, 30.1576091115809, 99999.9942800988, -0.00296954037601605, 27.4628817788011, 2 ),
      ( 500, 0.240547792522542, 999.999488869128, -1.3941403038125E-05, 29.1381368955708, 2 ),
      ( 500, 2.40577979845859, 9999.99999999973, -0.000139417991532801, 29.1398892130051, 2 ),
      ( 500, 24.0880363264676, 99999.9999968939, -0.00139456912502129, 29.1574388837894, 2 ),
      ( 500, 243.956104965261, 999999.999999999, -0.013984753604117, 29.3355604507008, 2 ),
      ( 500, 2805.87200681632, 9999999.99999999, -0.142710578519796, 31.3280043080547, 2 ),
      ( 600, 0.200455080232987, 999.998353777244, -6.88986343708636E-06, 30.9583728227682, 2 ),
      ( 600, 2.00467510196705, 9999.99999999993, -6.88943265983504E-05, 30.9592791343888, 2 ),
      ( 600, 20.0591806780336, 99999.9999992413, -0.000688500990909877, 30.968349795563, 2 ),
      ( 600, 201.834368109306, 999999.993015589, -0.00684060389651347, 31.0597896803442, 2 ),
      ( 600, 2140.5312391406, 9999999.99999935, -0.0635329423967375, 32.0220385902721, 2 ),
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
      ( 500, 0.240565156792799, 999.982612443009, -8.55259631194196E-05, 146.132097910295, 2 ),
      ( 500, 2.40750619482309, 9999.99999986976, -0.00085581343225783, 146.15341388257, 2 ),
      ( 600, 0.200462361734448, 999.999511455862, -4.26190064683325E-05, 160.99663421377, 2 ),
      ( 600, 2.00539304023448, 9999.99999999996, -0.000426279527987788, 161.00537473149, 2 ),
      ( 600, 20.1313780754428, 99999.9999993219, -0.00427175191109293, 161.093250430158, 2 ),
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
      ( 400, 9782.51918372532, 10000000.0000254, -0.692634483672811, 268.605767230889, 1 ),
      ( 500, 0.240626918678409, 999.999999977302, -0.000341582268481275, 263.135544938994, 2 ),
      ( 500, 2.41371812255596, 9999.9997605133, -0.00342661156808285, 263.263564108521, 2 ),
      ( 500, 9063.55344290364, 10000000.0001293, -0.73460218852566, 302.58596578387, 1 ),
      ( 600, 0.200487991975784, 999.999999996884, -0.000169858806637948, 291.039283097182, 2 ),
      ( 600, 2.00795482067589, 9999.99996798643, -0.00170095829874639, 291.096861850726, 2 ),
      ( 600, 20.3973369598148, 99999.9999999846, -0.0172543713934817, 291.686496988185, 2 ),
      ( 600, 8183.42179950895, 10000000.0010362, -0.755048753139852, 324.917102699896, 1 ),
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
