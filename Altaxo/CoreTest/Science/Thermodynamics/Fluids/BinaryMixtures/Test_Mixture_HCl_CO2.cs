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
  /// Tests and test data for <see cref="Mixture_HCl_CO2"/>.
  /// </summary>
  /// <remarks>
  /// <para>Reference:</para>
  /// <para>The test data was created automatically using calls into the TREND.DLL of the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// </remarks>
  
  public class Test_Mixture_HCl_CO2 : MixtureTestBase
  {

    public Test_Mixture_HCl_CO2()
    {
      _mixture = MixtureOfFluids.FromCASRegistryNumbersAndMoleFractions(new[] { ("7647-01-0", 0.5), ("124-38-9", 0.5) });

      // TestData for 1 Permille to 999 Permille Molefraction contains:
      // 0. Temperature (Kelvin)
      // 1. Mole density (mol/m³)
      // 2. Pressure (Pa)
      // 3. delta*AlphaR_delta
      // 4. Isochoric heat capacity (J/mol K)
      // 5. Phasetype (1: liquid, 2: gas)
      _testData_001_999 = new (double temperature, double moleDensity, double pressure, double deltaPhiR_delta, double cv, int phase)[]
      {
      ( 250, 0.481129285273251, 999.999999867072, -8.85498210241059E-05, 26.5204120357793, 2 ),
      ( 250, 4.81513345443585, 9999.99863385329, -0.000886089717094947, 26.542016651801, 2 ),
      ( 250, 48.5417146691828, 99999.9999853253, -0.00892112132561805, 26.760527615942, 2 ),
      ( 300, 0.400925064858955, 999.999999995168, -4.86301798253373E-05, 28.9039549551269, 2 ),
      ( 300, 4.01100681847742, 9999.99985637189, -0.000486446552711423, 28.9133951350949, 2 ),
      ( 300, 40.2871196265056, 99999.9999999242, -0.00487904936580486, 29.0082999987909, 2 ),
      ( 300, 422.169016916423, 999999.999999997, -0.0503671474301441, 30.0145138082474, 2 ),
      ( 300, 18219.9491382221, 9999999.99977527, -0.779963399045403, 41.7842678487587, 1 ),
      ( 350, 0.343643313093058, 999.999999997489, -2.90104980623556E-05, 31.069858733949, 2 ),
      ( 350, 3.43733074303782, 9999.99997468475, -0.000290139301669478, 31.0747368792683, 2 ),
      ( 350, 34.4634450749366, 99999.999999999, -0.00290483704141067, 31.1236526899387, 2 ),
      ( 350, 354.043919749262, 1000000.0000207, -0.0294047583911744, 31.6265619934656, 2 ),
      ( 350, 5199.80299364932, 9999999.99981911, -0.339141609301937, 38.3183165866634, 2 ),
      ( 400, 0.300684626550277, 999.999999999519, -1.81276296840427E-05, 33.0075334388034, 2 ),
      ( 400, 3.00733692556339, 9999.99999518081, -0.000181279008459177, 33.0103582473707, 2 ),
      ( 400, 30.122531583245, 100000, -0.00181306136194658, 33.0386474000731, 2 ),
      ( 400, 306.239765670607, 999999.997083754, -0.0181576347260855, 33.3255702840229, 2 ),
      ( 400, 3670.54741727985, 9999999.99678147, -0.180832879435024, 36.4586106522427, 2 ),
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
      ( 250, 0.481134493143626, 999.99999995903, -9.64985412535123E-05, 23.670654641417, 2 ),
      ( 250, 4.81553115669881, 9999.99958079459, -0.00096573206207189, 23.7004301578709, 2 ),
      ( 250, 48.5816690707874, 99999.9999986729, -0.00973335486201827, 24.0019227377262, 2 ),
      ( 250, 538.547401322586, 1000000, -0.10669318395109, 27.5485372672867, 2 ),
      ( 300, 0.400928017344972, 999.999999998516, -5.31193588619391E-05, 24.8637813499378, 2 ),
      ( 300, 4.01119866614643, 9999.99995596788, -0.000531378088426066, 24.8758313139767, 2 ),
      ( 300, 40.3055963586767, 99999.9999999924, -0.00533236935517514, 24.9971333246629, 2 ),
      ( 300, 424.390663059646, 1000000.00703534, -0.0553356729316369, 26.2995023809126, 2 ),
      ( 300, 20809.1749939459, 9999999.99927167, -0.807341367258899, 37.5284730773446, 1 ),
      ( 350, 0.343645446616198, 999.999999999187, -3.23441665595292E-05, 25.95204673944, 2 ),
      ( 350, 3.43745529882647, 9999.99999180187, -0.000323489855871166, 25.9577265151367, 2 ),
      ( 350, 34.4751232608119, 99999.9999999998, -0.00323973001251439, 26.0147452416173, 2 ),
      ( 350, 355.321953154134, 1000000.00000077, -0.0328930463197091, 26.6077122246944, 2 ),
      ( 350, 5696.61239249402, 9999999.99982873, -0.396774244025599, 35.3566738699172, 2 ),
      ( 400, 0.300686322983575, 999.999999999831, -2.0894712491936E-05, 26.9340027737225, 2 ),
      ( 400, 3.00742882147671, 9999.99999828252, -0.00020895562982293, 26.9370582285289, 2 ),
      ( 400, 30.130989971006, 99999.9824773726, -0.00209040383470081, 26.9676845228013, 2 ),
      ( 400, 307.125372210492, 999999.99951342, -0.0209859964731919, 27.2811092512601, 2 ),
      ( 400, 3819.97445347933, 9999999.99899325, -0.212874211858914, 30.9844486723481, 2 ),
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
      ( 250, 0.48113987583587, 999.999999999966, -0.000104810384292823, 20.8210536230317, 2 ),
      ( 250, 4.81594655888468, 9999.99999965048, -0.00104903262853207, 20.860262337841, 2 ),
      ( 250, 48.6236222970215, 99999.9962008863, -0.0105849277157163, 21.2575539793907, 2 ),
      ( 250, 545.203187764244, 999999.999260175, -0.117596048168062, 25.8799177686247, 2 ),
      ( 250, 28470.8572226506, 10000000.0005747, -0.83102389800024, 34.5150067430119, 1 ),
      ( 300, 0.400930888658472, 999.990280450245, -5.740544880804E-05, 20.8237050773093, 2 ),
      ( 300, 4.01138242458568, 9999.99999991984, -0.00057428990062286, 20.8390325471149, 2 ),
      ( 300, 40.3232940525987, 99999.9990802912, -0.00576606580959859, 20.9934713452354, 2 ),
      ( 300, 426.580659389001, 999999.999992457, -0.0601827251343629, 22.6654170519935, 2 ),
      ( 300, 23272.6913269857, 9999999.99989081, -0.827734632329219, 33.5385682093738, 1 ),
      ( 350, 0.343647478607661, 999.989214117277, -3.5381953044371E-05, 20.8342950403444, 2 ),
      ( 350, 3.43756970849965, 9999.99999997932, -0.00035388749237546, 20.8410545281261, 2 ),
      ( 350, 34.4857946748613, 99999.9997884792, -0.00354530668903218, 20.9089734907766, 2 ),
      ( 350, 356.510287824468, 999999.99999987, -0.0361138758308274, 21.6217855622795, 2 ),
      ( 350, 6404.28184166007, 9999999.99998451, -0.463428799583926, 32.9956052750838, 2 ),
      ( 400, 0.300687936150781, 999.994281580831, -2.33847017469677E-05, 20.8605185920693, 2 ),
      ( 400, 3.00751239469411, 9999.99999999616, -0.000233863895853096, 20.8639088358771, 2 ),
      ( 400, 30.1386205452616, 99999.9999613381, -0.00234018951745415, 20.8979181951741, 2 ),
      ( 400, 307.933689358697, 999999.999999998, -0.023553073263716, 21.24879869731, 2 ),
      ( 400, 3979.55691440331, 9999999.98892871, -0.244436224459878, 25.7230071575094, 2 ),
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
