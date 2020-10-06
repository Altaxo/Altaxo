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
  /// Tests and test data for <see cref="Mixture_Oxygen_Water"/>.
  /// </summary>
  /// <remarks>
  /// <para>Reference:</para>
  /// <para>The test data was created automatically using calls into the TREND.DLL of the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// </remarks>
  
  public class Test_Mixture_Oxygen_Water : MixtureTestBase
  {

    public Test_Mixture_Oxygen_Water()
    {
      _mixture = MixtureOfFluids.FromCASRegistryNumbersAndMoleFractions(new[] { ("7782-44-7", 0.5), ("7732-18-5", 0.5) });

      // TestData for 1 Permille to 999 Permille Molefraction contains:
      // 0. Temperature (Kelvin)
      // 1. Mole density (mol/m³)
      // 2. Pressure (Pa)
      // 3. delta*AlphaR_delta
      // 4. Isochoric heat capacity (J/mol K)
      // 5. Phasetype (1: liquid, 2: gas)
      _testData_001_999 = new (double temperature, double moleDensity, double pressure, double deltaPhiR_delta, double cv, int phase)[]
      {
      ( 300, 0.40110512802755, 999.999999770333, -0.000480861243839717, 25.4000683009942, 2 ),
      ( 300, 55507.5709920153, 10000000.0000901, -0.927773410914426, 73.9382334787594, 1 ),
      ( 350, 0.343707799955624, 999.999999728591, -0.000199956802575905, 25.5916737709262, 2 ),
      ( 350, 3.44332058213598, 9999.99983044311, -0.00201254847390891, 25.882052338592, 2 ),
      ( 350, 54233.9052145848, 9999999.99999254, -0.93663759379249, 69.766111959992, 1 ),
      ( 400, 0.30071564987522, 999.999999994073, -0.000104619721955959, 25.9548146547279, 2 ),
      ( 400, 3.00999919702, 9999.99993842012, -0.00104893919158058, 26.056489386706, 2 ),
      ( 400, 30.3957744715272, 99999.9999987302, -0.010769771778249, 27.1567771526286, 2 ),
      ( 400, 52248.4835500628, 10000000.0000007, -0.942451116538228, 65.2351954513755, 1 ),
      ( 500, 0.2405571489407, 999.99999999894, -4.07290771032906E-05, 26.910285001834, 2 ),
      ( 500, 2.40645425051571, 9999.99998925822, -0.000407544757635043, 26.9329212186369, 2 ),
      ( 500, 24.1537902345797, 99999.9999999997, -0.00410101711865293, 27.1647294082231, 2 ),
      ( 500, 251.556604120022, 999999.999999978, -0.0437645153006439, 30.0594986034443, 2 ),
      ( 500, 46438.9554604523, 10000000.0000042, -0.948201386340512, 58.0070857930535, 1 ),
      ( 600, 0.200460141452496, 999.999999999997, -2.00308854684547E-05, 28.0071979531326, 2 ),
      ( 600, 2.00496296022415, 9999.99999930182, -0.000200352649140034, 28.0154275288216, 2 ),
      ( 600, 20.085943516609, 99999.9912176511, -0.00200791698712038, 28.0983660459388, 2 ),
      ( 600, 204.657753145831, 999999.999796984, -0.0205300166840083, 28.9930515665065, 2 ),
      ( 600, 2759.49966804139, 10000000, -0.273578002636113, 47.0964371787682, 2 ),
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
      ( 300, 0.400936194018997, 999.999999995318, -5.78331278444141E-05, 23.17762452672, 2 ),
      ( 350, 0.343651253801532, 999.999999999332, -3.35626227588817E-05, 23.4728963702388, 2 ),
      ( 350, 3.43755116422864, 9999.99999326403, -0.000335693792267163, 23.4809329990644, 2 ),
      ( 400, 0.300690949779217, 999.999999999885, -2.06019190081299E-05, 23.8705551100654, 2 ),
      ( 400, 3.00746720994694, 9999.99999881647, -0.000206040570811291, 23.8754296331191, 2 ),
      ( 400, 30.130621175032, 99999.9878358923, -0.00206254196312421, 23.9242662657784, 2 ),
      ( 500, 0.240549833520939, 999.999999999995, -8.43711263969201E-06, 24.8454710399987, 2 ),
      ( 500, 2.40568101770131, 9999.9999999482, -8.43744248845379E-05, 24.8477339225031, 2 ),
      ( 500, 24.0751014969028, 99999.9994762772, -0.000844071178940953, 24.8703637387455, 2 ),
      ( 500, 242.602761897575, 999999.999999859, -0.00847046384222191, 25.0967498567946, 2 ),
      ( 600, 0.200457208249806, 999.997251372627, -3.44768400957421E-06, 25.8948665710103, 2 ),
      ( 600, 2.00463414975977, 9999.99999999798, -3.44784287088439E-05, 25.8961428914184, 2 ),
      ( 600, 20.052567107672, 99999.9999753943, -0.000344932216980468, 25.9089005182657, 2 ),
      ( 600, 201.152981729162, 1000000, -0.00346243147408662, 26.0359192561637, 2 ),
      ( 600, 2075.85021383528, 9999999.99778182, -0.0343402333275534, 27.2480738261655, 2 ),
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
      ( 300, 0.400916253913907, 999.998453849983, -6.21769322357737E-06, 21.0749600832136, 2 ),
      ( 300, 4.00938686279594, 9999.99999999993, -6.21669713321073E-05, 21.0756686768776, 2 ),
      ( 300, 40.1162746922653, 99999.9999992644, -0.000620660290030585, 21.0827511496287, 2 ),
      ( 300, 403.375127307807, 999999.993664012, -0.006101928389012, 21.1532087174721, 2 ),
      ( 350, 0.343641184453062, 999.999494861715, -2.33304853660567E-06, 21.384008734237, 2 ),
      ( 350, 3.43648381356477, 10000, -2.33222942460905E-05, 21.3844945742377, 2 ),
      ( 350, 34.3720247662835, 99999.9999999632, -0.000232401133095542, 21.3893493334569, 2 ),
      ( 350, 344.411924039177, 999999.999742882, -0.00224021671938757, 21.4375256935363, 2 ),
      ( 350, 3480.30156363478, 10000000, -0.0126132451323654, 21.8762033986783, 1 ),
      ( 400, 0.300685375465476, 999.999956608602, -1.78071794222842E-07, 21.7964674583349, 2 ),
      ( 400, 3.00685854456774, 9999.99578325738, -1.77462527127943E-06, 21.796825410835, 2 ),
      ( 400, 30.0690473578623, 99999.9999999999, -1.71363153895281E-05, 21.8004019373189, 2 ),
      ( 400, 300.71829037971, 1000000, -0.00010963592482112, 21.8358648401915, 1 ),
      ( 400, 2989.90374349378, 10000000, 0.00566889989734174, 22.1586175516, 1 ),
      ( 500, 0.240547812411068, 1000.00042214433, 1.81069629484496E-06, 22.7826953421711, 1 ),
      ( 500, 2.40543900376856, 10000, 1.81102268822887E-05, 22.7829141404937, 1 ),
      ( 500, 24.0504622035684, 100000.000000016, 0.000181429551805007, 22.785100202357, 1 ),
      ( 500, 240.104733018582, 1000000.00018368, 0.0018472091618163, 22.8067687685321, 1 ),
      ( 500, 2353.94312050782, 10000000.0000001, 0.0218949412404378, 23.0045106715235, 1 ),
      ( 600, 0.200456367194563, 1000.00053138608, 2.51080697933046E-06, 23.7831894715658, 1 ),
      ( 600, 2.00451847242034, 10000, 2.51099000819801E-05, 23.7833361270255, 1 ),
      ( 600, 20.0406521755206, 100000.000000034, 0.000251283302765555, 23.7848013909043, 1 ),
      ( 600, 199.950740116135, 1000000.00034621, 0.0025313257202913, 23.7993253419973, 1 ),
      ( 600, 1951.48490393972, 10000000, 0.0272017998356043, 23.9321273261064, 1 ),
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
