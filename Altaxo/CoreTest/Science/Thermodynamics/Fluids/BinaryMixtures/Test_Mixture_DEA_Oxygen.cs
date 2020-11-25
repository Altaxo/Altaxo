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
  /// Tests and test data for <see cref="Mixture_DEA_Oxygen"/>.
  /// </summary>
  /// <remarks>
  /// <para>Reference:</para>
  /// <para>The test data was created automatically using calls into the TREND.DLL of the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// </remarks>
  
  public class Test_Mixture_DEA_Oxygen : MixtureTestBase
  {

    public Test_Mixture_DEA_Oxygen()
    {
      _mixture = MixtureOfFluids.FromCASRegistryNumbersAndMoleFractions(new[] { ("111-42-2", 0.5), ("7782-44-7", 0.5) });

      // TestData for 1 Permille to 999 Permille Molefraction contains:
      // 0. Temperature (Kelvin)
      // 1. Mole density (mol/m³)
      // 2. Pressure (Pa)
      // 3. delta*AlphaR_delta
      // 4. Isochoric heat capacity (J/mol K)
      // 5. Phasetype (1: liquid, 2: gas)
      _testData_001_999 = new (double temperature, double moleDensity, double pressure, double deltaPhiR_delta, double cv, int phase)[]
      {
      ( 500, 0.240547808787942, 1000.00042075873, 1.81476295685147E-06, 23.0191357802379, 1 ),
      ( 500, 2.40543887956346, 9999.99999999999, 1.81509487630697E-05, 23.0193548117714, 1 ),
      ( 500, 24.0504520166226, 100000.000000016, 0.000181842278370447, 23.0215432196572, 1 ),
      ( 500, 240.10361329186, 1000000.00018321, 0.00185187035677778, 23.0432365888084, 1 ),
      ( 500, 2353.74410560873, 10000000, 0.0219813338371387, 23.2413219040715, 1 ),
      ( 600, 0.200456363147851, 1000.00052190467, 2.51989865934195E-06, 24.0464685779509, 1 ),
      ( 600, 2.00451826823761, 9999.99999999999, 2.52008496440424E-05, 24.0466155475599, 1 ),
      ( 600, 20.0406336694731, 100000.000000033, 0.000252196044357135, 24.0480839586118, 1 ),
      ( 600, 199.948854789513, 1000000.0003353, 0.00254076769087996, 24.0626399283114, 1 ),
      ( 600, 1951.26145622738, 10000000, 0.027319418114674, 24.195799701049, 1 ),
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
      ( 500, 0.240552689072067, 999.998455869006, -2.57645969398794E-05, 143.066821892876, 2 ),
      ( 500, 2.4060848166905, 9999.99999999967, -0.000257639918591902, 143.071490614376, 2 ),
      ( 600, 0.200457090894459, 999.999771005096, -8.38813541869666E-06, 157.535522748207, 2 ),
      ( 600, 2.00472219900097, 9999.99999999997, -8.38543477799953E-05, 157.53809733311, 2 ),
      ( 600, 20.0623095961491, 99999.9999999779, -0.000835828635852187, 157.563837816232, 2 ),
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
      ( 500, 0.24062675966462, 999.99999997726, -0.000340905791974642, 263.129385127784, 2 ),
      ( 500, 2.41370166737337, 9999.99976006419, -0.00341980170176197, 263.257122940834, 2 ),
      ( 600, 0.200487927422516, 999.999999993853, -0.000169521007244164, 291.032342533205, 2 ),
      ( 600, 2.00794803655727, 9999.99993676177, -0.00169756955880437, 291.089762339928, 2 ),
      ( 600, 20.3966105340516, 99999.9999999388, -0.0172193552822501, 291.677746138718, 2 ),
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
