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
  /// Tests and test data for <see cref="Mixture_Isopentane_Decane"/>.
  /// </summary>
  /// <remarks>
  /// <para>Reference:</para>
  /// <para>The test data was created automatically using calls into the TREND.DLL of the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// </remarks>
  
  public class Test_Mixture_Isopentane_Decane : MixtureTestBase
  {

    public Test_Mixture_Isopentane_Decane()
    {
      _mixture = MixtureOfFluids.FromCASRegistryNumbersAndMoleFractions(new[] { ("78-78-4", 0.5), ("124-18-5", 0.5) });

      // TestData for 1 Permille to 999 Permille Molefraction contains:
      // 0. Temperature (Kelvin)
      // 1. Mole density (mol/m³)
      // 2. Pressure (Pa)
      // 3. delta*AlphaR_delta
      // 4. Isochoric heat capacity (J/mol K)
      // 5. Phasetype (1: liquid, 2: gas)
      _testData_001_999 = new (double temperature, double moleDensity, double pressure, double deltaPhiR_delta, double cv, int phase)[]
      {
      ( 350, 0.344177023473309, 999.999992127252, -0.00157509380168374, 257.85868822663, 2 ),
      ( 400, 0.300956546967573, 999.999998458195, -0.000917071397923794, 289.574668966556, 2 ),
      ( 400, 3.03486911021666, 9999.9836288294, -0.00924704791960771, 290.038710270729, 2 ),
      ( 400, 4537.33314853913, 99999.9999999948, -0.993373187762908, 310.716719763759, 1 ),
      ( 400, 4547.36074229024, 1000000.00000082, -0.933878008505063, 310.759675457239, 1 ),
      ( 400, 4637.14305320683, 10000000.0000262, -0.35158233233471, 311.228164803835, 1 ),
      ( 400, 5138.62646427958, 99999999.9999978, 4.85137974935502, 316.129262967906, 1 ),
      ( 500, 0.240636295481714, 999.999894279732, -0.000381724673436939, 347.963549933783, 2 ),
      ( 500, 2.41469132808628, 9999.99999960812, -0.00382945072276069, 348.127958182019, 2 ),
      ( 500, 25.0465132948278, 100000.012016876, -0.0396090881396621, 349.842550486543, 2 ),
      ( 500, 3894.76222784964, 999999.999999956, -0.938238992629472, 365.614542913328, 1 ),
      ( 500, 4092.29582770247, 10000000.0012658, -0.412201735124306, 365.418967772055, 1 ),
      ( 500, 4823.10157990821, 100000000.000991, 3.98733926038143, 369.380861351253, 1 ),
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
      ( 300, 6268.75525821693, 100000.000000609, -0.99360467299717, 188.276165724718, 1 ),
      ( 300, 6277.83560689119, 1000000.01550491, -0.936139231882116, 188.322615073115, 1 ),
      ( 300, 6362.20685520877, 10000000.0000233, -0.369861108114543, 188.79415534876, 1 ),
      ( 300, 6900.89345320893, 99999999.9999989, 4.80949989282017, 193.214212907287, 1 ),
      ( 350, 0.343862946900458, 999.999999978766, -0.000663155277643933, 193.057579631801, 2 ),
      ( 350, 5871.62548270384, 1000000.00000053, -0.941475335342012, 209.887801101273, 1 ),
      ( 350, 5991.47879218462, 10000000.0002428, -0.426460604635991, 210.26793341544, 1 ),
      ( 350, 6656.25494951834, 100000000.000271, 4.16258639410074, 214.356124963189, 1 ),
      ( 400, 0.300801925260659, 999.999858403152, -0.000403511106181538, 217.13703918338, 2 ),
      ( 400, 3.0190298018727, 9999.99983751572, -0.0040490883417132, 217.292849502152, 2 ),
      ( 400, 5422.81974350686, 1000000.00678662, -0.944552730022502, 231.89811694759, 1 ),
      ( 400, 5603.71355842819, 10000000.0047925, -0.463426270221663, 232.077897089511, 1 ),
      ( 400, 6425.41089494423, 100000000.000003, 3.67955362298316, 235.808497779725, 1 ),
      ( 500, 0.240587805577791, 999.999991915412, -0.000180254010168202, 261.533092488207, 2 ),
      ( 500, 2.409793771691, 9999.99999999927, -0.00180487854702689, 261.588527246798, 2 ),
      ( 500, 24.5025897844838, 100000.000213551, -0.0182897368534596, 262.153548059409, 2 ),
      ( 500, 4724.74394782093, 10000000.0000065, -0.490883651437417, 273.730608816731, 1 ),
      ( 500, 5998.27778724895, 100000000.005136, 3.010225054642, 276.333994144471, 1 ),
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
      ( 250, 0.481476878178973, 999.999998953511, -0.000805855638611428, 94.1369244595473, 2 ),
      ( 250, 9176.15269728806, 1000000.00137269, -0.94757183173791, 109.605645008944, 1 ),
      ( 250, 9285.52214405986, 10000000.0000006, -0.481893565210908, 110.0065995164, 1 ),
      ( 250, 10013.2445409017, 100000000.00003, 3.80452539989788, 113.642985152687, 1 ),
      ( 300, 0.401076512596699, 999.9998410606, -0.000421652220162093, 111.338939141268, 2 ),
      ( 300, 4.02611286908603, 9999.99848529433, -0.00423209415842043, 111.482649835067, 2 ),
      ( 300, 8509.16566104954, 1000000.00000019, -0.952885228265907, 123.615086729297, 1 ),
      ( 300, 8672.73937831943, 9999999.99999993, -0.537738446551242, 123.952776445405, 1 ),
      ( 300, 9605.71113568617, 99999999.9999992, 3.17363578921709, 127.333453740681, 1 ),
      ( 350, 0.343720861195022, 999.999975536717, -0.000250054128832188, 128.29498590836, 2 ),
      ( 350, 3.44498124788852, 9999.9999999894, -0.00250571005539399, 128.36487635606, 2 ),
      ( 350, 35.2664654395519, 100000.001674437, -0.025604329964873, 129.08594043063, 2 ),
      ( 350, 7745.70892524643, 1000000.0050752, -0.955635447003642, 138.531694596308, 1 ),
      ( 350, 8013.63823142637, 10000000.0053699, -0.571187390015278, 138.695921104142, 1 ),
      ( 350, 9218.85000571728, 100000000.000879, 2.72752471471244, 141.801957380431, 1 ),
      ( 400, 0.300728958556965, 999.999994944207, -0.000160976286065928, 144.720160519661, 2 ),
      ( 400, 3.01165945652035, 9999.99999999975, -0.00161172713226966, 144.757695263458, 2 ),
      ( 400, 30.5668928568991, 100000.000157508, -0.0163195529046857, 145.139666412833, 2 ),
      ( 400, 372.568044410543, 999999.998465735, -0.192951319964143, 150.46785038928, 2 ),
      ( 400, 7270.15070738893, 10000000.0000001, -0.586417723060499, 153.67064981857, 1 ),
      ( 400, 8849.42243307154, 100000000.000217, 2.39774206283517, 156.365838502841, 1 ),
      ( 500, 0.240563056212013, 999.999999630188, -7.73915646557802E-05, 175.110027960038, 2 ),
      ( 500, 2.40730817877765, 9999.99619881716, -0.000774222211436137, 175.123471084123, 2 ),
      ( 500, 24.2428890275272, 99999.9999010918, -0.00777321377630246, 175.259008875961, 2 ),
      ( 500, 261.791585599563, 1000000.00000297, -0.0811605418496582, 176.739063949988, 2 ),
      ( 500, 5269.25108105224, 10000000.0009884, -0.543494065899619, 183.322390034659, 1 ),
      ( 500, 8159.50314239997, 100000000, 1.94802801668667, 184.156130001807, 1 ),
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
