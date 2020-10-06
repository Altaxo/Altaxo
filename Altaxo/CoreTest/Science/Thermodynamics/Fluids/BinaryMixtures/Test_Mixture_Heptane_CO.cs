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
  /// Tests and test data for <see cref="Mixture_Heptane_CO"/>.
  /// </summary>
  /// <remarks>
  /// <para>Reference:</para>
  /// <para>The test data was created automatically using calls into the TREND.DLL of the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// </remarks>
  
  public class Test_Mixture_Heptane_CO : MixtureTestBase
  {

    public Test_Mixture_Heptane_CO()
    {
      _mixture = MixtureOfFluids.FromCASRegistryNumbersAndMoleFractions(new[] { ("142-82-5", 0.5), ("630-08-0", 0.5) });

      // TestData for 1 Permille to 999 Permille Molefraction contains:
      // 0. Temperature (Kelvin)
      // 1. Mole density (mol/m³)
      // 2. Pressure (Pa)
      // 3. delta*AlphaR_delta
      // 4. Isochoric heat capacity (J/mol K)
      // 5. Phasetype (1: liquid, 2: gas)
      _testData_001_999 = new (double temperature, double moleDensity, double pressure, double deltaPhiR_delta, double cv, int phase)[]
      {
      ( 200, 0.601376975215006, 999.999999958954, -2.64082470215175E-05, 20.8905651212598, 2 ),
      ( 250, 0.481094020731907, 999.999999999996, -1.06956417107926E-05, 20.915506681839, 2 ),
      ( 250, 4.81140316899686, 9999.99999996721, -0.000106916371891252, 20.9164541872526, 2 ),
      ( 250, 48.1601857501101, 99999.9996807338, -0.00106515863411776, 20.9259428945954, 2 ),
      ( 250, 22279.8842348449, 100000000, 1.15929701452049, 25.2979975641735, 1 ),
      ( 300, 0.400908793064976, 999.999959597874, -3.48490583732235E-06, 20.9628571756418, 2 ),
      ( 300, 4.00921355715091, 9999.9959873505, -3.48192326711236E-05, 20.9634770115622, 2 ),
      ( 300, 40.1045842981229, 100000, -0.000345215012501216, 20.9696798431192, 2 ),
      ( 300, 402.177533344065, 999999.999999816, -0.00315815107002436, 21.0321068270799, 2 ),
      ( 300, 4028.69253707099, 10000000.0165899, -0.00486971328343254, 21.6533912918092, 1 ),
      ( 350, 0.343634853435386, 1000.00001934564, 1.63658015918041E-07, 21.0538075865911, 1 ),
      ( 350, 3.43634341380409, 10000.0019896233, 1.65705669519515E-06, 21.0542663815992, 1 ),
      ( 350, 34.3628514440871, 100000, 1.86141741471538E-05, 21.0588558522811, 1 ),
      ( 350, 343.502115851736, 1000000.00000537, 0.000386591359935061, 21.1048805478033, 1 ),
      ( 350, 3365.9187611781, 10000000.0086377, 0.0209245534272758, 21.5597601331575, 1 ),
      ( 400, 0.300679898617322, 1000.00034203629, 2.11394328894763E-06, 21.2049261813588, 1 ),
      ( 400, 3.00674186679211, 10000, 2.11533703780328E-05, 21.2052907106956, 1 ),
      ( 400, 30.0616538211373, 100000.000000016, 0.000212924879823461, 21.2089363865966, 1 ),
      ( 400, 300.000971834916, 1000000.00027869, 0.0022652430482833, 21.2454201000184, 1 ),
      ( 400, 2908.32302342179, 10000000.0051004, 0.0338622791786146, 21.6039492484364, 1 ),
      ( 400, 16943.9929143642, 100000000, 0.774555433723911, 23.7472761823233, 1 ),
      ( 500, 0.24054351511654, 1000.00079052137, 3.75964212669847E-06, 21.6988121331403, 1 ),
      ( 500, 2.40535392697951, 10000, 3.76030457044301E-05, 21.699070449295, 1 ),
      ( 500, 24.0453860068707, 100000.000000143, 0.000376693859606808, 21.7016533035122, 1 ),
      ( 500, 239.626286678752, 1000000.00167828, 0.00383159499672187, 21.7274475199202, 1 ),
      ( 500, 2305.18297224042, 10000000.0000038, 0.0434939024711539, 21.9792170868172, 1 ),
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
      ( 300, 0.40097392319623, 999.99999993056, -0.000168194400796967, 89.2500666626372, 2 ),
      ( 300, 4.01582780317815, 10000.0000009873, -0.00168408289190535, 89.2881113998019, 2 ),
      ( 350, 0.343668935462228, 999.999999988547, -0.000101284548495425, 100.417580581123, 2 ),
      ( 350, 3.43982743368598, 9999.99988338973, -0.00101347018683925, 100.436556131223, 2 ),
      ( 350, 34.7174636821877, 99999.9999941628, -0.010198065356469, 100.628335585311, 2 ),
      ( 400, 0.300699292228643, 999.999999999203, -6.46194006308681E-05, 111.68463279143, 2 ),
      ( 400, 3.00874330148208, 9999.99999193261, -0.000646346025366659, 111.695163613708, 2 ),
      ( 400, 30.2640598650211, 99999.9999999998, -0.00647876531343408, 111.801206535056, 2 ),
      ( 500, 0.240550765881455, 999.985831003223, -2.85877598710033E-05, 132.631071561224, 2 ),
      ( 500, 2.40612660881242, 9999.9999995898, -0.000285819975137502, 132.635203520341, 2 ),
      ( 500, 24.123188896413, 99999.9950253512, -0.00285202738059732, 132.676669549763, 2 ),
      ( 500, 247.446526224271, 999999.999964412, -0.0278954703986774, 133.104623436557, 2 ),
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
      ( 300, 0.401356977095122, 999.999943690601, -0.00112470885451373, 157.590882703642, 2 ),
      ( 300, 6862.75810357315, 10000000.000008, -0.415824423280098, 178.91279512661, 1 ),
      ( 350, 0.343846594769512, 999.999993031145, -0.000620193281027535, 179.807712598406, 2 ),
      ( 350, 3.45789892842103, 9999.999999998, -0.00623658889227376, 180.080866772406, 2 ),
      ( 350, 6345.68395124594, 1000000.0000005, -0.945847705927282, 197.155242837208, 1 ),
      ( 350, 6464.74389233149, 10000000.0001667, -0.468450182806777, 197.605096950362, 1 ),
      ( 400, 0.30079243473155, 999.999998791384, -0.000376536194506039, 202.178700130289, 2 ),
      ( 400, 3.01819382749374, 9999.98696584288, -0.00377777514274597, 202.325475453316, 2 ),
      ( 400, 31.2919198607174, 99999.9999999995, -0.0391156029062952, 203.85472163589, 2 ),
      ( 400, 5867.99638146438, 1000000.00269977, -0.948759481643335, 217.021936034418, 1 ),
      ( 400, 6047.2642566283, 10000000.0023996, -0.502784791933845, 217.333763659769, 1 ),
      ( 500, 0.24058360632324, 999.999999929595, -0.000167367717578706, 243.56875265841, 2 ),
      ( 500, 2.40947130511457, 9999.9992734142, -0.00167584518159912, 243.621086411763, 2 ),
      ( 500, 24.469860697762, 99999.999991508, -0.0169811615784219, 244.153602802164, 2 ),
      ( 500, 301.165167402691, 999999.999921089, -0.201290964439132, 250.816830452693, 2 ),
      ( 500, 5088.21999635369, 10000000, -0.527254441501087, 255.522885394891, 1 ),
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
