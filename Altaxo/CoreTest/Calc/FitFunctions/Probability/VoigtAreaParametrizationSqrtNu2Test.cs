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

namespace Altaxo.Calc.FitFunctions.Probability
{
  public class VoigtAreaParametrizationSqrtNu2Test
  {
    [Fact]
    public void TestDerivativesSecondaryParameters_GeneralCase()
    {
      // see VoigtArea-Derivatives-ParametrizationSqrtNuLog4.nb
      double area = 17;
      double pos = 7;
      double w = 3;
      double nu = 5 / 7d;

      var pahf = new double[] { pos, area, 2.35429674800053710973966, 6.04835222264692750740263 };
      // derivatives
      var ymaxDerivs = new double[] { 0.138488044000031594690569, 0, -0.784765582666845703246555, 0.986294916351210818615044 };
      var xmaxDerivs = new double[] { 0, 1, 0, 0 };
      var areaDerivs = new double[] { 1, 0, 0, 0 };
      var fwhmDerivs = new double[] { 0, 0, 2.01653911742859190892489, -0.134690859032882012668096 };

      double[] pars = new double[] { area, pos, w, nu };
      double[] X = new double[1];
      double[] Y = new double[1];

      var func = new VoigtAreaParametrizationSqrtNu2();

      for (int i = 0; i < pars.Length; i++)
      {
        var cov = CreateMatrix.Dense<double>(pars.Length, pars.Length);
        cov[i, i] = 1;
        var result = func.GetPositionAreaHeightFWHMFromSinglePeakParameters(pars, cov);
        AssertEx.AreEqual(Math.Abs(areaDerivs[i]), result.AreaStdDev, 1E-13, 1E-7);
        AssertEx.AreEqual(Math.Abs(xmaxDerivs[i]), result.PositionStdDev, 1E-13, 1E-7);
        AssertEx.AreEqual(Math.Abs(ymaxDerivs[i]), result.HeightStdDev, 1E-13, 1E-7);
        AssertEx.AreEqual(Math.Abs(fwhmDerivs[i]), result.FWHMStdDev, 1E-13, 1E-7);

        AssertEx.AreEqual(pahf[0], result.Position, 1E-13, 1E-10);
        AssertEx.AreEqual(pahf[1], result.Area, 1E-13, 1E-10);
        AssertEx.AreEqual(pahf[2], result.Height, 1E-13, 1E-10);
        AssertEx.AreEqual(pahf[3], result.FWHM, 1E-13, 1E-10);
      }
    }


    [Fact]
    public void TestDerivativesSecondaryParameters_NearlyLorentzianLimit()
    {
      // see VoigtArea-Derivatives-ParametrizationSqrtNuLog4.nb
      double area = 17;
      double pos = 7;
      double w = 3;
      double nu = 1 / 32767d;

      var pahf = new double[] { pos, area, 1.80377136162144979963231, 6.00001501741722156713951 };
      // derivatives
      var ymaxDerivs = new double[] { 0.106104197742438223507783, 0, -0.601257120540483266544103, 0.502664788477749705785319 };
      var xmaxDerivs = new double[] { 0, 1, 0, 0 };
      var areaDerivs = new double[] { 1, 0, 0, 0 };
      var fwhmDerivs = new double[] { 0, 0, 2.00000452341909985950189, 0.444626388979414132950005 };

      double[] pars = new double[] { area, pos, w, nu };
      double[] X = new double[1];
      double[] Y = new double[1];

      var func = new VoigtAreaParametrizationSqrtNu2();

      for (int i = 0; i < pars.Length; i++)
      {
        var cov = CreateMatrix.Dense<double>(pars.Length, pars.Length);
        cov[i, i] = 1;
        var result = func.GetPositionAreaHeightFWHMFromSinglePeakParameters(pars, cov);
        AssertEx.AreEqual(Math.Abs(areaDerivs[i]), result.AreaStdDev, 1E-13, 1E-7);
        AssertEx.AreEqual(Math.Abs(xmaxDerivs[i]), result.PositionStdDev, 1E-13, 1E-7);
        AssertEx.AreEqual(Math.Abs(ymaxDerivs[i]), result.HeightStdDev, 1E-13, 1E-7);
        AssertEx.AreEqual(Math.Abs(fwhmDerivs[i]), result.FWHMStdDev, 1E-13, 1E-7);

        AssertEx.AreEqual(pahf[0], result.Position, 1E-13, 1E-10);
        AssertEx.AreEqual(pahf[1], result.Area, 1E-13, 1E-10);
        AssertEx.AreEqual(pahf[2], result.Height, 1E-13, 1E-10);
        AssertEx.AreEqual(pahf[3], result.FWHM, 1E-13, 1E-10);
      }
    }

    [Fact]
    public void TestDerivativesSecondaryParameters_ExactlyLorentzianLimit()
    {
      // see VoigtArea-Derivatives-ParametrizationSqrtNuLog4.nb
      double area = 17;
      double pos = 7;
      double w = 3;
      double nu = 0;

      var pahf = new double[] { pos, area, 1.80375602170814713871402, 6 };
      // derivatives
      var ymaxDerivs = new double[] { 0.106103295394596890512589, 0, -0.601252007236049046238005, 0.502621087962172486855736 };
      var xmaxDerivs = new double[] { 0, 1, 0, 0 };
      var areaDerivs = new double[] { 1, 0, 0, 0 };
      var fwhmDerivs = new double[] { 0, 0, 2, 0.444686854097446089796046 };

      double[] pars = new double[] { area, pos, w, nu };
      double[] X = new double[1];
      double[] Y = new double[1];

      var func = new VoigtAreaParametrizationSqrtNu2();

      for (int i = 0; i < pars.Length; i++)
      {
        var cov = CreateMatrix.Dense<double>(pars.Length, pars.Length);
        cov[i, i] = 1;
        var result = func.GetPositionAreaHeightFWHMFromSinglePeakParameters(pars, cov);
        AssertEx.AreEqual(Math.Abs(areaDerivs[i]), result.AreaStdDev, 1E-13, 1E-7);
        AssertEx.AreEqual(Math.Abs(xmaxDerivs[i]), result.PositionStdDev, 1E-13, 1E-7);
        AssertEx.AreEqual(Math.Abs(ymaxDerivs[i]), result.HeightStdDev, 1E-13, 1E-7);
        AssertEx.AreEqual(Math.Abs(fwhmDerivs[i]), result.FWHMStdDev, 1E-13, 1E-7);

        AssertEx.AreEqual(pahf[0], result.Position, 1E-13, 1E-10);
        AssertEx.AreEqual(pahf[1], result.Area, 1E-13, 1E-10);
        AssertEx.AreEqual(pahf[2], result.Height, 1E-13, 1E-10);
        AssertEx.AreEqual(pahf[3], result.FWHM, 1E-13, 1E-10);
      }
    }

    [Fact]
    public void TestDerivativesSecondaryParameters_NearlyGaussianLimit()
    {
      // see VoigtArea-Derivatives-ParametrizationSqrtNuLog4.nb
      double area = 17;
      double pos = 7;
      double w = 3;
      double nu = 1 - 1 / 32767d;

      var pahf = new double[] { pos, area, 2.66170326013146224987024, 6.00000595967162241141443 };
      // derivatives
      var ymaxDerivs = new double[] { 0.156570780007733073521779, 0, -0.887234420043820749956747, 1.16964641292668431368984 };
      var xmaxDerivs = new double[] { 0, 1, 0, 0 };
      var areaDerivs = new double[] { 1, 0, 0, 0 };
      var fwhmDerivs = new double[] { 0, 0, 2.00000210576251930356528, -0.206995511602332887432860 };

      double[] pars = new double[] { area, pos, w, nu };
      double[] X = new double[1];
      double[] Y = new double[1];

      var func = new VoigtAreaParametrizationSqrtNu2();

      for (int i = 0; i < pars.Length; i++)
      {
        var cov = CreateMatrix.Dense<double>(pars.Length, pars.Length);
        cov[i, i] = 1;
        var result = func.GetPositionAreaHeightFWHMFromSinglePeakParameters(pars, cov);
        AssertEx.AreEqual(Math.Abs(areaDerivs[i]), result.AreaStdDev, 1E-13, 1E-7);
        AssertEx.AreEqual(Math.Abs(xmaxDerivs[i]), result.PositionStdDev, 1E-13, 1E-7);
        AssertEx.AreEqual(Math.Abs(ymaxDerivs[i]), result.HeightStdDev, 1E-13, 1E-7);
        AssertEx.AreEqual(Math.Abs(fwhmDerivs[i]), result.FWHMStdDev, 1E-13, 1E-7);

        AssertEx.AreEqual(pahf[0], result.Position, 1E-13, 1E-10);
        AssertEx.AreEqual(pahf[1], result.Area, 1E-13, 1E-10);
        AssertEx.AreEqual(pahf[2], result.Height, 1E-13, 1E-10);
        AssertEx.AreEqual(pahf[3], result.FWHM, 1E-13, 1E-10);
      }
    }

    [Fact]
    public void TestDerivativesSecondaryParameters_ExactlyGaussianLimit()
    {
      // see VoigtArea-Derivatives-ParametrizationSqrtNuLog4.nb
      double area = 17;
      double pos = 7;
      double w = 3;
      double nu = 1;

      var pahf = new double[] { pos, area, 2.66173895631567877902163, 6 };
      // derivatives
      var ymaxDerivs = new double[] { 0.156572879783275222295390, 0, -0.887246318771892926340544, 1.16966732357221200231569 };
      var xmaxDerivs = new double[] { 0, 1, 0, 0 };
      var areaDerivs = new double[] { 1, 0, 0, 0 };
      var fwhmDerivs = new double[] { 0, 0, 2, -0.207001611171248813604190 };

      double[] pars = new double[] { area, pos, w, nu };
      double[] X = new double[1];
      double[] Y = new double[1];

      var func = new VoigtAreaParametrizationSqrtNu2();

      for (int i = 0; i < pars.Length; i++)
      {
        var cov = CreateMatrix.Dense<double>(pars.Length, pars.Length);
        cov[i, i] = 1;
        var result = func.GetPositionAreaHeightFWHMFromSinglePeakParameters(pars, cov);
        AssertEx.AreEqual(Math.Abs(areaDerivs[i]), result.AreaStdDev, 1E-13, 1E-7);
        AssertEx.AreEqual(Math.Abs(xmaxDerivs[i]), result.PositionStdDev, 1E-13, 1E-7);
        AssertEx.AreEqual(Math.Abs(ymaxDerivs[i]), result.HeightStdDev, 1E-13, 1E-7);
        AssertEx.AreEqual(Math.Abs(fwhmDerivs[i]), result.FWHMStdDev, 1E-13, 1E-7);

        AssertEx.AreEqual(pahf[0], result.Position, 1E-13, 1E-10);
        AssertEx.AreEqual(pahf[1], result.Area, 1E-13, 1E-10);
        AssertEx.AreEqual(pahf[2], result.Height, 1E-13, 1E-10);
        AssertEx.AreEqual(pahf[3], result.FWHM, 1E-13, 1E-10);
      }
    }


    [Fact]
    public void TestDerivatives()
    {
      var v = new VoigtAreaParametrizationSqrtNu2();

      // General case
      double area = 17;
      double position = 7;
      double w = 3;
      double nu = 5 / 11d;

      double expectedFunctionValue = 1.53969430505912493288384;
      double expectedDerivativeWrtArea = 0.0905702532387720548755199;
      double expectedDerivativeWrtPosition = 0.469766282659418172420053;
      double expectedDerivativeWrtW = -0.200053913246762862681244;
      double expectedDerivativeWrtNu = 0.677517478495227285639865;


      var parameters = new double[] { area, position, w, nu };

      var X = Matrix<double>.Build.Dense(1, 1);
      X[0, 0] = 9;

      var FV = Vector<double>.Build.Dense(1);
      var DY = Matrix<double>.Build.Dense(1, 4);

      v.Evaluate(X, parameters, FV, null);
      v.EvaluateDerivative(X, parameters, null, DY, null);

      AssertEx.AreEqual(expectedFunctionValue, FV[0], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtArea, DY[0, 0], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtPosition, DY[0, 1], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtW, DY[0, 2], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtNu, DY[0, 3], 0, 1E-12);


      // Case gamma close to 0 (Lorentz limit

      area = 17;
      position = 7;
      w = 3;
      nu = 1 / 16383d;

      // the expected values are those of the Lorentz limit!
      expectedFunctionValue = 1.248792269568032230171214;
      expectedDerivativeWrtArea = 0.0734561275808747703548694;
      expectedDerivativeWrtPosition = 0.384232051961498798779317;
      expectedDerivativeWrtW = -0.160096688317291166158049;
      expectedDerivativeWrtNu = 0.624202576968383211902258;

      parameters = new double[] { area, position, w, nu };

      X[0, 0] = 9;

      v.Evaluate(X, parameters, FV, null);
      v.EvaluateDerivative(X, parameters, null, DY, null);

      AssertEx.AreEqual(expectedFunctionValue, FV[0], 0, 1E-6);
      AssertEx.AreEqual(expectedDerivativeWrtArea, DY[0, 0], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtPosition, DY[0, 1], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtW, DY[0, 2], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtNu, DY[0, 3], 0, 1E-12);

      // Case sigma == 0

      area = 17;
      position = 7;
      w = 3;
      nu = 1;

      expectedFunctionValue = 1.95602477676540327048287;
      expectedDerivativeWrtArea = 0.1150602809862001923813453;
      expectedDerivativeWrtPosition = 0.602583581831260309934069;
      expectedDerivativeWrtW = -0.250285871034294216871578;
      expectedDerivativeWrtNu = 0.865084621539303204435295;

      parameters = new double[] { area, position, w, nu };

      X[0, 0] = 9;

      v.Evaluate(X, parameters, FV, null);
      v.EvaluateDerivative(X, parameters, null, DY, null);

      AssertEx.AreEqual(expectedFunctionValue, FV[0], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtArea, DY[0, 0], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtPosition, DY[0, 1], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtW, DY[0, 2], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtNu, DY[0, 3], 0, 1E-12);
    }
  }
}
