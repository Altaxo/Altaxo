#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
using Altaxo.Calc.Interpolation;
using Altaxo.Calc.LinearAlgebra;
using NUnit.Framework;

namespace AltaxoTest.Calc.Interpolation
{
  /// <summary>
  /// Test for the cross validated cubic spline.
  /// </summary>
  /// <remarks> The test data originates from the output of Algorithm 642, which was translated by f2c into C and feeded with the test data below.
  /// Note that I replaced the tau which has only few decimal places in original 642.f with the more accurate tau=1.6180339887498948482045868343656.
  /// </remarks>
  [TestFixture]
  public class TestCrossValidatedCubicSpline
  {
    private double[][] _testvals = {
new double[]{0.00843301352350958, -0.152243837374951},
new double[]{0.025019470131189, -0.119734055655511},
new double[]{0.0501815571171146, 0.461729355636735},
new double[]{0.0694124844388163, 0.0851220476980274},
new double[]{0.0892763556691863, 0.180380168471979},
new double[]{0.105400639177331, 0.389756960468003},
new double[]{0.13304193020319, 0.757836858329517},
new double[]{0.144587330384143, 0.704785058099651},
new double[]{0.166677919882355, 0.793994884037036},
new double[]{0.192998934131891, 1.01678524670068},
new double[]{0.214620181867521, 0.906192511335025},
new double[]{0.228451359764883, 0.665808010311522},
new double[]{0.251834099563693, 1.04051259747373},
new double[]{0.268998843539661, 0.742630635194807},
new double[]{0.290879450890956, 1.1810765143109},
new double[]{0.30715736285651, 1.02738155522738},
new double[]{0.333202886269367, 1.0417072436816},
new double[]{0.349254425425356, 0.721461550776648},
new double[]{0.367263622515492, 0.830680249332228},
new double[]{0.395793876672378, 1.17334020997532},
new double[]{0.413910668584166, 1.12441984632209},
new double[]{0.43034162488161, 0.920228278925585},
new double[]{0.450030182820448, 1.14215552834455},
new double[]{0.466854705948781, 0.788935908970938},
new double[]{0.496360513884432, 0.542289221010453},
new double[]{0.503539552750783, 0.488133260181762},
new double[]{0.523896284687285, 0.907649946408259},
new double[]{0.556218648496807, 0.668149586712674},
new double[]{0.573949241518951, 0.713550473948648},
new double[]{0.589557848376947, 0.550237275721738},
new double[]{0.609388991802025, 0.151980110647414},
new double[]{0.630538900131611, 0.235846260250003},
new double[]{0.654442123906272, 0.115629584585516},
new double[]{0.671535796514496, -0.227757721400102},
new double[]{0.687882554148269, -0.367649455893867},
new double[]{0.707735100750688, -0.485247286448983},
new double[]{0.732802827163973, -0.0880249669597303},
new double[]{0.744480400706865, -0.646258600064633},
new double[]{0.764294638162306, -0.236593751557764},
new double[]{0.791869410957149, -0.463411501001707},
new double[]{0.808997759337381, -0.347033662234902},
new double[]{0.82560029086452, -0.764451069370005},
new double[]{0.85426094111564, -0.994121364304904},
new double[]{0.870214382736112, -0.659071940610107},
new double[]{0.891374850047151, -0.776223930086174},
new double[]{0.906590191208101, -1.01820147789573},
new double[]{0.933629603868799, -1.18556910376756},
new double[]{0.948752005481511, -0.980323659089009},
new double[]{0.967142143027768, -0.739075684559176},
new double[]{0.992733618149565, -0.756103993847516}
                           };

    // expected y at the x given by testvals
    private double[] _refy = {
-0.131090586,
-0.0251559085,
0.130926503,
0.241280859,
0.356777998,
0.454968197,
0.619212637,
0.679389831,
0.774522519,
0.851581524,
0.887872243,
0.904753667,
0.93223235,
0.950458655,
0.970376279,
0.977149942,
0.976968636,
0.97713894,
0.984496535,
0.998469747,
0.990577501,
0.966552414,
0.919057324,
0.865723266,
0.772068453,
0.753289447,
0.706807534,
0.611634304,
0.533778246,
0.44654683,
0.316797851,
0.16901967,
0.000604496579,
-0.112043033,
-0.203892394,
-0.288035304,
-0.359437859,
-0.387221126,
-0.432846144,
-0.513327949,
-0.577719146,
-0.648831254,
-0.767658077,
-0.82295138,
-0.884530269,
-0.917669387,
-0.938292074,
-0.924731089,
-0.890753047,
-0.831558495
};

    private double[][] _expectedCoefficients = {
new double[]{6.39641658,0,-34.8903219},
new double[]{6.3676205,-1.73612043,-190.888186},
new double[]{5.9176802,-16.1455559,354.740377},
new double[]{5.69027155,4.32040334,97.1709305},
new double[]{5.97693461,10.1109759,-193.780896},
new double[]{6.15185435,0.737241562,-301.340771},
new double[]{5.50190088,-24.2511023,-72.6930009},
new double[]{4.9128544,-26.7689117,-30.8059346},
new double[]{3.68507297,-28.8104754,1.3119224},
new double[]{2.17115779,-28.7068821,273.80011},
new double[]{1.31378744,-10.947182,304.01769},
new double[]{1.18543971,1.66758626,-90.1009651},
new double[]{1.11563639,-4.65283601,88.4972403},
new double[]{1.03412832,-0.0957385926,-254.295765},
new double[]{0.664698015,-16.7881759,93.234692},
new double[]{0.192258422,-12.2351776,176.08707},
new double[]{-0.0867286052,1.52366214,282.867388},
new double[]{0.180829608,15.145033,-138.848704},
new double[]{0.591230485,7.64337198,-392.554373},
new double[]{0.0687763651,-25.9556561,-104.121795},
new double[]{-0.974214063,-31.6147147,116.638715},
new double[]{-1.91866498,-25.8652578,40.2313599},
new double[]{-2.89037847,-23.4889655,408.211114},
new double[]{-3.33410943,-2.88509346,281.557504},
new double[]{-2.76899773,22.0376515,-97.4419844},
new double[]{-2.46764549,19.9390321,-534.792472},
new double[]{-2.32070687,-12.7208489,-203.521578},
new double[]{-3.78092116,-32.4557444,-110.304871},
new double[]{-5.03587127,-38.3230567,186.212712},
new double[]{-6.09611009,-29.6034937,357.240466},
new double[]{-6.84877192,-8.35003294,85.3889202},
new double[]{-7.08738862,-2.93212943,195.61317},
new double[]{-6.89226428,11.0952267,385.336363},
new double[]{-6.17516943,30.8556676,194.475738},
new double[]{-5.01048782,40.3928108,-75.6263177},
new double[]{-3.49610586,35.8886858,-400.909155},
new double[]{-2.45259334,5.73904276,46.761187},
new double[]{-2.29942729,7.37721436,-380.497053},
new double[]{-2.45523354,-15.2405625,-56.796971},
new double[]{-3.42530345,-19.9390533,25.5355708},
new double[]{-4.08587465,-18.6269068,406.029838},
new double[]{-4.36862313,1.59646277,215.325698},
new double[]{-3.74648499,20.1105863,-158.204448},
new double[]{-3.22561379,12.53887,112.099457},
new double[]{-2.54437433,19.6551007,290.740699},
new double[]{-1.74433102,32.9262575,124.922357},
new double[]{0.310285693,43.059739,-282.938123},
new double[]{1.41850619,30.2236272,-374.632901},
new double[]{2.15004016,9.55497549,-124.455187}
                                       };

    private double[] _expectedErrorEstimates = {
0.132517647,
0.102039213,
0.0846107055,
0.0820962986,
0.0816778104,
0.0816300186,
0.0811463571,
0.0811504826,
0.0819473088,
0.0817198469,
0.0803674577,
0.0798173555,
0.0796064479,
0.0796853309,
0.0800043072,
0.0803659288,
0.0806063281,
0.0807355927,
0.0813185798,
0.0808448937,
0.0796054088,
0.0789691485,
0.0790163562,
0.0794348711,
0.0796531177,
0.0798887128,
0.0815401371,
0.0816898407,
0.080333695,
0.0797964693,
0.0800988647,
0.080451246,
0.0797499759,
0.079040064,
0.0789288798,
0.0792479114,
0.0792564641,
0.0794847774,
0.0805881605,
0.0811427318,
0.0810255812,
0.081391672,
0.0814648554,
0.0811561804,
0.0812694977,
0.0816238406,
0.0819776354,
0.0841143745,
0.095333084,
0.139785028
                                       };

    private double[] _expectedFitVars = {
0.9614375,
39.6239287,
0.0433056129,
0.0271968902,
0.00712188019,
0.0343187704,
1
  };

    private void AreEqual(double expected, double current, double reldev, double absdev, string msg)
    {
      double min, max;
      bool passes = false;
      if (expected == 0)
      {
        min = -Math.Abs(absdev);
        max = Math.Abs(absdev);
        passes = current >= min && current <= max;
      }
      else
      {
        double dev = Math.Abs(expected * reldev);
        min = expected - dev;
        max = expected + dev;
        passes = current >= min && current <= max;
      }

      if (!passes)
        Assert.Fail("Value {0} is not in the interval [{1},{2}], ({3})", current, min, max, msg);
    }

    [Test]
    public void Test1()
    {
      int len = _testvals.Length;

      double[] x = new double[len];
      double[] y = new double[len];
      double[] dy = new double[len];

      for (int i = 0; i < len; i++)
      {
        x[i] = _testvals[i][0];
        y[i] = _testvals[i][1];
        dy[i] = 1;
      }

      var spline = new CrossValidatedCubicSpline
      {
        CalculateErrorEstimates = true
      };

      spline.SetErrorVariance(VectorMath.ToROVector(dy), -1);
      spline.Interpolate(VectorMath.ToROVector(x), VectorMath.ToROVector(y));

      // Test the values y at the points x[i] - this are the coefficients of 0th order
      for (int i = 0; i < len; i++)
      {
        AreEqual(_refy[i], spline.Coefficient0[i], 1e-7, 0, "Coeff0[" + i.ToString() + "]");
      }

      // test the higher order coefficients
      for (int i = 0; i < len - 1; i++)
      {
        AreEqual(_expectedCoefficients[i][0], spline.Coefficient1[i], 1e-7, 0, "Coeff1[" + i.ToString() + "]");
        AreEqual(_expectedCoefficients[i][1], spline.Coefficient2[i], 1e-7, 0, "Coeff2[" + i.ToString() + "]");
        AreEqual(_expectedCoefficients[i][2], spline.Coefficient3[i], 1e-7, 0, "Coeff3[" + i.ToString() + "]");
      }

      // test the standard error estimates
      for (int i = 0; i < len; i++)
      {
        AreEqual(_expectedErrorEstimates[i], spline.ErrorEstimate[i], 1e-7, 0, "Error[" + i.ToString() + "]");
      }

      AreEqual(_expectedFitVars[0], spline.SmoothingParameter, 1e-7, 0, "SmoothingParameter");
      AreEqual(_expectedFitVars[1], spline.EstimatedDegreesOfFreedom, 1e-7, 0, "EstimatedDegreesofFreedom");
      AreEqual(_expectedFitVars[2], spline.GeneralizedCrossValidation, 1e-7, 0, "GeneralizedCrossValidation");
      AreEqual(_expectedFitVars[3], spline.MeanSquareResidual, 1e-7, 0, "MeanSquareResidual");
      AreEqual(_expectedFitVars[4], spline.EstimatedTrueMeanSquareError, 1e-7, 0, "EstimatedTrueMeanSquareError");
      AreEqual(_expectedFitVars[5], spline.EstimatedErrorVariance, 1e-7, 0, "EstimatedErrorVariance");
      AreEqual(_expectedFitVars[6], spline.MeanSquareOfInputVariance, 1e-7, 0, "MeanSquareOfInputVariance");
    }
  }
}
