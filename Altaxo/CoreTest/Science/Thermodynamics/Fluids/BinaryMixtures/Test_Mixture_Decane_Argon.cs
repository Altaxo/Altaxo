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
  /// Tests and test data for <see cref="Mixture_Decane_Argon"/>.
  /// </summary>
  /// <remarks>
  /// <para>Reference:</para>
  /// <para>The test data was created automatically using calls into the TREND.DLL of the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// </remarks>
  
  public class Test_Mixture_Decane_Argon : MixtureTestBase
  {

    public Test_Mixture_Decane_Argon()
    {
      _mixture = MixtureOfFluids.FromCASRegistryNumbersAndMoleFractions(new[] { ("124-18-5", 0.5), ("7440-37-1", 0.5) });

      // TestData for 1 Permille to 999 Permille Molefraction contains:
      // 0. Temperature (Kelvin)
      // 1. Mole density (mol/m³)
      // 2. Pressure (Pa)
      // 3. delta*AlphaR_delta
      // 4. Isochoric heat capacity (J/mol K)
      // 5. Phasetype (1: liquid, 2: gas)
      _testData_001_999 = new (double temperature, double moleDensity, double pressure, double deltaPhiR_delta, double cv, int phase)[]
      {
      ( 250, 0.481093145705346, 999.999999980539, -1.34379736732796E-05, 12.6551473896911, 2 ),
      ( 300, 0.400908037235771, 999.99999999997, -6.16079485013738E-06, 12.6852438585803, 2 ),
      ( 300, 4.00930262970535, 9999.99999970385, -6.15958664517941E-05, 12.6859097394831, 2 ),
      ( 300, 40.1152174687222, 99999.9963678995, -0.000614747644975502, 12.6925647038436, 2 ),
      ( 300, 24090.0104272741, 100000000.000212, 0.664198396817949, 15.7634180438797, 1 ),
      ( 350, 0.343634178747625, 999.999999999999, -2.38320295440935E-06, 12.7172372307131, 2 ),
      ( 350, 3.43641529744531, 9999.99999998728, -2.38222787233318E-05, 12.717680762452, 2 ),
      ( 350, 34.3714888676754, 99999.9998832331, -0.000237246797029252, 12.7221131786086, 2 ),
      ( 350, 344.416497364992, 1000000, -0.00227385723437716, 12.766142812912, 2 ),
      ( 350, 3478.06695436614, 10000000.0000299, -0.0119990560019137, 13.1728245745375, 1 ),
      ( 350, 21680.7822517032, 100000000.000297, 0.584967458421773, 15.2379939313852, 1 ),
      ( 400, 0.300679260791283, 999.996650372751, -2.78126087201803E-07, 12.7489988173855, 2 ),
      ( 400, 3.00680009558968, 9999.99999999997, -2.77393322456661E-06, 12.7493193655954, 2 ),
      ( 400, 30.068729565194, 99999.999999916, -2.70053286485406E-05, 12.7525229753448, 2 ),
      ( 400, 300.738226253107, 999999.99963486, -0.000196352692276188, 12.784370572699, 2 ),
      ( 400, 2990.00011802839, 10000000.0000864, 0.00561593185155211, 13.0828382636977, 1 ),
      ( 400, 19669.9889182766, 100000000.001001, 0.528618936916533, 14.8676960983489, 1 ),
      ( 500, 0.240542927787345, 1000.01641544403, 1.68170680914612E-06, 12.8074351789178, 1 ),
      ( 500, 2.40539294311852, 10000.0000000023, 1.68208787646846E-05, 12.8076320397195, 1 ),
      ( 500, 24.0502787453824, 100000.000023951, 0.000168617340259158, 12.8095999106072, 1 ),
      ( 500, 240.128647488506, 999999.999999999, 0.00172696140049357, 12.8292043790824, 1 ),
      ( 500, 2355.36599748435, 10000000.000018, 0.0212567416320162, 13.0177166174642, 1 ),
      ( 500, 16578.5939156527, 100000000.000003, 0.450927271745606, 14.3920701263096, 1 ),
      ( 600, 0.200452296163471, 1000.0195712805, 2.38435107996953E-06, 12.856728848529, 1 ),
      ( 600, 2.00448003897351, 10000.0000000046, 2.38454080651083E-05, 12.8568673159837, 1 ),
      ( 600, 20.0404948951513, 100000.000046982, 0.000238690264679697, 12.858251692205, 1 ),
      ( 600, 199.970768704953, 999999.999999999, 0.00241042708076597, 12.8720652513781, 1 ),
      ( 600, 1953.04901049939, 10000000.0000033, 0.0263581844807201, 13.0071295090554, 1 ),
      ( 600, 14350.9408775952, 100000000, 0.39679192724394, 14.1081831655107, 1 ),
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
      ( 350, 0.343677854611231, 999.999999999878, -0.000127233981988244, 135.18632063706, 2 ),
      ( 400, 0.300703889584472, 999.999999549076, -7.99070603501432E-05, 151.072425070987, 2 ),
      ( 400, 3.00920379194071, 9999.99999965387, -0.000799274459468007, 151.089330254073, 2 ),
      ( 500, 0.240552258319771, 999.999999975358, -3.4792204165219E-05, 180.296266864402, 2 ),
      ( 500, 2.40627580900148, 9999.99975552169, -0.000347806852818107, 180.302861699946, 2 ),
      ( 500, 24.1380649768262, 99999.9998828494, -0.00346656117472013, 180.369065731888, 2 ),
      ( 600, 0.200456337822884, 999.99999999991, -1.54497940985293E-05, 204.945913289665, 2 ),
      ( 600, 2.00484189966092, 9999.99998776471, -0.000154372034624854, 204.949117513845, 2 ),
      ( 600, 20.0760639920213, 100000, -0.00153117217826407, 204.981223025479, 2 ),
      ( 600, 203.32015840395, 999999.999999995, -0.0141005083446533, 205.307428654733, 2 ),
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
      ( 350, 0.344175272747034, 999.999992237632, -0.00157001964833929, 257.742646809563, 2 ),
      ( 400, 0.300955684775164, 999.999998490584, -0.000914213743005093, 289.44237968754, 2 ),
      ( 400, 3.03478007295802, 9999.98385944776, -0.00921798506225001, 289.904667519599, 2 ),
      ( 500, 0.240636021713086, 999.999895364713, -0.000380591989216632, 347.801020369937, 2 ),
      ( 500, 2.41466367191546, 9999.99999961842, -0.00381804571044529, 347.964816847377, 2 ),
      ( 500, 25.0433099754371, 100000.011770943, -0.0394862476747824, 349.672793241978, 2 ),
      ( 500, 4092.81241774295, 10000000.0011516, -0.412275929032605, 365.241527278238, 1 ),
      ( 500, 4825.31688188578, 100000000.001011, 3.98504955118107, 369.199588964803, 1 ),
      ( 600, 0.200491522375748, 999.999999487976, -0.000188658390721049, 397.041361113793, 2 ),
      ( 600, 2.00833064001533, 9999.99999999941, -0.00188896203612249, 397.112024496719, 2 ),
      ( 600, 20.4364127028538, 100000.000210404, -0.0191346158740319, 397.832853770331, 2 ),
      ( 600, 259.422256573054, 999999.998798241, -0.22730724531463, 407.21461950156, 2 ),
      ( 600, 3470.57654516976, 10000000.0044634, -0.422419602511828, 412.339095345385, 1 ),
      ( 600, 4541.27459211491, 100000000.000002, 3.41404046158788, 414.726889280543, 1 ),
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
