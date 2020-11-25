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
  /// Tests and test data for <see cref="Mixture_DEA_SO2"/>.
  /// </summary>
  /// <remarks>
  /// <para>Reference:</para>
  /// <para>The test data was created automatically using calls into the TREND.DLL of the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// </remarks>
  
  public class Test_Mixture_DEA_SO2 : MixtureTestBase
  {

    public Test_Mixture_DEA_SO2()
    {
      _mixture = MixtureOfFluids.FromCASRegistryNumbersAndMoleFractions(new[] { ("111-42-2", 0.5), ("7446-09-5", 0.5) });

      // TestData for 1 Permille to 999 Permille Molefraction contains:
      // 0. Temperature (Kelvin)
      // 1. Mole density (mol/m³)
      // 2. Pressure (Pa)
      // 3. delta*AlphaR_delta
      // 4. Isochoric heat capacity (J/mol K)
      // 5. Phasetype (1: liquid, 2: gas)
      _testData_001_999 = new (double temperature, double moleDensity, double pressure, double deltaPhiR_delta, double cv, int phase)[]
      {
      ( 350, 0.343666740976518, 999.999999999955, -9.14241487834766E-05, 33.5981342038327, 2 ),
      ( 350, 3.43950002215533, 9999.99999912393, -0.000914902387458822, 33.6334382551897, 2 ),
      ( 350, 19317.7557498687, 10000000.0030239, -0.822114263122649, 51.0483607358744, 1 ),
      ( 400, 0.300698090142873, 999.999999999989, -5.71463482274043E-05, 35.3492822575242, 2 ),
      ( 400, 3.0085289954253, 9999.9999998746, -0.000571685357448778, 35.3658540931297, 2 ),
      ( 400, 30.2416549604087, 99999.9986836915, -0.00573924683257227, 35.5328795569132, 2 ),
      ( 400, 16324.0085486252, 10000000.0000005, -0.815804491005095, 51.407191777023, 1 ),
      ( 500, 0.240550967787844, 999.998197452582, -2.59516748841053E-05, 38.4651117254707, 2 ),
      ( 500, 2.4060717404607, 9999.99999999397, -0.000259547414722961, 38.469891929019, 2 ),
      ( 500, 24.1171408785966, 99999.9999260801, -0.00259849918249533, 38.5178122485026, 2 ),
      ( 500, 247.040065508803, 999999.999999991, -0.0262926599349107, 39.0093028487523, 2 ),
      ( 500, 3418.80794480936, 10000000.0000044, -0.296407610608132, 45.6283240354125, 2 ),
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
      ( 350, 14261.3138370307, 9999999.99999612, -0.759043714070629, 148.95418274344, 1 ),
      ( 400, 13592.0107746183, 9999999.99999545, -0.778781144798337, 160.021779929763, 1 ),
      ( 500, 0.240576337238088, 999.999999999557, -0.000131401792747991, 150.80305268402, 2 ),
      ( 500, 2.40861565199938, 9999.99999546533, -0.00131544492513655, 150.851766133582, 2 ),
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
      ( 350, 10071.9337730894, 10000.0000031695, -0.999658818920398, 245.965492024426, 1 ),
      ( 350, 10072.3392537171, 99999.999999293, -0.996588326573323, 245.968011790908, 1 ),
      ( 350, 10076.3850131232, 1000000.00006135, -0.965896963927767, 245.993197173642, 1 ),
      ( 350, 10115.9710730466, 10000000.0000098, -0.660304167480617, 246.243771232521, 1 ),
      ( 400, 9729.03512794305, 9999.99999806542, -0.999690944781808, 268.382484026084, 1 ),
      ( 400, 9729.54122081065, 100000.000000203, -0.996909608586465, 268.384713441162, 1 ),
      ( 400, 9734.58740579836, 1000000.00011292, -0.969112105746649, 268.407023188036, 1 ),
      ( 400, 9783.64383528996, 9999999.99999439, -0.692669815655939, 268.63138059672, 1 ),
      ( 500, 0.240626999349189, 999.999999976787, -0.000341916216076593, 263.144899810237, 2 ),
      ( 500, 2.41372626838655, 9999.9997550098, -0.0034299736100493, 263.273092803417, 2 ),
      ( 500, 8975.84750299631, 100000.000001197, -0.997320088994398, 302.488524284238, 1 ),
      ( 500, 8984.32978567759, 1000000.00036091, -0.973226191502211, 302.497835346002, 1 ),
      ( 500, 9064.98069588129, 9999999.99999939, -0.734643974271983, 302.610666025759, 1 ),
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
