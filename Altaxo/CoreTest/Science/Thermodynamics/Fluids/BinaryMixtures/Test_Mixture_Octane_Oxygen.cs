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
  /// Tests and test data for <see cref="Mixture_Octane_Oxygen"/>.
  /// </summary>
  /// <remarks>
  /// <para>Reference:</para>
  /// <para>The test data was created automatically using calls into the TREND.DLL of the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// </remarks>
  
  public class Test_Mixture_Octane_Oxygen : MixtureTestBase
  {

    public Test_Mixture_Octane_Oxygen()
    {
      _mixture = MixtureOfFluids.FromCASRegistryNumbersAndMoleFractions(new[] { ("111-65-9", 0.5), ("7782-44-7", 0.5) });

      // TestData for 1 Permille to 999 Permille Molefraction contains:
      // 0. Temperature (Kelvin)
      // 1. Mole density (mol/m³)
      // 2. Pressure (Pa)
      // 3. delta*AlphaR_delta
      // 4. Isochoric heat capacity (J/mol K)
      // 5. Phasetype (1: liquid, 2: gas)
      _testData_001_999 = new (double temperature, double moleDensity, double pressure, double deltaPhiR_delta, double cv, int phase)[]
      {
      ( 250, 0.481103149534728, 999.999999999881, -1.38103317978515E-05, 21.0225220003413, 2 ),
      ( 250, 4.81162952181074, 9999.99999879681, -0.00013809633547299, 21.0236938687294, 2 ),
      ( 300, 0.400916267398787, 999.998443629401, -6.26800301458366E-06, 21.2300607773956, 2 ),
      ( 300, 4.00938881252924, 9999.99999999993, -6.26699070491717E-05, 21.2307675914003, 2 ),
      ( 300, 40.1164752533228, 99999.9999992486, -0.000625673320298251, 21.2378324093472, 2 ),
      ( 300, 403.394817326679, 999999.993526597, -0.00615045790746839, 21.3081271017883, 2 ),
      ( 350, 0.343641188882375, 999.999490787472, -2.36203326906777E-06, 21.5643366846758, 2 ),
      ( 350, 3.43648475182647, 10000, -2.36119920750238E-05, 21.5648213395752, 2 ),
      ( 350, 34.3721232780163, 99999.9999999623, -0.000235283173906596, 21.5696643204536, 2 ),
      ( 350, 344.421362499077, 999999.999734995, -0.00226757578283937, 21.6177298912663, 2 ),
      ( 350, 3480.82827771971, 9999999.99999999, -0.0127626715826028, 22.0557681341435, 1 ),
      ( 400, 0.300685375307447, 999.999952924014, -1.93904719489524E-07, 22.0019463551401, 2 ),
      ( 400, 3.00685897013739, 9999.99542200029, -1.93283290686518E-06, 22.0023035434265, 2 ),
      ( 400, 30.0690940644653, 100000, -1.870627283829E-05, 22.0058724695978, 2 ),
      ( 400, 300.722651086102, 1000000, -0.000124151766054357, 22.0412633834519, 1 ),
      ( 400, 2990.0472182824, 10000000, 0.00562062698997482, 22.3635739436632, 1 ),
      ( 500, 0.240547808821885, 1000.00041995528, 1.80897642695114E-06, 23.0342598528667, 1 ),
      ( 500, 2.40543900484688, 9999.99999999999, 1.80931031923534E-05, 23.0344783349383, 1 ),
      ( 500, 24.0504657403404, 100000.000000016, 0.000181265790724657, 23.0366612503315, 1 ),
      ( 500, 240.104946660341, 1000000.00018225, 0.00184630102739858, 23.058299899301, 1 ),
      ( 500, 2353.8295256488, 10000000, 0.0219442404351589, 23.2558522974574, 1 ),
      ( 600, 0.200456362875943, 1000.00052346941, 2.51558032762548E-06, 24.0736400619182, 1 ),
      ( 600, 2.00451834322021, 10000, 2.51576806369227E-05, 24.0737865872851, 1 ),
      ( 600, 20.0406421744889, 100000.000000034, 0.000251765786360584, 24.0752505569932, 1 ),
      ( 600, 199.949683399896, 1000000.00033702, 0.00253660729147221, 24.0897622178353, 1 ),
      ( 600, 1951.31510202366, 10000000, 0.0272911690037303, 24.2224868996107, 1 ),
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
      ( 300, 0.400971686777287, 999.999999951684, -0.00015468112001345, 100.731240072774, 2 ),
      ( 350, 0.343668404614243, 999.999999992136, -9.18028336382806E-05, 113.639049906492, 2 ),
      ( 350, 3.43952794581027, 9999.99992004105, -0.00091855540224113, 113.657677997373, 2 ),
      ( 400, 0.300699572196702, 999.999999999295, -5.76129051889405E-05, 126.611734521947, 2 ),
      ( 400, 3.00855617653808, 9999.9999928993, -0.000576255262462677, 126.622221951435, 2 ),
      ( 400, 30.2428844217898, 99999.9999935453, -0.00577523016914717, 126.727766251309, 2 ),
      ( 500, 0.24055167187007, 999.999999999941, -2.441659796728E-05, 150.629259414636, 2 ),
      ( 500, 2.40604534162161, 9999.99999939383, -0.000244117367752989, 150.633457660765, 2 ),
      ( 500, 24.1133272248306, 99999.9939450274, -0.00243630333565058, 150.675551067957, 2 ),
      ( 500, 246.42680990078, 1000000.00000077, -0.0238651447322039, 151.106436051353, 2 ),
      ( 600, 0.200456833143324, 999.999999999996, -9.98284495192397E-06, 171.121763376957, 2 ),
      ( 600, 2.00474833579915, 9999.99999997752, -9.97709573726023E-05, 171.123895749205, 2 ),
      ( 600, 20.0653874537588, 99999.9997848836, -0.000991969498440674, 171.145243836344, 2 ),
      ( 600, 202.347701781386, 999999.999999991, -0.00935454048097848, 171.360703983217, 2 ),
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
      ( 300, 0.40153841711606, 999.999999904193, -0.00157604702411319, 180.317853060672, 2 ),
      ( 350, 0.343927086142818, 999.999976126635, -0.000854068411264704, 205.756534215465, 2 ),
      ( 350, 3.46616870018175, 9999.98574886647, -0.00860753490664146, 206.164904544585, 2 ),
      ( 400, 0.300832641756133, 999.999996060598, -0.000510122466710217, 231.245639898676, 2 ),
      ( 400, 3.02227985046259, 9999.99999999949, -0.00512462395400114, 231.465227494657, 2 ),
      ( 400, 31.7793484520981, 99999.999988213, -0.0538535403845682, 233.793501861266, 2 ),
      ( 500, 0.240596400375447, 999.999999788985, -0.000220519352576384, 278.234655236983, 2 ),
      ( 500, 2.41075940774431, 9999.99779622545, -0.00220924759887283, 278.313141786279, 2 ),
      ( 500, 24.6083610664873, 99999.9998598267, -0.0225137563163142, 279.116339880639, 2 ),
      ( 500, 4745.25195193226, 10000000.0000043, -0.493086254113671, 293.150000356253, 1 ),
      ( 600, 0.200475110448123, 999.99999998028, -0.000111353493769498, 318.176226476182, 2 ),
      ( 600, 2.00676412406204, 9999.99979870587, -0.00111435895629912, 318.210437190963, 2 ),
      ( 600, 20.2728877534624, 99999.9999996678, -0.0112272640140231, 318.55657345194, 2 ),
      ( 600, 228.34490201551, 999999.999748598, -0.12214906011654, 322.483050246092, 2 ),
      ( 600, 3795.03565281767, 9999999.9999521, -0.471802625363747, 331.250760866216, 1 ),
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
