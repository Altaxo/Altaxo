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
  /// Fit function with one or more PearsonVII shaped peaks, with a baseline polynomial
  /// of variable order.
  /// </summary>
  [FitFunctionClass]
  [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.Peaks.PearsonVIIAmplitude}")]
  public record PearsonVIIAmplitude : IFitFunctionWithDerivative, IFitFunctionPeak, IImmutable
  {
    private const string ParameterBaseName0 = "a";
    private const string ParameterBaseName1 = "xc";
    private const string ParameterBaseName2 = "w";
    private const string ParameterBaseName3 = "m";
    private const int NumberOfParametersPerPeak = 4;


    /// <summary>The order of the baseline polynomial.</summary>
    private readonly int _orderOfBaselinePolynomial;
    /// <summary>The order of the polynomial with negative exponents.</summary>
    private readonly int _numberOfTerms;

    #region Serialization

    /// <summary>
    /// 2022-06-01 Initial version
    /// 2022-07-30 Renamed from PearsonVII into PearsonVIIAmplitude
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoCore", "Altaxo.Calc.FitFunctions.Peaks.PearsonVII", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PearsonVIIAmplitude), 1)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PearsonVIIAmplitude)obj;
        info.AddValue("NumberOfTerms", s._numberOfTerms);
        info.AddValue("OrderOfBackgroundPolynomial", s._orderOfBaselinePolynomial);
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var numberOfTerms = info.GetInt32("NumberOfTerms");
        var orderOfBackgroundPolynomial = info.GetInt32("OrderOfBackgroundPolynomial");
        return new PearsonVIIAmplitude(numberOfTerms, orderOfBackgroundPolynomial);
      }
    }

    #endregion Serialization

    public PearsonVIIAmplitude()
    {
      _numberOfTerms = 1;
      _orderOfBaselinePolynomial = -1;
    }

    public PearsonVIIAmplitude(int numberOfTerms, int orderOfBackgroundPolynomial)
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

    [FitFunctionCreator("PearsonVIIAmplitude", "Peaks", 1, 1, NumberOfParametersPerPeak)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.Peaks.PearsonVIIAmplitude}")]
    public static IFitFunction Create_1_M1()
    {
      return new PearsonVIIAmplitude(1, -1);
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
        if (!(value >= 1))
          throw new ArgumentOutOfRangeException(nameof(NumberOfTerms), $"{nameof(NumberOfTerms)} must be greater than or equal to 1");
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
          3 => 1, // m (Lorentzian),
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
        double x = (X[0] - P[j + 1]) / P[j + 2];
        sumTerms += P[j] * Math.Pow(1 + (Math.Pow(2, 1 / P[j + 3]) - 1) * RMath.Pow2(x), -P[j + 3]);
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
          sumTerms += P[j] * Math.Pow(1 + (Math.Pow(2, 1 / P[j + 3]) - 1) * RMath.Pow2(arg), -P[j + 3]);
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
          if (isParameterFixed is not null && isParameterFixed[j] && isParameterFixed[j + 1] && isParameterFixed[j + 2] && isParameterFixed[j + 3])
          {
            continue;
          }

          var height = P[j];
          var w = P[j + 2];
          var arg = (x - P[j + 1]) / w;
          var m = P[j + 3];

          var twoTo1ByM = Math.Pow(2, 1 / m);
          var twoTo1ByM_Minus1 = twoTo1ByM - 1;
          var termInner = 1 + twoTo1ByM_Minus1 * arg * arg;
          var body = Math.Pow(termInner, -m);
          var dbodydarg = -2 * twoTo1ByM_Minus1 * arg * m * body / termInner;

          DY[r, j + 0] = body;
          DY[r, j + 1] = -height * dbodydarg / w;
          DY[r, j + 2] = -height * dbodydarg * arg / w;
          DY[r, j + 3] = height * body * (arg * arg * Log2 * twoTo1ByM / (termInner * m) - Math.Log(termInner));
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
      return new double[NumberOfParametersPerPeak] { height, position, w, 1 }; // Parameters for the Lorentz limit
    }

    const double DefaultMinWidth = 1E-81; // Math.Pow(double.Epsilon, 0.25);
    const double DefaultMaxWidth = 1E+77; // Math.Pow(double.MaxValue, 0.25);


    /// <inheritdoc/>
    public (IReadOnlyList<double?>? LowerBounds, IReadOnlyList<double?>? UpperBounds) GetParameterBoundariesForPositivePeaks(double? minimalPosition = null, double? maximalPosition = null, double? minimalFWHM = null, double? maximalFWHM = null)
    {
      const double DefaultMinWidth = 1.4908919308538355E-81; // Math.Pow(double.Epsilon, 0.25);
      const double DefaultMaxWidth = 1.157920892373162E+77; // Math.Pow(double.MaxValue, 0.25);

      var lowerBounds = new double?[NumberOfParameters];
      var upperBounds = new double?[NumberOfParameters];

      for (int i = 0, j = 0; i < NumberOfTerms; ++i, j += NumberOfParametersPerPeak)
      {
        lowerBounds[j + 0] = 0; // minimal amplitude is 0
        upperBounds[j + 0] = null; // maximal amplitude is unlimited

        lowerBounds[j + 1] = minimalPosition;
        upperBounds[j + 1] = maximalPosition;

        lowerBounds[j + 2] = minimalFWHM.HasValue ? minimalFWHM.Value / 2 : DefaultMinWidth;
        upperBounds[j + 2] = maximalFWHM.HasValue ? maximalFWHM.Value / 2 : DefaultMaxWidth;

        lowerBounds[j + 3] = 1 / 1024.0;
        upperBounds[j + 3] = 1024;
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
      var m = parameters[3];
      double fwhm = 2 * w;
      double area = height * w * Math.Sqrt(1 / (Math.Pow(2, 1 / m) - 1)) * GammaRelated.Beta(m - 0.5, 0.5);

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
      var m = parameters[3];

      var heightStdDev = cv is null ? 0 : Math.Sqrt(cv[0, 0]);
      var posStdDev = cv is null ? 0 : Math.Sqrt(cv[1, 1]);

      double fwhm = 2 * w;
      double fwhmStdDev = cv is null ? 0 : 2 * Math.Sqrt(cv[2, 2]);

      double area = height * w * Math.Sqrt(1 / (Math.Pow(2, 1 / m) - 1)) * GammaRelated.Beta(m - 0.5, 0.5);
      double areaStdDev = 0;

      if (cv is not null)
      {
        var deriv = new double[NumberOfParametersPerPeak];
        var resVec = VectorMath.ToVector(new double[NumberOfParametersPerPeak]);

        var betaTerm = GammaRelated.Beta(m - 0.5, 0.5);
        var powTerm = Math.Sqrt(Math.Pow(2, 1 / m) - 1);
        double digammaMminus0p5 = SpecialFunctions.DiGamma(m - 0.5);
        double digammaM = SpecialFunctions.DiGamma(m);

        deriv[0] = w * betaTerm / powTerm;
        deriv[1] = 0;
        deriv[2] = height * betaTerm / powTerm;
        deriv[3] = height * w * GammaRelated.Beta(m - 0.5, 1.5) * (Math.Log(2) * Math.Pow(2, 1 / m) + 2 * powTerm * powTerm * m * m * (digammaMminus0p5 - digammaM)) / (m * powTerm * powTerm * powTerm);
        MatrixMath.Multiply(cv, deriv, resVec);
        areaStdDev = SafeSqrt(VectorMath.DotProduct(deriv, resVec));

        /*
        // calculation of the area variance
        double gammaMminus0p5 = GammaRelated.Gamma(m - 0.5);
        double gammaM = GammaRelated.Gamma(m);
        double gammaMplus1 = GammaRelated.Gamma(m + 1);
        double TwoBy1ByM = Math.Pow(2, 1 / m);

        areaVariance =
          (Math.PI * RMath.Pow2(gammaMminus0p5) * (4 * RMath.Pow2(-1 + TwoBy1ByM) * RMath.Pow4(m) * (RMath.Pow2(height) * cv[2, 2] + height * (cv[0, 2] + cv[2, 0]) * w + cv[0, 0] * RMath.Pow2(w)) * RMath.Pow2(gammaM) +
         2 * (-1 + TwoBy1ByM) * height * w * (height * (cv[2, 3] + cv[3, 2]) + (cv[0, 3] + cv[3, 0]) * w) * RMath.Pow2(gammaMplus1) * (TwoBy1ByM * Math.Log(2) + 2 * (-1 + TwoBy1ByM) * RMath.Pow2(m) * (digammaMminus0p5 - digammaM)) +
         RMath.Pow2(height) * cv[3, 3] * RMath.Pow2(w) * RMath.Pow2(gammaM) * RMath.Pow2(TwoBy1ByM * Math.Log(2) + 2 * (-1 + TwoBy1ByM) * RMath.Pow2(m) * (digammaMminus0p5 - digammaM)))) / (4 * RMath.Pow3(-1 + TwoBy1ByM) * RMath.Pow4(gammaMplus1));
        */

      }

      return (pos, posStdDev, area, areaStdDev, height, heightStdDev, fwhm, fwhmStdDev);
    }


  }
}


