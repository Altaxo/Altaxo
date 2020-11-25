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
  /// Tests and test data for <see cref="Mixture_Cl2_CO2"/>.
  /// </summary>
  /// <remarks>
  /// <para>Reference:</para>
  /// <para>The test data was created automatically using calls into the TREND.DLL of the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// </remarks>
  
  public class Test_Mixture_Cl2_CO2 : MixtureTestBase
  {

    public Test_Mixture_Cl2_CO2()
    {
      _mixture = MixtureOfFluids.FromCASRegistryNumbersAndMoleFractions(new[] { ("7782-50-5", 0.5), ("124-38-9", 0.5) });

      // TestData for 1 Permille to 999 Permille Molefraction contains:
      // 0. Temperature (Kelvin)
      // 1. Mole density (mol/m³)
      // 2. Pressure (Pa)
      // 3. delta*AlphaR_delta
      // 4. Isochoric heat capacity (J/mol K)
      // 5. Phasetype (1: liquid, 2: gas)
      _testData_001_999 = new (double temperature, double moleDensity, double pressure, double deltaPhiR_delta, double cv, int phase)[]
      {
      ( 250, 0.481129320669614, 999.999999866638, -8.8623383841615E-05, 26.5242577711448, 2 ),
      ( 250, 4.81513700508894, 9999.99862933945, -0.000886826457184994, 26.5458784085457, 2 ),
      ( 250, 48.5420811495348, 99999.9999852031, -0.00892860371839873, 26.7645542894131, 2 ),
      ( 300, 0.400925081059514, 999.999999995154, -4.86705858057669E-05, 28.9087969387544, 2 ),
      ( 300, 4.01100844118695, 9999.99985589775, -0.00048685091986277, 28.9182463891999, 2 ),
      ( 300, 40.2872845953671, 99999.9999999236, -0.00488312419908143, 29.0132449680948, 2 ),
      ( 300, 422.188734147898, 999999.999999995, -0.0504114975732355, 30.020531950998, 2 ),
      ( 300, 18229.5635542798, 9999999.99966887, -0.780079448091579, 41.7781915940084, 1 ),
      ( 350, 0.34364332148603, 999.999999997482, -2.90349208507866E-05, 31.0754449196259, 2 ),
      ( 350, 3.43733158311784, 9999.99997460018, -0.00029038362964525, 31.0803280116128, 2 ),
      ( 350, 34.463529872993, 99999.9999999989, -0.00290729040970463, 31.129293569819, 2 ),
      ( 350, 354.053267452086, 1000000.00002103, -0.0294303840151085, 31.632731036834, 2 ),
      ( 350, 5203.63980179843, 9999999.99982793, -0.339628881088046, 38.3380116294463, 2 ),
      ( 400, 0.300684631282411, 999.999999999519, -1.81433672619742E-05, 33.01366379177, 2 ),
      ( 400, 3.00733739903685, 9999.99999516429, -0.000181436419339613, 33.0164911808123, 2 ),
      ( 400, 30.122579192117, 100000, -0.00181463900089436, 33.0448062144173, 2 ),
      ( 400, 306.244802756601, 999999.997057113, -0.0181737839773312, 33.3319957263181, 2 ),
      ( 400, 3671.48287218915, 9999999.99669018, -0.18104159453054, 36.4686137956987, 2 ),
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
      ( 250, 0.481156981634052, 999.99999984457, -0.000143232395626269, 25.5936591721645, 2 ),
      ( 250, 4.8177898869788, 9999.99839025146, -0.00143411031933297, 25.6327460025886, 2 ),
      ( 250, 48.8179677203236, 100000.000012335, -0.0145266449391389, 26.0324004942687, 2 ),
      ( 300, 0.400938191543042, 999.999999977264, -7.84939872625549E-05, 27.2849528582171, 2 ),
      ( 300, 4.01221845285514, 9999.99976827294, -0.000785413297232396, 27.3033232614425, 2 ),
      ( 300, 40.4100003068456, 99999.9999997001, -0.00790220910053913, 27.4889024033232, 2 ),
      ( 300, 438.01780824039, 999999.999994979, -0.0847250665070642, 29.6035896571137, 2 ),
      ( 300, 20524.0703207082, 10000000.0040938, -0.804665100928135, 38.2620804829475, 1 ),
      ( 350, 0.343650660791946, 999.999999996206, -4.75165718370904E-05, 28.7453033487554, 2 ),
      ( 350, 3.43797741836501, 9999.99996161129, -0.000475308954497093, 28.7550049603192, 2 ),
      ( 350, 34.5280467561954, 99999.9999999958, -0.00476753253777263, 28.85261487549, 2 ),
      ( 350, 361.435355546468, 1000000.00637022, -0.0492509205323401, 29.8947269566, 2 ),
      ( 350, 16133.5749740242, 10000000.0000368, -0.787006703570086, 39.5157760057716, 1 ),
      ( 400, 0.300689272144468, 999.999999999199, -3.07025092586275E-05, 29.9992724868969, 2 ),
      ( 400, 3.00772397979493, 9999.99999191854, -0.000307068569894684, 30.0045374724544, 2 ),
      ( 400, 30.1607500307137, 99999.9999999999, -0.00307505640651242, 30.0573892959362, 2 ),
      ( 400, 310.367035274956, 1000000.0000007, -0.0312114172727001, 30.6069543060706, 2 ),
      ( 400, 4958.95983919805, 9999999.99999934, -0.393663086656599, 39.6253538244868, 2 ),
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
      ( 250, 0.481195304277302, 999.999999998082, -0.00021998738511145, 24.6616095262597, 2 ),
      ( 250, 4.82152345219084, 9999.99998018924, -0.00220448546167188, 24.7050630038611, 2 ),
      ( 250, 49.2177065268203, 99999.9999999913, -0.0225277012482809, 25.1535973937254, 2 ),
      ( 250, 21654.7835366335, 1000000.00007078, -0.977783687073706, 35.0667074196196, 1 ),
      ( 250, 21877.9779812205, 10000000.0104799, -0.780103331170825, 35.2025925235773, 1 ),
      ( 300, 0.400956342241887, 999.999999999574, -0.00012088454434193, 25.6611518552995, 2 ),
      ( 300, 4.01393617249395, 9999.99999565575, -0.00121014488275967, 25.6887599235665, 2 ),
      ( 300, 40.5873570710273, 99999.9999999998, -0.0122345924830544, 25.9693657133935, 2 ),
      ( 300, 19570.6689941104, 1000000.0000004, -0.979514861095792, 34.4674326339205, 1 ),
      ( 300, 19934.3110380756, 10000000.000004, -0.798885513499265, 34.4506399804404, 1 ),
      ( 350, 0.343660441277534, 999.999999999892, -7.31004123725105E-05, 26.4154300484497, 2 ),
      ( 350, 3.43886848690687, 9999.99999890376, -0.000731429928272784, 26.4322500227412, 2 ),
      ( 350, 34.6182355240079, 99999.9879869946, -0.00735749615096085, 26.6021070498376, 2 ),
      ( 350, 372.965309470327, 999999.993561381, -0.0786399940248337, 28.4922701861334, 2 ),
      ( 350, 17700.5196393742, 10000000.0000007, -0.805861451212353, 34.0378874235723, 1 ),
      ( 400, 0.300695192311339, 999.991541050681, -4.7515151475129E-05, 26.9851864920593, 2 ),
      ( 400, 3.00823888271386, 9999.99999975576, -0.000475306862537173, 26.9954845229208, 2 ),
      ( 400, 30.2121523931093, 99999.9968851728, -0.00476834370427407, 27.0991832268489, 2 ),
      ( 400, 316.29599735441, 999999.999906983, -0.0493686068449824, 28.2143773570625, 2 ),
      ( 400, 14647.4251593529, 10000000.00014, -0.794720982462583, 35.3264840678722, 1 ),
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
