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

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.Regression.Nonlinear;

namespace Altaxo.Calc.Interpolation
{
  /// <summary>
  /// Uses a non-linear fit as a interpolation. Initial parameters for the fit must be provided beforehand, thus this interpolation
  /// is limited to use cases for which approximate parameters are already known.
  /// </summary>
  /// <seealso cref="Altaxo.Calc.Interpolation.IInterpolationFunctionOptions" />
  public class NonlinearFitAsInterpolation : IInterpolationFunctionOptions
  {
    /// <summary>
    /// Gets the shape of the fit.
    /// </summary>
    public IFitFunction CurveShape { get; init; }

    /// <summary>
    /// Gets the curve parameters for the <see cref="CurveShape"/> function.
    /// </summary>
    public ImmutableList<ParameterSetElement> Parameters { get; init; }


    #region Serialization

    /// <summary>
    /// 2024-02-27 V0 Serialization surrogate for <see cref="NonlinearFitAsInterpolation"/>.
    /// </summary>
    /// <seealso cref="Altaxo.Serialization.Xml.IXmlSerializationSurrogate" />
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(NonlinearFitAsInterpolation), 0)]
    public class SerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc />
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (NonlinearFitAsInterpolation)obj;
        info.AddValue("CurveShape", s.CurveShape);
        info.AddArray("CurveParameters", s.Parameters, s.Parameters.Count);
      }

      /// <inheritdoc />
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var curve = info.GetValue<IFitFunction>("CurveShape", null);
        var curveParameters = info.GetArrayOfValues<ParameterSetElement>("CurveParameters", null);
        return new NonlinearFitAsInterpolation(curve, curveParameters);
      }
    }

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="NonlinearFitAsInterpolation"/> class with a quadratic polynomial and zero parameters.
    /// </summary>
    public NonlinearFitAsInterpolation()
    {
      CurveShape = new Altaxo.Calc.FitFunctions.General.Polynomial(2, 0);
      var parameters = new List<ParameterSetElement>();
      for (int i = 0; i < CurveShape.NumberOfParameters; ++i)
      {
        parameters.Add(new ParameterSetElement(CurveShape.ParameterName(i), 0));
      }
      Parameters = parameters.ToImmutableList();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NonlinearFitAsInterpolation"/> class using numeric parameter values for the provided fit function.
    /// </summary>
    /// <param name="fitFunction">The fit function that defines the curve shape.</param>
    /// <param name="parameters">The parameter values for the fit function.</param>
    public NonlinearFitAsInterpolation(IFitFunction fitFunction, IReadOnlyList<double> parameters)
    {
      CurveShape = fitFunction ?? throw new ArgumentNullException(nameof(fitFunction));
      if (parameters is null)
        throw new ArgumentNullException(nameof(parameters));
      if (fitFunction.NumberOfParameters != parameters.Count)
        throw new ArgumentException($"Number of provided parameters is {parameters.Count}, but {fitFunction.NumberOfParameters} parameters are expected.");

      var para = new List<ParameterSetElement>();
      for (int i = 0; i < CurveShape.NumberOfParameters; ++i)
      {
        para.Add(new ParameterSetElement(CurveShape.ParameterName(i), parameters[i]));
      }
      Parameters = para.ToImmutableList();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NonlinearFitAsInterpolation"/> class using detailed parameter descriptors.
    /// </summary>
    /// <param name="fitFunction">The fit function that defines the curve shape.</param>
    /// <param name="parameters">The parameter descriptors to use for fitting.</param>
    public NonlinearFitAsInterpolation(IFitFunction fitFunction, IReadOnlyList<ParameterSetElement> parameters)
    {
      CurveShape = fitFunction ?? throw new ArgumentNullException(nameof(fitFunction));
      if (parameters is null)
        throw new ArgumentNullException(nameof(parameters));
      if (fitFunction.NumberOfParameters != parameters.Count)
        throw new ArgumentException($"Number of provided parameters is {parameters.Count}, but {fitFunction.NumberOfParameters} parameters are expected.");

      var para = new List<ParameterSetElement>();
      for (int i = 0; i < CurveShape.NumberOfParameters; ++i)
      {
        para.Add(parameters[i] with { Name = CurveShape.ParameterName(i) });
      }
      Parameters = para.ToImmutableList();
    }

    /// <summary>
    /// Fits the interpolation model to the provided data vectors.
    /// </summary>
    /// <param name="xvec">The x-coordinate data points.</param>
    /// <param name="yvec">The y-coordinate data points.</param>
    /// <param name="yStdDev">The standard deviations of the y-coordinates (optional).</param>
    /// <returns>An interpolation function that represents the fitted model.</returns>
    /// <inheritdoc />
    public IInterpolationFunction Interpolate(IReadOnlyList<double> xvec, IReadOnlyList<double> yvec, IReadOnlyList<double>? yStdDev = null)
    {
      var regression = new QuickNonlinearRegression(CurveShape);
      var (paraValues, isFixed, lowerBounds, upperBounds) = ParameterSetElement.GetFitArrays(Parameters);
      var regResult = regression.Fit(xvec, yvec, paraValues, lowerBounds, upperBounds, null, isFixed, CancellationToken.None);
      return new FitFuncAsInterpolationWrapper(CurveShape, regResult.MinimizingPoint.ToArray(), xvec.Min(), xvec.Max());
    }

    /// <inheritdoc />
    IInterpolationCurve IInterpolationCurveOptions.Interpolate(IReadOnlyList<double> xvec, IReadOnlyList<double> yvec, IReadOnlyList<double>? yStdDev)
    {
      return Interpolate(xvec, yvec, yStdDev);
    }

    /// <summary>
    /// Provides access to the fitted function through the <see cref="IInterpolationFunction"/> interface.
    /// </summary>
    private class FitFuncAsInterpolationWrapper : IInterpolationFunction
    {
      IFitFunction _fitFunction;
      double[] _parameters;
      double _xmin, _xmax;

      double[] X = new double[1];
      double[] Y = new double[1];

      /// <summary>
      /// Initializes a new instance of the <see cref="FitFuncAsInterpolationWrapper"/> class.
      /// </summary>
      /// <param name="fitFunction">The fit function used for interpolation.</param>
      /// <param name="parameters">The parameters resulting from the regression.</param>
      /// <param name="xmin">The minimum x-value observed.</param>
      /// <param name="xmax">The maximum x-value observed.</param>
      public FitFuncAsInterpolationWrapper(IFitFunction fitFunction, double[] parameters, double xmin, double xmax)
      {
        _fitFunction = fitFunction;
        _parameters = parameters;
        _xmin = xmin;
        _xmax = xmax;
      }

      /// <inheritdoc />
      public double GetXOfU(double u)
      {

        return (1 - u) * _xmin + (u) * _xmax;
      }

      /// <inheritdoc />
      public double GetYOfU(double u)
      {
        return GetYOfX(GetXOfU(u));
      }

      /// <inheritdoc />
      public double GetYOfX(double x)
      {
        X[0] = x;
        _fitFunction.Evaluate(X, _parameters, Y);
        return Y[0];
      }

      /// <inheritdoc />
      public void Interpolate(IReadOnlyList<double> xvec, IReadOnlyList<double> yvec)
      {
        throw new NotImplementedException();
      }
    }
  }
}
