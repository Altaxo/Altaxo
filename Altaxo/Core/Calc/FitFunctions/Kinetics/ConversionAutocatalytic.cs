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
using System.Diagnostics.CodeAnalysis;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.Regression.Nonlinear;
using Altaxo.Main;

namespace Altaxo.Calc.FitFunctions.Kinetics
{
  /// <summary>
  /// Represents solutions related to the differential equation y'=(k1+k2*y^m)(1-y)^n with the initial condition y(t0)=0./>.
  /// </summary>
  [FitFunctionClass]
  public class ConversionAutocatalytic : IFitFunction, IImmutable
  {
    #region Serialization

    /// <summary>
    /// Initial version 2021-07-07.
    /// V1: 2023-01-11 Move from AltaxoBase to AltaxoCore
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Calc.FitFunctions.Kinetics.ConversionAutocatalytic", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ConversionAutocatalytic), 1)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ConversionAutocatalytic)obj;
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return new ConversionAutocatalytic();
      }
    }

    #endregion Serialization

    /// <summary>
    /// Initializes a new instance of the <see cref="ConversionAutocatalytic"/> class.
    /// </summary>
    public ConversionAutocatalytic()
    {
    }

    /// <summary>
    /// Creates the fit function.
    /// </summary>
    /// <returns>The fit function.</returns>
    [FitFunctionCreator("ConversionAutocatalytic", "Kinetics", 1, 1, 6)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.Kinetics.ConversionAutocatalytic}")]
    public static IFitFunction CreateFitFunction()
    {
      return new ConversionAutocatalytic();
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
        return 6;
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
        2 => "k1",
        3 => "k2",
        4 => "m",
        5 => "n",
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
        4 => 1,
        5 => 1,
        _ => throw new ArgumentOutOfRangeException(nameof(i))
      };
    }

    /// <inheritdoc/>
    public IVarianceScaling? DefaultVarianceScaling(int i)
    {
      return null;
    }

    private Altaxo.Calc.Ode.DOP853? _ode = null;
    private double _x_previous_step = double.NaN;
    protected double[] _y0 = new double[1];
    protected Evaluator _evaluator = new Evaluator();
    private IEnumerator<(double x, double[] y)> _solution;


    /// <inheritdoc/>
    public virtual void Evaluate(double[] X, double[] P, double[] Y)
    {
      EvaluateConversion(X, P, Y);
      Y[0] *= P[1];
    }

    public void Evaluate(IROMatrix<double> independent, IReadOnlyList<double> P, IVector<double> FV, IReadOnlyList<bool>? dependentVariableChoice)
    {
      IEnumerable<double> GetXPoints()
      {
        for (int r = 0; r < independent.RowCount; ++r)
        {
          var x = independent[r, 0] - P[0];
          if (x > 0)
            yield return x;
        }
      }

      int rd = 0;

      for (int r = 0; r < independent.RowCount; ++r)
      {
        var x = independent[r, 0] - P[0];
        if (!(x > 0))
        {
          for (int s = 0; s < _y0.Length; ++s)
          {
            if (dependentVariableChoice is null || dependentVariableChoice[s] == true)
              FV[rd++] = _y0[s];
          }
        }
      }

      var ode = new Ode.DOP853();
      ode.Initialize(0, _y0, _evaluator.EvaluateRate);

      var solution = _ode.GetSolutionPointsVolatile(new Ode.OdeMethodOptions()
      {
        AutomaticStepSizeControl = true,
        AbsoluteTolerance = 1E-4,
        RelativeTolerance = 1E-4,
        OptionalSolutionPoints = GetXPoints(),
        IncludeAutomaticStepsInOutput = false,
        IncludeInitialValueInOutput = false,
      }).GetEnumerator();

      while (solution.MoveNext())
      {
        for (int s = 0; s < _y0.Length; ++s)
        {
          if (dependentVariableChoice is null || dependentVariableChoice[s] == true)
            FV[rd++] = solution.Current.Y_volatile[s];
        }
      }
    }
    /// <summary>
    /// Evaluates the conversion rate (without any prefactor). Thus, the resulting value
    /// is the time derivative of the conversion (also without prefactor).
    /// </summary>
    /// <param name="X">The x value.</param>
    /// <param name="P">The parameter array.</param>
    /// <param name="Y">Outpust the y value.</param>
    public virtual void EvaluateConversionRate(double[] X, double[] P, double[] Y)
    {
      if (!(X[0] >= P[0]))
      {
        Y[0] = 0;
      }
      else
      {
        EvaluateConversion(X, P, Y);
        _evaluator.EvaluateRate(X[0], Y, Y);
      }
    }

    /// <summary>
    /// Evaluates the conversion (from 0..1). Thus, the prefactor A0 is not used here.
    /// </summary>
    /// <param name="X">The x value.</param>
    /// <param name="P">The parameter array.</param>
    /// <param name="Y">Outpust the y value.</param>
    public virtual void EvaluateConversion(double[] X, double[] P, double[] Y)
    {
      var x = X[0] - P[0];
      if (!(x >= 0))
      {
        Y[0] = 0;
      }
      else
      {
        bool initRequired = false;

        if (_ode is null)
        {
          initRequired = true;
        }

        if (!_evaluator.IsSameParameterSet(P))
        {
          _evaluator.Initialize(P);
          initRequired = true;
        }
        if (!(x >= _x_previous_step))
        {
          initRequired = true;
        }

        if (initRequired)
        {
          _ode = new Ode.DOP853();
          _ode.Initialize(0, _y0, _evaluator.EvaluateRate);

          _solution = _ode.GetSolutionPointsVolatile(new Ode.OdeMethodOptions()
          {
            AutomaticStepSizeControl = true,
            AbsoluteTolerance = 1E-4,
            RelativeTolerance = 1E-4,

          }).GetEnumerator();

          _x_previous_step = 0;
          try
          {
            _solution.MoveNext();
          }
          catch (Exception)
          {
            Y[0] = double.NaN;
            return;
          }
        }


        // Forward the ode, until x <= _solution.Current.x
        while (x > _solution.Current.x)
        {
          _x_previous_step = _solution.Current.x;
          try
          {
            _solution.MoveNext();
          }
          catch (Exception)
          {
            Y[0] = double.NaN;
            return;
          }
        }
        Y[0] = _ode.GetInterpolatedSolutionPointVolatile(x)[0];
      }
    }


    public class Evaluator
    {
      private double t0, A0, k1, k2, m, n;

      public void Initialize(double[] P)
      {
        t0 = P[0];
        A0 = P[1];
        k1 = P[2];
        k2 = P[3];
        m = P[4];
        n = P[5];
      }

      public bool IsSameParameterSet(double[] P)
      {
        return
        t0 == P[0] &&
        A0 == P[1] &&
        k1 == P[2] &&
        k2 == P[3] &&
        m == P[4] &&
        n == P[5];
      }

      public void EvaluateRate(double x, double[] y, double[] dy)
      {
        double yy = y[0];
        if (!(0 <= yy && yy < 1))
        {
          dy[0] = 0;
        }
        else if (yy == 0 && m < 0)
        {
          dy[0] = float.MaxValue; // limit the slope
        }
        else
        {
          dy[0] = (k1 + k2 * Math.Pow(yy, m)) * Math.Pow(1 - yy, n);
        }
      }

      public double EvaluateRate(double x, double yy)
      {
        double dy;
        if (!(0 <= yy && yy < 1))
        {
          dy = 0;
        }
        else if (yy == 0 && m < 0)
        {
          dy = float.MaxValue; // limit the slope
        }
        else
        {
          dy = (k1 + k2 * Math.Pow(yy, m)) * Math.Pow(1 - yy, n);
        }
        return dy;
      }


      public void EvaluateJacobian(double x, double[] yy, [AllowNull][NotNull] ref IMatrix<double> jac)
      {
        jac ??= CreateMatrix.Dense<double>(1, 1);

        double y = yy[0];

        if (!(0 <= y && y < 1))
          jac[0, 0] = -1;
        else
          jac[0, 0] = Math.Pow(1 - y, n) * (k2 * m * Math.Pow(y, m - 1) + n * (k1 + k2 * Math.Pow(y, m)) / (y - 1));
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
  }
}
