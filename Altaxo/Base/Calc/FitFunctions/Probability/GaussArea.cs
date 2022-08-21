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
  public class GaussArea
        : IFitFunctionWithGradient, IImmutable, IFitFunctionPeak
  {
    /// <summary>The order of the background polynomial.</summary>
    private readonly int _orderOfBackgroundPolynomial;
    /// <summary>The order of the polynomial with negative exponents.</summary>
    private readonly int _numberOfTerms;
    private const string ParameterBaseName0 = "A";
    private const string ParameterBaseName1 = "xc";
    private const string ParameterBaseName2 = "w";

    #region Serialization

    /// <summary>
    /// 2021-06-12 Initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(GaussArea), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (GaussArea)obj;
        info.AddValue("NumberOfTerms", s._numberOfTerms);
        info.AddValue("OrderOfBackgroundPolynomial", s._orderOfBackgroundPolynomial);
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var numberOfTerms = info.GetInt32("NumberOfTerms");
        var orderOfBackgroundPolynomial = info.GetInt32("OrderOfBackgroundPolynomial");
        return new GaussArea(numberOfTerms, orderOfBackgroundPolynomial);
      }
    }

    #endregion Serialization

    public GaussArea()
    {
      _numberOfTerms = 1;
      _orderOfBackgroundPolynomial = -1;
    }

    public GaussArea(int numberOfGaussianTerms, int orderOfBackgroundPolynomial)
    {
      _numberOfTerms = numberOfGaussianTerms;
      _orderOfBackgroundPolynomial = orderOfBackgroundPolynomial;

      if (!(_orderOfBackgroundPolynomial >= -1))
        throw new ArgumentOutOfRangeException("Order of background polynomial has to be greater than or equal to zero, or -1 in order to deactivate it.");
      if (!(_numberOfTerms >= 1))
        throw new ArgumentOutOfRangeException("Number of gaussian terms has to be greater than or equal to 1");

    }

    [FitFunctionCreator("GaussArea", "General", 1, 1, 4)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.Probability.GaussArea}")]
    public static IFitFunction Create_1_0()
    {
      return new GaussArea(1, 0);
    }

    [FitFunctionCreator("GaussArea", "Probability", 1, 1, 4)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.Probability.GaussArea}")]
    public static IFitFunction Create_1_M1()
    {
      return new GaussArea(1, -1);
    }

    /// <summary>
    /// Gets the order of the background polynomial.
    /// </summary>
    public int OrderOfBackgroundPolynomial
    {
      get { return _orderOfBackgroundPolynomial; }
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
    public GaussArea WithOrderOfBackgroundPolynomial(int orderOfBackgroundPolynomial)
    {
      if (!(orderOfBackgroundPolynomial >= -1))
        throw new ArgumentOutOfRangeException($"{nameof(orderOfBackgroundPolynomial)} must be greater than or equal to 0, or -1 in order to deactivate it.");

      if (!(_orderOfBackgroundPolynomial == orderOfBackgroundPolynomial))
      {
        return new GaussArea(_numberOfTerms, orderOfBackgroundPolynomial);
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
          0 => FormattableString.Invariant($"{ParameterBaseName0}{j}"),
          1 => FormattableString.Invariant($"{ParameterBaseName1}{j}"),
          2 => FormattableString.Invariant($"{ParameterBaseName2}{j}"),
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
        sumGauss += P[j] / P[j + 2] * Math.Exp(-0.5 * x * x);
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
      Y[0] = sumGauss / Math.Sqrt(2 * Math.PI) + sumPolynomial;
    }

    /// <summary>
    /// Not functional because instance is immutable.
    /// </summary>
    public event EventHandler? Changed { add { } remove { } }

    #endregion IFitFunction Members

    public void EvaluateGradient(double[] X, double[] P, double[][] DY)
    {
      // at first, the gaussian terms
      for (int i = 0, j = 0; i < _numberOfTerms; ++i, j += 3)
      {
        var x = (X[0] - P[j + 1]) / P[j + 2];
        var expTerm = Math.Exp(-0.5 * x * x) / (P[j + 2] * Math.Sqrt(2 * Math.PI));
        DY[0][j + 0] = expTerm;
        DY[0][j + 1] = expTerm * x * P[j] / P[j + 2];
        DY[0][j + 2] = expTerm * P[j] / P[j + 2] * (x * x - 1);
      }

      if (_orderOfBackgroundPolynomial >= 0)
      {
        double xn = 1;
        for (int i = 0, j = 3 * _numberOfTerms; i <= _orderOfBackgroundPolynomial; ++i, ++j)
        {
          DY[0][j] = xn;
          xn *= X[0];
        }
      }
    }

    /// <inheritdoc/>
    public double[] GetInitialParametersFromHeightPositionAndWidthAtRelativeHeight(double height, double position, double width, double relativeHeight)
    {
      if (!(relativeHeight > 0 && relativeHeight < 1))
        throw new ArgumentException("RelativeHeight should be in the open interval (0,1)", nameof(relativeHeight));

      double w = 0.5 * width / Math.Sqrt(-2 * Math.Log(relativeHeight));
      return new double[] { height * w * Math.Sqrt(2 * Math.PI), position, w };
    }

    /// <inheritdoc/>
    IFitFunctionPeak IFitFunctionPeak.WithNumberOfTerms(int numberOfTerms)
    {
      return new GaussArea { NumberOfTerms = numberOfTerms, OrderOfBackgroundPolynomial = this.OrderOfBackgroundPolynomial };
    }


    /// <inheritdoc/>
    public string[] ParameterNamesForOnePeak => new string[] { ParameterBaseName0, ParameterBaseName1, ParameterBaseName2 };

    /// <inheritdoc/>
    public (double Position, double Area, double Height, double FWHM) GetPositionAreaHeightFWHMFromSinglePeakParameters(double[] parameters)
    {
      var (pos, _, area, _, height, _, fwhm, _) = GetPositionAreaHeightFWHMFromSinglePeakParameters(parameters, null);
      return (pos, area, height, fwhm);
    }

    private static double SafeSqrt(double x) => Math.Sqrt(Math.Max(0, x));

    /// <inheritdoc/>
    public (double Position, double PositionStdDev, double Area, double AreaStdDev, double Height, double HeightStdDev, double FWHM, double FWHMStdDev)
      GetPositionAreaHeightFWHMFromSinglePeakParameters(double[] parameters, IROMatrix<double>? cv)
    {
      const double Sqrt2Pi = 2.5066282746310005024;
      const double SqrtLog4 = 1.1774100225154746910;

      if (parameters == null || parameters.Length != 3)
        throw new ArgumentException(nameof(parameters));

      var area = parameters[0];
      var pos = parameters[1];
      var sigma = parameters[2];

      var height = area / (sigma * Sqrt2Pi);
      var fwhm = sigma * 2 * SqrtLog4;

      double posStdDev = 0, areaStdDev = 0, heightStdDev = 0, fwhmStdDev = 0;

      if (cv is not null)
      {
        areaStdDev = Math.Sqrt(cv[0, 0]);
        posStdDev = Math.Sqrt(cv[1, 1]);
        heightStdDev = SafeSqrt(RMath.Pow2(area) * cv[2, 2] - area * sigma * (cv[0, 2] + cv[2, 0]) + RMath.Pow2(sigma) * cv[0, 0]) / (RMath.Pow2(sigma) * Sqrt2Pi);
        fwhmStdDev = Math.Sqrt(cv[2, 2]) * 2 * SqrtLog4;
      }
      return (pos, posStdDev, area, areaStdDev, height, heightStdDev, fwhm, fwhmStdDev);

    }
  }
}
