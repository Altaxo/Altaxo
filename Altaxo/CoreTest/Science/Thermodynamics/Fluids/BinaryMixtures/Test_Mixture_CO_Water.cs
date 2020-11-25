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
  /// Tests and test data for <see cref="Mixture_CO_Water"/>.
  /// </summary>
  /// <remarks>
  /// <para>Reference:</para>
  /// <para>The test data was created automatically using calls into the TREND.DLL of the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// </remarks>
  
  public class Test_Mixture_CO_Water : MixtureTestBase
  {

    public Test_Mixture_CO_Water()
    {
      _mixture = MixtureOfFluids.FromCASRegistryNumbersAndMoleFractions(new[] { ("630-08-0", 0.5), ("7732-18-5", 0.5) });

      // TestData for 1 Permille to 999 Permille Molefraction contains:
      // 0. Temperature (Kelvin)
      // 1. Mole density (mol/m³)
      // 2. Pressure (Pa)
      // 3. delta*AlphaR_delta
      // 4. Isochoric heat capacity (J/mol K)
      // 5. Phasetype (1: liquid, 2: gas)
      _testData_001_999 = new (double temperature, double moleDensity, double pressure, double deltaPhiR_delta, double cv, int phase)[]
      {
      ( 300, 0.401104908894901, 999.999999771193, -0.000480331052513288, 25.3994836631613, 2 ),
      ( 300, 57508.0233948426, 99999999.9999017, -0.302858588967145, 69.9739886094803, 1 ),
      ( 350, 0.343707752860288, 999.999999728978, -0.000199835681474538, 25.5911278967653, 2 ),
      ( 350, 3.44331621390181, 9999.99983077841, -0.00201129825878844, 25.8809189874004, 2 ),
      ( 350, 56196.3825661569, 100000000.000037, -0.38850322370349, 67.2260496314542, 1 ),
      ( 400, 0.300715636166445, 999.999999994074, -0.00010459001402104, 25.9540272838993, 2 ),
      ( 400, 3.00999823430586, 9999.99993844796, -0.00104863554778201, 26.055530780235, 2 ),
      ( 400, 30.3956598706723, 99999.9999987309, -0.0107660577859124, 27.1538128091532, 2 ),
      ( 400, 54430.3012137777, 100000000.003515, -0.447579422456167, 63.6197711530624, 1 ),
      ( 500, 0.240557145598957, 999.999999998939, -4.07310614673546E-05, 26.9089801408378, 2 ),
      ( 500, 2.40645425903758, 9999.99998925239, -0.000407564167104065, 26.9315902765603, 2 ),
      ( 500, 24.1537934998419, 99999.9999999997, -0.00410116756157482, 27.163121936146, 2 ),
      ( 500, 251.555762400804, 999999.999999981, -0.0437613308662363, 30.0534372041005, 2 ),
      ( 500, 46424.0579633791, 10000000.0000028, -0.948184764968906, 58.0204479087109, 1 ),
      ( 500, 49836.1986807278, 99999999.9999998, -0.517324045936572, 57.3187088228231, 1 ),
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
      ( 300, 0.400937913315037, 999.999999988467, -7.00585864818001E-05, 23.055551528515, 2 ),
      ( 350, 0.343650867726117, 999.999999998383, -4.03769650149778E-05, 23.2305684866584, 2 ),
      ( 350, 3.43775829922897, 9999.99998369508, -0.000403861364086045, 23.240238591935, 2 ),
      ( 400, 0.300689736900718, 999.999999999726, -2.45062306462444E-05, 23.4863499114461, 2 ),
      ( 400, 3.00756080704512, 9999.99999722695, -0.000245090891083259, 23.4923849809127, 2 ),
      ( 400, 30.142198657014, 99999.9999999999, -0.00245376407007211, 23.5528391742017, 2 ),
      ( 500, 0.240548224133754, 999.99999999999, -9.68467268642718E-06, 24.1945325687711, 2 ),
      ( 500, 2.40569193983702, 9999.99999989083, -9.68514728673216E-05, 24.1974451043787, 2 ),
      ( 500, 24.0779205774558, 99999.998894221, -0.000968984295153517, 24.2265686647533, 2 ),
      ( 500, 242.909824326583, 999999.999999252, -0.00973171764541641, 24.5176138093402, 2 ),
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
      ( 300, 0.40090878154632, 999.999981466571, -3.43949987604313E-06, 20.830473729555, 2 ),
      ( 300, 4.00921180553867, 9999.99817051012, -3.43656843961771E-05, 20.8310997080873, 2 ),
      ( 300, 40.1044050191788, 99999.9999999999, -0.000340729581027066, 20.8373635624299, 2 ),
      ( 300, 402.161343824585, 1000000, -0.00311800530316451, 20.9003618580691, 2 ),
      ( 350, 0.343634850335185, 1000.00002153499, 1.88850957908867E-07, 20.899436456486, 1 ),
      ( 350, 3.43634260673051, 10000.0022000897, 1.90859615699648E-06, 20.8998991984258, 1 ),
      ( 350, 34.3627669151862, 100000, 2.10907930966205E-05, 20.9045278987867, 1 ),
      ( 350, 343.494873048204, 1000000.00000523, 0.000407701816476484, 20.9509216930156, 1 ),
      ( 350, 3365.9325792803, 10000000.0086314, 0.0209203792676729, 21.4078222190292, 1 ),
      ( 350, 18450.997154507, 100000000, 0.862419215906666, 23.867937921683, 1 ),
      ( 400, 0.300679899890585, 1000.00033893211, 2.12613973133738E-06, 21.0285313480395, 1 ),
      ( 400, 3.00674155110435, 9999.99999999999, 2.12750410978521E-05, 21.0288984996634, 1 ),
      ( 400, 30.0616186318687, 100000.000000015, 0.000214112378789554, 21.0325702464636, 1 ),
      ( 400, 299.998248736296, 1000000.00027154, 0.00227435737111808, 21.069299942849, 1 ),
      ( 400, 2908.5293580265, 10000000.0050944, 0.0337889529711677, 21.4292086876586, 1 ),
      ( 400, 16978.3513300499, 100000000.013204, 0.770964365980981, 23.5713304115588, 1 ),
      ( 500, 0.2405435196737, 1000.00078661007, 3.75741626352364E-06, 21.4819400033129, 1 ),
      ( 500, 2.40535402102951, 10000, 3.75806194190131E-05, 21.4821995580275, 1 ),
      ( 500, 24.0453921995369, 100000.000000141, 0.00037645290304019, 21.4847947278348, 1 ),
      ( 500, 239.627244085056, 1000000.00165393, 0.00382760102820712, 21.5107053054192, 1 ),
      ( 500, 2305.48600183312, 10000000.0000036, 0.0433567646088418, 21.7631266666844, 1 ),
      ( 500, 14667.0210303695, 100000000.005167, 0.640036112853907, 23.4466272164878, 1 ),
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
