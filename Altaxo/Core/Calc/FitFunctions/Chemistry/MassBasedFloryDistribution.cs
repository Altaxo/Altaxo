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
  /// </summary>
  [FitFunctionClass]
  public record MassBasedFloryDistribution : IFitFunctionWithDerivative, IFitFunctionPeak, IImmutable
  {
    private const double lambda = 2.3025850929940456840; // Log(10)

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
    /// Default is false.
    /// </summary>
    public bool IndependentVariableIsDecadicLogarithm { get; init; }

    #region Serialization

    /// <summary>
    /// 2025-09-25 Initial version
    /// </summary>
    [Serialization.Xml.XmlSerializationSurrogateFor(typeof(MassBasedFloryDistribution), 0)]
    private class XmlSerializationSurrogate0 : Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (MassBasedFloryDistribution)obj;
        info.AddValue("NumberOfTerms", s.NumberOfTerms);
        info.AddValue("OrderOfBackgroundPolynomial", s.OrderOfBaselinePolynomial);
        info.AddValue("MolecularWeightOfMonomerUnit", s.MolecularWeightOfMonomerUnit);
        info.AddValue("IndependentVariableIsDecadicLogarithm", s.IndependentVariableIsDecadicLogarithm);
      }

      public virtual object Deserialize(object? o, Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var numberOfTerms = info.GetInt32("NumberOfTerms");
        var orderOfBackgroundPolynomial = info.GetInt32("OrderOfBackgroundPolynomial");
        var molecularWeightOfMonomerUnit = info.GetDouble("MolecularWeightOfMonomerUnit");
        var independentVariableIsDecadicLogarithm = info.GetBoolean("IndependentVariableIsDecadicLogarithm");
        return new MassBasedFloryDistribution(numberOfTerms, orderOfBackgroundPolynomial) { MolecularWeightOfMonomerUnit = molecularWeightOfMonomerUnit, IndependentVariableIsDecadicLogarithm = independentVariableIsDecadicLogarithm };
      }
    }

    #endregion Serialization

    public MassBasedFloryDistribution()
    {
      NumberOfTerms = 1;
      OrderOfBaselinePolynomial = -1;
    }

    public MassBasedFloryDistribution(int numberOfTerms, int orderOfBackgroundPolynomial)
    {
      NumberOfTerms = numberOfTerms;
      OrderOfBaselinePolynomial = orderOfBackgroundPolynomial;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
      return $"{GetType().Name} NumberOfTerms={NumberOfTerms} OrderOfBaseline={OrderOfBaselinePolynomial}";
    }

    [FitFunctionCreator("MassBasedFloryDistribution", "Chemistry", 1, 1, NumberOfParametersPerPeak)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.Peaks.MassBasedFloryDistribution}")]
    public static IFitFunction Create_1_M1()
    {
      return new MassBasedFloryDistribution(1, -1);
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



    /// <summary>
    /// Evaluates the mass based Flory distribution in dependency of the molecular mass M.
    /// </summary>
    /// <param name="M">The molecular mass.</param>
    /// <param name="tau">The probability for chain termination in every reation step.</param>
    /// <param name="molecularMassMonomer">The molecular mass of a monomer unit.</param>
    /// <returns>The distribution function. ATTENTION: the area of the distribution is area only if integrated over Log10(M), not M itself!</returns>
    public static double GetYOfOneTerm(double M, double area, double tau, double molecularMassMonomer)
    {
      var xx = tau * M / molecularMassMonomer;
      return area * lambda * xx * xx * Math.Exp(-xx);
    }

    void IFitFunction.Evaluate(double[] independent, double[] parameters, double[] FV)
    {
      var x = IndependentVariableIsDecadicLogarithm ? Math.Pow(10, independent[0]) : independent[0];

      // evaluation of gaussian terms
      double sumTerms = 0, sumPolynomial = 0;
      for (int i = 0, j = 0; i < NumberOfTerms; ++i, j += NumberOfParametersPerPeak)
      {
        sumTerms += GetYOfOneTerm(x, parameters[j], parameters[j + 1], MolecularWeightOfMonomerUnit);
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
          sumTerms += GetYOfOneTerm(x, P[j], P[j + 1], MolecularWeightOfMonomerUnit);
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
        var x = IndependentVariableIsDecadicLogarithm ? Math.Pow(10, X[r, 0]) : X[r, 0];
        var arg = x / MolecularWeightOfMonomerUnit;


        // at first, the peak terms
        for (int i = 0, j = 0; i < NumberOfTerms; ++i, j += NumberOfParametersPerPeak)
        {
          if (isParameterFixed is not null && isParameterFixed[j] && isParameterFixed[j + 1])
          {
            continue;
          }
          var area = P[j];
          var tau = P[j + 1];

          var argtau = arg * tau;
          var body = lambda * argtau * argtau * Math.Exp(-argtau);

          DY[r, j + 0] = body;
          DY[r, j + 1] = area * body * (2 / tau - arg);
        }

        // then, the baseline
        if (OrderOfBaselinePolynomial >= 0)
        {
          double xn = 1;
          for (int i = 0, j = NumberOfParametersPerPeak * NumberOfTerms; i <= OrderOfBaselinePolynomial; ++i, ++j)
          {
            DY[r, j] = xn;
            xn *= x;
          }
        }
      }
    }

    public double[] GetInitialParametersFromHeightPositionAndWidthAtRelativeHeight(double height, double position, double width, double relativeHeight)
    {
      if (IndependentVariableIsDecadicLogarithm)
      {
        position = Math.Pow(10, position);
      }
      var tau = 2 * MolecularWeightOfMonomerUnit / position;
      var area = height / (lambda * 4 * Math.Exp(-2));
      return [area, tau];
    }

    public (IReadOnlyList<double?>? LowerBounds, IReadOnlyList<double?>? UpperBounds) GetParameterBoundariesForPositivePeaks(double? minimalPosition = null, double? maximalPosition = null, double? minimalFWHM = null, double? maximalFWHM = null)
    {
      var lowerBounds = new double?[NumberOfParameters];
      var upperBounds = new double?[NumberOfParameters];

      for (int i = 0, j = 0; i < NumberOfTerms; ++i, j += NumberOfParametersPerPeak)
      {
        lowerBounds[j + 0] = 0; // minimal amplitude is 0
        upperBounds[j + 0] = null; // maximal amplitude is not limited

        lowerBounds[j + 1] = 2 * MolecularWeightOfMonomerUnit / maximalPosition;
        upperBounds[j + 1] = 2 * MolecularWeightOfMonomerUnit / minimalPosition;
      }

      return (lowerBounds, upperBounds);
    }

    public (IReadOnlyList<double?>? LowerBounds, IReadOnlyList<double?>? UpperBounds) GetParameterBoundariesHardLimit()
    {
      var lowerBounds = new double?[NumberOfParameters];
      var upperBounds = new double?[NumberOfParameters];

      for (int i = 0, j = 0; i < NumberOfTerms; ++i, j += NumberOfParametersPerPeak)
      {
        lowerBounds[j + 1] = double.Epsilon;
      }
      return (lowerBounds, upperBounds);
    }

    public (IReadOnlyList<double?>? LowerBounds, IReadOnlyList<double?>? UpperBounds) GetParameterBoundariesSoftLimit()
    {
      return (null, null);
    }

    public (double Position, double Area, double Height, double FWHM) GetPositionAreaHeightFWHMFromSinglePeakParameters(IReadOnlyList<double> parameters)
    {
      const double leftXX = 0.76124022935440305941; // while the peak maximum is at xx=2, the FWHM is at xx=0.76124022935440305941 and xx=4.1559209002009059020
      const double rightXX = 4.1559209002009059020;

      var area = parameters[0];
      var height = area * lambda * 4 * Math.Exp(-2);
      var position = 2 * MolecularWeightOfMonomerUnit / parameters[1];
      var width = (rightXX - leftXX) * MolecularWeightOfMonomerUnit / parameters[1];

      return (position, area, height, width);
    }

    public (double Position, double PositionStdDev, double Area, double AreaStdDev, double Height, double HeightStdDev, double FWHM, double FWHMStdDev) GetPositionAreaHeightFWHMFromSinglePeakParameters(IReadOnlyList<double> parameters, IROMatrix<double>? cv)
    {
      const double leftXX = 0.76124022935440305941; // while the peak maximum is at xx=2, the FWHM is at xx=0.76124022935440305941 and xx=4.1559209002009059020
      const double rightXX = 4.1559209002009059020;

      var area = parameters[0];
      double areaVar = 0;
      var height = area * lambda * 4 * Math.Exp(-2);
      double heightVar = 0;
      var position = 2 * MolecularWeightOfMonomerUnit / parameters[1];
      double positionVar = 0;
      var width = (rightXX - leftXX) * MolecularWeightOfMonomerUnit / parameters[1];
      double widthVar = 0;

      if (cv is not null)
      {
        areaVar = Math.Sqrt(cv[0, 0]);
        heightVar = areaVar * lambda * 4 * Math.Exp(-2);
        positionVar = 2 * MolecularWeightOfMonomerUnit / (parameters[1] * parameters[1]) * Math.Sqrt(cv[1, 1]);
        widthVar = (rightXX - leftXX) * MolecularWeightOfMonomerUnit / (parameters[1] * parameters[1]) * Math.Sqrt(cv[1, 1]);
      }

      return (position, positionVar, area, areaVar, height, heightVar, width, widthVar);
    }



    #endregion
  }


}
