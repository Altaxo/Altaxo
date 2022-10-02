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

namespace Altaxo.Calc.FitFunctions.Probability
{
  /// <summary>
  /// Fit fuction with one or more gaussian shaped peaks (bell shape), with a background polynomial
  /// of variable order.
  /// </summary>
  [FitFunctionClass]
  public class GaussAmplitude
        : IFitFunctionWithDerivative, IImmutable, IFitFunctionPeak
  {
    /// <summary>The order of the background polynomial.</summary>
    private readonly int _orderOfBackgroundPolynomial;
    /// <summary>The order of the polynomial with negative exponents.</summary>
    private readonly int _numberOfTerms;

    const string ParameterBaseName0 = "a";
    const string ParameterBaseName1 = "xc";
    const string ParameterBaseName2 = "w";

    #region Serialization

    /// <summary>
    /// 2021-06-07 Initial version 0
    /// 2022-07-18 Moved from AltaxoBase to AltaxoCore
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Calc.FitFunctions.Probability.GaussAmplitude", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(GaussAmplitude), 1)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (GaussAmplitude)obj;
        info.AddValue("NumberOfTerms", s._numberOfTerms);
        info.AddValue("OrderOfBackgroundPolynomial", s._orderOfBackgroundPolynomial);
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
      _orderOfBackgroundPolynomial = -1;
    }

    public GaussAmplitude(int numberOfGaussianTerms, int orderOfBackgroundPolynomial)
    {
      _numberOfTerms = numberOfGaussianTerms;
      _orderOfBackgroundPolynomial = orderOfBackgroundPolynomial;

      if (!(_orderOfBackgroundPolynomial >= -1))
        throw new ArgumentOutOfRangeException("Order of background polynomial has to be greater than or equal to zero, or -1 in order to deactivate it.");
      if (!(_numberOfTerms >= 1))
        throw new ArgumentOutOfRangeException("Number of gaussian terms has to be greater than or equal to 1");

    }

    [FitFunctionCreator("GaussAmplitude", "General", 1, 1, 4)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.Probability.GaussAmplitude}")]
    public static IFitFunction Create_1_0()
    {
      return new GaussAmplitude(1, 0);
    }

    [FitFunctionCreator("GaussAmplitude", "Probability", 1, 1, 4)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.Probability.GaussAmplitude}")]
    public static IFitFunction Create_1_M1()
    {
      return new GaussAmplitude(1, -1);
    }

    /// <summary>
    /// Gets the order of the background polynomial.
    /// </summary>
    public int OrderOfBackgroundPolynomial
    {
      get
      {
        return _orderOfBackgroundPolynomial;
      }
      init
      {
        if (!(_orderOfBackgroundPolynomial >= -1))
          throw new ArgumentOutOfRangeException("Order of background polynomial must either be -1 (to disable it) or >=0", nameof(OrderOfBackgroundPolynomial));
        _orderOfBackgroundPolynomial = value;
      }
    }

    /// <summary>
    /// Creates a new instance with the provided order of the background polynomial.
    /// </summary>
    /// <param name="orderOfBackgroundPolynomial">The order of the background polynomial. If set to -1, the background polynomial will be disabled.</param>
    /// <returns>New instance with the background polynomial of the provided order.</returns>
    public GaussAmplitude WithOrderOfBackgroundPolynomial(int orderOfBackgroundPolynomial)
    {
      if (!(orderOfBackgroundPolynomial >= -1))
        throw new ArgumentOutOfRangeException($"{nameof(orderOfBackgroundPolynomial)} must be greater than or equal to 0, or -1 in order to deactivate it.");

      if (!(_orderOfBackgroundPolynomial == orderOfBackgroundPolynomial))
      {
        return new GaussAmplitude(_numberOfTerms, orderOfBackgroundPolynomial);
      }
      else
      {
        return this;
      }
    }

    /// <summary>
    /// Gets the number of Gaussian terms.
    /// </summary>
    public int NumberOfTerms
    {
      get
      {
        return _numberOfTerms;
      }
      init
      {
        if (!(value >= 1))
          throw new ArgumentOutOfRangeException("Number of terms must be >=1", nameof(NumberOfTerms));
        _numberOfTerms = value;
      }
    }

    /// <summary>
    /// Creates a new instance with the provided number of Gaussian terms.
    /// </summary>
    /// <param name="numberOfGaussianTerms">The number of Gaussian terms (should be greater than or equal to 1).</param>
    /// <returns>New instance with the provided number of Gaussian terms.</returns>
    public GaussAmplitude WithNumberOfTerms(int numberOfGaussianTerms)
    {
      if (!(numberOfGaussianTerms >= 1))
        throw new ArgumentOutOfRangeException($"{nameof(numberOfGaussianTerms)} must be greater than or equal to 1");

      if (!(_numberOfTerms == numberOfGaussianTerms))
      {
        return new GaussAmplitude(numberOfGaussianTerms, _orderOfBackgroundPolynomial);
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
        return _numberOfTerms * 3 + _orderOfBackgroundPolynomial + 1;
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
      int k = i - 3 * _numberOfTerms;
      if (k < 0)
      {
        int j = i / 3;
        return (i % 3) switch
        {
          0 => FormattableString.Invariant($"a{j}"),
          1 => FormattableString.Invariant($"xc{j}"),
          2 => FormattableString.Invariant($"w{j}"),
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
      int k = i - 3 * _numberOfTerms;
      if (k < 0 && i % 3 == 2)
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
      double sumGauss = 0, sumPolynomial = 0;
      for (int i = 0, j = 0; i < _numberOfTerms; ++i, j += 3)
      {
        double x = (X[0] - P[j + 1]) / P[j + 2];
        sumGauss += P[j] * Math.Exp(-0.5 * x * x);
      }

      if (_orderOfBackgroundPolynomial >= 0)
      {
        int offset = 3 * _numberOfTerms;
        // evaluation of terms x^0 .. x^n
        sumPolynomial = P[_orderOfBackgroundPolynomial + offset];
        for (int i = _orderOfBackgroundPolynomial - 1; i >= 0; i--)
        {
          sumPolynomial *= X[0];
          sumPolynomial += P[i + offset];
        }
      }
      Y[0] = sumGauss + sumPolynomial;
    }

    public void Evaluate(IROMatrix<double> independent, IReadOnlyList<double> P, IReadOnlyList<bool>? independentVariableChoice, IVector<double> FV)
    {
      var rowCount = independent.RowCount;
      for (int r = 0; r < rowCount; ++r)
      {
        var x = independent[r, 0];
        // evaluation of gaussian terms
        double sumGauss = 0, sumPolynomial = 0;
        for (int i = 0, j = 0; i < _numberOfTerms; ++i, j += 3)
        {
          double arg = (x - P[j + 1]) / P[j + 2];
          sumGauss += P[j] * Math.Exp(-0.5 * arg * arg);
        }

        if (_orderOfBackgroundPolynomial >= 0)
        {
          int offset = 3 * _numberOfTerms;
          // evaluation of terms x^0 .. x^n
          sumPolynomial = P[_orderOfBackgroundPolynomial + offset];
          for (int i = _orderOfBackgroundPolynomial - 1; i >= 0; i--)
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

    public void EvaluateDerivative(IROMatrix<double> X, IReadOnlyList<double> P, IReadOnlyList<bool>? independentVariableChoice, IMatrix<double> DY)
    {
      var rowCount = X.RowCount;
      for (int r = 0; r < rowCount; ++r)
      {
        var x = X[r, 0];

        // at first, the gaussian terms
        for (int i = 0, j = 0; i < _numberOfTerms; ++i, j += 3)
        {
          var arg = (x - P[j + 1]) / P[j + 2];
          var expTerm = Math.Exp(-0.5 * arg * arg);
          DY[r, j + 0] = expTerm;
          DY[r, j + 1] = expTerm * arg * P[j] / P[j + 2];
          DY[r, j + 2] = expTerm * arg * arg * P[j] / P[j + 2];
        }

        if (_orderOfBackgroundPolynomial >= 0)
        {
          double xn = 1;
          for (int i = 0, j = 3 * _numberOfTerms; i <= _orderOfBackgroundPolynomial; ++i, ++j)
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

    /// <inheritdoc/>
    IFitFunctionPeak IFitFunctionPeak.WithNumberOfTerms(int numberOfTerms)
    {
      return new GaussAmplitude { NumberOfTerms = numberOfTerms, OrderOfBackgroundPolynomial = this.OrderOfBackgroundPolynomial };
    }

    /// <inheritdoc/>
    public string[] ParameterNamesForOnePeak => new string[] { ParameterBaseName0, ParameterBaseName1, ParameterBaseName2 };

    /// <inheritdoc/>
    public (double Position, double Area, double Height, double FWHM) GetPositionAreaHeightFWHMFromSinglePeakParameters(double[] parameters)
    {
      var (pos, _, area, _, height, _, fwhm, _) = GetPositionAreaHeightFWHMFromSinglePeakParameters(parameters, null);
      return (pos, area, height, fwhm);
    }

    static double SafeSqrt(double x) => Math.Sqrt(Math.Max(0, x));

    /// <inheritdoc/>
    public (double Position, double PositionStdDev, double Area, double AreaStdDev, double Height, double HeightStdDev, double FWHM, double FWHMStdDev)
      GetPositionAreaHeightFWHMFromSinglePeakParameters(double[] parameters, IROMatrix<double>? cv)
    {
      const double Sqrt2Pi = 2.5066282746310005024;
      const double SqrtLog4 = 1.1774100225154746910;

      if (parameters == null || parameters.Length != 3)
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
