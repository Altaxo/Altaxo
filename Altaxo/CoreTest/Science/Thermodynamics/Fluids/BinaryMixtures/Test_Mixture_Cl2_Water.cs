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
  /// Tests and test data for <see cref="Mixture_Cl2_Water"/>.
  /// </summary>
  /// <remarks>
  /// <para>Reference:</para>
  /// <para>The test data was created automatically using calls into the TREND.DLL of the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// </remarks>
  
  public class Test_Mixture_Cl2_Water : MixtureTestBase
  {

    public Test_Mixture_Cl2_Water()
    {
      _mixture = MixtureOfFluids.FromCASRegistryNumbersAndMoleFractions(new[] { ("7782-50-5", 0.5), ("7732-18-5", 0.5) });

      // TestData for 1 Permille to 999 Permille Molefraction contains:
      // 0. Temperature (Kelvin)
      // 1. Mole density (mol/m³)
      // 2. Pressure (Pa)
      // 3. delta*AlphaR_delta
      // 4. Isochoric heat capacity (J/mol K)
      // 5. Phasetype (1: liquid, 2: gas)
      _testData_001_999 = new (double temperature, double moleDensity, double pressure, double deltaPhiR_delta, double cv, int phase)[]
      {
      ( 300, 0.401105826744593, 999.999999766769, -0.00048261706131355, 25.4055492246924, 2 ),
      ( 300, 55272.221387348, 1000000.00004872, -0.992746587052995, 74.3277400801446, 1 ),
      ( 300, 55493.6049060899, 10000000.0001977, -0.927755234694117, 73.8095313400263, 1 ),
      ( 350, 0.343707973585918, 999.999999940793, -0.000200476552149957, 25.5968733965616, 2 ),
      ( 350, 3.44333882040147, 9999.99935127389, -0.00201784905283207, 25.8888767129542, 2 ),
      ( 350, 54000.6862141543, 1000000.00004761, -0.99363639441833, 70.006480091787, 1 ),
      ( 350, 54220.3705416373, 10000000.0000184, -0.936621777984023, 69.6980358839887, 1 ),
      ( 400, 0.300715706032675, 999.999999994033, -0.000104821132294765, 25.9600546546311, 2 ),
      ( 400, 3.01000527868613, 9999.99993799917, -0.00105097222568364, 26.062224297321, 2 ),
      ( 400, 30.3964590469346, 99999.9999987383, -0.0107920653023647, 27.1682063085304, 2 ),
      ( 400, 51986.3042051478, 1000000.00016267, -0.994216088461797, 65.3887148965634, 1 ),
      ( 400, 52238.5886348031, 10000000.0000098, -0.942440216604222, 65.1998459957896, 1 ),
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
      ( 300, 0.400998473534656, 999.999999589197, -0.000220476352525544, 25.4894493665248, 2 ),
      ( 350, 0.343677305639974, 999.999999955674, -0.000116705140202372, 25.9940628646822, 2 ),
      ( 350, 3.4403958592968, 9999.99953717169, -0.00116960139333628, 26.062144968858, 2 ),
      ( 350, 30278.6739100301, 10000000.0001679, -0.886508505042616, 49.8514687362608, 1 ),
      ( 400, 0.300703646628184, 999.999999992649, -7.01670500721724E-05, 26.4662717979167, 2 ),
      ( 400, 3.00893879698616, 9999.99992489648, -0.000702349136723251, 26.4969291477521, 2 ),
      ( 400, 30.2830258264186, 99999.9999999703, -0.00709212848645056, 26.8160835032906, 2 ),
      ( 400, 27928.4021669615, 10000000.0000246, -0.892338077437188, 46.8683039842735, 1 ),
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
      ( 300, 0.400956439628075, 999.99999999958, -0.000121110726859225, 25.6575355663373, 2 ),
      ( 300, 4.013945364144, 9999.99999573374, -0.00121241538602644, 25.6852662371923, 2 ),
      ( 300, 40.5883272591713, 99999.9999999999, -0.0122581866971954, 25.9672515851173, 2 ),
      ( 300, 19588.8241531463, 1000000.00000049, -0.979533846628898, 34.488424614042, 1 ),
      ( 300, 19951.4141101816, 10000000.000001, -0.799057912743531, 34.4716523016884, 1 ),
      ( 350, 0.343660489495962, 999.999999999894, -7.32240368123218E-05, 26.4099206728949, 2 ),
      ( 350, 3.43887280660696, 9999.99999892073, -0.000732668485237087, 26.4267943193486, 2 ),
      ( 350, 34.6186762452722, 99999.9881731033, -0.0073701166815227, 26.5972138690213, 2 ),
      ( 350, 373.027890696907, 999999.993745182, -0.0787945511397909, 28.495778278329, 2 ),
      ( 350, 17720.6097248453, 10000000.0000002, -0.806081545373811, 34.0520180247718, 1 ),
      ( 400, 0.300695219595366, 999.991605322306, -4.75892120236193E-05, 26.9781188221745, 2 ),
      ( 400, 3.00824116422667, 9999.99999975908, -0.000476048255988933, 26.9884523324228, 2 ),
      ( 400, 30.212380452988, 99999.9969275661, -0.00477583967372743, 27.0925123960946, 2 ),
      ( 400, 316.323987232944, 999999.999909324, -0.0494527074649973, 28.2120055611643, 2 ),
      ( 400, 14676.2325309677, 10000000.0000458, -0.795123912766166, 35.3165778925403, 1 ),
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
