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
  public abstract class FermiDiracTransitionBase : IFitFunction
  {
    /// <summary>
    /// Returns a value which is 1 for p=0 and is 0 for p=1.
    /// </summary>
    /// <param name="p">Argument (between 0 and 1).</param>
    /// <param name="pc">Location of the transition (between 0 and 1).</param>
    /// <param name="w">Parameter determing the width of the transition.</param>
    /// <returns>A value between 1 (for p=0) and 0 (for p=1).</returns>
    /// <remarks>
    /// The original formula was y(p)=1/(1+Exp(b*(p-pc)).
    /// This formula has the disadvantage that it is not 1 for p=0 nor 0 for p=1. Thus we use
    /// the modified formula
    /// core(p)=(y(p)-y(1))/(y(0)-y(1)) with the definition above for y(p). Additionally, instead of b, we use w=1/b, because
    /// w is directly related to the width of the transition.
    /// </remarks>
    public static double Core(double p, double pc, double w)
    {
      double b = 1 / w;
      double A = Math.Exp(b * (p - pc));
      double B = Math.Exp(b * (1 - pc));
      double C = Math.Exp(b * (0 - pc));
      return ((B - A) / (B - C)) * ((1 + C) / (1 + A));
    }

    /// <summary>
    /// Provides a linear scaled transition y = y1+(y0-y1)*Core(...).
    /// </summary>
    /// <param name="y0"></param>
    /// <param name="y1"></param>
    /// <param name="p"></param>
    /// <param name="pc"></param>
    /// <param name="w"></param>
    /// <returns></returns>
    public static double LinearScaledTransition(double p, double y0, double y1, double pc, double w)
    {
      double core = Core(p, pc, w);
      return y1 + (y0 - y1) * core;
    }

    /// <summary>
    /// Provides a logarithmically scaled transition lg(y) = lg(y1)+(lg(y0)-lg(y1))*Core(...).
    /// </summary>
    /// <param name="y0"></param>
    /// <param name="y1"></param>
    /// <param name="p"></param>
    /// <param name="pc"></param>
    /// <param name="w"></param>
    /// <returns></returns>
    public static double LogarithmicScaledTransition(double p, double y0, double y1, double pc, double w)
    {
      double core = Core(p, pc, w);
      return Math.Pow(y0, core) * Math.Pow(y1, 1 - core);
    }

    #region IFitFunction Members

    public int NumberOfIndependentVariables
    {
      get { return 1; }
    }

    public int NumberOfDependentVariables
    {
      get { return 1; }
    }

    public int NumberOfParameters
    {
      get { return 4; }
    }

    public string IndependentVariableName(int i)
    {
      return "p";
    }

    public string DependentVariableName(int i)
    {
      return "y";
    }

    public string ParameterName(int i)
    {
      return i switch
      {
        0 => "y0",
        1 => "y1",
        2 => "pc",
        3 => "w",
        _ => throw new ArgumentOutOfRangeException(nameof(i)),
      };
    }

    public double DefaultParameterValue(int i)
    {
      return i switch
      {
        0 => 1,
        1 => 10,
        2 => 0.5,
        3 => 0.5,
        _ => throw new ArgumentOutOfRangeException(nameof(i)),
      };
    }

    public abstract IVarianceScaling DefaultVarianceScaling(int i);

    public abstract void Evaluate(double[] independent, double[] parameters, double[] FV);

    public abstract void Evaluate(IROMatrix<double> independent, IReadOnlyList<double> P, IVector<double> FV, IReadOnlyList<bool>? dependentVariableChoice);

    /// <summary>
    /// Unused because this instance is immutable.
    /// </summary>
    public event EventHandler? Changed { add { } remove { } }

    #endregion IFitFunction Members

    /// <inheritdoc/>
    public virtual (IReadOnlyList<double?>? LowerBounds, IReadOnlyList<double?>? UpperBounds) GetParameterBoundariesHardLimit()
    {
      return (null, null);
    }

    /// <inheritdoc/>
    public virtual (IReadOnlyList<double?>? LowerBounds, IReadOnlyList<double?>? UpperBounds) GetParameterBoundariesSoftLimit()
    {
      return (null, null);
    }
  }

  [FitFunctionClass]
  public class LinearFermiDiracTransition : FermiDiracTransitionBase, Main.IImmutable
  {
    #region Serialization

    /// <summary>
    /// 2023-01-11 Move from AltaxoBase to AltaxoCore
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Calc.FitFunctions.Transitions.LinearFermiDiracTransition", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(LinearFermiDiracTransition), 1)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (LinearFermiDiracTransition)obj;
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (LinearFermiDiracTransition?)o ?? new LinearFermiDiracTransition();
        return s;
      }
    }

    #endregion Serialization

    public LinearFermiDiracTransition()
    {
    }

    [FitFunctionCreator("LinearFermiDiracTransition", "Transitions", 1, 1, 4)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.Transitions.LinearFermiDiracTransition}")]
    public static IFitFunction CreateLinearFermiDiracTransition()
    {
      return new LinearFermiDiracTransition();
    }

    public override void Evaluate(double[] independent, double[] parameters, double[] FV)
    {
      FV[0] = LinearScaledTransition(independent[0], parameters[0], parameters[1], parameters[2], parameters[3]);
    }

    public override void Evaluate(IROMatrix<double> independent, IReadOnlyList<double> P, IVector<double> FV, IReadOnlyList<bool>? independentVariableChoice)
    {
      var rowCount = independent.RowCount;
      for (int r = 0; r < rowCount; ++r)
      {
        var x = independent[r, 0];
      }
    }

    public override IVarianceScaling DefaultVarianceScaling(int i)
    {
      return new ConstantVarianceScaling();
    }
  }

  [FitFunctionClass]
  public class LogarithmicFermiDiracTransition : FermiDiracTransitionBase, Main.IImmutable
  {
    #region Serialization

    /// <summary>
    /// 2023-01-11 Move from AltaxoBase to AltaxoCore
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Calc.FitFunctions.Transitions.LogarithmicFermiDiracTransition", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(LogarithmicFermiDiracTransition), 1)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (LogarithmicFermiDiracTransition)obj;
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (LogarithmicFermiDiracTransition?)o ?? new LogarithmicFermiDiracTransition();
        return s;
      }
    }

    #endregion Serialization

    public LogarithmicFermiDiracTransition()
    {
    }

    [FitFunctionCreator("LogarithmicFermiDiracTransition", "Transitions", 1, 1, 4)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.Transitions.LogarithmicFermiDiracTransition}")]
    public static IFitFunction CreateLogarithmicFermiDiracTransition()
    {
      return new LogarithmicFermiDiracTransition();
    }

    public override void Evaluate(double[] independent, double[] parameters, double[] FV)
    {
      FV[0] = LogarithmicScaledTransition(independent[0], parameters[0], parameters[1], parameters[2], parameters[3]);
    }

    public override void Evaluate(IROMatrix<double> independent, IReadOnlyList<double> P, IVector<double> FV, IReadOnlyList<bool>? independentVariableChoice)
    {
      var rowCount = independent.RowCount;
      for (int r = 0; r < rowCount; ++r)
      {
        var x = independent[r, 0];

        FV[r] = LogarithmicScaledTransition(x, P[0], P[1], P[2], P[3]);

      }
    }

    public override IVarianceScaling DefaultVarianceScaling(int i)
    {
      return new RelativeVarianceScaling();
    }
  }
}
