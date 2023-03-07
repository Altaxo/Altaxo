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

#nullable enable
using System;
using System.Collections.Generic;
using Altaxo.Calc.FitFunctions.Peaks;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.Regression.Nonlinear;
using Altaxo.Main;
using Complex64 = System.Numerics.Complex;

namespace Altaxo.Calc.FitFunctions.Probability
{
  /// <summary>
  /// Fit function with one or more Voigt shaped peaks, with a baseline polynomial
  /// of variable order.
  /// </summary>
  [FitFunctionClass]
  public record VoigtArea : IFitFunctionWithDerivative, IFitFunctionPeak, IImmutable
  {
    private const string ParameterBaseName0 = "A";
    private const string ParameterBaseName1 = "xc";
    private const string ParameterBaseName2 = "w";
    private const string ParameterBaseName3 = "gamma";
    private const int NumberOfParametersPerPeak = 4;



    /// <summary>The order of the baseline polynomial.</summary>
    private readonly int _orderOfBaselinePolynomial;
    /// <summary>The order of the polynomial with negative exponents.</summary>
    private readonly int _numberOfTerms;

    #region Serialization

    /// <summary>
    /// 2021-06-12 Initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(VoigtArea), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (VoigtArea)obj;
        info.AddValue("NumberOfTerms", s._numberOfTerms);
        info.AddValue("OrderOfBackgroundPolynomial", s._orderOfBaselinePolynomial);
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var numberOfTerms = info.GetInt32("NumberOfTerms");
        var orderOfBackgroundPolynomial = info.GetInt32("OrderOfBackgroundPolynomial");
        return new VoigtArea(numberOfTerms, orderOfBackgroundPolynomial);
      }
    }

    #endregion Serialization

    public VoigtArea()
    {
      _numberOfTerms = 1;
      _orderOfBaselinePolynomial = -1;
    }

    public VoigtArea(int numberOfGaussianTerms, int orderOfBackgroundPolynomial)
    {
      _numberOfTerms = numberOfGaussianTerms;
      _orderOfBaselinePolynomial = orderOfBackgroundPolynomial;

      if (!(_orderOfBaselinePolynomial >= -1))
        throw new ArgumentOutOfRangeException("Order of baseline polynomial has to be greater than or equal to zero, or -1 in order to deactivate it.");
      if (!(_numberOfTerms >= 1))
        throw new ArgumentOutOfRangeException("Number of terms has to be greater than or equal to 1");
    }

    /// <inheritdoc/>
    public override string ToString()
    {
      return $"{this.GetType().Name}\r\nNumberOfTerms={NumberOfTerms}\r\nOrderOfBaseline={OrderOfBaselinePolynomial}";
    }

    [FitFunctionCreator("VoigtArea", "General", 1, 1, NumberOfParametersPerPeak)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.Probability.VoigtArea}")]
    public static IFitFunction Create_1_0()
    {
      return new VoigtArea(1, 0);
    }

    [FitFunctionCreator("VoigtArea", "Peaks", 1, 1, NumberOfParametersPerPeak)]
    [FitFunctionCreator("VoigtArea", "Probability", 1, 1, NumberOfParametersPerPeak)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.Probability.VoigtArea}")]
    public static IFitFunction Create_1_M1()
    {
      return new VoigtArea(1, -1);
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
      if (k < 0 && i % NumberOfParametersPerPeak == 2)
        return 1;
      else
        return 0;
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
        sumTerms += P[j] * Altaxo.Calc.ComplexErrorFunctionRelated.Voigt(X[0] - P[j + 1], P[j + 2], P[j + 3]);
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
          sumTerms += P[j] * Altaxo.Calc.ComplexErrorFunctionRelated.Voigt(x - P[j + 1], P[j + 2], P[j + 3]);
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

      var w = 0.5 * width / Math.Sqrt(-2 * Math.Log(relativeHeight));
      var amp = height * w * Math.Sqrt(2 * Math.PI);

      return new double[NumberOfParametersPerPeak] { amp, position, w, 0 };
    }

    /// <summary>
    /// Gets the parameter boundaries in order to have positive peaks only.
    /// </summary>
    /// <returns></returns>
    public (IReadOnlyList<double?>? LowerBounds, IReadOnlyList<double?>? UpperBounds) GetParameterBoundariesForPositivePeaks(double? minimalPosition = null, double? maximalPosition = null, double? minimalFWHM = null, double? maximalFWHM = null)
    {
      var lowerBounds = new double?[NumberOfParameters];

      for (int i = 0, j = 0; i < NumberOfTerms; ++i, j += NumberOfParametersPerPeak)
      {
        lowerBounds[j] = 0; // minimal area is 0
        lowerBounds[j + 2] = 0; // minimal Gaussian width is 0
        lowerBounds[j + 3] = 0; // minimal Lorentzian width
      }

      return (lowerBounds, null);
    }

    /// <inheritdoc/>
    public (IReadOnlyList<double?>? LowerBounds, IReadOnlyList<double?>? UpperBounds) GetParameterBoundariesHardLimit()
    {
      var lowerBounds = new double?[NumberOfParameters];
      var upperBounds = new double?[NumberOfParameters];

      for (int i = 0, j = 0; i < NumberOfTerms; ++i, j += NumberOfParametersPerPeak)
      {
        lowerBounds[j + 2] = 0;
        upperBounds[j + 2] = 1E77;

        lowerBounds[j + 3] = 0;
        upperBounds[j + 3] = 1E77;
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

      var area = parameters[0];
      var pos = parameters[1];
      var height = parameters[0] * Altaxo.Calc.ComplexErrorFunctionRelated.Voigt(0, parameters[2], parameters[3]);
      var fwhm = 2 * Altaxo.Calc.ComplexErrorFunctionRelated.VoigtHalfWidthHalfMaximum(parameters[2], parameters[3]);

      return (pos, area, height, fwhm);
    }

    private static double SafeSqrt(double x) => Math.Sqrt(Math.Max(0, x));

    public (double Position, double PositionStdDev, double Area, double AreaStdDev, double Height, double HeightStdDev, double FWHM, double FWHMStdDev)
      GetPositionAreaHeightFWHMFromSinglePeakParameters(IReadOnlyList<double> parameters, IROMatrix<double>? cv)
    {
      const double Sqrt2Pi = 2.5066282746310005024;
      const double SqrtLog4 = 1.1774100225154746910;

      if (parameters is null || parameters.Count != NumberOfParametersPerPeak)
        throw new ArgumentException(nameof(parameters));


      var area = parameters[0];
      var pos = parameters[1];
      var sigma = Math.Abs(parameters[2]);
      var gamma = Math.Abs(parameters[3]);

      var areaStdDev = cv is null ? 0 : Math.Sqrt(cv[0, 0]);
      var posStdDev = cv is null ? 0 : Math.Sqrt(cv[1, 1]);

      double height;
      double fwhm;
      double heightStdDev = 0;
      double fwhmStdDev = 0;

      double C2 = 0.86676; // Approximation constant for FWHM of Voigt
      double C1 = 2 - Math.Sqrt(C2);

      if (sigma == 0) // limiting case sigma -> 0
      {
        // we have a pure Lorenzian
        height = area / (gamma * Math.PI);
        fwhm = 2 * gamma;

        if (cv is not null)
        {
          heightStdDev = Math.Sqrt(area * area * cv[3, 3] - area * gamma * (cv[0, 3] + cv[3, 0]) + gamma * gamma * cv[0, 0]) / (gamma * gamma * Math.PI);
          fwhmStdDev = 2 * Math.Sqrt(cv[3, 3]);
        }
      }
      else if (gamma == 0)
      {
        // we have a pure Gaussian
        height = area / (Sqrt2Pi * sigma);
        fwhm = 2 * sigma * SqrtLog4;

        if (cv is not null)
        {
          heightStdDev = Math.Sqrt(
                                      RMath.Pow2(area) * (2 * cv[3, 3] + cv[2, 2] * Math.PI + (cv[2, 3] + cv[3, 2]) * Sqrt2Pi) -
                                      area * ((cv[0, 2] + cv[2, 0]) * Math.PI + (cv[0, 3] + cv[3, 0]) * Sqrt2Pi) * sigma +
                                      cv[0, 0] * Math.PI * RMath.Pow2(sigma)
                                    ) / (Math.Sqrt(2) * Math.PI * RMath.Pow2(sigma));

          fwhmStdDev = Math.Sqrt(
                                    RMath.Pow2(C1) * cv[3, 3] +
                                    8 * Math.Log(2) * cv[2, 2] +
                                    2 * C1 * SqrtLog4 * (cv[2, 3] + cv[3, 2])
                                  );
        }
      }
      else
      {
        // expErfcTerm is normally: Math.Exp(0.5 * RMath.Pow2(gamma / sigma)) * Erfc(gamma / (Math.Sqrt(2) * sigma))
        // but for large gamma/sigma, we need a approximation, because the exp term becomes too large
        double expErfcTerm;

        // for gamma > 20*sigma we need an approximation of the expErfcTerm, since the expTerm will get too large and the Erfc term too small
        // we use a series expansion

        if (gamma >= 20 * sigma) // approximation by series expansion is needed
        {
          var x = sigma / gamma;
          var xx = x * x;
          expErfcTerm = Math.Sqrt(2 / Math.PI) * ((((((((2027025 * xx - 135135) * xx + 10395) * xx - 945) * xx + 105) * xx - 15) * xx + 3) * xx - 1) * xx + 1) * x;
        }
        else // normal case
        {
          var expTerm = Math.Exp(0.5 * RMath.Pow2(gamma / sigma));
          var erfcTerm = Altaxo.Calc.ErrorFunction.Erfc(gamma / (Math.Sqrt(2) * sigma));
          expErfcTerm = expTerm * erfcTerm;
        }

        height = area * expErfcTerm / (sigma * Sqrt2Pi);


        double fwhmSqrtTerm = Math.Sqrt(C2 * gamma * gamma + 8 * sigma * sigma * Math.Log(2));
        fwhm = C1 * gamma + fwhmSqrtTerm;


        if (cv is not null)
        {
          var dHeightByDArea = expErfcTerm / (sigma * Sqrt2Pi);
          var dHeightByDSigma = area * (2 * gamma * sigma - expErfcTerm * Sqrt2Pi * (RMath.Pow2(gamma) + RMath.Pow2(sigma))) / RMath.Pow2(Sqrt2Pi * sigma * sigma);
          var dHeightByDGamma = area * (gamma * expErfcTerm / (Sqrt2Pi * RMath.Pow3(sigma)) - 1 / (Math.PI * RMath.Pow2(sigma)));

          heightStdDev = Math.Sqrt(
            cv[0, 0] * RMath.Pow2(dHeightByDArea) +
            dHeightByDGamma * ((cv[0, 3] + cv[3, 0]) * dHeightByDArea +
            cv[3, 3] * dHeightByDGamma) +
            ((cv[0, 2] + cv[2, 0]) * dHeightByDArea +
            (cv[2, 3] + cv[3, 2]) * dHeightByDGamma) * dHeightByDSigma +
            cv[2, 2] * RMath.Pow2(dHeightByDSigma));


          var dFwhmByDSigma = 8 * sigma * Math.Log(2) / fwhmSqrtTerm;
          var dFwhmByDGamma = (2 - Math.Sqrt(C2)) + C2 * gamma / fwhmSqrtTerm;

          fwhmStdDev = Math.Sqrt(
                                cv[3, 3] * RMath.Pow2(dFwhmByDGamma) +
                                dFwhmByDSigma * ((cv[2, 3] + cv[3, 2]) * dFwhmByDGamma +
                                cv[2, 2] * dFwhmByDSigma));

        }

      }
      return (pos, posStdDev, area, areaStdDev, height, heightStdDev, fwhm, fwhmStdDev);
    }

    private static double Pow2(double x) => x * x;

    public void EvaluateDerivative(IROMatrix<double> X, IReadOnlyList<double> parameters, IReadOnlyList<bool>? isParameterFixed, IMatrix<double> DF, IReadOnlyList<bool> dependentVariableChoice)
    {
      const double Sqrt2 = 1.4142135623730950488016887242097;  // Math.Sqrt(2)
      const double Sqrt2Pi = 2.5066282746310005024157652848110; // Math.Sqrt(2*Math.Pi)

      var rows = X.RowCount;
      for (int r = 0; r < rows; ++r)
      {
        var x = X[r, 0];
        for (int i = 0, j = 0; i < _numberOfTerms; ++i, j += NumberOfParametersPerPeak)
        {
          if (isParameterFixed is not null && isParameterFixed[j] && isParameterFixed[j + 1] && isParameterFixed[j + 2] && isParameterFixed[j + 3])
          {
            // avoid calculation of the derivatives, if all parameters of that peak are fixed
            continue;
          }

          var amp = parameters[j + 0];
          var arg = x - parameters[j + 1];
          var sigma = parameters[j + 2];
          var gamma = parameters[j + 3];

          if (!(sigma >= 0 && gamma >= 0 && (sigma + gamma) > 0))
          {
            DF[r, j + 0] = double.NaN;
            DF[r, j + 1] = double.NaN;
            DF[r, j + 2] = double.NaN;
            DF[r, j + 3] = double.NaN;
          }
          else if (sigma == 0) // Pure Lorentzian
          {
            arg /= gamma;
            DF[r, j + 0] = 1 / (Math.PI * gamma * (1 + arg * arg));
            DF[r, j + 1] = amp * (2 * arg / (Math.PI * gamma * gamma * Pow2(1 + arg * arg)));
            DF[r, j + 2] = 0;
            DF[r, j + 3] = amp * ((arg * arg - 1) / (Math.PI * gamma * gamma * Pow2(1 + arg * arg)));
          }
          else // general case including gamma==0
          {
            var z = new Complex64(arg, gamma) / (Sqrt2 * sigma);
            var wOfZ = ComplexErrorFunctionRelated.W_of_z(z);
            var wOfZBySqrt2PiSigma = wOfZ / (Sqrt2Pi * sigma);
            var term1 = (Sqrt2 * Complex64.ImaginaryOne - z * wOfZ * Sqrt2Pi) / (Math.PI * sigma); // Derivative of wOfZBySqrt2PiSigma w.r.t. z

            DF[r, j + 0] = wOfZBySqrt2PiSigma.Real; // Derivative w.r.t. amplitude

            DF[r, j + 1] = -amp * term1.Real / (Sqrt2 * sigma); // Derivative w.r.t. position

            DF[r, j + 2] = -amp * (term1 * z + wOfZBySqrt2PiSigma).Real / sigma; // Derivative w.r.t. sigma

            DF[r, j + 3] = -amp * term1.Imaginary / (Sqrt2 * sigma); // Derivative w.r.t. gamma
          }
        }

        if (_orderOfBaselinePolynomial >= 0)
        {
          double xn = 1;
          for (int i = 0, j = NumberOfParametersPerPeak * _numberOfTerms; i <= _orderOfBaselinePolynomial; ++i, ++j)
          {
            DF[r, j] = xn;
            xn *= x;
          }
        }
      }
    }
  }
}


