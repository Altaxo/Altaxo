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
  /// Fit fuction with one or more Voigt shaped peaks, with a baseline polynomial of variable order.
  /// One term of this function has the parameters area, position, w, and nu.
  /// Sigma and gamma of the usual Voigt function are calculated here as sigma=w*Sqrt(nu/Log(4)), gamma = w*(1-nu), with nu in the range of [0,1].
  /// The FWHM of the function is within 3% equal to 2*w, and the derivatives at nu=0 and nu=1 w.r.t parameter are independent of each other.
  /// </summary>
  [FitFunctionClass]
  public record VoigtAreaParametrizationNu : IFitFunctionWithDerivative, IFitFunctionPeak, IImmutable
  {
    private const string ParameterBaseName0 = "A";
    private const string ParameterBaseName1 = "xc";
    private const string ParameterBaseName2 = "w";
    private const string ParameterBaseName3 = "nu";
    private const int NumberOfParametersPerPeak = 4;

    private const double SqrtLog4 = 1.1774100225154746910115693264597;
    private const double OneBySqrtLog4 = 0.84932180028801904272150283410289;

    public const double C2_FWHM = 0.21669; // Approximation constant for FWHM of Voigt
    private static readonly double C1_FWHM = 1 - Math.Sqrt(C2_FWHM);


    /// <summary>The order of the baseline polynomial.</summary>
    private readonly int _orderOfBaselinePolynomial;
    /// <summary>The order of the polynomial with negative exponents.</summary>
    private readonly int _numberOfTerms;

    #region Serialization

    /// <summary>
    /// 2022-11-02 Initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(VoigtAreaParametrizationNu), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (VoigtAreaParametrizationNu)obj;
        info.AddValue("NumberOfTerms", s._numberOfTerms);
        info.AddValue("OrderOfBackgroundPolynomial", s._orderOfBaselinePolynomial);
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var numberOfTerms = info.GetInt32("NumberOfTerms");
        var orderOfBackgroundPolynomial = info.GetInt32("OrderOfBackgroundPolynomial");
        return new VoigtAreaParametrizationNu(numberOfTerms, orderOfBackgroundPolynomial);
      }
    }

    #endregion Serialization

    public VoigtAreaParametrizationNu()
    {
      _numberOfTerms = 1;
      _orderOfBaselinePolynomial = -1;
    }

    public VoigtAreaParametrizationNu(int numberOfGaussianTerms, int orderOfBackgroundPolynomial)
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

    [FitFunctionCreator("VoigtArea (Parametrization Nu)", "General", 1, 1, NumberOfParametersPerPeak)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.Probability.VoigtAreaParametrizationNu}")]
    public static IFitFunction Create_1_0()
    {
      return new VoigtAreaParametrizationNu(1, 0);
    }

    [FitFunctionCreator("VoigtArea (Parametrization Nu)", "Peaks", 1, 1, NumberOfParametersPerPeak)]
    [FitFunctionCreator("VoigtArea (Parametrization Nu)", "Probability", 1, 1, NumberOfParametersPerPeak)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.Probability.VoigtAreaParametrizationNu}")]
    public static IFitFunction Create_1_M1()
    {
      return new VoigtAreaParametrizationNu(1, -1);
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
        sumTerms += P[j] * Altaxo.Calc.ComplexErrorFunctionRelated.Voigt(X[0] - P[j + 1], P[j + 2] * Math.Sqrt(P[j + 3]) * OneBySqrtLog4, P[j + 2] * (1 - P[j + 3]));
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
          sumTerms += P[j] * Altaxo.Calc.ComplexErrorFunctionRelated.Voigt(x - P[j + 1], P[j + 2] * Math.Sqrt(P[j + 3]) * OneBySqrtLog4, P[j + 2] * (1 - P[j + 3]));
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


    /// <summary>
    /// Not functional because instance is immutable.
    /// </summary>
    public event EventHandler? Changed { add { } remove { } }

    #endregion IFitFunction Members

    /// <inheritdoc/>
    public double[] GetInitialParametersFromHeightPositionAndWidthAtRelativeHeight(double height, double position, double fullWidth, double relativeHeight)
    {
      bool useLorenzLimit = false;

      if (!(relativeHeight > 0 && relativeHeight < 1))
        throw new ArgumentException("RelativeHeight should be in the open interval (0,1)", nameof(relativeHeight));

      if (useLorenzLimit)
      {
        // we calculate at the Lorentz limit (nu==0)
        var w = 0.5 * fullWidth * Math.Sqrt(relativeHeight / (1 - relativeHeight));
        var area = height * w * Math.PI;
        return new double[NumberOfParametersPerPeak] { area, position, w, 0 };
      }
      else // use Gaussian limit
      {
        const double SqrtLog2 = 0.832554611157697756353165; // Math.Sqrt(Math.Log(2))
        const double Sqrt2PiByLog4 = 2.12893403886245235863054; // Math.Sqrt(2*Math.Pi/Math.Log(4))

        var w = 0.5 * fullWidth * SqrtLog2 / Math.Sqrt(-Math.Log(relativeHeight));
        var area = height * w * Sqrt2PiByLog4;
        return new double[] { area, position, w, 1 };
      }
    }

    const double DefaultMinWidth = 1E-81; // Math.Pow(double.Epsilon, 0.25);
    const double DefaultMaxWidth = 1E+77; // Math.Pow(double.MaxValue, 0.25);

    /// <summary>
    /// Gets the parameter boundaries in order to have positive peaks only.
    /// </summary>
    /// <returns></returns>
    public (IReadOnlyList<double?>? LowerBounds, IReadOnlyList<double?>? UpperBounds) GetParameterBoundariesForPositivePeaks(double? minimalPosition = null, double? maximalPosition = null, double? minimalFWHM = null, double? maximalFWHM = null)
    {
      var lowerBounds = new double?[NumberOfParameters];
      var upperBounds = new double?[NumberOfParameters];

      for (int i = 0, j = 0; i < NumberOfTerms; ++i, j += NumberOfParametersPerPeak)
      {
        lowerBounds[j + 0] = 0; // minimal area is 0

        lowerBounds[j + 1] = minimalPosition;
        upperBounds[j + 1] = maximalPosition;

        lowerBounds[j + 2] = minimalFWHM.HasValue ? minimalFWHM / 2 : DefaultMinWidth; // minimal Gaussian width is 0
        upperBounds[j + 2] = maximalFWHM.HasValue ? maximalFWHM / 2 : DefaultMaxWidth;

        lowerBounds[j + 3] = 0; // minimal nu
        upperBounds[j + 3] = 1; // maximal nu
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

        lowerBounds[j + 3] = 0; // minimal nu
        upperBounds[j + 3] = 1; // maximal nu
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

      var (pos, _, area, _, height, _, fwhm, _) = GetPositionAreaHeightFWHMFromSinglePeakParameters(parameters, null);
      return (pos, area, height, fwhm);
    }

    public (double Position, double PositionStdDev, double Area, double AreaStdDev, double Height, double HeightStdDev, double FWHM, double FWHMStdDev)
      GetPositionAreaHeightFWHMFromSinglePeakParameters(IReadOnlyList<double> parameters, IROMatrix<double>? cv)
    {
      const double Sqrt2Pi = 2.5066282746310005024;
      const double SqrtLog4 = 1.1774100225154746910;

      if (parameters is null || parameters.Count != NumberOfParametersPerPeak)
        throw new ArgumentException(nameof(parameters));


      var area = parameters[0];
      var pos = parameters[1];
      var w = parameters[2];
      var nu = parameters[3];
      var sigma = w * Math.Sqrt(nu) * OneBySqrtLog4;
      var gamma = w * (1 - nu);

      var areaStdDev = cv is null ? 0 : SafeSqrt(cv[0, 0]);
      var posStdDev = cv is null ? 0 : SafeSqrt(cv[1, 1]);

      double height;
      double fwhm;
      double heightStdDev = 0;
      double fwhmStdDev = 0;


      // expErfcTerm is normally: Math.Exp(0.5 * RMath.Pow2(gamma / sigma)) * Erfc(gamma / (Math.Sqrt(2) * sigma))
      // but for large gamma/sigma, we need a approximation, because the exp term becomes too large
      double expErfcTermBySqrtNu;

      // for gamma > 20*sigma we need an approximation of the expErfcTerm, since the expTerm will get too large and the Erfc term too small
      // we use a series expansion

      if (nu < (1 / 20d)) // approximation by series expansion is needed in the Lorentz limit
      {
        expErfcTermBySqrtNu = ((((((4.21492418972454262863068) * nu + 0.781454843465470036843478) * nu + 0.269020594012510850443784) * nu + 0.188831848731661377618477) * nu) + 0.677660751603104996618259);
      }
      else // normal case
      {
        var expTerm = Math.Exp(0.5 * RMath.Pow2(gamma / sigma));
        var erfcTerm = Altaxo.Calc.ErrorFunction.Erfc(-gamma / (Math.Sqrt(2) * sigma));
        expErfcTermBySqrtNu = expTerm * (2 - erfcTerm) / Math.Sqrt(nu);
      }

      var bodyheight = expErfcTermBySqrtNu / (w * OneBySqrtLog4 * Sqrt2Pi);
      height = area * bodyheight;


      double fwhmSqrtTerm = Math.Sqrt(C2_FWHM * gamma * gamma + 8 * sigma * sigma * Math.Log(2));
      // fwhm = C1_FWHM * gamma + fwhmSqrtTerm; // this is the approximation formula
      fwhm = 2 * Altaxo.Calc.ComplexErrorFunctionRelated.VoigtHalfWidthHalfMaximum(sigma, gamma); // we use the exact formula for fwhm


      if (cv is not null)
      {
        const double SqrtPi = 1.77245385090551602729817; // Math.Sqrt(Math.PI)
        const double Log2 = 0.693147180559945309417232; // Math.Log(2)
        const double Log2P32 = 0.577082881386139784459964; // Math.Pow(Log2, 1.5)
        const double SqrtPiLog2 = 1.47566462663560588938882; // Math.Sqrt(Math.PI*Math.Log(2))

        var dHeightByDArea = bodyheight;
        var dHeightByDW = -bodyheight * area / w;
        double dHeightByDNu;

        if (nu < (1 / 20d))
        {
          // Series expansion for Lorentzian limit
          dHeightByDNu = ((((((88758.4100339208444038841 * nu - 7108.49617246574864439307) * nu + 641.286392761620402559762) * nu - 66.1018151520036002556775) * nu + 7.91931382144031445585169) * nu - 1.10119171735779477287440) * nu + 0.252727974753276908699454) * nu + 0.0886978390521480859157182;
        }
        else
        {
          // General case
          dHeightByDNu = ((1 + nu) * Log2 -
                                (expErfcTermBySqrtNu) *
                                ((1 - nu * nu) * SqrtPi * Log2P32 + 0.5 * nu * SqrtPiLog2)
                             ) /
                             (nu * nu * Math.PI);
        }

        dHeightByDNu *= (area / w);

        heightStdDev = SafeSqrt(
          cv[0, 0] * RMath.Pow2(dHeightByDArea) +
          cv[2, 2] * RMath.Pow2(dHeightByDW) +
          cv[3, 3] * RMath.Pow2(dHeightByDNu) +
          dHeightByDNu * (cv[0, 3] + cv[3, 0]) * dHeightByDArea +
          dHeightByDW * (cv[0, 2] + cv[2, 0]) * dHeightByDArea +
          dHeightByDNu * (cv[2, 3] + cv[3, 2]) * dHeightByDW
          );


        var dFwhmByDW = 2 * (1 + Math.Sqrt(C2_FWHM) * (-1 + nu) - nu + Math.Sqrt(C2_FWHM * Pow2(1 - nu) + nu));

        var dFwhmByDNu = 2 * w * (-1 + Math.Sqrt(C2_FWHM) + (1 + 2 * C2_FWHM * (-1 + nu)) / (2 * Math.Sqrt(C2_FWHM * Pow2(1 - nu) + nu)));

        fwhmStdDev = SafeSqrt(
                              cv[3, 3] * RMath.Pow2(dFwhmByDNu) +
                              dFwhmByDW * ((cv[2, 3] + cv[3, 2]) * dFwhmByDNu +
                              cv[2, 2] * dFwhmByDW));

      }


      return (pos, posStdDev, area, areaStdDev, height, heightStdDev, fwhm, fwhmStdDev);
    }

    private static double Pow2(double x) => x * x;

    private static double SafeSqrt(double x) => Math.Sqrt(Math.Max(0, x));

  }
}


