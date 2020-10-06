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
  /// Tests and test data for <see cref="Mixture_Decane_Oxygen"/>.
  /// </summary>
  /// <remarks>
  /// <para>Reference:</para>
  /// <para>The test data was created automatically using calls into the TREND.DLL of the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// </remarks>
  
  public class Test_Mixture_Decane_Oxygen : MixtureTestBase
  {

    public Test_Mixture_Decane_Oxygen()
    {
      _mixture = MixtureOfFluids.FromCASRegistryNumbersAndMoleFractions(new[] { ("124-18-5", 0.5), ("7782-44-7", 0.5) });

      // TestData for 1 Permille to 999 Permille Molefraction contains:
      // 0. Temperature (Kelvin)
      // 1. Mole density (mol/m³)
      // 2. Pressure (Pa)
      // 3. delta*AlphaR_delta
      // 4. Isochoric heat capacity (J/mol K)
      // 5. Phasetype (1: liquid, 2: gas)
      _testData_001_999 = new (double temperature, double moleDensity, double pressure, double deltaPhiR_delta, double cv, int phase)[]
      {
      ( 250, 0.481103161367255, 999.999999987439, -1.38303556780013E-05, 21.0614708848264, 2 ),
      ( 300, 0.400916273383304, 999.998438031724, -6.27835958118241E-06, 21.2755511806912, 2 ),
      ( 300, 4.00938924595929, 9999.99999999991, -6.27734339054202E-05, 21.2762590682454, 2 ),
      ( 300, 40.1165168360306, 99999.9999992421, -0.000626704652516393, 21.2833346248265, 2 ),
      ( 350, 0.343641192312977, 999.99948677187, -2.36733980288164E-06, 21.6163342860956, 2 ),
      ( 350, 3.43648494978719, 9999.99999999999, -2.36650259908261E-05, 21.6168196037843, 2 ),
      ( 350, 34.3721415598783, 99999.9999999614, -0.000235810359641565, 21.6216692100307, 2 ),
      ( 350, 344.423075275301, 999999.999731585, -0.00227253283104588, 21.6698007348389, 2 ),
      ( 350, 3480.89519208911, 10000000, -0.012781645051735, 22.1084297779785, 1 ),
      ( 400, 0.300685377430612, 999.999952098499, -1.9634655944642E-07, 22.0601841265992, 2 ),
      ( 400, 3.00685905723097, 9999.99534144583, -1.95722738550575E-06, 22.060541770156, 2 ),
      ( 400, 30.0691014654372, 99999.9999999999, -1.89478300315008E-05, 22.0641152459034, 2 ),
      ( 400, 300.723307694741, 999999.999999998, -0.000126330356293918, 22.0995513181686, 1 ),
      ( 400, 2990.04711934847, 10000000, 0.00562066485980322, 22.4222643078904, 1 ),
      ( 600, 0.200456363492931, 1000.00053590044, 2.51704359812241E-06, 24.1525915197952, 1 ),
      ( 600, 2.00451832303471, 10000, 2.51723214223622E-05, 24.1527382120823, 1 ),
      ( 600, 20.0406393159081, 100000.000000035, 0.000251913033040617, 24.1542038492428, 1 ),
      ( 600, 199.949374133261, 1000000.00035362, 0.00253816252165367, 24.1687320082032, 1 ),
      ( 600, 1951.27179437786, 10000000, 0.0273139739886213, 24.3016034525415, 1 ),
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
      ( 350, 0.343682263514375, 999.999999999965, -0.000129838989773169, 139.639246907358, 2 ),
      ( 400, 0.300707410171301, 999.999999971769, -8.13915239690402E-05, 155.731372039276, 2 ),
      ( 400, 3.00927935647927, 9999.99999999458, -0.000814150106897856, 155.748937916726, 2 ),
      ( 500, 0.240554844257906, 999.99999999277, -3.53189660245363E-05, 185.447998181217, 2 ),
      ( 500, 2.4063131085279, 9999.99992780489, -0.000353082657197048, 185.454770998609, 2 ),
      ( 500, 24.1396092868149, 99999.9999623712, -0.00352012638696836, 185.522774778834, 2 ),
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
      ( 350, 0.344175292723144, 999.999992236958, -0.00157005718354861, 257.751526419422, 2 ),
      ( 400, 0.300955701533424, 999.999998490431, -0.00091424894799311, 289.451689745376, 2 ),
      ( 400, 3.03478123761392, 9999.98385562308, -0.00921834503200476, 289.913936754112, 2 ),
      ( 500, 0.240636030450049, 999.999895571905, -0.00038060784477202, 347.811321003131, 2 ),
      ( 500, 2.41466410859205, 9999.99999962012, -0.0038182054953841, 347.975122514723, 2 ),
      ( 500, 25.0433556237996, 100000.011799757, -0.0394879788468645, 349.683153085067, 2 ),
      ( 600, 0.200491527908284, 999.999999488974, -0.000188665537959426, 397.052661069104, 2 ),
      ( 600, 2.00833082549591, 9999.99999999943, -0.00188903380960295, 397.123328626304, 2 ),
      ( 600, 20.4364287234077, 100000.000210711, -0.0191353647404321, 397.844201069721, 2 ),
      ( 600, 259.426575500647, 999999.998977144, -0.227320093338422, 407.226649255852, 2 ),
      ( 600, 3470.63836195354, 10000000.0044636, -0.422429878194128, 412.350863585642, 1 ),
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
