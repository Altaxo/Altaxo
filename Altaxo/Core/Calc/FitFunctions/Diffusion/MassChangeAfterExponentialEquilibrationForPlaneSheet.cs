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
  public record MassChangeAfterExponentialEquilibrationForPlaneSheet : IFitFunction, Main.IImmutable
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

    [FitFunctionCreator("Mass change of plane sheet", "Diffusion", 1, 1, 4)]
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

      var rv = D / (d * d); // reduced variable

      // Calculate the number of terms to reach the smallest term to be 10^-16
      var NN = Math.Sqrt(Math.Log(10000)) / (Math.PI * Math.Sqrt(t * rv));

      if (NN < 1000)
      {

        double sum = 0;
        for (int n = (int)Math.Ceiling(NN); n >= 0; --n)
        {
          double sqr2np1 = (2 * n + 1) * Math.PI;
          sqr2np1 *= sqr2np1; // (2n+1)^2 Pi^2
          double term = Math.Exp(-t * rv * sqr2np1) / (sqr2np1 * (1 - sqr2np1 * rv * tau));
          sum += term;
        }

        return 1 - Math.Exp(-t / tau) * Math.Sqrt(4 * rv * tau) * Math.Tan(1 / Math.Sqrt(4 * rv * tau)) - 8 * sum;
      }
      else
      {
        throw new NotImplementedException("Currently we have no approximation for small t");
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
  }
}
