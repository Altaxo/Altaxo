﻿#region Copyright

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

#nullable enable
using System;
using System.Collections.Generic;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.Regression.Nonlinear;
using Altaxo.Main;

namespace Altaxo.Calc.FitFunctions.Peaks
{
  /// <summary>
  /// Fit fuction with one or more gaussian shaped peaks (bell shape), with a baseline polynomial
  /// of variable order.
  /// </summary>
  [FitFunctionClass]
  [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.Peaks.GaussAmplitude}")]
  public record GaussAmplitude : IFitFunctionWithDerivative, IFitFunctionPeak, IImmutable
  {
    private const double SqrtLog4 = 1.1774100225154746910;


    /// <summary>The order of the baseline polynomial.</summary>
    private readonly int _orderOfBaselinePolynomial;
    /// <summary>The order of the polynomial with negative exponents.</summary>
    private readonly int _numberOfTerms;

    public const int NumberOfParametersPerPeak = 3;
    private const string ParameterBaseName0 = "a";
    private const string ParameterBaseName1 = "xc";
    private const string ParameterBaseName2 = "w";

    #region Serialization

    /// <summary>
    /// 2021-06-07 Initial version 0
    /// 2022-07-18 Moved from AltaxoBase to AltaxoCore
    /// 2022-10-21 Moved from namespace Probability to namespace Peaks
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Calc.FitFunctions.Probability.GaussAmplitude", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoCore", "Altaxo.Calc.FitFunctions.Probability.GaussAmplitude", 1)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(GaussAmplitude), 2)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (GaussAmplitude)obj;
        info.AddValue("NumberOfTerms", s._numberOfTerms);
        info.AddValue("OrderOfBackgroundPolynomial", s._orderOfBaselinePolynomial);
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var numberOfTerms = info.GetInt32("NumberOfTerms");
        var orderOfBackgroundPolynomial = info.GetInt32("OrderOfBackgroundPolynomial");
        return new GaussAmplitude(numberOfTerms, orderOfBackgroundPolynomial);
      }
    }

    #endregion Serialization

    public GaussAmplitude()
    {
      _numberOfTerms = 1;
      _orderOfBaselinePolynomial = -1;
    }

    public GaussAmplitude(int numberOfGaussianTerms, int orderOfBackgroundPolynomial)
    {
      _numberOfTerms = numberOfGaussianTerms;
      _orderOfBaselinePolynomial = orderOfBackgroundPolynomial;

      if (!(_orderOfBaselinePolynomial >= -1))
        throw new ArgumentOutOfRangeException("Order of baseline polynomial has to be greater than or equal to zero, or -1 in order to deactivate it.");
      if (!(_numberOfTerms >= 0))
        throw new ArgumentOutOfRangeException("Number of gaussian terms has to be greater than or equal to 0");
    }

    /// <inheritdoc/>
    public override string ToString()
    {
      return $"{this.GetType().Name} NumberOfTerms={NumberOfTerms} OrderOfBaseline={OrderOfBaselinePolynomial}";
    }


    [FitFunctionCreator("GaussAmplitude", "General", 1, 1, 4)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.Peaks.GaussAmplitude}")]
    public static IFitFunction Create_1_0()
    {
      return new GaussAmplitude(1, 0);
    }

    [FitFunctionCreator("GaussAmplitude", "Peaks", 1, 1, 3)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.Peaks.GaussAmplitude}")]
    public static IFitFunction Create_1_M1()
    {
      return new GaussAmplitude(1, -1);
    }

    /// <summary>
    /// Gets/sets the order of the baseline polynomial.
    /// </summary>
    public int OrderOfBaselinePolynomial
    {
      get => _orderOfBaselinePolynomial;
      init
      {
        if (!(value >= -1))
          throw new ArgumentOutOfRangeException(nameof(OrderOfBaselinePolynomial), $"{nameof(OrderOfBaselinePolynomial)} must be greater than or equal to 0, or -1 in order to deactivate it.");
        _orderOfBaselinePolynomial = value;
      }
    }

    /// <inheritdoc/>
    IFitFunctionPeak IFitFunctionPeak.WithOrderOfBaselinePolynomial(int orderOfBaselinePolynomial)
    {
      return this with { OrderOfBaselinePolynomial = orderOfBaselinePolynomial };
    }

    /// <summary>
    /// Gets/sets the number of peak terms.
    /// </summary>
    public int NumberOfTerms
    {
      get => _numberOfTerms;
      init
      {
        if (!(value >= 0))
          throw new ArgumentOutOfRangeException(nameof(NumberOfTerms), $"{nameof(NumberOfTerms)} must be greater than or equal to 0");
        _numberOfTerms = value;
      }
    }

    /// <inheritdoc/>
    IFitFunctionPeak IFitFunctionPeak.WithNumberOfTerms(int numberOfTerms)
    {
      return this with { NumberOfTerms = numberOfTerms };
    }

    private const double DefaultMinWidth = 1E-81; // Math.Pow(double.Epsilon, 0.25);
    private const double DefaultMaxWidth = 1E+77; // Math.Pow(double.MaxValue, 0.25);

    /// <inheritdoc/>
    public (IReadOnlyList<double?>? LowerBounds, IReadOnlyList<double?>? UpperBounds) GetParameterBoundariesForPositivePeaks(double? minimalPosition = null, double? maximalPosition = null, double? minimalFWHM = null, double? maximalFWHM = null)
    {

      var lowerBounds = new double?[NumberOfParameters];
      var upperBounds = new double?[NumberOfParameters];

      for (int i = 0, j = 0; i < NumberOfTerms; ++i, j += NumberOfParametersPerPeak)
      {
        lowerBounds[j + 0] = 0; // minimal amplitude is 0
        upperBounds[j + 0] = null; // maximal amplitude is not limited

        lowerBounds[j + 1] = minimalPosition;
        upperBounds[j + 1] = maximalPosition;

        lowerBounds[j + 2] = minimalFWHM.HasValue ? minimalFWHM.Value / (2 * SqrtLog4) : DefaultMinWidth; // minimal width is 0
        upperBounds[j + 2] = maximalFWHM.HasValue ? maximalFWHM.Value / (2 * SqrtLog4) : DefaultMaxWidth;
      }

      return (lowerBounds, upperBounds);
    }

    /// <inheritdoc/>
    public (IReadOnlyList<double?>? LowerBounds, IReadOnlyList<double?>? UpperBounds) GetParameterBoundariesHardLimit()
    {
      var lowerBounds = new double?[NumberOfParameters];
      var upperBounds = new double?[NumberOfParameters];

      for (int i = 0, j = 0; i < NumberOfTerms; ++i, j += NumberOfParametersPerPeak)
      {
        lowerBounds[j + 2] = DefaultMinWidth; // minimal width
        upperBounds[j + 2] = DefaultMaxWidth; // maximal width
      }
      return (lowerBounds, upperBounds);
    }

    /// <inheritdoc/>
    public (IReadOnlyList<double?>? LowerBounds, IReadOnlyList<double?>? UpperBounds) GetParameterBoundariesSoftLimit()
    {
      var lowerBounds = new double?[NumberOfParameters];
      for (int i = 0, j = 0; i < NumberOfTerms; ++i, j += NumberOfParametersPerPeak)
      {
        lowerBounds[j] = 0; // use only nonnegative amplitudes
      }
      return (lowerBounds, null);
    }


    #region IFitFunction Members

    public int NumberOfIndependentVariables
    {
      get
      {
        return 1;
      }
    }

    public int NumberOfDependentVariables
    {
      get
      {
        return 1;
      }
    }

    public int NumberOfParameters
    {
      get
      {
        return _numberOfTerms * NumberOfParametersPerPeak + _orderOfBaselinePolynomial + 1;
      }
    }

    public string IndependentVariableName(int i)
    {
      return "x";
    }

    public string DependentVariableName(int i)
    {
      return "y";
    }

    public string ParameterName(int i)
    {
      int k = i - NumberOfParametersPerPeak * _numberOfTerms;
      if (k < 0)
      {
        int j = i / NumberOfParametersPerPeak;
        return (i % NumberOfParametersPerPeak) switch
        {
          0 => FormattableString.Invariant($"a{j}"),
          1 => FormattableString.Invariant($"xc{j}"),
          2 => FormattableString.Invariant($"w{j}"),
          _ => throw new InvalidProgramException()
        };
      }
      else if (k <= OrderOfBaselinePolynomial)
      {
        return FormattableString.Invariant($"b{k}");
      }
      else
      {
        throw new ArgumentOutOfRangeException(nameof(i), $"Parameter index {i} is out of range.");
      }
    }

    public double DefaultParameterValue(int i)
    {
      int k = i - NumberOfParametersPerPeak * _numberOfTerms;
      if (k < 0)
      {
        return (i % NumberOfParametersPerPeak) switch
        {
          0 => 0,
          1 => 0,
          2 => 1,
          _ => throw new InvalidProgramException()
        };
      }
      else if (k <= OrderOfBaselinePolynomial)
      {
        return 0;
      }
      else
      {
        throw new ArgumentOutOfRangeException(nameof(i), $"Parameter index {i} is out of range.");
      }
    }

    public IVarianceScaling? DefaultVarianceScaling(int i)
    {
      return null;
    }

    public static double GetYOfOneTerm(double x, double a, double xc, double w)
    {
      double arg = (x - xc) / w;
      return a * Math.Exp(-0.5 * arg * arg);
    }

    public void Evaluate(double[] X, double[] P, double[] Y)
    {
      // evaluation of gaussian terms
      double sumGauss = 0, sumPolynomial = 0;
      for (int i = 0, j = 0; i < _numberOfTerms; ++i, j += NumberOfParametersPerPeak)
      {
        double x = (X[0] - P[j + 1]) / P[j + 2];
        sumGauss += P[j] * Math.Exp(-0.5 * x * x);
      }

      if (_orderOfBaselinePolynomial >= 0)
      {
        int offset = NumberOfParametersPerPeak * _numberOfTerms;
        // evaluation of terms x^0 .. x^n
        sumPolynomial = P[_orderOfBaselinePolynomial + offset];
        for (int i = _orderOfBaselinePolynomial - 1; i >= 0; i--)
        {
          sumPolynomial *= X[0];
          sumPolynomial += P[i + offset];
        }
      }
      Y[0] = sumGauss + sumPolynomial;
    }

    public void Evaluate(IROMatrix<double> independent, IReadOnlyList<double> P, IVector<double> FV, IReadOnlyList<bool>? dependentVariableChoice)
    {
      var rowCount = independent.RowCount;
      for (int r = 0; r < rowCount; ++r)
      {
        var x = independent[r, 0];
        // evaluation of gaussian terms
        double sumGauss = 0, sumPolynomial = 0;
        for (int i = 0, j = 0; i < _numberOfTerms; ++i, j += NumberOfParametersPerPeak)
        {
          double arg = (x - P[j + 1]) / P[j + 2];
          sumGauss += P[j] * Math.Exp(-0.5 * arg * arg);
        }

        if (_orderOfBaselinePolynomial >= 0)
        {
          int offset = NumberOfParametersPerPeak * _numberOfTerms;
          // evaluation of terms x^0 .. x^n
          sumPolynomial = P[_orderOfBaselinePolynomial + offset];
          for (int i = _orderOfBaselinePolynomial - 1; i >= 0; i--)
          {
            sumPolynomial *= x;
            sumPolynomial += P[i + offset];
          }
        }
        FV[r] = sumGauss + sumPolynomial;
      }
    }

    /// <summary>
    /// Not functional because instance is immutable.
    /// </summary>
    public event EventHandler? Changed { add { } remove { } }

    #endregion IFitFunction Members

    public void EvaluateDerivative(IROMatrix<double> X, IReadOnlyList<double> P, IReadOnlyList<bool>? isParameterFixed, IMatrix<double> DY, IReadOnlyList<bool> dependentVariableChoice)
    {
      var rowCount = X.RowCount;
      for (int r = 0; r < rowCount; ++r)
      {
        var x = X[r, 0];

        // at first, the gaussian terms
        for (int i = 0, j = 0; i < _numberOfTerms; ++i, j += NumberOfParametersPerPeak)
        {
          if (isParameterFixed is not null && isParameterFixed[j] && isParameterFixed[j + 1] && isParameterFixed[j + 2])
          {
            continue;
          }

          var arg = (x - P[j + 1]) / P[j + 2];
          var expTerm = Math.Exp(-0.5 * arg * arg);
          DY[r, j + 0] = expTerm;
          DY[r, j + 1] = expTerm * arg * P[j] / P[j + 2];
          DY[r, j + 2] = expTerm * arg * arg * P[j] / P[j + 2];
        }

        if (_orderOfBaselinePolynomial >= 0)
        {
          double xn = 1;
          for (int i = 0, j = NumberOfParametersPerPeak * _numberOfTerms; i <= _orderOfBaselinePolynomial; ++i, ++j)
          {
            DY[r, j] = xn;
            xn *= x;
          }
        }
      }
    }

    #region IFitFunctionPeak

    /// <inheritdoc/>
    public double[] GetInitialParametersFromHeightPositionAndWidthAtRelativeHeight(double height, double position, double width, double relativeHeight)
    {
      if (!(relativeHeight > 0 && relativeHeight < 1))
        throw new ArgumentException("RelativeHeight should be in the open interval (0,1)", nameof(relativeHeight));

      double w = 0.5 * width / Math.Sqrt(-2 * Math.Log(relativeHeight));
      return new double[] { height, position, w };
    }

    /// <summary>
    /// Gets the FWHM from the width value and the relative height at which the width was determined.
    /// </summary>
    /// <param name="width">The width value.</param>
    /// <param name="relativeHeightOfWidthDetermination">The relative height (relative to the maximum), at which the width was determined.</param>
    /// <returns>The estimated full width half maximum value.</returns>
    public static double GetFWHMFromWidthAndRelativeHeight(double width, double relativeHeightOfWidthDetermination)
    {
      return SqrtLog4 * width / Math.Sqrt(-2 * Math.Log(relativeHeightOfWidthDetermination));
    }

    /// <inheritdoc/>
    public string[] ParameterNamesForOnePeak => new string[] { ParameterBaseName0, ParameterBaseName1, ParameterBaseName2 };

    /// <inheritdoc/>
    public (double Position, double Area, double Height, double FWHM) GetPositionAreaHeightFWHMFromSinglePeakParameters(IReadOnlyList<double> parameters)
    {
      var (pos, _, area, _, height, _, fwhm, _) = GetPositionAreaHeightFWHMFromSinglePeakParameters(parameters, null);
      return (pos, area, height, fwhm);
    }

    private static double SafeSqrt(double x) => Math.Sqrt(Math.Max(0, x));

    /// <inheritdoc/>
    public (double Position, double PositionStdDev, double Area, double AreaStdDev, double Height, double HeightStdDev, double FWHM, double FWHMStdDev)
      GetPositionAreaHeightFWHMFromSinglePeakParameters(IReadOnlyList<double> parameters, IROMatrix<double>? cv)
    {
      const double Sqrt2Pi = 2.5066282746310005024;

      if (parameters is null || parameters.Count != NumberOfParametersPerPeak)
        throw new ArgumentException(nameof(parameters));

      var height = parameters[0];
      var pos = parameters[1];
      var sigma = parameters[2];

      var area = height * (sigma * Sqrt2Pi);
      var fwhm = sigma * 2 * SqrtLog4;

      double posStdDev = 0, areaStdDev = 0, heightStdDev = 0, fwhmStdDev = 0;

      if (cv is not null)
      {
        areaStdDev = Sqrt2Pi * SafeSqrt(RMath.Pow2(height) * cv[2, 2] + height * sigma * (cv[0, 2] + cv[2, 0]) + RMath.Pow2(sigma) * cv[0, 0]);
        posStdDev = Math.Sqrt(cv[1, 1]);
        heightStdDev = Math.Sqrt(cv[0, 0]);
        fwhmStdDev = 2 * Math.Sqrt(cv[2, 2]) * SqrtLog4;
      }
      return (pos, posStdDev, area, areaStdDev, height, heightStdDev, fwhm, fwhmStdDev);
    }


    #endregion
  }
}
