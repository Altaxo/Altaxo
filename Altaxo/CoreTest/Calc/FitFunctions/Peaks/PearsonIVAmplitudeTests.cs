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
        var cov = new DoubleMatrix(pars.Length, pars.Length);
        cov[i, i] = 1;
        var result = func.GetPositionAreaHeightFWHMFromSinglePeakParameters(pars, cov);
        AssertEx.AreEqual(Math.Abs(areaDerivs[i]), result.AreaVariance, 1E-13, 1E-7);
        AssertEx.AreEqual(Math.Abs(xmaxDerivs[i]), result.PositionVariance, 1E-13, 1E-7);
        AssertEx.AreEqual(Math.Abs(ymaxDerivs[i]), result.HeightVariance, 1E-13, 1E-7);
        AssertEx.AreEqual(Math.Abs(fwhmDerivs[i]), result.FWHMVariance, 1E-13, 1E-7);
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
        var cov = new DoubleMatrix(pars.Length, pars.Length);
        cov[i, i] = 1;
        var result = func.GetPositionAreaHeightFWHMFromSinglePeakParameters(pars, cov);
        AssertEx.AreEqual(Math.Abs(areaDerivs[i]), result.AreaVariance, 1E-13, 1E-7);
        AssertEx.AreEqual(Math.Abs(xmaxDerivs[i]), result.PositionVariance, 1E-13, 1E-7);
        AssertEx.AreEqual(Math.Abs(ymaxDerivs[i]), result.HeightVariance, 1E-13, 1E-7);
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

      AssertEx.AreEqual(1.610212109928116475803608029, left, 1E-15, 2E-15);
      AssertEx.AreEqual(1.357415926553722674910974552, right, 1E-15, 2E-15);

       w = 5;
       m = 300;
       v = -1290;
       left = PearsonIVAmplitude.GetHWHM(w, m, v, false);
       right = PearsonIVAmplitude.GetHWHM(w, m, v, true);

      AssertEx.AreEqual(0.5537655602052538, left, 1E-15, 2E-15);
      AssertEx.AreEqual(0.58690275900381228, right, 1E-15, 2E-15);
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
  }
}
