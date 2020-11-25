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
  /// Tests and test data for <see cref="Mixture_DEA_Water"/>.
  /// </summary>
  /// <remarks>
  /// <para>Reference:</para>
  /// <para>The test data was created automatically using calls into the TREND.DLL of the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// </remarks>
  
  public class Test_Mixture_DEA_Water : MixtureTestBase
  {

    public Test_Mixture_DEA_Water()
    {
      _mixture = MixtureOfFluids.FromCASRegistryNumbersAndMoleFractions(new[] { ("111-42-2", 0.5), ("7732-18-5", 0.5) });

      // TestData for 1 Permille to 999 Permille Molefraction contains:
      // 0. Temperature (Kelvin)
      // 1. Mole density (mol/m³)
      // 2. Pressure (Pa)
      // 3. delta*AlphaR_delta
      // 4. Isochoric heat capacity (J/mol K)
      // 5. Phasetype (1: liquid, 2: gas)
      _testData_001_999 = new (double temperature, double moleDensity, double pressure, double deltaPhiR_delta, double cv, int phase)[]
      {
      ( 350, 0.343708318632556, 999.999999720511, -0.000201480244612701, 25.7646374275916, 2 ),
      ( 350, 53894.3292156569, 1000000.00002068, -0.993623836251621, 70.2232987153108, 1 ),
      ( 350, 54113.3458586412, 10000000.0000338, -0.936496429347593, 69.9145671950215, 1 ),
      ( 400, 0.30071585306834, 999.999999970194, -0.000105310033196242, 26.1548218108205, 2 ),
      ( 400, 3.0100200936783, 9999.99968720025, -0.00105588891776955, 26.25762017567, 2 ),
      ( 400, 30.3980584886846, 100000.000138447, -0.0108441140478726, 27.370849575376, 2 ),
      ( 400, 51889.3213671731, 1000000.00001198, -0.994205278142975, 65.6332498820224, 1 ),
      ( 400, 52140.7519239474, 10000000.0000004, -0.942332211643077, 65.4440722376658, 1 ),
      ( 500, 0.240557199664618, 999.999999998918, -4.09546134383535E-05, 27.1508828425655, 2 ),
      ( 500, 2.40645965411608, 9999.99998900638, -0.000409803978355535, 27.1737280017928, 2 ),
      ( 500, 24.1543472723342, 99999.9999999997, -0.0041239987614482, 27.4077245340022, 2 ),
      ( 500, 251.628533574374, 999999.999999976, -0.0440378747174285, 30.3353356876979, 2 ),
      ( 500, 46371.6094423569, 9999999.9999919, -0.948126159382949, 58.2758353330893, 1 ),
      ( 600, 0.200460158963512, 999.999999999994, -2.01329229380681E-05, 28.2747167016564, 2 ),
      ( 600, 2.00496497821569, 9999.99999928648, -0.000201373627095108, 28.2830098299491, 2 ),
      ( 600, 20.0861499282096, 99999.9910217488, -0.00201818731885456, 28.3665951723752, 2 ),
      ( 600, 204.680529261036, 999999.999785057, -0.020639022972909, 29.2689042692213, 2 ),
      ( 600, 2768.69984170657, 10000000.000001, -0.275991856917901, 47.679003756944, 2 ),
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
      ( 350, 18573.3452134385, 1000000.00016753, -0.981498368072968, 159.379634769824, 1 ),
      ( 350, 18645.1799849496, 10000000.0000008, -0.815696498003906, 159.279840366947, 1 ),
      ( 400, 17937.6113122833, 1000000.00052935, -0.983237313928476, 168.514621139357, 1 ),
      ( 400, 18024.7054437518, 9999999.99999869, -0.833183100784387, 168.467979272025, 1 ),
      ( 500, 0.240583867897054, 999.999999797461, -0.000157243231198573, 145.142247657052, 2 ),
      ( 500, 2.40925680501508, 9999.99999999636, -0.0015757672168604, 145.224031508648, 2 ),
      ( 500, 16563.3830031614, 9999999.9999993, -0.854772399052576, 181.329643611362, 1 ),
      ( 600, 0.200470458370566, 999.999999983461, -7.69537007489853E-05, 159.654622330598, 2 ),
      ( 600, 2.00609527162201, 9999.99983110156, -0.000770131582750142, 159.684711143285, 2 ),
      ( 600, 20.202303857798, 99999.9999998378, -0.00776152632162974, 159.993192129376, 2 ),
      ( 600, 14565.9503982606, 10000000.0006254, -0.862381083308458, 189.93758445731, 1 ),
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
      ( 350, 10076.2370019638, 99999.9999941239, -0.996589646259537, 245.989558376254, 1 ),
      ( 350, 10080.2822866835, 1000000.00029109, -0.965910148581499, 246.014681615985, 1 ),
      ( 350, 10119.8641884013, 10000000.0180434, -0.660434844272342, 246.264649254939, 1 ),
      ( 400, 9733.48148333816, 99999.9999964306, -0.996910859590645, 268.403412994702, 1 ),
      ( 400, 9738.52643586668, 1000000.00057457, -0.969124598902018, 268.425680460089, 1 ),
      ( 400, 9787.57185824128, 9999999.99999798, -0.692793152395316, 268.649615257623, 1 ),
      ( 500, 0.240627029390534, 999.999999979796, -0.000342030109415037, 263.1335850843, 2 ),
      ( 500, 2.41372910256955, 9999.99978693343, -0.00343113290985699, 263.261922366634, 2 ),
      ( 500, 8979.87289842179, 100000.000000992, -0.997321290285232, 302.498513015671, 1 ),
      ( 500, 8988.35091601302, 1000000.00222873, -0.973238168993995, 302.507837240909, 1 ),
      ( 500, 9068.9651085419, 10000000, -0.734760554459763, 302.620730092198, 1 ),
      ( 600, 0.200488036182172, 999.99999999719, -0.000170067160636211, 291.036608158007, 2 ),
      ( 600, 2.00795905235074, 9999.9999711144, -0.0017030500814652, 291.094326493787, 2 ),
      ( 600, 20.3977892721136, 99999.9999999878, -0.0172761514642988, 291.685456165027, 2 ),
      ( 600, 8029.2115926315, 1000000.00649347, -0.975034417920497, 325.136778810046, 1 ),
      ( 600, 8189.54932097432, 9999999.99999985, -0.755232025736005, 324.945388019563, 1 ),
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
