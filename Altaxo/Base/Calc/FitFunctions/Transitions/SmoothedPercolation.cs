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

using System;
using Altaxo.Calc.Regression.Nonlinear;

namespace Altaxo.Calc.FitFunctions.Transitions
{
  /// <summary>
  /// Only for testing purposes - use a "real" linear fit instead.
  /// </summary>
  [FitFunctionClass]
  public class SmoothedPercolation : IFitFunction
  {
    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(SmoothedPercolation), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (SmoothedPercolation)obj;
      }

      public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        SmoothedPercolation s = o != null ? (SmoothedPercolation)o : new SmoothedPercolation();
        return s;
      }
    }

    #endregion Serialization

    public SmoothedPercolation()
    {
    }

    [FitFunctionCreator("SmoothedPercolation", "Transitions", 1, 1, 5)]
    [System.ComponentModel.Description("FitFunctions.Transitions.SmoothedPercolation")]
    public static IFitFunction CreateSmoothedPercolation()
    {
      return new SmoothedPercolation();
    }

    #region IFitFunction Members

    public int NumberOfIndependentVariables
    {
      get
      {
        return 1;
      }
    }

    public int NumberOfDependentVariables
    {
      get
      {
        return 1;
      }
    }

    public int NumberOfParameters
    {
      get
      {
        return 5;
      }
    }

    public string IndependentVariableName(int i)
    {
      return "phi";
    }

    public string DependentVariableName(int i)
    {
      return "y";
    }

    public string ParameterName(int i)
    {
      switch (i)
      {
        case 0:
          return "y0";

        case 1:
          return "y1";

        case 2:
          return "phi_c";

        case 3:
          return "s";

        case 4:
          return "t";
      }
      return string.Empty;
    }

    public double DefaultParameterValue(int i)
    {
      return i <= 1 ? 0 : i == 2 ? 0.5 : 1;
    }

    public IVarianceScaling DefaultVarianceScaling(int i)
    {
      return null;
    }

    public void Evaluate(double[] X, double[] P, double[] Y)
    {
      Y[0] = Evaluate(X[0], P[0], P[1], P[2], P[3], P[4]);
    }

    public static double Evaluate(double phi, double y0, double y1, double phi_c, double s, double t)
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

    private static System.Collections.Generic.Dictionary<P1Var, double> _sp1Hash = new System.Collections.Generic.Dictionary<P1Var, double>();

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

    private static double CalculateP2(double p, double pc, double s, double t)
    {
      return (pc * (s + t) - p * t) / s;
    }

    private static double CalculateLgSigmaLeft(double p, double sigmam, double pc, double s)
    {
      return Math.Log10(sigmam * Math.Pow((pc - p) / pc, -s));
    }

    private static double CalculateLgSigmaRight(double p, double sigmat, double pc, double t)
    {
      return Math.Log10(sigmat * Math.Pow((p - pc) / (1 - pc), t));
    }

    private static double CalculateLgSigma(double p, double sigmam, double sigmat, double pc, double s, double t)
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
    /// Not used here since this fit function never changed.
    /// </summary>
    public event EventHandler Changed;

    protected virtual void OnChanged()
    {
      Changed?.Invoke(this, EventArgs.Empty);
    }

    #endregion IFitFunction Members
  }
}
