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
  public class PearsonVIIAmplitudeTests
  {
    [Fact]
    public void TestDerivedQuantities()
    {
      double amp = 3;
      double pos = 7;
      double w = 5;
      double m = 11;

      var areaDerivs = new double[] { 10.852370479261061516, 0, 6.5114222875566369095, -0.060225693099807316307 };

      double[] pars = new double[] { amp, pos, w, m };
      double[] X = new double[1];
      double[] Y = new double[1];

      var func = new PearsonVIIAmplitude();

      for (int i = 0; i < pars.Length; i++)
      {
        var cov = CreateMatrix.Dense<double>(pars.Length, pars.Length);
        cov[i, i] = 1;
        var result = func.GetPositionAreaHeightFWHMFromSinglePeakParameters(pars, cov);
        AssertEx.AreEqual(Math.Abs(areaDerivs[i]), result.AreaStdDev, 1E-13, 1E-7);
      }
    }

    [Fact]
    public void TestDerivatives()
    {
      var v = new PearsonVIIAmplitude();

      // General case
      double amplitude = 17;
      double position = 7;
      double w = 3;
      double m = 5;

      double expectedFunctionValue = 12.344779169979760569647872188548;
      double expectedDerivativeWrtAmplitude = 0.72616348058704473939105130520869;
      double expectedDerivativeWrtPosition = 3.8263426319394849663517294157831;
      double expectedDerivativeWrtW = 2.5508950879596566442344862771887;
      double expectedDerivativeWrtM = 0.029521332942012426374627381351118;


      var parameters = new double[] { amplitude, position, w, m };

      var X = Matrix<double>.Build.Dense(1, 1);
      X[0, 0] = 9;

      var FV = Vector<double>.Build.Dense(1);
      var DY = Matrix<double>.Build.Dense(1, 4);

      v.Evaluate(X, parameters, FV, null);
      v.EvaluateDerivative(X, parameters, null, DY, null);

      AssertEx.AreEqual(expectedFunctionValue, FV[0], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtAmplitude, DY[0, 0], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtPosition, DY[0, 1], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtW, DY[0, 2], 0, 1E-12);
      AssertEx.AreEqual(expectedDerivativeWrtM, DY[0, 3], 0, 1E-12);
    }


  }
}
