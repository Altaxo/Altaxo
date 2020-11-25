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
  /// Tests and test data for <see cref="Mixture_Decane_Helium"/>.
  /// </summary>
  /// <remarks>
  /// <para>Reference:</para>
  /// <para>The test data was created automatically using calls into the TREND.DLL of the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// </remarks>
  
  public class Test_Mixture_Decane_Helium : MixtureTestBase
  {

    public Test_Mixture_Decane_Helium()
    {
      _mixture = MixtureOfFluids.FromCASRegistryNumbersAndMoleFractions(new[] { ("124-18-5", 0.5), ("7440-59-7", 0.5) });

      // TestData for 1 Permille to 999 Permille Molefraction contains:
      // 0. Temperature (Kelvin)
      // 1. Mole density (mol/m³)
      // 2. Pressure (Pa)
      // 3. delta*AlphaR_delta
      // 4. Isochoric heat capacity (J/mol K)
      // 5. Phasetype (1: liquid, 2: gas)
      _testData_001_999 = new (double temperature, double moleDensity, double pressure, double deltaPhiR_delta, double cv, int phase)[]
      {
      ( 250, 0.481086066595906, 1000.00000000221, 5.84245868592108E-06, 12.6549769189596, 1 ),
      ( 300, 0.400905477753022, 1000, 4.78919569971025E-06, 12.6851218815549, 1 ),
      ( 300, 4.00888198642984, 10000.0000000282, 4.7891469225783E-05, 12.6852112620783, 1 ),
      ( 350, 0.343633526897087, 1000, 4.03183977899846E-06, 12.7171382496619, 1 ),
      ( 350, 3.43621058254142, 10000.0000000123, 4.03180129269151E-05, 12.7172135515776, 1 ),
      ( 350, 34.3496434660289, 100000.000121996, 0.000403141629177692, 12.7179664207453, 1 ),
      ( 350, 342.256456111681, 1000000, 0.0040275537002494, 12.7254801400617, 1 ),
      ( 350, 3304.5314564804, 10000000.0000018, 0.0398899719914053, 12.7991481566751, 1 ),
      ( 350, 25071.7379655976, 100000000, 0.370606668129397, 13.4156724452338, 1 ),
      ( 400, 0.30067950675764, 1000.01838822879, 3.46411161388707E-06, 12.7489121833785, 1 ),
      ( 400, 3.00670133060244, 10000.0000000059, 3.4640173629591E-05, 12.7489769183069, 1 ),
      ( 400, 30.057643731162, 100000.00005857, 0.000346371176758116, 12.7496241540285, 1 ),
      ( 400, 299.643586672499, 1000000, 0.00346065024248356, 12.7560850429508, 1 ),
      ( 400, 2907.08684273729, 10000000.0000002, 0.0343019131920399, 12.8195682463871, 1 ),
      ( 400, 22776.3671203291, 100000000, 0.320142702026813, 13.3613673761645, 1 ),
      ( 500, 0.240543782376324, 1000.0111051151, 2.67486917296625E-06, 12.8073603875011, 1 ),
      ( 500, 2.40538004702948, 10000.0000000017, 2.67481961815434E-05, 12.8074104146604, 1 ),
      ( 500, 24.0480119374924, 100000.000016829, 0.000267461967086728, 12.8079106146938, 1 ),
      ( 500, 239.903268564588, 999999.999999999, 0.00267261925617167, 12.8129054092857, 1 ),
      ( 500, 2343.28244995926, 10000000.0019943, 0.0265277182701513, 12.8621449208331, 1 ),
      ( 500, 19247.6549357332, 100000000.000748, 0.249733744002536, 13.2948058817587, 1 ),
      ( 600, 0.200453257908753, 1000.00734680405, 2.15677083026935E-06, 12.8566592462881, 1 ),
      ( 600, 2.00449375709411, 10000.0000000006, 2.15674131193487E-05, 12.8566996040214, 1 ),
      ( 600, 20.0410478286995, 100000.000005967, 0.000215660365081184, 12.8571031330585, 1 ),
      ( 600, 200.022604733306, 999999.999999999, 0.00215522715932456, 12.8611335681979, 1 ),
      ( 600, 1962.50829408898, 10000000.0002264, 0.0214158049051879, 12.9009597217085, 1 ),
      ( 600, 16660.5428899067, 99999999.9999998, 0.20316426786635, 13.2581744104212, 1 ),
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
      ( 350, 0.343643531526391, 999.991402833263, -2.50814553446911E-05, 135.182975822678, 2 ),
      ( 400, 0.300684529104497, 999.999999996996, -1.32390537333745E-05, 151.070399217539, 2 ),
      ( 400, 3.00720285231103, 9999.9999999983, -0.000132139091361128, 151.072164369546, 2 ),
      ( 500, 0.240544802457217, 999.999999999997, -1.48279518962934E-06, 180.295216352094, 2 ),
      ( 500, 2.40547967676284, 9999.99999998865, -1.46707354484325E-05, 180.296062271858, 2 ),
      ( 500, 24.0575985120515, 99999.9999999973, -0.000131128881429925, 180.304546240044, 2 ),
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
      ( 350, 0.344173806460061, 999.999992242956, -0.00156576146436491, 257.742351788579, 2 ),
      ( 400, 0.300954962820995, 999.999896734081, -0.000911812399285828, 289.442220058784, 2 ),
      ( 400, 3.03470527447151, 9999.98267669422, -0.00919355898651628, 289.903056580775, 2 ),
      ( 500, 0.240635794228009, 999.999896172727, -0.000379642430687437, 347.800962266903, 2 ),
      ( 500, 2.41464050522251, 9999.99999962607, -0.0038084835272116, 347.964246508948, 2 ),
      ( 500, 25.0406219552377, 100000.011616666, -0.0393831355302374, 349.666672352128, 2 ),
      ( 500, 3891.17945627021, 999999.999999882, -0.938182126688333, 365.425811955108, 1 ),
      ( 500, 4090.66717021596, 10000000.0010225, -0.411967709243045, 365.216632684714, 1 ),
      ( 500, 4824.77941525903, 100000000.001032, 3.98560489422945, 369.167893967648, 1 ),
      ( 600, 0.200491431278414, 999.999999491806, -0.000188199536747865, 397.041334851814, 2 ),
      ( 600, 2.00832138457463, 9999.99999999939, -0.00188435763416156, 397.111777452878, 2 ),
      ( 600, 20.4354189587531, 100000.000206866, -0.0190869133619633, 397.830319321819, 2 ),
      ( 600, 259.156581640028, 999999.999090357, -0.226515114207789, 407.170851110017, 2 ),
      ( 600, 3467.79952643184, 10000000.0046731, -0.421957072664735, 412.324579393773, 1 ),
      ( 600, 4540.75248734469, 100000000.000002, 3.41454801693284, 414.701176003285, 1 ),
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
