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
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.Regression.Nonlinear;
using Altaxo.Main;

namespace Altaxo.Calc.FitFunctions.Peaks
{
  /// <summary>
  /// Fit fuction with one or more PearsonIV shaped peaks, with a background polynomial
  /// of variable order.
  /// </summary>
  /// <remarks>See <see href="https://en.wikipedia.org/wiki/Pearson_distribution#The_Pearson_type_IV_distribution"/>.</remarks>
  [FitFunctionClass]
  public class PearsonIV : IFitFunction, IFitFunctionPeak, IImmutable
  {
    const string ParameterBaseName0 = "a";
    const string ParameterBaseName1 = "xc";
    const string ParameterBaseName2 = "w";
    const string ParameterBaseName3 = "m";
    const string ParameterBaseName4 = "ν";
    const int NumberOfParametersPerPeak = 5;


    /// <summary>The order of the background polynomial.</summary>
    private readonly int _orderOfBackgroundPolynomial;
    /// <summary>The order of the polynomial with negative exponents.</summary>
    private readonly int _numberOfTerms;

    #region Serialization

    /// <summary>
    /// 2022-07-20 Initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PearsonIV), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PearsonIV)obj;
        info.AddValue("NumberOfTerms", s._numberOfTerms);
        info.AddValue("OrderOfBackgroundPolynomial", s._orderOfBackgroundPolynomial);
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var numberOfTerms = info.GetInt32("NumberOfTerms");
        var orderOfBackgroundPolynomial = info.GetInt32("OrderOfBackgroundPolynomial");
        return new PearsonIV(numberOfTerms, orderOfBackgroundPolynomial);
      }
    }

    #endregion Serialization

    public PearsonIV()
    {
      _numberOfTerms = 1;
      _orderOfBackgroundPolynomial = -1;
    }

    public PearsonIV(int numberOfTerms, int orderOfBackgroundPolynomial)
    {
      _numberOfTerms = numberOfTerms;
      _orderOfBackgroundPolynomial = orderOfBackgroundPolynomial;

      if (!(_orderOfBackgroundPolynomial >= -1))
        throw new ArgumentOutOfRangeException("Order of background polynomial has to be greater than or equal to zero, or -1 in order to deactivate it.");
      if (!(_numberOfTerms >= 1))
        throw new ArgumentOutOfRangeException("Number of terms has to be greater than or equal to 1");

    }

    [FitFunctionCreator("PearsonIV", "Peaks", 1, 1, NumberOfParametersPerPeak)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.Peaks.PearsonIV}")]
    public static IFitFunction Create_1_M1()
    {
      return new PearsonIV(1, -1);
    }

    /// <summary>
    /// Gets the order of the background polynomial.
    /// </summary>
    public int OrderOfBackgroundPolynomial
    {
      get { return _orderOfBackgroundPolynomial; }
    }



    /// <summary>
    /// Creates a new instance with the provided order of the background polynomial.
    /// </summary>
    /// <param name="orderOfBackgroundPolynomial">The order of the background polynomial. If set to -1, the background polynomial will be disabled.</param>
    /// <returns>New instance with the background polynomial of the provided order.</returns>
    public PearsonIV WithOrderOfBackgroundPolynomial(int orderOfBackgroundPolynomial)
    {
      if (!(orderOfBackgroundPolynomial >= -1))
        throw new ArgumentOutOfRangeException($"{nameof(orderOfBackgroundPolynomial)} must be greater than or equal to 0, or -1 in order to deactivate it.");

      if (!(_orderOfBackgroundPolynomial == orderOfBackgroundPolynomial))
      {
        return new PearsonIV(_numberOfTerms, orderOfBackgroundPolynomial);
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
    public PearsonIV WithNumberOfTerms(int numberOfTerms)
    {
      if (!(numberOfTerms >= 1))
        throw new ArgumentOutOfRangeException($"{nameof(numberOfTerms)} must be greater than or equal to 1");

      if (!(_numberOfTerms == numberOfTerms))
      {
        return new PearsonIV(numberOfTerms, _orderOfBackgroundPolynomial);
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
        double x = (X[0] - P[j + 1]) / P[j + 2];
        sumTerms += P[j] * Math.Pow(1 + RMath.Pow2(x), -P[j + 3]) * Math.Exp(-P[j + 4] * Math.Atan(x));
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

    /// <inheritdoc/>
    IFitFunctionPeak IFitFunctionPeak.WithNumberOfTerms(int numberOfTerms)
    {
      return new PearsonIV(numberOfTerms, this.OrderOfBackgroundPolynomial);
    }

    /// <inheritdoc/>
    public string[] ParameterNamesForOnePeak => new string[] { ParameterBaseName0, ParameterBaseName1, ParameterBaseName2, ParameterBaseName3, ParameterBaseName4 };

    /*
    /// <summary>
    /// Gets the half width half maximum of a given side of the peak.
    /// </summary>
    /// <param name="w">The width parameter.</param>
    /// <param name="m">The m parameter.</param>
    /// <param name="v">The v parameter.</param>
    /// <param name="rightSide">If set to <c>true</c>, the HWHM of the right side of the peak is determined; otherwise, the HWMH of the left side is determined.</param>
    /// <returns></returns>
    public static double GetHWHM1(double w, double m, double v, bool rightSide)
    {
      w = Math.Abs(w);
      var sign = rightSide ? 1 : -1;
      double zmax = -v / (2 * m);
      double ymax = Math.Pow(1 + zmax*zmax, -m) * Math.Exp(-v * Math.Atan(zmax));
      double ymaxHalf = ymax / 2;

      var zapp = zmax + sign* Math.Sqrt(Math.Pow(2, m) - 1);
      double y = Math.Pow(1 + zapp * zapp, -m) * Math.Exp(-v * Math.Atan(zapp));
    }
    */

    /// <summary>
    /// Gets the half width half maximum of a given side of the peak.
    /// </summary>
    /// <param name="w">The width parameter.</param>
    /// <param name="m">The m parameter.</param>
    /// <param name="v">The v parameter.</param>
    /// <param name="rightSide">If set to <c>true</c>, the HWHM of the right side of the peak is determined; otherwise, the HWMH of the left side is determined.</param>
    /// <returns></returns>
    public static double GetHWHM(double w, double m, double v, bool rightSide)
    {
      w = Math.Abs(w);
      var sign = rightSide ? 1 : -1;
      double xmax = -v * w / (2 * m);
      double ymax = Math.Pow(1 + RMath.Pow2(v / (2 * m)), -m) * Math.Exp(-v * Math.Atan(-v / (2 * m)));
      double ymaxHalf = ymax / 2;

      // goto the left in steps of w, until the amplitude falls below ymaxHalf
      double xNear = xmax;
      double xFar;
      for (xFar = xmax + sign*w; ; xFar += sign * w)
      {
        var y = Math.Pow(1 + RMath.Pow2(xFar / w), -m) * Math.Exp(-v * Math.Atan(xFar / w));
        if (y < ymaxHalf)
          break;
        else
          xNear = xFar;
      }

      if (xNear > xFar)
      {
        (xNear, xFar) = (xFar, xNear);
      }

      // now make a bisection
      var rootFinder = new Altaxo.Calc.RootFinding.BisectionRootFinder(x => ymaxHalf - Math.Pow(1 + RMath.Pow2(x / w), -m) * Math.Exp(-v * Math.Atan(x / w)));
      var xfound = rootFinder.Solve(xNear, xFar, false);
      return Math.Abs(xfound - xmax);
    }

    /// <inheritdoc/>
    public (double Position, double Area, double Height, double FWHM) GetPositionAreaHeightFWHMFromSinglePeakParameters(double[] parameters)
    {
      if (parameters is null || parameters.Length != NumberOfParametersPerPeak)
        throw new ArgumentException(nameof(parameters));

      var amp = parameters[0];
      var loc = parameters[1];
      var w = parameters[2];
      var m = parameters[3];
      var v = parameters[4];

      var xmax = loc - v * w / (2 * m);
      var ymax = amp * Math.Pow(1 + RMath.Pow2(-v / (2 * m)), -m) * Math.Exp(-v * Math.Atan(-v / (2 * m)));


      var area = (amp * w * Altaxo.Calc.GammaRelated.Beta(m - 0.5, 0.5)) /
                (Altaxo.Calc.GammaRelated.Gamma(new Altaxo.Calc.Complex(m, v / 2)) / Altaxo.Calc.GammaRelated.Gamma(m)).GetModulusSquared();
      var pos = xmax;
      var height = ymax;
      var fwhm = GetHWHM(w, m, v, true) + GetHWHM(w, m, v, false);

      return (pos, area, height, fwhm);
    }



    static double SafeSqrt(double x) => Math.Sqrt(Math.Max(0, x));

    public (double Position, double PositionVariance, double Area, double AreaVariance, double Height, double HeightVariance, double FWHM, double FWHMVariance)
      GetPositionAreaHeightFWHMFromSinglePeakParameters(double[] parameters, IROMatrix<double> cv)
    {
      if (parameters is null || parameters.Length != NumberOfParametersPerPeak)
        throw new ArgumentException(nameof(parameters));

      var amp = parameters[0];
      var loc = parameters[1];
      var w = parameters[2];
      var m = parameters[3];
      var v = parameters[4];

      var xmax = loc - v * w / (2 * m);
      var ymax = amp * Math.Pow(1 + RMath.Pow2(-v / (2 * m)), -m) * Math.Exp(-v * Math.Atan(-v / (2 * m)));


      var area = (amp * w * Altaxo.Calc.GammaRelated.Beta(m - 0.5, 0.5)) /
                (Altaxo.Calc.GammaRelated.Gamma(new Altaxo.Calc.Complex(m, v / 2)) / Altaxo.Calc.GammaRelated.Gamma(m)).GetModulusSquared();
      var pos = xmax;
      var height = ymax;
      var fwhm = GetHWHM(w, m, v, true) + GetHWHM(w, m, v, false);

      double posVariance = 0, areaVariance = 0, heightVariance = 0, fwhmVariance = 0;

      if (cv is not null)
      {
        var deriv = new double[5];
        var resVec = new DoubleVector(5);

        // PositionVariance
        deriv[0] = 0;
        deriv[1] = 1;
        deriv[2] = -v / (2 * m);
        deriv[3] = v * w / (2 * m * m);
        deriv[4] = -w / (2 * m);
        MatrixMath.Multiply(cv, deriv, resVec);
        posVariance = SafeSqrt(VectorMath.DotProduct(deriv, resVec));

        var expTerm = Math.Exp(-v * Math.Atan(-v / (2 * m)));
        var powTerm = Math.Pow(1 + RMath.Pow2(-v / (2 * m)), -m);

        // Height variance
        deriv[0] = powTerm * expTerm;
        deriv[1] = 0;
        deriv[2] = 0;
        deriv[3] = amp * powTerm * expTerm * 2 * v * v * (v - m);
        deriv[4] = amp * powTerm * expTerm * (2 * (m - v) * v + (4 * m * m + v * v) * (-Math.Atan(-v / (2 * m)) - Math.Log(1 + v * v / (4 * m * m)))) / (4 * m * m + v * v);
        MatrixMath.Multiply(cv, deriv, resVec);
        heightVariance = SafeSqrt(VectorMath.DotProduct(deriv, resVec));

        // Area variance
        deriv[0] = GetArea(1, pos, w, m, v);
        deriv[1] = 0;
        double absDelta = w * 1E-5;
        deriv[2] = (GetArea(amp, pos, w + absDelta, m, v) - GetArea(amp, pos, w - absDelta, m, v)) / (2 * absDelta);
        absDelta = m * 1E-5;
        deriv[3] = (GetArea(amp, pos, w, m + absDelta, v) - GetArea(amp, pos, w, m - absDelta, v)) / (2 * absDelta);
        absDelta = v == 0 ? 1E-5 : Math.Abs(v * 1E-5);
        deriv[4] = (GetArea(amp, pos, w, m, v + absDelta) - GetArea(amp, pos, w, m, v - absDelta)) / (2 * absDelta);
        MatrixMath.Multiply(cv, deriv, resVec);
        areaVariance = SafeSqrt(VectorMath.DotProduct(deriv, resVec));

        // FWHM variance
        deriv[0] = 0;
        deriv[1] = 0;
        absDelta = w * 1E-5;
        deriv[2] = (GetFWHM(amp, pos, w, m + absDelta, v) - GetFWHM(amp, pos, w, m - absDelta, v)) / (2 * absDelta);
        absDelta = m * 1E-5;
        deriv[3] = (GetFWHM(amp, pos, w, m + absDelta, v) - GetFWHM(amp, pos, w, m - absDelta, v)) / (2 * absDelta);
        absDelta = v == 0 ? 1E-5 : Math.Abs(v * 1E-5);
        deriv[4] = (GetFWHM(amp, pos, w, m, v + absDelta) - GetFWHM(amp, pos, w, m, v - absDelta)) / (2 * absDelta);
        MatrixMath.Multiply(cv, deriv, resVec);
        fwhmVariance = SafeSqrt(VectorMath.DotProduct(deriv, resVec));
      }

      return (pos, posVariance, area, areaVariance, height, heightVariance, fwhm, fwhmVariance);
    }

    private double GetArea(double amp, double pos, double w, double m, double v)
    {
      return (amp * w * GammaRelated.Beta(m - 0.5, 0.5)) /
          (GammaRelated.Gamma(new Altaxo.Calc.Complex(m, v / 2)) /GammaRelated.Gamma(m)).GetModulusSquared();
    }

    private double GetFWHM(double amp, double pos, double w, double m, double v)
    {
      return GetHWHM(w, m, v, true) + GetHWHM(w, m, v, false);
    }
  }
}


