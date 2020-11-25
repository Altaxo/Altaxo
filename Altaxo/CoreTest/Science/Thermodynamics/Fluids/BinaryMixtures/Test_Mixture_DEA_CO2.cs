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
  /// Tests and test data for <see cref="Mixture_DEA_CO2"/>.
  /// </summary>
  /// <remarks>
  /// <para>Reference:</para>
  /// <para>The test data was created automatically using calls into the TREND.DLL of the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// </remarks>
  
  public class Test_Mixture_DEA_CO2 : MixtureTestBase
  {

    public Test_Mixture_DEA_CO2()
    {
      _mixture = MixtureOfFluids.FromCASRegistryNumbersAndMoleFractions(new[] { ("111-42-2", 0.5), ("124-38-9", 0.5) });

      // TestData for 1 Permille to 999 Permille Molefraction contains:
      // 0. Temperature (Kelvin)
      // 1. Mole density (mol/m³)
      // 2. Pressure (Pa)
      // 3. delta*AlphaR_delta
      // 4. Isochoric heat capacity (J/mol K)
      // 5. Phasetype (1: liquid, 2: gas)
      _testData_001_999 = new (double temperature, double moleDensity, double pressure, double deltaPhiR_delta, double cv, int phase)[]
      {
      ( 350, 0.343643367162187, 999.999999930759, -2.91678343171224E-05, 31.2430105228615, 2 ),
      ( 400, 0.300684655731227, 999.999999999512, -1.82246762735127E-05, 33.2083671581638, 2 ),
      ( 400, 3.00733984506548, 9999.99999508108, -0.00018224962468065, 33.2112114774769, 2 ),
      ( 400, 30.1228249456284, 100000, -0.00182278257831347, 33.2396962929813, 2 ),
      ( 500, 0.240545128628633, 999.999999999979, -7.43290072219899E-06, 36.5447893343914, 2 ),
      ( 500, 2.40561219323256, 9999.99999977909, -7.43205523032308E-05, 36.5459881101669, 2 ),
      ( 500, 24.0722043082462, 99999.9978227667, -0.000742359925564058, 36.5579796825091, 2 ),
      ( 500, 242.321713458259, 999999.999998778, -0.00733889148832586, 36.6782354666816, 2 ),
      ( 500, 2570.29350056216, 9999999.99987854, -0.0641405713856171, 37.8719396603296, 2 ),
      ( 600, 0.20045333567494, 1000, -2.69866340485287E-06, 39.2681853312843, 2 ),
      ( 600, 2.00458192156956, 9999.99999999247, -2.69794734775154E-05, 39.2688071324943, 2 ),
      ( 600, 20.0506736110143, 99999.9999274892, -0.000269079287174056, 39.2750246425307, 2 ),
      ( 600, 200.97928681285, 1000000, -0.00261968743211915, 39.3371365463706, 2 ),
      ( 600, 2043.95671018944, 9999999.99981769, -0.0192904629475187, 39.9401868430683, 2 ),
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
      ( 500, 0.240561711045594, 999.999999976692, -7.34899251921228E-05, 149.839051275911, 2 ),
      ( 500, 2.40721021083221, 9999.99999999959, -0.000735244886221077, 149.857053666405, 2 ),
      ( 600, 0.200460436595237, 999.999999998123, -3.5300942602391E-05, 165.155871371507, 2 ),
      ( 600, 2.00524153921917, 9999.99998108615, -0.000353043598621499, 165.163447554617, 2 ),
      ( 600, 20.1164243442424, 99999.9999999993, -0.00353384515858896, 165.239509777045, 2 ),
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
      ( 500, 0.240626901114822, 999.999999979305, -0.00034151387124792, 263.142958998189, 2 ),
      ( 500, 2.41371644322214, 9999.99978174949, -0.00342592276804127, 263.270968195633, 2 ),
      ( 500, 9063.41946303272, 9999999.9999999, -0.734598266502604, 302.597177272205, 1 ),
      ( 600, 0.200487983623981, 999.999999997131, -0.000169821725911058, 291.047600903086, 2 ),
      ( 600, 2.00795406303336, 9999.99997051238, -0.00170058618291107, 291.105166125119, 2 ),
      ( 600, 20.3972567838534, 99999.999999987, -0.0172505129843848, 291.694660226825, 2 ),
      ( 600, 8183.03895285573, 10000000.0000006, -0.755037294145101, 324.927200595111, 1 ),
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
