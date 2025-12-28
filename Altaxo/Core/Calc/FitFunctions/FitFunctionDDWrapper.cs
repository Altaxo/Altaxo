#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2024 Dr. Dirk Lellinger
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

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Altaxo.Calc.Regression.Nonlinear;

namespace Altaxo.Calc.FitFunctions
{
  /// <summary>
  /// Simple wrapper around an <see cref="IFitFunction"/> that stores the function and the parameters and mimics a scalar function
  /// of one independent variable and one dependent variable.
  /// </summary>
  /// <seealso cref="Altaxo.Calc.IScalarFunctionDD" />
  /// <seealso cref="Altaxo.Main.IImmutable" />
  public record FitFunctionDDWrapper : IScalarFunctionDD, Main.IImmutable
  {
    private static ThreadLocal<(double[] x, double[] y)> _workingArrays = new(() => (new double[1], new double[1]));

    /// <summary>
    /// Gets the fit function.
    /// </summary>
    public IFitFunction FitFunction { get; }

    private double[] _parameters;

    /// <summary>
    /// Gets the parameters.
    /// </summary>
    public IReadOnlyList<double> Parameters => _parameters;


    #region Serialization

    /// <summary>
    /// V0: 2024-05-05
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(FitFunctionDDWrapper), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (FitFunctionDDWrapper)obj;
        info.AddValue("Function", s.FitFunction);
        info.AddArray("Parameters", s._parameters, s._parameters.Length);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var fitFunction = info.GetValue<IFitFunction>("Function", parent);
        info.GetArray("Parameters", out double[] parameters);
        return new FitFunctionDDWrapper(fitFunction, parameters);
      }
    }
    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="FitFunctionDDWrapper"/> class.
    /// </summary>
    /// <param name="fitFunction">The fit function.</param>
    /// <param name="parameters">The parameters.</param>
    /// <exception cref="System.ArgumentException">Thrown if the fit function is not immutable, does not have exactly one independent variable or one dependent variable, or if the number of provided parameters does not match the function's expected number.</exception>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="fitFunction"/> or <paramref name="parameters"/> is null.</exception>
    public FitFunctionDDWrapper(IFitFunction fitFunction, IReadOnlyList<double> parameters)
    {
      if (fitFunction is null)
      {
        throw new System.ArgumentNullException(nameof(fitFunction));
      }
      if (parameters is null)
      {
        throw new System.ArgumentNullException(nameof(parameters));
      }
      if (fitFunction is not Main.IImmutable)
      {
        throw new System.ArgumentException("Fit function has to be immutable", nameof(fitFunction));
      }
      if (fitFunction.NumberOfIndependentVariables != 1)
      {
        throw new System.ArgumentException("Fit function must have only one independent variable", nameof(fitFunction));
      }
      if (fitFunction.NumberOfDependentVariables != 1)
      {
        throw new System.ArgumentException("Fit function must have only one dependent variable", nameof(fitFunction));
      }

      if (fitFunction.NumberOfParameters != parameters.Count)
      {
        throw new System.ArgumentException($"Number of provided parameters is {parameters.Count}, but {fitFunction.NumberOfParameters} are expected", nameof(parameters));
      }

      FitFunction = fitFunction;
      _parameters = parameters.ToArray();
    }

    /// <inheritdoc/>
    public double Evaluate(double x)
    {
      var workingArrays = _workingArrays.Value!;
      workingArrays.x[0] = x;
      FitFunction.Evaluate(workingArrays.x, _parameters, workingArrays.y);
      return workingArrays.y[0];
    }
  }

}
