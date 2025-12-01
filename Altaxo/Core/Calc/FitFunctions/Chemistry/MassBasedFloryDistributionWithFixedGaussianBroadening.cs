#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2025 Dr. Dirk Lellinger
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

using System;
using System.Collections.Generic;
using Altaxo.Calc.FitFunctions.Peaks;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.Regression.Nonlinear;
using Altaxo.Main;

namespace Altaxo.Calc.FitFunctions.Chemistry
{
  /// <summary>
  /// Mass based Flory distribution based on the decadic logarithm of the molecular weight. It designates the mass based distribution of the molecular
  /// weight after a polymerization reaction. The two parameters are the area A and the probability for a terminal reaction tau.
  /// The Flory distribution is broadened by a Gaussian function, whose sigma is not a parameter, but is dependent on the independent variable (molecular weight).
  /// The dependency of sigma is a polynomial function of the decadic logarithm of the molecular weight with fixed polynomial coefficients.
  /// </summary>
  [FitFunctionClass]
  public record MassBasedFloryDistributionWithFixedGaussianBroadening : IFitFunctionWithDerivative, IFitFunctionPeak, IImmutable
  {
    private const double _lnOf10 = 2.3025850929940456840; // Log(10)

    private const string ParameterBaseName0 = "A";
    private const string ParameterBaseName1 = "tau";
    private const int NumberOfParametersPerPeak = 2;

    public double MolecularWeightOfMonomerUnit
    {
      get => field;
      init
      {
        if (value <= 0)
          throw new ArgumentException($"{nameof(MolecularWeightOfMonomerUnit)} should be greater than zero.");
        field = value;
      }
    } = 1;

    /// <summary>
    /// If false, the independent variable is the molecular weight M. If true, the independent variable is log10(M).
    /// The default value is true.
    /// </summary>
    public bool IndependentVariableIsDecadicLogarithm { get; init; } = true;

    private double[] _polynomialCoefficientsForSigma = [0];

    /// <summary>
    /// Gets the polynomial coefficients for sigma. The polynomial is evaluated in the decadic logarithm of the molecular weight.
    /// </summary>
    /// <value>
    /// The polynomial coefficients for sigma.
    /// </value>
    public double[] PolynomialCoefficientsForSigma
    {
      get => (double[])_polynomialCoefficientsForSigma.Clone();
      init
      {
        if (value.Length == 0)
          throw new ArgumentException("Array must not be empty", nameof(PolynomialCoefficientsForSigma));

        _polynomialCoefficientsForSigma = (double[])value.Clone();
      }
    }

    /// <summary>
    /// Gets the accuracy of the evaluation of the fit function. Standard is 1e-3.
    /// </summary>
    /// <value>
    /// The accuracy of the evaluation of the fit function.
    /// </value>
    public double Accuracy { get; init; } = 1e-3;

    #region Serialization

    /// <summary>
    /// 2025-09-25 Initial version
    /// </summary>
    [Serialization.Xml.XmlSerializationSurrogateFor(typeof(MassBasedFloryDistributionWithFixedGaussianBroadening), 0)]
    private class XmlSerializationSurrogate0 : Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (MassBasedFloryDistributionWithFixedGaussianBroadening)obj;
        info.AddValue("NumberOfTerms", s.NumberOfTerms);
        info.AddValue("OrderOfBackgroundPolynomial", s.OrderOfBaselinePolynomial);
        info.AddValue("MolecularWeightOfMonomerUnit", s.MolecularWeightOfMonomerUnit);
        info.AddValue("IndependentVariableIsDecadicLogarithm", s.IndependentVariableIsDecadicLogarithm);
        info.AddArray("PolynomialCoefficientsForSigma", s._polynomialCoefficientsForSigma, s._polynomialCoefficientsForSigma.Length);
      }

      public virtual object Deserialize(object? o, Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var numberOfTerms = info.GetInt32("NumberOfTerms");
        var orderOfBackgroundPolynomial = info.GetInt32("OrderOfBackgroundPolynomial");
        var molecularWeightOfMonomerUnit = info.GetDouble("MolecularWeightOfMonomerUnit");
        var independentVariableIsDecadicLogarithm = info.GetBoolean("IndependentVariableIsDecadicLogarithm");
        info.GetArray("PolynomialCoefficientsForSigma", out double[] polynomialCoefficientsForSigma);
        return new MassBasedFloryDistributionWithFixedGaussianBroadening(numberOfTerms, orderOfBackgroundPolynomial)
        {
          MolecularWeightOfMonomerUnit = molecularWeightOfMonomerUnit,
          IndependentVariableIsDecadicLogarithm = independentVariableIsDecadicLogarithm,
          PolynomialCoefficientsForSigma = polynomialCoefficientsForSigma,
        };
      }
    }

    #endregion Serialization

    public MassBasedFloryDistributionWithFixedGaussianBroadening()
    {
      NumberOfTerms = 1;
      OrderOfBaselinePolynomial = -1;
    }

    public MassBasedFloryDistributionWithFixedGaussianBroadening(int numberOfTerms, int orderOfBackgroundPolynomial)
    {
      NumberOfTerms = numberOfTerms;
      OrderOfBaselinePolynomial = orderOfBackgroundPolynomial;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
      return $"{GetType().Name} NumberOfTerms={NumberOfTerms} OrderOfBaseline={OrderOfBaselinePolynomial}";
    }

    [FitFunctionCreator("Mass based Flory distribution with fixed Gaussian broadening", "Chemistry", 1, 1, NumberOfParametersPerPeak)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.Chemistry.MassBasedFloryDistributionWithFixedGaussianBroadening}")]
    public static IFitFunction Create_1_M1()
    {
      return new MassBasedFloryDistributionWithFixedGaussianBroadening(1, -1);
    }

    /// <summary>
    /// Gets/sets the order of the baseline polynomial.
    /// </summary>
    public int OrderOfBaselinePolynomial
    {
      get => field;
      init
      {
        if (!(value >= -1))
          throw new ArgumentOutOfRangeException(nameof(OrderOfBaselinePolynomial), $"{nameof(OrderOfBaselinePolynomial)} must be greater than or equal to 0, or -1 in order to deactivate it.");
        field = value;
      }
    } = -1;

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
      get => field;
      init
      {
        if (!(value >= 0))
          throw new ArgumentOutOfRangeException(nameof(NumberOfTerms), $"{nameof(NumberOfTerms)} must be greater than or equal to 0");
        field = value;
      }
    } = 1;

    /// <inheritdoc/>
    IFitFunctionPeak IFitFunctionPeak.WithNumberOfTerms(int numberOfTerms)
    {
      return this with { NumberOfTerms = numberOfTerms };
    }

    #region IFitFunction Members

    public int NumberOfIndependentVariables => 1;

    public int NumberOfDependentVariables => 1;

    public int NumberOfParameters => OrderOfBaselinePolynomial + 1 + NumberOfTerms * NumberOfParametersPerPeak;

    public string IndependentVariableName(int i)
    {
      return IndependentVariableIsDecadicLogarithm ? "log10(M)" : "M";
    }

    public string DependentVariableName(int i)
    {
      return "y";
    }

    public string ParameterName(int i)
    {
      var k = i - NumberOfParametersPerPeak * NumberOfTerms;
      if (k < 0)
      {
        var j = i / NumberOfParametersPerPeak;
        return (i % NumberOfParametersPerPeak) switch
        {
          0 => FormattableString.Invariant($"{ParameterBaseName0}{j}"),
          1 => FormattableString.Invariant($"{ParameterBaseName1}{j}"),
          _ => throw new InvalidProgramException()
        };
      }
      else if (k <= OrderOfBaselinePolynomial)
      {
        return FormattableString.Invariant($"b{k}");
      }
      else
      {
        throw new ArgumentOutOfRangeException(nameof(i), $"Parameter index {i} is out of range.");
      }
    }

    public double DefaultParameterValue(int i)
    {
      var k = i - NumberOfParametersPerPeak * NumberOfTerms;
      if (k < 0)
      {
        return (i % NumberOfParametersPerPeak) switch
        {
          0 => 1, // area
          1 => 1, // position
          _ => throw new InvalidProgramException(),
        };
      }
      else if (k <= OrderOfBaselinePolynomial)
      {
        return 0; // no baseline
      }
      else
      {
        throw new ArgumentOutOfRangeException(nameof(i), $"Parameter index {i} is out of range.");
      }
    }

    public IVarianceScaling? DefaultVarianceScaling(int i)
    {
      return null;
    }



    public string[] ParameterNamesForOnePeak => new string[] { ParameterBaseName0, ParameterBaseName1 };

    /// <summary>
    /// Not functional because instance is immutable.
    /// </summary>
    public event EventHandler? Changed { add { } remove { } }


    public (int N, double relativeSigmaEnd) GetNAndRelativeSigmaEnd(double M)
    {
      var log10MCenter = Math.Log10(M);
      var sigmaCenter = Math.Max(0, RMath.EvaluatePolynomOrderAscending(log10MCenter, _polynomialCoefficientsForSigma));
      if (sigmaCenter == 0)
      {
        return (1, 1);
      }
      else
      {
        int Nmin;
        double xc, a;
        double relSigmaEnd = 0;
        switch (Accuracy)
        {
          case <= 9.9E-8:
            {
              relSigmaEnd = 6;
              Nmin = 15; xc = 0.14023621067404723; a = 66.55798223242172;
            }
            break;
          case <= 9.9E-7:
            {
              relSigmaEnd = 5.5;
              Nmin = 13; xc = 0.15538004726171484; a = 55.659311182796955;
            }
            break;
          case <= 9.9E-6:
            {
              relSigmaEnd = 5;
              Nmin = 11; xc = 0.15545005559921243; a = 45.121426153182945;
            }
            break;
          case <= 9.9E-5:
            {
              relSigmaEnd = 4.6;
              Nmin = 11; xc = 0.24527435302734366; a = 36.338748931884766;
            }
            break;
          case <= 9.9E-4:
            {
              relSigmaEnd = 4.2;
              Nmin = 9; xc = 0.21734252929687492; a = 27.35855712890623;
            }
            break;
          default:
            {
              relSigmaEnd = 3.6;
              Nmin = 7; xc = 0.17651916503906245; a = 18.434545898437506;
            }
            break;
        }

        var nd = (sigmaCenter <= xc) ? Nmin : Nmin + (sigmaCenter - xc) * a;
        int N = (int)(1 + 2 * Math.Ceiling((nd - 1) / 2)); // make sure N is odd for symmetry
        return (N, relSigmaEnd);
      }
    }


    public IEnumerable<(double M, double sigma, double log10Delta)> GetMSigmaLog10Delta(double M, int N, double relSigmaEnd)
    {
      var log10MCenter = Math.Log10(M);
      var sigmaCenter = Math.Max(0, RMath.EvaluatePolynomOrderAscending(log10MCenter, _polynomialCoefficientsForSigma));

      if (sigmaCenter == 0)
      {
        yield return (M, 1, 0);
      }
      else if (sigmaCenter > 0)
      {
        double outer = Math.Max(1, (N % 2 == 0) ? N / 2 - 0.5 : (N - 1) / 2); // outer index for symmetry
        for (int i = 0, j = N - 1; i <= j; ++i, --j)
        {
          double log10Delta = ((i - outer) / outer) * sigmaCenter * relSigmaEnd; // decadic logarithm of the Gaussian shift
          var log10M = log10MCenter - log10Delta;
          var sigmaLocal = Math.Max(0, RMath.EvaluatePolynomOrderAscending(log10M, _polynomialCoefficientsForSigma));
          yield return (Math.Pow(10, log10M), sigmaLocal, log10Delta);

          if (i != j)
          {
            log10Delta = ((j - outer) / outer) * sigmaCenter * relSigmaEnd;
            log10M = log10MCenter - log10Delta;
            sigmaLocal = Math.Max(0, RMath.EvaluatePolynomOrderAscending(log10M, _polynomialCoefficientsForSigma));
            yield return (Math.Pow(10, log10M), sigmaLocal, log10Delta);
          }
        }
      }
      else
      {
        throw new InvalidOperationException($"The evaluation of {nameof(sigmaCenter)} resulted in a value of {sigmaCenter}, which is invalid.");
      }
    }


    /// <summary>
    /// Evaluates the mass based Flory distribution in dependency of the molecular mass M.
    /// </summary>
    /// <param name="M">The molecular mass.</param>
    /// <param name="area">The area of the peak.</param>
    /// <param name="tau">The probability for chain termination in every reation step.</param>
    /// <returns>The distribution function. ATTENTION: the area of the distribution is meaningful only if integrated over Log10(M), not M itself!</returns>
    public double GetYOfOneTerm(double M, double area, double tau)
    {
      var (N, relSigmaEnd) = GetNAndRelativeSigmaEnd(M);

      double sumFloryGauss = 0;
      double sumGauss = 0;
      foreach (var (m, sigma, log10Delta) in GetMSigmaLog10Delta(M, N, relSigmaEnd))
      {
        var gauss = Math.Exp(-0.5 * RMath.Pow2(log10Delta / sigma));
        var xx = tau * m / MolecularWeightOfMonomerUnit;
        var flory = _lnOf10 * xx * xx * Math.Exp(-xx);

        sumGauss += gauss;
        sumFloryGauss += flory * gauss;
      }
      return area * sumFloryGauss / sumGauss;
    }



    void IFitFunction.Evaluate(double[] independent, double[] parameters, double[] FV)
    {
      var x = IndependentVariableIsDecadicLogarithm ? Math.Pow(10, independent[0]) : independent[0];

      // evaluation of gaussian terms
      double sumTerms = 0, sumPolynomial = 0;
      for (int i = 0, j = 0; i < NumberOfTerms; ++i, j += NumberOfParametersPerPeak)
      {
        sumTerms += GetYOfOneTerm(x, parameters[j], parameters[j + 1]);
      }

      if (OrderOfBaselinePolynomial >= 0)
      {
        var offset = NumberOfParametersPerPeak * NumberOfTerms;
        // evaluation of terms x^0 .. x^n
        sumPolynomial = parameters[OrderOfBaselinePolynomial + offset];
        for (var i = OrderOfBaselinePolynomial - 1; i >= 0; i--)
        {
          sumPolynomial *= x;
          sumPolynomial += parameters[i + offset];
        }
      }
      FV[0] = sumTerms + sumPolynomial;
    }

    public void Evaluate(IROMatrix<double> independent, IReadOnlyList<double> P, IVector<double> FV, IReadOnlyList<bool>? dependentVariableChoice)
    {
      var rowCount = independent.RowCount;
      for (var r = 0; r < rowCount; ++r)
      {
        var x = IndependentVariableIsDecadicLogarithm ? Math.Pow(10, independent[r, 0]) : independent[r, 0];
        // evaluation of gaussian terms
        double sumTerms = 0, sumPolynomial = 0;
        for (int i = 0, j = 0; i < NumberOfTerms; ++i, j += NumberOfParametersPerPeak)
        {
          sumTerms += GetYOfOneTerm(x, P[j], P[j + 1]);
        }

        if (OrderOfBaselinePolynomial >= 0)
        {
          var offset = NumberOfParametersPerPeak * NumberOfTerms;
          // evaluation of terms x^0 .. x^n
          sumPolynomial = P[OrderOfBaselinePolynomial + offset];
          for (var i = OrderOfBaselinePolynomial - 1; i >= 0; i--)
          {
            sumPolynomial *= x;
            sumPolynomial += P[i + offset];
          }
        }
        FV[r] = sumTerms + sumPolynomial;
      }
    }

    public void EvaluateDerivative(IROMatrix<double> X, IReadOnlyList<double> P, IReadOnlyList<bool>? isParameterFixed, IMatrix<double> DY, IReadOnlyList<bool>? dependentVariableChoice)
    {
      var rowCount = X.RowCount;
      for (var r = 0; r < rowCount; ++r)
      {
        var M = IndependentVariableIsDecadicLogarithm ? Math.Pow(10, X[r, 0]) : X[r, 0];

        // at first, the peak terms
        for (int i = 0, j = 0; i < NumberOfTerms; ++i, j += NumberOfParametersPerPeak)
        {
          if (isParameterFixed is not null && isParameterFixed[j] && isParameterFixed[j + 1])
          {
            continue;
          }
          var area = P[j];
          var tau = P[j + 1];

          double sumFloryGauss = 0;
          double sumGauss = 0;
          double sumFloryGauss_dTau = 0;

          var (N, relSigmaEnd) = GetNAndRelativeSigmaEnd(M);
          foreach (var (m, sigma, log10Delta) in GetMSigmaLog10Delta(M, N, relSigmaEnd))
          {
            var gauss = Math.Exp(-0.5 * RMath.Pow2(log10Delta / sigma));
            var xx = tau * m / MolecularWeightOfMonomerUnit;
            var flory = _lnOf10 * xx * xx * Math.Exp(-xx);

            sumGauss += gauss;
            sumFloryGauss += gauss * flory;
            sumFloryGauss_dTau += gauss * flory * (2 / tau - m / MolecularWeightOfMonomerUnit);
          }

          DY[r, j + 0] = sumFloryGauss / sumGauss;
          DY[r, j + 1] = area * sumFloryGauss_dTau / sumGauss;
        }

        // then, the baseline
        if (OrderOfBaselinePolynomial >= 0)
        {
          double xn = 1;
          for (int i = 0, j = NumberOfParametersPerPeak * NumberOfTerms; i <= OrderOfBaselinePolynomial; ++i, ++j)
          {
            DY[r, j] = xn;
            xn *= M;
          }
        }
      }
    }



    public (IReadOnlyList<double?>? LowerBounds, IReadOnlyList<double?>? UpperBounds) GetParameterBoundariesForPositivePeaks(double? minimalPosition = null, double? maximalPosition = null, double? minimalFWHM = null, double? maximalFWHM = null)
    {
      var lowerBounds = new double?[NumberOfParameters];
      var upperBounds = new double?[NumberOfParameters];

      if (IndependentVariableIsDecadicLogarithm)
      {
        for (int i = 0, j = 0; i < NumberOfTerms; ++i, j += NumberOfParametersPerPeak)
        {
          lowerBounds[j + 0] = 0; // minimal amplitude is 0
          upperBounds[j + 0] = null; // maximal amplitude is not limited
          if (maximalPosition.HasValue)
            lowerBounds[j + 1] = 2 * MolecularWeightOfMonomerUnit / Math.Pow(10, maximalPosition.Value);
          if (minimalPosition.HasValue)
            upperBounds[j + 1] = 2 * MolecularWeightOfMonomerUnit / Math.Pow(10, minimalPosition.Value);
        }
      }
      else
      {
        for (int i = 0, j = 0; i < NumberOfTerms; ++i, j += NumberOfParametersPerPeak)
        {
          lowerBounds[j + 0] = 0; // minimal amplitude is 0
          upperBounds[j + 0] = null; // maximal amplitude is not limited

          if (maximalPosition.HasValue)
            lowerBounds[j + 1] = 2 * MolecularWeightOfMonomerUnit / maximalPosition.Value;
          if (minimalPosition.HasValue)
            upperBounds[j + 1] = 2 * MolecularWeightOfMonomerUnit / minimalPosition.Value;
        }
      }

      return (lowerBounds, upperBounds);
    }

    public (IReadOnlyList<double?>? LowerBounds, IReadOnlyList<double?>? UpperBounds) GetParameterBoundariesHardLimit()
    {
      var lowerBounds = new double?[NumberOfParameters];
      var upperBounds = new double?[NumberOfParameters];

      for (int i = 0, j = 0; i < NumberOfTerms; ++i, j += NumberOfParametersPerPeak)
      {
        lowerBounds[j + 1] = double.Epsilon; // tau must be greater than zero
      }
      return (lowerBounds, upperBounds);
    }

    public (IReadOnlyList<double?>? LowerBounds, IReadOnlyList<double?>? UpperBounds) GetParameterBoundariesSoftLimit()
    {
      return (null, null);
    }

    public double[] GetInitialParametersFromHeightPositionAndWidthAtRelativeHeight(double height, double position, double width, double relativeHeight)
    {
      if (IndependentVariableIsDecadicLogarithm)
      {
        position = Math.Pow(10, position);
      }
      var tau = 2 * MolecularWeightOfMonomerUnit / position;
      var area = height / (_lnOf10 * 4 * Math.Exp(-2));
      return [area, tau];
    }

    public (double Position, double Area, double Height, double FWHM) GetPositionAreaHeightFWHMFromSinglePeakParameters(IReadOnlyList<double> parameters)
    {
      var (position, _, area, _, height, _, fwhm, _) = GetPositionAreaHeightFWHMFromSinglePeakParameters(parameters, null);
      return (position, area, height, fwhm);
    }

    public (double Position, double PositionStdDev, double Area, double AreaStdDev, double Height, double HeightStdDev, double FWHM, double FWHMStdDev) GetPositionAreaHeightFWHMFromSinglePeakParameters(IReadOnlyList<double> parameters, IROMatrix<double>? cv)
    {
      //leftXX and rightXX are ProductLog[-(1/(Sqrt[2]*E))] and ProductLog[-1, -(1/(Sqrt[2]*E))]
      const double leftXX = 0.76124022935440305941; // while the peak maximum is at xx=2, the FWHM is at xx=0.76124022935440305941 and xx=4.1559209002009059020
      const double rightXX = 4.1559209002009059020;
      const double log10Mean = 0.18361290376839959366;

      var area = parameters[0];
      var tau = parameters[1];

      double areaVar = 0;
      double heightVar = 0;
      double positionVar = 0;
      double widthVar = 0;

      var log10FloryPosition = Math.Log10(2 * MolecularWeightOfMonomerUnit / tau);
      var sigma = RMath.EvaluatePolynomOrderAscending(log10FloryPosition, this.PolynomialCoefficientsForSigma);
      var log10Max = log10Mean + (Math.Log10(2) - log10Mean) * 0.5 * ErrorFunction.Erfc((Math.Log10(sigma) + 0.46773301028340636) / (0.3924498314183897 * Math.Sqrt(2)));
      var log10FloryGaussPosition = log10Max + Math.Log10(MolecularWeightOfMonomerUnit / tau);

      var heightFlory = area * _lnOf10 * 4 * Math.Exp(-2);
      var heightGauss = area / (sigma * Math.Sqrt(2 * Math.PI));
      var heightFloryGauss = sigma == 0 ? heightFlory : 1 / Math.Sqrt(1 / RMath.Pow2(heightFlory) + 1 / RMath.Pow2(heightGauss));

      var log10FwhmFlory = Math.Log10(rightXX) - Math.Log10(leftXX);
      var log10FwhmGauss = sigma * 2 * Math.Sqrt(Math.Log(4));
      var log10FwhmFloryGauss = Math.Sqrt(RMath.Pow2(log10FwhmFlory) + RMath.Pow2(log10FwhmGauss));

      if (cv is not null)
      {
        areaVar = Math.Sqrt(cv[0, 0]);
        heightVar = areaVar * _lnOf10 * 4 * Math.Exp(-2);
      }

      if (IndependentVariableIsDecadicLogarithm)
      {
        if (cv is not null)
        {
          positionVar = Math.Sqrt(cv[1, 1]) / (tau * _lnOf10);
          widthVar = 0;
        }

        return (log10FloryGaussPosition, positionVar, area, areaVar, heightFloryGauss, heightVar, log10FwhmFloryGauss, widthVar);
      }
      else
      {
        if (cv is not null)
        {
          positionVar = 2 * MolecularWeightOfMonomerUnit / (tau * tau) * Math.Sqrt(cv[1, 1]);
          widthVar = (rightXX - leftXX) * MolecularWeightOfMonomerUnit / (tau * tau) * Math.Sqrt(cv[1, 1]);
        }

        var log10XLeftFloryCorr = Math.Log10(leftXX) - log10Mean;
        var log10XRightFloryCorr = Math.Log10(rightXX) - log10Mean;
        var log10XLeftRightGauss = 0.5 * sigma * 2 * Math.Sqrt(Math.Log(4));
        var log10XLeft = -Math.Sqrt(RMath.Pow2(log10XLeftFloryCorr) + RMath.Pow2(log10XLeftRightGauss)) + log10Mean;
        var log10XRight = Math.Sqrt(RMath.Pow2(log10XRightFloryCorr) + RMath.Pow2(log10XLeftRightGauss)) + log10Mean;
        return (Math.Pow(10, log10FloryGaussPosition), positionVar, area, areaVar, heightFloryGauss, heightVar, (MolecularWeightOfMonomerUnit / tau) * (Math.Pow(10, log10XRight) - Math.Pow(10, log10XLeft)), widthVar);
      }
    }
    #endregion
  }
}
