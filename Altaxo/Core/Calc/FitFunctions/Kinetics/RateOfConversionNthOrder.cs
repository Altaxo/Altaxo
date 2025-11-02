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
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.Regression.Nonlinear;
using Altaxo.Main;

namespace Altaxo.Calc.FitFunctions.Kinetics
{
  /// <summary>
  /// Represents solutions related to the differential equation y'=k*(1-y)^n with the initial condition y(t0)=0. For the direct solution of this equation, see <see cref="EvaluateConversionRate"/>.
  /// </summary>
  [FitFunctionClass]
  public class RateOfConversionNthOrder : IFitFunctionWithDerivative, IImmutable
  {
    #region Serialization

    /// <summary>
    /// Initial version 2021-07-07.
    /// V1: 2023-01-11 Move from AltaxoBase to AltaxoCore
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Calc.FitFunctions.Kinetics.RateOfConversionNthOrder", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(RateOfConversionNthOrder), 1)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (RateOfConversionNthOrder)obj;
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return new RateOfConversionNthOrder();
      }
    }

    #endregion Serialization

    /// <summary>
    /// Initializes a new instance of the <see cref="RateOfConversionNthOrder"/> class.
    /// </summary>
    public RateOfConversionNthOrder()
    {
    }

    /// <summary>
    /// Creates the fit function.
    /// </summary>
    /// <returns>The fit function.</returns>
    [FitFunctionCreator("RateOfConversionNthOrder", "Kinetics", 1, 1, 4)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.Kinetics.RateOfConversionNthOrder}")]
    public static IFitFunction CreateFitFunction()
    {
      return new RateOfConversionNthOrder();
    }

    /// <summary>
    /// Not functional since this instance is immutable.
    /// </summary>
    public event EventHandler? Changed { add { } remove { } }

    /// <inheritdoc/>
    public int NumberOfIndependentVariables
    {
      get
      {
        return 1;
      }
    }

    /// <inheritdoc/>
    public int NumberOfDependentVariables
    {
      get
      {
        return 1;
      }
    }

    /// <inheritdoc/>
    public int NumberOfParameters
    {
      get
      {
        return 4;
      }
    }

    /// <inheritdoc/>
    public string IndependentVariableName(int i)
    {
      return "x";
    }

    /// <inheritdoc/>
    public string DependentVariableName(int i)
    {
      return "y";
    }

    /// <inheritdoc/>
    public string ParameterName(int i)
    {
      return i switch
      {
        0 => "t0",
        1 => "A0",
        2 => "k",
        3 => "n",
        _ => throw new ArgumentOutOfRangeException(nameof(i))
      };
    }

    /// <inheritdoc/>
    public double DefaultParameterValue(int i)
    {
      return i switch
      {
        0 => 0,
        1 => 1,
        2 => 1,
        3 => 1,
        _ => throw new ArgumentOutOfRangeException(nameof(i))
      };
    }

    /// <inheritdoc/>
    public IVarianceScaling? DefaultVarianceScaling(int i)
    {
      return null;
    }

    /// <inheritdoc/>
    public void Evaluate(double[] X, double[] P, double[] Y)
    {
      Y[0] = P[1] * EvaluateConversionRate(X[0], P[0], P[2], P[3]);
    }
    public void Evaluate(IROMatrix<double> independent, IReadOnlyList<double> P, IVector<double> FV, IReadOnlyList<bool>? dependentVariableChoice)
    {
      var rowCount = independent.RowCount;
      for (int r = 0; r < rowCount; ++r)
      {
        var x = independent[r, 0];

        FV[r] = P[1] * EvaluateConversionRate(x, P[0], P[2], P[3]);
      }
    }
    /// <inheritdoc/>
    public void EvaluateDerivative(IROMatrix<double> X, IReadOnlyList<double> P, IReadOnlyList<bool>? isParameterFixed, IMatrix<double> DY, IReadOnlyList<bool>? dependentVariableChoice)
    {
      var rowCount = X.RowCount;
      for (int r = 0; r < rowCount; ++r)
      {
        var x = X[r, 0];
        double t0 = P[0];
        double A0 = P[1];
        double k = P[2];
        double n = P[3];

        if (!(x >= t0))
        {
          DY[r, 0] = 0;
          DY[r, 1] = 0;
          DY[r, 2] = 0;
          DY[r, 3] = 0;
        }
        else if (n < 1 && x >= t0 + 1 / (k * (1 - n)))
        {
          DY[r, 0] = 0;
          DY[r, 1] = 0;
          DY[r, 2] = 0;
          DY[r, 3] = 0;
        }
        else
        {
          if (n == 1)
          {
            var term = Math.Exp(k * (t0 - x));
            DY[r, 0] = A0 * term * k * k;
            DY[r, 1] = term * k;
            DY[r, 2] = A0 * term * (1 + k * (t0 - x));
            DY[r, 3] = A0 * term * 0.5 * RMath.Pow2(k) * (2 + k * (t0 - x)) * (t0 - x);
          }
          else
          {
            var term = 1 - k * (n - 1) * (t0 - x);
            var termE = Math.Pow(term, 1 / (1 - n));
            var termEN = Math.Pow(term, n / (1 - n));

            DY[r, 0] = A0 * k * k * n * termE / RMath.Pow2(term);
            DY[r, 1] = k * termEN;
            DY[r, 2] = A0 * termEN * (1 + (k * n * (t0 - x)) / term);
            DY[r, 3] = A0 * k * termEN * (n * (1 / term - 1) + Math.Log(term)) / RMath.Pow2(n - 1);
          }
        }
      }
    }

    /// <inheritdoc/>
    public (IReadOnlyList<double?>? LowerBounds, IReadOnlyList<double?>? UpperBounds) GetParameterBoundariesHardLimit()
    {
      return (null, null);
    }

    /// <inheritdoc/>
    public (IReadOnlyList<double?>? LowerBounds, IReadOnlyList<double?>? UpperBounds) GetParameterBoundariesSoftLimit()
    {
      return (null, null);
    }


    #region Static functions

    /// <summary>
    /// Represents the real solution of the nth order kinetic equation y'=k*(1-y)^n with y[t0]&gt;=0.
    /// </summary>
    /// <param name="x">The independent variable (time).</param>
    /// <param name="t0">Time at which y is zero.</param>
    /// <param name="k">Kinetic constant (must be a positive value).</param>
    /// <param name="n">The order n of the kinetics equation.</param>
    /// <returns>The value y'(x) of the solution of y'=k*(1-y)^n, presuming that k is nonnegative.</returns>
    public static double EvaluateConversionRate(double x, double t0, double k, double n)
    {
      if (!(k >= 0))
        return double.NaN;

      if (!(x >= t0))
      {
        return 0;
      }
      else if (n < 1 && x >= t0 + 1 / (k * (1 - n)))
      {
        return 0;
      }
      else
      {
        if (n == 1)
          return k * Math.Exp(-k * (x - t0));
        else
          return k * Math.Pow(1 - k * (n - 1) * (t0 - x), n / (1 - n));
      }
    }

    #endregion
  }
}
