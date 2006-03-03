#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
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
using Altaxo.Calc.Regression.Nonlinear;

namespace Altaxo.Calc.FitFunctions.General
{


  /// <summary>
  /// Only for testing purposes - use a "real" linear fit instead.
  /// </summary>
  [FitFunctionClass]
  public class LinearFitWithGradient : IFitFunctionWithGradient
  {
    public LinearFitWithGradient()
    {
      //
      // TODO: Add constructor logic here
      //
    }

    [FitFunctionCreator("LinearFitWithGradient", "Relaxation", 1, 1, 2)]
    public static IFitFunction LinearFitWithGradientOrder2()
    {
      return new LinearFitWithGradient();
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
        return 2;
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
      return (new string[] { "intercept", "slope", })[i];
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
      Y[0] = P[0] + P[1] * X[0];
    }

    #endregion

    public void EvaluateGradient(double[] X, double[] P, double[][] DY)
    {
      DY[0][0] = 1;
      DY[0][1] = X[0];
    }

  }


  /// <summary>
  /// Only for testing purposes - use a "real" linear fit instead.
  /// </summary>
  [FitFunctionClass]
  public class LinearFit : IFitFunction
  {
    public LinearFit()
    {
      //
      // TODO: Add constructor logic here
      //
    }

    [FitFunctionCreator("LinearFit", "Relaxation", 1, 1, 2)]
    public static IFitFunction LinearFitOrder2()
    {
      return new LinearFit();
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
        return 2;
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
      return (new string[] { "intercept", "slope", })[i];
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
      Y[0] = P[0] + P[1] * X[0];
    }

    #endregion




    #region IFitFunction Members




    #endregion


  }
}
