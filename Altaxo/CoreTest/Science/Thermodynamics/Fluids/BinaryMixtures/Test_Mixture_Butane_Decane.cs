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
  /// Tests and test data for <see cref="Mixture_Butane_Decane"/>.
  /// </summary>
  /// <remarks>
  /// <para>Reference:</para>
  /// <para>The test data was created automatically using calls into the TREND.DLL of the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// </remarks>
  
  public class Test_Mixture_Butane_Decane : MixtureTestBase
  {

    public Test_Mixture_Butane_Decane()
    {
      _mixture = MixtureOfFluids.FromCASRegistryNumbersAndMoleFractions(new[] { ("106-97-8", 0.5), ("124-18-5", 0.5) });

      // TestData for 1 Permille to 999 Permille Molefraction contains:
      // 0. Temperature (Kelvin)
      // 1. Mole density (mol/m³)
      // 2. Pressure (Pa)
      // 3. delta*AlphaR_delta
      // 4. Isochoric heat capacity (J/mol K)
      // 5. Phasetype (1: liquid, 2: gas)
      _testData_001_999 = new (double temperature, double moleDensity, double pressure, double deltaPhiR_delta, double cv, int phase)[]
      {
      ( 350, 0.344176968958562, 999.999992129517, -0.00157493565954963, 257.834078937859, 2 ),
      ( 400, 0.300956520612446, 999.999998458619, -0.000916983907022072, 289.546545805033, 2 ),
      ( 400, 3.0348663999659, 9999.98363152588, -0.00924616314370341, 290.010520455115, 2 ),
      ( 400, 4538.18804410446, 100000.000000736, -0.993374436109611, 310.68834093578, 1 ),
      ( 400, 4548.21490010857, 1000000.00000014, -0.933890426259601, 310.73130867872, 1 ),
      ( 400, 4637.99315822854, 10000000.0000276, -0.351701181823477, 311.19986477817, 1 ),
      ( 400, 5139.50086693872, 100000000.000001, 4.85038423205791, 316.100646106666, 1 ),
      ( 500, 0.240636286668255, 999.999894539915, -0.00038168806187815, 347.928950584635, 2 ),
      ( 500, 2.41469043645796, 9999.99999961038, -0.00382908288518902, 348.093339138473, 2 ),
      ( 500, 25.046412213561, 100000.012033473, -0.039605212240352, 349.807731258078, 2 ),
      ( 500, 3895.53768574023, 1000000.00000009, -0.938251286968365, 365.578913620613, 1 ),
      ( 500, 4093.05490590896, 10000000.0012506, -0.412310745354201, 365.383849598998, 1 ),
      ( 500, 4823.90555197461, 100000000.001002, 3.9865080497772, 369.345999332087, 1 ),
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
      ( 300, 6780.41659908718, 1000000.0055615, -0.940872748238281, 177.507370741809, 1 ),
      ( 300, 6868.75905434297, 10000000.0000015, -0.416332128439793, 177.978225543222, 1 ),
      ( 300, 7441.37911367297, 100000000.000079, 4.38754163232275, 182.170803492118, 1 ),
      ( 350, 0.343841269621561, 999.999999987944, -0.0006001526499741, 180.755776708117, 2 ),
      ( 350, 6335.35626723741, 999999.999999556, -0.945759181034624, 197.017426802029, 1 ),
      ( 350, 6461.45975569354, 10000000.0002395, -0.468177586235, 197.432727149244, 1 ),
      ( 350, 7170.37244069846, 100000000.000327, 3.79242766279485, 201.47478815332, 1 ),
      ( 400, 0.300790714328043, 999.999970117281, -0.000366254653619333, 203.076858426118, 2 ),
      ( 400, 3.01789425058795, 9999.99994868289, -0.0036743392316491, 203.211863379579, 2 ),
      ( 400, 6034.05216074826, 10000000.000323, -0.501693819804957, 217.444880278251, 1 ),
      ( 400, 6914.69583443091, 99999999.9966207, 3.34842768960855, 221.263535400843, 1 ),
      ( 500, 0.240583681151256, 999.999998252045, -0.000163113683821869, 244.233809682506, 2 ),
      ( 500, 2.40937914854078, 9999.9814768785, -0.00163309898153947, 244.282908058594, 2 ),
      ( 500, 24.4588175211378, 100000.000007679, -0.0165328374828865, 244.783379160769, 2 ),
      ( 500, 5056.2730282392, 10000000.0000002, -0.524265328796022, 255.811744099777, 1 ),
      ( 500, 6442.70977977132, 99999999.9989389, 2.73359109568628, 258.658673023552, 1 ),
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
      ( 250, 0.481340814706527, 999.99998069188, -0.00052340747464911, 78.665922284799, 2 ),
      ( 250, 10742.233953801, 100000.000001892, -0.995521519299962, 91.5712026605141, 1 ),
      ( 250, 10758.2328754882, 1000000.00077286, -0.955281793672427, 91.6161639897363, 1 ),
      ( 250, 10907.0811131898, 10000000.0000003, -0.558920601833136, 92.0554514109668, 1 ),
      ( 250, 11864.0009091154, 99999999.9999977, 3.05503068489931, 95.5594667442293, 1 ),
      ( 300, 0.401020682223899, 999.999998552752, -0.000282490307714135, 90.7775540426958, 2 ),
      ( 300, 4.02046134899856, 9999.984450276, -0.00283234996046798, 90.8516666590777, 2 ),
      ( 300, 41.2928700595476, 100000.009169402, -0.0291122994056533, 91.6379529376066, 2 ),
      ( 300, 9832.05193326854, 999999.999999554, -0.959224442620023, 100.698459913899, 1 ),
      ( 300, 10071.9020727087, 10000000.0007931, -0.601954630910637, 101.112553276288, 1 ),
      ( 300, 11333.624037492, 100000000.000008, 2.5373274818507, 104.662952809862, 1 ),
      ( 350, 0.343693124624611, 999.999999766614, -0.000169372757311637, 103.721549363119, 2 ),
      ( 350, 3.44218750295735, 9999.99757341772, -0.00169612428314711, 103.759278510781, 2 ),
      ( 350, 34.9651859888542, 99999.9998823916, -0.0172083955476671, 104.148008990312, 2 ),
      ( 350, 8689.75648624685, 1000000.00012382, -0.960455173518545, 111.895006767017, 1 ),
      ( 350, 9140.96416600081, 9999999.99999976, -0.624071480719565, 111.991065508712, 1 ),
      ( 350, 10830.7660915951, 100000000.000038, 2.17276644576835, 115.446690464282, 1 ),
      ( 400, 0.300713219336787, 999.999999951925, -0.000108645077188154, 116.630732457493, 2 ),
      ( 400, 3.0100784758853, 9999.99950762985, -0.00108734456289221, 116.652048600949, 2 ),
      ( 400, 30.401387126966, 99999.9999979005, -0.0109643775356111, 116.868661385486, 2 ),
      ( 400, 341.96608085514, 999999.998098716, -0.120729904959655, 119.465557712847, 2 ),
      ( 400, 8030.58370378961, 10000000.0002383, -0.625580705683403, 123.759231076087, 1 ),
      ( 400, 10353.0603806968, 100000000.000309, 1.90426731101979, 126.73810736098, 1 ),
      ( 500, 0.24055652742915, 999.999999996835, -5.02533379256617E-05, 140.546913208403, 2 ),
      ( 500, 2.40665417247135, 9999.99996799849, -0.000502683715169904, 140.555380560088, 2 ),
      ( 500, 24.1763392111549, 99999.999999997, -0.00504192729081202, 140.640425919894, 2 ),
      ( 500, 253.734247246286, 1000000.00039552, -0.0519827683260782, 141.529737577039, 2 ),
      ( 500, 4617.30903183763, 10000000, -0.479037601767466, 148.056966313718, 1 ),
      ( 500, 9468.8686750589, 100000000.008627, 1.54037147343097, 148.42438420713, 1 ),
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
