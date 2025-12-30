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
using System.Text;
using Altaxo.Calc;
using Altaxo.Calc.Regression;

namespace Altaxo.Science.Signals
{
  /// <summary>
  /// Executes a four point step evaluation and stores the result.
  /// Two points on the curve define a left straight line, and two other points on the curve define a right straight line.
  /// The step should be located in between the two inner points. The step position is then evaluated by building a regression
  /// line of the curve between the inner points, but only from a certain level to another given level (e.g., from 25% to 75% of the distance between the left and right line).
  /// </summary>
  public class FourPointStepEvaluation : Main.IImmutable
  {
    private StringBuilder? _errors;
    /// <summary>
    /// If true, any error will throw an <see cref="InvalidOperationException"/>.
    /// </summary>
    private bool _throwOnError = true;

    /// <summary>
    /// Gets the error message(s).
    /// </summary>
    public string Errors => _errors is null ? string.Empty : _errors.ToString();

    /// <summary>
    /// Gets a value indicating whether during the evaluation an error occurred.
    /// </summary>
    public bool HasErrors => _errors is not null && _errors.Length > 0;

    /// <summary>
    /// Gets the index of the left outer point of the step evaluation.
    /// </summary>
    public double IndexLeftOuter { get; private set; } = double.NaN;

    /// <summary>
    /// Gets the index of the left inner point of the step evaluation.
    /// </summary>
    public double IndexLeftInner { get; private set; } = double.NaN;

    /// <summary>
    /// Gets the index of the right inner point of the step evaluation.
    /// </summary>
    public double IndexRightInner { get; private set; } = double.NaN;

    /// <summary>
    /// Gets the index of the right outer point of the step evaluation.
    /// </summary>
    public double IndexRightOuter { get; private set; } = double.NaN;

    /// <summary>
    /// Gets the regression levels for the middle line. For instance, if the value is set to (0.25, 0.75), then all points between 25% and 75% distance from the left and right line are used
    /// for the regression of the middle line.
    /// </summary>
    /// <value>
    /// The regression limits. Both values must be between 0 and 1, and the first value must be smaller than the second value.
    /// </value>
    public (double LowerLevel, double UpperLevel) MiddleRegressionLevels { get; private set; } = (double.NaN, double.NaN);

    /// <summary>
    /// Gets the x value of the step middle point.
    /// </summary>
    public double MiddlePointX { get; private set; } = double.NaN;

    /// <summary>
    /// Gets the y value of the step middle point.
    /// </summary>
    public double MiddlePointY { get; private set; } = double.NaN;

    /// <summary>
    /// Gets the x and y value of the step middle point.
    /// </summary>
    public (double X, double Y) MiddlePoint => (MiddlePointX, MiddlePointY);

    /// <summary>
    /// Gets the left regression.
    /// </summary>
    public QuickLinearRegression LeftRegression { get; private set; } = QuickLinearRegression.Invalid;

    /// <summary>
    /// Gets the right regression.
    /// </summary>
    public QuickLinearRegression RightRegression { get; private set; } = QuickLinearRegression.Invalid;

    /// <summary>
    /// Gets the middle regression.
    /// </summary>
    public QuickLinearRegression MiddleRegression { get; private set; } = QuickLinearRegression.Invalid;

    /// <summary>
    /// Gets the intersection point between the left regression line and the middle regression line.
    /// </summary>
    public (double X, double Y) IntersectionPointLeftMiddle { get; private set; } = (double.NaN, double.NaN);

    /// <summary>
    /// Gets the intersection point between the right regression line and the middle regression line.
    /// </summary>
    public (double X, double Y) IntersectionPointRightMiddle { get; private set; } = (double.NaN, double.NaN);

    /// <summary>
    /// Gets the height of the step.
    /// </summary>
    /// <value>
    /// The step height is defined as the difference of the y-values between the left and right regression line at the middle point's x-value.
    /// </value>
    public double StepHeight { get; private set; } = double.NaN;

    /// <summary>
    /// Gets the width of the step.
    /// </summary>
    /// <value>
    /// The width of the step is defined as the absolute difference of the x-values of the intersection points of the left and right regression line with the middle regression line.
    /// </value>
    public double StepWidth { get; private set; } = double.NaN;

    /// <summary>
    /// Gets the step slope.
    /// </summary>
    /// <value>
    /// The step slope is the slope of the middle regression line.
    /// </value>
    public double StepSlope { get; private set; } = double.NaN;


    /// <summary>
    /// Creates a step evaluation from indices.
    /// </summary>
    /// <param name="x">The x array.</param>
    /// <param name="y">The y array.</param>
    /// <param name="indexLeftOuter">The index of the left outer point.</param>
    /// <param name="indexLeftInner">The index of the left inner point.</param>
    /// <param name="indexRightInner">The index of the right inner point.</param>
    /// <param name="indexRightOuter">The index of the right outer point.</param>
    /// <param name="useRegressionForLeftAndRightLine">
    /// If set to <c>true</c>, a full regression is used for the left and right line.
    /// If set to <c>false</c>, only the inner and outer points are used to form the line.
    /// </param>
    /// <param name="middleRegressionLevels">
    /// The regression levels for the middle line. For instance, if the value is set to (0.25, 0.75), then all points between 25% and 75% distance from the left and right line are used
    /// for the regression of the middle line.
    /// </param>
    /// <param name="throwOnError">
    /// If true, any error in the evaluation will throw an <see cref="InvalidOperationException"/>. If false,
    /// no exception is thrown; instead, the error is stored and can be read out using the property <see cref="Errors"/>.
    /// </param>
    /// <returns>The result of the step evaluation.</returns>
    public static FourPointStepEvaluation CreateFromIndices(IReadOnlyList<double> x, IReadOnlyList<double> y, double indexLeftOuter, double indexLeftInner, double indexRightInner, double indexRightOuter, bool useRegressionForLeftAndRightLine, (double LowerLevel, double UpperLevel) middleRegressionLevels, bool throwOnError = true)
    {
      return new FourPointStepEvaluation(x, y, indexLeftOuter, indexLeftInner, indexRightInner, indexRightOuter, useRegressionForLeftAndRightLine, middleRegressionLevels, throwOnError);
    }

    /// <summary>
    /// Creates the step evaluation from the x-values of the left outer and inner point and the right inner and outer point.
    /// </summary>
    /// <param name="x">The x array.</param>
    /// <param name="y">The y array.</param>
    /// <param name="xLeftOuter">The x-value of the left outer point.</param>
    /// <param name="xLeftInner">The x-value of the left inner point.</param>
    /// <param name="xRightInner">The x-value of the right inner point.</param>
    /// <param name="xRightOuter">The x-value of the right outer point.</param>
    /// <param name="useRegressionForLeftAndRightLine">
    /// If set to <c>true</c>, a full regression is used for the left and right line.
    /// If set to <c>false</c>, only the inner and outer points are used to form the line.
    /// </param>
    /// <param name="middleRegressionLevels">
    /// The regression levels for the middle line. For instance, if the value is set to (0.25, 0.75), then all points between 25% and 75% distance from the left and right line are used
    /// for the regression of the middle line.
    /// </param>
    /// <param name="throwOnError">
    /// If true, any error in the evaluation will throw an <see cref="InvalidOperationException"/>. If false,
    /// no exception is thrown; instead, the error is stored and can be read out using the property <see cref="Errors"/>.
    /// </param>
    /// <returns>The result of the step evaluation.</returns>
    public static FourPointStepEvaluation CreateFromValues(IReadOnlyList<double> x, IReadOnlyList<double> y, double xLeftOuter, double xLeftInner, double xRightInner, double xRightOuter, bool useRegressionForLeftAndRightLine, (double LowerLevel, double UpperLevel) middleRegressionLevels, bool throwOnError = true)
    {
      var (indexLeftOuter, foundLeftOuter) = RMath.FindNearestIndex(x, xLeftOuter);
      var (indexLeftInner, foundLeftInner) = RMath.FindNearestIndex(x, xLeftInner);
      var (indexRightInner, foundRightInner) = RMath.FindNearestIndex(x, xRightInner);
      var (indexRightOuter, foundRightOuter) = RMath.FindNearestIndex(x, xRightOuter);
      return new FourPointStepEvaluation(x, y, indexLeftOuter, indexLeftInner, indexRightInner, indexRightOuter, useRegressionForLeftAndRightLine, middleRegressionLevels, throwOnError);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FourPointStepEvaluation"/> class.
    /// </summary>
    /// <param name="x">The x array.</param>
    /// <param name="y">The y array.</param>
    /// <param name="indexLeftOuter">The index of the left outer point.</param>
    /// <param name="indexLeftInner">The index of the left inner point.</param>
    /// <param name="indexRightInner">The index of the right inner point.</param>
    /// <param name="indexRightOuter">The index of the right outer point.</param>
    /// <param name="useRegressionForLeftAndRightLine">
    /// If set to <c>true</c>, a full regression is used for the left and right line.
    /// If set to <c>false</c>, only the inner and outer points are used to form the line.
    /// </param>
    /// <param name="middleRegressionLevels">The regression levels used to select points for the middle regression line.</param>
    /// <param name="throwOnError">If true, throws an exception on error; otherwise errors are accumulated in <see cref="Errors"/>.</param>
    private FourPointStepEvaluation(IReadOnlyList<double> x, IReadOnlyList<double> y, double indexLeftOuter, double indexLeftInner, double indexRightInner, double indexRightOuter, bool useRegressionForLeftAndRightLine, (double LowerLevel, double UpperLevel) middleRegressionLevels, bool throwOnError)
    {
      _throwOnError = throwOnError;

      if (x is null)
      {
        throw new ArgumentNullException(nameof(x));
      }
      if (y is null)
      {
        throw new ArgumentNullException(nameof(y));
      }
      if (x.Count != y.Count)
      {
        throw new ArgumentException("The x and y arrays must have the same length.");
      }

      if (!(middleRegressionLevels.LowerLevel >= 0 && middleRegressionLevels.LowerLevel <= 1) ||
         !(middleRegressionLevels.UpperLevel >= 0 && middleRegressionLevels.UpperLevel <= 1) ||
         !(middleRegressionLevels.LowerLevel < middleRegressionLevels.UpperLevel)
        )
      {
        throw new ArgumentException($"LowerLevel and UpperLevel should be both in the range [0,1], and LowerLevel<UpperLevel, but it was {middleRegressionLevels}", nameof(middleRegressionLevels));
      }

      MiddleRegressionLevels = middleRegressionLevels;

      var rowCount = x.Count;

      // test the data
      if (indexLeftOuter < 0 && indexLeftOuter >= rowCount)
      {
        ReportError($"The index of the left outer point has the invalid value #{indexLeftOuter}. It should be in the interval [0, {rowCount - 1}]");
        return;
      }
      if (indexLeftInner < 0 && indexLeftInner >= rowCount)
      {
        ReportError($"The index of the left inner point has the invalid value #{indexLeftInner}. It should be in the interval [0, {rowCount - 1}]");
        return;
      }
      if (indexRightInner < 0 && indexRightInner >= rowCount)
      {
        ReportError($"The index of the right inner point has the invalid value #{indexRightInner}. It should be in the interval [0, {rowCount - 1}]");
        return;
      }
      if (indexRightOuter < 0 && indexRightOuter >= rowCount)
      {
        ReportError($"The index of the right outer point has the invalid value #{indexRightOuter}. It should be in the interval [0, {rowCount - 1}]");
        return;
      }

      IndexLeftInner = indexLeftInner;
      IndexLeftOuter = indexLeftOuter;
      IndexRightInner = indexRightInner;
      IndexRightOuter = indexRightOuter;

      var leftRegression = GetLeftRightRegression(x, y, indexLeftOuter, indexLeftInner, useRegressionForLeftAndRightLine);

      if (!leftRegression.IsValid)
      {
        ReportError($"The left regression line is not valid.");
        return;
      }

      var rightRegression = GetLeftRightRegression(x, y, indexRightInner, indexRightOuter, useRegressionForLeftAndRightLine);

      if (!rightRegression.IsValid)
      {
        ReportError($"The right regression line is not valid.");
        return;
      }

      // both lines must not intersect in the inner region

      var (intersectionX, _) = leftRegression.GetIntersectionPoint(rightRegression);
      var (xinnerleft, xinnerright) = RMath.MinMax(RMath.InterpolateLinear(indexLeftInner, x), RMath.InterpolateLinear(indexRightInner, x));

      if (RMath.IsInIntervalCC(intersectionX, xinnerleft, xinnerright))
      {
        ReportError($"The left and right line intersect in the inner region. This is not allowed.");
        return;
      }

      // create the middle regression line
      var middleRegression = GetMiddleRegression(x, y, indexLeftInner, indexRightInner, leftRegression, rightRegression, middleRegressionLevels.LowerLevel, middleRegressionLevels.UpperLevel);

      if (!middleRegression.IsValid)
      {
        ReportError($"The middle regression line is not valid.");
        return;
      }


      // now find the point on the middle regression line where the relative y between left and right regression is 0.5
      MiddlePointX = (leftRegression.GetA0() + rightRegression.GetA0() - 2 * middleRegression.GetA0()) / (2 * middleRegression.GetA1() - leftRegression.GetA1() - rightRegression.GetA1());
      MiddlePointY = middleRegression.GetYOfX(MiddlePointX);

      LeftRegression = leftRegression;
      RightRegression = rightRegression;
      MiddleRegression = middleRegression;

      IntersectionPointLeftMiddle = leftRegression.GetIntersectionPoint(middleRegression);
      IntersectionPointRightMiddle = rightRegression.GetIntersectionPoint(middleRegression);

      StepHeight = Math.Abs(leftRegression.GetYOfX(MiddlePointX) - rightRegression.GetYOfX(MiddlePointX));
      StepWidth = Math.Abs(IntersectionPointLeftMiddle.X - IntersectionPointRightMiddle.X);
      StepSlope = MiddleRegression.GetA1();
    }


    /// <summary>
    /// Gets the regression for the left or the right line.
    /// </summary>
    /// <param name="x">The x values.</param>
    /// <param name="y">The y values.</param>
    /// <param name="index1">The start index.</param>
    /// <param name="index2">The end index (inclusive).</param>
    /// <param name="useAllPointsForRegression">If set to <c>true</c>, all points from index1 to index2 are used
    /// to create the linear regression; otherwise, only point[index1] and point[index2] are used to calculate the line.</param>
    /// <returns>The regression that forms a line (either the left line of the step or the right line).</returns>
    public static QuickLinearRegression GetLeftRightRegression(IReadOnlyList<double> x, IReadOnlyList<double> y, double index1, double index2, bool useAllPointsForRegression)
    {
      var min = Math.Min(index1, index2);
      var max = Math.Max(index1, index2);
      var result = new QuickLinearRegression();

      if (useAllPointsForRegression)
      {
        int i = (int)min;
        if (Math.IEEERemainder(min, 1) != 0)
        {
          result.Add(RMath.InterpolateLinear(min, x), RMath.InterpolateLinear(min, y));
          i = (int)Math.Ceiling(min);
        }

        for (; i <= max; ++i)
        {
          result.Add(x[i], y[i]);
        }

        if (Math.IEEERemainder(max, 1) != 0)
        {
          result.Add(RMath.InterpolateLinear(max, x), RMath.InterpolateLinear(max, y));
        }
      }
      else
      {
        result.Add(RMath.InterpolateLinear(min, x), RMath.InterpolateLinear(min, y));
        result.Add(RMath.InterpolateLinear(max, x), RMath.InterpolateLinear(max, y));
      }

      return result;
    }

    /// <summary>
    /// Gets the middle regression line.
    /// </summary>
    /// <param name="x">The x values.</param>
    /// <param name="y">The y values.</param>
    /// <param name="index1">The start index of the middle section.</param>
    /// <param name="index2">The end index of the middle section.</param>
    /// <param name="leftRegression">The left regression line.</param>
    /// <param name="rightRegression">The right regression line.</param>
    /// <param name="lowerRegressionLevel">The lower regression level (0..1). Usually, it is 0.25.</param>
    /// <param name="upperRegressionLevel">The upper regression level (0..1). Usually, it is 0.75.</param>
    /// <returns>The regression that forms the middle line of the step.</returns>
    public static QuickLinearRegression GetMiddleRegression(IReadOnlyList<double> x, IReadOnlyList<double> y, double index1, double index2, QuickLinearRegression leftRegression, QuickLinearRegression rightRegression, double lowerRegressionLevel, double upperRegressionLevel)
    {
      var min = Math.Min(index1, index2);
      var max = Math.Max(index1, index2);
      var result = new QuickLinearRegression();

      int i = (int)min;
      if (Math.IEEERemainder(min, 1) != 0)
      {
        result.Add(RMath.InterpolateLinear(min, x), RMath.InterpolateLinear(min, y));
        i = (int)Math.Ceiling(min);
      }
      for (; i <= max; ++i)
      {
        var r = QuickLinearRegression.GetRelativeYBetweenRegressions(leftRegression, rightRegression, x[i], y[i]);
        if (RMath.IsInIntervalCC(r, lowerRegressionLevel, upperRegressionLevel))
        {
          result.Add(x[i], y[i]);
        }
      }
      if (Math.IEEERemainder(max, 1) != 0)
      {
        result.Add(RMath.InterpolateLinear(max, x), RMath.InterpolateLinear(max, y));
      }

      return result;
    }

    /// <summary>
    /// Reports an error either by throwing an exception or by collecting the message.
    /// </summary>
    /// <param name="message">The error message.</param>
    private void ReportError(string message)
    {
      if (_throwOnError)
      {
        throw new InvalidOperationException(message);
      }
      else
      {
        _errors ??= new StringBuilder();
        _errors.AppendLine(message);
      }
    }
  }
}
