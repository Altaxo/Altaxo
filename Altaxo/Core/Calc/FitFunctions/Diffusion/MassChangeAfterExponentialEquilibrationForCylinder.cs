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
  /// Describes the mass change of a plane sheet (with given thickness and infinite lateral dimensions) in a diffusion process
  /// after a concentration change that is modeled by an exponential equilibration.
  /// </summary>
  /// <remarks>
  /// Ref. [1]: Crank, "The Mathematics of Diffusion", 2nd edition, 1975, Oxford University Press.
  /// </remarks>
  [FitFunctionClass]
  public record MassChangeAfterExponentialEquilibrationForCylinder : IFitFunction, Main.IImmutable, IFitFunctionWithDerivative
  {
    /// <summary>
    /// Radius of the cylinder. The default value is 1.
    /// The resulting diffusion coefficent is then in units of the square of the radius unit by the time unit that is used by the fit.
    public double Radius
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
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(MassChangeAfterExponentialEquilibrationForCylinder), 0)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (MassChangeAfterExponentialEquilibrationForCylinder)obj;
        info.AddValue(nameof(Radius), s.Radius);
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var radius = info.GetDouble(nameof(Radius));
        return new MassChangeAfterExponentialEquilibrationForCylinder() { Radius = radius };
      }
    }

    #endregion Serialization

    [FitFunctionCreator("Mass change of a cylinder after exponential equilibration concentration change", "Diffusion", 1, 1, 5)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.Diffusion.MassChangeAfterExponentialEquilibrationForCylinder}")]
    public static IFitFunction Create()
    {
      return new MassChangeAfterExponentialEquilibrationForCylinder();
    }

    /// <inheritdoc/>
    public int NumberOfIndependentVariables => 1;

    /// <inheritdoc/>
    public int NumberOfDependentVariables => 1;

    /// <inheritdoc/>
    public int NumberOfParameters => 5;



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
        1 => "M0", // Initial mass of the cylinder
        2 => "ΔM", // Mass change of the cylinder
        3 => "D", // Diffusion coefficient
        4 => "τ", // Time constant of the outer concentration change
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
        4 => 1,
        _ => throw new ArgumentOutOfRangeException(nameof(i), i, null)
      };
    }

    /// <inheritdoc/>
    public IVarianceScaling? DefaultVarianceScaling(int i)
    {
      return null;
    }

    /// <summary>
    /// Evaluates the response of a unit step in dependence of the reduced variables.
    /// </summary>
    /// <param name="rv">Reduced variable rv = D*t/r², where D is the diffusion coefficient, t is the time and r is the radius of the cylinder.</param>
    /// <param name="rz">Reduced variable rz = D*τ/r², where D is the diffusion coefficient, τ is the time constant of the outer concentration change, and r is the radius of the cylinder.</param>
    /// <returns>The response to a unit step in dependence on rv and rz.</returns>
    public static double EvaluateUnitStepWrtReducedVariables(double rv, double rz)
    {
      if (!(rv > 0))
      {
        return 0; // No mass change before t0
      }

      // Calculate the number of terms to reach the smallest term to be 10^-16
      var NN = Math.Ceiling(0.25 + Math.Sqrt(9 / 16d - Math.Log(1E-18) / (rv * Math.PI * Math.PI)));
      if (NN < 1000)
      {
        double sum = 0;
        for (int n = (int)NN; n >= 1; --n)
        {
          double sqbess = BesselRelated.BesselJ0Zero(n);
          sqbess *= sqbess;
          sum += Math.Exp(-rv * sqbess) / (sqbess * (1 - rz * sqbess));
        }
        var oneBySqrtRz = 1 / Math.Sqrt(rz);

        // Evaluate the unit step response, see Ref. [1], p. 75, eq. (5.29)
        return 1
               - 2 * Math.Exp(-rv / rz) * BesselRelated.BesselJ1(oneBySqrtRz) / (oneBySqrtRz * BesselRelated.BesselJ0(oneBySqrtRz))
               - 4 * sum;
      }
      else
      {
        return 0;
      }
    }

    /// <summary>
    /// Evaluates the response of a unit step in dependence of the reduced variables.
    /// </summary>
    /// <param name="rv">Reduced variable rv = D*t/r², where D is the diffusion coefficient, t is the time and r is the radius of the cylinder.</param>
    /// <param name="rz">Reduced variable rz = D*τ/r², where D is the diffusion coefficient, τ is the time constant of the outer concentration change, and r is the radius of the cylinder.</param>
    /// <returns>The response to a unit step in dependence on rv and rz, and the derivatives w.r.t. rv and rz.</returns>
    public static (double functionValue, double derivativeWrtRv, double derivativeWrtRz) EvaluateUnitStepAndDerivativesWrtReducedVariables(double rv, double rz)
    {
      if (!(rv > 0))
      {
        return (0, 0, 0); // No mass change before t0
      }

      // Calculate the number of terms to reach the smallest term to be 10^-16
      var NN = Math.Ceiling(0.25 + Math.Sqrt(9 / 16d - Math.Log(1E-20) / (rv * Math.PI * Math.PI)));
      if (NN < 1000)
      {
        double sum0 = 0;
        double sum1 = 0;
        double sum2 = 0;
        for (int n = (int)NN; n >= 1; --n)
        {
          double sqbess = BesselRelated.BesselJ0Zero(n);
          sqbess *= sqbess;
          var denom = (1 - rz * sqbess);
          var term = Math.Exp(-rv * sqbess) / denom;
          sum0 += term / sqbess; // for the function value
          sum1 += term; // for the derivative wrt rv
          sum2 += term / denom; // for the derivative wrt rz
        }
        var oneBySqrtRz = 1 / Math.Sqrt(rz);
        var besselJ0 = BesselRelated.BesselJ0(oneBySqrtRz);
        var besselJ1 = BesselRelated.BesselJ1(oneBySqrtRz);

        // Evaluate the unit step response, see Ref. [1], p. 75, eq. (5.29)
        var fv = 1
               - 2 * Math.Exp(-rv / rz) * besselJ1 / (oneBySqrtRz * besselJ0)
               - 4 * sum0;

        // derivative w.r.t. rv
        var dWrtRv =
          2 * Math.Exp(-rv / rz) * oneBySqrtRz * besselJ1 / besselJ0
          + 4 * sum1;

        // derivative w.r.t. rz
        var dWrtRz =
          Math.Exp(-rv / rz) / rz * (1 + besselJ1 * (besselJ1 - 2 * (rv + rz) * oneBySqrtRz * besselJ0) / (besselJ0 * besselJ0))
          - 4 * sum2;

        return (fv, dWrtRv, dWrtRz);
      }
      else
      {
        return (0, 0, 0);
      }
    }


    /// <summary>
    /// Evaluates the response of a unit step (M0 = 0, ΔM = 1) at t0 = 0.
    /// </summary>
    /// <param name="t">The time t.</param>
    /// <param name="r">The radius of the cylinder.</param>
    /// <param name="D">The diffusion constant D.</param>
    /// <param name="tau">The time constant of the concentration change tau.</param>
    /// <returns>The response to a unit step (M0 = 0, ΔM = 1) at t0 = 0.</returns>
    public static double EvaluateUnitStep(double t, double r, double D, double tau)
    {
      return EvaluateUnitStepWrtReducedVariables(D * t / (r * r), D * tau / (r * r));
    }

    /// <inheritdoc/>
    public static double Evaluate(double t, double r, double t0, double M0, double ΔM, double D, double tau)
    {
      return M0 + ΔM * EvaluateUnitStepWrtReducedVariables(D * (t - t0) / (r * r), D * tau / (r * r));
    }

    /// <inheritdoc/>
    public void Evaluate(double[] independent, double[] parameters, double[] FV)
    {
      FV[0] = Evaluate(independent[0], Radius, parameters[0], parameters[1], parameters[2], parameters[3], parameters[4]);
    }

    /// <inheritdoc/>
    public void Evaluate(IROMatrix<double> independent, IReadOnlyList<double> parameters, IVector<double> FV, IReadOnlyList<bool>? dependentVariableChoice)
    {
      for (int i = 0; i < independent.RowCount; i++)
      {
        FV[i] = Evaluate(independent[i, 0], Radius, parameters[0], parameters[1], parameters[2], parameters[3], parameters[4]);
      }
    }

    /// <inheritdoc/>
    public (IReadOnlyList<double?>? LowerBounds, IReadOnlyList<double?>? UpperBounds) GetParameterBoundariesHardLimit()
    {
      return ([null, null, null, double.Epsilon, double.Epsilon], null);
    }

    /// <inheritdoc/>
    public (IReadOnlyList<double?>? LowerBounds, IReadOnlyList<double?>? UpperBounds) GetParameterBoundariesSoftLimit()
    {
      return ([null, double.Epsilon, null, double.Epsilon, double.Epsilon], null);
    }

    /// <inheritdoc/>
    public void EvaluateDerivative(IROMatrix<double> independent, IReadOnlyList<double> parameters, IReadOnlyList<bool>? isFixed, IMatrix<double> DF, IReadOnlyList<bool>? dependentVariableChoice)
    {
      double r = Radius;
      double t0 = parameters[0];
      double M0 = parameters[1];
      double ΔM = parameters[2];
      double D = parameters[3];
      double tau = parameters[4];

      var rz = D * tau / (r * r); // reduced variable

      for (int i = 0; i < independent.RowCount; i++)
      {
        double t = independent[i, 0] - t0;
        var rv = D * t / (r * r); // reduced variable
        if (!(rv <= 0))
        {
          // Evaluate the derivatives
          var (fv, derivWrtRv, derivWrtRz) = EvaluateUnitStepAndDerivativesWrtReducedVariables(rv, rz);
          DF[i, 0] = -ΔM * derivWrtRv * D / (r * r); // wrt t0
          DF[i, 1] = 1; // wrt M0
          DF[i, 2] = fv; // wrt ΔM, which is the mass change at this time
          DF[i, 3] = ΔM * (derivWrtRv * t / (r * r) + derivWrtRz * tau / (r * r)); // wrt D
          DF[i, 4] = ΔM * derivWrtRz * D / (r * r); // wrt tau
        }
        else
        {
          DF[i, 0] = 0; // 0 wrt t0, no mass change before t0
          DF[i, 1] = 1; // 1 wrt M0, initial mass of the sheet
          DF[i, 2] = 0; // 0 wrt ΔM, no mass change before t0
          DF[i, 3] = 0; // 0 wrt D, no mass change before t0
          DF[i, 4] = 0; // 0 wrt tau, no mass change before t0
        }
      }
    }

  }
}
