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
  public class ShiftedLogNormalTest
  {
    /// <summary>
    /// Tests the basics of function values, like that the function value at x=pos should be = amp, and that 3 w away to each side
    /// the value should be less than 1/2 of the amp.
    /// </summary>
    [Fact]
    public void TestFunctionValuesBasics()
    {
      var testData = new (double amp, double pos, double w, double rho)[]
      {
        (3,7,5,11),
        (3,7,1/5d,11),
        (3,7,5,1+1E-14),
        (3,7,1/5d,1+1E-14),
        (3,7,5,1),
        (3,7,1/5d,1),
        (3,7,5,1-1E-14),
        (3,7,1/5d,1-1E-14),
        (3,7,5,1E-14),
        (3,7,1/5d,1E-14),
      };

      var func = new ShiftedLogNormal_ParametrizationNIST(1, -1);
      foreach (var (amp, pos, w, rho) in testData)
      {
        double[] pars = new double[] { amp, pos, w, rho };
        double[] X = new double[1];
        double[] Y = new double[1];
        double y;

        // Y at x=pos should be equal to amp
        X[0] = pos;
        y = ShiftedLogNormal_ParametrizationNIST.GetYOfOneTerm(X[0], amp, pos, w, rho);
        AssertEx.AreEqual(amp, y, 1e-10, 1e-10);
        func.Evaluate(X, pars, Y);
        AssertEx.AreEqual(y, Y[0], 1e-15, 1e-15);
        func.Evaluate(MatrixMath.ToROMatrixWithOneColumn(X), pars, VectorMath.ToVector(Y), null);
        AssertEx.AreEqual(y, Y[0], 1e-15, 1e-15);

        // Y at x= pos - w /1000 should be less than amp
        X[0] = pos - w / 1000;
        y = ShiftedLogNormal_ParametrizationNIST.GetYOfOneTerm(X[0], amp, pos, w, rho);
        AssertEx.Greater(amp, y);
        func.Evaluate(X, pars, Y);
        AssertEx.AreEqual(y, Y[0], 1e-15, 1e-15);
        func.Evaluate(MatrixMath.ToROMatrixWithOneColumn(X), pars, VectorMath.ToVector(Y), null);
        AssertEx.AreEqual(y, Y[0], 1e-15, 1e-15);

        // Y at x= pos + w /1000 should be less than amp
        X[0] = pos + w / 1000;
        y = ShiftedLogNormal_ParametrizationNIST.GetYOfOneTerm(X[0], amp, pos, w, rho);
        AssertEx.Greater(amp, y);
        func.Evaluate(X, pars, Y);
        AssertEx.AreEqual(y, Y[0], 1e-15, 1e-15);
        func.Evaluate(MatrixMath.ToROMatrixWithOneColumn(X), pars, VectorMath.ToVector(Y), null);
        AssertEx.AreEqual(y, Y[0], 1e-15, 1e-15);

        // Y at x= pos - 1.1 w should be less than amp/2
        X[0] = pos - 1.1 * w;
        y = ShiftedLogNormal_ParametrizationNIST.GetYOfOneTerm(X[0], amp, pos, w, rho);
        AssertEx.Greater(amp / 2, y);
        func.Evaluate(X, pars, Y);
        AssertEx.AreEqual(y, Y[0], 1e-15, 1e-15);
        func.Evaluate(MatrixMath.ToROMatrixWithOneColumn(X), pars, VectorMath.ToVector(Y), null);
        AssertEx.AreEqual(y, Y[0], 1e-15, 1e-15);

        // Y at x= pos + 1.1 w should be less than amp/2
        X[0] = pos + 1.1 * w;
        y = ShiftedLogNormal_ParametrizationNIST.GetYOfOneTerm(X[0], amp, pos, w, rho);
        AssertEx.Greater(amp / 2, y);
        func.Evaluate(X, pars, Y);
        AssertEx.AreEqual(y, Y[0], 1e-15, 1e-15);
        func.Evaluate(MatrixMath.ToROMatrixWithOneColumn(X), pars, VectorMath.ToVector(Y), null);
        AssertEx.AreEqual(y, Y[0], 1e-15, 1e-15);
      }
    }

    [Fact]
    public void TestFunctionValuesInDependenceOnX()
    {
      var testData = new (double x, double amp, double pos, double w, double rho, double y)[]
      {
        (-1, 3, 5, 7, 1/1024d, 1.5460911731844943666),
        (5+1/256d, 3, 5, 7, 1/1024d, 2.9690885343525164625),
        (6, 3, 5, 7, 1/1024d, 0), // normally, this is either undefined or complex
        // rho close to 1, but smaller
        (-1, 3, 5, 7, 1-1/1073741824d, 0.39125660038507084345),
        (11, 3, 5, 7, 1-1/1073741824d, 0.39125659784019335105),
        // rho equal to 1
        (-1, 3, 5, 7, 1, 0.39125659911263209727),
        (11, 3, 5, 7, 1, 0.39125659911263209727),
        // rho close to 1, but greater
        (-1, 3, 5, 7, 1+1/1073741824d, 0.39125659784019335224),
        (11, 3, 5, 7, 1+1/1073741824d, 0.39125659784019335105),
        // rho greater than 1
        (-1, 3, 5, 7, 2, 0), // normally, either undefined or complex
        (1, 3, 5, 7, 2, 0.012724218332112091886),
        (11, 3, 5, 7, 2, 1.1192716455868243319),
      };

      var func = new ShiftedLogNormal_ParametrizationNIST(1, -1);
      foreach (var (x, amp, pos, w, rho, yexpected) in testData)
      {
        double[] pars = new double[] { amp, pos, w, rho };
        double[] X = new double[1];
        double[] Y = new double[1];
        double y;

        X[0] = x;
        y = ShiftedLogNormal_ParametrizationNIST.GetYOfOneTerm(X[0], amp, pos, w, rho);

        double accuracy = 1e-13;
        if (rho != 1 && Math.Abs(rho - 1) < 1E-5)
        {
          accuracy = 1e-7;
        }

        AssertEx.AreEqual(yexpected, y, 0, accuracy);
        func.Evaluate(X, pars, Y);
        AssertEx.AreEqual(y, Y[0], 1e-15, accuracy);
        func.Evaluate(MatrixMath.ToROMatrixWithOneColumn(X), pars, VectorMath.ToVector(Y), null);
        AssertEx.AreEqual(y, Y[0], 1e-15, accuracy);
      }
    }

    [Fact]
    public void TestDerivativesWrtParameters()
    {
      var ff = new ShiftedLogNormal_ParametrizationNIST(1, -1);

      var testData = new (double x, double amp, double pos, double w, double rho, double dydh, double dydx0, double dydw, double dydrho)[]
      {
        (-1, 3, 5, 7, 1/1024d, 0.51536372439483145552,-0.050341346575493912623, 0.043149725636137639391, 6.4837476785032961204),
        (5, 3, 5, 7, 1/1024d, 1, 0, 0, 0),
        (5+1/256d, 3, 5, 7, 1/1024d, 0.98969617811750548751, 24.776552445883675402,0.013826201141676158148,90.020391249908595764),

        (-1, 3, 5, 7, 1-1/1024d, 0.13086382411098995226, -0.26590185324786706100, 0.22791587421245748086, -1.3675485206370591715),
        (5, 3, 5, 7, 1-1/1024d, 1,0,0,0),
        (6, 3, 5, 7, 1-1/1024d, 0.94497268102785436079, 0.32095333364897360741,0.045850476235567658201, 0.046004204596586105133),

        (-1, 3, 5, 7, 1, 0.13041886637087736576, -0.26566374715140611075, 0.22771178327263380921, -1.3662706996358028553),
        (5, 3, 5, 7, 1, 1, 0, 0, 0),
        (6, 3, 5, 7, 1, 0.94498762830860671637, 0.32082392337606872087, 0.045831989053724102981, 0.045831989053724102981),

        (-1, 3, 5, 7, 1+1/1024d, 0.12997432569699114518, -0.26542418807913193474, 0.22750644692497022978, -1.3649860642345467271),
        (5, 3, 5, 7, 1+1/1024d, 1, 0, 0, 0),
        (6, 3, 5, 7, 1+1/1024d, 0.94500251962780975638, 0.32069491654608704898,0.045813559506583864140, 0.045660376377965771003),

        (1, 3, 5, 7, 2, 0.0042414061107040306286, -0.10716419089197143715, 0.061236680509697964088, -0.25693133005027256320),
        (5, 3, 5, 7, 2, 1, 0, 0, 0),
        (6, 3, 5, 7, 2, 0.94706775894085793551, 0.28088595454388902920, 0.040126564934841289886, -0.011149551389604555276),

      };

      foreach (var tuple in testData)
      {
        var parameters = new double[] { tuple.amp, tuple.pos, tuple.w, tuple.rho };

        var X = Matrix<double>.Build.Dense(1, 1);
        X[0, 0] = tuple.x;

        var DY = Matrix<double>.Build.Dense(1, 4);

        ff.EvaluateDerivative(X, parameters, null, DY, null);

        var expectedAccuracy = (tuple.rho != 1 && Math.Abs(tuple.rho - 1) < 1E-3) ? 1E-8 : 1E-12;

        AssertEx.AreEqual(tuple.dydh, DY[0, 0], 0, expectedAccuracy);
        AssertEx.AreEqual(tuple.dydx0, DY[0, 1], 0, expectedAccuracy);
        AssertEx.AreEqual(tuple.dydw, DY[0, 2], 0, expectedAccuracy);
        AssertEx.AreEqual(tuple.dydrho, DY[0, 3], 0, expectedAccuracy);
      }
    }

    [Fact]
    public void TestAreaFwhm()
    {
      var testData = new (double amp, double pos, double w, double rho, double area)[]
      {
        (3, 5, 7, 1/1024d, 1.0154471166033409828e7),
        // rho close to 1, but smaller
        (3, 5, 7, 1-1/1024d, 22.35381154799093225),
        // rho equal to 1
        (3, 5, 7, 1, 22.353807408055749766),
        // rho close to 1, but greater
        (3, 5, 7, 1+1/1024d, 22.353811539913009617),
        // rho greater than 1
        (3, 5, 7, 2, 24.568192226940549851),
        (3, 5, 7, 1024, 1.0154471166033409828E7),
      };

      var func = new ShiftedLogNormal_ParametrizationNIST(1, -1);

      for (int i = 0; i < testData.Length; i++)
      {
        var (amp, pos, w, rho, expectedArea) = testData[i];
        double[] pars = new double[] { amp, pos, w, rho };
        double[] X = new double[1];
        double[] Y = new double[1];
        double y;

        var (position, area, height, fwhm) = func.GetPositionAreaHeightFWHMFromSinglePeakParameters(pars);

        AssertEx.AreEqual(pos, position, 0, 1E-14);
        AssertEx.AreEqual(amp, height, 0, 1E-14);
        AssertEx.AreEqual(expectedArea, area, 0, 1E-8);
        AssertEx.AreEqual(w, fwhm, 0, 1E-14);
      }

      {
        // with the first test set, we test additionally
        var (amp, pos, w, rho, expectedARea) = testData[0];
        double[] pars = new double[] { amp, pos, w, rho };
        var (position, area, height, fwhm) = func.GetPositionAreaHeightFWHMFromSinglePeakParameters(pars);

        // (i) when we vary the amplitude, area and height should change accordingly, pos and fwhm stay constant
        foreach (var newAmp in new double[] { -11, 0, 2, 17, 33, 1023, -11 })
        {
          pars = new double[] { newAmp, pos, w, rho };
          var (newposition, newarea, newheight, newfwhm) = func.GetPositionAreaHeightFWHMFromSinglePeakParameters(pars);
          AssertEx.AreEqual(position, newposition, 0, 1E-14);
          AssertEx.AreEqual(newAmp, newheight, 0, 1E-14);
          AssertEx.AreEqual(area * newAmp / height, newarea, 0, 1E-14);
          AssertEx.AreEqual(fwhm, newfwhm, 0, 1E-14);
        }

        // (ii) when we vary the position, position should change accordingly, everything else is the same
        foreach (var newPos in new double[] { -11, 0, 2, 17, 33, 1023, -11 })
        {
          pars = new double[] { amp, newPos, w, rho };
          var (newposition, newarea, newheight, newfwhm) = func.GetPositionAreaHeightFWHMFromSinglePeakParameters(pars);
          AssertEx.AreEqual(newPos, newposition, 0, 1E-14);
          AssertEx.AreEqual(height, newheight, 0, 1E-14);
          AssertEx.AreEqual(area, newarea, 0, 1E-14);
          AssertEx.AreEqual(fwhm, newfwhm, 0, 1E-14);
        }

        // (iii) when we vary the width, area and fwhm should change accordingly, everything else is the same
        foreach (var newW in new double[] { 1 / 1000d, 2, 17, 33, 1023 })
        {
          pars = new double[] { amp, pos, newW, rho };
          var (newposition, newarea, newheight, newfwhm) = func.GetPositionAreaHeightFWHMFromSinglePeakParameters(pars);
          AssertEx.AreEqual(position, newposition, 0, 1E-14);
          AssertEx.AreEqual(height, newheight, 0, 1E-14);
          AssertEx.AreEqual(area * (newW / w), newarea, 0, 1E-14);
          AssertEx.AreEqual(fwhm * (newW / w), newfwhm, 0, 1E-14);
        }
      }
    }

    [Fact]
    public void TestDerivativesOfAreaFwhm_RhoLessThan1()
    {
      // see PearsonIV (amplitudeModified).nb
      double amp = 3;
      double pos = 5;
      double w = 7;
      double rho = 1 / 1024d;

      var ymaxDerivs = new double[] { 1, 0, 0, 0 };
      var xmaxDerivs = new double[] { 0, 1, 0, 0 };
      var fwhmDerivs = new double[] { 0, 0, 1, 0 };
      var areaDerivs = new double[] { 3.3848237220111366095E6, 0, 1.4506387380047728326E6, -4.3092834114976880604E10 };

      double[] pars = new double[] { amp, pos, w, rho };
      double[] X = new double[1];
      double[] Y = new double[1];

      var func = new ShiftedLogNormal_ParametrizationNIST();



      for (int i = 0; i < pars.Length; i++)
      {
        var cov = CreateMatrix.Dense<double>(pars.Length, pars.Length);
        cov[i, i] = 1;
        var result = func.GetPositionAreaHeightFWHMFromSinglePeakParameters(pars, cov);
        AssertEx.AreEqual(Math.Abs(ymaxDerivs[i]), result.HeightStdDev, 1E-13, 1E-7);
        AssertEx.AreEqual(Math.Abs(xmaxDerivs[i]), result.PositionStdDev, 1E-13, 1E-7);
        AssertEx.AreEqual(Math.Abs(areaDerivs[i]), result.AreaStdDev, 1E-13, 1E-4);
        AssertEx.AreEqual(Math.Abs(fwhmDerivs[i]), result.FWHMStdDev, 1E-13, 1E-4);
      }
    }

    [Fact]
    public void TestDerivativesOfAreaFwhm_Rho1()
    {
      // see PearsonIV (amplitudeModified).nb
      double amp = 3;
      double pos = 5;
      double w = 7;
      double rho = 1;

      var ymaxDerivs = new double[] { 1, 0, 0, 0 };
      var xmaxDerivs = new double[] { 0, 1, 0, 0 };
      var fwhmDerivs = new double[] { 0, 0, 1, 0 };
      var areaDerivs = new double[] { 7.4512691360185832552, 0, 3.1934010582936785379, 0 };

      double[] pars = new double[] { amp, pos, w, rho };
      double[] X = new double[1];
      double[] Y = new double[1];

      var func = new ShiftedLogNormal_ParametrizationNIST();



      for (int i = 0; i < pars.Length; i++)
      {
        var cov = CreateMatrix.Dense<double>(pars.Length, pars.Length);
        cov[i, i] = 1;
        var result = func.GetPositionAreaHeightFWHMFromSinglePeakParameters(pars, cov);
        AssertEx.AreEqual(Math.Abs(ymaxDerivs[i]), result.HeightStdDev, 1E-13, 1E-7);
        AssertEx.AreEqual(Math.Abs(xmaxDerivs[i]), result.PositionStdDev, 1E-13, 1E-7);
        AssertEx.AreEqual(Math.Abs(areaDerivs[i]), result.AreaStdDev, 1E-13, 1E-4);
        AssertEx.AreEqual(Math.Abs(fwhmDerivs[i]), result.FWHMStdDev, 1E-13, 1E-4);
      }
    }

    [Fact]
    public void TestDerivativesOfAreaFwhm_RhoGreaterThan1()
    {
      // see PearsonIV (amplitudeModified).nb
      double amp = 3;
      double pos = 5;
      double w = 7;
      double rho = 2;

      var ymaxDerivs = new double[] { 1, 0, 0, 0 };
      var xmaxDerivs = new double[] { 0, 1, 0, 0 };
      var fwhmDerivs = new double[] { 0, 0, 1, 0 };
      var areaDerivs = new double[] { 8.1893974089801832835, 0, 3.5097417467057928358, 3.3907590789916340133 };

      double[] pars = new double[] { amp, pos, w, rho };
      double[] X = new double[1];
      double[] Y = new double[1];

      var func = new ShiftedLogNormal_ParametrizationNIST();



      for (int i = 0; i < pars.Length; i++)
      {
        var cov = CreateMatrix.Dense<double>(pars.Length, pars.Length);
        cov[i, i] = 1;
        var result = func.GetPositionAreaHeightFWHMFromSinglePeakParameters(pars, cov);
        AssertEx.AreEqual(Math.Abs(ymaxDerivs[i]), result.HeightStdDev, 1E-13, 1E-7);
        AssertEx.AreEqual(Math.Abs(xmaxDerivs[i]), result.PositionStdDev, 1E-13, 1E-7);
        AssertEx.AreEqual(Math.Abs(areaDerivs[i]), result.AreaStdDev, 1E-13, 1E-4);
        AssertEx.AreEqual(Math.Abs(fwhmDerivs[i]), result.FWHMStdDev, 1E-13, 1E-4);
      }
    }

  }
}
