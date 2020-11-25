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
  /// Tests and test data for <see cref="Mixture_Octane_CO"/>.
  /// </summary>
  /// <remarks>
  /// <para>Reference:</para>
  /// <para>The test data was created automatically using calls into the TREND.DLL of the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// </remarks>
  
  public class Test_Mixture_Octane_CO : MixtureTestBase
  {

    public Test_Mixture_Octane_CO()
    {
      _mixture = MixtureOfFluids.FromCASRegistryNumbersAndMoleFractions(new[] { ("111-65-9", 0.5), ("630-08-0", 0.5) });

      // TestData for 1 Permille to 999 Permille Molefraction contains:
      // 0. Temperature (Kelvin)
      // 1. Mole density (mol/m³)
      // 2. Pressure (Pa)
      // 3. delta*AlphaR_delta
      // 4. Isochoric heat capacity (J/mol K)
      // 5. Phasetype (1: liquid, 2: gas)
      _testData_001_999 = new (double temperature, double moleDensity, double pressure, double deltaPhiR_delta, double cv, int phase)[]
      {
      ( 250, 0.481094027159438, 999.999999999914, -1.07090018047924E-05, 20.9351314771519, 2 ),
      ( 250, 4.8114038116867, 9999.99999891725, -0.000107049933965321, 20.9360801438796, 2 ),
      ( 300, 0.400908795626105, 999.999961824494, -3.49129412987813E-06, 20.9855764677907, 2 ),
      ( 300, 4.00921381313435, 9999.9962097668, -3.4883079986003E-05, 20.9861969630197, 2 ),
      ( 300, 40.1046097670178, 99999.9999999999, -0.000345849854800169, 20.9924063980406, 2 ),
      ( 300, 402.179950428021, 999999.999999858, -0.00316414204602786, 21.0549003494314, 2 ),
      ( 300, 20157.1336475522, 100000000, 0.988910739718013, 24.5350178652636, 1 ),
      ( 350, 0.343634854401486, 1000.0000189509, 1.60901715287178E-07, 21.0797670004557, 1 ),
      ( 350, 3.43634350842675, 10000.00195066, 1.62952078030573E-06, 21.0802262330819, 1 ),
      ( 350, 34.3628608131442, 100000, 1.8341518398054E-05, 21.0848200840439, 1 ),
      ( 350, 343.502961560018, 1000000.00000531, 0.000384128396524781, 21.1308889352501, 1 ),
      ( 350, 3365.92850681442, 10000000.0087115, 0.020921597464782, 21.5862005344945, 1 ),
      ( 400, 0.300679898838234, 1000.00034137954, 2.11322297409906E-06, 21.2340133201121, 1 ),
      ( 400, 3.00674188839134, 9999.99999999999, 2.11461866265783E-05, 21.2343781707859, 1 ),
      ( 400, 30.0616559221631, 100000.000000016, 0.000212854974388814, 21.2380270616804, 1 ),
      ( 400, 300.00112500012, 1000000.00027782, 0.00226473134300009, 21.2745430597472, 1 ),
      ( 400, 2908.29645303621, 10000000.0051387, 0.033871724611831, 21.6333870396903, 1 ),
      ( 400, 16939.0882602516, 99999999.9999999, 0.775069250080039, 23.7778876174684, 1 ),
      ( 500, 0.240543514822771, 1000.00079039761, 3.76083945470567E-06, 21.7335084026269, 1 ),
      ( 500, 2.40535389815684, 10000, 3.76150288699082E-05, 21.7337669254989, 1 ),
      ( 500, 24.0453831028744, 100000.000000143, 0.000376814676574559, 21.7363518469415, 1 ),
      ( 500, 239.625975569141, 1000000.0016786, 0.00383289828470718, 21.7621667320249, 1 ),
      ( 500, 2305.13858425676, 10000000.0000038, 0.0435139960961544, 22.0141361399635, 1 ),
      ( 500, 14637.7478964043, 99999999.9999999, 0.643315893016606, 23.7002189425396, 1 ),
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
      ( 300, 0.400989640733043, 999.999999838081, -0.000207384673077093, 100.611341725182, 2 ),
      ( 350, 0.343676607196722, 999.9999999741, -0.000123604826642714, 113.398109539797, 2 ),
      ( 350, 3.44059758627205, 9999.99973508931, -0.00123708598825667, 113.424449446579, 2 ),
      ( 400, 0.300703423655033, 999.999999994758, -7.83577191685929E-05, 126.228667335853, 2 ),
      ( 400, 3.00915740301023, 9999.99994686183, -0.000783870856656525, 126.243251689893, 2 ),
      ( 400, 30.3064480977088, 99999.9994642625, -0.00786835760440504, 126.390318151815, 2 ),
      ( 500, 0.240552198007742, 999.999999999848, -3.45414896405771E-05, 149.979404527428, 2 ),
      ( 500, 2.40626992260993, 9999.99999849366, -0.000345361439755966, 149.985068609725, 2 ),
      ( 500, 24.1376218159924, 99999.9847612737, -0.00344826454407252, 150.041944568851, 2 ),
      ( 500, 248.994451357053, 1000000.00000179, -0.0339387578097597, 150.632200294218, 2 ),
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
      ( 300, 0.401538992065195, 999.999992053469, -0.00157749246960913, 180.317751344621, 2 ),
      ( 300, 6180.46931734839, 10000000.0000248, -0.351334750305341, 207.253255826377, 1 ),
      ( 350, 0.343927331493397, 999.999976041587, -0.00085479704331604, 205.756109395456, 2 ),
      ( 350, 3.4661944985807, 9999.98570932963, -0.00861492939513924, 206.164992529856, 2 ),
      ( 350, 5848.5837268934, 10000000.0000846, -0.41245033078057, 228.355334534798, 1 ),
      ( 400, 0.300832762905722, 999.999996046971, -0.000510540843102962, 231.244901665649, 2 ),
      ( 400, 3.02229262815169, 9999.99999999949, -0.00512884589575916, 231.464735283376, 2 ),
      ( 400, 31.7809215434929, 99999.9999882502, -0.0539003877425423, 233.795748532515, 2 ),
      ( 400, 5366.95485944258, 1000000.00019636, -0.943975832958127, 250.142055189484, 1 ),
      ( 400, 5505.77353405998, 10000000.0009368, -0.453883866352422, 250.530219838494, 1 ),
      ( 400, 6208.71820394887, 100000000, 3.84285428371167, 254.78532453106, 1 ),
      ( 500, 0.240596438998825, 999.999999788269, -0.000220695721325936, 278.233365789557, 2 ),
      ( 500, 2.41076364727889, 9999.99778866071, -0.0022110181381498, 278.311929710205, 2 ),
      ( 500, 24.6088247781968, 99999.9998585969, -0.0225321909102756, 279.115932908973, 2 ),
      ( 500, 4744.74755315599, 10000000.0000033, -0.493032373800405, 293.152454171792, 1 ),
      ( 500, 5812.21158958358, 100000000, 3.1385854022461, 296.576793020105, 1 ),
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
