#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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
using Altaxo.Calc.LinearAlgebra;
using Xunit;

namespace Altaxo.Calc.FitFunctions.Peaks
{
  public class PearsonIVAmplitudeTests
  {
    [Fact]
    public void TestDerivatives1()
    {
      // see PearsonIV (amplitudeModified).nb
      double amp = 3;
      double pos = 7;
      double w = 5;
      double m = 11;
      double v = 13;

      var ymaxDerivs = new double[] { 1, 0, 0, 0, 0 };
      var xmaxDerivs = new double[] { 0, 1, 0, 0, 0 };
      var areaDerivs = new double[] { 3.2274058360605690545, 0, 1.9364435016363414327, -0.70837802505116421171, 0.19708150944514741889 };
      var fwhmDerivs = new double[] { 0, 0, 0.59352560729636783014, -0.21021029561763018768, 0.05968873919496908876 };

      double[] pars = new double[] { amp, pos, w, m, v };
      double[] X = new double[1];
      double[] Y = new double[1];

      var func = new PearsonIVAmplitude();

      for (int i = 0; i < pars.Length; i++)
      {
        var cov = CreateMatrix.Dense<double>(pars.Length, pars.Length);
        cov[i, i] = 1;
        var result = func.GetPositionAreaHeightFWHMFromSinglePeakParameters(pars, cov);
        AssertEx.AreEqual(Math.Abs(areaDerivs[i]), result.AreaStdDev, 1E-13, 1E-7);
        AssertEx.AreEqual(Math.Abs(xmaxDerivs[i]), result.PositionStdDev, 1E-13, 1E-7);
        AssertEx.AreEqual(Math.Abs(ymaxDerivs[i]), result.HeightStdDev, 1E-13, 1E-7);
        AssertEx.AreEqual(Math.Abs(fwhmDerivs[i]), result.FWHMStdDev, 1E-13, 1E-7);
      }
    }

    [Fact]
    public void TestDerivatives2()
    {
      // see PearsonIV (amplitudeModified).nb
      double amp = 3;
      double pos = 7;
      double w = 5;
      double m = 300;
      double v = -1290;

      var ymaxDerivs = new double[] { 1, 0, 0, 0, 0 };
      var xmaxDerivs = new double[] { 0, 1, 0, 0, 0 };
      var areaDerivs = new double[] { 1.21532065684380740658, 0, 0.72919239410628444395, -0.016091029911639133025, -0.0023241047381251426829 };

      double[] pars = new double[] { amp, pos, w, m, v };
      double[] X = new double[1];
      double[] Y = new double[1];

      var func = new PearsonIVAmplitude();

      for (int i = 0; i < pars.Length; i++)
      {
        var cov = CreateMatrix.Dense<double>(pars.Length, pars.Length);
        cov[i, i] = 1;
        var result = func.GetPositionAreaHeightFWHMFromSinglePeakParameters(pars, cov);
        AssertEx.AreEqual(Math.Abs(areaDerivs[i]), result.AreaStdDev, 1E-13, 1E-7);
        AssertEx.AreEqual(Math.Abs(xmaxDerivs[i]), result.PositionStdDev, 1E-13, 1E-7);
        AssertEx.AreEqual(Math.Abs(ymaxDerivs[i]), result.HeightStdDev, 1E-13, 1E-7);
        // AssertEx.AreEqual(Math.Abs(fwhmDerivs[i]), result.FWHMVariance, 1E-13, 1E-7);
      }
    }

    [Fact]
    public void TestFHWM()
    {
      double w = 5;
      double m = 11;
      double v = 13;
      var left = PearsonIVAmplitude.GetHWHM(w, m, v, false);
      var right = PearsonIVAmplitude.GetHWHM(w, m, v, true);

      AssertEx.AreEqual(1.610212109928116475803608029, left, 1E-15, 1E-13);
      AssertEx.AreEqual(1.357415926553722674910974552, right, 1E-15, 1E-13);

      w = 5;
      m = 300;
      v = -1290;
      left = PearsonIVAmplitude.GetHWHM(w, m, v, false);
      right = PearsonIVAmplitude.GetHWHM(w, m, v, true);

      AssertEx.AreEqual(0.5537655602052538, left, 1E-15, 1E-13);
      AssertEx.AreEqual(0.58690275900381228, right, 1E-15, 2E-13);
    }

    [Fact]
    public void TestArea()
    {
      double result;

      result = PearsonIVAmplitude.GetArea(3, 5, 11, 13);
      AssertEx.AreEqual(9.6822175081817071635, result, 1E-13, 1E-12);


      result = PearsonIVAmplitude.GetArea(3, 27, 300, -1290);
      AssertEx.AreEqual(19.688194640869679987, result, 1E-13, 1E-12);
    }

    /// <summary>
    /// The half width half maximum data. First value: m = 2^mexp, 2nd value: v = 2^vexp, 3rd value left hwhm (negative), 4th value: right hwhm (positive).
    /// </summary>
    private static readonly (int mexp, int vexp, double leftHwhm, double rightHwhm)[] _hwhmData =
      new (int mexp, int vexp, double leftHwhm, double rightHwhm)[]
      {
        // values see PearsonIV_HWHMLimitForBigV notebook
        // Accuracy: 20 digits
        (-10,-10,-2.6075165635974747471e154,5.4204943856823757152e153),
        (-10,-8,-7.5781287303315414265e154,1.4151721417405125979e152),
        (-10,-6,-2.9232634846867489338e155,3.5551434645563366552e144),
        (-10,-4,-1.1664682047886355347e156,2.5516998171116092832e112),
        (-10,-2,-4.6651611392581847943e156,130.9001626588707435167),
        (-10,0,-1.8660466599569012555e157,510.8281593907665904418),
        (-10,2,-7.4641821908324085691e157,2042.398062831019854009),
        (-10,4,-2.9856727651079919462e158,8169.371257080991939586),
        (-10,6,-1.1942691032625723484e159,32677.429893645861045791),
        (-10,8,-4.7770764123551332837e159,130709.70579268809819762),
        (-10,10,-1.9108305649246744107e160,522838.8197253062727686),
        (-8,-10,-4.1089307548813017411e38,2.774481147935063354e38),
        (-8,-8,-6.6177253782475629663e38,1.3756899480357889964e38),
        (-8,-6,-1.9232849953272139427e39,3.5916245855268389074e36),
        (-8,-4,-7.4190726992836348074e39,9.0227473363568317138e28),
        (-8,-2,-2.9604284588315566484e40,35.6039148762950938017),
        (-8,0,-1.1839907633142484824e41,126.8954276921648686526),
        (-8,2,-4.735917888687748492e41,506.6299127799120423136),
        (-8,4,-1.8943660263459950874e42,2026.290197760684539801),
        (-8,6,-7.5774638231014718725e42,8105.103553846265135316),
        (-8,8,-3.030985522183525674e43,32420.39990805081007411),
        (-8,10,-1.2123942086969837842e44,129681.5960554003728732),
        (-6,-10,-4.5088544740271630801e9,4.0872335762086799083e9),
        (-6,-8,-5.1861997354077905015e9,3.5018875360558976736e9),
        (-6,-6,-8.3527437308214964306e9,1.7363648293375347183e9),
        (-6,-4,-2.4275269474589047217e10,4.5332679548043840472e7),
        (-6,-2,-9.3641862471318138517e10,13.164273669432868315),
        (-6,0,-3.7365860375614506539e11,31.07252667304393651501),
        (-6,2,-1.4944064402582316695e12,123.2295357724348528931),
        (-6,4,-5.977568755332160481e12,492.664147731102612275),
        (-6,6,-2.3910260769715724534e13,1970.593256511927709641),
        (-6,8,-9.5641039515956735491e13,7882.35719500477660639),
        (-6,10,-3.8256415717310035547e14,31529.4248222983670341),
        (-4,-10,-259.1354849924818384,252.88353594154514528),
        (-4,-8,-268.68430668561238861,243.67855136598364632),
        (-4,-6,-308.86994932776822775,208.97633165956920486),
        (-4,-4,-496.86106598281641801,104.48940832569722187),
        (-4,-2,-1442.9170830967305291,6.1965075733350978867),
        (-4,0,-5565.4841156897015829,7.44147518404087720735),
        (-4,2,-22207.765292603713455,28.4111136249969896628),
        (-4,4,-88817.472885715116238,113.324813727539856997),
        (-4,6,-355266.49375588479017,453.21962500658213978),
        (-4,8,-1.4210651255656059429e6,1812.85859691362667795),
        (-4,10,-5.6842602898977656296e6,7251.42941194449021767),
        (-2,-10,-3.8816759831446146058,3.8643112320515738011),
        (-2,-8,-3.9078768618012050169,3.8384181902996900102),
        (-2,-6,-4.0145163072573957704,3.736702907818645871),
        (-2,-4,-4.4698010809018832558,3.3598924598223454809),
        (-2,-2,-6.6905671916989966714,2.3220595020978699748),
        (-2,0,-18.4820547057437791,1.988323326380394479438),
        (-2,2,-70.783953097870321021,5.9613108809781128727),
        (-2,4,-282.30936462299129125,23.3659529228455572651),
        (-2,6,-1129.030146449362533,93.344363329525836638),
        (-2,8,-4516.068746770666273,373.34759716189133416),
        (-2,10,-18064.262027153766113,1493.38292470131924549),
        (0,-10,-1.0002788452304109558,0.99972142694646949398),
        (0,-8,-1.0011170138624497687,0.99888734096333377345),
        (0,-6,-1.0044941762813220056,0.9955754998295152863),
        (0,-4,-1.018394136694690131,0.98272039919080443611),
        (0,-2,-1.0801974743160773945,0.93756353315748672311),
        (0,0,-1.417689652557437386,0.85027067581518000307),
        (0,2,-3.4810036972847196216,1.2510662206451750368),
        (0,4,-13.077826987057486033,4.2056375402911356903),
        (0,6,-52.088252645503486311,16.614212555775078635),
        (0,8,-208.29704646832284508,66.404620139923864769),
        (0,10,-833.1741916924752671,265.60542075228219557),
        (2,-10,-0.43499430326325320721,0.43496458755568299844),
        (2,-8,-0.43503892727529063729,0.43492006444521354884),
        (2,-6,-0.43521802872563625451,0.43474257741836763541),
        (2,-4,-0.43594412045403539326,0.43404231605945274339),
        (2,-2,-0.43900339131704656106,0.43139622709382285703),
        (2,0,-0.45370470438033907791,0.42327940941897947662),
        (2,2,-0.5489276301806886869,0.427398434425988831988),
        (2,4,-1.2293782345935824227,0.7454801911947143025),
        (2,6,-4.539042916238930979,2.6061591734018259254),
        (2,8,-18.056159156415827597,10.325467890712821302),
        (2,10,-72.19953922944628574,41.276988339061446865),
        (4,-10,-0.21041425388076058064,0.21041246803239084151),
        (4,-8,-0.21041693413712247362,0.21040979074364370907),
        (4,-6,-0.21042767296825113797,0.21039909939434836776),
        (4,-4,-0.21047091318301441612,0.21035661888818976421),
        (4,-2,-0.21064843217568023864,0.21019125504671001397),
        (4,0,-0.2114314136108823888,0.20960270831287959738),
        (4,2,-0.21572378759893068761,0.208409169186959225894),
        (4,4,-0.25009888283657085475,0.22085079621327147519),
        (4,6,-0.5307333801713230348,0.413875782172491301),
        (4,8,-1.9380556731140923733,1.470790772299404392),
        (4,10,-7.703318782733698564,5.834310816724195935),
        (6,-10,-0.10435179723648817179,0.10435168670506776058),
        (6,-8,-0.10435196307928384176,0.10435152095360219713),
        (6,-6,-0.10435262699844715216,0.10435085849572058551),
        (6,-4,-0.10435529144278951622,0.10434821743188401059),
        (6,-2,-0.1043660895029916292,0.1043377934594183088),
        (6,0,-0.1044115262230904038,0.104298342051913869376),
        (6,2,-0.10462917321458367775,0.104176436729155382597),
        (6,4,-0.1060712415716721585,0.10426030818816446998),
        (6,6,-0.12031839024559590623,0.11307530023243388659),
        (6,8,-0.2480440443533549167,0.2190800455916883335),
        (6,10,-0.9002296095217339639,0.7843838997611279043),
        (8,-10,-0.052069908863436885579,0.052069901972042452258),
        (8,-8,-0.052069919201950109254,0.052069891636372375971),
        (8,-6,-0.052069960573061888275,0.052069850310750955153),
        (8,-4,-0.052070126330451151857,0.052069685281207420113),
        (8,-2,-0.052070793727082208168,0.052069029530107328641),
        (8,0,-0.052073533186702173452,0.052066476398805692192),
        (8,2,-0.052085608972327145052,0.052057381820935566284),
        (8,4,-0.052151793554755942862,0.052038884961615699691),
        (8,6,-0.0527011844052549625,0.052249550815775245095),
        (8,8,-0.05912267307084593025,0.05731617884437130046),
        (8,10,-0.1200725925262831369,0.1128471372590141824),
        (10,-10,-0.026021735227835241603,0.026021734797385426188),
        (10,-8,-0.026021735873554346331,0.026021734151755084671),
        (10,-6,-0.026021738456963344518,0.026021731569766297877),
        (10,-4,-0.026021748799120605618,0.026021721250332419055),
        (10,-2,-0.026021790304089942993,0.026021680108937196784),
        (10,0,-0.026021958505411808938,0.026021517724800827067),
        (10,2,-0.026022666213767941927,0.026020903091324204136),
        (10,4,-0.026026055485144916312,0.02601900299538210471),
        (10,6,-0.026048544726721264569,0.026020334768446194723),
        (10,8,-0.02628069207522740289,0.026167852291040381946),
        (10,10,-0.02931930010478171906,0.02886794347486872248)
      };

    [Fact]
    public void TestHwhmTable()
    {
      foreach (var (mexp, vexp, expectedleft, expectedright) in _hwhmData)
      {
        double m = RMath.Pow(2, mexp);
        double v = RMath.Pow(2, vexp);

        //if (Math.Abs(mexp) > 8 || Math.Abs(vexp) > 8)
        //continue;

        var left = PearsonIVAmplitude.GetHWHM(1, m, v, false);
        var right = PearsonIVAmplitude.GetHWHM(1, m, v, true);

        AssertEx.AreEqual(-expectedleft, left, 1E-8, 1E-8);
        AssertEx.AreEqual(expectedright, right, 1E-8, 1E-8);

        var fwhm = PearsonIVAmplitude.GetFWHM(1, m, v);
        var expectedfwhm = expectedright - expectedleft;

        AssertEx.AreEqual(expectedfwhm, fwhm, 1E-8, 1E-8);


        // but has left and right really the half amplitude?
        // we do make this test only for fwhm values < 1E100
        // and m>1/2
        if (Math.Abs(fwhm) < 1E100)
        {
          var ymax = PearsonIVAmplitude.GetYOfOneTerm(0, 1, 0, 1, m, v);
          var yleft = PearsonIVAmplitude.GetYOfOneTerm(-left, 1, 0, 1, m, v);
          var yright = PearsonIVAmplitude.GetYOfOneTerm(right, 1, 0, 1, m, v);
          AssertEx.AreEqual(0.5, yleft / ymax, 0, 1E-7);
          AssertEx.AreEqual(0.5, yright / ymax, 0, 1E-7);
        }

      }
    }

    [Fact]
    public void TestFwhmApproximation()
    {
      const double claimedMaxApproximationError = 0.19; // 19% error
      double w = 7;

      for (int idx_v = -1; idx_v <= 150; ++idx_v)
      {
        double v = idx_v < 0 ? 0 : Math.Pow(10, (idx_v - 75) / 25.0);

        for (int idx_m = 0; idx_m <= 150; idx_m++)
        {
          double m = Math.Pow(10, (idx_m - 75) / 25.0);
          var fwhmP = PearsonIVAmplitude.GetFWHM(w, m, v);
          var fwhmN = PearsonIVAmplitude.GetFWHM(w, m, -v);

          var fwhmApproxP = PearsonIVAmplitude.GetFWHMApproximation(w, m, v);
          var fwhmApproxN = PearsonIVAmplitude.GetFWHMApproximation(w, m, -v);

          // FWHM should be independent of whether v is positive or negative
          AssertEx.AreEqual(fwhmP, fwhmN, 1E-8, 1E-8);

          AssertEx.AreEqual(fwhmP, fwhmApproxP, 0, claimedMaxApproximationError);
          AssertEx.AreEqual(fwhmN, fwhmApproxN, 0, claimedMaxApproximationError);
        }
      }
    }

    [Fact]
    public void TestDerivativesWrtParameters()
    {
      var ff = new PearsonIVAmplitude();

      // General case
      double amplitude = 17;
      double position = 7;
      double w = 3;
      double m = 5;
      double v = 7;

      double expectedFunctionValue = 2.1808325179027502207687005375167;
      double expectedDerivativeWrtAmplitude = 0.12828426575898530710404120808922;
      double expectedDerivativeWrtPosition = 4.8409156890183134756241965316685;
      double expectedDerivativeWrtW = 3.2272771260122089837494643544457;
      double expectedDerivativeWrtM = -1.1659424616671277074214895658281;
      double expectedDerivativeWrtV = 0.1930511753781758455580674695759;

      var parameters = new double[] { amplitude, position, w, m, v };

      var X = Matrix<double>.Build.Dense(1, 1);
      X[0, 0] = 9;

      var FV = Vector<double>.Build.Dense(1);
      var DY = Matrix<double>.Build.Dense(1, 5);

      ff.Evaluate(X, parameters, FV, null);
      ff.EvaluateDerivative(X, parameters, null, DY, null);

      AssertEx.AreEqual(expectedFunctionValue, FV[0], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtAmplitude, DY[0, 0], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtPosition, DY[0, 1], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtW, DY[0, 2], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtM, DY[0, 3], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtV, DY[0, 4], 0, 1E-12);
    }
  }
}
