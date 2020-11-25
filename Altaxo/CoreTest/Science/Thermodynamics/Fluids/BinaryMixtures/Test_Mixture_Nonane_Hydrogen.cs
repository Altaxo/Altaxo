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
  /// Tests and test data for <see cref="Mixture_Nonane_Hydrogen"/>.
  /// </summary>
  /// <remarks>
  /// <para>Reference:</para>
  /// <para>The test data was created automatically using calls into the TREND.DLL of the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// </remarks>
  
  public class Test_Mixture_Nonane_Hydrogen : MixtureTestBase
  {

    public Test_Mixture_Nonane_Hydrogen()
    {
      _mixture = MixtureOfFluids.FromCASRegistryNumbersAndMoleFractions(new[] { ("111-84-2", 0.5), ("1333-74-0", 0.5) });

      // TestData for 1 Permille to 999 Permille Molefraction contains:
      // 0. Temperature (Kelvin)
      // 1. Mole density (mol/m³)
      // 2. Pressure (Pa)
      // 3. delta*AlphaR_delta
      // 4. Isochoric heat capacity (J/mol K)
      // 5. Phasetype (1: liquid, 2: gas)
      _testData_001_999 = new (double temperature, double moleDensity, double pressure, double deltaPhiR_delta, double cv, int phase)[]
      {
      ( 250, 0.481085800015419, 1000.00000000065, 6.39658446563571E-06, 20.1642323732539, 1 ),
      ( 300, 0.400905051052182, 1000.00000000404, 5.85354468353529E-06, 20.7154621377593, 1 ),
      ( 300, 4.00883931436493, 10000.000036586, 5.85364729395722E-05, 20.7156125505416, 1 ),
      ( 300, 40.0672816679057, 100000, 0.000585467940357249, 20.7171166090092, 1 ),
      ( 350, 0.343633092376145, 1000.00000000224, 5.29633584598832E-06, 20.9831266461166, 1 ),
      ( 350, 3.43616713137026, 10000.0000224814, 5.29637666888785E-05, 20.9832552340161, 1 ),
      ( 350, 34.3452992602528, 100000, 0.000529678802602011, 20.9845410281851, 1 ),
      ( 350, 341.822838003922, 1000000.00060036, 0.005301209157887, 20.997390112472, 1 ),
      ( 350, 3261.4602456521, 10000000, 0.0536228759204252, 21.1246828983957, 1 ),
      ( 400, 0.300679107936985, 1000.00000000129, 4.79045213273516E-06, 21.1109815279998, 1 ),
      ( 400, 3.00666145024832, 10000.0000129382, 4.79046320429663E-05, 21.1110937697003, 1 ),
      ( 400, 30.0536574003885, 99999.9999999999, 0.000479057573754435, 21.1122160939729, 1 ),
      ( 400, 299.246594412495, 1000000.00295658, 0.00479188046817485, 21.1234298370778, 1 ),
      ( 400, 2868.65930593266, 10000000.0000001, 0.0481570526831945, 21.2344275711474, 1 ),
      ( 500, 0.240543466109616, 1000, 3.96386283361488E-06, 21.2326204749032, 1 ),
      ( 500, 2.40534904212251, 10000.0000000341, 3.96385234287532E-05, 21.2327096335214, 1 ),
      ( 500, 24.0449130754937, 100000.000339777, 0.00039637450870475, 21.2336011293635, 1 ),
      ( 500, 239.594984221499, 1000000.00000002, 0.00396274754780641, 21.2425069689452, 1 ),
      ( 500, 2313.86304543555, 10000000.000056, 0.0395794130307272, 21.3305818151851, 1 ),
      ( 500, 17310.54510143, 100000000.000341, 0.389583269917014, 22.1025754775182, 1 ),
      ( 600, 0.200453015376654, 1000, 3.34295333988673E-06, 21.3511401895052, 1 ),
      ( 600, 2.00446998064637, 10000.0000000143, 3.34293820000626E-05, 21.351213740551, 1 ),
      ( 600, 20.0386713914636, 100000.000142055, 0.000334278496088369, 21.3519491723472, 1 ),
      ( 600, 199.786155319627, 1000000, 0.00334129039981142, 21.3592955913875, 1 ),
      ( 600, 1939.95222094109, 10000000.000003, 0.0332919373997049, 21.431942300082, 1 ),
      ( 600, 15145.5904223918, 99999999.9999997, 0.323511948319605, 22.0758407426194, 1 ),
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
      ( 300, 0.400973982659742, 999.997954487449, -0.000166057546353311, 111.842526567043, 2 ),
      ( 350, 0.343669416695363, 999.999999999973, -0.000100399748352076, 126.342277253969, 2 ),
      ( 350, 3.43980453459371, 9999.99999895873, -0.00100453698278322, 126.35940645848, 2 ),
      ( 400, 0.300699809148275, 999.999999515924, -6.40533243483081E-05, 140.665006373413, 2 ),
      ( 400, 3.008732984536, 9999.99999987235, -0.000640635538989836, 140.674747803101, 2 ),
      ( 400, 30.2622350228361, 99999.9999988779, -0.00641658451462145, 140.772785081409, 2 ),
      ( 500, 0.240551196810221, 999.999999972894, -2.80944332152454E-05, 167.051455222109, 2 ),
      ( 500, 2.40612016548766, 9999.99973221037, -0.000280858318040991, 167.055419528298, 2 ),
      ( 500, 24.1219844224167, 99999.9999613501, -0.00279995853720003, 167.095170719534, 2 ),
      ( 500, 247.247848521563, 999999.999976601, -0.0271121059326051, 167.501794865984, 2 ),
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
      ( 350, 0.344025429890951, 999.999962295482, -0.00113514138714396, 231.759949920114, 2 ),
      ( 400, 0.300883485123137, 999.999993924962, -0.00067446970691289, 260.250427034098, 2 ),
      ( 400, 3.02734783543835, 9999.99999999831, -0.00678559356339262, 260.554451972228, 2 ),
      ( 500, 0.240614336379815, 999.999962206365, -0.000290496890317643, 312.881374315152, 2 ),
      ( 500, 2.41246942172069, 9999.99999996917, -0.00291196855741166, 312.989290908246, 2 ),
      ( 500, 24.7947616105242, 100000.002980273, -0.0298578296006428, 314.102434770919, 2 ),
      ( 500, 4399.86311434953, 10000000.0000011, -0.453291085634606, 327.706430012226, 1 ),
      ( 500, 5266.01042835077, 100000000.002033, 3.56786863486035, 331.908085538627, 1 ),
      ( 600, 0.200483260496959, 999.999996484447, -0.000147451776651305, 357.55989499291, 2 ),
      ( 600, 2.00749998470881, 9999.99999999987, -0.00147596308394959, 357.606555436655, 2 ),
      ( 600, 20.3487148332277, 100000.00002893, -0.0149073262568545, 358.080437191624, 2 ),
      ( 600, 240.956752484233, 999999.999995699, -0.168092627338601, 363.759647422839, 2 ),
      ( 600, 3644.98922782639, 10000000.000407, -0.450056813993864, 370.289454424999, 1 ),
      ( 600, 4944.56447723329, 100000000.017483, 3.05402133667653, 373.030663698953, 1 ),
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
