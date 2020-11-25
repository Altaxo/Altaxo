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
  /// Tests and test data for <see cref="Mixture_Decane_Hydrogen"/>.
  /// </summary>
  /// <remarks>
  /// <para>Reference:</para>
  /// <para>The test data was created automatically using calls into the TREND.DLL of the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// </remarks>
  
  public class Test_Mixture_Decane_Hydrogen : MixtureTestBase
  {

    public Test_Mixture_Decane_Hydrogen()
    {
      _mixture = MixtureOfFluids.FromCASRegistryNumbersAndMoleFractions(new[] { ("124-18-5", 0.5), ("1333-74-0", 0.5) });

      // TestData for 1 Permille to 999 Permille Molefraction contains:
      // 0. Temperature (Kelvin)
      // 1. Mole density (mol/m³)
      // 2. Pressure (Pa)
      // 3. delta*AlphaR_delta
      // 4. Isochoric heat capacity (J/mol K)
      // 5. Phasetype (1: liquid, 2: gas)
      _testData_001_999 = new (double temperature, double moleDensity, double pressure, double deltaPhiR_delta, double cv, int phase)[]
      {
      ( 250, 0.481085815868376, 1000.0000000068, 6.36363180313611E-06, 20.1839446783419, 1 ),
      ( 300, 0.400905055475786, 1000.00000000023, 5.84251057266386E-06, 20.7381930805007, 1 ),
      ( 300, 4.00883975640799, 10000.0000018458, 5.8426199200603E-05, 20.7383442496741, 1 ),
      ( 300, 40.0673255543866, 99999.9999999999, 0.000584371980634893, 20.7398558760232, 1 ),
      ( 350, 0.343633092242048, 1000.00000000018, 5.29672608269175E-06, 21.0091115308831, 1 ),
      ( 350, 3.43616711781858, 10000.0000018101, 5.29677106250257E-05, 21.0092407543298, 1 ),
      ( 350, 34.3452977630059, 100000.018147779, 0.000529722515777227, 21.0105329080601, 1 ),
      ( 350, 341.822544937746, 1000000.00453617, 0.0053020710873157, 21.023445957028, 1 ),
      ( 400, 0.300679105931248, 1000.00000000012, 4.79712285528144E-06, 21.1401938236209, 1 ),
      ( 400, 3.0066612496115, 10000.0000012122, 4.79713659546676E-05, 21.1403066296356, 1 ),
      ( 400, 30.0536372735365, 100000.012114035, 0.000479727650405111, 21.1414346001872, 1 ),
      ( 400, 299.244517919995, 1000000.00003512, 0.00479885282174154, 21.1527050945327, 1 ),
      ( 400, 2868.38894561968, 10000000.0000002, 0.0482558468405349, 21.2642923097277, 1 ),
      ( 500, 0.240543463115066, 1000.00000000008, 3.97606787344406E-06, 21.2675774244331, 1 ),
      ( 500, 2.4053487485315, 10000.0000007584, 3.97605858331769E-05, 21.267667061576, 1 ),
      ( 500, 24.0448837085168, 100000.0075596, 0.000397596361461523, 21.2685633442558, 1 ),
      ( 500, 239.592039376213, 1000000.00001071, 0.00397508733569448, 21.277517199107, 1 ),
      ( 500, 2313.55969827869, 10000000.0000326, 0.0397157196318933, 21.3660832672187, 1 ),
      ( 600, 0.200453012553406, 1000.00000000005, 3.35676155398175E-06, 21.3906671105834, 1 ),
      ( 600, 2.00446970386257, 10000.0000004562, 3.35674699189302E-05, 21.3907410838355, 1 ),
      ( 600, 20.0386437182155, 100000.004543591, 0.000335659966567783, 21.3914807384145, 1 ),
      ( 600, 199.783392972684, 1000000.0000032, 0.00335516330830828, 21.3988694510768, 1 ),
      ( 600, 1939.68012083802, 10000000.0000013, 0.0334368885386222, 21.47194377859, 1 ),
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
      ( 350, 0.343704754789384, 999.998838527855, -0.000203204450522354, 139.339120413768, 2 ),
      ( 400, 0.300719623415674, 999.99999985709, -0.000129938609811494, 155.273525186117, 2 ),
      ( 400, 3.01072120111868, 9999.99999993419, -0.00130059131920909, 155.304926060348, 2 ),
      ( 500, 0.240559248944391, 999.999999986586, -6.15660539715234E-05, 184.530774704373, 2 ),
      ( 500, 2.40692658831368, 9999.99986425528, -0.000615806769415682, 184.54258729727, 2 ),
      ( 500, 24.2038490725063, 99999.9999999317, -0.00617278701589736, 184.661476990711, 2 ),
      ( 500, 8735.24210459986, 100000000.000114, 1.75372377526061, 192.09990293035, 1 ),
      ( 600, 0.200460216145288, 999.999999999923, -3.2511495333905E-05, 209.216968562152, 2 ),
      ( 600, 2.00518883843851, 9999.9999887918, -0.000325081401918369, 209.222426433693, 2 ),
      ( 600, 20.1106783180935, 99999.9999999999, -0.00324745036113667, 209.277189703327, 2 ),
      ( 600, 207.107839197809, 1000000.00000752, -0.0321288674524644, 209.842186482419, 2 ),
      ( 600, 8101.17050245757, 100000000.010176, 1.47437945959986, 215.685713831159, 1 ),
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
      ( 250, 5418.60882146882, 10000000.0000009, -0.112154257352346, 227.599040612896, 1 ),
      ( 250, 5699.74100003804, 100000000.00773, 7.44053926929977, 233.950281235595, 1 ),
      ( 350, 0.344176154655994, 999.999992191793, -0.00157257343880649, 257.751132891039, 2 ),
      ( 350, 4834.23635368592, 1000000.00000568, -0.928916402254037, 281.701963387196, 1 ),
      ( 350, 4899.84709566237, 9999999.999999, -0.298682375922259, 282.271416192645, 1 ),
      ( 350, 5315.37853822988, 99999999.9999993, 5.46491891218805, 287.566784395801, 1 ),
      ( 400, 0.300956117597626, 999.999998481251, -0.000915646020098079, 289.450878017042, 2 ),
      ( 400, 3.03482471266277, 9999.98370810585, -0.00923255393357459, 289.914081562465, 2 ),
      ( 400, 4551.2758329752, 999999.999999585, -0.933934887851171, 310.617943084705, 1 ),
      ( 400, 4641.17067098752, 10000000.0000272, -0.352145030566635, 311.085154515434, 1 ),
      ( 400, 5143.22640296913, 100000000.000001, 4.84614646075627, 315.978519071892, 1 ),
      ( 500, 0.240636160106658, 999.999894712996, -0.000381162316865463, 347.809522697653, 2 ),
      ( 500, 2.41467760705928, 9999.99999961231, -0.00382379013981729, 347.973638448894, 2 ),
      ( 500, 25.0449288100207, 100000.011921468, -0.0395483282944756, 349.685066483614, 2 ),
      ( 500, 3897.88463203534, 1000000.00000003, -0.938288466343069, 365.450714895878, 1 ),
      ( 500, 4095.70871041209, 10000000.0012733, -0.41269153717701, 365.252924660971, 1 ),
      ( 500, 4827.36732523788, 100000000.000981, 3.98293215445266, 369.206865758204, 1 ),
      ( 600, 0.200491580440625, 999.999999484804, -0.000188943379146411, 397.049916901507, 2 ),
      ( 600, 2.00833640520693, 9999.9999999994, -0.00189182268233785, 397.12071623765, 2 ),
      ( 600, 20.4370322823794, 100000.000212671, -0.0191643478085465, 397.842953034999, 2 ),
      ( 600, 259.594539436165, 999999.998653496, -0.227820048061018, 407.250579211435, 2 ),
      ( 600, 3473.91962127448, 10000000.0042976, -0.422975425991818, 412.349088375933, 1 ),
      ( 600, 4543.26303554515, 100000000.000002, 3.41210859498138, 414.736223349985, 1 ),
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
