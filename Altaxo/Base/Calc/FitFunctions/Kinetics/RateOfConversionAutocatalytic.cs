#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2021 Dr. Dirk Lellinger
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
using System.Collections.Generic;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.Regression.Nonlinear;

namespace Altaxo.Calc.FitFunctions.Kinetics
{
  /// <summary>
  /// Represents solutions related to the differential equation y'=(k1+k2*y^m)(1-y)^n with the initial condition y(t0)=0.
  /// In this class, not the conversion y(t), but the conversion rate y'(t) is the dependent variable.
  /// </summary>
  [FitFunctionClass]
  public class RateOfConversionAutocatalytic : ConversionAutocatalytic
  {
    #region Serialization

    /// <summary>
    /// Initial version 2021-07-20.
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(RateOfConversionAutocatalytic), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (RateOfConversionAutocatalytic)obj;
      }

      public virtual object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return new RateOfConversionAutocatalytic();
      }
    }

    #endregion Serialization

    /// <summary>
    /// Creates the fit function.
    /// </summary>
    /// <returns>The fit function.</returns>
    [FitFunctionCreator("RateOfConversionAutocatalytic", "Kinetics", 1, 1, 6)]
    [System.ComponentModel.Description("${res:Altaxo.Calc.FitFunctions.Kinetics.RateOfConversionAutocatalytic}")]
    public static new IFitFunction CreateFitFunction()
    {
      return new RateOfConversionAutocatalytic();
    }

    /// <inheritdoc/>
    public override void Evaluate(double[] X, double[] P, double[] Y)
    {

      base.EvaluateConversionRate(X, P, Y);
      Y[0] *= P[1];

    }

    public void EvaluateMultiple(IROMatrix<double> independent, IReadOnlyList<double> P, IReadOnlyList<bool>? independentVariableChoice, IVector<double> FV)
    {
      IEnumerable<double> GetXPoints()
      {
        for (int r = 0; r < independent.RowCount; ++r)
        {
          var x = independent[r, 0] - P[0];
          if (x > 0)
            yield return x;
        }
      }

      int rd = 0;
      for (int r = 0; r < independent.RowCount; ++r)
      {
        var x = independent[r, 0] - P[0];
        if (!(x > 0))
        {
          FV[rd++] = 0;
        }
      }

      var ode = new Ode.DOP853();
      var y0 = new double[1] { 0 };
      ode.Initialize(0, y0, _evaluator.EvaluateRate);

      var solution = ode.GetSolutionPointsVolatile(new Ode.OdeMethodOptions()
      {
        AutomaticStepSizeControl = true,
        AbsoluteTolerance = 1E-4,
        RelativeTolerance = 1E-4,
        OptionalSolutionPoints = GetXPoints(),
        IncludeAutomaticStepsInOutput = false,
        IncludeInitialValueInOutput = false,
      }).GetEnumerator();

      while (solution.MoveNext())
      {
        var rate = _evaluator.EvaluateRate(solution.Current.X, solution.Current.Y_volatile[0]);
        FV[rd++] = rate * P[1];
      }
    }
  }
}
