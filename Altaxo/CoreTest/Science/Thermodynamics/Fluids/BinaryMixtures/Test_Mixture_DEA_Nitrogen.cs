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
  /// Tests and test data for <see cref="Mixture_DEA_Nitrogen"/>.
  /// </summary>
  /// <remarks>
  /// <para>Reference:</para>
  /// <para>The test data was created automatically using calls into the TREND.DLL of the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// </remarks>
  
  public class Test_Mixture_DEA_Nitrogen : MixtureTestBase
  {

    public Test_Mixture_DEA_Nitrogen()
    {
      _mixture = MixtureOfFluids.FromCASRegistryNumbersAndMoleFractions(new[] { ("111-42-2", 0.5), ("7727-37-9", 0.5) });

      // TestData for 1 Permille to 999 Permille Molefraction contains:
      // 0. Temperature (Kelvin)
      // 1. Mole density (mol/m³)
      // 2. Pressure (Pa)
      // 3. delta*AlphaR_delta
      // 4. Isochoric heat capacity (J/mol K)
      // 5. Phasetype (1: liquid, 2: gas)
      _testData_001_999 = new (double temperature, double moleDensity, double pressure, double deltaPhiR_delta, double cv, int phase)[]
      {
      ( 350, 0.343632948608537, 1000.00000000115, 1.12756740651278E-06, 21.0251152114435, 1 ),
      ( 400, 0.300678344798651, 1000.0000000001, 2.70972483497226E-06, 21.1359689882254, 1 ),
      ( 400, 3.0067102553994, 10000.0000006987, 2.71070707985916E-05, 21.1363006770154, 1 ),
      ( 400, 30.059739771181, 100000.013422447, 0.000272052088766011, 21.139615057372, 1 ),
      ( 500, 0.24054236595793, 1000.00000000021, 3.97272959576462E-06, 21.5078929550894, 1 ),
      ( 500, 2.4053378371571, 10000.0000020815, 3.97323183311834E-05, 21.5081095316946, 1 ),
      ( 500, 24.0447684701184, 99999.9999999999, 0.0003978244976821, 21.5102742967201, 1 ),
      ( 500, 239.578370453216, 1000000.00035042, 0.0040277852398151, 21.5318213173346, 1 ),
      ( 500, 2302.40512245405, 10000000.0075631, 0.0447481129331859, 21.7371088384008, 1 ),
      ( 600, 0.200451918770914, 1000.00000000001, 4.23126836892477E-06, 22.0641105809192, 1 ),
      ( 600, 2.00444302012596, 10000.0000001325, 4.23154352404575E-05, 22.0642701769386, 1 ),
      ( 600, 20.036794235017, 100000.001343477, 0.000423428770340985, 22.0658656757005, 1 ),
      ( 600, 199.602208430156, 1000000.00000049, 0.00426135298687305, 22.0817741517845, 1 ),
      ( 600, 1918.18683637578, 10000000.0091863, 0.0450117793924981, 22.236019085856, 1 ),
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
      ( 500, 0.240553731861784, 999.999999974548, -4.0322294168267E-05, 142.311942268469, 2 ),
      ( 500, 2.40641075488901, 9999.99999997338, -0.000403269912484506, 142.31903073297, 2 ),
      ( 600, 0.200456658827227, 999.999999998773, -1.64557983531484E-05, 156.544826351194, 2 ),
      ( 600, 2.0048634642814, 9999.99998771843, -0.000164531280143503, 156.548372730423, 2 ),
      ( 600, 20.0783170626695, 99999.9999966794, -0.00164262010908495, 156.583866757302, 2 ),
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
      ( 500, 0.240626832410898, 999.999999983372, -0.000341228448131214, 263.127892302101, 2 ),
      ( 500, 2.41370948486115, 9999.99982485481, -0.00342304980993207, 263.255766076975, 2 ),
      ( 600, 0.200487955845197, 999.999999997628, -0.000169683193570772, 291.03037162835, 2 ),
      ( 600, 2.00795126789407, 9999.99997567225, -0.00169919651565348, 291.087863441033, 2 ),
      ( 600, 20.3969588743517, 99999.9999999916, -0.0172361593539327, 291.676597345376, 2 ),
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
