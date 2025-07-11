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
  [FitFunctionClass]
  public record MassChangeAfterExponentialEquilibrationForPlaneSheet : IFitFunction, Main.IImmutable, IFitFunctionWithDerivative
  {
    /// <summary>
    /// Thickness of the plane sheet (Default value is 1).
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
    } = 1;

    /// <inheritdoc/>
    public event EventHandler? Changed;

    #region Serialization

    /// <summary>
    /// V0: 2025-06-09 initial version.
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(MassChangeAfterExponentialEquilibrationForPlaneSheet), 0)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (MassChangeAfterExponentialEquilibrationForPlaneSheet)obj;
        info.AddValue(nameof(Thickness), s.Thickness);
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var thickness = info.GetDouble(nameof(Thickness));
        return new MassChangeAfterExponentialEquilibrationForPlaneSheet() { Thickness = thickness };
      }
    }

    #endregion Serialization

    [FitFunctionCreator("Mass change of plane sheet after exponential change", "Diffusion", 1, 1, 5)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.Diffusion.MassChangePlaneSheet}")]
    public static IFitFunction Create()
    {
      return new MassChangeAfterExponentialEquilibrationForPlaneSheet();
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
        1 => "M0", // Initial mass of the sheet
        2 => "ΔM", // Mass change
        3 => "D", // Diffusion coefficient
        4 => "τ", // Time constant of the concentration change
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

    private const double _accuracy = 1E-20;

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
      var NN = Math.Ceiling(Math.Sqrt(-Math.Log(_accuracy) / (Math.PI * Math.PI * rv)));
      if (NN < 1000)
      {
        double sum = 0;
        for (int n = (int)NN; n >= 0; --n)
        {
          double sqrnpi = (n + 0.5) * Math.PI;
          sqrnpi *= sqrnpi;
          sum += Math.Exp(-rv * sqrnpi) / (sqrnpi * (1 - rz * sqrnpi));
        }
        var sqrtRz = Math.Sqrt(rz);

        // Evaluate the unit step response, see Ref. [1], p. 75, eq. (5.29)
        return 1
               - Math.Exp(-rv / rz) * sqrtRz * Math.Tan(1 / sqrtRz)
               - 2 * sum;
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
      var NN = Math.Ceiling(Math.Sqrt(-Math.Log(_accuracy) / (Math.PI * Math.PI * rv)));
      if (NN < 1000)
      {
        double sum0 = 0;
        double sum1 = 0;
        double sum2 = 0;
        for (int n = (int)NN; n >= 0; --n)
        {
          double sqrnpi = (n + 0.5) * Math.PI;
          sqrnpi *= sqrnpi;
          var denom = (1 - rz * sqrnpi);
          var term = Math.Exp(-rv * sqrnpi) / denom;
          sum0 += term / sqrnpi; // for the function value
          sum1 += term; // for the derivative wrt rv
          sum2 += term / denom; // for the derivative wrt rz
        }
        var sqrtRz = Math.Sqrt(rz);
        var oneBySqrtRz = 1 / Math.Sqrt(rz);
        var tan = Math.Tan(oneBySqrtRz);
        var sec = 1 / Math.Cos(oneBySqrtRz);

        // Evaluate the unit step response, see Ref. [1], p. 75, eq. (5.29)
        var fv = 1
               - Math.Exp(-rv / rz) * sqrtRz * tan
               - 2 * sum0;

        // derivative w.r.t. rv
        var dWrtRv =
          Math.Exp(-rv / rz) * oneBySqrtRz * tan
          + 2 * sum1;

        // derivative w.r.t. rz
        var dWrtRz =
          (0.5 * Math.Exp(-rv / rz) / rz) * (sec * sec - (2 * rv + rz) * oneBySqrtRz * tan)
          - 2 * sum2;

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
    /// <param name="d">The total thickness of the sheet d.</param>
    /// <param name="D">The diffusion constant D.</param>
    /// <param name="tau">The time constant of the concentration change tau.</param>
    /// <returns>The response to a unit step (M0 = 0, ΔM = 1) at t0 = 0.</returns>
    public static double EvaluateUnitStep(double t, double d, double D, double tau)
    {
      if (!(t > 0))
      {
        return 0; // No mass change before t0
      }

      return EvaluateUnitStepWrtReducedVariables(D * t / (d * d), D * tau / (d * d));
    }

    /// <inheritdoc/>
    public static double Evaluate(double t, double d, double t0, double M0, double ΔM, double D, double tau)
    {
      return M0 + ΔM * EvaluateUnitStepWrtReducedVariables(D * (t - t0) / (d * d), D * tau / (d * d));
    }

    /// <inheritdoc/>
    public void Evaluate(double[] independent, double[] parameters, double[] FV)
    {
      FV[0] = Evaluate(independent[0], Thickness, parameters[0], parameters[1], parameters[2], parameters[3], parameters[4]);
    }

    /// <inheritdoc/>
    public void Evaluate(IROMatrix<double> independent, IReadOnlyList<double> parameters, IVector<double> FV, IReadOnlyList<bool>? dependentVariableChoice)
    {
      for (int i = 0; i < independent.RowCount; i++)
      {
        FV[i] = Evaluate(independent[i, 0], Thickness, parameters[0], parameters[1], parameters[2], parameters[3], parameters[4]);
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
      double d = Thickness;
      double t0 = parameters[0];
      double ΔM = parameters[2];
      double D = parameters[3];
      double tau = parameters[4];

      var rz = D * tau / (d * d); // reduced variable

      for (int i = 0; i < independent.RowCount; i++)
      {
        double t = independent[i, 0] - t0;
        var rv = D * t / (d * d); // reduced variable
        if (!(rv <= 0))
        {
          // Evaluate the derivatives
          var (fv, derivWrtRv, derivWrtRz) = EvaluateUnitStepAndDerivativesWrtReducedVariables(rv, rz);
          DF[i, 0] = -ΔM * derivWrtRv * D / (d * d); // wrt t0
          DF[i, 1] = 1; // wrt M0
          DF[i, 2] = fv; // wrt ΔM, which is the mass change at this time
          DF[i, 3] = ΔM * (derivWrtRv * t / (d * d) + derivWrtRz * tau / (d * d)); // wrt D
          DF[i, 4] = ΔM * derivWrtRz * D / (d * d); // wrt tau
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
