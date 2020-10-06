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
  /// Tests and test data for <see cref="Mixture_Decane_CO"/>.
  /// </summary>
  /// <remarks>
  /// <para>Reference:</para>
  /// <para>The test data was created automatically using calls into the TREND.DLL of the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// </remarks>
  
  public class Test_Mixture_Decane_CO : MixtureTestBase
  {

    public Test_Mixture_Decane_CO()
    {
      _mixture = MixtureOfFluids.FromCASRegistryNumbersAndMoleFractions(new[] { ("124-18-5", 0.5), ("630-08-0", 0.5) });

      // TestData for 1 Permille to 999 Permille Molefraction contains:
      // 0. Temperature (Kelvin)
      // 1. Mole density (mol/m³)
      // 2. Pressure (Pa)
      // 3. delta*AlphaR_delta
      // 4. Isochoric heat capacity (J/mol K)
      // 5. Phasetype (1: liquid, 2: gas)
      _testData_001_999 = new (double temperature, double moleDensity, double pressure, double deltaPhiR_delta, double cv, int phase)[]
      {
      ( 250, 0.481094079389518, 999.999999999997, -1.08129955593473E-05, 20.9740811549403, 2 ),
      ( 250, 22301.141144734, 100000000.018446, 1.15723883481634, 25.3661406529799, 1 ),
      ( 300, 0.400908826359394, 999.999983038643, -3.56338265602545E-06, 21.0310676340106, 2 ),
      ( 300, 4.0092167218201, 9999.99832893728, -3.5603991515706E-05, 21.0316895662612, 2 ),
      ( 300, 40.104899245348, 99999.9999999999, -0.00035306081882672, 21.0379134245998, 2 ),
      ( 300, 20181.7367919766, 100000000.012405, 0.986486108362656, 24.5853662559567, 1 ),
      ( 350, 0.343634874650287, 1000.00001226041, 1.07612483682785E-07, 21.1317654253132, 1 ),
      ( 350, 3.43634535526448, 10000.0012906973, 1.09664766567622E-06, 21.1322252897078, 1 ),
      ( 350, 34.3630440108642, 100000, 1.30147485381069E-05, 21.1368254863273, 1 ),
      ( 350, 343.521179900445, 1000000.00000416, 0.000331078485372462, 21.1829604639813, 1 ),
      ( 350, 3367.562875532, 10000000.0085762, 0.0204261214539905, 21.6390924408087, 1 ),
      ( 350, 18424.8833596878, 100000000.007856, 0.865058821157579, 24.1039514701423, 1 ),
      ( 400, 0.300679912844838, 1000.00032978167, 2.0720338609525E-06, 21.292252000919, 1 ),
      ( 400, 3.00674314047071, 10000, 2.0734324473025E-05, 21.2926171465712, 1 ),
      ( 400, 30.0617797599726, 100000.000000014, 0.000208739225133068, 21.2962690038784, 1 ),
      ( 400, 300.013357771998, 1000000.00025766, 0.00222386949048355, 21.3328162804026, 1 ),
      ( 400, 2909.35328409678, 10000000.0051062, 0.0334961724174818, 21.6920815163578, 1 ),
      ( 400, 16956.0240811034, 100000000.004889, 0.773296303988351, 23.8387131402147, 1 ),
      ( 500, 0.240543522537341, 1000.00078123373, 3.73387746528809E-06, 21.803146457405, 1 ),
      ( 500, 2.40535455759388, 9999.99999999999, 3.73454352929406E-05, 21.8034050443825, 1 ),
      ( 500, 24.0454479513157, 100000.000000138, 0.000374121321048919, 21.8059906147966, 1 ),
      ( 500, 239.632344628139, 1000000.00163121, 0.00380622253862664, 21.8318127622293, 1 ),
      ( 500, 2305.67459783248, 10000000.0000037, 0.0432714091000607, 22.083913273061, 1 ),
      ( 500, 14649.693271062, 100000000.00192, 0.641975939099849, 23.771356893523, 1 ),
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
      ( 300, 10140.7842965315, 99999999.9990343, 2.9534160873668, 142.181167920399, 1 ),
      ( 350, 0.343715744910477, 999.999999999713, -0.000235172636876025, 139.402433485723, 2 ),
      ( 350, 9696.59226852769, 99999999.9993308, 2.54387296953456, 155.458087261305, 1 ),
      ( 400, 0.300725121425246, 999.999999198527, -0.000148218742550365, 155.350626932886, 2 ),
      ( 400, 3.01127359993198, 9999.99999882317, -0.00148379631495077, 155.391140375508, 2 ),
      ( 400, 9276.36137980792, 99999999.9999662, 2.24136303033931, 169.260967556436, 1 ),
      ( 500, 0.240560931857681, 999.999999947001, -6.85614113033114E-05, 184.79915013183, 2 ),
      ( 500, 2.40709515980979, 9999.99946212035, -0.000685794704549277, 184.813956316487, 2 ),
      ( 500, 24.2209917730247, 99999.9997155196, -0.00687618023464342, 184.963112534182, 2 ),
      ( 500, 8503.68700956804, 100000000.00022, 1.82870757579036, 195.6179018427, 1 ),
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
      ( 350, 0.344176267422748, 999.999992171139, -0.00157290056597483, 257.751276227958, 2 ),
      ( 350, 4833.19739337183, 1000000.00000399, -0.928901121885663, 281.719997836548, 1 ),
      ( 350, 4898.78023933724, 10000000.0000004, -0.298529642974583, 282.290147988076, 1 ),
      ( 350, 5314.15382044255, 99999999.9999997, 5.46640883917417, 287.589919056184, 1 ),
      ( 400, 0.300956173200364, 999.999998478326, -0.000915830604529866, 289.451041538417, 2 ),
      ( 400, 3.03483045686943, 9999.98369489396, -0.00923442920332895, 289.914348801275, 2 ),
      ( 400, 4540.23279953598, 100000.000000598, -0.993377420022197, 310.588504522373, 1 ),
      ( 400, 4550.26961309271, 999999.99999994, -0.933920278600486, 310.631412677808, 1 ),
      ( 400, 4640.13207911936, 10000000.0000272, -0.352000022430561, 311.099463071681, 1 ),
      ( 400, 5142.02075268331, 100000000.000001, 4.84751720748998, 315.997406561336, 1 ),
      ( 500, 0.240636177053666, 999.99989505495, -0.000381232715995974, 347.810062735239, 2 ),
      ( 500, 2.4146793233481, 9999.99999961517, -0.00382449819494491, 347.974214262405, 2 ),
      ( 500, 25.0451254107785, 100000.011963951, -0.0395558677252466, 349.686027325876, 2 ),
      ( 500, 3896.92920228933, 999999.999999985, -0.938273336215765, 365.458228774439, 1 ),
      ( 500, 4094.71822768376, 10000000.001224, -0.412549471509713, 365.261521461435, 1 ),
      ( 500, 4826.20142966055, 100000000.001008, 3.98413591244977, 369.220180791246, 1 ),
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
