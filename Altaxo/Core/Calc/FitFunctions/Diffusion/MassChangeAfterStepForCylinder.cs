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
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.Regression.Nonlinear;

namespace Altaxo.Calc.FitFunctions.Diffusion
{
  /// <summary>
  /// Describes the mass change of a infinite long cylinder (with given radius) in a diffusion process after a concentration step change.
  /// </summary>
  /// <remarks>
  /// Ref. [1]: Crank, "The Mathematics of Diffusion", 2nd edition, 1975, Oxford University Press.
  /// </remarks>
  [FitFunctionClass]
  public record MassChangeAfterStepForCylinder : IFitFunction, IFitFunctionWithDerivative, Main.IImmutable
  {
    /// <summary>
    /// The small approximation coefficients.
    /// </summary>
    /// <remarks>They can not be found in Ref.[1], but were evaluated by D.Lellinger.</remarks>
    protected static readonly double[] _smallApproximationCoefficients =
      new double[]
      {
        0,
        4 / Math.Sqrt(Math.PI),
        -1,
        -1 / (3 * Math.Sqrt(Math.PI)),
        -1 / 8d,
        -5 / (24 * Math.Sqrt(Math.PI)),
        -13 / 96d,
        -1073 / (3360 * Math.Sqrt(Math.PI)),
      };


    /// <summary>
    /// Radius of the cylinder. The default value is 1.
    /// The resulting diffusion coefficent is then in units of the square of the radius unit by the time unit that is used by the fit.
    /// </summary>
    public double Radius
    {
      get => field;
      init
      {
        if (!(value > 0))
        {
          throw new ArgumentOutOfRangeException(nameof(value), "Radius must be positive.");
        }
        field = value;
      }
    } = 1;

    /// <summary>
    /// Diameter of the cylinder. The default value is 2.
    /// The resulting diffusion coefficent is then in units of the square of the diameter unit by the time unit that is used by the fit.
    /// </summary>
    public double Diameter
    {
      get => 2 * Radius;
      init
      {
        if (!(value > 0))
        {
          throw new ArgumentOutOfRangeException(nameof(value), "Diameter must be positive.");
        }
        Radius = value / 2;
      }
    }

    /// <inheritdoc/>
    public event EventHandler? Changed;

    #region Serialization

    /// <summary>
    /// V0: 2025-06-09 initial version.
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(MassChangeAfterStepForCylinder), 0)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (MassChangeAfterStepForCylinder)obj;
        info.AddValue(nameof(Radius), s.Radius);
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var thickness = info.GetDouble(nameof(Radius));
        return new MassChangeAfterStepForCylinder() { Radius = thickness };
      }
    }

    #endregion Serialization

    [FitFunctionCreator("Mass change of a cylinder after a step", "Diffusion", 1, 1, 4)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.Diffusion.MassChangeAfterStepForCylinder}")]
    public static IFitFunction Create()
    {
      return new MassChangeAfterStepForCylinder();
    }

    /// <inheritdoc/>
    public int NumberOfIndependentVariables => 1;

    /// <inheritdoc/>
    public int NumberOfDependentVariables => 1;

    /// <inheritdoc/>
    public int NumberOfParameters => 4;



    /// <inheritdoc/>
    public string IndependentVariableName(int i)
    {
      return "t";
    }

    /// <inheritdoc/>
    public string DependentVariableName(int i)
    {
      return "M";
    }

    /// <inheritdoc/>
    public string ParameterName(int i)
    {
      return i switch
      {
        0 => "t0", // when did the mass change start
        1 => "M0", // Initial mass of the sheet
        2 => "ΔM", // Mass change
        3 => "D", // Diffusion coefficient
        _ => throw new ArgumentOutOfRangeException(nameof(i), i, null)
      };
    }

    /// <inheritdoc/>
    public double DefaultParameterValue(int i)
    {
      return i switch
      {
        0 => 0,
        1 => 0,
        2 => 1,
        3 => 1E-9,
        _ => throw new ArgumentOutOfRangeException(nameof(i), i, null)
      };
    }

    /// <inheritdoc/>
    public IVarianceScaling? DefaultVarianceScaling(int i)
    {
      return null;
    }


    private const double RV_SmallApproximation = 1 / 16384d; // Reduced variable small approximation threshold
    private const double RV_SumTo9 = 71 / 2048d;
    private const double RV_SumTo8 = 88 / 2048d;
    private const double RV_SumTo7 = 113 / 2048d;
    private const double RV_SumTo6 = 150 / 2048d;
    private const double RV_SumTo5 = 207 / 2048d;
    private const double RV_SumTo4 = 305 / 2048d;
    private const double RV_SumTo3 = 494 / 2048d;
    private const double RV_SumTo2 = 929 / 2048d;
    private const double RV_SumTo1 = 2340 / 2048d;

    /// <inheritdoc/>
    public static double Evaluate(double t, double r, double t0, double M0, double ΔM, double D)
    {
      double tDiff = t - t0;
      if (tDiff <= 0)
      {
        return M0; // No mass change before t0
      }

      var rv = D * tDiff / (r * r); // reduced variable

      if (rv < RV_SmallApproximation)
      {
        // If the reduced variable is very small, we can use a small approximation, see Ref. [1], eq. 5.25
        return M0 + ΔM * RMath.EvaluatePolynomOrderAscending(Math.Sqrt(rv), _smallApproximationCoefficients);
      }
      else
      {
        int N = rv switch
        {
          >= RV_SumTo1 => 1,
          >= RV_SumTo2 => 2,
          >= RV_SumTo3 => 3,
          >= RV_SumTo4 => 4,
          >= RV_SumTo5 => 5,
          >= RV_SumTo6 => 6,
          >= RV_SumTo7 => 7,
          >= RV_SumTo8 => 8,
          >= RV_SumTo9 => 9,
          _ => (int)Math.Ceiling(0.25 + Math.Sqrt(9 / 16d - Math.Log(1E-18) / (rv * Math.PI * Math.PI))),
        };
        double sum = 0;
        for (int n = N; n >= 1; n--)
        {
          double sqbess = BesselRelated.BesselJ0Zero(n);
          sqbess *= sqbess;
          sum += Math.Exp(-rv * sqbess) / sqbess;
        }
        return M0 + ΔM * (1 - 4 * sum); // Ref. [1], eq. 5.23
      }
    }

    /// <inheritdoc/>
    public void Evaluate(double[] independent, double[] parameters, double[] FV)
    {
      FV[0] = Evaluate(independent[0], Radius, parameters[0], parameters[1], parameters[2], parameters[3]);
    }

    /// <inheritdoc/>
    public void Evaluate(IROMatrix<double> independent, IReadOnlyList<double> parameters, IVector<double> FV, IReadOnlyList<bool>? dependentVariableChoice)
    {
      for (int i = 0; i < independent.RowCount; i++)
      {
        FV[i] = Evaluate(independent[i, 0], Radius, parameters[0], parameters[1], parameters[2], parameters[3]);
      }
    }

    /// <inheritdoc/>
    public (IReadOnlyList<double?>? LowerBounds, IReadOnlyList<double?>? UpperBounds) GetParameterBoundariesHardLimit()
    {
      return ([null, null, null, double.Epsilon], null);
    }

    /// <inheritdoc/>
    public (IReadOnlyList<double?>? LowerBounds, IReadOnlyList<double?>? UpperBounds) GetParameterBoundariesSoftLimit()
    {
      return ([null, double.Epsilon, null, double.Epsilon], null);
    }

    /// <inheritdoc/>
    public void EvaluateDerivative(IROMatrix<double> independent, IReadOnlyList<double> parameters, IReadOnlyList<bool>? isFixed, IMatrix<double> DF, IReadOnlyList<bool>? dependentVariableChoice)
    {
      double d = Radius;
      double t0 = parameters[0];
      double M0 = parameters[1];
      double ΔM = parameters[2];
      double D = parameters[3];

      for (int i = 0; i < independent.RowCount; i++)
      {
        double t = independent[i, 0];
        double tDiff = t - t0;
        if (tDiff > 0)
        {
          var rv = D * tDiff / (d * d); // reduced variable
          double derivWrtRV; // Derivative with respect to reduced variable
          double stepValue;
          if (rv <= RV_SmallApproximation)
          {
            var sqrtRV = Math.Sqrt(rv);
            derivWrtRV = RMath.EvaluatePolynom1stDerivativeOrderAscending(sqrtRV, _smallApproximationCoefficients) / (2 * sqrtRV); // Derivative of the small approximation, Crank, eq. 4.22
            stepValue = RMath.EvaluatePolynomOrderAscending(Math.Sqrt(rv), _smallApproximationCoefficients); // Function value for the small approximation
          }
          else
          {
            int N = rv switch
            {
              >= RV_SumTo1 => 1,
              >= RV_SumTo2 => 2,
              >= RV_SumTo3 => 3,
              >= RV_SumTo4 => 4,
              >= RV_SumTo5 => 5,
              >= RV_SumTo6 => 6,
              >= RV_SumTo7 => 7,
              >= RV_SumTo8 => 8,
              >= RV_SumTo9 => 9,
              _ => (int)Math.Ceiling(0.25 + Math.Sqrt(9 / 16d - Math.Log(1E-18) / (rv * Math.PI * Math.PI))), // Calculate the required number of summands
            };

            double sum0 = 0;
            double sum1 = 0;
            for (int n = N; n >= 1; n--)
            {
              double sqbess = BesselRelated.BesselJ0Zero(n);
              sqbess *= sqbess;
              double term = Math.Exp(-rv * sqbess);
              sum0 += term / sqbess; // Sum for the function value
              sum1 += term;
            }
            derivWrtRV = 4 * sum1;
            stepValue = 1 - 4 * sum0; // Function value
          }
          DF[i, 0] = -ΔM * derivWrtRV * D / (d * d); // wrt t0 
          DF[i, 1] = 1; // wrt M0 
          DF[i, 2] = stepValue; // wrt ΔM, which is the mass change at this time
          DF[i, 3] = ΔM * derivWrtRV * tDiff / (d * d); // wrt D 
        }
        else
        {
          DF[i, 0] = 0; // 0 wrt t0, no mass change before t0
          DF[i, 1] = 1; // 1 wrt M0, initial mass of the sheet
          DF[i, 2] = 0; // 0 wrt ΔM, no mass change before t0
          DF[i, 3] = 0; // 0 wrt D, no mass change before t0
        }
      }
    }
  }
}
