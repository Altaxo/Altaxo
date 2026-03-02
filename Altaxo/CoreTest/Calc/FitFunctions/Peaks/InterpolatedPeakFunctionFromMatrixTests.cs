#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
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

using Altaxo.Calc.LinearAlgebra;
using Xunit;

namespace Altaxo.Calc.FitFunctions.Peaks
{
  public class InterpolatedPeakFunctionFromMatrixTests
  {
    /// <summary>
    /// Tests the InterpolatedPeakFunctionFromMatrix with 3 arguments (height, position and width).
    /// The test function is choosen such that the order of the width dependence is 2 at max, so that the interpolation should be exact.
    /// </summary>
    [Fact]
    public void Test_WidthDependent()
    {
      const int NumberOfSpectralPoints = 100;
      const int NumberOfWidths = 90;

      (double z, double dzdheight, double dzdposition, double dzdwidth) ExpectedFunction(double x, double height, double position, double width)
      {
        var arg = (x - position) * width;
        double body = 1 - RMath.Pow2(arg);
        double z = body * height;
        double dzdheight = body;
        double dzdposition = height * 2 * arg * width;
        double dzdwidth = -height * 2 * arg * arg / width;
        return (z, dzdheight, dzdposition, dzdwidth);
      }

      var x = new double[NumberOfSpectralPoints];
      var widths = new double[NumberOfWidths];

      for (int i = 0; i < NumberOfSpectralPoints; i++)
      {
        x[i] = i - NumberOfSpectralPoints / 2;
      }

      for (int i = 0; i < NumberOfWidths; i++)
      {
        widths[i] = 1 / 16d + (1 / 8d) * (i / 128d);
      }

      var matrix = CreateMatrix.Dense<double>(NumberOfWidths, NumberOfSpectralPoints);
      for (int i = 0; i < NumberOfWidths; i++)
      {
        for (int j = 0; j < NumberOfSpectralPoints; j++)
        {
          matrix[i, j] = ExpectedFunction(x[j], 1.0, 0, widths[i]).z;
        }
      }

      var interpolatedPeakFunction = new InterpolatedPeakFunctionFromMatrix(1, -1, widths, propertyValuesArePeakWidth: true, x, matrix);
      var expectedPeakFunction = new GaussAmplitude(1, -1);

      var X = new double[1];
      var FVactual = new double[1];
      var P = new double[3];
      var XX = CreateMatrix.Dense<double>(1, 1);
      var DYactual = CreateMatrix.Dense<double>(1, 3);

      // Test the interpolation inside:
      // since we are directly on the points, the
      // expected and actual values should be identical, so we can use a very tight tolerance.
      var position = 177;
      var height = 13;
      for (int i = 3; i < NumberOfWidths - 3; i++)
      {
        for (int j = 3; j < NumberOfSpectralPoints - 3; j++)
        {
          XX[0, 0] = X[0] = x[j] + position;
          P[0] = height;
          P[1] = position;
          P[2] = widths[i];
          interpolatedPeakFunction.Evaluate(X, P, FVactual);
          var FVexpected = ExpectedFunction(X[0], P[0], P[1], P[2]);
          AssertEx.AreEqual(FVexpected.z, FVactual[0], 1E-14, 1E-14, $"i={i}, j={j}"); // very tight tolerance, since we are directly on the points

          interpolatedPeakFunction.EvaluateDerivative(XX, P, null, DYactual, null);

          AssertEx.AreEqual(FVexpected.dzdheight, DYactual[0, 0], 1E-14, 1E-14, $"i={i}, j={j}"); // very tight tolerance, since we are directly on the points and this is height derivative
          AssertEx.AreEqual(FVexpected.dzdposition, DYactual[0, 1], 1E-14, 1E-14, $"i={i}, j={j}");
          AssertEx.AreEqual(FVexpected.dzdwidth, DYactual[0, 2], 1E-14, 1E-14, $"i={i}, j={j}");
        }
      }
    }

    /// <summary>
    /// Tests the InterpolatedPeakFunctionFromMatrix with 2 arguments (height and position).
    /// The test function is choosen such that the order of the position dependence is 2 at max, so that the interpolation should be exact.
    /// </summary>
    [Fact]
    public void Test_PositionDependent()
    {
      const int NumberOfSpectralPoints = 100;
      const int NumberOfPositions = 90;

      (double z, double dzdheight, double dzdposition) ExpectedFunction(double x, double height, double position)
      {
        var width = position / 64.0;
        var arg = (x - position) * width;
        double body = 1 - RMath.Pow2(arg);
        double z = body * height;
        double dzdheight = body;
        double dzdposition = height * width * (-((position - x) * (2 * position - x)) / 32d);
        return (z, dzdheight, dzdposition);
      }

      var x = new double[NumberOfSpectralPoints];
      var positions = new double[NumberOfPositions];

      for (int i = 0; i < NumberOfSpectralPoints; i++)
      {
        x[i] = i - NumberOfSpectralPoints / 2;
      }

      for (int i = 0; i < NumberOfPositions; i++)
      {
        positions[i] = 13 + i * 1.5;
      }

      var matrix = CreateMatrix.Dense<double>(NumberOfPositions, NumberOfSpectralPoints);
      for (int i = 0; i < NumberOfPositions; i++)
      {
        for (int j = 0; j < NumberOfSpectralPoints; j++)
        {
          matrix[i, j] = ExpectedFunction(x[j] + positions[i], 1.0, positions[i]).z;
        }
      }

      var interpolatedPeakFunction = new InterpolatedPeakFunctionFromMatrix(1, -1, positions, propertyValuesArePeakWidth: false, x, matrix);
      var expectedPeakFunction = new GaussAmplitude(1, -1);

      var X = new double[1];
      var FVactual = new double[1];
      var P = new double[3];
      var XX = CreateMatrix.Dense<double>(1, 1);
      var DYactual = CreateMatrix.Dense<double>(1, 3);

      // Test the interpolation inside:
      // since we are directly on the points, the
      // expected and actual values should be identical, so we can use a very tight tolerance.
      var height = 13;
      for (int i = 3; i < NumberOfPositions - 3; i++)
      {
        for (int j = 3; j < NumberOfSpectralPoints - 3; j++)
        {
          XX[0, 0] = X[0] = x[j] + positions[i];
          P[0] = height;
          P[1] = positions[i];
          interpolatedPeakFunction.Evaluate(X, P, FVactual);
          var FVexpected = ExpectedFunction(X[0], P[0], P[1]);
          AssertEx.AreEqual(FVexpected.z, FVactual[0], 1E-14, 1E-14, $"i={i}, j={j}"); // very tight tolerance, since we are directly on the points

          interpolatedPeakFunction.EvaluateDerivative(XX, P, null, DYactual, null);

          AssertEx.AreEqual(FVexpected.dzdheight, DYactual[0, 0], 1E-14, 1E-14, $"i={i}, j={j}"); // very tight tolerance, since we are directly on the points and this is height derivative
          AssertEx.AreEqual(FVexpected.dzdposition, DYactual[0, 1], 1E-14, 1E-14, $"i={i}, j={j}");
        }
      }
    }
  }
}
