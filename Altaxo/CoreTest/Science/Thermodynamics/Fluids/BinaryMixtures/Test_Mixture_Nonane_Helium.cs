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
  /// Tests and test data for <see cref="Mixture_Nonane_Helium"/>.
  /// </summary>
  /// <remarks>
  /// <para>Reference:</para>
  /// <para>The test data was created automatically using calls into the TREND.DLL of the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// </remarks>
  
  public class Test_Mixture_Nonane_Helium : MixtureTestBase
  {

    public Test_Mixture_Nonane_Helium()
    {
      _mixture = MixtureOfFluids.FromCASRegistryNumbersAndMoleFractions(new[] { ("111-84-2", 0.5), ("7440-59-7", 0.5) });

      // TestData for 1 Permille to 999 Permille Molefraction contains:
      // 0. Temperature (Kelvin)
      // 1. Mole density (mol/m³)
      // 2. Pressure (Pa)
      // 3. delta*AlphaR_delta
      // 4. Isochoric heat capacity (J/mol K)
      // 5. Phasetype (1: liquid, 2: gas)
      _testData_001_999 = new (double temperature, double moleDensity, double pressure, double deltaPhiR_delta, double cv, int phase)[]
      {
      ( 250, 0.481086067853061, 1000.00000000001, 5.8398455096989E-06, 12.6352647197058, 1 ),
      ( 300, 0.400905478638615, 1000, 4.78698670538244E-06, 12.6623910181281, 1 ),
      ( 300, 4.0088820749885, 10000.0000000281, 4.78693775550069E-05, 12.6624803561311, 1 ),
      ( 300, 40.0715597333492, 100000.0002802, 0.000478644794773381, 12.6633735346252, 1 ),
      ( 350, 0.343633527550325, 1000, 4.02993879841387E-06, 12.6911534314299, 1 ),
      ( 350, 3.43621064786421, 10000.0000000122, 4.02990020346099E-05, 12.6912286966464, 1 ),
      ( 350, 34.3496499972884, 100000.000121634, 0.000402951412026373, 12.6919811988011, 1 ),
      ( 350, 342.257108082832, 1000000, 0.00402564111179487, 12.6994912462188, 1 ),
      ( 350, 3304.59460251891, 10000000.0000018, 0.0398701011932861, 12.7731226565589, 1 ),
      ( 350, 25075.8043782964, 100000000, 0.370384403978813, 13.38935974944, 1 ),
      ( 400, 0.300679507256957, 1000.01836493013, 3.46245086975408E-06, 12.719699946885, 1 ),
      ( 400, 3.00670138053317, 10000.0000000059, 3.46235665741574E-05, 12.7197646496793, 1 ),
      ( 400, 30.0576487232984, 100000.000058393, 0.000346205033837079, 12.7204115640553, 1 ),
      ( 400, 299.644084881587, 1000000, 0.00345898181905494, 12.7268692394128, 1 ),
      ( 400, 2907.13518805661, 10000000.0000002, 0.0342847128717822, 12.7903204706346, 1 ),
      ( 400, 22779.6338676609, 99999999.9999998, 0.319953384995881, 13.3318606063433, 1 ),
      ( 500, 0.240543782698481, 1000.01109060685, 2.67355608690782E-06, 12.7724034883059, 1 ),
      ( 500, 2.40538007861296, 10000.0000000017, 2.67350654835026E-05, 12.7724534899698, 1 ),
      ( 500, 24.0480150952067, 100000.000016776, 0.000267330623235644, 12.7729534350564, 1 ),
      ( 500, 239.903583681122, 999999.999999999, 0.00267130223241497, 12.77794568116, 1 ),
      ( 500, 2343.31314836386, 10000000.0019808, 0.0265142703179019, 12.8271598792868, 1 ),
      ( 500, 19249.8791286622, 100000000.000737, 0.249589345747436, 13.2596062308789, 1 ),
      ( 600, 0.200453258128768, 1000.00733701956, 2.1556947311412E-06, 12.8171323697937, 1 ),
      ( 600, 2.00449377866383, 10000.0000000006, 2.15566522042349E-05, 12.8171727065829, 1 ),
      ( 600, 20.0410499852604, 100000.000005948, 0.000215552734692515, 12.8175760261891, 1 ),
      ( 600, 200.022819969599, 1000000, 0.0021541487814918, 12.8216043681334, 1 ),
      ( 600, 1962.52933382398, 10000000.0002249, 0.021404854588424, 12.8614097369449, 1 ),
      ( 600, 16662.1462790669, 99999999.9999998, 0.203048488031466, 13.2184428000279, 1 ),
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
      ( 300, 0.400922912675361, 999.999523186343, -3.86979632786308E-05, 107.808687140624, 2 ),
      ( 350, 0.343642012807416, 999.995622353741, -2.06622114337118E-05, 122.190478256729, 2 ),
      ( 350, 3.43705860250903, 9999.99999999108, -0.000206420334061895, 122.192603328039, 2 ),
      ( 400, 0.300683654811857, 999.999999998576, -1.033140960201E-05, 136.464233379599, 2 ),
      ( 400, 3.00711566230632, 9999.99999999956, -0.000103148359612534, 136.465553837811, 2 ),
      ( 400, 30.0986062523561, 99999.9999999961, -0.00101504433488805, 136.478795905622, 2 ),
      ( 500, 0.240544456398876, 999.999691114219, -7.23404413901584E-08, 162.816749911226, 2 ),
      ( 500, 2.40544589892231, 10000, -6.20834794028213E-07, 162.817431374247, 2 ),
      ( 500, 24.0543484207156, 100000.000006673, 3.96790432584393E-06, 162.824259181025, 1 ),
      ( 500, 240.308514200702, 1000000.00004046, 0.000981756557311915, 162.89363884537, 1 ),
      ( 600, 0.200452834642558, 1000.00000000002, 4.22687146875842E-06, 185.181702063781, 1 ),
      ( 600, 2.00445213414243, 10000.0000002339, 4.23331119667801E-05, 185.182153435433, 1 ),
      ( 600, 20.036759649777, 100000.003445655, 0.000429722124002101, 185.186673567274, 1 ),
      ( 600, 199.47809406939, 1000000.00002, 0.00489078668677214, 185.232434411124, 1 ),
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
      ( 350, 0.344024286253624, 999.999848514671, -0.00113182073946863, 231.751432973379, 2 ),
      ( 400, 0.300882910172417, 999.999965112075, -0.000672560097742936, 260.241911012232, 2 ),
      ( 400, 3.0272889048328, 9999.99335160519, -0.00676625466417365, 260.54487495859, 2 ),
      ( 500, 0.240614146771805, 999.999962622146, -0.000289709102503264, 312.872864210948, 2 ),
      ( 500, 2.41245026033348, 9999.99999997002, -0.0029040489786849, 312.980406024006, 2 ),
      ( 500, 24.7926218159525, 100000.002935757, -0.0297740988289894, 314.089581728737, 2 ),
      ( 500, 4142.46525101823, 1000000.00000047, -0.941932056378939, 327.769467417289, 1 ),
      ( 500, 4398.25570251692, 10000000.0000006, -0.453091282249516, 327.679404354411, 1 ),
      ( 500, 5266.4527215791, 100000000.002088, 3.56748501091838, 331.877467654712, 1 ),
      ( 600, 0.200483180844588, 999.999996522713, -0.00014705453323123, 357.551334527399, 2 ),
      ( 600, 2.00749197711768, 9999.99999999991, -0.00147198011798506, 357.597833274102, 2 ),
      ( 600, 20.3478696475279, 100000.000022891, -0.0148664086493742, 358.070053862923, 2 ),
      ( 600, 240.790602376077, 999999.99999588, -0.167518595384829, 363.725150867143, 2 ),
      ( 600, 3642.07099772598, 10000000.0004126, -0.449616168888152, 370.271918075212, 1 ),
      ( 600, 4944.8751325737, 100000000.017568, 3.05376664806827, 373.003552393876, 1 ),
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
