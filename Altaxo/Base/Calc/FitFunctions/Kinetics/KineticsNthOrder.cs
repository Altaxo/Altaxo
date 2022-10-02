#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
  /// Represents solutions related to the differential equation y'=-k*y^n. For the direct solution of this equation, see <see cref="CoreSolution"/>.
  /// </summary>
  [FitFunctionClass]
  public class KineticsNthOrder : IFitFunctionWithDerivative, IImmutable
  {
    #region Serialization

    /// <summary>
    /// Initial version 2021-07-07.
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(KineticsNthOrder), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (KineticsNthOrder)obj;
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return new KineticsNthOrder();
      }
    }

    #endregion Serialization

    /// <summary>
    /// Initializes a new instance of the <see cref="KineticsNthOrder"/> class.
    /// </summary>
    public KineticsNthOrder()
    {
    }

    /// <summary>
    /// Creates the fit function.
    /// </summary>
    /// <returns>The fit function.</returns>
    [FitFunctionCreator("KinecticsNthOrder", "Kinetics", 1, 1, 3)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.Kinetics.KineticsNthOrder}")]
    public static IFitFunction CreateFitFunction()
    {
      return new KineticsNthOrder();
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
        return 3;
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
        0 => "y0",
        1 => "k",
        2 => "n",
        _ => throw new InvalidOperationException()
      };
    }

    /// <inheritdoc/>
    public double DefaultParameterValue(int i)
    {
      return i switch
      {
        0 => 1,
        1 => 1,
        2 => 1,
        _ => throw new InvalidOperationException()
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
      Y[0] = CoreSolution(X[0], P[0], P[1], P[2]);
    }
    public void Evaluate(IROMatrix<double> independent, IReadOnlyList<double> P, IReadOnlyList<bool>? independentVariableChoice, IVector<double> FV)
    {
      var rowCount = independent.RowCount;
      for (int r = 0; r < rowCount; ++r)
      {
        var x = independent[r, 0];
        FV[r] = CoreSolution(x, P[0], P[1], P[2]);
      }
    }
    /// <inheritdoc/>
    public void EvaluateDerivative(IROMatrix<double> X, IReadOnlyList<double> P, IReadOnlyList<bool>? independentVariableChoice, IMatrix<double> DY)
    {
      var rowCount = X.RowCount;
      for (int r = 0; r < rowCount; ++r)
      {
        var x = X[r, 0];
        double y0 = P[0];
        double k = P[1];
        double n = P[2];

        if (!(y0 >= 0))
        {
          DY[r, 0] = double.NaN;
          DY[r, 1] = double.NaN;
          DY[r, 2] = double.NaN;
        }
        else
        {
          if (n == 1)
          {
            var term = Math.Exp(-k * x);
            DY[r, 0] = term;
            DY[r, 1] = -term * y0 * x;
            DY[r, 2] = term * 0.5 * k * x * y0 * (k * x - 2 * Math.Log(y0));
          }
          else
          {
            var term = k * (n - 1) * x + Math.Pow(y0, 1 - n);
            var termE = Math.Pow(term, 1 / (1 - n));

            DY[r, 0] = termE / (term * Math.Pow(y0, n));
            DY[r, 1] = -termE * x / term;
            DY[r, 2] = termE * ((k * x - Math.Pow(y0, 1 - n) * Math.Log(y0)) / ((1 - n) * term) + Math.Log(term) / ((1 - n) * (1 - n)));
          }
        }
      }
    }

    #region Static functions

    /// <summary>
    /// Represents the real solution of the nth order kinetic equation y'=-k*y^n with y[0]&gt;=0.
    /// </summary>
    /// <param name="t">Time.</param>
    /// <param name="y0">Starting value of y at t=0.</param>
    /// <param name="k">Kinetic constant.</param>
    /// <param name="order">The order (n in above formula) of the kinetics equation ( has to be nonnegative).</param>
    /// <returns>The solution if y'=-k*y^n, presuming that y0 is nonnegative.</returns>
    public static double CoreSolution(double t, double y0, double k, double order)
    {
      if (!(y0 >= 0))
        return double.NaN; // throw new ArgumentOutOfRangeException("y0 has to be nonnegative");


      if (order == 0)
        return y0 - k * t;
      else if (order == 1)
        return y0 * Math.Exp(-k * t);
      else
        return Math.Pow(Math.Pow(y0, 1 - order) + (order - 1) * k * t, 1 / (1 - order));

    }

    /// <summary>
    /// Represents the solution of a nth order kinetics to the problem of aggregation.
    /// </summary>
    /// <param name="t">Time.</param>
    /// <param name="p0">Volume fraction of aggregating species at time t=0, which is free (i.e. which is at this time not contained inside an aggragate).</param>
    /// <param name="pSample">Total volume fraction of aggregating species in the sample.</param>
    /// <param name="pInsideAggregate">Aggregates are assumed to contain a constant volume fraction of aggregating species. This parameter represents this constant.</param>
    /// <param name="k">Kinetic constant.</param>
    /// <param name="order">Order of the kinetics. Has to be equal or greater than 0.</param>
    /// <returns>The volume fraction of aggregates at time t. At time t=0, this value is <c>(pSample-p0)/pInsideAggregate</c>. For t going to infinity,
    /// this value tends to <c>pSample/pInsideAggregate</c>.</returns>
    /// <remarks>
    /// The kinetic equation for this problem (see <see cref="CoreSolution"/> is formulated with the number
    /// of free aggregating particles as variable x and the number of aggregating particels inside aggregates as the variable y.
    /// The solution was reformulated with volume fractions, using a new kinetic constant scaled by the volume of one aggregating particel.
    /// </remarks>
    public static double AgglomerateConcentrationFromP0AndPInsideAggregate(double t, double p0, double k, double order, double pSample, double pInsideAggregate)
    {
      return (pSample - p0 * CoreSolution(t, p0, k, order)) / pInsideAggregate;
    }

    /// <summary>
    /// Represents the solution of a nth order kinetics to the problem of aggregation.
    /// </summary>
    /// <param name="t">Time.</param>
    /// <param name="pA0">Volume fraction of aggregates at time t=0.</param>
    /// <param name="pSample">Total volume fraction of aggregating species in the sample.</param>
    /// <param name="pAInf">Volume fraction of aggregates at time t=Infinity.</param>
    /// <param name="k">Kinetic constant.</param>
    /// <param name="order">Order of the kinetics. Has to be equal or greater than 0.</param>
    /// <returns>The volume fraction of aggregates at time t. At time t=0, this value is <c>pA0</c>. For t going to infinity,
    /// this value tends to <c>pAInf</c>.</returns>
    /// <remarks>The provided volume fraction of aggregating species <c>pSample</c>is influencing only the rate. It is important only
    /// if you want to compare aggregation processes for sample with different content of aggregating species. If such a comparism is not neccessary,
    /// you can set <c>pSample</c> to 1.
    /// The kinetic equation for this problem (see <see cref="CoreSolution"/> is formulated with the number
    /// of free aggregating particles as variable x and the number of aggregating particels inside aggregates as the variable y.
    /// The solution was reformulated with volume fractions, using a new kinetic constant scaled by the volume of one aggregating particel.
    /// </remarks>
    public static double AgglomerateConcentrationFromPA0AndPAInf(double t, double pA0, double pAInf, double k, double order, double pSample)
    {
      double p0 = pSample * (1 - pA0 / pAInf);
      return (pAInf - (pAInf - pA0) * CoreSolution(t, p0, k, order));
    }

    #endregion
  }
}
