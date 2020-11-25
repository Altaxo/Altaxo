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
  /// Tests and test data for <see cref="Mixture_Nonane_Argon"/>.
  /// </summary>
  /// <remarks>
  /// <para>Reference:</para>
  /// <para>The test data was created automatically using calls into the TREND.DLL of the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// </remarks>
  
  public class Test_Mixture_Nonane_Argon : MixtureTestBase
  {

    public Test_Mixture_Nonane_Argon()
    {
      _mixture = MixtureOfFluids.FromCASRegistryNumbersAndMoleFractions(new[] { ("111-84-2", 0.5), ("7440-37-1", 0.5) });

      // TestData for 1 Permille to 999 Permille Molefraction contains:
      // 0. Temperature (Kelvin)
      // 1. Mole density (mol/m³)
      // 2. Pressure (Pa)
      // 3. delta*AlphaR_delta
      // 4. Isochoric heat capacity (J/mol K)
      // 5. Phasetype (1: liquid, 2: gas)
      _testData_001_999 = new (double temperature, double moleDensity, double pressure, double deltaPhiR_delta, double cv, int phase)[]
      {
      ( 250, 0.481093141582265, 999.999999999566, -1.34294035531219E-05, 12.6354349779802, 2 ),
      ( 250, 4.81151291695173, 9999.99998790719, -0.000134283972400776, 12.6365569822256, 2 ),
      ( 300, 0.400908035445526, 999.999999999971, -6.15632940155582E-06, 12.6625128311382, 2 ),
      ( 300, 4.0093024507576, 9999.99999970475, -6.1551236063443E-05, 12.663178127415, 2 ),
      ( 300, 40.1151996507898, 99999.9969904047, -0.00061430375275403, 12.669827243123, 2 ),
      ( 300, 24096.2570048219, 100000000.000216, 0.663766979426545, 15.739050232167, 1 ),
      ( 350, 0.343634177922064, 1000, -2.38084761697917E-06, 12.6912522601154, 2 ),
      ( 350, 3.43641521656872, 9999.99999998732, -2.37987441226436E-05, 12.6916954516297, 2 ),
      ( 350, 34.371480841007, 99999.9998836646, -0.000237013325446873, 12.6961244654047, 2 ),
      ( 350, 344.415755188159, 1000000, -0.00227170724786841, 12.7401200847152, 2 ),
      ( 350, 3478.04708431627, 10000000.0000296, -0.0119934115619315, 13.1464820286067, 1 ),
      ( 350, 21685.7407534444, 100000000.000296, 0.584605051437762, 15.2106598214838, 1 ),
      ( 400, 0.3006792604331, 999.996665882908, -2.76958201312699E-07, 12.7197864267633, 2 ),
      ( 400, 3.00680006051619, 9999.99999999998, -2.76226853257941E-06, 12.720106756061, 2 ),
      ( 400, 30.0687261004447, 99999.999999917, -2.68901040915791E-05, 12.7233081767595, 2 ),
      ( 400, 300.737921690913, 999999.999643667, -0.000195340174828703, 12.7551339003237, 2 ),
      ( 400, 2990.00462883606, 10000000.0000853, 0.00561441475019767, 13.053392952119, 1 ),
      ( 400, 19674.0017404659, 100000000.000995, 0.52830715103428, 14.8373578961491, 1 ),
      ( 500, 0.240542927783809, 1000.01640849192, 1.68172121365268E-06, 12.7724781105124, 1 ),
      ( 500, 2.40539294279204, 10000.0000000023, 1.68210144949798E-05, 12.7726748593749, 1 ),
      ( 500, 24.0502787329737, 100000.000023928, 0.000168617856295354, 12.774641610721, 1 ),
      ( 500, 240.128666021975, 999999.999999999, 0.00172688408579332, 12.7942348723713, 1 ),
      ( 500, 2355.3833872087, 10000000.0000178, 0.0212492017243608, 12.9826364901881, 1 ),
      ( 500, 16581.3935002453, 100000000.000001, 0.45068229875012, 14.3562980083515, 1 ),
      ( 600, 0.200452296256636, 1000.01955905509, 2.38389538159215E-06, 12.8172017862104, 1 ),
      ( 600, 2.00448004811754, 10000.0000000046, 2.38408461614778E-05, 12.8173401845402, 1 ),
      ( 600, 20.0404958197595, 100000.000046914, 0.000238644116678884, 12.8187238692762, 1 ),
      ( 600, 199.970871112647, 1000000, 0.00240991373330293, 12.832530491787, 1 ),
      ( 600, 1953.06691041828, 10000000.0000033, 0.0263487778760823, 12.9675244287978, 1 ),
      ( 600, 14353.0305408737, 99999999.9999997, 0.396588567765963, 14.0680349620619, 1 ),
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
      ( 300, 0.400979447220217, 999.99783984332, -0.000181968017569422, 107.813353768505, 2 ),
      ( 350, 0.343671405861379, 999.990617654493, -0.000108471061860244, 122.19298163171, 2 ),
      ( 350, 3.44007523007564, 9999.99999870213, -0.00108542945484142, 122.215150534177, 2 ),
      ( 400, 0.300700478641808, 999.999999757149, -6.85646437537183E-05, 136.465782406547, 2 ),
      ( 400, 3.00886212021606, 9999.99999992215, -0.000685810092759197, 136.478246524086, 2 ),
      ( 400, 30.2761220512679, 99999.9999980718, -0.00687459004301437, 136.603821037114, 2 ),
      ( 500, 0.24055106144263, 999.999999986782, -2.9816813525746E-05, 162.817597490376, 2 ),
      ( 500, 2.40615615245636, 9999.99986832789, -0.000298094818700976, 162.822560046001, 2 ),
      ( 500, 24.1261306437424, 99999.9999785038, -0.00297361176609987, 162.872341794463, 2 ),
      ( 500, 247.726643658126, 999999.999997422, -0.0289946796822274, 163.384113711156, 2 ),
      ( 600, 0.200455831019296, 999.999999999228, -1.29215775070886E-05, 185.182349881096, 2 ),
      ( 600, 2.00479128905173, 9999.99999243158, -0.000129131104848651, 185.184823368794, 2 ),
      ( 600, 20.071072510954, 100000, -0.00128286266505498, 185.20959455113, 2 ),
      ( 600, 202.888367918472, 999999.999992772, -0.0120023002289354, 185.460251531673, 2 ),
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
      ( 350, 0.344025324708888, 999.999997083888, -0.00113484059959197, 231.751633182257, 2 ),
      ( 400, 0.300883428937113, 999.999266455073, -0.00067428717242241, 260.242019506151, 2 ),
      ( 400, 3.02734220648385, 9999.99369451321, -0.00678374704151008, 260.545962088408, 2 ),
      ( 500, 0.240614315115848, 999.999962339461, -0.000290413111394029, 312.872903985968, 2 ),
      ( 500, 2.4124673734491, 9999.99999996942, -0.00291112655084586, 312.980792724641, 2 ),
      ( 500, 24.7945346284255, 100000.002980817, -0.02984895284715, 314.093640589287, 2 ),
      ( 500, 4400.81905200564, 10000000.0000013, -0.453409843217152, 327.703975249462, 1 ),
      ( 500, 5267.04885642975, 100000000.002051, 3.56696803319256, 331.908244087355, 1 ),
      ( 600, 0.200483250194858, 999.999996496992, -0.000147404967559915, 357.551352764354, 2 ),
      ( 600, 2.00749903218127, 9999.99999999991, -0.00147549386318704, 357.598001328437, 2 ),
      ( 600, 20.3486154109192, 100000.000027508, -0.0149025176464199, 358.07176059587, 2 ),
      ( 600, 240.93788748602, 999999.999995649, -0.168027494307337, 363.749126286511, 2 ),
      ( 600, 3645.51741092288, 10000000.0004063, -0.450136495373403, 370.285191610846, 1 ),
      ( 600, 4945.45181406704, 100000000.01748, 3.0532939260743, 373.027780391809, 1 ),
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
