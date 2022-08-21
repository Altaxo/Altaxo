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
  /// Fit fuction with one or more Lorentzian shaped peaks (bell shape), with a background polynomial
  /// of variable order. In case you need the probability density function of the Cauchy distribution,
  /// the <see cref="CauchyArea"/> fit function is better suited.
  /// </summary>
  /// <remarks>
  /// Reference: <see href="https://en.wikipedia.org/wiki/Cauchy_distribution"/>
  /// </remarks>
  [FitFunctionClass]
  public class CauchyAmplitude
        : IFitFunctionWithGradient, IFitFunctionPeak, IImmutable
  {
    const string ParameterBaseName0 = "a";
    const string ParameterBaseName1 = "xc";
    const string ParameterBaseName2 = "w";
    const int NumberOfParametersPerPeak = 3;

    /// <summary>The number of Lorentzian (Cauchy) terms.</summary>
    private readonly int _numberOfTerms;

    /// <summary>The order of the background polynomial.</summary>
    private readonly int _orderOfBackgroundPolynomial;

    #region Serialization

    /// <summary>
    /// 2021-06-13 Initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(CauchyAmplitude), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (CauchyAmplitude)obj;
        info.AddValue("NumberOfTerms", s._numberOfTerms);
        info.AddValue("OrderOfBackgroundPolynomial", s._orderOfBackgroundPolynomial);
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var numberOfTerms = info.GetInt32("NumberOfTerms");
        var orderOfBackgroundPolynomial = info.GetInt32("OrderOfBackgroundPolynomial");
        return new CauchyAmplitude(numberOfTerms, orderOfBackgroundPolynomial);
      }
    }

    #endregion Serialization

    public CauchyAmplitude()
    {
      _numberOfTerms = 1;
      _orderOfBackgroundPolynomial = -1;
    }

    public CauchyAmplitude(int numberOfTerms, int orderOfBackgroundPolynomial)
    {
      _numberOfTerms = numberOfTerms;
      _orderOfBackgroundPolynomial = orderOfBackgroundPolynomial;

      if (!(_orderOfBackgroundPolynomial >= -1))
        throw new ArgumentOutOfRangeException("Order of background polynomial has to be greater than or equal to zero, or -1 in order to deactivate it.");
      if (!(_numberOfTerms >= 1))
        throw new ArgumentOutOfRangeException("Number of peak terms has to be greater than or equal to 1");

    }

    [FitFunctionCreator("LorentzianAmplitude", "General", 1, 1, 4)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.Probability.CauchyAmplitude}")]
    public static IFitFunction Create_1_0()
    {
      return new CauchyAmplitude(1, 0);
    }

    /// <summary>
    /// Gets the order of the background polynomial.
    /// </summary>
    public int OrderOfBackgroundPolynomial => _orderOfBackgroundPolynomial;

    /// <summary>
    /// Creates a new instance with the provided order of the background polynomial.
    /// </summary>
    /// <param name="orderOfBackgroundPolynomial">The order of the background polynomial. If set to -1, the background polynomial will be disabled.</param>
    /// <returns>New instance with the background polynomial of the provided order.</returns>
    public CauchyAmplitude WithOrderOfBackgroundPolynomial(int orderOfBackgroundPolynomial)
    {
      if (!(orderOfBackgroundPolynomial >= -1))
        throw new ArgumentOutOfRangeException($"{nameof(orderOfBackgroundPolynomial)} must be greater than or equal to 0, or -1 in order to deactivate it.");

      if (!(_orderOfBackgroundPolynomial == orderOfBackgroundPolynomial))
      {
        return new CauchyAmplitude(_numberOfTerms, orderOfBackgroundPolynomial);
      }
      else
      {
        return this;
      }
    }

    /// <summary>
    /// Gets the number of Cauchy (Lorentzian) terms.
    /// </summary>
    public int NumberOfTerms => _numberOfTerms;

    /// <summary>
    /// Creates a new instance with the provided number of Lorentzian (Cauchy) terms.
    /// </summary>
    /// <param name="numberOfTerms">The number of Lorentzian (Cauchy) terms (should be greater than or equal to 1).</param>
    /// <returns>New instance with the provided number of Lorentzian (Cauchy) terms.</returns>
    public CauchyAmplitude WithNumberOfTerms(int numberOfTerms)
    {
      if (!(numberOfTerms >= 1))
        throw new ArgumentOutOfRangeException($"{nameof(numberOfTerms)} must be greater than or equal to 1");

      if (!(_numberOfTerms == numberOfTerms))
      {
        return new CauchyAmplitude(numberOfTerms, _orderOfBackgroundPolynomial);
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
      // evaluation of terms
      double sumTerms = 0, sumPolynomial = 0;
      for (int i = 0, j = 0; i < _numberOfTerms; ++i, j += NumberOfParametersPerPeak)
      {
        double x = (X[0] - P[j + 1]) / P[j + 2];
        sumTerms += P[j] / (1 + x * x);
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

    public void EvaluateGradient(double[] X, double[] P, double[][] DY)
    {
      // at first, the gaussian terms
      for (int i = 0, j = 0; i < _numberOfTerms; ++i, j += NumberOfParametersPerPeak)
      {
        var x = (X[0] - P[j + 1]) / P[j + 2];
        var term = 1 / (1 + x * x);
        DY[0][j + 0] = term;
        DY[0][j + 1] = term * term * 2 * x * P[j] / P[j + 2];
        DY[0][j + 2] = term * term * 2 * x * x * P[j] / P[j + 2];
      }

      if (_orderOfBackgroundPolynomial >= 0)
      {
        double xn = 1;
        for (int i = 0, j = NumberOfParametersPerPeak * _numberOfTerms; i <= _orderOfBackgroundPolynomial; ++i, ++j)
        {
          DY[0][j] = xn;
          xn *= X[0];
        }
      }
    }

    /// <inheritdoc/>
    public string[] ParameterNamesForOnePeak => new string[] { ParameterBaseName0, ParameterBaseName1, ParameterBaseName2};


    /// <inheritdoc/>
    IFitFunctionPeak IFitFunctionPeak.WithNumberOfTerms(int numberOfTerms)
    {
      return new CauchyAmplitude(numberOfTerms, this.OrderOfBackgroundPolynomial);
    }

    /// <inheritdoc/>
    public double[] GetInitialParametersFromHeightPositionAndWidthAtRelativeHeight(double height, double position, double width, double relativeHeight)
    {
      var result = new double[NumberOfParametersPerPeak];
      result[0] = height;
      result[1] = position;
      result[2] = 0.5*width * Math.Sqrt(relativeHeight / (1 - relativeHeight));
      return result;
    }

    /// <inheritdoc/>
    public (double Position, double Area, double Height, double FWHM) GetPositionAreaHeightFWHMFromSinglePeakParameters(double[] parameters)
    {
      var height = parameters[0];
      var pos = parameters[1];
      var area = height * Math.PI * parameters[2];
      var fwhm = 2 * parameters[2];
      return (pos, area, height, fwhm);
    }

    /// <inheritdoc/>
    public (double Position, double PositionStdDev, double Area, double AreaStdDev, double Height, double HeightStdDev, double FWHM, double FWHMStdDev) GetPositionAreaHeightFWHMFromSinglePeakParameters(double[] parameters, IROMatrix<double>? cv)
    {
      var height = parameters[0];
      var pos = parameters[1];
      var w = parameters[2];
      var area = height * Math.PI * parameters[2];
      var fwhm = 2 * parameters[2];

      double posStdDev = 0, areaStdDev = 0, heightStdDev = 0, fwhmStdDev = 0;
      if(cv is not null)
      {
        heightStdDev  = Math.Sqrt(cv[0, 0]);
        posStdDev  = Math.Sqrt(cv[1, 1]);
        fwhmStdDev  = 2*Math.Sqrt(cv[2, 2]);
        areaStdDev  = Math.PI*Math.Sqrt(height*height*cv[2,2] + height*w*(cv[2,0]+cv[0,2]) + w*w*cv[0,0]);
      }

      return (pos, posStdDev , area, areaStdDev , height, heightStdDev , fwhm, fwhmStdDev );
    }
  }
}
