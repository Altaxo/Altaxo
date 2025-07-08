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
  public record MassChangeAfterExponentialEquilibrationForSphere : IFitFunction, Main.IImmutable, IFitFunctionWithDerivative
  {
    /// <summary>
    /// Radius of the sphere. The default value is 1.
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
    /// Diameter of the sphere. The default value is 2.
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
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(MassChangeAfterExponentialEquilibrationForSphere), 0)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (MassChangeAfterExponentialEquilibrationForSphere)obj;
        info.AddValue(nameof(Radius), s.Radius);
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var radius = info.GetDouble(nameof(Radius));
        return new MassChangeAfterExponentialEquilibrationForSphere() { Radius = radius };
      }
    }

    #endregion Serialization

    [FitFunctionCreator("Mass change of a sphere after exponential equilibration concentration change", "Diffusion", 1, 1, 5)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.Diffusion.MassChangeExponentialEquilibrationForSphere}")]
    public static IFitFunction Create()
    {
      return new MassChangeAfterExponentialEquilibrationForSphere();
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
        1 => "M0", // Initial mass of the sphere
        2 => "ΔM", // Mass change of the sphere
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
    /// Evaluates the response of a unit step (M0 = 0, ΔM = 1) at t0 = 0.
    /// </summary>
    /// <param name="t">The time t.</param>
    /// <param name="tau">The time constant of the concentration change tau.</param>
    /// <param name="rv">The diffusion constand divided by the square of the total thickness of the sheet d.</param>
    /// <param name="N">The number of terms to sum.</param>
    /// <returns>The response to a unit step (M0 = 0, ΔM = 1) at t0 = 0.</returns>
    protected static double EvaluateSumTerm(double t, double tau, double rv, int N)
    {
      double OneByRVTau = 1 / (rv * tau);
      double sum = 0;
      for (int n = N; n >= 1; --n)
      {
        double sqrnpi = n * Math.PI;
        sqrnpi *= sqrnpi; // n^2 Pi^2
        double term = Math.Exp(-t * rv * sqrnpi) / (sqrnpi * (sqrnpi - OneByRVTau));
        sum += term;
      }
      return sum;
    }


    /// <summary>
    /// Evaluates the response of a unit step (M0 = 0, ΔM = 1) at t0 = 0.
    /// </summary>
    /// <param name="t">The time t.</param>
    /// <param name="r">The radius of the sphere.</param>
    /// <param name="D">The diffusion constant D.</param>
    /// <param name="tau">The time constant of the concentration change tau.</param>
    /// <returns>The response to a unit step (M0 = 0, ΔM = 1) at t0 = 0.</returns>
    public static double EvaluateUnitStep(double t, double r, double D, double tau)
    {
      if (!(t > 0))
      {
        return 0; // No mass change before t0
      }

      var rv = D / (r * r); // reduced variable

      // Calculate the number of terms to reach the smallest term to be 10^-16
      var NN = 4 * Math.Sqrt(Math.Log(10) / (t * rv)) / Math.PI;
      if (NN < 1000)
      {
        // Evaluate the unit step response, see Ref. [1], p. 92, eq. (6.26)
        double sum = EvaluateSumTerm(t, tau, rv, (int)Math.Ceiling(NN));
        return 1 - 3 * rv * tau * Math.Exp(-t / tau) * (1 - 1 / (Math.Sqrt(rv * tau) * Math.Tan(1 / Math.Sqrt(rv * tau)))) + 6 / (rv * tau) * sum;
      }
      else
      {
        return 0;
      }
    }

    /// <inheritdoc/>
    public static double Evaluate(double t, double d, double t0, double M0, double ΔM, double D, double tau)
    {
      return M0 + ΔM * EvaluateUnitStep(t - t0, d, D, tau);
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

      double Pow2(double x) => x * x;

      var rv = D / (r * r); // reduced variable

      for (int i = 0; i < independent.RowCount; i++)
      {
        double t = independent[i, 0] - t0;
        // Calculate the number of terms to reach the smallest term to be 10^-16
        var NN = 4 * Math.Sqrt(Math.Log(10) / (t * rv)) / Math.PI;

        if (t > 0 && NN < 1000)
        {
          int N = (int)Math.Ceiling(NN);
          double sqrtRvTau = Math.Sqrt(rv * tau);
          double oneBySqrtRvTau = 1 / sqrtRvTau;
          double termExp = Math.Exp(-t / tau);

          double sum0 = 0;
          double sum1 = 0;
          double sum4 = 0;
          double sum5 = 0;
          for (int n = N; n >= 1; --n)
          {
            double sqrnpi = n * Math.PI;
            sqrnpi *= sqrnpi; // n^2 Pi^2
            double denom = sqrnpi - 1 / (rv * tau);
            double term = Math.Exp(-t * rv * sqrnpi) / denom;
            sum0 += term / sqrnpi; // for the function value
            sum1 += term; // for the derivative wrt t0
            sum4 += term * (1 + 1 / (rv * tau * denom)) / sqrnpi; // for the derivative wrt tau
            sum5 += term / sqrnpi + term * t * rv + term / (denom * sqrnpi * rv * tau); // for the derivative wrt rv
          }

          // unit step derivatives
          var derivWrt_t = 3 * rv * termExp * (1 - 1 / (Math.Tan(oneBySqrtRvTau) * sqrtRvTau)) - 6 * sum1 / tau;

          var derivWrt_tau =
            0.5 * termExp / (tau * tau) * (3 * sqrtRvTau * (2 * t + tau) / Math.Tan(oneBySqrtRvTau) + 3 * tau * (-2 * rv * (t + tau) + Pow2(1 / Math.Sin(oneBySqrtRvTau))))
            - 6 * sum4 / (tau * tau * rv);

          var derivWrt_rv =
            1.5 * termExp / rv * (-2 * rv * tau + sqrtRvTau / Math.Tan(oneBySqrtRvTau) + RMath.Pow2(1 / Math.Sin(oneBySqrtRvTau)))
            - 6 * sum5 / (rv * rv * tau);


          var stepValue =
            1 - 3 * rv * tau * termExp * (1 - oneBySqrtRvTau / Math.Tan(oneBySqrtRvTau)) + 6 / (rv * tau) * sum0;

          DF[i, 0] = -ΔM * derivWrt_t; // wrt t0 
          DF[i, 1] = 1; // wrt M0 
          DF[i, 2] = stepValue; // wrt ΔM, which is the mass change at this time
          DF[i, 3] = ΔM * derivWrt_rv / (r * r); // wrt D
          DF[i, 4] = ΔM * derivWrt_tau; // wrt tau
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
