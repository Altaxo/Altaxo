#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2021 Dr. Dirk Lellinger
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
using Altaxo.Calc.FitFunctions.General;
using Altaxo.Calc.FitFunctions.Probability;
using Altaxo.Calc.FitFunctions.Transitions;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.Regression.Nonlinear;
using Xunit;

namespace Altaxo.Calc.FitFunctions
{
  public class DoubleEqualityComparer : IEqualityComparer<double>
  {
    private double _relativePrecision;
    private double _absolutePrecision;

    public DoubleEqualityComparer(double absolutePrecision, double relativePrecision)
    {
      _absolutePrecision = absolutePrecision;
      _relativePrecision = relativePrecision;
    }

    public bool Equals(double x, double y)
    {
      return Math.Abs(x - y) <= _absolutePrecision + _relativePrecision * Math.Max(Math.Abs(x), Math.Abs(y));
    }

    public int GetHashCode(double obj)
    {
      throw new NotImplementedException();
    }
  }

  /// <summary>
  /// Test for fit functions, especially those which implement <see cref="IFitFunctionWithDerivative"/>.
  /// </summary>
  public class FitFunctionWithDerivative_Tests
  {
    private static readonly (Func<IFitFunctionWithDerivative> Creation, double x, double[] parameters, double expectedY)[]
      _fitData = new (Func<IFitFunctionWithDerivative> Creation, double x, double[] parameters, double expectedY)[]
      {
        (() => new ExponentialDecay(2), 0.25, new double[]{1,3,5,7,11}, 1+3*Math.Exp(-0.25/5)+7*Math.Exp(-0.25/11)),
        (() => new ExponentialEquilibration(2), 0.5, new double[]{0.125,1,3,5,7,11}, 1+3*(1-Math.Exp(-(0.5-0.125)/5))+7*(1-Math.Exp(-(0.5-0.125)/11)) ),
        (() => new ExponentialGrowth(2), 0.25, new double[]{1,3,5,7,11}, 1+3*Math.Exp(0.25/5)+7*Math.Exp(0.25/11)),
        (() => new FitFunctions.General.Polynomial(2,0), 0.25, new double[]{1,3,5}, 1+3*0.25 + 5*0.25*0.25),
        (() => new FitFunctions.General.Polynomial(2,1), 0.25, new double[]{1,3,5,7}, 1+3*0.25 + 5*0.25*0.25 + 7/0.25),
        (() => new FitFunctions.General.Polynomial(0,2), 0.25, new double[]{1,3,5}, 1+3/0.25 + 5/(0.25*0.25)),
        (() => new Peaks.PseudoVoigtAmplitude(1, 1), 0.5, new double[]{2,3,5, 0.75, 1, 3 }, 4.1204482076268572715),
        (() => new PowerLawPrefactor(2), 0.5, new double[]{1,3,5,7,11}, 1+3*Math.Pow(0.5,5) + 7*Math.Pow(0.5,11)),
        (() => new PowerLawRatio(2), 1.25, new double[]{1,3,5,7,11}, 1+Math.Pow(1.25/3,5) + Math.Pow(1.25/7,11)),
        (() => new Rational(2,0), 0.25, new double[]{2,3,5}, 2+3*0.25 + 5*0.25*0.25),
        (() => new Rational(0,2), 0.25, new double[]{2,3,5}, 2/(1 + 3*0.25 + 5*0.25*0.25)),
        (() => new Rational(1,1), 0.25, new double[]{2,3,5}, (2+3*0.25)/(1 + 5*0.25)),
        (() => new Rational(2,2), 0.25, new double[]{2,3,5,7,11}, (2+3*0.25+5*0.25*0.25)/(1 + 7*0.25 + 11*0.25*0.25)),
        (() => new RationalInverse(2,0), 0.25, new double[]{2,3,5}, (1+2*0.25 + 3*0.25*0.25)/(5)),
        (() => new RationalInverse(0,2), 0.25, new double[]{2,3,5}, 1/(2 + 3*0.25 + 5*0.25*0.25)),
        (() => new RationalInverse(1,1), 0.25, new double[]{2,3,5}, (1+2*0.25)/(3 + 5*0.25)),
        (() => new RationalInverse(2,2), 0.25, new double[]{2,3,5,7,11}, (1+2*0.25+3*0.25*0.25)/(5 + 7*0.25 + 11*0.25*0.25)),
        (() => new StretchedExponentialDecay(1), 0.5, new double[]{0.125,1,3,5,0.5}, 1+3*(Math.Exp(-Math.Pow((0.5-0.125)/5,0.5)))),
        (() => new StretchedExponentialDecay(1), -0.5, new double[]{0.125,1,3,5,0.5}, 4),
        (() => new StretchedExponentialEquilibration(1), 0.5, new double[]{0.125,1,3,5,0.5}, 1+3*(1-Math.Exp(-Math.Pow((0.5-0.125)/5,0.5)))),
        (() => new StretchedExponentialEquilibration(1), -0.5, new double[]{0.125,1,3,5,0.5}, 1),
        (() => new StretchedExponentialGrowth(1), 0.5, new double[]{0.125,1,3,5,0.5}, 1+3*(Math.Exp(Math.Pow((0.5-0.125)/5,0.5)))),
        (() => new StretchedExponentialGrowth(1), -0.5, new double[]{0.125,1,3,5,0.5}, 4),
        (() => new GaussAmplitude(1,1), 0.5, new double[]{2,3,5,1,3}, 1+3*0.5+2*Math.Exp(-0.5*RMath.Pow2((0.5-3)/5))),
        (() => new GaussArea(1,1), 0.5, new double[]{2,3,5,1,3}, 1+3*0.5+(2/(5*Math.Sqrt(2*Math.PI)))*Math.Exp(-0.5*RMath.Pow2((0.5-3)/5))),
        (() => new CauchyAmplitude(1,1), 0.5, new double[]{2,3,5,1,3}, 1+3*0.5+2/(1+RMath.Pow2((0.5-3)/5))),
        (() => new CauchyArea(1,1), 0.5, new double[]{2,3,5,1,3}, 1+3*0.5+(2/(5*Math.PI*(1+RMath.Pow2((0.5-3)/5))))),
        (() => new LogisticDecreasing(1,1), 0.5, new double[]{2,3,5,1,3}, 1+3*0.5+2/(1+Math.Exp((0.5-3)/5))),
        (() => new LogisticIncreasing(1,1), 0.5, new double[]{2,3,5,1,3}, 1+3*0.5+2/(1+Math.Exp(-(0.5-3)/5))),
        (() => new GeneralizedLogisticDecreasing(1,1), 0.5, new double[]{2,3,5,0.4, 0.3, 1,3}, 1+3*0.5+2/Math.Pow(1+Math.Pow(Math.Exp((0.5-3)/5),0.4),0.3/0.4)),
        (() => new GeneralizedLogisticIncreasing(1,1), 0.5, new double[]{2,3,5,0.4, 0.3, 1,3}, 1+3*0.5+2/Math.Pow(1+Math.Pow(Math.Exp(-(0.5-3)/5),0.4),0.3/0.4)),
        (() => new Kinetics.KineticsNthOrder(), 0.5, new double[]{2,3,1}, 2*Math.Exp(-3*0.5)),
        (() => new Kinetics.KineticsNthOrder(), 0.5, new double[]{2,3,1.5}, Math.Pow(3*(1.5-1)*0.5 + Math.Pow(2, 1-1.5), 1/(1-1.5))),
        (() => new Kinetics.ConversionNthOrder(), 3.5, new double[]{2,1,3,1}, 1-Math.Exp(3*(2-3.5))),
        (() => new Kinetics.RateOfConversionNthOrder(), 3.5, new double[]{2,1,3,1}, 0.033326989614726919488),
        (() => new Kinetics.ConversionNthOrder(), 3.5, new double[]{2,1,3,1.5}, 153/169d),
        (() => new Kinetics.RateOfConversionNthOrder(), 3.5, new double[]{2,1,3,1.5}, 192/2197d),
      };
    private static DoubleEqualityComparer CompareD = new DoubleEqualityComparer(1E-100, 1E-12);
    private static DoubleEqualityComparer CompareDerivatives = new DoubleEqualityComparer(1E-5, 1E-5);

    /// <summary>
    /// Tests the function values, and for fit functions implementing <see cref="IFitFunctionWithDerivative"/>,
    /// test the gradient values.
    /// </summary>
    [Fact]
    public void Test01()
    {
      double[] x = new double[1];
      double[] y = new double[1];
      foreach (var entry in _fitData)
      {
        var fitFunction = entry.Creation();
        Assert.Equal(fitFunction.NumberOfParameters, entry.parameters.Length);
        x[0] = entry.x;
        fitFunction.Evaluate(x, entry.parameters, y);
        Assert.Equal(entry.expectedY, y[0], CompareD);

        if (fitFunction is IFitFunctionWithDerivative fg)
        {
          TestGradients(fg, entry.x, entry.parameters);
        }
      }
    }

    [Fact]
    public void TestCoverage()
    {
      var typeHash = _fitData.Select(x => x.Creation().GetType()).ToHashSet();
      var allTypes = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(IFitFunctionWithDerivative)).ToHashSet();
      allTypes.ExceptWith(typeHash);

      Assert.True(0 == allTypes.Count);
    }



    private static void TestGradients(IFitFunctionWithDerivative ff, double x0, double[] parameters)
    {
      const double delta = 1 / 131072d;
      double[] paraVariation = new double[parameters.Length];
      var actualDerivative = Matrix<double>.Build.Dense(5, parameters.Length);
      double[] x = new double[5];
      double[] y = new double[5];
      var xx = MatrixMath.ToROMatrixWithOneColumn(x);
      var yy = VectorMath.ToVector(y);

      for (int i = 0; i < 5; ++i)
      {
        x[i] = x0;
        y[i] = double.NaN;
        for (int k = 0; k < parameters.Length; ++k)
          actualDerivative[i, k] = double.NaN;
      }

      ff.Evaluate(xx, parameters, null, yy);
      var y0 = y[4];

      ff.EvaluateDerivative(xx, parameters, null, actualDerivative);

      for (int i = 0; i < paraVariation.Length; ++i)
      {
        Array.Copy(parameters, paraVariation, paraVariation.Length);

        double d = delta;
        if (paraVariation[i] != 0)
        {
          d = Math.Abs(paraVariation[i]) * delta;
        }
        paraVariation[i] += d; ;
        ff.Evaluate(xx, paraVariation, null, yy);
        var y1 = y[4];

        var approximatedDerivative = (y1 - y0) / d;
        Assert.Equal(approximatedDerivative, actualDerivative[4, i], CompareDerivatives);
      }
    }
  }
}
