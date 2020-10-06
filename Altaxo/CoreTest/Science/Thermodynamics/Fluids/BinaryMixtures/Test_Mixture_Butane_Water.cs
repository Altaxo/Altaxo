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
  /// Tests and test data for <see cref="Mixture_Butane_Water"/>.
  /// </summary>
  /// <remarks>
  /// <para>Reference:</para>
  /// <para>The test data was created automatically using calls into the TREND.DLL of the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// </remarks>
  
  public class Test_Mixture_Butane_Water : MixtureTestBase
  {

    public Test_Mixture_Butane_Water()
    {
      _mixture = MixtureOfFluids.FromCASRegistryNumbersAndMoleFractions(new[] { ("106-97-8", 0.5), ("7732-18-5", 0.5) });

      // TestData for 1 Permille to 999 Permille Molefraction contains:
      // 0. Temperature (Kelvin)
      // 1. Mole density (mol/m³)
      // 2. Pressure (Pa)
      // 3. delta*AlphaR_delta
      // 4. Isochoric heat capacity (J/mol K)
      // 5. Phasetype (1: liquid, 2: gas)
      _testData_001_999 = new (double temperature, double moleDensity, double pressure, double deltaPhiR_delta, double cv, int phase)[]
      {
      ( 300, 0.401105736128645, 999.999999768457, -0.000482392445117418, 25.4701539950465, 2 ),
      ( 350, 0.343707986235346, 999.999999955625, -0.000200514538079165, 25.6739769814248, 2 ),
      ( 350, 3.44334008445394, 9999.99951554342, -0.00201821663443233, 25.9654912098192, 2 ),
      ( 350, 56033.8816792146, 100000000.001652, -0.38672985434042, 67.1791892918416, 1 ),
      ( 400, 0.300715723697631, 999.999999993969, -0.000104881059761905, 26.0495179718941, 2 ),
      ( 400, 3.01000707929561, 9999.99993733021, -0.00105157099408612, 26.1515721582174, 2 ),
      ( 400, 30.3966411853714, 99999.9999986645, -0.0107979938710952, 27.2562832481895, 2 ),
      ( 400, 51834.4985950512, 1000000.0000011, -0.994199149360697, 65.4588141294584, 1 ),
      ( 400, 52086.9470362425, 10000000.0000041, -0.942272641914553, 65.2700152641089, 1 ),
      ( 400, 54273.7760817629, 100000000.003543, -0.445986245970655, 63.6246971776961, 1 ),
      ( 500, 0.240557166209302, 999.999999998925, -4.08167355108209E-05, 27.0278517036468, 2 ),
      ( 500, 2.40645632639631, 9999.9999891, -0.000408422905369926, 27.0505639413652, 2 ),
      ( 500, 24.1540066568386, 99999.9999999996, -0.004109956281595, 27.2831714822498, 2 ),
      ( 500, 251.584698686881, 999999.999999978, -0.043871313690441, 30.1901904562471, 2 ),
      ( 500, 46297.9144033554, 10000000.0000005, -0.948043588885516, 58.0889433419612, 1 ),
      ( 500, 49700.1346868628, 99999999.9999984, -0.516002624607105, 57.387828257163, 1 ),
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
      ( 300, 0.400998237175268, 999.999999999913, -0.000220482275251822, 57.9702932789903, 2 ),
      ( 350, 0.343679584983521, 999.999999736595, -0.000123931819977696, 64.568667070309, 2 ),
      ( 350, 3.44064024390081, 9999.99999999578, -0.0012411418010106, 64.6163013110451, 2 ),
      ( 400, 0.30070532748676, 999.999999952415, -7.63516838174027E-05, 71.2038472070596, 2 ),
      ( 400, 3.00912287782737, 9999.99951234012, -0.000764075287554385, 71.2278727503531, 2 ),
      ( 400, 30.3014723352702, 99999.9987968085, -0.00769716778541519, 71.4743564364024, 2 ),
      ( 500, 0.240554007550397, 999.999999999846, -3.37265113230658E-05, 83.6256733712985, 2 ),
      ( 500, 2.40627069347491, 9999.9999780193, -0.000337347101442402, 83.6343699891523, 2 ),
      ( 500, 24.1362104463697, 99999.9999999985, -0.00338168230958853, 83.7218532429882, 2 ),
      ( 500, 249.18006794647, 1000000.00024988, -0.0346503374724314, 84.648140284697, 2 ),
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
      ( 300, 0.401020166757853, 999.999998575816, -0.000281193182907124, 90.5769216151865, 2 ),
      ( 300, 4.02040886743029, 9999.98470245139, -0.00281932116598642, 90.6507012134497, 2 ),
      ( 300, 41.287059245223, 100000.006932449, -0.0289756425037376, 91.4338955995848, 2 ),
      ( 300, 9845.91348323048, 999999.999999916, -0.959281847915563, 100.497830692681, 1 ),
      ( 300, 10087.0586643506, 10000000.0009655, -0.602552720276377, 100.910902686817, 1 ),
      ( 300, 11353.6801718607, 99999999.9999988, 2.53107887972929, 104.456275771942, 1 ),
      ( 350, 0.343692861384625, 999.999999770338, -0.000168594868229456, 103.489196778003, 2 ),
      ( 350, 3.4421606495833, 9999.99761254629, -0.00168832412620916, 103.526724953263, 2 ),
      ( 350, 34.9623318561078, 99999.9998872316, -0.0171281539427258, 103.913409539545, 2 ),
      ( 350, 8697.49838584136, 1000000.00006403, -0.960490373057936, 111.668996018467, 1 ),
      ( 350, 9152.22748753374, 10000000.0000003, -0.624534118060754, 111.758183809013, 1 ),
      ( 350, 10849.4224739914, 100000000.000032, 2.16731067812151, 115.208437490773, 1 ),
      ( 400, 0.300713071384987, 999.999999997308, -0.000108141024247315, 116.366997527981, 2 ),
      ( 400, 3.0100632981298, 9999.99960739893, -0.00108229562767624, 116.388197364151, 2 ),
      ( 400, 30.3998089704774, 99999.9999980649, -0.0109130213962769, 116.603624977645, 2 ),
      ( 400, 341.718896638109, 999999.998273123, -0.120093869542569, 119.184801917924, 2 ),
      ( 400, 8036.07650109658, 10000000.0000082, -0.625836623226976, 123.49992120775, 1 ),
      ( 400, 10370.366754016, 100000000.000302, 1.89942061933193, 126.468686617912, 1 ),
      ( 500, 0.240556470952605, 999.999999996889, -5.00064712597129E-05, 140.225700242764, 2 ),
      ( 500, 2.40664825368471, 9999.9999685424, -0.000500213504603616, 140.23412285279, 2 ),
      ( 500, 24.1757354962316, 99999.9999999972, -0.00501706921796878, 140.318716568491, 2 ),
      ( 500, 253.663118452458, 1000000.00036418, -0.0517169266272497, 141.203031684074, 2 ),
      ( 500, 4602.64438685084, 10000000, -0.477377739067657, 147.728776327092, 1 ),
      ( 500, 9483.66171081524, 100000000.008512, 1.53640892014262, 148.097870826822, 1 ),
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
