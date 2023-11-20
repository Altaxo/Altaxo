#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2023 Dr. Dirk Lellinger
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
  /// Fit fuction with one or more shifted log-normal peaks, with a baseline polynomial
  /// of variable order. The parametrization is according to what the National Institute of Standards and Technology
  /// is using for describing the intensity curves of their Raman standards.
  /// </summary>
  /// <remarks>One peak of this function has 4 parameters:
  /// <para>h: height of the peak (y-value at the maximum)</para>
  /// <para>xc: location the peak (x-value of the maximum)</para>
  /// <para>w: Full Width Half Maximum (FWHM) of the peak</para>
  /// <para>rho: Shape parameter (0 &lt; rho &lt; Infinity). If rho=1, the shape is Gaussian, otherwise it is skewed to one or the other side.</para>
  /// </remarks>
  [FitFunctionClass]
  [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.Peaks.ShiftedLogNormalParametrizationNIST}")]
  public record ShiftedLogNormalParametrizationNIST : IFitFunctionWithDerivative, IFitFunctionPeak, IImmutable
  {
    private const double Log2 = 0.69314718055994530941723212145818; //  Math.Log(2);
    private const double Log4 = 1.3862943611198906188344642429164; // Math.Log(4);
    private const double Log16 = 2.7725887222397812376689284858327; // Math.Log(16)
    private const double SqrtPiByLog2 = 2.1289340388624523586305351924692; // Math.Sqrt(Math.Pi/Math.Log(2))


    /// <summary>The order of the baseline polynomial.</summary>
    private readonly int _orderOfBaselinePolynomial;
    /// <summary>The order of the polynomial with negative exponents.</summary>
    private readonly int _numberOfTerms;

    public const int NumberOfParametersPerPeak = 4;
    private const string ParameterBaseName0 = "a";
    private const string ParameterBaseName1 = "xc";
    private const string ParameterBaseName2 = "w";
    private const string ParameterBaseName3 = "rho";

    #region Serialization

    /// <summary>
    /// 2023-11-10 Initial version 0
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ShiftedLogNormalParametrizationNIST), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ShiftedLogNormalParametrizationNIST)obj;
        info.AddValue("NumberOfTerms", s._numberOfTerms);
        info.AddValue("OrderOfBackgroundPolynomial", s._orderOfBaselinePolynomial);
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var numberOfTerms = info.GetInt32("NumberOfTerms");
        var orderOfBackgroundPolynomial = info.GetInt32("OrderOfBackgroundPolynomial");
        return new ShiftedLogNormalParametrizationNIST(numberOfTerms, orderOfBackgroundPolynomial);
      }
    }

    #endregion Serialization

    public ShiftedLogNormalParametrizationNIST()
    {
      _numberOfTerms = 1;
      _orderOfBaselinePolynomial = -1;
    }

    public ShiftedLogNormalParametrizationNIST(int numberOfPeakTerms, int orderOfBackgroundPolynomial)
    {
      _numberOfTerms = numberOfPeakTerms;
      _orderOfBaselinePolynomial = orderOfBackgroundPolynomial;

      if (!(_orderOfBaselinePolynomial >= -1))
        throw new ArgumentOutOfRangeException("Order of baseline polynomial has to be greater than or equal to zero, or -1 in order to deactivate it.");
      if (!(_numberOfTerms >= 0))
        throw new ArgumentOutOfRangeException("Number of peak terms has to be greater than or equal to 0");
    }

    /// <inheritdoc/>
    public override string ToString()
    {
      return $"{this.GetType().Name} NumberOfTerms={NumberOfTerms} OrderOfBaseline={OrderOfBaselinePolynomial}";
    }


    [FitFunctionCreator("Shifted Log-Normal (NIST)", "Peaks", 1, 1, 4)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.Peaks.ShiftedLogNormalParametrizationNIST}")]
    public static IFitFunction Create_1_M1()
    {
      return new ShiftedLogNormalParametrizationNIST(1, -1);
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

    private const double DefaultMinRho = 1E-81;
    private const double DefaultMaxRho = 1E+77;

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

        lowerBounds[j + 2] = minimalFWHM.HasValue ? minimalFWHM.Value : DefaultMinWidth; // minimal width is 0
        upperBounds[j + 2] = maximalFWHM.HasValue ? maximalFWHM.Value : DefaultMaxWidth;

        lowerBounds[j + 3] = DefaultMinRho;
        upperBounds[j + 3] = DefaultMaxRho;
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
        lowerBounds[j + 3] = DefaultMinRho; // minimum rho
        upperBounds[j + 3] = DefaultMaxRho; // minimum rho
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
          0 => FormattableString.Invariant($"{ParameterBaseName0}{j}"),
          1 => FormattableString.Invariant($"{ParameterBaseName1}{j}"),
          2 => FormattableString.Invariant($"{ParameterBaseName2}{j}"),
          3 => FormattableString.Invariant($"{ParameterBaseName3}{j}"),
          _ => throw new InvalidProgramException()
        };
      }
      else
      {
        return FormattableString.Invariant($"b{k}");
      }
    }

    public double DefaultParameterValue(int i)
    {
      if (i >= NumberOfParametersPerPeak * _numberOfTerms)
        return 0; // Polynomials
      else
        return (i % NumberOfParametersPerPeak) switch
        {
          0 => 1, // height
          1 => 0, // location
          2 => 1, // FWHM
          3 => 1, // Shape
          _ => throw new InvalidProgramException(),
        };
    }

    public IVarianceScaling? DefaultVarianceScaling(int i)
    {
      return null;
    }

    public static double GetYOfOneTerm(double x, double a, double xc, double w, double rho)
    {
      double arg = (x - xc) / w;

      if (rho == 1)
      {
        return a * Math.Exp(-4 * Log2 * arg * arg);
      }
      else
      {
        var logarg = arg * (rho - 1 / rho);
        return logarg > -1 ? a * Math.Exp(-Log2 * Pow2(Log1p(logarg) / Math.Log(rho))) : 0;
      }
    }

    private static double Pow2(double x) => x * x;

    private static double Log1p(double x)
    {
      double y = 1 + x;
      return Math.Log(y) - ((y - 1) - x) / y;  // cancels errors with IEEE arithmetic
    }

    public void Evaluate(double[] X, double[] P, double[] Y)
    {
      // evaluation of gaussian terms
      double sumPeaks = 0, sumPolynomial = 0;
      for (int i = 0, j = 0; i < _numberOfTerms; ++i, j += NumberOfParametersPerPeak)
      {
        sumPeaks += GetYOfOneTerm(X[0], P[j + 0], P[j + 1], P[j + 2], P[j + 3]);
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
      Y[0] = sumPeaks + sumPolynomial;
    }

    public void Evaluate(IROMatrix<double> independent, IReadOnlyList<double> P, IVector<double> FV, IReadOnlyList<bool>? dependentVariableChoice)
    {
      var rowCount = independent.RowCount;
      for (int r = 0; r < rowCount; ++r)
      {
        var x = independent[r, 0];
        // evaluation of gaussian terms
        double sumPeaks = 0, sumPolynomial = 0;
        for (int i = 0, j = 0; i < _numberOfTerms; ++i, j += NumberOfParametersPerPeak)
        {
          sumPeaks += GetYOfOneTerm(x, P[j + 0], P[j + 1], P[j + 2], P[j + 3]);
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
        FV[r] = sumPeaks + sumPolynomial;
      }
    }

    /// <summary>
    /// Not functional because instance is immutable.
    /// </summary>
    public event EventHandler? Changed { add { } remove { } }

    #endregion IFitFunction Members

    /// <inheritdoc/>
    public void EvaluateDerivative(IROMatrix<double> X, IReadOnlyList<double> P, IReadOnlyList<bool>? isParameterFixed, IMatrix<double> DY, IReadOnlyList<bool>? dependentVariableChoice)
    {
      var rowCount = X.RowCount;
      for (int r = 0; r < rowCount; ++r)
      {
        var x = X[r, 0];

        // at first, the peak terms
        for (int i = 0, j = 0; i < _numberOfTerms; ++i, j += NumberOfParametersPerPeak)
        {
          if (isParameterFixed is not null && isParameterFixed[j] && isParameterFixed[j + 1] && isParameterFixed[j + 2] && isParameterFixed[j + 3])
          {
            continue;
          }

          var h = P[j + 0];
          var x0 = P[j + 1];
          var w = P[j + 2];
          var rho = P[j + 3];
          var arg = (x - P[j + 1]) / P[j + 2];
          double logarg;

          if (arg == 0)
          {
            DY[r, j + 0] = 1;
            DY[r, j + 1] = 0;
            DY[r, j + 2] = 0;
            DY[r, j + 3] = 0;
          }
          else if (rho == 1)
          {
            var expterm = Math.Exp(-Log2 * 4 * arg * arg);
            DY[r, j + 0] = expterm;
            DY[r, j + 1] = h * expterm * 8 * Log2 * arg / w;
            DY[r, j + 2] = h * expterm * 8 * Log2 * arg * arg / w;
            DY[r, j + 3] = h * expterm * 8 * Log2 * arg * arg * arg;
          }
          else if ((logarg = arg * (rho - 1 / rho)) <= -1)
          {
            DY[r, j + 0] = 0;
            DY[r, j + 1] = 0;
            DY[r, j + 2] = 0;
            DY[r, j + 3] = 0;
          }
          else
          {
            var loglogarg = Log1p(logarg);
            var logrho = Math.Log(rho);
            var expTerm = Math.Exp(-Log2 * Pow2(loglogarg / logrho));
            DY[r, j + 0] = expTerm;
            DY[r, j + 1] = 2 * h * expTerm * Log2 * loglogarg / (Pow2(logrho) * ((x - x0) + w / (rho - 1 / rho)));
            DY[r, j + 2] = 2 * h * expTerm * (x - x0) * Log2 * loglogarg / (Pow2(logrho) * w * ((x - x0) + w / (rho - 1 / rho)));
            DY[r, j + 3] = h * (2 * expTerm * Log2 * loglogarg * (loglogarg - logrho * (rho + 1 / rho) / (rho - 1 / rho + 1 / arg))) /
                           (logrho * logrho * logrho * rho);
          }
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

      var lrh = Math.Log(relativeHeight, 2);
      double w = width / Math.Sqrt(-lrh);
      return new double[] { height, position, w };
    }

    /// <inheritdoc/>
    public string[] ParameterNamesForOnePeak => new string[] { ParameterBaseName0, ParameterBaseName1, ParameterBaseName2, ParameterBaseName3 };

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
      var h = parameters[0];
      var x0 = parameters[1];
      var w = parameters[2];
      var rho = parameters[3];

      double posStdDev = 0, areaStdDev = 0, heightStdDev = 0, fwhmStdDev = 0;
      var fwhm = w;
      double area;

      if (rho == 1)
      {
        area = 0.5 * h * w * SqrtPiByLog2;

        if (cv is not null)
        {
          heightStdDev = SafeSqrt(cv[0, 0]);
          posStdDev = SafeSqrt(cv[1, 1]);
          fwhmStdDev = SafeSqrt(cv[2, 2]);

          var deriv = new double[4];
          var resVec = VectorMath.ToVector(new double[4]);

          deriv[0] = 0.5 * w * SqrtPiByLog2;
          deriv[1] = 0;
          deriv[2] = 0.5 * h * SqrtPiByLog2;
          deriv[3] = 0;

          MatrixMath.Multiply(cv, deriv, resVec);
          areaStdDev = SafeSqrt(VectorMath.DotProduct(deriv, resVec));
        }
      }
      else
      {
        var logrho = Math.Log(rho);
        area = h * Math.Exp(logrho * logrho / Log16) * w * logrho * SqrtPiByLog2 / (rho - 1 / rho);

        if (cv is not null)
        {
          heightStdDev = SafeSqrt(cv[0, 0]);
          posStdDev = SafeSqrt(cv[1, 1]);
          fwhmStdDev = SafeSqrt(cv[2, 2]);

          var deriv = new double[4];
          var resVec = VectorMath.ToVector(new double[4]);

          deriv[0] = Math.Exp(logrho * logrho / Log16) * w * logrho * SqrtPiByLog2 / (rho - 1 / rho);
          deriv[1] = 0;
          deriv[2] = Math.Exp(logrho * logrho / Log16) * h * logrho * SqrtPiByLog2 / (rho - 1 / rho);
          deriv[3] = Math.Exp(logrho * logrho / Log16) * h * w * SqrtPiByLog2 * ((rho * rho - 1) * Log4 + rho * rho * logrho * (logrho - Log4) - logrho * (logrho + Log4)) / (2 * Log2 * Pow2(rho * rho - 1));

          MatrixMath.Multiply(cv, deriv, resVec);
          areaStdDev = SafeSqrt(VectorMath.DotProduct(deriv, resVec));
        }
      }

      return (x0, posStdDev, area, areaStdDev, h, heightStdDev, fwhm, fwhmStdDev);
    }


    #endregion
  }
}
