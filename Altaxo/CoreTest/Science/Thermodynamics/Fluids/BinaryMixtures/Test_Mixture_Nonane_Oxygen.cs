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
  /// Tests and test data for <see cref="Mixture_Nonane_Oxygen"/>.
  /// </summary>
  /// <remarks>
  /// <para>Reference:</para>
  /// <para>The test data was created automatically using calls into the TREND.DLL of the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// </remarks>
  
  public class Test_Mixture_Nonane_Oxygen : MixtureTestBase
  {

    public Test_Mixture_Nonane_Oxygen()
    {
      _mixture = MixtureOfFluids.FromCASRegistryNumbersAndMoleFractions(new[] { ("111-84-2", 0.5), ("7782-44-7", 0.5) });

      // TestData for 1 Permille to 999 Permille Molefraction contains:
      // 0. Temperature (Kelvin)
      // 1. Mole density (mol/m³)
      // 2. Pressure (Pa)
      // 3. delta*AlphaR_delta
      // 4. Isochoric heat capacity (J/mol K)
      // 5. Phasetype (1: liquid, 2: gas)
      _testData_001_999 = new (double temperature, double moleDensity, double pressure, double deltaPhiR_delta, double cv, int phase)[]
      {
      ( 250, 0.48110315708115, 999.999999999845, -1.38214468908697E-05, 21.0417588674231, 2 ),
      ( 250, 4.81163007863175, 9999.99999355549, -0.000138207473620188, 21.0429315622486, 2 ),
      ( 300, 0.400916271542103, 999.998439290533, -6.27376714345439E-06, 21.2528206136585, 2 ),
      ( 300, 4.00938906190755, 9999.99999999992, -6.27275316038229E-05, 21.2535278815854, 2 ),
      ( 300, 40.1164984992919, 99999.9999992438, -0.000626247851757329, 21.26059723817, 2 ),
      ( 350, 0.343641191478984, 999.999487409306, -2.36496045925446E-06, 21.5903498432463, 2 ),
      ( 350, 3.43648486807936, 9999.99999999998, -2.36412499826503E-05, 21.5908347935936, 2 ),
      ( 350, 34.3721334455717, 99999.9999999619, -0.000235574343065112, 21.5956807264549, 2 ),
      ( 350, 344.422320092545, 999999.999732358, -0.0022703452069229, 21.6437755478073, 2 ),
      ( 350, 3480.87239821574, 9999999.99999999, -0.012775180428044, 22.0820633848877, 1 ),
      ( 400, 0.3006853770813, 999.999988638155, -1.95207622107428E-07, 22.0309723300354, 2 ),
      ( 400, 3.00685902302415, 9999.99583070405, -1.94585124453214E-06, 22.0313297310674, 2 ),
      ( 400, 30.0690980842857, 100000, -1.88353861041544E-05, 22.0349007821271, 2 ),
      ( 400, 300.723008588328, 1000000, -0.00012533585764033, 22.0703126964667, 1 ),
      ( 400, 2990.05080817791, 10000000, 0.00561942422432282, 22.3928025822466, 1 ),
      ( 500, 0.240547809850726, 1000.00042228571, 1.80926401801235E-06, 23.0689404625553, 1 ),
      ( 500, 2.40543900890861, 9999.99999999999, 1.80959850998533E-05, 23.0691590740251, 1 ),
      ( 500, 24.050465142759, 100000.000000016, 0.000181295213455981, 23.0713432817203, 1 ),
      ( 500, 240.104862859158, 1000000.00018444, 0.00184665526975198, 23.0929946841289, 1 ),
      ( 500, 2353.80841503604, 10000000.0000001, 0.0219534106220996, 23.2906569121288, 1 ),
      ( 600, 0.200456363602115, 1000.00053523266, 2.51650959611481E-06, 24.1130652637559, 1 ),
      ( 600, 2.00451833374823, 10000, 2.51669765989495E-05, 24.1132118771139, 1 ),
      ( 600, 20.0406403964204, 100000.000000035, 0.000251859103399973, 24.1146767255657, 1 ),
      ( 600, 199.949491128036, 1000000.00035261, 0.00253757591487909, 24.1291970568319, 1 ),
      ( 600, 1951.29051612266, 10000000, 0.0273041173783989, 24.2619968807343, 1 ),
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
      ( 300, 0.400985148490577, 999.997992069871, -0.00018596239423969, 112.112038853625, 2 ),
      ( 350, 0.343675650150802, 999.99999982159, -0.000110598450315686, 126.646008822387, 2 ),
      ( 350, 3.44018381797175, 9999.99999891692, -0.00110674795507426, 126.669177357728, 2 ),
      ( 400, 0.300703919834054, 999.999999994211, -6.97852463912602E-05, 141.124860012784, 2 ),
      ( 400, 3.0089297005285, 9999.99994142951, -0.000698038623646686, 141.137816310352, 2 ),
      ( 400, 30.2802286509446, 99999.9999981279, -0.00699912598313493, 141.268390811632, 2 ),
      ( 500, 0.240553624240022, 999.99999999613, -3.0247436554776E-05, 167.969502011132, 2 ),
      ( 500, 2.40619113088429, 9999.99996126605, -0.000302407309535315, 167.974593160809, 2 ),
      ( 500, 24.1274358417153, 99999.9999999979, -0.00301735464589206, 168.025674672709, 2 ),
      ( 500, 247.854607247333, 999999.999999999, -0.029486073383764, 168.551754126165, 2 ),
      ( 600, 202.919573435581, 999999.999999922, -0.012144138205003, 191.119189369631, 2 ),
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
      ( 350, 0.344025342703993, 999.99999708361, -0.00113487242465584, 231.760521471336, 2 ),
      ( 400, 0.300883443189565, 999.999267945334, -0.000674314077662269, 260.251332241523, 2 ),
      ( 400, 3.02734310093844, 9999.99369308813, -0.00678402018707652, 260.555250360606, 2 ),
      ( 500, 0.240614322857717, 999.999962413768, -0.000290424837159161, 312.883205270549, 2 ),
      ( 500, 2.41246770819941, 9999.99999996955, -0.00291124451861081, 312.991098451841, 2 ),
      ( 600, 0.200483255358642, 999.999996503845, -0.000147410277183665, 357.562653398099, 2 ),
      ( 600, 2.00749918034048, 9999.99999999987, -0.00147554714106194, 357.60930496972, 2 ),
      ( 600, 20.3486272158453, 100000.000026824, -0.0149030689930042, 358.08309511007, 2 ),
      ( 600, 240.940230026092, 999999.999995593, -0.168035566144899, 363.760868406096, 2 ),
      ( 600, 3645.58908894204, 10000000.0004043, -0.450147295312966, 370.296917702617, 1 ),
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
