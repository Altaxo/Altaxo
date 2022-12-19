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
  public class PearsonIVAmplitudeParametrizationHPWTests
  {
    /// <summary>
    /// Tests the basics of function values, like that the function value at x=pos should be = amp, and that 3 w away to each side
    /// the value should be less than 1/2 of the amp.
    /// </summary>
    [Fact]
    public void TestFunctionValuesBasics()
    {
      var testData = new (double amp, double pos, double w, double m, double v)[]
      {
        (3,7,5,11,13),
        (3,7,1/5d,1/1000d,1000),
        (3,7,1/5d,1/1000d,-1000),
        (3,7,1/5d,1/1000d,0),
        (3,7,1/5d,1000d,1000),
        (3,7,1/5d,1000d,-1000),
        (3,7,1/5d,1000d,0),
      };

      var func = new PearsonIVAmplitudeParametrizationHPW(1, -1);
      foreach (var (amp, pos, w, m, v) in testData)
      {
        double[] pars = new double[] { amp, pos, w, m, v };
        double[] X = new double[1];
        double[] Y = new double[1];
        double y;

        // Y at x=pos should be equal to amp
        X[0] = pos;
        y = PearsonIVAmplitudeParametrizationHPW.GetYOfOneTerm(X[0], amp, pos, w, m, v);
        AssertEx.AreEqual(amp, y, 1e-10, 1e-10);
        func.Evaluate(X, pars, Y);
        AssertEx.AreEqual(y, Y[0], 1e-15, 1e-15);
        func.Evaluate(MatrixMath.ToROMatrixWithOneColumn(X), pars, VectorMath.ToVector(Y), null);
        AssertEx.AreEqual(y, Y[0], 1e-15, 1e-15);

        // Y at x= pos - w /1000 should be less than amp
        X[0] = pos - w / 1000;
        y = PearsonIVAmplitudeParametrizationHPW.GetYOfOneTerm(X[0], amp, pos, w, m, v);
        AssertEx.Greater(amp, y);
        func.Evaluate(X, pars, Y);
        AssertEx.AreEqual(y, Y[0], 1e-15, 1e-15);
        func.Evaluate(MatrixMath.ToROMatrixWithOneColumn(X), pars, VectorMath.ToVector(Y), null);
        AssertEx.AreEqual(y, Y[0], 1e-15, 1e-15);

        // Y at x= pos + w /1000 should be less than amp
        X[0] = pos + w / 1000;
        y = PearsonIVAmplitudeParametrizationHPW.GetYOfOneTerm(X[0], amp, pos, w, m, v);
        AssertEx.Greater(amp, y);
        func.Evaluate(X, pars, Y);
        AssertEx.AreEqual(y, Y[0], 1e-15, 1e-15);
        func.Evaluate(MatrixMath.ToROMatrixWithOneColumn(X), pars, VectorMath.ToVector(Y), null);
        AssertEx.AreEqual(y, Y[0], 1e-15, 1e-15);

        // Y at x= pos -3 w should be less than amp/2
        X[0] = pos - 3 * w;
        y = PearsonIVAmplitudeParametrizationHPW.GetYOfOneTerm(X[0], amp, pos, w, m, v);
        AssertEx.Greater(amp / 2, y);
        func.Evaluate(X, pars, Y);
        AssertEx.AreEqual(y, Y[0], 1e-15, 1e-15);
        func.Evaluate(MatrixMath.ToROMatrixWithOneColumn(X), pars, VectorMath.ToVector(Y), null);
        AssertEx.AreEqual(y, Y[0], 1e-15, 1e-15);

        // Y at x= pos + 3 w should be less than amp/2
        X[0] = pos + 3 * w;
        y = PearsonIVAmplitudeParametrizationHPW.GetYOfOneTerm(X[0], amp, pos, w, m, v);
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
      var testData = new (double x, double amp, double pos, double w, double m, double v, double y)[]
      {
        (-1e9, 3, 7, 5, 11, 13, 2.7867315365921926531E-160),
        (-19, 3, 7, 5, 11, 13, 0.0071554075200805234436),
        (-1, 3, 7, 5, 11, 13, 0.94609302487197904495),
        (0, 3, 7, 5, 11, 13, 1.1845028216303614895),
        (6, 3, 7, 5, 11, 13, 2.9207184451185306856),
        (7, 3, 7, 5, 11, 13, 3.0000000000000000000),
        (8, 3, 7, 5, 11, 13, 2.9093699544835131623),
        (23, 3, 7, 5, 11, 13, 5.7924047779440271306E-25),
        (28, 3, 7, 5, 11, 13, 7.9007462868389145363E-260),
      };

      var func = new PearsonIVAmplitudeParametrizationHPW(1, -1);
      foreach (var (x, amp, pos, w, m, v, yexpected) in testData)
      {
        double[] pars = new double[] { amp, pos, w, m, v };
        double[] X = new double[1];
        double[] Y = new double[1];
        double y;

        X[0] = x;
        y = PearsonIVAmplitudeParametrizationHPW.GetYOfOneTerm(X[0], amp, pos, w, m, v);
        if (yexpected < 1E-100)
        {
          AssertEx.AreEqual(yexpected, y, 0, 1E-7);
        }
        else
        {
          AssertEx.AreEqual(yexpected, y, 0, 1E-13);
        }

        func.Evaluate(X, pars, Y);
        AssertEx.AreEqual(y, Y[0], 1e-15, 1e-15);
        func.Evaluate(MatrixMath.ToROMatrixWithOneColumn(X), pars, VectorMath.ToVector(Y), null);
        AssertEx.AreEqual(y, Y[0], 1e-15, 1e-15);
      }
    }


    [Fact]
    public void TestAreaFwhm()
    {
      var testData = new (double amp, double pos, double w, double m, double v)[]
      {
        (1, 7, 5, 11, 13),
        (3, 7, 5, 1000, -1000),
        (3, 7, 5, 1000, 0),
        (3, 7, 5, 1000, 1000),
        (3, 7, 5, 513/1024d, -1000),
        (3, 7, 5, 513/1024d, 0),
        (3, 7, 5, 513/1024d, 1000),
      };

      var expectedAreas = new double[] {
        11.017024491957193080,
        31.945780266160134464,
        31.940456418616702875,
        31.945780266160134464,
        12084.703859367018981,
        8896.1196272036681069,
        12084.703859367018981
      };

      var expectedFwhm = new double[]
        {
          10.069306246733044125,
          10.000770127169960848,
          10.000000000000000000,
          10.000770127169960848,
          11.364892411874591570,
          10.000000000000000000,
          11.364892411874591570};

      var func = new PearsonIVAmplitudeParametrizationHPW(1, -1);

      for (int i = 0; i < testData.Length; i++)
      {
        var (amp, pos, w, m, v) = testData[i];
        double[] pars = new double[] { amp, pos, w, m, v };
        double[] X = new double[1];
        double[] Y = new double[1];
        double y;

        var (position, area, height, fwhm) = func.GetPositionAreaHeightFWHMFromSinglePeakParameters(pars);

        AssertEx.AreEqual(pos, position, 0, 1E-14);
        AssertEx.AreEqual(amp, height, 0, 1E-14);
        AssertEx.AreEqual(expectedAreas[i], area, 0, 1E-8);
        AssertEx.AreEqual(expectedFwhm[i], fwhm, 0, 1E-8);
      }

      {
        // with the first test set, we test additionally
        var (amp, pos, w, m, v) = testData[0];
        double[] pars = new double[] { amp, pos, w, m, v };
        var (position, area, height, fwhm) = func.GetPositionAreaHeightFWHMFromSinglePeakParameters(pars);

        // (i) when we vary the amplitude, area and height should change accordingly, pos and fwhm stay constant
        foreach (var newAmp in new double[] { -11, 0, 2, 17, 33, 1023, -11 })
        {
          pars = new double[] { newAmp, pos, w, m, v };
          var (newposition, newarea, newheight, newfwhm) = func.GetPositionAreaHeightFWHMFromSinglePeakParameters(pars);
          AssertEx.AreEqual(position, newposition, 0, 1E-14);
          AssertEx.AreEqual(height * newAmp, newheight, 0, 1E-14);
          AssertEx.AreEqual(area * newAmp, newarea, 0, 1E-14);
          AssertEx.AreEqual(fwhm, newfwhm, 0, 1E-14);
        }

        // (ii) when we vary the position, position should change accordingly, everything else is the same
        foreach (var newPos in new double[] { -11, 0, 2, 17, 33, 1023, -11 })
        {
          pars = new double[] { amp, newPos, w, m, v };
          var (newposition, newarea, newheight, newfwhm) = func.GetPositionAreaHeightFWHMFromSinglePeakParameters(pars);
          AssertEx.AreEqual(newPos, newposition, 0, 1E-14);
          AssertEx.AreEqual(height, newheight, 0, 1E-14);
          AssertEx.AreEqual(area, newarea, 0, 1E-14);
          AssertEx.AreEqual(fwhm, newfwhm, 0, 1E-14);
        }

        // (iii) when we vary the width, area and fwhm should change accordingly, everything else is the same
        foreach (var newW in new double[] { 1 / 1000d, 2, 17, 33, 1023 })
        {
          pars = new double[] { amp, pos, newW, m, v };
          var (newposition, newarea, newheight, newfwhm) = func.GetPositionAreaHeightFWHMFromSinglePeakParameters(pars);
          AssertEx.AreEqual(position, newposition, 0, 1E-14);
          AssertEx.AreEqual(height, newheight, 0, 1E-14);
          AssertEx.AreEqual(area * (newW / w), newarea, 0, 1E-14);
          AssertEx.AreEqual(fwhm * (newW / w), newfwhm, 0, 1E-14);
        }
      }
    }

    [Fact]
    public void TestDerivativesOfAreaFwhm()
    {
      // see PearsonIV (amplitudeModified).nb
      double amp = 3;
      double pos = 7;
      double w = 5;
      double m = 11;
      double v = 13;

      var ymaxDerivs = new double[] { 1, 0, 0, 0, 0 };
      var xmaxDerivs = new double[] { 0, 1, 0, 0, 0 };
      var areaDerivs = new double[] { 11.0170244919571930798, 0, 6.6102146951743158479, -0.106359067713399883296, 0.00045052750734765666701 };
      var fwhmDerivs = new double[] { 0, 0, 2.0138612493466088250930420306, -0.0062732720000624223737773164, 0.0000628521667333096080982645 };

      double[] pars = new double[] { amp, pos, w, m, v };
      double[] X = new double[1];
      double[] Y = new double[1];

      var func = new PearsonIVAmplitudeParametrizationHPW();



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
    public void TestDerivativesWrtParameters()
    {
      var ff = new PearsonIVAmplitudeParametrizationHPW();

      // General case
      double amplitude = 17;
      double position = 7;
      double w = 3;
      double m = 5;
      double v = 7;

      double expectedFunctionValue = 10.456864176583425249219583024250;
      double expectedDerivativeWrtAmplitude = 0.61510965744608383818938723672062;
      double expectedDerivativeWrtPosition = 6.2023944308469781499266683603126;
      double expectedDerivativeWrtW = 4.1349296205646520999511122402084;
      double expectedDerivativeWrtM = 0.31213582841409496793210643128803;
      double expectedDerivativeWrtV = -0.0080620681798224796387712054749779;

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
      AssertEx.AreEqual(expectedDerivativeWrtV, DY[0, 4], 0, 1E-11);
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
          var fwhmP = PearsonIVAmplitudeParametrizationHPW.GetFWHM(w, m, v);
          var fwhmN = PearsonIVAmplitudeParametrizationHPW.GetFWHM(w, m, -v);

          var fwhmApproxP = PearsonIVAmplitudeParametrizationHPW.GetFWHMApproximation(w, m, v);
          var fwhmApproxN = PearsonIVAmplitudeParametrizationHPW.GetFWHMApproximation(w, m, -v);

          // FWHM should be independent of whether v is positive or negative
          AssertEx.AreEqual(fwhmP, fwhmN, 1E-8, 1E-8);

          AssertEx.AreEqual(fwhmP, fwhmApproxP, 0, claimedMaxApproximationError);
          AssertEx.AreEqual(fwhmN, fwhmApproxN, 0, claimedMaxApproximationError);
        }
      }
    }


  }
}
