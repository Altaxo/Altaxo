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
  /// Tests and test data for <see cref="Mixture_Octane_Helium"/>.
  /// </summary>
  /// <remarks>
  /// <para>Reference:</para>
  /// <para>The test data was created automatically using calls into the TREND.DLL of the following software:</para>
  /// <para>TREND 3.0.: Span, R.; Eckermann, T.; Herrig, S.; Hielscher, S.; Jäger, A.; Thol, M. (2016): TREND.Thermodynamic Reference and Engineering Data 3.0.Lehrstuhl für Thermodynamik, Ruhr-Universität Bochum.</para>
  /// </remarks>
  
  public class Test_Mixture_Octane_Helium : MixtureTestBase
  {

    public Test_Mixture_Octane_Helium()
    {
      _mixture = MixtureOfFluids.FromCASRegistryNumbersAndMoleFractions(new[] { ("111-65-9", 0.5), ("7440-59-7", 0.5) });

      // TestData for 1 Permille to 999 Permille Molefraction contains:
      // 0. Temperature (Kelvin)
      // 1. Mole density (mol/m³)
      // 2. Pressure (Pa)
      // 3. delta*AlphaR_delta
      // 4. Isochoric heat capacity (J/mol K)
      // 5. Phasetype (1: liquid, 2: gas)
      _testData_001_999 = new (double temperature, double moleDensity, double pressure, double deltaPhiR_delta, double cv, int phase)[]
      {
      ( 250, 0.481086067697128, 1000.00000000001, 5.835599268749E-06, 12.6160275922207, 1 ),
      ( 250, 4.81060802636142, 10000.0000000738, 5.83553811424816E-05, 12.6161364002616, 1 ),
      ( 300, 0.400905478227789, 1000, 4.78344108994722E-06, 12.6396308248065, 1 ),
      ( 300, 4.00888219879912, 10000.000000028, 4.78339214367664E-05, 12.6397200975305, 1 ),
      ( 300, 40.0715737510793, 100000.000278826, 0.000478290237489331, 12.6406126234562, 1 ),
      ( 300, 399.000978712902, 1000000.00000002, 0.0047779762847541, 12.6495177862085, 1 ),
      ( 350, 0.34363352701948, 1000, 4.02691324262856E-06, 12.6651398462484, 1 ),
      ( 350, 3.43621073611937, 10000.0000000122, 4.02687466110309E-05, 12.6652150557603, 1 ),
      ( 350, 34.3496602282816, 100000.000121027, 0.000402648871367157, 12.6659670010423, 1 ),
      ( 350, 342.258137325073, 1000000, 0.00402261720673456, 12.6734714972158, 1 ),
      ( 350, 3304.68995442745, 10000000.0000017, 0.03984009253727, 12.7470491772117, 1 ),
      ( 350, 25081.0027295045, 100000000, 0.370100368426018, 13.3629003835952, 1 ),
      ( 400, 0.300679506672571, 1000.01832528136, 3.45982388096061E-06, 12.6906734873364, 1 ),
      ( 400, 3.00670144576843, 10000.0000000058, 3.45972986996182E-05, 12.6907381417631, 1 ),
      ( 400, 30.0576564782186, 100000.000058097, 0.000345942371098776, 12.6913845725977, 1 ),
      ( 400, 299.644867352443, 1000000, 0.00345635687298786, 12.6978374264626, 1 ),
      ( 400, 2907.20835506297, 10000000.0000002, 0.0342586778412463, 12.7612418462675, 1 ),
      ( 400, 22783.8774789316, 99999999.9999996, 0.319707531036741, 13.3024330055811, 1 ),
      ( 500, 0.240543782104482, 1000.01106591118, 2.67149624667595E-06, 12.7377222916747, 1 ),
      ( 500, 2.40538011716272, 10000.0000000017, 2.67144681126335E-05, 12.7377722554036, 1 ),
      ( 500, 24.0480199368846, 100000.00001669, 0.000267124664105716, 12.7382718212301, 1 ),
      ( 500, 239.904075016672, 999999.999999999, 0.00266924412888065, 12.7432602839397, 1 ),
      ( 500, 2343.35974045713, 10000000.0019595, 0.0264938558498852, 12.7924375666095, 1 ),
      ( 500, 19252.8424579685, 100000000.000723, 0.249397007671904, 13.224593050699, 1 ),
      ( 600, 0.200453257555942, 1000.00732036092, 2.15401557113037E-06, 12.7777064973721, 1 ),
      ( 600, 2.00449380315933, 10000.0000000006, 2.15398612015398E-05, 12.777746803232, 1 ),
      ( 600, 20.0410532578001, 100000.000005917, 0.000215384836362691, 12.7781498136082, 1 ),
      ( 600, 200.023153933033, 1000000, 0.00215247098081699, 12.7821750696612, 1 ),
      ( 600, 1962.56132047851, 10000000.0002224, 0.0213882026319677, 12.8219502163767, 1 ),
      ( 600, 16664.3228473026, 100000000, 0.20289134940884, 13.1787357694591, 1 ),
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
      ( 300, 0.400918713184372, 999.990907821084, -3.05085427193903E-05, 96.4286980340297, 2 ),
      ( 350, 0.343639508877267, 999.999999999597, -1.56610862866047E-05, 109.183866241104, 2 ),
      ( 350, 3.43687903828872, 9999.99999999906, -0.000156469631328933, 109.185603277606, 2 ),
      ( 400, 0.300682024597601, 999.99999999972, -7.19489964637533E-06, 121.951232017159, 2 ),
      ( 400, 3.00701462007982, 9999.99979592088, -7.1834658829555E-05, 121.952336748152, 2 ),
      ( 400, 30.0892592162714, 99999.9999981354, -0.000706999599269207, 121.963407734036, 2 ),
      ( 600, 0.200452313834476, 1000.00000000003, 4.53373731855221E-06, 165.469110711248, 1 ),
      ( 600, 2.00444144535559, 10000.0000002936, 4.53806131470364E-05, 165.469509988693, 1 ),
      ( 600, 20.0361455190753, 100000.003790224, 0.000458100218972109, 165.473506595789, 1 ),
      ( 600, 199.459400008258, 1000000.00000829, 0.00498267215338559, 165.51380834109, 1 ),
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
      ( 350, 0.343926281415557, 999.99999998277, -0.000851746467514159, 205.74749102324, 2 ),
      ( 350, 3.46608614774209, 9999.99980485424, -0.00858395066820877, 206.154587450355, 2 ),
      ( 400, 0.300832232808294, 999.999996007057, -0.000508779639554475, 231.236244636389, 2 ),
      ( 400, 3.02223862416494, 9999.99999999948, -0.00511106867349319, 231.455123157487, 2 ),
      ( 400, 31.7742897498863, 99999.9999884441, -0.0537029218855645, 233.775402901319, 2 ),
      ( 400, 5364.90074181777, 1000000.00000029, -0.943954382389764, 250.093200559676, 1 ),
      ( 400, 5504.42638963225, 10000000.0009849, -0.453750210768801, 250.479187522799, 1 ),
      ( 400, 6209.43039072123, 100000000, 3.84229883553834, 254.730547743673, 1 ),
      ( 500, 0.2405962631761, 999.999999785973, -0.000219965103485417, 278.224323567628, 2 ),
      ( 500, 2.41074592398894, 9999.99776473783, -0.00220368260270081, 278.302549446965, 2 ),
      ( 500, 24.6068995888053, 99999.9998561208, -0.0224557159906714, 279.103030424034, 2 ),
      ( 500, 4742.01030987129, 10000000.0000025, -0.492739735522648, 293.116479350448, 1 ),
      ( 500, 5812.75269461764, 100000000.000001, 3.13820014426036, 296.532117521563, 1 ),
      ( 600, 0.200475051522532, 999.999999979997, -0.000111075470798299, 318.164911276912, 2 ),
      ( 600, 2.00675849527858, 9999.99979580224, -0.00111157302669381, 318.199008475631, 2 ),
      ( 600, 20.2723042908101, 99999.9999996592, -0.0111988215797815, 318.543986534618, 2 ),
      ( 600, 228.251070729485, 999999.999771151, -0.121788200036972, 322.455680287283, 2 ),
      ( 600, 3790.60455982354, 9999999.99995225, -0.471185188268036, 331.22622419004, 1 ),
      ( 600, 5452.31500941467, 99999999.9999997, 2.67647106440958, 332.95936202883, 1 ),
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
