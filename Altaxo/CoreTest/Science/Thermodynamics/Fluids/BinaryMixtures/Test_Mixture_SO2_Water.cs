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
  /// Tests and test data for <see cref="Mixture_SO2_Water"/>.
  /// </summary>
  /// <remarks>
  /// <para>Reference:</para>
  /// <para>The test data was created automatically using calls into the TREND.DLL of the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// </remarks>
  
  public class Test_Mixture_SO2_Water : MixtureTestBase
  {

    public Test_Mixture_SO2_Water()
    {
      _mixture = MixtureOfFluids.FromCASRegistryNumbersAndMoleFractions(new[] { ("7446-09-5", 0.5), ("7732-18-5", 0.5) });

      // TestData for 1 Permille to 999 Permille Molefraction contains:
      // 0. Temperature (Kelvin)
      // 1. Mole density (mol/m³)
      // 2. Pressure (Pa)
      // 3. delta*AlphaR_delta
      // 4. Isochoric heat capacity (J/mol K)
      // 5. Phasetype (1: liquid, 2: gas)
      _testData_001_999 = new (double temperature, double moleDensity, double pressure, double deltaPhiR_delta, double cv, int phase)[]
      {
      ( 300, 55286.890778996, 1000000.00001153, -0.992748511617091, 74.3522535468883, 1 ),
      ( 300, 55508.2896744082, 9999999.99995267, -0.927774347114767, 73.8329396881009, 1 ),
      ( 350, 0.343708335634344, 999.999999722006, -0.00020152970038997, 25.6041765820863, 2 ),
      ( 350, 3.44337574011153, 9999.99982243696, -0.0020285494652914, 25.8988474973282, 2 ),
      ( 350, 54023.0600593182, 1000000.00000957, -0.993639029928713, 70.0556938587163, 1 ),
      ( 350, 54242.3741052891, 9999999.99999966, -0.936647487529027, 69.7463263968933, 1 ),
      ( 400, 0.300715846653201, 999.999999970395, -0.000105288702549582, 25.9683271093079, 2 ),
      ( 400, 3.0100194611626, 9999.99968930283, -0.00105567900313081, 26.0713135097708, 2 ),
      ( 400, 30.3980049822874, 100000.00028789, -0.0108423729580436, 27.1866500965812, 2 ),
      ( 400, 52016.3761554788, 1000000.00000317, -0.994219432284683, 65.4478576800115, 1 ),
      ( 400, 52267.9081987016, 10000000.0000275, -0.942472504633231, 65.2582202025987, 1 ),
      ( 500, 0.240557193970674, 999.999999998924, -4.09309445958846E-05, 26.9257711822949, 2 ),
      ( 500, 2.40645908454195, 9999.9999890935, -0.000409567389786081, 26.9486367459036, 2 ),
      ( 500, 24.1542901325008, 99999.9999999997, -0.00412164289887251, 27.1828470264826, 2 ),
      ( 500, 251.622619736481, 999999.999999979, -0.0440154069240037, 30.1136939255807, 2 ),
      ( 500, 46492.7626480875, 9999999.99999407, -0.948261334875374, 58.0546179150303, 1 ),
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
      ( 300, 36659.4163979096, 1000000.00001346, -0.989063926753661, 66.5548535177859, 1 ),
      ( 300, 36765.6865996518, 10000000.0000229, -0.890955371724513, 66.4102214752959, 1 ),
      ( 350, 0.343781185413229, 999.99999814501, -0.000418838165765384, 29.5678071416404, 2 ),
      ( 350, 35353.3164284128, 1000000.00005641, -0.990279916243681, 66.9824353027949, 1 ),
      ( 350, 35467.978672277, 10000000.000015, -0.90311339704268, 66.8278622115084, 1 ),
      ( 400, 0.300747499036219, 999.999999989514, -0.000215968199476725, 30.578557871232, 2 ),
      ( 400, 3.01336980508763, 9999.998357568, -0.00217176550487785, 30.8014012754149, 2 ),
      ( 400, 33986.3788728303, 1000000.00011296, -0.991152851314529, 65.2412004235852, 1 ),
      ( 400, 34117.9652498071, 9999999.99998967, -0.911869730525174, 65.1195033269008, 1 ),
      ( 500, 0.240565807164818, 999.999999993724, -8.21789794958282E-05, 32.5813417474278, 2 ),
      ( 500, 2.4074414034451, 9999.99993566231, -0.000822876231338072, 32.6327546209126, 2 ),
      ( 500, 24.2568823752254, 99999.9999999721, -0.00833901891940417, 33.170923232039, 2 ),
      ( 500, 31053.6104584775, 9999999.99999546, -0.922538463592228, 60.6963060375132, 1 ),
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
      ( 300, 0.400972538281182, 999.999999999869, -0.000161254881009424, 31.643414014204, 2 ),
      ( 300, 4.01556345481446, 9999.99999867123, -0.00161488166252148, 31.7198257456548, 2 ),
      ( 300, 40.758817287286, 99999.9999999995, -0.0163898116168132, 32.5004084300828, 2 ),
      ( 300, 21319.2883144422, 1000000.01324688, -0.981195062476231, 51.9422569612198, 1 ),
      ( 300, 21613.4162838604, 10000000.0000178, -0.814509712746597, 51.7069535964169, 1 ),
      ( 350, 0.343666775263034, 999.999999999962, -9.15129936020605E-05, 33.4297422148698, 2 ),
      ( 350, 3.43950312770809, 9999.99999961921, -0.000915793565002269, 33.4651738970525, 2 ),
      ( 350, 34.6835027426485, 99999.9958726055, -0.00922542908480363, 33.824017993183, 2 ),
      ( 350, 382.007784184175, 999999.998323753, -0.100449415990669, 37.9672021266582, 2 ),
      ( 350, 19371.5107716985, 10000000.0030048, -0.822607885653395, 50.8956941250662, 1 ),
      ( 400, 0.30069810865709, 999.995316393714, -5.71967344105059E-05, 35.1535589386393, 2 ),
      ( 400, 3.00853055578579, 9999.99999990993, -0.00057219279877393, 35.170197448018, 2 ),
      ( 400, 30.2418124113214, 99999.9988591903, -0.00574441250864499, 35.3379011343896, 2 ),
      ( 400, 319.848662688888, 999999.999987146, -0.0599275698100956, 37.1624576391574, 2 ),
      ( 400, 16384.0464126579, 9999999.99999739, -0.816479456873175, 51.2148203632346, 1 ),
      ( 500, 0.240550978387173, 999.992791936856, -2.59846824140559E-05, 38.2286694090467, 2 ),
      ( 500, 2.40607256504355, 9999.99999999323, -0.000259879123857032, 38.2334652520823, 2 ),
      ( 500, 24.1172219052794, 99999.9999312572, -0.00260183926919522, 38.2815432278047, 2 ),
      ( 500, 247.049162576307, 999999.99999999, -0.0263285040399945, 38.7747438207836, 2 ),
      ( 500, 3423.07953441383, 10000000.0000012, -0.297285601213992, 45.4337521668105, 2 ),
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
