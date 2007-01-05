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

namespace Altaxo.Calc.FitFunctions.General
{


  /// <summary>
  /// Only for testing purposes - use a "real" linear fit instead.
  /// </summary>
  [FitFunctionClass]
  public class PolynomialFit : IFitFunctionWithGradient
  {
    int _order;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PolynomialFit), 0)]
    class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        PolynomialFit s = (PolynomialFit)obj;
        info.AddValue("Order", s._order);
    
      }

      public virtual object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        PolynomialFit s = o != null ? (PolynomialFit)o : new PolynomialFit();
        s._order = info.GetInt32("Order");
        return s;
      }
    }

    #endregion

    public PolynomialFit()
    {
      _order = 2;
    }

    public PolynomialFit(int order)
    {
      _order = order;
    }

    [FitFunctionCreator("PolynomialFit", "General", 1, 1, 10)]
    [System.ComponentModel.Description("FitFunctions.General.PolynomialFit")]
    public static IFitFunction CreatePolynomialFitOrder9()
    {
      return new PolynomialFit(9);
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
        return _order+1;
      }
    }

    public string IndependentVariableName(int i)
    {
      // TODO:  Add KohlrauschDecay.IndependentVariableName implementation
      return "x";
    }

    public string DependentVariableName(int i)
    {
      return "y";
    }

    public string ParameterName(int i)
    {
      return "a" + i.ToString();
    }

    public double DefaultParameterValue(int i)
    {
      return 0;
    }

    public IVarianceScaling DefaultVarianceScaling(int i)
    {
      return null;
    }

    public void Evaluate(double[] X, double[] P, double[] Y)
    {
      double sum = P[_order];
      for (int i = _order-1; i >= 0; i--)
      {
        sum *= X[0];
        sum += P[i];
      }

      Y[0] = sum;
    }

    #endregion

    public void EvaluateGradient(double[] X, double[] P, double[][] DY)
    {
      double sum = 1;
      for (int i = 0; i <= _order; i++)
      {
        DY[0][i] = sum;
        sum *= (i + 1) * X[0];
      }
    }

  }


}
