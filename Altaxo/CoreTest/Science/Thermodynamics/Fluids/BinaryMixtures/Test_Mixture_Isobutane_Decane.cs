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
  /// Tests and test data for <see cref="Mixture_Isobutane_Decane"/>.
  /// </summary>
  /// <remarks>
  /// <para>Reference:</para>
  /// <para>The test data was created automatically using calls into the TREND.DLL of the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// </remarks>
  
  public class Test_Mixture_Isobutane_Decane : MixtureTestBase
  {

    public Test_Mixture_Isobutane_Decane()
    {
      _mixture = MixtureOfFluids.FromCASRegistryNumbersAndMoleFractions(new[] { ("75-28-5", 0.5), ("124-18-5", 0.5) });

      // TestData for 1 Permille to 999 Permille Molefraction contains:
      // 0. Temperature (Kelvin)
      // 1. Mole density (mol/m³)
      // 2. Pressure (Pa)
      // 3. delta*AlphaR_delta
      // 4. Isochoric heat capacity (J/mol K)
      // 5. Phasetype (1: liquid, 2: gas)
      _testData_001_999 = new (double temperature, double moleDensity, double pressure, double deltaPhiR_delta, double cv, int phase)[]
      {
      ( 350, 0.344176798545709, 999.999992136986, -0.0015744413076384, 257.833047944311, 2 ),
      ( 400, 0.300956438965062, 999.999998459993, -0.000916712862765189, 289.546193307115, 2 ),
      ( 400, 3.03485795790518, 9999.98364690387, -0.009243407184081, 290.009982344835, 2 ),
      ( 400, 4537.53969685295, 100000.000001313, -0.993373489414593, 310.685824443721, 1 ),
      ( 400, 4547.57120498016, 999999.999999776, -0.933881068646821, 310.728785692274, 1 ),
      ( 400, 4637.38565185401, 10000000.0000276, -0.351616253440318, 311.19731917189, 1 ),
      ( 500, 0.240636262233823, 999.999894648092, -0.000381586559747189, 347.929540378973, 2 ),
      ( 500, 2.41468795787847, 9999.99999961141, -0.00382806035605723, 348.09386485408, 2 ),
      ( 500, 25.0461235302448, 100000.012011941, -0.039594142654606, 349.807571560212, 2 ),
      ( 500, 3894.77946788214, 1000000.00000003, -0.938239266011257, 365.578777377061, 1 ),
      ( 500, 4092.41879969991, 10000000.0012266, -0.412219397719278, 365.382887579267, 1 ),
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
      ( 350, 0.343817525958954, 999.999999998103, -0.000531135188714287, 180.254626065535, 2 ),
      ( 350, 6058.29849530638, 999999.999999908, -0.94327864294271, 195.779617695403, 1 ),
      ( 350, 6207.07888450559, 10000000.0000017, -0.446382237496178, 196.209454949526, 1 ),
      ( 400, 0.300778947760744, 999.999984191587, -0.000327148672004052, 202.9082700891, 2 ),
      ( 400, 3.01670219356983, 9999.99997761837, -0.00328063880105291, 203.020666911442, 2 ),
      ( 400, 5762.91211645104, 10000000.003838, -0.478248943657218, 216.706851805073, 1 ),
      ( 500, 0.240579957486921, 999.999999032344, -0.00014763834265641, 244.531379813619, 2 ),
      ( 500, 2.40900445599287, 9999.98988353667, -0.00147781618675057, 244.572661097647, 2 ),
      ( 500, 24.4188948418745, 99999.9805964633, -0.0149249542130184, 244.992855937591, 2 ),
      ( 500, 4736.69182431919, 10000000.0000062, -0.492167851356075, 255.940341990356, 1 ),
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
      ( 250, 0.481306231863915, 999.999999997217, -0.000451593036080439, 75.7621043642275, 2 ),
      ( 250, 10420.7675214533, 99999.9999999979, -0.995383364290577, 87.723275514602, 1 ),
      ( 250, 10439.191589426, 999999.999999635, -0.953915121397823, 87.7809792773653, 1 ),
      ( 250, 10608.5233682432, 10000000.0000509, -0.546507218185856, 88.3331962922328, 1 ),
      ( 300, 0.401007644538453, 999.999999999556, -0.000249987181055479, 88.9701636647082, 2 ),
      ( 300, 4.01914429694371, 9999.99999538397, -0.00250558788243014, 89.0254276388131, 2 ),
      ( 300, 41.1464421023337, 99999.9999999994, -0.0256571958986618, 89.6144623351069, 2 ),
      ( 300, 9448.52643533388, 999999.999999929, -0.957569320410795, 98.4239180492132, 1 ),
      ( 300, 9734.56983487446, 10000000.0022548, -0.588161156934307, 98.8642537149644, 1 ),
      ( 350, 0.343687417735772, 999.999999999955, -0.000152770688282918, 102.728653475793, 2 ),
      ( 350, 3.44161313159992, 9999.99999952915, -0.00152951760462713, 102.757390967919, 2 ),
      ( 350, 34.9038485800506, 99999.9926230084, -0.0154813100670548, 103.053645867792, 2 ),
      ( 350, 419.749594231298, 999999.999978363, -0.181333544814432, 107.257882830935, 2 ),
      ( 350, 8750.41185420632, 9999999.99999986, -0.607292870212679, 110.862312995849, 1 ),
      ( 400, 0.300710485572099, 999.997898186292, -9.95548367922264E-05, 116.298369288823, 2 ),
      ( 400, 3.00980382915382, 9999.99999996754, -0.000996193129128432, 116.315063910751, 2 ),
      ( 400, 30.3726072807192, 99999.999458343, -0.010027207653705, 116.484710489761, 2 ),
      ( 400, 337.10361197419, 999999.999431414, -0.10804708799279, 118.510582355125, 2 ),
      ( 400, 7551.29197540503, 10000000.0000014, -0.601815756422575, 123.668031410876, 1 ),
      ( 500, 0.240555850192509, 999.997934199683, -4.74380824190381E-05, 141.143019950514, 2 ),
      ( 500, 2.40658619500775, 9999.99999999897, -0.000474451487836428, 141.149962809862, 2 ),
      ( 500, 24.1692842521863, 99999.9999877057, -0.00475150130165949, 141.219682439969, 2 ),
      ( 500, 252.733455929168, 999999.999999991, -0.0482287444836619, 141.946935911368, 2 ),
      ( 500, 4105.66566561794, 9999999.99999999, -0.414115862684394, 148.08490299926, 1 ),
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
