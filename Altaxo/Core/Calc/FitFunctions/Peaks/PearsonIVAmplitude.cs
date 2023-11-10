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

#nullable enable
using System;
using System.Collections.Generic;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.Regression.Nonlinear;
using Altaxo.Main;
using Complex64T = System.Numerics.Complex;

namespace Altaxo.Calc.FitFunctions.Peaks
{
  /// <summary>
  /// Fit function with one or more PearsonIV shaped peaks, with a baseline polynomial
  /// of variable order. The PearsonIV is relocated and scaled, so that the amplitude parameter is the maximum height of the peak, and the position parameter is the x-value of the maximum.
  /// </summary>
  /// <remarks>See <see href="https://en.wikipedia.org/wiki/Pearson_distribution#The_Pearson_type_IV_distribution"/>.</remarks>
  [FitFunctionClass]
  [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.Peaks.PearsonIVAmplitude}")]
  public record PearsonIVAmplitude : IFitFunctionWithDerivative, IFitFunctionPeak, IImmutable
  {
    private const string ParameterBaseName0 = "a";
    private const string ParameterBaseName1 = "xc";
    private const string ParameterBaseName2 = "w";
    private const string ParameterBaseName3 = "m";
    private const string ParameterBaseName4 = "ν";
    private const int NumberOfParametersPerPeak = 5;


    /// <summary>The order of the baseline polynomial.</summary>
    private readonly int _orderOfBaselinePolynomial;
    /// <summary>The order of the polynomial with negative exponents.</summary>
    private readonly int _numberOfTerms;

    #region Serialization

    /// <summary>
    /// 2022-07-20 Initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PearsonIVAmplitude), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PearsonIVAmplitude)obj;
        info.AddValue("NumberOfTerms", s._numberOfTerms);
        info.AddValue("OrderOfBackgroundPolynomial", s._orderOfBaselinePolynomial);
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var numberOfTerms = info.GetInt32("NumberOfTerms");
        var orderOfBackgroundPolynomial = info.GetInt32("OrderOfBackgroundPolynomial");
        return new PearsonIVAmplitude(numberOfTerms, orderOfBackgroundPolynomial);
      }
    }

    #endregion Serialization

    public PearsonIVAmplitude()
    {
      _numberOfTerms = 1;
      _orderOfBaselinePolynomial = -1;
    }

    public PearsonIVAmplitude(int numberOfTerms, int orderOfBackgroundPolynomial)
    {
      _numberOfTerms = numberOfTerms;
      _orderOfBaselinePolynomial = orderOfBackgroundPolynomial;

      if (!(_orderOfBaselinePolynomial >= -1))
        throw new ArgumentOutOfRangeException("Order of baseline polynomial has to be greater than or equal to zero, or -1 in order to deactivate it.");
      if (!(_numberOfTerms >= 1))
        throw new ArgumentOutOfRangeException("Number of terms has to be greater than or equal to 1");
    }

    /// <inheritdoc/>
    public override string ToString()
    {
      return $"{this.GetType().Name} NumberOfTerms={NumberOfTerms} OrderOfBaseline={OrderOfBaselinePolynomial}";
    }

    [FitFunctionCreator("PearsonIVAmplitude", "Peaks", 1, 1, NumberOfParametersPerPeak)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.Peaks.PearsonIVAmplitude}")]
    public static IFitFunction Create_1_M1()
    {
      return new PearsonIVAmplitude(1, -1);
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
          4 => FormattableString.Invariant($"{ParameterBaseName4}{j}"),
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
      int k = i - NumberOfParametersPerPeak * _numberOfTerms;
      if (k < 0)
      {
        return (i % NumberOfParametersPerPeak) switch
        {
          0 => 1, // amplitude
          1 => 0, // position
          2 => 1, // width
          3 => 1, // m (Lorentzian),
          4 => 0, // v (symmetric)
          _ => 0
        };
      }
      else
      {
        return 0; // no baseline
      }
    }

    public IVarianceScaling? DefaultVarianceScaling(int i)
    {
      return null;
    }

    public void Evaluate(double[] X, double[] P, double[] Y)
    {
      // evaluation of gaussian terms
      double sumTerms = 0, sumPolynomial = 0;
      for (int i = 0, j = 0; i < _numberOfTerms; ++i, j += NumberOfParametersPerPeak)
      {
        sumTerms += GetYOfOneTerm(X[0], P[j], P[j + 1], P[j + 2], P[j + 3], P[j + 4]);
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
      Y[0] = sumTerms + sumPolynomial;
    }

    public void Evaluate(IROMatrix<double> independent, IReadOnlyList<double> P, IVector<double> FV, IReadOnlyList<bool>? dependentVariableChoice)
    {
      var rowCount = independent.RowCount;
      for (int r = 0; r < rowCount; ++r)
      {
        var x = independent[r, 0];
        // evaluation of gaussian terms
        double sumTerms = 0, sumPolynomial = 0;
        for (int i = 0, j = 0; i < _numberOfTerms; ++i, j += NumberOfParametersPerPeak)
        {
          sumTerms += GetYOfOneTerm(x, P[j], P[j + 1], P[j + 2], P[j + 3], P[j + 4]);
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
        FV[r] = sumTerms + sumPolynomial;
      }
    }

    public void EvaluateDerivative(IROMatrix<double> X, IReadOnlyList<double> P, IReadOnlyList<bool>? isParameterFixed, IMatrix<double> DY, IReadOnlyList<bool> dependentVariableChoice)
    {
      const double Log2 = 0.69314718055994530941723212145818; // Math.Log(2)

      var rowCount = X.RowCount;
      for (int r = 0; r < rowCount; ++r)
      {
        var x = X[r, 0];

        // at first, the peak terms
        for (int i = 0, j = 0; i < _numberOfTerms; ++i, j += NumberOfParametersPerPeak)
        {
          if (isParameterFixed is not null && isParameterFixed[j] && isParameterFixed[j + 1] && isParameterFixed[j + 2] && isParameterFixed[j + 3] && isParameterFixed[j + 4])
          {
            continue;
          }

          var height = P[j];
          var pos = P[j + 1];
          var w = P[j + 2];
          var m = P[j + 3];
          var v = P[j + 4];

          double z0 = -v / (2 * m);
          double z = (x - pos) / w + z0;

          var log1z2_1z02 = Math.Log((1 + z * z) / (1 + z0 * z0));
          var atanZ_M_atanZ0 = Math.Atan(z) - Math.Atan(z0);
          var body = Math.Exp(-m * log1z2_1z02 - v * atanZ_M_atanZ0);
          var dbodydz = -body * (2 * z * m + v) / (1 + z * z);

          DY[r, j + 0] = body;
          DY[r, j + 1] = -height * dbodydz / w;
          DY[r, j + 2] = -height * dbodydz * (x - pos) / (w * w);
          DY[r, j + 3] = -height * (body * log1z2_1z02 + dbodydz * z0 / m);
          DY[r, j + 4] = -height * (body * atanZ_M_atanZ0 + dbodydz * 0.5 / m);
        }

        // then, the baseline
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


    /// <summary>
    /// Not functional because instance is immutable.
    /// </summary>
    public event EventHandler? Changed { add { } remove { } }

    #endregion IFitFunction Members



    /// <inheritdoc/>
    public double[] GetInitialParametersFromHeightPositionAndWidthAtRelativeHeight(double height, double position, double width, double relativeHeight)
    {
      if (!(relativeHeight > 0 && relativeHeight < 1))
        throw new ArgumentException("RelativeHeight should be in the open interval (0,1)", nameof(relativeHeight));

      var w = Math.Abs(0.5 * width * Math.Sqrt(relativeHeight / (1 - relativeHeight)));
      return new double[NumberOfParametersPerPeak] { height, position, w, 1, 0 }; // Parameters for the Lorentz limit
    }

    const double DefaultMinWidth = 1E-81; // Math.Pow(double.Epsilon, 0.25);
    const double DefaultMaxWidth = 1E+77; // Math.Pow(double.MaxValue, 0.25);

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

        lowerBounds[j + 2] = minimalFWHM.HasValue ? minimalFWHM / 2 : DefaultMinWidth;
        upperBounds[j + 2] = maximalFWHM.HasValue ? maximalFWHM / 2 : DefaultMaxWidth;

        lowerBounds[j + 3] = 1 / 1024.0;
        upperBounds[j + 3] = 1024;

        lowerBounds[j + 4] = -1024;
        upperBounds[j + 4] = 1024;
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
        lowerBounds[j + 2] = DefaultMinWidth;
        upperBounds[j + 2] = DefaultMaxWidth;

        lowerBounds[j + 3] = 1 / 1024.0;
        upperBounds[j + 3] = 1024;

        lowerBounds[j + 4] = -1024;
        upperBounds[j + 4] = 1024;
      }
      return (lowerBounds, upperBounds);
    }

    /// <inheritdoc/>
    public (IReadOnlyList<double?>? LowerBounds, IReadOnlyList<double?>? UpperBounds) GetParameterBoundariesSoftLimit()
    {
      return (null, null);
    }


    /// <inheritdoc/>
    public string[] ParameterNamesForOnePeak => new string[] { ParameterBaseName0, ParameterBaseName1, ParameterBaseName2, ParameterBaseName3, ParameterBaseName4 };

    public static double GetYOfOneTerm(double x, double amplitude, double pos, double w, double m, double v)
    {
      double z0 = -v / (2 * m);
      double z = (x - pos) / w + z0;
      return amplitude * Math.Exp(-m * Math.Log((1 + z * z) / (1 + z0 * z0)) - v * (Math.Atan(z) - Math.Atan(z0)));
    }

    /// <inheritdoc/>
    public (double Position, double Area, double Height, double FWHM) GetPositionAreaHeightFWHMFromSinglePeakParameters(IReadOnlyList<double> parameters)
    {
      var result = GetPositionAreaHeightFWHMFromSinglePeakParameters(parameters, null);

      return (result.Position, result.Area, result.Height, result.FWHM);
    }

    private static double SafeSqrt(double x) => Math.Sqrt(Math.Max(0, x));

    public (double Position, double PositionStdDev, double Area, double AreaStdDev, double Height, double HeightStdDev, double FWHM, double FWHMStdDev)
      GetPositionAreaHeightFWHMFromSinglePeakParameters(IReadOnlyList<double> parameters, IROMatrix<double> cv)
    {
      if (parameters is null || parameters.Count != NumberOfParametersPerPeak)
        throw new ArgumentException(nameof(parameters));

      var amp = parameters[0];
      var loc = parameters[1];
      var w = parameters[2];
      var m = parameters[3];
      var v = parameters[4];

      var area = GetArea(amp, w, m, v);
      var pos = loc;
      var height = amp;
      var fwhm = GetHWHM(w, m, v, true) + GetHWHM(w, m, v, false);

      double posStdDev = 0, areaStdDev = 0, heightStdDev = 0, fwhmStdDev = 0;

      if (cv is not null)
      {
        var deriv = new double[5];
        var resVec = VectorMath.ToVector(new double[5]);

        // PositionVariance
        posStdDev = SafeSqrt(cv[1, 1]);

        // Height variance
        heightStdDev = SafeSqrt(cv[0, 0]);

        // Area variance
        deriv[0] = GetArea(1, w, m, v);
        deriv[1] = 0;
        double absDelta = w * 1E-5;
        deriv[2] = (GetArea(amp, w + absDelta, m, v) - GetArea(amp, w - absDelta, m, v)) / (2 * absDelta);
        absDelta = m * 1E-5;
        deriv[3] = (GetArea(amp, w, m + absDelta, v) - GetArea(amp, w, m - absDelta, v)) / (2 * absDelta);
        absDelta = v == 0 ? 1E-5 : Math.Abs(v * 1E-5);
        deriv[4] = (GetArea(amp, w, m, v + absDelta) - GetArea(amp, w, m, v - absDelta)) / (2 * absDelta);
        MatrixMath.Multiply(cv, deriv, resVec);
        areaStdDev = SafeSqrt(VectorMath.DotProduct(deriv, resVec));

        // FWHM variance
        deriv[0] = 0;
        deriv[1] = 0;
        absDelta = w * 1E-5;
        deriv[2] = (GetFWHM(w + absDelta, m, v) - GetFWHM(w - absDelta, m, v)) / (2 * absDelta);
        absDelta = m * 1E-5;
        deriv[3] = (GetFWHM(w, m + absDelta, v) - GetFWHM(w, m - absDelta, v)) / (2 * absDelta);
        absDelta = v == 0 ? 1E-5 : Math.Abs(v * 1E-5);
        deriv[4] = (GetFWHM(w, m, v + absDelta) - GetFWHM(w, m, v - absDelta)) / (2 * absDelta);
        MatrixMath.Multiply(cv, deriv, resVec);
        fwhmStdDev = SafeSqrt(VectorMath.DotProduct(deriv, resVec));
      }

      return (pos, posStdDev, area, areaStdDev, height, heightStdDev, fwhm, fwhmStdDev);
    }

    /// <summary>
    /// Gets the area under the peak.
    /// </summary>
    /// <param name="amp">The amplitude parameter.</param>
    /// <param name="w">The width parameter.</param>
    /// <param name="m">The m exponent.</param>
    /// <param name="v">The skewness parameter.</param>
    /// <returns>The area under the peak.</returns>
    public static double GetArea(double amp, double w, double m, double v)
    {
      var lnprefactor = (
                        GammaRelated.LnBeta(m - 0.5, 0.5) +
                        2 * GammaRelated.LnGamma(m) -
                        2 * GammaRelated.LnGamma(new Complex64T(m, v / 2)).Real
                      );

      var z0 = -v / (2 * m);

      var lnbody = m * Math.Log(1 + z0 * z0) + v * Math.Atan(z0);

      return amp * w * Math.Exp(lnprefactor + lnbody);
    }

    /// <summary>
    /// Gets the full width half maximum (FWHM)
    /// </summary>
    /// <param name="w">The width parameter.</param>
    /// <param name="m">The m exponent.</param>
    /// <param name="v">The skewness parameter.</param>
    /// <returns>The FWHM of the peak.</returns>
    public static double GetFWHM(double w, double m, double v)
    {
      return GetHWHM(w, m, v, true) + GetHWHM(w, m, v, false);
    }

    /// <summary>
    /// Gets the half width half maximum of a given side of the peak.
    /// </summary>
    /// <param name="w">The width parameter.</param>
    /// <param name="m">The m parameter.</param>
    /// <param name="v">The v parameter.</param>
    /// <param name="rightSide">If set to <c>true</c>, the HWHM of the right side of the peak is determined; otherwise, the HWMH of the left side is determined.</param>
    /// <returns>The half width half maximum of the given side of the peak. The returned value is always positive.</returns>
    /// <remarks>Newton-Raphson iteration is used to calculate HWMH, because a analytical formula is not available.</remarks>
    public static double GetHWHM(double w, double m, double v, bool rightSide)
    {
      return Probability.PearsonIVArea.GetHWHM(w, m, v, rightSide);
    }

    /// <summary>
    /// Gets an approximate value (19% error) for the full width half maximum
    /// </summary>
    /// <param name="w">The width parameter of PearsonIV.</param>
    /// <param name="m">The m parameter of PearsonIV.</param>
    /// <param name="v">The v parameter of PearsonIV.</param>
    /// <returns>An approximate value for the FWHM (full width half maximum).
    /// The maximal error in the range m: (1e-3..1e3) and v: (-1e3..1e3) is 18%.</returns>
    public static double GetFWHMApproximation(double w, double m, double v)
    {
      return Probability.PearsonIVArea.GetFWHMApproximation(w, m, v);
    }

  }
}


