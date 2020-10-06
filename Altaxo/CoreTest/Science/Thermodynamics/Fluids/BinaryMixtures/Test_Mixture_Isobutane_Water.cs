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
  /// Tests and test data for <see cref="Mixture_Isobutane_Water"/>.
  /// </summary>
  /// <remarks>
  /// <para>Reference:</para>
  /// <para>The test data was created automatically using calls into the TREND.DLL of the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// </remarks>
  
  public class Test_Mixture_Isobutane_Water : MixtureTestBase
  {

    public Test_Mixture_Isobutane_Water()
    {
      _mixture = MixtureOfFluids.FromCASRegistryNumbersAndMoleFractions(new[] { ("75-28-5", 0.5), ("7732-18-5", 0.5) });

      // TestData for 1 Permille to 999 Permille Molefraction contains:
      // 0. Temperature (Kelvin)
      // 1. Mole density (mol/m³)
      // 2. Pressure (Pa)
      // 3. delta*AlphaR_delta
      // 4. Isochoric heat capacity (J/mol K)
      // 5. Phasetype (1: liquid, 2: gas)
      _testData_001_999 = new (double temperature, double moleDensity, double pressure, double deltaPhiR_delta, double cv, int phase)[]
      {
      ( 300, 0.401106138638843, 999.999999764034, -0.000483395461502051, 25.468932877288, 2 ),
      ( 300, 55177.8518837129, 1000000.0002261, -0.992734181702232, 74.4000748505919, 1 ),
      ( 300, 55398.9530824049, 9999999.99993143, -0.927631801064933, 73.8816306533842, 1 ),
      ( 350, 0.343708084436407, 999.999999940196, -0.000200800191438859, 25.6730847099684, 2 ),
      ( 350, 3.44335012832002, 9999.99934459343, -0.00202112760039173, 25.9655674950677, 2 ),
      ( 350, 53908.2869764659, 1000000.00003746, -0.99362548715479, 70.0889997952274, 1 ),
      ( 350, 54127.7558737623, 10000000.0000703, -0.936513335488914, 69.7803972599237, 1 ),
      ( 400, 0.3007157568827, 999.999999993969, -0.000104991401799227, 26.0492164728246, 2 ),
      ( 400, 3.0100104323444, 9999.99993735205, -0.00105268378850417, 26.1515500640051, 2 ),
      ( 400, 30.3970131928974, 99999.9999987044, -0.0108101000131093, 27.2594510194366, 2 ),
      ( 400, 51897.6182056985, 1000000.00015746, -0.994206204547432, 65.4816053498118, 1 ),
      ( 400, 52149.6988179241, 10000000.000015, -0.942342105298742, 65.2926638653252, 1 ),
      ( 500, 0.240557173358784, 999.999999998925, -4.08464548093402E-05, 27.0284533761695, 2 ),
      ( 500, 2.40645704354321, 9999.99998910641, -0.000408720793088718, 27.0512090854768, 2 ),
      ( 500, 24.154080607604, 99999.9999999997, -0.00411300532458612, 27.2842722616236, 2 ),
      ( 500, 251.594799183725, 999999.999999978, -0.0439096983264111, 30.1981936958887, 2 ),
      ( 500, 46365.1362762438, 9999999.99999578, -0.948118917192946, 58.1099951665212, 1 ),
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
      ( 300, 0.401052515685644, 999.980148954815, -0.000355785521645988, 57.0886996785132, 2 ),
      ( 300, 19482.0350065014, 1000000.00009812, -0.979421563297912, 86.5652977664048, 1 ),
      ( 300, 19643.4921375003, 10000000.0000085, -0.795907050861658, 86.525037343268, 1 ),
      ( 350, 0.34370209212099, 999.999999391139, -0.000189408139748262, 64.0788737697386, 2 ),
      ( 350, 3.44291368310613, 9999.99335618278, -0.00190064517115219, 64.1876301393492, 2 ),
      ( 350, 18238.3781683859, 1000000.00326403, -0.9811585772543, 88.469732438116, 1 ),
      ( 350, 18468.1381886866, 10000000.0000014, -0.813929813262048, 88.5207413718292, 1 ),
      ( 400, 0.300716833123131, 999.999999896048, -0.000114609461988411, 71.0403958108685, 2 ),
      ( 400, 3.01027904201397, 9999.99891791267, -0.0011478538379901, 71.0884318291627, 2 ),
      ( 400, 30.4229009599516, 99999.9939373852, -0.0116578006610501, 71.6010736833331, 2 ),
      ( 400, 17107.7997195476, 10000000.0003181, -0.824242525007782, 91.7379131543709, 1 ),
      ( 500, 0.240558276553927, 999.999999993852, -5.1472147066866E-05, 83.9248165099141, 2 ),
      ( 500, 2.40669829023275, 9999.99993760746, -0.000514956611136947, 83.939909599756, 2 ),
      ( 500, 24.1796765385446, 99999.9999999872, -0.00517323246119038, 84.0933546380423, 2 ),
      ( 500, 254.34911517302, 999999.999999994, -0.0542687976747709, 85.8889089012248, 2 ),
      ( 500, 13064.6590213784, 10000000.000001, -0.815880464917345, 101.198785693922, 1 ),
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
      ( 300, 0.401007419849038, 999.999999999557, -0.000249414907295762, 88.7695612374699, 2 ),
      ( 300, 4.01912122688813, 9999.99999538298, -0.00249985011590964, 88.8247630010463, 2 ),
      ( 300, 41.1439485479325, 99999.9999999993, -0.0255981334568935, 89.4134753788744, 2 ),
      ( 300, 9470.4255456561, 1000000.00000034, -0.957667435250085, 98.2381615551667, 1 ),
      ( 300, 9756.74750631756, 10000000.0022534, -0.589097286317182, 98.6774034239448, 1 ),
      ( 350, 0.34368730061826, 999.999999999955, -0.00015241787005431, 102.49631700517, 2 ),
      ( 350, 3.44160098935432, 9999.99999950737, -0.00152598283429915, 102.524999039939, 2 ),
      ( 350, 34.902571296365, 99999.9923085542, -0.0154452689621017, 102.820726398618, 2 ),
      ( 350, 419.496749684987, 999999.999910579, -0.180840097572145, 107.022600297089, 2 ),
      ( 350, 8769.98277733868, 10000000.0000002, -0.608169223068814, 110.640862929048, 1 ),
      ( 400, 0.300710420055013, 999.997836013122, -9.93248751627023E-05, 116.034643205121, 2 ),
      ( 400, 3.00979693003679, 9999.99999996569, -0.000993891100063154, 116.051301314514, 2 ),
      ( 400, 30.3718936026722, 99999.9994292337, -0.0100039333113957, 116.220583024439, 2 ),
      ( 400, 337.002522530486, 999999.999434552, -0.107779521262431, 118.242338309107, 2 ),
      ( 400, 7567.0163475487, 10000000.0000027, -0.602643184373562, 123.41459343115, 1 ),
      ( 500, 0.240555826914591, 999.997863609319, -4.73292127624431E-05, 140.821810232025, 2 ),
      ( 500, 2.40658360267929, 9999.9999999989, -0.000473362718181857, 140.82873767416, 2 ),
      ( 500, 24.1690198862147, 99999.9999869156, -0.00474060301113667, 140.8983023745, 2 ),
      ( 500, 252.704204181832, 999999.999999989, -0.0481185607847208, 141.623919282812, 2 ),
      ( 500, 4105.78440612147, 9999999.99999997, -0.414132799535902, 147.76877351653, 1 ),
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
