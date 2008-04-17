#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

using System;
using System.ComponentModel;
using Altaxo.Calc.Regression.Nonlinear;

namespace Altaxo.Calc.FitFunctions.Transitions
{


  /// <summary>
  /// Only for testing purposes - use a "real" linear fit instead.
  /// </summary>
  [FitFunctionClass]
  public class GeneralEffectiveMedium : IFitFunction
  {
    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(GeneralEffectiveMedium), 0)]
    class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        GeneralEffectiveMedium s = (GeneralEffectiveMedium)obj;
        

      }

      public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        GeneralEffectiveMedium s = o != null ? (GeneralEffectiveMedium)o : new GeneralEffectiveMedium();
        return s;
      }
    }

    #endregion

    public GeneralEffectiveMedium()
    {
    }


    [FitFunctionCreator("GeneralEffectiveMedium", "Transitions", 1, 1, 5)]
    [System.ComponentModel.Description("FitFunctions.Transitions.GeneralEffectiveMedium")]
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
      // TODO:  Add KohlrauschDecay.IndependentVariableName implementation
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
      return i<=1 ? 0 : i==2 ? 0.5 : 1;
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


      // we denote with ss and tt the powers 1/s and 1/t respectively
      double y0ss = Math.Pow(y0, 1 / s);
      double y1tt = Math.Pow(y1, 1 / t);
      double A = (1 - phi_c) / phi_c;

      double lmin = Math.Log(y0);
      double lmax = Math.Log(y1);

      double logy = FindDecreasingYEqualToZero(
         delegate(double x) // x is the natural logarithm of the effective value
         {
           double yss = Math.Exp(x / s);
           double ytt = Math.Exp(x / t);
           return (1 - phi) * (y0ss - yss) / (y0ss + A * yss) + phi * (y1tt - ytt) / (y1tt + A * ytt);
         }, Math.Log(y0), Math.Log(y1));


      return Math.Exp(logy);
    }

    /// <summary>
    /// Finds the x where func(x)==0 between x0<x<x1 for a monoton decreasing function func.
    /// </summary>
    /// <param name="func"></param>
    /// <param name="x0"></param>
    /// <param name="x1"></param>
    /// <returns></returns>
    private static double FindDecreasingYEqualToZero(ScalarFunctionDD func, double x0, double x1)
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

    

    #endregion

   

  }


}
