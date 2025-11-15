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
  /// Describes the mass change of a plane sheet (with given thickness and infinite lateral dimensions) in a diffusion process. The diffusion takes place on both sides of the sheet.
  /// The diffusion process occurs on both planes of the plane sheet.
  /// </summary>
  [FitFunctionClass]
  public record MassChangeAfterStepForPlaneSheet : IFitFunction, IFitFunctionWithDerivative, Main.IImmutable
  {
    /// <summary>
    /// Thickness of the plane sheet (Default value is 2). Note that the diffusion process occurs on both planes of the plane sheet.
    /// The resulting diffusion coefficent is then in units of the square of the thickness unit by the time unit that is used by the fit.
    /// </summary>
    public double Thickness
    {
      get => field;
      init
      {
        if (!(value > 0))
        {
          throw new ArgumentOutOfRangeException(nameof(value), "Thickness must be positive.");
        }
        field = value;
      }
    } = 2;

    /// <summary>
    /// Half the thickness of the plane sheet (Default value is 1). Note that the diffusion process occurs on both planes of the plane sheet.
    /// The resulting diffusion coefficent is then in units of the square of the thickness unit by the time unit that is used by the fit.
    /// </summary>
    public double HalfThickness
    {
      get => Thickness / 2;
      init
      {
        if (!(value > 0))
        {
          throw new ArgumentOutOfRangeException(nameof(value), "HalfThickness must be positive.");
        }
        Thickness = value * 2;
      }
    }

    /// <inheritdoc/>
    public event EventHandler? Changed;

    #region Serialization

    /// <summary>
    /// V0: 2025-06-09 initial version.
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(MassChangeAfterStepForPlaneSheet), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (MassChangeAfterStepForPlaneSheet)obj;
        info.AddValue(nameof(Thickness), s.Thickness);
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var thickness = info.GetDouble(nameof(Thickness));
        return new MassChangeAfterStepForPlaneSheet() { Thickness = thickness };
      }
    }

    #endregion Serialization

    [FitFunctionCreator("Mass change of a plane sheet after a concentration step", "Diffusion", 1, 1, 4)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.Diffusion.MassChangeAfterStepForPlaneSheet}")]
    public static IFitFunction Create()
    {
      return new MassChangeAfterStepForPlaneSheet();
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


    private const double RV_SmallApproximation = 54 / 2048d; // Reduced variable small approximation threshold
    private const double RV_SumTo10 = 57 / 2048d;
    private const double RV_SumTo9 = 69 / 2048d;
    private const double RV_SumTo8 = 84 / 2048d;
    private const double RV_SumTo7 = 106 / 2048d;
    private const double RV_SumTo6 = 136 / 2048d;
    private const double RV_SumTo5 = 182 / 2048d;
    private const double RV_SumTo4 = 255 / 2048d;
    private const double RV_SumTo3 = 382 / 2048d;
    private const double RV_SumTo2 = 638 / 2048d;
    private const double RV_SumTo1 = 1275 / 2048d;
    private const double RV_SumTo0 = 3830 / 2048d;
    private const double C2BySqrtPi = 1.12837916709551257389616; // 2 / Math.Sqrt(Math.PI);

    private const double _accuracy = 1E-20;

    /// <summary>
    /// Evaluates the response of a unit step in dependence of the reduced variable.
    /// </summary>
    /// <param name="rv">Reduced variable rv = D*t/l², where D is the diffusion coefficient, t is the time and l is the half thickness of the plane sheet.</param>
    /// <returns>The response to a unit step in dependence on rv and rz.</returns>
    public static double EvaluateUnitStepWrtReducedVariable(double rv)
    {
      if (rv <= 0)
      {
        return 0; // No mass change before t0
      }
      else if (rv <= RV_SmallApproximation)
      {
        return Math.Sqrt(rv) * C2BySqrtPi; // Small approximation, Crank, eq. 4.18
      }
      else
      {
        int N = rv switch
        {
          >= RV_SumTo0 => 0,
          >= RV_SumTo1 => 1,
          >= RV_SumTo2 => 2,
          >= RV_SumTo3 => 3,
          >= RV_SumTo4 => 4,
          >= RV_SumTo5 => 5,
          >= RV_SumTo6 => 6,
          >= RV_SumTo7 => 7,
          >= RV_SumTo8 => 8,
          >= RV_SumTo9 => 9,
          >= RV_SumTo10 => 10,
          _ => 11
        };

        double sum = 0;
        for (int n = N; n >= 0; --n)
        {
          double sqrnpi = (n + 0.5) * Math.PI;
          sqrnpi *= sqrnpi;
          sum += Math.Exp(-rv * sqrnpi) / sqrnpi;
        }

        // Evaluate the unit step response, see Ref. [1], p. 75, eq. (5.29)
        return 1 - 2 * sum;
      }
    }

    /// <summary>
    /// Evaluates the response of a unit step in dependence of the reduced variables.
    /// </summary>
    /// <param name="rv">Reduced variable rv = D*t/l², where D is the diffusion coefficient, t is the time and l is the half thickness of the plane sheet.</param>
    /// <returns>The response to a unit step in dependence on rv and rz, and the derivatives w.r.t. rv and rz.</returns>
    public static (double functionValue, double derivativeWrtRv) EvaluateUnitStepAndDerivativesWrtReducedVariable(double rv)
    {
      if (rv <= 0)
      {
        return (0, 0); // No mass change before t0
      }
      else if (rv <= RV_SmallApproximation)
      {
        return (C2BySqrtPi * Math.Sqrt(rv), 0.5 * C2BySqrtPi / Math.Sqrt(rv)); // Small approximation, Crank, eq. 4.18
      }
      else
      {
        int N = rv switch
        {
          >= RV_SumTo0 => 0,
          >= RV_SumTo1 => 1,
          >= RV_SumTo2 => 2,
          >= RV_SumTo3 => 3,
          >= RV_SumTo4 => 4,
          >= RV_SumTo5 => 5,
          >= RV_SumTo6 => 6,
          >= RV_SumTo7 => 7,
          >= RV_SumTo8 => 8,
          >= RV_SumTo9 => 9,
          >= RV_SumTo10 => 10,
          _ => 11
        };

        double sum0 = 0;
        double sum1 = 0;
        for (int n = N; n >= 0; --n)
        {
          double sqrnpi = (n + 0.5) * Math.PI;
          sqrnpi *= sqrnpi;
          var term = Math.Exp(-rv * sqrnpi);
          sum0 += term / sqrnpi; // for the function value
          sum1 += term; // for the derivative wrt rv
        }

        // Evaluate the unit step response, see Ref. [1], p. 75, eq. (5.29)
        var fv = 1 - 2 * sum0;

        // derivative w.r.t. rv
        var dWrtRv = 2 * sum1;

        return (fv, dWrtRv);
      }
    }

    /// <summary>
    /// Evaluates the response of a unit step (M0 = 0, ΔM = 1) at t0 = 0.
    /// </summary>
    /// <param name="t">The time t.</param>
    /// <param name="l">The half thickness of the plane sheet.</param>
    /// <param name="D">The diffusion constant D.</param>
    /// <returns>The response to a unit step (M0 = 0, ΔM = 1) at t0 = 0.</returns>
    public static double EvaluateUnitStep(double t, double l, double D)
    {
      return EvaluateUnitStepWrtReducedVariable(D * t / (l * l));
    }


    /// <summary>
    /// Evaluates the response of a concentration step at t0.
    /// </summary>
    /// <param name="t">The time.</param>
    /// <param name="l">The half thickness of the plane sheet.</param>
    /// <param name="t0">The time of the concentration step.</param>
    /// <param name="M0">The initial mass.</param>
    /// <param name="ΔM">The total change of mass due to the concentration step.</param>
    /// <param name="D">The diffusion coefficient.</param>
    /// <returns>The response to a concentration step at t0.</returns>
    public static double Evaluate(double t, double l, double t0, double M0, double ΔM, double D)
    {
      return M0 + ΔM * EvaluateUnitStepWrtReducedVariable(D * (t - t0) / (l * l));
    }

    /// <inheritdoc/>
    public void Evaluate(double[] independent, double[] parameters, double[] FV)
    {
      FV[0] = Evaluate(independent[0], HalfThickness, parameters[0], parameters[1], parameters[2], parameters[3]);
    }

    /// <inheritdoc/>
    public void Evaluate(IROMatrix<double> independent, IReadOnlyList<double> parameters, IVector<double> FV, IReadOnlyList<bool>? dependentVariableChoice)
    {
      for (int i = 0; i < independent.RowCount; i++)
      {
        FV[i] = Evaluate(independent[i, 0], HalfThickness, parameters[0], parameters[1], parameters[2], parameters[3]);
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
      return ([null, 0, null, double.Epsilon], null);
    }

    /// <inheritdoc/>
    public void EvaluateDerivative(IROMatrix<double> independent, IReadOnlyList<double> parameters, IReadOnlyList<bool>? isFixed, IMatrix<double> DF, IReadOnlyList<bool>? dependentVariableChoice)
    {
      double l = HalfThickness;
      double t0 = parameters[0];
      double ΔM = parameters[2];
      double D = parameters[3];

      for (int i = 0; i < independent.RowCount; i++)
      {
        double t = independent[i, 0] - t0;
        var rv = D * t / (l * l); // reduced variable
        if (!(rv <= 0))
        {
          // Evaluate the derivatives
          var (fv, derivWrtRv) = EvaluateUnitStepAndDerivativesWrtReducedVariable(rv);
          DF[i, 0] = -ΔM * derivWrtRv * D / (l * l); // wrt t0
          DF[i, 1] = 1; // wrt M0
          DF[i, 2] = fv; // wrt ΔM, which is the mass change at this time
          DF[i, 3] = ΔM * (derivWrtRv * t / (l * l)); // wrt D
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
