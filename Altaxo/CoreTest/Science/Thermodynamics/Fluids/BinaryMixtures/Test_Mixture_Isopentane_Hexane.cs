﻿#region Copyright

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

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Science.Thermodynamics.Fluids
{

  /// <summary>
  /// Tests and test data for <see cref="Mixture_Isopentane_Hexane"/>.
  /// </summary>
  /// <remarks>
  /// <para>Reference:</para>
  /// <para>The test data was created automatically using calls into the TREND.DLL of the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// </remarks>
  [TestFixture]
  public class Test_Mixture_Isopentane_Hexane : MixtureTestBase
  {

    public Test_Mixture_Isopentane_Hexane()
    {
      _mixture = MixtureOfFluids.FromCASRegistryNumbersAndMoleFractions(new[] { ("78-78-4", 0.5), ("110-54-3", 0.5) });

      // TestData for 1 Permille to 999 Permille Molefraction contains:
      // 0. Temperature (Kelvin)
      // 1. Mole density (mol/m³)
      // 2. Pressure (Pa)
      // 3. delta*AlphaR_delta
      // 4. Isochoric heat capacity (J/mol K)
      // 5. Phasetype (1: liquid, 2: gas)
      _testData_001_999 = new(double temperature, double moleDensity, double pressure, double deltaPhiR_delta, double cv, int phase)[]
      {
      ( 200, 8610.79241171322, 100000.00000053, -0.993016225342552, 124.113269947074, 1 ),
      ( 200, 8616.85944570452, 999999.999999909, -0.930211424058757, 124.161294828103, 1 ),
      ( 200, 8675.37187636954, 9999999.99999919, -0.306821241215951, 124.636893092889, 1 ),
      ( 200, 9121.67289100534, 100000000.007238, 5.5926322749409, 128.933378597969, 1 ),
      ( 250, 0.481825597213477, 999.995106240558, -0.00153356900798593, 117.622202923943, 2 ),
      ( 250, 8100.23917089632, 100000.000001041, -0.994060833717821, 136.173000566826, 1 ),
      ( 250, 8108.68448164178, 1000000.00114026, -0.94067019357105, 136.217052405998, 1 ),
      ( 250, 8188.83489313162, 10000000.0000016, -0.412508999093241, 136.65447981527, 1 ),
      ( 250, 8750.15068152477, 100000000.011679, 4.49803881500197, 140.573799089167, 1 ),
      ( 300, 0.401207801992475, 999.999987468016, -0.000753312033189638, 134.968289545788, 2 ),
      ( 300, 4.03969663545151, 9999.99031143873, -0.00758495865016782, 135.309907139793, 2 ),
      ( 300, 7579.18083118254, 100000.000000436, -0.994710436756681, 150.812698942933, 1 ),
      ( 300, 7591.35991045406, 1000000.01790817, -0.947189228427151, 150.852129225833, 1 ),
      ( 300, 7703.96450647413, 10000000.0000302, -0.47961135207702, 151.250963723511, 1 ),
      ( 300, 8407.28014374366, 100000000.01769, 3.76855249896844, 154.915931330415, 1 ),
      ( 350, 0.343778847530117, 999.999998357324, -0.000423249163493288, 153.935415795884, 2 ),
      ( 350, 3.45099496115092, 9999.98211373086, -0.00424848506746241, 154.102095550852, 2 ),
      ( 350, 35.9537413278429, 99999.9999996236, -0.0442348119299111, 155.851451226689, 2 ),
      ( 350, 7037.48952444066, 999999.999999745, -0.951171033078682, 167.639621576232, 1 ),
      ( 350, 7203.25626558493, 10000000.0005057, -0.522947218917532, 167.964845124606, 1 ),
      ( 350, 8085.15179910222, 100000000.000001, 3.25017800480119, 171.406771124569, 1 ),
      ( 400, 0.300757585094988, 999.999999702582, -0.000260706981801292, 173.1705694822, 2 ),
      ( 400, 3.01466889568428, 9999.99687086811, -0.00261293646100293, 173.260366435147, 2 ),
      ( 400, 30.8942021774156, 99999.999589438, -0.0267456210688724, 174.183545295155, 2 ),
      ( 400, 6403.20810431794, 1000000.00000013, -0.953042417083089, 185.311999923092, 1 ),
      ( 400, 6668.92169543342, 10000000.0009789, -0.549133744176023, 185.432418972195, 1 ),
      ( 400, 7779.78646468069, 99999999.999999, 2.86487697137351, 188.60527233347, 1 ),
      ( 500, 0.240571666656198, 999.999999998965, -0.000117745628286463, 209.005847127315, 2 ),
      ( 500, 2.40827148607355, 9999.99984950447, -0.0011784726534441, 209.038074863703, 2 ),
      ( 500, 24.3437376311323, 99999.9999997023, -0.0118882148507688, 209.36433523805, 2 ),
      ( 500, 276.999481971658, 999999.983418999, -0.131610863075301, 213.116007016891, 2 ),
      ( 500, 5395.71398374019, 10000000.0015541, -0.554195531580097, 219.288806033941, 1 ),
      ( 500, 7212.81119020523, 100000000, 2.33494575209195, 221.344281753973, 1 ),
      };

      // TestData for 500 Permille to 500 Permille Molefraction contains:
      // 0. Temperature (Kelvin)
      // 1. Mole density (mol/m³)
      // 2. Pressure (Pa)
      // 3. delta*AlphaR_delta
      // 4. Isochoric heat capacity (J/mol K)
      // 5. Phasetype (1: liquid, 2: gas)
      _testData_500_500 = new(double temperature, double moleDensity, double pressure, double deltaPhiR_delta, double cv, int phase)[]
      {
      ( 200, 9147.57615112196, 1000000.00137014, -0.934260211449449, 111.062429141201, 1 ),
      ( 200, 9214.49592744399, 9999999.99999913, -0.347376430449275, 111.519561455935, 1 ),
      ( 200, 9717.69025053076, 100000000.000001, 5.18829893665595, 115.627086554111, 1 ),
      ( 250, 0.481625385258308, 999.999997198206, -0.001116235398188, 105.836294114242, 2 ),
      ( 250, 8589.10267514547, 1000000.00000052, -0.943988586976088, 123.027411225456, 1 ),
      ( 250, 8682.33863302759, 10000000.0000327, -0.445900697642743, 123.446095905638, 1 ),
      ( 250, 9319.69036378102, 100000000.000022, 4.16205752739057, 127.207753222212, 1 ),
      ( 300, 0.401132908868963, 999.999851322268, -0.000564469288808557, 123.106334678055, 2 ),
      ( 300, 4.03193948896112, 9999.99619031338, -0.00567336484952815, 123.332578921144, 2 ),
      ( 300, 7994.31194734291, 100000.000000968, -0.994985103410135, 137.210695594582, 1 ),
      ( 300, 8009.02590759456, 1000000.00000015, -0.949943165884783, 137.246679969721, 1 ),
      ( 300, 8143.53222742719, 10000000.0000405, -0.507699521008561, 137.617977505352, 1 ),
      ( 300, 8949.59628465007, 100000000.000205, 3.47960409472166, 141.133498068978, 1 ),
      ( 350, 0.343745979578099, 999.99997886527, -0.000325392799072297, 141.062314589478, 2 ),
      ( 350, 3.44759111284429, 9999.99999998996, -0.00326310207885659, 141.172586412718, 2 ),
      ( 350, 35.5588246072295, 100000.010714209, -0.0336178724143811, 142.320039030286, 2 ),
      ( 350, 7369.70870834539, 1000000.00138489, -0.953372088208272, 153.023161447019, 1 ),
      ( 350, 7576.68434465164, 10000000.001474, -0.546458435477595, 153.286702469094, 1 ),
      ( 350, 8599.986180055, 100000000.000931, 2.99575208521314, 156.563586038369, 1 ),
      ( 400, 0.300741454262332, 999.999995899595, -0.000204803962332552, 158.886615145376, 2 ),
      ( 400, 3.01297966279532, 9999.9999999998, -0.00205147437943787, 158.945957760953, 2 ),
      ( 400, 30.7089331271166, 100000.000780598, -0.0208716795046024, 159.55267267214, 2 ),
      ( 400, 6595.39235535533, 1000000.00000009, -0.954410618046628, 169.558691005277, 1 ),
      ( 400, 6956.79242793346, 10000000.0000002, -0.567789517461108, 169.46027035954, 1 ),
      ( 400, 8267.26941176968, 99999999.9891736, 2.63699120248111, 172.415202311101, 1 ),
      ( 500, 0.240566856617963, 999.999999724453, -9.54730081791709E-05, 191.9881854882, 2 ),
      ( 500, 2.40773902185004, 9999.99716202841, -0.000955307618751644, 192.009468186706, 2 ),
      ( 500, 24.2878351523781, 99999.9999287358, -0.00961165326927569, 192.224441612949, 2 ),
      ( 500, 268.137338198223, 999999.999999999, -0.102907895659558, 194.627004004881, 2 ),
      ( 500, 5394.41588618151, 10000000.0001834, -0.554087237514984, 201.161682991207, 1 ),
      ( 500, 7647.41834534288, 99999999.9985426, 2.14542605243977, 202.648220730192, 1 ),
      };

      // TestData for 999 Permille to 1 Permille Molefraction contains:
      // 0. Temperature (Kelvin)
      // 1. Mole density (mol/m³)
      // 2. Pressure (Pa)
      // 3. delta*AlphaR_delta
      // 4. Isochoric heat capacity (J/mol K)
      // 5. Phasetype (1: liquid, 2: gas)
      _testData_999_001 = new(double temperature, double moleDensity, double pressure, double deltaPhiR_delta, double cv, int phase)[]
      {
      ( 200, 9810.47803409885, 1000000.00240986, -0.938702161872807, 97.2493907202079, 1 ),
      ( 200, 9887.22454373052, 10000000.0000011, -0.391779673650911, 97.6924452149382, 1 ),
      ( 200, 10455.8774402606, 100000000.000009, 4.75141682120054, 101.665952363161, 1 ),
      ( 250, 0.481476174719519, 999.999998959174, -0.000804400335403717, 94.0586650950316, 2 ),
      ( 250, 4.85019816806477, 9999.99990281657, -0.00810470305743874, 94.3969035980834, 2 ),
      ( 250, 9182.26859573978, 1000000.00135956, -0.947606752036255, 109.514674925225, 1 ),
      ( 250, 9291.74078461861, 10000000, -0.482240318284583, 109.915450774873, 1 ),
      ( 250, 10020.1015360537, 100000000.000033, 3.80123752630412, 113.549991835953, 1 ),
      ( 300, 0.401076227912206, 999.999842301566, -0.000420947287045104, 111.24797978392, 2 ),
      ( 300, 4.02608413255335, 9999.99849288265, -0.00422499133207313, 111.391345967712, 2 ),
      ( 300, 8514.56088451019, 999999.999999931, -0.952915082601048, 123.513368681775, 1 ),
      ( 300, 8678.31629921695, 9999999.99999958, -0.538035510446095, 123.85089709495, 1 ),
      ( 300, 9612.18271594249, 99999999.9999996, 3.17082579246639, 127.229981225316, 1 ),
      ( 350, 0.343720724537634, 999.999975719084, -0.000249661214882722, 128.191016445806, 2 ),
      ( 350, 3.4449676042439, 9999.99999998958, -0.0025017640814948, 128.260739535091, 2 ),
      ( 350, 35.2649703706096, 100000.001662293, -0.025563024619254, 128.980031758104, 2 ),
      ( 350, 7750.13978986528, 1000000.00492086, -0.955660811056014, 138.418694635185, 1 ),
      ( 350, 8018.450392486, 10000000.0053907, -0.571444737874418, 138.582595904987, 1 ),
      ( 350, 9224.94415958797, 100000000.001025, 2.72506223192854, 141.687178436849, 1 ),
      ( 400, 0.300728884505682, 999.999994980721, -0.000160734655977352, 144.603670988127, 2 ),
      ( 400, 3.01165213516886, 9999.99999999977, -0.00160930460503202, 144.641116035197, 2 ),
      ( 400, 30.5661196932172, 100000.000156438, -0.016294675406683, 145.022163408601, 2 ),
      ( 400, 372.395563990148, 999999.998551481, -0.192577527344951, 150.332124756038, 2 ),
      ( 400, 7273.97406449015, 9999999.99999991, -0.586635112683226, 153.546250120595, 1 ),
      ( 400, 8855.14812269849, 100000000.000223, 2.39554508615738, 156.239873870746, 1 ),
      ( 500, 0.24056302846104, 999.999999632773, -7.72807851475804E-05, 174.970942118244, 2 ),
      ( 500, 2.40730549645857, 9999.99622564645, -0.000773113402167624, 174.984353510573, 2 ),
      ( 500, 24.2426154825301, 99999.9999028046, -0.00776202237977626, 175.119569806005, 2 ),
      ( 500, 261.756226789022, 1000000.00000338, -0.0810364264796588, 176.595892743072, 2 ),
      ( 500, 5269.47630287117, 10000000.0010154, -0.543513579430104, 183.179568497399, 1 ),
      ( 500, 8164.52994725285, 100000000, 1.94621293712004, 184.00951353445, 1 ),
      };
    }

    [Test]
    public override void CASNumberAttribute_Test()
    {
      base.CASNumberAttribute_Test();
    }

    [Test]
    public override void Constants_Test()
    {
      base.Constants_Test();
    }

    [Test]
    public override void DeltaPhiRDelta_001_999_Test()
    {
      base.DeltaPhiRDelta_001_999_Test();
    }

    [Test]
    public override void DeltaPhiRDelta_500_500_Test()
    {
      base.DeltaPhiRDelta_500_500_Test();
    }

    [Test]
    public override void DeltaPhiRDelta_999_001_Test()
    {
      base.DeltaPhiRDelta_999_001_Test();
    }
  }
}