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
  /// Fit fuction with one or more PearsonVII shaped peaks, with a background polynomial
  /// of variable order.
  /// </summary>
  [FitFunctionClass]
  public class PearsonVIIAmplitude : IFitFunction, IFitFunctionPeak, IImmutable
  {
    private const string ParameterBaseName0 = "a";
    private const string ParameterBaseName1 = "xc";
    private const string ParameterBaseName2 = "w";
    private const string ParameterBaseName3 = "m";
    const int NumberOfParametersPerPeak = 4;


    /// <summary>The order of the background polynomial.</summary>
    private readonly int _orderOfBackgroundPolynomial;
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
        info.AddValue("OrderOfBackgroundPolynomial", s._orderOfBackgroundPolynomial);
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
      _orderOfBackgroundPolynomial = -1;
    }

    public PearsonVIIAmplitude(int numberOfTerms, int orderOfBackgroundPolynomial)
    {
      _numberOfTerms = numberOfTerms;
      _orderOfBackgroundPolynomial = orderOfBackgroundPolynomial;

      if (!(_orderOfBackgroundPolynomial >= -1))
        throw new ArgumentOutOfRangeException("Order of background polynomial has to be greater than or equal to zero, or -1 in order to deactivate it.");
      if (!(_numberOfTerms >= 1))
        throw new ArgumentOutOfRangeException("Number of terms has to be greater than or equal to 1");

    }

    [FitFunctionCreator("PearsonVIIAmplitude", "Peaks", 1, 1, NumberOfParametersPerPeak)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.Peaks.PearsonVIIAmplitude}")]
    public static IFitFunction Create_1_M1()
    {
      return new PearsonVIIAmplitude(1, -1);
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
    public PearsonVIIAmplitude WithOrderOfBackgroundPolynomial(int orderOfBackgroundPolynomial)
    {
      if (!(orderOfBackgroundPolynomial >= -1))
        throw new ArgumentOutOfRangeException($"{nameof(orderOfBackgroundPolynomial)} must be greater than or equal to 0, or -1 in order to deactivate it.");

      if (!(_orderOfBackgroundPolynomial == orderOfBackgroundPolynomial))
      {
        return new PearsonVIIAmplitude(_numberOfTerms, orderOfBackgroundPolynomial);
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
    public PearsonVIIAmplitude WithNumberOfTerms(int numberOfTerms)
    {
      if (!(numberOfTerms >= 1))
        throw new ArgumentOutOfRangeException($"{nameof(numberOfTerms)} must be greater than or equal to 1");

      if (!(_numberOfTerms == numberOfTerms))
      {
        return new PearsonVIIAmplitude(numberOfTerms, _orderOfBackgroundPolynomial);
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
        return 0; // no background
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

    public void EvaluateMultiple(IROMatrix<double> independent, IReadOnlyList<double> P, IReadOnlyList<bool>? independentVariableChoice, IVector<double> FV)
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

      var w = Math.Abs(0.5 * width * Math.Sqrt(relativeHeight / (1 - relativeHeight)));
      return new double[NumberOfParametersPerPeak] { height, position, w, 1 }; // Parameters for the Lorentz limit
    }

    /// <inheritdoc/>
    IFitFunctionPeak IFitFunctionPeak.WithNumberOfTerms(int numberOfTerms)
    {
      return new PearsonVIIAmplitude(numberOfTerms, this.OrderOfBackgroundPolynomial);
    }

    /// <inheritdoc/>
    public string[] ParameterNamesForOnePeak => new string[] { ParameterBaseName0, ParameterBaseName1, ParameterBaseName2, ParameterBaseName3 };

    /// <inheritdoc/>
    public (double Position, double Area, double Height, double FWHM) GetPositionAreaHeightFWHMFromSinglePeakParameters(double[] parameters)
    {
      if (parameters is null || parameters.Length != NumberOfParametersPerPeak)
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
      GetPositionAreaHeightFWHMFromSinglePeakParameters(double[] parameters, IROMatrix<double> cv)
    {
      if (parameters is null || parameters.Length != NumberOfParametersPerPeak)
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


