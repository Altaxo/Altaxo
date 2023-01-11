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
  public class GeneralEffectiveMedium : IFitFunction, Main.IImmutable
  {
    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Calc.FitFunctions.Transitions.GeneralEffectiveMedium", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(GeneralEffectiveMedium), 1)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (GeneralEffectiveMedium)obj;
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (GeneralEffectiveMedium?)o ?? new GeneralEffectiveMedium();
        return s;
      }
    }

    #endregion Serialization

    public GeneralEffectiveMedium()
    {
    }

    [FitFunctionCreator("GeneralEffectiveMedium", "Transitions", 1, 1, 5)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.Transitions.GeneralEffectiveMedium}")]
    public static IFitFunction CreateGeneralEffectiveMedium()
    {
      return new GeneralEffectiveMedium();
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

    public virtual string ParameterName(int i)
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

    public IVarianceScaling? DefaultVarianceScaling(int i)
    {
      return null;
    }

    /// <summary>
    /// Unused because this instance is immutable.
    /// </summary>
    public event EventHandler? Changed { add { } remove { } }

    public virtual void Evaluate(double[] X, double[] P, double[] Y)
    {
      Y[0] = Evaluate(X[0], P[0], P[1], P[2], P[3], P[4]);
    }

    public void Evaluate(IROMatrix<double> independent, IReadOnlyList<double> P, IVector<double> FV, IReadOnlyList<bool>? dependentVariableChoice)
    {
      var rowCount = independent.RowCount;
      for (int r = 0; r < rowCount; ++r)
      {
        var x = independent[r, 0];

        FV[r] = Evaluate(x, P[0], P[1], P[2], P[3], P[4]);
      }
    }

    public static double EvaluateOld(double phi, double y0, double y1, double phi_c, double s, double t)
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

      // we denote with ss and tt the powers 1/s and 1/t respectively
      double y0ss = Math.Pow(y0, 1 / s);
      double y1tt = Math.Pow(y1, 1 / t);
      double A = (1 - phi_c) / phi_c;

      double lmin = Math.Log(y0);
      double lmax = Math.Log(y1);

      double logy = FindDecreasingYEqualToZero(
         delegate (double x) // x is the natural logarithm of the effective value
         {
           double yss = Math.Exp(x / s);
           double ytt = Math.Exp(x / t);
           return (1 - phi) * (y0ss - yss) / (y0ss + A * yss) + phi * (y1tt - ytt) / (y1tt + A * ytt);
         }, Math.Log(y0), Math.Log(y1));

      return Math.Exp(logy);
    }

    private static double Sqr(double x)
    {
      return x * x;
    }

    public static double Evaluate(double phi, double y0, double y1, double phi_c, double s, double t)
    {
      if (!(y0 > 0))
        return double.NaN;
      if (!(y1 > 0))
        return double.NaN;
      if (!(phi_c >= 0 && phi_c <= 1))
        return double.NaN;
      if (!(s >= 0))
        return double.NaN;
      if (!(t >= 0))
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

      // we denote with ss and tt the powers 1/s and 1/t respectively
      double y0ss = Math.Pow(y0, 1 / s);
      double y1tt = Math.Pow(y1, 1 / t);
      double A = (1 - phi_c) / phi_c;

      double lmin = Math.Log(y0);
      double lmax = Math.Log(y1);

      // guess a value for the conductivity from the left or right side approximation
      double lystart;
      if (phi < phi_c)
      {
        lystart = lmin - s * Math.Log(1 - phi / phi_c);
        if (!(lystart <= lmax))
          lystart = 0.5 * (lmin + lmax);
      }
      else if (phi > phi_c)
      {
        lystart = lmax + t * Math.Log((phi - phi_c) / (1 - phi_c));
        if (!(lystart >= lmin))
          lystart = 0.5 * (lmin + lmax);
      }
      else
        lystart = 0.5 * (lmin + lmax);

      double ly = lystart;
      double threshold = 1e-8 * Math.Abs(lmax - lmin);

      for (int i = 0; i < 20; i++)
      {
        double yss = Math.Exp(ly / s);
        double ytt = Math.Exp(ly / t);
        // errechne den Funktionswert (Abweichung von 0)
        double f = ((1 - phi) * (y0ss - yss)) / (A * yss + y0ss) + (phi * (y1tt - ytt)) / (A * ytt + y1tt);

        // calculate the derivative of f with respect to ly
        double fs = (1 + A) * ((yss * (-1 + phi) * y0ss) / (s * Sqr(A * yss + y0ss)) - (ytt * phi * y1tt) / (t * Sqr(A * ytt + y1tt)));

        double deltaly = f / fs;
        if (Math.Abs(deltaly) < threshold)
          break;
        ly -= deltaly;
      }

      return Math.Exp(ly);
    }



    /// <summary>
    /// Finds the x where func(x)==0 between x0&lt;x&lt;x1 for a monoton decreasing function func.
    /// </summary>
    /// <param name="func"></param>
    /// <param name="x0"></param>
    /// <param name="x1"></param>
    /// <returns></returns>
    private static double FindDecreasingYEqualToZero(Func<double, double> func, double x0, double x1)
    {
      double low = x0;
      double high = x1;
      double xm;
      for (; ; )
      {
        xm = 0.5 * (low + high);
        double y = func(xm);
        if (y < 0)
          high = xm;
        else
          low = xm;

        if ((high - low) < 1E-15 * Math.Max(Math.Abs(high), Math.Abs(low)))
          break;
      }
      return xm;
    }

    #endregion IFitFunction Members
  }

  /// <summary>
  /// Only for testing purposes - use a "real" linear fit instead.
  /// </summary>
  [FitFunctionClass]
  public class GeneralEffectiveMediumLog10 : GeneralEffectiveMedium
  {
    #region Serialization

    /// <summary>
    /// 2023-01-11 Move from AltaxoBase to AltaxoCore
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Calc.FitFunctions.Transitions.GeneralEffectiveMediumLog10", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(GeneralEffectiveMediumLog10), 1)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (GeneralEffectiveMediumLog10)obj;
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (GeneralEffectiveMediumLog10?)o ?? new GeneralEffectiveMediumLog10();
        return s;
      }
    }

    #endregion Serialization

    public GeneralEffectiveMediumLog10()
    {
    }

    [FitFunctionCreator("GeneralEffectiveMediumLog10", "Transitions", 1, 1, 5)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.Transitions.Lg10GeneralEffectiveMedium}")]
    public static IFitFunction CreateGeneralEffectiveMediumLog10()
    {
      return new GeneralEffectiveMediumLog10();
    }

    public override void Evaluate(double[] X, double[] P, double[] Y)
    {
      Y[0] = EvaluateLog10(X[0], P[0], P[1], P[2], P[3], P[4]);
    }

    public static double EvaluateLog10(double phi, double lg_y0, double lg_y1, double phi_c, double s, double t)
    {
      return Math.Log10(GeneralEffectiveMedium.Evaluate(phi, Math.Pow(10, lg_y0), Math.Pow(10, lg_y1), phi_c, s, t));
    }

    public override string ParameterName(int i)
    {
      switch (i)
      {
        case 0:
          return "Lg_y0";

        case 1:
          return "Lg_y1)";

        case 2:
          return "phi_c";

        case 3:
          return "s";

        case 4:
          return "t";
      }
      return string.Empty;
    }
  }
}
