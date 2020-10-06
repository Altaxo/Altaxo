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
  /// Tests and test data for <see cref="Mixture_DEA_Hydrogen"/>.
  /// </summary>
  /// <remarks>
  /// <para>Reference:</para>
  /// <para>The test data was created automatically using calls into the TREND.DLL of the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// </remarks>
  
  public class Test_Mixture_DEA_Hydrogen : MixtureTestBase
  {

    public Test_Mixture_DEA_Hydrogen()
    {
      _mixture = MixtureOfFluids.FromCASRegistryNumbersAndMoleFractions(new[] { ("111-42-2", 0.5), ("1333-74-0", 0.5) });

      // TestData for 1 Permille to 999 Permille Molefraction contains:
      // 0. Temperature (Kelvin)
      // 1. Mole density (mol/m³)
      // 2. Pressure (Pa)
      // 3. delta*AlphaR_delta
      // 4. Isochoric heat capacity (J/mol K)
      // 5. Phasetype (1: liquid, 2: gas)
      _testData_001_999 = new (double temperature, double moleDensity, double pressure, double deltaPhiR_delta, double cv, int phase)[]
      {
      ( 350, 0.343633097292557, 1000.00000000104, 5.28321931627101E-06, 20.9451883565782, 1 ),
      ( 400, 0.300679111647899, 1000.00000000031, 4.77930100017112E-06, 21.0722036660255, 1 ),
      ( 400, 3.0066617891231, 10000.0000029787, 4.77931093694757E-05, 21.072315886262, 1 ),
      ( 400, 30.0536909706866, 99999.9999999998, 0.000477941219082698, 21.073437993454, 1 ),
      ( 500, 0.240543468492741, 1000.00000000001, 3.9553171566397E-06, 21.1828149531462, 1 ),
      ( 500, 2.40534925054599, 10000.0000013853, 3.95530607573829E-05, 21.1829040664434, 1 ),
      ( 500, 24.0449336595728, 100000.01667444, 0.000395519357944459, 21.1837951081868, 1 ),
      ( 500, 239.597039358489, 1000000.00005323, 0.00395413728075302, 21.1926963025739, 1 ),
      ( 500, 2314.06607058134, 10000000, 0.0394882065172536, 21.2807166813311, 1 ),
      ( 600, 0.200453017025431, 1000.00000000007, 3.3360566919969E-06, 21.2845424055053, 1 ),
      ( 600, 2.00447012127657, 10000.0000007251, 3.33604120970825E-05, 21.284615905637, 1 ),
      ( 600, 20.0386852382051, 100000.007216755, 0.000333588479293731, 21.2853508278473, 1 ),
      ( 600, 199.787536301873, 1000000.00000795, 0.00333435624445861, 21.2926921014056, 1 ),
      ( 600, 1940.0880833063, 10000000.0001165, 0.0332195782628946, 21.3652835342886, 1 ),
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
      ( 500, 0.240550712729815, 999.999999995718, -2.54867734381911E-05, 142.148629846491, 2 ),
      ( 500, 2.40605903760397, 9999.99999999529, -0.000254864453192814, 142.152638765574, 2 ),
      ( 600, 0.200455532927403, 999.999999999883, -8.55403587956622E-06, 156.154214081014, 2 ),
      ( 600, 2.00470961939475, 9999.99999885665, -8.5517202713201E-05, 156.15653854491, 2 ),
      ( 600, 20.0624918987608, 99999.9999998819, -0.000852839043444843, 156.179768403405, 2 ),
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
      ( 500, 0.240626789223559, 999.999999982881, -0.000341044462067057, 263.127557355414, 2 ),
      ( 500, 2.41370500974869, 9999.99981964881, -0.00342119755712967, 263.255354429173, 2 ),
      ( 600, 0.200487938820966, 999.999999997564, -0.000169593724410094, 291.029585298, 2 ),
      ( 600, 2.00794947182662, 9999.99997496837, -0.00169829899443505, 291.087034076713, 2 ),
      ( 600, 20.3967665243732, 99999.9999999908, -0.0172268869911877, 291.675320389522, 2 ),
      ( 600, 8181.11023487147, 10000000.0000032, -0.754979542430941, 324.882656771189, 1 ),
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
