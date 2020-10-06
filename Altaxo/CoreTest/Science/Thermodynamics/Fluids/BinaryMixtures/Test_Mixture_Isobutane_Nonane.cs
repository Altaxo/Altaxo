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
  /// Tests and test data for <see cref="Mixture_Isobutane_Nonane"/>.
  /// </summary>
  /// <remarks>
  /// <para>Reference:</para>
  /// <para>The test data was created automatically using calls into the TREND.DLL of the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// </remarks>
  
  public class Test_Mixture_Isobutane_Nonane : MixtureTestBase
  {

    public Test_Mixture_Isobutane_Nonane()
    {
      _mixture = MixtureOfFluids.FromCASRegistryNumbersAndMoleFractions(new[] { ("75-28-5", 0.5), ("111-84-2", 0.5) });

      // TestData for 1 Permille to 999 Permille Molefraction contains:
      // 0. Temperature (Kelvin)
      // 1. Mole density (mol/m³)
      // 2. Pressure (Pa)
      // 3. delta*AlphaR_delta
      // 4. Isochoric heat capacity (J/mol K)
      // 5. Phasetype (1: liquid, 2: gas)
      _testData_001_999 = new (double temperature, double moleDensity, double pressure, double deltaPhiR_delta, double cv, int phase)[]
      {
      ( 250, 5870.25656766175, 1000000.00213508, -0.918046362694814, 207.658581167351, 1 ),
      ( 250, 5913.40939037451, 10000000.0000004, -0.186444155096989, 208.35227884149, 1 ),
      ( 350, 0.344026407039912, 999.999997044386, -0.00113797853371372, 231.841935796568, 2 ),
      ( 350, 5254.82273719672, 1000000.00013975, -0.934605803244425, 253.802735064285, 1 ),
      ( 350, 5331.64117223099, 10000000.0000101, -0.355480045894946, 254.413193173858, 1 ),
      ( 400, 0.300883972687969, 999.999261236978, -0.000676088559626468, 260.345780126289, 2 ),
      ( 400, 3.02739781978219, 9999.99361141025, -0.00680198781577551, 260.650731085757, 2 ),
      ( 400, 4917.54224869902, 100000.000004839, -0.993885552311831, 279.055247423953, 1 ),
      ( 400, 4929.55491060994, 1000000.00000142, -0.93900452405771, 279.107264562416, 1 ),
      ( 400, 5036.36440116526, 10000000.0001563, -0.402980951384642, 279.647889651452, 1 ),
      ( 500, 0.240614494372451, 999.999962087963, -0.000291153320951999, 313.001405205847, 2 ),
      ( 500, 2.41248538813469, 9999.99999996891, -0.00291856752804151, 313.109649501699, 2 ),
      ( 500, 24.7965446843333, 100000.003028057, -0.029927590752389, 314.226266547506, 2 ),
      ( 500, 4147.42069014852, 1000000.00000016, -0.942001437369488, 327.921864450465, 1 ),
      ( 500, 4400.56074042996, 10000000.0000011, -0.453377756043141, 327.845668582955, 1 ),
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
      ( 300, 0.401218480308675, 999.99146455559, -0.00077533787361674, 146.020514643249, 2 ),
      ( 300, 6922.60446002082, 1000000.01919521, -0.942087200333276, 163.119643259037, 1 ),
      ( 300, 7033.37117560568, 10000000.0000444, -0.429992548726352, 163.673213283841, 1 ),
      ( 350, 0.343787405429674, 999.999956018363, -0.00044356787985757, 167.256639203009, 2 ),
      ( 350, 3.45172213522432, 9999.99987715996, -0.00445372212657615, 167.412222110449, 2 ),
      ( 350, 6407.1271455539, 1000000.00022042, -0.946366771824239, 181.420684079748, 1 ),
      ( 350, 6572.30868971348, 10000000.0002583, -0.477147333449356, 181.906197394657, 1 ),
      ( 400, 0.300763832228337, 999.999991663208, -0.000276907968505319, 188.299126717068, 2 ),
      ( 400, 3.0151743297192, 9999.99999808741, -0.00277557631195259, 188.384117214487, 2 ),
      ( 400, 30.9482439057368, 99999.9982149638, -0.0284406785625288, 189.267554742851, 2 ),
      ( 400, 6079.81639734001, 10000000.0000026, -0.505444689979578, 200.725043352245, 1 ),
      ( 500, 0.240575052220917, 999.999999460608, -0.00012725159957501, 227.051837382711, 2 ),
      ( 500, 2.40851182303163, 9999.99440111626, -0.00127358092055234, 227.083245553976, 2 ),
      ( 500, 24.3674297773365, 99999.9991775291, -0.0128444362360468, 227.402156638094, 2 ),
      ( 500, 280.347903752567, 999999.99999879, -0.141978821881735, 231.201497521184, 2 ),
      ( 500, 4910.56724983142, 10000000, -0.51014938514701, 237.135778723962, 1 ),
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
      ( 250, 0.481306120670423, 999.999999997221, -0.000451362115937361, 75.7423797550197, 2 ),
      ( 250, 4.83281103636435, 9999.99996379406, -0.00453613079708548, 75.8639953579505, 2 ),
      ( 250, 10422.8169430327, 99999.99999924, -0.995384272052155, 87.7008162800686, 1 ),
      ( 250, 10441.2450452322, 1000000.00000111, -0.953924184805524, 87.7585272954933, 1 ),
      ( 250, 10610.6141662102, 10000000.0000502, -0.546596577931827, 88.310804058224, 1 ),
      ( 300, 0.401007598732012, 999.999999999555, -0.000249872981250899, 88.9474273397656, 2 ),
      ( 300, 4.01913967365699, 9999.99999539251, -0.00250444044715417, 89.0026417429094, 2 ),
      ( 300, 41.1459326665365, 99999.9999999994, -0.0256451323708474, 89.5911396148741, 2 ),
      ( 300, 9450.27255214697, 999999.999999941, -0.957577160282359, 98.3981148788527, 1 ),
      ( 300, 9736.39681293353, 10000000.0022567, -0.588238436090424, 98.8385725384924, 1 ),
      ( 350, 0.343687395645927, 999.999999999956, -0.000152706425028646, 102.702665914857, 2 ),
      ( 350, 3.44161091106866, 9999.99999953133, -0.00152887339033959, 102.731378587888, 2 ),
      ( 350, 34.9036143361922, 99999.9926570915, -0.0154747028071326, 103.027375131664, 2 ),
      ( 350, 419.700529338837, 999999.999978025, -0.18123783899849, 107.227213611785, 2 ),
      ( 350, 8751.89174850563, 10000000.0000001, -0.607359274718047, 110.833355608103, 1 ),
      ( 400, 0.300710473672795, 999.997903157018, -9.95152706775696E-05, 116.269155527132, 2 ),
      ( 400, 3.00980263556612, 9999.99999996771, -0.000995796957417159, 116.285836358284, 2 ),
      ( 400, 30.3724841460628, 99999.9994610939, -0.0100231941544368, 116.455341806996, 2 ),
      ( 400, 337.085823121937, 999999.999434734, -0.108000017432904, 118.479369109332, 2 ),
      ( 400, 7552.22716142765, 10000000.0000015, -0.601865063247037, 123.636163398195, 1 ),
      ( 500, 0.240555845885049, 999.997939367642, -4.74201772720097E-05, 141.108062476754, 2 ),
      ( 500, 2.40658576383725, 9999.99999999899, -0.000474272410098303, 141.115000135132, 2 ),
      ( 500, 24.1692407057356, 99999.9999877712, -0.00474970813241624, 141.184667323997, 2 ),
      ( 500, 252.728618199784, 999999.99999999, -0.0482105256851646, 141.911351457081, 2 ),
      ( 500, 4105.01182095062, 9999999.99999998, -0.414022543289575, 148.046354872326, 1 ),
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
