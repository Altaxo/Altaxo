﻿#region Copyright

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

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Science.Thermodynamics.Fluids
{

  /// <summary>
  /// Tests and test data for <see cref="Mixture_Propane_Helium"/>.
  /// </summary>
  /// <remarks>
  /// <para>Reference:</para>
  /// <para>The test data was created automatically using calls into the TREND.DLL of the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// </remarks>
  [TestFixture]
  public class Test_Mixture_Propane_Helium : MixtureTestBase
  {

    public Test_Mixture_Propane_Helium()
    {
      _mixture = MixtureOfFluids.FromCASRegistryNumbersAndMoleFractions(new[] { ("74-98-6", 0.5), ("7440-59-7", 0.5) });

      // TestData for 1 Permille to 999 Permille Molefraction contains:
      // 0. Temperature (Kelvin)
      // 1. Mole density (mol/m³)
      // 2. Pressure (Pa)
      // 3. delta*AlphaR_delta
      // 4. Isochoric heat capacity (J/mol K)
      // 5. Phasetype (1: liquid, 2: gas)
      _testData_001_999 = new(double temperature, double moleDensity, double pressure, double deltaPhiR_delta, double cv, int phase)[]
      {
      ( 150, 0.801806935702097, 1000.00000000307, 9.80265095789808E-06, 12.4998032218956, 1 ),
      ( 150, 8.01736204370457, 10000.000026228, 9.80262147879022E-05, 12.4999857125086, 1 ),
      ( 200, 0.601356671488352, 1000.00000000002, 7.35863352857786E-06, 12.5069552029863, 1 ),
      ( 200, 6.01316848361784, 10000.0000002304, 7.35856479911571E-05, 12.5070922938195, 1 ),
      ( 200, 60.09189479905, 100000.002290023, 0.000735787534696192, 12.5084628016285, 1 ),
      ( 200, 596.972884490038, 1000000.00000167, 0.00735077300108102, 12.5221278573822, 1 ),
      ( 200, 5605.76914219095, 10000000.0151475, 0.0727539459068429, 12.6547914493748, 1 ),
      ( 200, 36072.086063353, 100000000, 0.667109286653271, 13.6599896651431, 1 ),
      ( 250, 0.481086079282512, 1000.00000000001, 5.8160877709377E-06, 12.5152269700747, 1 ),
      ( 250, 4.81060898690435, 10000.0000000725, 5.81602683062212E-05, 12.5153354398712, 1 ),
      ( 250, 48.0809266700857, 100000.000721359, 0.000581541667101204, 12.5164198596635, 1 ),
      ( 250, 478.310256487786, 1000000.00000013, 0.0058092436384252, 12.5272362815681, 1 ),
      ( 250, 4549.45884554088, 10000000.0004427, 0.0574639614424782, 12.6326645384295, 1 ),
      ( 250, 31471.9235935787, 100000000.00205, 0.528628766189211, 13.4672103196696, 1 ),
      ( 250, 90857.5314015152, 1000000000, 4.29498072312078, 16.6105440162527, 1 ),
      ( 300, 0.400905486710257, 1000, 4.76685308271152E-06, 12.5246297779509, 1 ),
      ( 300, 4.00888288207245, 10000.0000000275, 4.76680440139279E-05, 12.5247187624935, 1 ),
      ( 300, 40.0716403616225, 100000.000273774, 0.000476631728516535, 12.5256084077599, 1 ),
      ( 300, 399.007556007027, 1000000.00000001, 0.00476141798202379, 12.5344848792483, 1 ),
      ( 300, 3828.65873546809, 10000000.0000222, 0.0471223095803979, 12.6212880511481, 1 ),
      ( 300, 27927.4167844732, 100000000, 0.435533407410256, 13.330606009407, 1 ),
      ( 350, 0.343633533517865, 1000, 4.01257272873386E-06, 12.5345912002768, 1 ),
      ( 350, 3.43621124456594, 10000.0000000119, 4.01253439754968E-05, 12.5346661601442, 1 ),
      ( 350, 34.3497096152365, 100000.000118777, 0.000401215095405947, 12.5354156099078, 1 ),
      ( 350, 342.263017967021, 1000000, 0.00400830451717405, 12.5428952432987, 1 ),
      ( 350, 3305.13760722099, 10000000.0000016, 0.0396992597876902, 12.61623330403, 1 ),
      ( 350, 25106.9420216331, 100000000, 0.368684852485506, 13.2303998084884, 1 ),
      ( 400, 0.300679511828079, 1000.01818343892, 3.44724727338244E-06, 12.5445416566135, 1 ),
      ( 400, 3.00670183760984, 10000.0000000057, 3.44715420224041E-05, 12.5446060917272, 1 ),
      ( 400, 30.0576943954553, 100000.000056994, 0.000344685024737257, 12.5452503301652, 1 ),
      ( 400, 299.648616797352, 1000000, 0.00344380540477687, 12.5516813346263, 1 ),
      ( 400, 2907.5555284475, 10000000.0000001, 0.0341351880781038, 12.6148745048352, 1 ),
      ( 400, 22804.9715608676, 99999999.9999998, 0.31848683750088, 13.15451962839, 1 ),
      ( 500, 0.240543785659145, 1000.01097680002, 2.66148874864431E-06, 12.5630985058252, 1 ),
      ( 500, 2.40538036885585, 10000.0000000017, 2.66143982273435E-05, 12.5631482945506, 1 ),
      ( 500, 24.0480441013066, 100000.000016362, 0.000266124127854643, 12.5636461108392, 1 ),
      ( 500, 239.906466200209, 1000000, 0.00265925495749145, 12.5686171284429, 1 ),
      ( 500, 2343.58437519032, 10000000.0018805, 0.0263954701537321, 12.6176248405627, 1 ),
      ( 500, 19267.5103260919, 100000000.000662, 0.248445879046678, 13.0484651769509, 1 ),
      ( 500, 77937.8134003833, 1000000000, 2.08636370672805, 15.2472225413041, 1 ),
      ( 600, 0.200453260157698, 1000.00725982027, 2.14577122695384E-06, 12.5792465073257, 1 ),
      ( 600, 2.00449397756818, 10000.0000000006, 2.14574208580518E-05, 12.579286668725, 1 ),
      ( 600, 20.0410698653547, 100000.000005798, 0.000214560553154513, 12.5796882348466, 1 ),
      ( 600, 200.024797688326, 999999.999999998, 0.00214424011493947, 12.5836990838538, 1 ),
      ( 600, 1962.71733907806, 10000000.0002131, 0.0213070160074744, 12.6233336120698, 1 ),
      ( 600, 16675.0839581162, 100000000, 0.202115080124317, 12.9789843217721, 1 ),
      ( 600, 73828.9725098629, 1000000000, 1.71510888028544, 14.9381740970326, 1 ),
      };

      // TestData for 500 Permille to 500 Permille Molefraction contains:
      // 0. Temperature (Kelvin)
      // 1. Mole density (mol/m³)
      // 2. Pressure (Pa)
      // 3. delta*AlphaR_delta
      // 4. Isochoric heat capacity (J/mol K)
      // 5. Phasetype (1: liquid, 2: gas)
      _testData_500_500 = new(double temperature, double moleDensity, double pressure, double deltaPhiR_delta, double cv, int phase)[]
      {
      ( 200, 0.601377894151204, 999.999999985556, -2.79316876295027E-05, 30.0878292872854, 2 ),
      ( 200, 6.01529095471405, 9999.99999998409, -0.000279286273846495, 30.0890545625757, 2 ),
      ( 250, 0.481094424217999, 999.999999999419, -1.15297462310214E-05, 34.2252443139883, 2 ),
      ( 250, 4.81144333358551, 9999.9999942201, -0.000115258630933879, 34.2259418809254, 2 ),
      ( 250, 48.1642142213249, 99999.9999933819, -0.00114870532123965, 34.2328951837171, 2 ),
      ( 300, 0.40090894810729, 999.999999999999, -3.86706129929173E-06, 38.9277063754736, 2 ),
      ( 300, 4.0092288994665, 9999.99999997482, -3.86412929434235E-05, 38.9281841386438, 2 ),
      ( 300, 40.1061198397308, 99999.999777568, -0.00038348419085882, 38.9329499371828, 2 ),
      ( 300, 402.333369234516, 999999.99962375, -0.00354425353092177, 38.9794369313424, 2 ),
      ( 500, 0.240543443694497, 1000.00000000001, 4.05521645637248E-06, 58.1642174982196, 1 ),
      ( 500, 2.40534682635973, 10000.0000000676, 4.05597421842602E-05, 58.1644217789781, 1 ),
      ( 500, 24.0446732314508, 100000.000701065, 0.000406353398616274, 58.1664629735342, 1 ),
      ( 500, 239.553226975605, 1000000.00000019, 0.00413775132013071, 58.1867159341711, 1 ),
      ( 600, 0.200452763261782, 1000.00000000001, 4.57601997652691E-06, 66.2387483215009, 1 ),
      ( 600, 2.00444525661826, 10000.0000000674, 4.57643931621849E-05, 66.2389093449904, 1 ),
      ( 600, 20.0361920699855, 100000.000684354, 0.000458062012759134, 66.2405187559595, 1 ),
      ( 600, 199.53154000918, 1000000.00000014, 0.00462161957151711, 66.2565314115214, 1 ),
      ( 600, 1909.4970991431, 10000000.0112666, 0.0497722095799021, 66.4091324228981, 1 ),
      ( 600, 12048.6220331318, 100000000.000007, 0.663706424956284, 67.5061423267595, 1 ),
      };

      // TestData for 999 Permille to 1 Permille Molefraction contains:
      // 0. Temperature (Kelvin)
      // 1. Mole density (mol/m³)
      // 2. Pressure (Pa)
      // 3. delta*AlphaR_delta
      // 4. Isochoric heat capacity (J/mol K)
      // 5. Phasetype (1: liquid, 2: gas)
      _testData_999_001 = new(double temperature, double moleDensity, double pressure, double deltaPhiR_delta, double cv, int phase)[]
      {
      ( 200, 0.60169971940426, 999.999988983363, -0.000562776976401135, 47.6816403875488, 2 ),
      ( 200, 6.04784016640887, 9999.99610448368, -0.00565973724617984, 47.8014022845908, 2 ),
      ( 250, 0.481220589695521, 999.999974257424, -0.000273704769857501, 55.9418784645347, 2 ),
      ( 250, 4.82412523503497, 9999.99959626407, -0.0027438054818911, 56.0029928065898, 2 ),
      ( 250, 49.5025227676301, 100000.003582594, -0.0281528093718931, 56.6430152444529, 2 ),
      ( 250, 12684.0766120557, 1000000.0000009, -0.962071430815513, 65.9734211965667, 1 ),
      ( 250, 12949.8539729314, 10000000.0000247, -0.628498608304371, 66.3037539551097, 1 ),
      ( 250, 14440.2991832674, 100000000.000002, 2.33157139762617, 69.4967203769725, 1 ),
      ( 300, 0.400969063100496, 999.999996793601, -0.000153790748866539, 65.333946564644, 2 ),
      ( 300, 4.01525649995375, 9999.99999999991, -0.00153975773970042, 65.3633566264855, 2 ),
      ( 300, 40.7255689392137, 100000.00015586, -0.0155879753179949, 65.6646888863739, 2 ),
      ( 300, 11620.0355946259, 10000000.0086258, -0.654986084307735, 73.6786195158491, 1 ),
      ( 300, 13697.3928057492, 100000000.000051, 1.92688837543256, 76.526493453011, 1 ),
      ( 300, 18256.0639749464, 999999999.999999, 20.9602318614786, 90.6811263330266, 1 ),
      ( 350, 0.343667465454008, 999.999999968226, -9.47226166835302E-05, 75.2854433737484, 2 ),
      ( 350, 3.43960921438618, 9999.99530608539, -0.000947807999976292, 75.3011171203485, 2 ),
      ( 350, 34.6943804142737, 99999.999626895, -0.00953725547682722, 75.4601319763176, 2 ),
      ( 350, 382.861368566964, 1000000, -0.102456030864056, 77.3238642239483, 2 ),
      ( 350, 9977.09254453167, 10000000.0018339, -0.655576100056275, 82.6727236294999, 1 ),
      ( 350, 12990.9798403326, 99999999.9999992, 1.64518086084285, 84.7550561537338, 1 ),
      ( 400, 0.30069916682509, 999.999999878662, -6.19173622101761E-05, 85.2263267945513, 2 ),
      ( 400, 3.00866894250492, 9999.99876250375, -0.000619363263662861, 85.2348701715631, 2 ),
      ( 400, 30.2560288443693, 99999.9999931078, -0.00621277865883769, 85.3210733992537, 2 ),
      ( 400, 321.319668662008, 1000000.01534058, -0.0642323601019205, 86.2667613851243, 2 ),
      ( 400, 7552.41443298847, 10000000.0011483, -0.60187493547688, 93.0087927852299, 1 ),
      ( 400, 12320.6219210969, 100000000, 1.44046567008925, 93.3395648126299, 1 ),
      ( 500, 0.240551434670339, 999.999999991708, -2.90832173226178E-05, 103.765632051863, 2 ),
      ( 500, 2.40614417956568, 9999.99991659248, -0.000290835835567999, 103.768651584837, 2 ),
      ( 500, 24.1246155329389, 99999.9999999895, -0.00290871647907565, 103.798914183392, 2 ),
      ( 500, 247.757440489397, 1000000.00002104, -0.0291131592843955, 104.108411681652, 2 ),
      ( 500, 3241.78807061749, 10000000, -0.257988389676353, 107.379768999519, 1 ),
      ( 500, 11093.55225083, 100000000, 1.16832654880848, 109.834129875792, 1 ),
      ( 500, 17173.7050581379, 1000000000, 13.0065546628627, 121.262287509075, 1 ),
      ( 600, 0.20045652704821, 999.999999999323, -1.41086166967024E-05, 119.898387513543, 2 ),
      ( 600, 2.00481980068993, 9999.99999324111, -0.00014106597041002, 119.899905685411, 2 ),
      ( 600, 20.0736462692226, 100000, -0.00140863201640929, 119.915074475478, 2 ),
      ( 600, 203.27416542536, 999999.999386267, -0.0138751844527367, 120.065441469541, 2 ),
      ( 600, 2243.49699407164, 10000000, -0.106512291236721, 121.381217996073, 2 ),
      ( 600, 10026.4366901164, 100000000.009923, 0.999251629400589, 124.554081704503, 1 ),
      };
    }

    [Test]
    public override void CASNumberAttribute_Test()
    {
      base.CASNumberAttribute_Test();
    }

    [Test]
    public override void Constants_Test()
    {
      base.Constants_Test();
    }

    [Test]
    public override void DeltaPhiRDelta_001_999_Test()
    {
      base.DeltaPhiRDelta_001_999_Test();
    }

    [Test]
    public override void DeltaPhiRDelta_500_500_Test()
    {
      base.DeltaPhiRDelta_500_500_Test();
    }

    [Test]
    public override void DeltaPhiRDelta_999_001_Test()
    {
      base.DeltaPhiRDelta_999_001_Test();
    }
  }
}