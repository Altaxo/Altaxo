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
using System.Linq;
using Complex64 = System.Numerics.Complex;

namespace Altaxo.Calc.Interpolation
{
  /// <summary>
  /// Base class of the options for Prony series interpolation, both in the time domain as well as in the frequency domain.
  /// </summary>
  public record PronySeriesInterpolationBase
  {
    /// <summary>
    /// Gets the minimum and maximum x values to be used. If this property is null, then the minimum and maximum x is determined automatically.
    /// In time domain, the x values are times. In frequency domain, the x values are frequencies (frequencies, not circular frequencies!).
    /// </summary>
    public (double xMinimum, double xMaximum)? XMinimumMaximum { get; init; }

    /// <summary>
    /// If <see cref="PointsPerDecade"/> is 0, this property specifies a fixed number of Prony terms.
    /// Else, if <see cref="PointsPerDecade"/> is &gt; 0, this property specifies the maximum number of Prony terms.
    /// </summary>
    public int NumberOfPoints { get; init; } = int.MaxValue;

    /// <summary>
    /// Gets the number of Prony terms per decade. If this value is &lt;= 0, the property <see cref="NumberOfPoints"/> specifies a fixed number of Prony terms.
    /// Else, if this property is &gt; 0, it specifies the number of Prony terms per decade, and <see cref="NumberOfPoints"/> only specifies the maximum number of Prony terms.
    /// </summary>
    public double PointsPerDecade { get; init; } = 2;

    /// <summary>
    /// If true, the Prony terms model a relaxation process, i.e. a modulus, where the real part increases with frequency.
    /// If false, the Prony terms model a retardation process, i.e. a susceptibility, where the real part decreases with frequency.
    /// </summary>
    public bool IsRelaxation { get; init; }

    /// <summary>
    /// If true, besides of the Prony terms, additionally an intercept is fitted to the data.
    /// </summary>
    public bool UseIntercept { get; init; }

    /// <summary>
    /// If true, also negative Prony coefficients are allowed. The default value is false.
    /// </summary>
    public bool AllowNegativePronyCoefficients { get; init; }

    /// <summary>
    /// Gets or sets the regularization parameter that controls the smoothing of the resulting curve. The higher the parameter, the smoother the resulting curve will be.
    /// </summary>
    public double RegularizationParameter { get; init; }

    /// <summary>
    /// Serializes the current options set by using the legacy V0 schema.
    /// </summary>
    /// <param name="info">Serialization writer that receives the persisted data.</param>
    protected void SerializeV0(Altaxo.Serialization.Xml.IXmlSerializationInfo info)
    {
      info.AddValue("IsXMinimumMaximumSpecified", XMinimumMaximum.HasValue);
      if (XMinimumMaximum.HasValue)
      {
        info.AddValue("XMinimum", XMinimumMaximum.Value.xMinimum);
        info.AddValue("XMaximum", XMinimumMaximum.Value.xMinimum);
      }
      info.AddValue("NumberOfPoints", NumberOfPoints);
      info.AddValue("PointsPerDecade", PointsPerDecade);
      info.AddValue("IsRelaxation", IsRelaxation);
      info.AddValue("UseIntercept", UseIntercept);
      info.AddValue("AllowNegativeCoefficients", AllowNegativePronyCoefficients);
      info.AddValue("RegularizationParameter", RegularizationParameter);
    }

    /// <summary>
    /// Deserializes the legacy V0 schema and returns a new options record populated with the loaded data.
    /// </summary>
    /// <param name="info">Serialization reader that provides the persisted data.</param>
    /// <returns>The deserialized options record.</returns>
    protected PronySeriesInterpolationBase DeserializeV0(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    {
      (double xMinimum, double xMaximum)? xMinimumMaximum = null;
      var isXMinimumMaximumSpecified = info.GetBoolean("IsXMinimumMaximumSpecified");
      if (isXMinimumMaximumSpecified)
      {
        var xmin = info.GetDouble("XMinimum");
        var xmax = info.GetDouble("XMaximum");
        xMinimumMaximum = (xmin, xmax);
      }
      var numberOfPoints = info.GetInt32("NumberOfPoints");
      var pointsPerDecade = info.GetDouble("PointsPerDecade");
      var isRelaxation = info.GetBoolean("IsRelaxation");
      var useIntercept = info.GetBoolean("UseIntercept");
      var allowNegativeCoefficients = info.GetBoolean("AllowNegativeCoefficients");
      var regularizationParameter = info.GetDouble("RegularizationParameter");

      return this with
      {
        XMinimumMaximum = xMinimumMaximum,
        NumberOfPoints = numberOfPoints,
        PointsPerDecade = pointsPerDecade,
        IsRelaxation = isRelaxation,
        UseIntercept = useIntercept,
        AllowNegativePronyCoefficients = allowNegativeCoefficients,
        RegularizationParameter = regularizationParameter,
      };
    }


    /// <summary>
    /// Creates a copy with a manually specified x-range and a fixed number of Prony terms.
    /// </summary>
    /// <param name="xmin">Lower bound of the x-range.</param>
    /// <param name="xmax">Upper bound of the x-range.</param>
    /// <param name="numberOfPoints">Exact number of Prony terms to use.</param>
    /// <returns>A new record that reflects the provided parameters.</returns>
    public PronySeriesInterpolationBase WithSpecifiedXMinimumMaximumAndFixedNumberOfPoints(double xmin, double xmax, int numberOfPoints)
    {
      return this with
      {
        XMinimumMaximum = (xmin, xmax),
        NumberOfPoints = numberOfPoints,
        PointsPerDecade = 0,
      };
    }

    /// <summary>
    /// Creates a copy configured for retardation with automatically detected x-range and a given number of Prony terms per decade.
    /// </summary>
    /// <param name="pointsPerDecade">Number of Prony terms per decade; must be positive.</param>
    /// <returns>A new record configured for automatic x-range detection and bounded by the provided density.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="pointsPerDecade"/> is not positive.</exception>
    public PronySeriesInterpolationBase RetardationWithAutomaticXMinimumMaximumAndNumberOfPointsPerDecade(double pointsPerDecade)
    {
      if (!(pointsPerDecade > 0))
        throw new ArgumentOutOfRangeException(nameof(pointsPerDecade));

      return this with
      {
        XMinimumMaximum = null,
        NumberOfPoints = int.MaxValue,
        PointsPerDecade = pointsPerDecade,
      };
    }

    /// <summary>
    /// Creates a copy with automatically determined x-range and a fixed number of Prony terms.
    /// </summary>
    /// <param name="numberOfPoints">Exact number of Prony terms to use.</param>
    /// <returns>A new record configured for automatic x-range detection with the requested amount of points.</returns>
    public PronySeriesInterpolationBase WithAutomaticXMinimumMaximumAndFixedNumberOfPoints(int numberOfPoints)
    {
      return this with
      {
        XMinimumMaximum = null,
        NumberOfPoints = numberOfPoints,
        PointsPerDecade = 0,
      };
    }

    /// <summary>
    /// Creates a copy with automatically determined x-range, a user-specified number of points per decade, and a maximum number of Prony terms.
    /// </summary>
    /// <param name="pointsPerDecade">Number of Prony terms per decade.</param>
    /// <param name="numberOfPoints">Maximum number of Prony terms.</param>
    /// <returns>A new record configured accordingly.</returns>
    public PronySeriesInterpolationBase WithAutomaticXMinimumMaximumAndNumberOfPointsPerDecadeAndMaximumNumberOfPoints(double pointsPerDecade, int numberOfPoints)
    {
      return this with
      {
        XMinimumMaximum = null,
        NumberOfPoints = numberOfPoints,
        PointsPerDecade = pointsPerDecade,
      };
    }

    /// <summary>
    /// Determines the working x-range and number of points based on either user-specified bounds or the data.
    /// </summary>
    /// <param name="xvec">Original x data used to infer the working range when automatic detection is active.</param>
    /// <returns>The effective x-minimum, x-maximum, and number of points to be used.</returns>
    /// <exception cref="InvalidOperationException">Thrown when manually provided bounds are invalid.</exception>
    /// <exception cref="ArgumentException">Thrown when automatic detection cannot find positive x-values.</exception>
    protected (double workingXMinimum, double workingXMaximum, int workingNumberOfPoints) GetWorkingXMinMaxNumberOfPoints(IReadOnlyList<double> xvec)
    {
      double workingXMinimum, workingXMaximum;
      var workingNumberOfPoints = NumberOfPoints;

      if (XMinimumMaximum is { } minmax)
      {
        workingXMinimum = minmax.xMinimum;
        workingXMaximum = minmax.xMaximum;

        if (!(workingXMaximum > workingXMinimum))
          throw new InvalidOperationException($"Error with manually fixed x-minimum/x-maximum values of Prony interpolation. X-max should be greater than x-min, but is: xmin={workingXMinimum}, xmax={workingXMaximum}");
      }
      else // min max done automatically
      {
        var xmin = xvec.Min(x => x > 0 ? x : null);
        if (!xmin.HasValue)
          throw new ArgumentException("Prony interpolation: the x-array does not contain positive elements", nameof(xvec));

        var xmax = xvec.Max(x => x > 0 ? x : null);
        if (!xmax.HasValue)
          throw new ArgumentException("Prony interpolation: The x-array does not contain positive elements", nameof(xvec));

        if (xmin.Value == xmax.Value)
        {
          workingXMinimum = workingXMaximum = xmin.Value;
          workingNumberOfPoints = 1;
        }
        else
        {
          workingXMinimum = xmin.Value;
          workingXMaximum = xmax.Value;
          if (PointsPerDecade > 0)
          {
            workingNumberOfPoints = (int)(Math.Ceiling(Math.Log10(xmax.Value / xmin.Value) * PointsPerDecade) + 1);
            workingNumberOfPoints = Math.Min(workingNumberOfPoints, NumberOfPoints);
          }
        }
      }

      return (workingXMinimum, workingXMaximum, workingNumberOfPoints);
    }


    /// <summary>
    /// Wraps a real-valued interpolation delegate inside an <see cref="IInterpolationFunction"/> implementation.
    /// </summary>
    protected class InterpolationResultDoubleWrapper : IInterpolationFunction
    {
      /// <summary>
      /// Gets the delegate that produces interpolated y-values.
      /// </summary>
      Func<double, double> YOfX { get; }

      /// <summary>
      /// Initializes a new instance of the <see cref="InterpolationResultDoubleWrapper"/> class.
      /// </summary>
      /// <param name="yOfX">Delegate used to compute y-values for given x inputs.</param>
      public InterpolationResultDoubleWrapper(Func<double, double> yOfX)
      {
        YOfX = yOfX;
      }

      /// <inheritdoc/>
      public double GetXOfU(double u)
      {
        return u;
      }

      /// <inheritdoc/>
      public double GetYOfU(double u)
      {
        return YOfX(u);
      }

      /// <inheritdoc/>
      public double GetYOfX(double x)
      {
        return YOfX(x);
      }

      /// <inheritdoc/>
      public void Interpolate(IReadOnlyList<double> xvec, IReadOnlyList<double> yvec)
      {
        throw new NotImplementedException();
      }
    }

    /// <summary>
    /// Wraps a complex-valued interpolation delegate inside an <see cref="IComplexInterpolationFunction"/> implementation.
    /// </summary>
    protected class InterpolationResultComplexWrapper : IComplexInterpolationFunction
    {
      /// <summary>
      /// Gets the delegate that produces complex y-values.
      /// </summary>
      Func<double, Complex64> YOfX { get; }

      /// <summary>
      /// Initializes a new instance of the <see cref="InterpolationResultComplexWrapper"/> class.
      /// </summary>
      /// <param name="yOfX">Delegate used to compute complex y-values for given x inputs.</param>
      public InterpolationResultComplexWrapper(Func<double, Complex64> yOfX)
      {
        YOfX = yOfX;
      }

      /// <summary>
      /// Returns the u-value unchanged and effectively acts as an identity transformation.
      /// </summary>
      /// <param name="u">The parameter value.</param>
      /// <returns>The same value that was provided.</returns>
      public double GetXOfU(double u)
      {
        return u;
      }

      /// <summary>
      /// Gets the complex y-value using the delegate and the provided parameter.
      /// </summary>
      /// <param name="u">The parameter value.</param>
      /// <returns>The complex y-value for <paramref name="u"/>.</returns>
      public Complex64 GetYOfU(double u)
      {
        return YOfX(u);
      }

      /// <inheritdoc/>
      public Complex64 GetYOfX(double x)
      {
        return YOfX(x);
      }

      /// <summary>
      /// Not implemented because this wrapper only exposes evaluation delegates.
      /// </summary>
      /// <param name="xvec">Ignored.</param>
      /// <param name="yvec">Ignored.</param>
      /// <exception cref="NotImplementedException">Always thrown.</exception>
      public void Interpolate(IReadOnlyList<double> xvec, IReadOnlyList<double> yvec)
      {
        throw new NotImplementedException();
      }
    }
  }
}
