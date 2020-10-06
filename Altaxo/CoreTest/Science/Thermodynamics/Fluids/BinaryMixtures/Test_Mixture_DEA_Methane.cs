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
  /// Tests and test data for <see cref="Mixture_DEA_Methane"/>.
  /// </summary>
  /// <remarks>
  /// <para>Reference:</para>
  /// <para>The test data was created automatically using calls into the TREND.DLL of the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// </remarks>
  
  public class Test_Mixture_DEA_Methane : MixtureTestBase
  {

    public Test_Mixture_DEA_Methane()
    {
      _mixture = MixtureOfFluids.FromCASRegistryNumbersAndMoleFractions(new[] { ("111-42-2", 0.5), ("74-82-8", 0.5) });

      // TestData for 1 Permille to 999 Permille Molefraction contains:
      // 0. Temperature (Kelvin)
      // 1. Mole density (mol/m³)
      // 2. Pressure (Pa)
      // 3. delta*AlphaR_delta
      // 4. Isochoric heat capacity (J/mol K)
      // 5. Phasetype (1: liquid, 2: gas)
      _testData_001_999 = new (double temperature, double moleDensity, double pressure, double deltaPhiR_delta, double cv, int phase)[]
      {
      ( 350, 0.343636515730789, 999.999999988282, -9.2303968144084E-06, 29.8127424054849, 2 ),
      ( 400, 0.300680596643829, 999.999999999876, -4.72525706546706E-06, 32.4827206290964, 2 ),
      ( 400, 3.00693380463944, 9999.99999877547, -4.7239527538142E-05, 32.4833316064989, 2 ),
      ( 400, 30.0820889268813, 99999.9883083139, -0.000471088965656033, 32.4894364734764, 2 ),
      ( 500, 0.240543402332815, 999.996828113137, -2.51281230631443E-07, 38.4175304370798, 2 ),
      ( 500, 2.40543943160748, 9999.99999999998, -2.5046574354284E-06, 38.4178504973648, 2 ),
      ( 500, 24.0549169167952, 99999.9999999442, -2.42299210588511E-05, 38.4210484457797, 2 ),
      ( 500, 240.581899604695, 1000000.00052066, -0.000160273587476492, 38.4527638688802, 1 ),
      ( 500, 2389.06247250759, 10000000.000352, 0.00685245132260355, 38.7447001507253, 1 ),
      ( 600, 0.200452451093228, 1000.01733664024, 1.62775176035925E-06, 44.4245516758225, 1 ),
      ( 600, 2.00449520150565, 10000.0000000025, 1.62821537811302E-05, 44.4247378799003, 1 ),
      ( 600, 20.0420052630871, 100000.000026197, 0.00016331334706433, 44.426598819496, 1 ),
      ( 600, 200.116122092257, 1000000, 0.001682332260239, 44.4450990773742, 1 ),
      ( 600, 1961.94333822308, 10000000.0000992, 0.0217052653624487, 44.6203843156237, 1 ),
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
      ( 500, 0.240556551782539, 999.999999986892, -5.20443072580225E-05, 150.775432321635, 2 ),
      ( 500, 2.40669317137106, 9999.99999999511, -0.00052056886663102, 150.784667435969, 2 ),
      ( 600, 0.200458086466428, 999.999999999121, -2.35775649727602E-05, 167.736287849764, 2 ),
      ( 600, 2.00500630848327, 9999.99999120302, -0.000235763325103093, 167.740583621824, 2 ),
      ( 600, 20.092681921558, 99999.9999999999, -0.00235637563981054, 167.783637653433, 2 ),
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
      ( 500, 0.240626855300401, 999.999999980028, -0.000341323540144354, 263.14482272573, 2 ),
      ( 500, 2.41371180447356, 9999.99978940172, -0.0034240075227756, 263.272732675208, 2 ),
      ( 600, 0.200487965632366, 999.999999997209, -0.000169732002025244, 291.052756670167, 2 ),
      ( 600, 2.00795225280621, 9999.99997132686, -0.00169968618720147, 291.110269731254, 2 ),
      ( 600, 20.3970639599342, 99999.9999999878, -0.0172412225488572, 291.699226545933, 2 ),
      ( 600, 8181.70795508854, 10000000.0053546, -0.75499744355782, 324.918931516053, 1 ),
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
