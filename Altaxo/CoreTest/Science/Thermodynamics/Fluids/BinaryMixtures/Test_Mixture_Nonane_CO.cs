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
  /// Tests and test data for <see cref="Mixture_Nonane_CO"/>.
  /// </summary>
  /// <remarks>
  /// <para>Reference:</para>
  /// <para>The test data was created automatically using calls into the TREND.DLL of the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// </remarks>
  
  public class Test_Mixture_Nonane_CO : MixtureTestBase
  {

    public Test_Mixture_Nonane_CO()
    {
      _mixture = MixtureOfFluids.FromCASRegistryNumbersAndMoleFractions(new[] { ("111-84-2", 0.5), ("630-08-0", 0.5) });

      // TestData for 1 Permille to 999 Permille Molefraction contains:
      // 0. Temperature (Kelvin)
      // 1. Mole density (mol/m³)
      // 2. Pressure (Pa)
      // 3. delta*AlphaR_delta
      // 4. Isochoric heat capacity (J/mol K)
      // 5. Phasetype (1: liquid, 2: gas)
      _testData_001_999 = new (double temperature, double moleDensity, double pressure, double deltaPhiR_delta, double cv, int phase)[]
      {
      ( 250, 0.481094037000763, 999.999999999887, -1.07248874279724E-05, 20.9543686638975, 2 ),
      ( 250, 4.81140459775833, 9999.99999499854, -0.000107208723325783, 20.9553182846998, 2 ),
      ( 300, 0.400908800595256, 999.999994739478, -3.49911859026226E-06, 21.008336677606, 2 ),
      ( 300, 4.00921414494189, 9999.9994966439, -3.496127960724E-05, 21.0089577301785, 2 ),
      ( 300, 40.1046410968235, 100000, -0.000346626217360257, 21.0151727481762, 2 ),
      ( 300, 20148.5417966745, 100000000.012368, 0.989758871110983, 24.5594757425578, 1 ),
      ( 350, 0.343634857215901, 1000.00001755437, 1.57352885997099E-07, 21.1057805837457, 1 ),
      ( 350, 3.43634364594372, 10000.0018092106, 1.59407263878483E-06, 21.1062402012191, 1 ),
      ( 350, 34.3628730132661, 100000, 1.79910444245528E-05, 21.1108379058531, 1 ),
      ( 350, 343.504032554653, 1000000.00000475, 0.000381013919245778, 21.1569457417367, 1 ),
      ( 350, 3365.92848300204, 10000000.0092165, 0.0209216093552192, 21.6126405839796, 1 ),
      ( 350, 18397.2839830414, 100000000.007839, 0.867856759207395, 24.0759755301176, 1 ),
      ( 400, 0.300679900554303, 1000.00006769465, 2.11210772215119E-06, 21.2630397679335, 1 ),
      ( 400, 3.00674193556185, 10000, 2.11350684912453E-05, 21.2634049129835, 1 ),
      ( 400, 30.0616593154504, 100000.000000014, 0.000212746644099164, 21.2670567498533, 1 ),
      ( 400, 300.001367903076, 1000000.00026529, 0.00226392441714023, 21.3036024067786, 1 ),
      ( 400, 2908.25973947752, 10000000.0054023, 0.0338847808238779, 21.6627373481136, 1 ),
      ( 400, 16932.9347386229, 100000000.004885, 0.775714328171033, 23.8082716021169, 1 ),
      ( 500, 0.240543515621325, 1000.0007853749, 3.7620654659829E-06, 21.7681895764501, 1 ),
      ( 500, 2.40535387962526, 9999.99999999999, 3.76273039790435E-05, 21.7684483020155, 1 ),
      ( 500, 24.0453802267985, 100000.000000141, 0.000376938904039792, 21.7710352507519, 1 ),
      ( 500, 239.62564607063, 1000000.00165824, 0.00383428319821459, 21.7968704336249, 1 ),
      ( 500, 2305.08467601371, 10000000.0000038, 0.0435384051777042, 22.0490370515406, 1 ),
      ( 500, 14633.1099280975, 100000000.001923, 0.643836749978179, 23.7360264736674, 1 ),
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
      ( 300, 0.401009872638, 999.997206303393, -0.000255541299180636, 111.992993098528, 2 ),
      ( 350, 0.343687273592475, 999.999998749045, -0.000152351349679145, 126.405523717204, 2 ),
      ( 350, 3.44159793552417, 9999.99999667454, -0.00152510894479624, 126.439577346354, 2 ),
      ( 400, 0.300709708889296, 999.999999746673, -9.69724706836867E-05, 140.742093039824, 2 ),
      ( 400, 3.00972549764561, 9999.99999998504, -0.000970192925816877, 140.76080707321, 2 ),
      ( 400, 30.3640848720664, 99999.9999932168, -0.00974934811738256, 140.949883986209, 2 ),
      ( 500, 0.240554922235334, 999.999999984444, -4.35807945620517E-05, 167.319850214203, 2 ),
      ( 500, 2.40649300367633, 9999.99984403482, -0.000435744899928098, 167.326983508811, 2 ),
      ( 500, 24.1595654695477, 99999.9999999482, -0.00435113798788184, 167.398683026268, 2 ),
      ( 500, 251.31697554873, 1000000.00581606, -0.0428643426337908, 168.149348314502, 2 ),
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
      ( 350, 0.344025697565648, 999.999876304469, -0.00113591847210298, 231.760126307216, 2 ),
      ( 350, 5332.27596327752, 10000000.0000015, -0.355556774012824, 254.318437155949, 1 ),
      ( 350, 5813.63640725678, 100000000.000006, 4.91084285806872, 259.557562425482, 1 ),
      ( 400, 0.300883617279251, 999.999265505005, -0.000674908145406431, 260.250608181155, 2 ),
      ( 400, 3.02736137903561, 9999.99367119383, -0.00679003260984803, 260.554896910276, 2 ),
      ( 400, 5036.749014055, 10000000.0001604, -0.403026540557212, 279.541125070471, 1 ),
      ( 400, 5621.27961418615, 100000000.000006, 4.34896978913951, 284.488555404151, 1 ),
      ( 500, 0.240614378372414, 999.999962294062, -0.000290671362039144, 312.881920561135, 2 ),
      ( 500, 2.41247366506407, 9999.99999996935, -0.00291372235349621, 312.989929025653, 2 ),
      ( 500, 24.7952351651156, 100000.002998769, -0.0298763579764387, 314.104041352376, 2 ),
      ( 500, 4146.35578411283, 1000000.00328528, -0.94198654144521, 327.797751431625, 1 ),
      ( 500, 4400.30733732088, 10000000.0000012, -0.453346277383679, 327.71792080779, 1 ),
      ( 500, 5265.90694445156, 100000000.002061, 3.56795840113776, 331.923441937083, 1 ),
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
