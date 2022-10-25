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
  /// Fit fuction with one or more Voigt shaped peaks, with a background polynomial of variable order.
  /// Sigma and gamma of the usual Voigt function are calculated here as sigma=w*Sqrt(nu), gamma = w*(1-nu), with nu in the range of [0,1].
  /// </summary>
  [FitFunctionClass]
  public class VoigtAreaParametrizationSqrtNu2 : IFitFunctionWithDerivative, IFitFunctionPeak, IImmutable
  {
    private const string ParameterBaseName0 = "A";
    private const string ParameterBaseName1 = "xc";
    private const string ParameterBaseName2 = "w";
    private const string ParameterBaseName3 = "nu";
    private const int NumberOfParametersPerPeak = 4;

    private const double SqrtLog4 = 1.1774100225154746910115693264597;
    private const double OneBySqrtLog4 = 0.84932180028801904272150283410289;


    /// <summary>The order of the background polynomial.</summary>
    private readonly int _orderOfBackgroundPolynomial;
    /// <summary>The order of the polynomial with negative exponents.</summary>
    private readonly int _numberOfTerms;

    #region Serialization

    /// <summary>
    /// 2021-06-12 Initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(VoigtAreaParametrizationSqrtNu2), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (VoigtAreaParametrizationSqrtNu2)obj;
        info.AddValue("NumberOfTerms", s._numberOfTerms);
        info.AddValue("OrderOfBackgroundPolynomial", s._orderOfBackgroundPolynomial);
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var numberOfTerms = info.GetInt32("NumberOfTerms");
        var orderOfBackgroundPolynomial = info.GetInt32("OrderOfBackgroundPolynomial");
        return new VoigtAreaParametrizationSqrtNu2(numberOfTerms, orderOfBackgroundPolynomial);
      }
    }

    #endregion Serialization

    public VoigtAreaParametrizationSqrtNu2()
    {
      _numberOfTerms = 1;
      _orderOfBackgroundPolynomial = -1;
    }

    public VoigtAreaParametrizationSqrtNu2(int numberOfGaussianTerms, int orderOfBackgroundPolynomial)
    {
      _numberOfTerms = numberOfGaussianTerms;
      _orderOfBackgroundPolynomial = orderOfBackgroundPolynomial;

      if (!(_orderOfBackgroundPolynomial >= -1))
        throw new ArgumentOutOfRangeException("Order of background polynomial has to be greater than or equal to zero, or -1 in order to deactivate it.");
      if (!(_numberOfTerms >= 1))
        throw new ArgumentOutOfRangeException("Number of terms has to be greater than or equal to 1");

    }

    [FitFunctionCreator("VoigtArea (Parametrization Nu)", "General", 1, 1, NumberOfParametersPerPeak)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.Probability.VoigtAreaParametrizationNu}")]
    public static IFitFunction Create_1_0()
    {
      return new VoigtAreaParametrizationSqrtNu2(1, 0);
    }

    [FitFunctionCreator("VoigtArea (Parametrization Nu)", "Peaks", 1, 1, NumberOfParametersPerPeak)]
    [FitFunctionCreator("VoigtArea (Parametrization Nu)", "Probability", 1, 1, NumberOfParametersPerPeak)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.Probability.VoigtAreaParametrizationNu}")]
    public static IFitFunction Create_1_M1()
    {
      return new VoigtAreaParametrizationSqrtNu2(1, -1);
    }

    /// <summary>
    /// Gets the order of the background polynomial.
    /// </summary>
    public int OrderOfBackgroundPolynomial
    {
      get { return _orderOfBackgroundPolynomial; }
      /*
      init {
        if (!(_orderOfBackgroundPolynomial >= -1))
          throw new ArgumentOutOfRangeException("Order of background polynomial must either be -1 (to disable it) or >=0", nameof(OrderOfBackgroundPolynomial));
        _orderOfBackgroundPolynomial = value;
      }
      */
    }



    /// <summary>
    /// Creates a new instance with the provided order of the background polynomial.
    /// </summary>
    /// <param name="orderOfBackgroundPolynomial">The order of the background polynomial. If set to -1, the background polynomial will be disabled.</param>
    /// <returns>New instance with the background polynomial of the provided order.</returns>
    public VoigtAreaParametrizationSqrtNu2 WithOrderOfBackgroundPolynomial(int orderOfBackgroundPolynomial)
    {
      if (!(orderOfBackgroundPolynomial >= -1))
        throw new ArgumentOutOfRangeException($"{nameof(orderOfBackgroundPolynomial)} must be greater than or equal to 0, or -1 in order to deactivate it.");

      if (!(_orderOfBackgroundPolynomial == orderOfBackgroundPolynomial))
      {
        return new VoigtAreaParametrizationSqrtNu2(_numberOfTerms, orderOfBackgroundPolynomial);
      }
      else
      {
        return this;
      }
    }

    /// <summary>
    /// Gets the number of Voigt terms.
    /// </summary>
    public int NumberOfTerms => _numberOfTerms;

    /// <summary>
    /// Creates a new instance with the provided number of Lorentzian (Cauchy) terms.
    /// </summary>
    /// <param name="numberOfTerms">The number of Lorentzian (Cauchy) terms (should be greater than or equal to 1).</param>
    /// <returns>New instance with the provided number of Lorentzian (Cauchy) terms.</returns>
    public VoigtAreaParametrizationSqrtNu2 WithNumberOfTerms(int numberOfTerms)
    {
      if (!(numberOfTerms >= 1))
        throw new ArgumentOutOfRangeException($"{nameof(numberOfTerms)} must be greater than or equal to 1");

      if (!(_numberOfTerms == numberOfTerms))
      {
        return new VoigtAreaParametrizationSqrtNu2(numberOfTerms, _orderOfBackgroundPolynomial);
      }
      else
      {
        return this;
      }
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
        return _numberOfTerms * NumberOfParametersPerPeak + _orderOfBackgroundPolynomial + 1;
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
        sumTerms += P[j] * Altaxo.Calc.ComplexErrorFunctionRelated.Voigt(X[0] - P[j + 1], P[j + 2] * Math.Sqrt(P[j + 3]) * OneBySqrtLog4, P[j + 2] * (1 - P[j + 3]));
      }

      if (_orderOfBackgroundPolynomial >= 0)
      {
        int offset = NumberOfParametersPerPeak * _numberOfTerms;
        // evaluation of terms x^0 .. x^n
        sumPolynomial = P[_orderOfBackgroundPolynomial + offset];
        for (int i = _orderOfBackgroundPolynomial - 1; i >= 0; i--)
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
          sumTerms += P[j] * Altaxo.Calc.ComplexErrorFunctionRelated.Voigt(x - P[j + 1], P[j + 2] * Math.Sqrt(P[j + 3]) * OneBySqrtLog4, P[j + 2] * (1 - P[j + 3]));
        }

        if (_orderOfBackgroundPolynomial >= 0)
        {
          int offset = NumberOfParametersPerPeak * _numberOfTerms;
          // evaluation of terms x^0 .. x^n
          sumPolynomial = P[_orderOfBackgroundPolynomial + offset];
          for (int i = _orderOfBackgroundPolynomial - 1; i >= 0; i--)
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

    /// <inheritdoc/>
    IFitFunctionPeak IFitFunctionPeak.WithNumberOfTerms(int numberOfTerms)
    {
      return new VoigtAreaParametrizationSqrtNu2(numberOfTerms, this.OrderOfBackgroundPolynomial);
    }

    /// <summary>
    /// Gets the parameter boundaries in order to have positive peaks only.
    /// </summary>
    /// <returns></returns>
    public (IReadOnlyList<double?>? LowerBounds, IReadOnlyList<double?>? upperBounds) GetParameterBoundariesForPositivePeaks(double? minimalPosition = null, double? maximalPosition = null, double? minimalFWHM = null, double? maximalFWHM = null)
    {
      var lowerBounds = new double?[NumberOfParameters];
      var upperBounds = new double?[NumberOfParameters];

      for (int i = 0, j = 0; i < NumberOfTerms; ++i, j += NumberOfParametersPerPeak)
      {
        lowerBounds[j] = 0; // minimal area is 0
        lowerBounds[j + 2] = Math.Sqrt(double.Epsilon); // minimal Gaussian width is 0
        lowerBounds[j + 3] = 0; // minimal nu
        upperBounds[j + 3] = 1; // maximal nu
      }

      return (lowerBounds, null);
    }

    /// <inheritdoc/>
    public string[] ParameterNamesForOnePeak => new string[] { ParameterBaseName0, ParameterBaseName1, ParameterBaseName2, ParameterBaseName3 };

    /// <inheritdoc/>
    public (double Position, double Area, double Height, double FWHM) GetPositionAreaHeightFWHMFromSinglePeakParameters(double[] parameters)
    {
      if (parameters is null || parameters.Length != NumberOfParametersPerPeak)
        throw new ArgumentException(nameof(parameters));

      var area = parameters[0];
      var pos = parameters[1];
      var sigma = parameters[2] * parameters[3];
      var gamma = parameters[2] * (1 - parameters[3]);
      var height = parameters[0] * Altaxo.Calc.ComplexErrorFunctionRelated.Voigt(0, sigma, gamma);
      var fwhm = 2 * Altaxo.Calc.ComplexErrorFunctionRelated.VoigtHalfWidthHalfMaximum(sigma, gamma);

      return (pos, area, height, fwhm);
    }

    private static double SafeSqrt(double x) => Math.Sqrt(Math.Max(0, x));

    public (double Position, double PositionStdDev, double Area, double AreaStdDev, double Height, double HeightStdDev, double FWHM, double FWHMStdDev)
      GetPositionAreaHeightFWHMFromSinglePeakParameters(double[] parameters, IROMatrix<double>? cv)
    {
      const double Sqrt2Pi = 2.5066282746310005024;
      const double SqrtLog4 = 1.1774100225154746910;

      if (parameters is null || parameters.Length != NumberOfParametersPerPeak)
        throw new ArgumentException(nameof(parameters));


      var area = parameters[0];
      var pos = parameters[1];
      var sigma = Math.Abs(parameters[2] * parameters[3]);
      var gamma = Math.Abs(parameters[2] * (1 - parameters[3]));

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

          var area = parameters[j + 0];
          var arg = x - parameters[j + 1];
          var w = parameters[j + 2];
          var nu = parameters[j + 3];
          var sigma = w * Math.Sqrt(nu) * OneBySqrtLog4;
          var gamma = w * (1 - nu);

          if (!(sigma >= 0 && gamma >= 0 && (sigma + gamma) > 0))
          {
            DF[r, j + 0] = double.NaN;
            DF[r, j + 1] = double.NaN;
            DF[r, j + 2] = double.NaN;
            DF[r, j + 3] = double.NaN;
          }
          else if (nu < 1E-4) // approximately this is a Lorentzian
          {
            const double Log4 = 1.3862943611198906188344642429164;
            arg /= w;
            var onePlusArg2 = 1 + arg * arg;
            var body = 1 / (Math.PI * w * onePlusArg2);
            DF[r, j + 0] = body;
            DF[r, j + 1] = area * body * 2 * arg / (w * onePlusArg2);
            DF[r, j + 2] = -area * body * (1 - arg * arg) / (w * onePlusArg2);
            DF[r, j + 3] = area * body * (arg * arg * (3 - arg * arg * Log4) + Log4 - 1) / (onePlusArg2 * onePlusArg2 * Log4);
          }
          else // general case including nu==1 (which means gamma==0, i.e. pure Gaussian).
          {
            var z = new Complex64(arg, gamma) / (Sqrt2 * sigma);
            var wOfZ = ComplexErrorFunctionRelated.W_of_z(z);
            var body = wOfZ / (Sqrt2Pi * sigma);
            var dbodydz = (Sqrt2 * Complex64.ImaginaryOne - z * wOfZ * Sqrt2Pi) / (Math.PI * sigma); // Derivative of wOfZBySqrt2PiSigma w.r.t. z

            DF[r, j + 0] = body.Real; // Derivative w.r.t. amplitude

            DF[r, j + 1] = -area * dbodydz.Real / (Sqrt2 * sigma); // Derivative w.r.t. position

            DF[r, j + 2] = -area / w * (dbodydz * arg / (Sqrt2 * sigma) + body).Real; // Derivative w.r.t. w

            DF[r, j + 3] = -area / (2 * nu) * ((dbodydz * new Complex64(arg, w * (1 + nu)) / (Sqrt2 * sigma) + body)).Real; // Derivative w.r.t. nu
          }
        }

        if (_orderOfBackgroundPolynomial >= 0)
        {
          double xn = 1;
          for (int i = 0, j = NumberOfParametersPerPeak * _numberOfTerms; i <= _orderOfBackgroundPolynomial; ++i, ++j)
          {
            DF[r, j] = xn;
            xn *= x;
          }
        }
      }
    }
  }
}


