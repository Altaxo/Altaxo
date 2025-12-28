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

namespace Altaxo.Calc.FitFunctions.Transitions
{
  /// <summary>
  /// Only for testing purposes - use a "real" linear fit instead.
  /// </summary>
  [FitFunctionClass]
  public class SmoothedPercolation : IFitFunction, Main.IImmutable
  {
    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Calc.FitFunctions.Transitions.SmoothedPercolation", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(SmoothedPercolation), 1)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (SmoothedPercolation)obj;
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (SmoothedPercolation?)o ?? new SmoothedPercolation();
        return s;
      }
    }

    #endregion Serialization

    /// <summary>
    /// Initializes a new instance of the <see cref="SmoothedPercolation"/> class.
    /// </summary>
    public SmoothedPercolation()
    {
    }

    /// <summary>
    /// Factory for creating an instance of <see cref="SmoothedPercolation"/>.
    /// </summary>
    /// <returns>A new <see cref="IFitFunction"/> instance.</returns>
    [FitFunctionCreator("SmoothedPercolation", "Transitions", 1, 1, 5)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.Transitions.SmoothedPercolation}")]
    public static IFitFunction CreateSmoothedPercolation()
    {
      return new SmoothedPercolation();
    }

    #region IFitFunction Members

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
        return 5;
      }
    }

    /// <inheritdoc/>
    public string IndependentVariableName(int i)
    {
      return "phi";
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
        1 => "y1",
        2 => "phi_c",
        3 => "s",
        4 => "t",
        _ => throw new ArgumentOutOfRangeException(nameof(i), i, "Parameter index out of range.")
      };
    }

    /// <inheritdoc/>
    public double DefaultParameterValue(int i)
    {
      return i switch
      {
        0 => 0,
        1 => 0,
        2 => 0.5,
        3 => 1,
        4 => 1,
        _ => throw new ArgumentOutOfRangeException(nameof(i), i, "Parameter index out of range.")
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
      Y[0] = Evaluate(X[0], P[0], P[1], P[2], P[3], P[4]);
    }

    /// <inheritdoc/>
    public void Evaluate(IROMatrix<double> independent, IReadOnlyList<double> P, IVector<double> FV, IReadOnlyList<bool>? dependentVariableChoice)
    {
      var rowCount = independent.RowCount;
      for (int r = 0; r < rowCount; ++r)
      {
        var x = independent[r, 0];

        FV[r] = Evaluate(x, P[0], P[1], P[2], P[3], P[4]);
      }
    }

    /// <summary>
    /// Evaluates the smoothed percolation function for a single scalar input.
    /// </summary>
    /// <param name="phi">Independent variable between 0 and 1 representing the occupation probability.</param>
    /// <param name="y0">Left-side amplitude (must be greater than 0).</param>
    /// <param name="y1">Right-side amplitude (must be greater than 0).</param>
    /// <param name="phi_c">Critical threshold (must be in [0,1]).</param>
    /// <param name="s">Left exponent parameter (must be non-zero).</param>
    /// <param name="t">Right exponent parameter (must be non-zero).</param>
    /// <returns>The function value at the given <paramref name="phi"/> or <c>double.NaN</c> if parameters are invalid.</returns>
    public double Evaluate(double phi, double y0, double y1, double phi_c, double s, double t)
    {
      if (!(y0 > 0))
        return double.NaN;
      if (!(y1 > 0))
        return double.NaN;
      if (!(phi_c >= 0 && phi_c <= 1))
        return double.NaN;
      if (!(s != 0))
        return double.NaN;
      if (!(t != 0))
        return double.NaN;

      if (y0 == y1)
        return y0; // then there is no transition
      if (phi <= 0)
        return y0;
      if (phi >= 1)
        return y1;

      // if y0>y1 then we exchange both and the phi value
      if (y0 > y1)
      {
        double h = y0;
        y0 = y1;
        y1 = h;
        phi = 1 - phi;
      }

      double lgy = CalculateLgSigma(phi, y0, y1, phi_c, s, t);
      return Math.Pow(10, lgy);
    }

    /// <summary>
    /// Container for the parameters used as key in the p1-cache dictionary.
    /// </summary>
    private struct P1Var
    {
      public double sigmam;
      public double sigmat;
      public double pc;
      public double s;
      public double t;

      public P1Var(double sigmam, double sigmat, double pc, double s, double t)
      {
        this.sigmam = sigmam;
        this.sigmat = sigmat;
        this.pc = pc;
        this.s = s;
        this.t = t;
      }
    }

    /// <summary>
    /// Stores, dependent on t, s, sigmam, sigmat and so on, the lower boundary p1, because it is cost intensive to calculate.
    /// </summary>
    private System.Collections.Generic.Dictionary<P1Var, double> _sp1Hash = new System.Collections.Generic.Dictionary<P1Var, double>();

    /// <summary>
    /// Calculates the lower boundary p1 numerically for the given parameters.
    /// </summary>
    /// <param name="sigmam">Left amplitude.</param>
    /// <param name="sigmat">Right amplitude.</param>
    /// <param name="pc">Critical threshold.</param>
    /// <param name="s">Left exponent.</param>
    /// <param name="t">Right exponent.</param>
    /// <returns>Computed p1 value.</returns>
    private static double CalculateP1(double sigmam, double sigmat, double pc, double s, double t)
    {
      double co1 = s + t + Math.Log(sigmam / sigmat);

      double plower = 0, pupper = pc;

      do
      {
        double p = (plower + pupper) / 2;
        double y = co1 - s * Math.Log(1 - p / pc) - t * Math.Log((pc - p) * t / ((1 - pc) * s));
        if (y < 0)
          plower = p;
        else
          pupper = p;
      } while ((pupper - plower) / pc > 1E-12);

      return (pupper + plower) / 2;
    }

    /// <summary>
    /// Calculates p2 from p1 and other parameters.
    /// </summary>
    private static double CalculateP2(double p, double pc, double s, double t)
    {
      return (pc * (s + t) - p * t) / s;
    }

    /// <summary>
    /// Left-side log10(sigma) expression.
    /// </summary>
    private static double CalculateLgSigmaLeft(double p, double sigmam, double pc, double s)
    {
      return Math.Log10(sigmam * Math.Pow((pc - p) / pc, -s));
    }

    /// <summary>
    /// Right-side log10(sigma) expression.
    /// </summary>
    private static double CalculateLgSigmaRight(double p, double sigmat, double pc, double t)
    {
      return Math.Log10(sigmat * Math.Pow((p - pc) / (1 - pc), t));
    }

    /// <summary>
    /// Calculates the log10(sigma) for the smoothed transition using cached p1 and computed p2.
    /// </summary>
    private double CalculateLgSigma(double p, double sigmam, double sigmat, double pc, double s, double t)
    {
      var var = new P1Var(sigmam, sigmat, pc, s, t);
      if (!_sp1Hash.TryGetValue(var, out var p1))
      {
        p1 = CalculateP1(sigmam, sigmat, pc, s, t);
        _sp1Hash.Add(var, p1);
      }
      double p2 = CalculateP2(p1, pc, s, t);

      if (p < p1)
      {
        return CalculateLgSigmaLeft(p, sigmam, pc, s);
      }
      else if (p > p2)
      {
        return CalculateLgSigmaRight(p, sigmat, pc, t);
      }
      else
      {
        double lgsp1 = CalculateLgSigmaLeft(p1, sigmam, pc, s);
        double lgsp2 = CalculateLgSigmaRight(p2, sigmat, pc, t);
        return lgsp1 + (lgsp2 - lgsp1) * (p - p1) / (p2 - p1);
      }
    }

    /// <summary>
    /// Unused because this instance is immutable.
    /// </summary>
    public event EventHandler? Changed { add { } remove { } }

    #endregion IFitFunction Members

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
