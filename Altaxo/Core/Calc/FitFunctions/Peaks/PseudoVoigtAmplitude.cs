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

namespace Altaxo.Calc.FitFunctions.Peaks
{
  /// <summary>
  /// Fit function with a blend of a Gaussian and a Cauchy (Lorentz) element, see <see href="https://de.wikipedia.org/wiki/Voigt-Profil"/>.
  /// The blend factor nu has a range of [0, 1].
  /// </summary>
  [FitFunctionClass]
  [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.Peaks.PseudoVoigtAmplitude}")]
  public record PseudoVoigtAmplitude : IFitFunctionWithDerivative, IFitFunctionPeak, IImmutable
  {
    private const string ParameterBaseName0 = "a";
    private const string ParameterBaseName1 = "xc";
    private const string ParameterBaseName2 = "w";
    private const string ParameterBaseName3 = "nu";
    private const int NumberOfParametersPerPeak = 4;
    private const double Log2 = 0.69314718055994530941723212145818;
    private const double SqrtPiByLog2 = 2.1289340388624523586305351924692;

    /// <summary>The order of the baseline polynomial.</summary>
    private readonly int _orderOfBaselinePolynomial;
    /// <summary>The order of the polynomial with negative exponents.</summary>
    private readonly int _numberOfTerms;

    #region Serialization

    /// <summary>
    /// 2022-09-23 Initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PseudoVoigtAmplitude), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PseudoVoigtAmplitude)obj;
        info.AddValue("NumberOfTerms", s._numberOfTerms);
        info.AddValue("OrderOfBackgroundPolynomial", s._orderOfBaselinePolynomial);
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var numberOfTerms = info.GetInt32("NumberOfTerms");
        var orderOfBackgroundPolynomial = info.GetInt32("OrderOfBackgroundPolynomial");
        return new PseudoVoigtAmplitude(numberOfTerms, orderOfBackgroundPolynomial);
      }
    }

    #endregion Serialization

    public PseudoVoigtAmplitude()
    {
      _numberOfTerms = 1;
      _orderOfBaselinePolynomial = -1;
    }

    public PseudoVoigtAmplitude(int numberOfTerms, int orderOfBackgroundPolynomial)
    {
      _numberOfTerms = numberOfTerms;
      _orderOfBaselinePolynomial = orderOfBackgroundPolynomial;

      if (!(_orderOfBaselinePolynomial >= -1))
        throw new ArgumentOutOfRangeException("Order of baseline polynomial has to be greater than or equal to zero, or -1 in order to deactivate it.");
      if (!(_numberOfTerms >= 0))
        throw new ArgumentOutOfRangeException("Number of terms has to be greater than or equal to 0");
    }

    /// <inheritdoc/>
    public override string ToString()
    {
      return $"{this.GetType().Name} NumberOfTerms={NumberOfTerms} OrderOfBaseline={OrderOfBaselinePolynomial}";
    }


    [FitFunctionCreator("PseudoVoigtAmplitude", "Peaks", 1, 1, NumberOfParametersPerPeak)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.Peaks.PseudoVoigtAmplitude}")]
    public static IFitFunction Create_1_M1()
    {
      return new PseudoVoigtAmplitude(1, -1);
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
          3 => 0, // nu (Gaussian),
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
        double arg = (X[0] - P[j + 1]) / P[j + 2];
        sumTerms += P[j] * ((P[j + 3]) / (1 + arg * arg) + (1 - P[j + 3]) * Math.Exp(-Log2 * arg * arg));
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
      for (int r = 0; r < independent.RowCount; ++r)
      {
        var x = independent[r, 0];
        // evaluation of gaussian terms
        double sumTerms = 0, sumPolynomial = 0;
        for (int i = 0, j = 0; i < _numberOfTerms; ++i, j += NumberOfParametersPerPeak)
        {
          double arg = (x - P[j + 1]) / P[j + 2];
          sumTerms += P[j] * ((P[j + 3]) / (1 + arg * arg) + (1 - P[j + 3]) * Math.Exp(-Log2 * arg * arg));
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

          var height = P[j];
          var w = P[j + 2];
          var arg = (x - P[j + 1]) / w;
          var nu = P[j + 3];

          var expTerm = Math.Exp(-Log2 * arg * arg);
          var lorentzTerm = 1 / (1 + arg * arg);

          DY[r, j + 0] = nu * lorentzTerm + (1 - nu) * expTerm;
          DY[r, j + 1] = height * (2 * arg / w) * (nu * lorentzTerm * lorentzTerm + (1 - nu) * expTerm * Log2);
          DY[r, j + 2] = height * (2 * arg * arg / w) * (nu * lorentzTerm * lorentzTerm + (1 - nu) * expTerm * Log2);
          DY[r, j + 3] = height * (lorentzTerm - expTerm);
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

      var w = Math.Abs(0.5 * width * Log2 / Math.Sqrt(-Math.Log(relativeHeight)));
      return new double[NumberOfParametersPerPeak] { height, position, w, 0 }; // Parameters for the Gaussian limit
    }

    const double DefaultMinWidth = 1E-81; // Math.Pow(double.Epsilon, 0.25);
    const double DefaultMaxWidth = 1E+77; // Math.Pow(double.MaxValue, 0.25);


    /// <inheritdoc/>
    public (IReadOnlyList<double?>? LowerBounds, IReadOnlyList<double?>? UpperBounds) GetParameterBoundariesForPositivePeaks(double? minimalPosition = null, double? maximalPosition = null, double? minimalFWHM = null, double? maximalFWHM = null)
    {
      // 
      var lowerBounds = new double?[NumberOfParameters];
      var upperBounds = new double?[NumberOfParameters];

      for (int i = 0, j = 0; i < NumberOfTerms; ++i, j += NumberOfParametersPerPeak)
      {
        lowerBounds[j] = 0; // minimal amplitude is 0

        lowerBounds[j + 1] = minimalPosition;
        upperBounds[j + 1] = maximalPosition;

        lowerBounds[j + 2] = minimalFWHM ?? DefaultMinWidth;
        upperBounds[j + 2] = maximalFWHM ?? DefaultMaxWidth;

        lowerBounds[j + 3] = 0;
        upperBounds[j + 3] = 1;
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

        lowerBounds[j + 3] = 0;
        upperBounds[j + 3] = 1;
      }
      return (lowerBounds, upperBounds);
    }

    /// <inheritdoc/>
    public (IReadOnlyList<double?>? LowerBounds, IReadOnlyList<double?>? UpperBounds) GetParameterBoundariesSoftLimit()
    {
      return (null, null);
    }

    /// <inheritdoc/>
    public string[] ParameterNamesForOnePeak => new string[] { ParameterBaseName0, ParameterBaseName1, ParameterBaseName2, ParameterBaseName3 };

    /// <inheritdoc/>
    public (double Position, double Area, double Height, double FWHM) GetPositionAreaHeightFWHMFromSinglePeakParameters(IReadOnlyList<double> parameters)
    {
      if (parameters is null || parameters.Count != NumberOfParametersPerPeak)
        throw new ArgumentException(nameof(parameters));

      var height = parameters[0];
      var pos = parameters[1];
      var w = Math.Abs(parameters[2]);
      var nu = parameters[3];
      double fwhm = 2 * w;
      double area = height * w * (nu * Math.PI + (1 - nu) * SqrtPiByLog2);
      return (pos, area, height, fwhm);
    }

    private static double SafeSqrt(double x) => Math.Sqrt(Math.Max(0, x));

    public (double Position, double PositionStdDev, double Area, double AreaStdDev, double Height, double HeightStdDev, double FWHM, double FWHMStdDev)
      GetPositionAreaHeightFWHMFromSinglePeakParameters(IReadOnlyList<double> parameters, IROMatrix<double> cv)
    {
      if (parameters is null || parameters.Count != NumberOfParametersPerPeak)
        throw new ArgumentException(nameof(parameters));


      var height = parameters[0];
      var pos = parameters[1];
      var w = Math.Abs(parameters[2]);
      var nu = parameters[3];

      var heightStdDev = cv is null ? 0 : Math.Sqrt(cv[0, 0]);
      var posStdDev = cv is null ? 0 : Math.Sqrt(cv[1, 1]);

      double fwhm = 2 * w;
      double fwhmStdDev = cv is null ? 0 : 2 * Math.Sqrt(cv[2, 2]);

      double area = height * w * (nu * Math.PI + (1 - nu) * SqrtPiByLog2);
      double areaStdDev = 0;

      if (cv is not null)
      {
        var deriv = new double[NumberOfParametersPerPeak];
        var resVec = VectorMath.ToVector(new double[NumberOfParametersPerPeak]);
        var mixTerm = (nu * Math.PI + (1 - nu) * SqrtPiByLog2);
        deriv[0] = w * mixTerm;
        deriv[1] = 0;
        deriv[2] = height * mixTerm;
        deriv[3] = height * w * (Math.PI - SqrtPiByLog2);
        MatrixMath.Multiply(cv, deriv, resVec);
        areaStdDev = SafeSqrt(VectorMath.DotProduct(deriv, resVec));
      }

      return (pos, posStdDev, area, areaStdDev, height, heightStdDev, fwhm, fwhmStdDev);
    }


    /// <summary>
    /// Converts the parameters of a pseudo Voigt term to the approximate values of a term of the Voigt (area) function.
    /// </summary>
    /// <param name="height">The height.</param>
    /// <param name="position">The position.</param>
    /// <param name="width">The width.</param>
    /// <param name="nu">The nu.</param>
    private (double area, double position, double width, double gamma) ConvertParametersToVoigtArea(double height, double position, double width, double nu)
    {
      double area = height * width * (nu * Math.PI + (1 - nu) * SqrtPiByLog2);

      double A = 0.161473943436452;
      double B = -0.215628128638965;
      double C = 0.105595155767278;
      double D = -(A + B + C);
      double gamma = (((D * nu + C) * nu + B) * nu + A) * nu + nu;

      return (area, position, width, gamma);
    }

  }
}


