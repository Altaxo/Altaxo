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
  /// Tests and test data for <see cref="Mixture_DEA_Argon"/>.
  /// </summary>
  /// <remarks>
  /// <para>Reference:</para>
  /// <para>The test data was created automatically using calls into the TREND.DLL of the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// </remarks>
  
  public class Test_Mixture_DEA_Argon : MixtureTestBase
  {

    public Test_Mixture_DEA_Argon()
    {
      _mixture = MixtureOfFluids.FromCASRegistryNumbersAndMoleFractions(new[] { ("111-42-2", 0.5), ("7440-37-1", 0.5) });

      // TestData for 1 Permille to 999 Permille Molefraction contains:
      // 0. Temperature (Kelvin)
      // 1. Mole density (mol/m³)
      // 2. Pressure (Pa)
      // 3. delta*AlphaR_delta
      // 4. Isochoric heat capacity (J/mol K)
      // 5. Phasetype (1: liquid, 2: gas)
      _testData_001_999 = new (double temperature, double moleDensity, double pressure, double deltaPhiR_delta, double cv, int phase)[]
      {
      ( 350, 0.343634183006592, 999.999999998184, -2.39418644077331E-06, 12.6533140059247, 2 ),
      ( 400, 0.300679263335712, 999.986277992964, -2.85252085272905E-07, 12.6810085535323, 2 ),
      ( 400, 3.00680031365473, 9999.99999999978, -2.84526628957226E-06, 12.6813302671799, 2 ),
      ( 400, 30.0687511803394, 99999.999995998, -2.77229760150137E-05, 12.6845455123924, 2 ),
      ( 500, 0.240542928899003, 1000, 1.67831571403806E-06, 12.7226724746488, 1 ),
      ( 500, 2.40539302694966, 10000.0000000371, 1.6787217561552E-05, 12.7228701106767, 1 ),
      ( 500, 24.0502869314091, 100000.000413136, 0.000168278103143574, 12.7248457206045, 1 ),
      ( 500, 240.12952262393, 1000000.00000008, 0.00172331186867033, 12.7445266248994, 1 ),
      ( 500, 2355.49878636726, 10000000.0000018, 0.0211991705228133, 12.9337161896272, 1 ),
      ( 600, 0.200452296769293, 1000.01951151741, 2.3825552305116E-06, 12.7506037823291, 1 ),
      ( 600, 2.00448007738859, 10000.0000000046, 2.38274337143856E-05, 12.7507427959291, 1 ),
      ( 600, 20.0404985555606, 100000.000046656, 0.000238508761446682, 12.752132627325, 1 ),
      ( 600, 199.971165489686, 1000000, 0.00240843928180904, 12.766000115783, 1 ),
      ( 600, 1953.11446208384, 10000000.0000032, 0.0263237910112531, 12.9015465437963, 1 ),
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
      ( 500, 0.240554283205226, 999.999999993633, -4.26141727539423E-05, 137.91497877248, 2 ),
      ( 500, 2.40646597900111, 9999.99999999847, -0.000426208878735859, 137.922338168311, 2 ),
      ( 600, 0.20045704128422, 999.999999999635, -1.83636919196821E-05, 151.883429692708, 2 ),
      ( 600, 2.00490173720159, 9999.99999633113, -0.000183617813240602, 151.887008681425, 2 ),
      ( 600, 20.0821716620361, 99999.9999999998, -0.00183424618334596, 151.922840080478, 2 ),
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
      ( 500, 0.240626835222711, 999.999999980051, -0.000341240129508627, 263.119098613148, 2 ),
      ( 500, 2.41370976962892, 9999.99978963794, -0.0034231673732749, 263.246974858961, 2 ),
      ( 600, 0.200487957225438, 999.999999997219, -0.000169690076807052, 291.021049232764, 2 ),
      ( 600, 2.00795140681045, 9999.99997142334, -0.00169926558051083, 291.078544457897, 2 ),
      ( 600, 20.3969737212087, 99999.999999988, -0.0172368747029075, 291.667313486041, 2 ),
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
