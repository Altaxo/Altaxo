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
using Altaxo.Calc;
using Altaxo.Calc.Regression;
using Altaxo.Data;

namespace Altaxo.Science.Signals
{
  public class FourPointStepEvaluationDataSource : TableDataSourceBaseImmutableOptions<FourPointStepEvaluationOptions, XAndYColumn>
  {
    public const string ColumnNameLeftX = "LeftX";
    public const string ColumnNameLeftY = "LeftY";
    public const string ColumnNameRightX = "RightX";
    public const string ColumnNameRightY = "RightY";
    public const string ColumnNameMiddleX = "MiddleX";
    public const string ColumnNameMiddleY = "MiddleY";
    public const string ColumnNameCurveX = "CurveX";
    public const string ColumnNameCurveY = "CurveY";
    public const string ColumnNameParameterName = "ParameterName";
    public const string ColumnNameParameterValue = "ParameterValue";
    public const int ColumnGroupNumberLeft = 0;
    public const int ColumnGroupNumberRight = 1;
    public const int ColumnGroupNumberMiddle = 2;
    public const int ColumnGroupParameter = 3;
    public const int ColumnGroupCurveValues = 4;

    public const string ParameterNameMiddleX = "StepMiddleX";
    public const string ParameterNameMiddleY = "StepMiddleY";
    public const string ParameterNameStepMiddleSlope = "StepMiddleSlope";
    public const string ParameterNameStepHeight = "StepHeight";
    public const string ParameterNameStepWidth = "StepWidth";
    public const string ParameterNameStepLeftX = "StepLeftX";
    public const string ParameterNameStepLeftY = "StepLeftY";
    public const string ParameterNameStepLeftSlope = "StepLeftSlope";
    public const string ParameterNameStepRightX = "StepRightX";
    public const string ParameterNameStepRightY = "StepRightY";
    public const string ParameterNameStepRightSlope = "StepRightSlope";

    public IEnumerable<string> AllParameterNames
    {
      get
      {
        yield return ParameterNameMiddleX;
        yield return ParameterNameMiddleY;
        yield return ParameterNameStepMiddleSlope;
        yield return ParameterNameStepHeight;
        yield return ParameterNameStepWidth;
        yield return ParameterNameStepLeftX;
        yield return ParameterNameStepLeftY;
        yield return ParameterNameStepLeftSlope;
        yield return ParameterNameStepRightX;
        yield return ParameterNameStepRightY;
        yield return ParameterNameStepRightSlope;
      }
    }

    #region Serialization

    #region Version 0

    /// <summary>
    /// 2024-12-22 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(FourPointStepEvaluationDataSource), 0)]
    private class XmlSerializationSurrogate0 : XmlSerializationSurrogateBase
    {
      public override object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return new FourPointStepEvaluationDataSource(info, 0);
      }
    }

    protected FourPointStepEvaluationDataSource(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, int version)
    : base(info, version)
    {
    }

    #endregion Version 0

    #endregion Serialization

    /// <summary>
    /// Initializes a new instance of the <see cref="ConvertXYVToMatrixDataSource"/> class.
    /// </summary>
    /// <param name="inputData">The input data designates the original source of data (used then for the processing).</param>
    /// <param name="dataSourceOptions">The Fourier transformation options.</param>
    /// <param name="importOptions">The data source import options.</param>
    public FourPointStepEvaluationDataSource(XAndYColumn inputData, FourPointStepEvaluationOptions dataSourceOptions, IDataSourceImportOptions importOptions)
        : base(inputData, dataSourceOptions, importOptions)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FourPointStepEvaluationDataSource"/> class.
    /// </summary>
    /// <param name="from">Another instance to copy from.</param>
    public FourPointStepEvaluationDataSource(FourPointStepEvaluationDataSource from)
      : base(from)
    {
    }

    /// <summary>
    /// Fills (or refills) the data table with the processed data. The data source is represented by this instance, the destination table is provided in the argument <paramref name="destinationTable" />.
    /// </summary>
    /// <param name="destinationTable">The destination table.</param>
    /// <param name="reporter"></param>
    public override void FillData_Unchecked(DataTable destinationTable, IProgressReporter reporter)
    {
      destinationTable.DataColumns.RemoveColumnsAll();
      destinationTable.PropCols.RemoveColumnsAll();
      foreach (var parameterName in AllParameterNames)
      {
        destinationTable.PropertyBagNotNull.RemoveValue(parameterName);
      }


      var (x, y, rowCount) = ProcessData.GetResolvedXYData();

      if (rowCount == 0 || x is null || y is null)
      {
        return;
      }

      var options = ProcessOptions;

      // test the data
      if (options.IndexLeftOuter < 0 && options.IndexLeftOuter >= rowCount)
      {
        ReportError(destinationTable, $"The index of the left outer point has the invalid value #{options.IndexLeftOuter}. It should be in the interval [0, {rowCount - 1}]");
      }
      if (options.IndexLeftInner < 0 && options.IndexLeftInner >= rowCount)
      {
        ReportError(destinationTable, $"The index of the left inner point has the invalid value #{options.IndexLeftInner}. It should be in the interval [0, {rowCount - 1}]");
      }
      if (options.IndexRightInner < 0 && options.IndexRightInner >= rowCount)
      {
        ReportError(destinationTable, $"The index of the right inner point has the invalid value #{options.IndexRightInner}. It should be in the interval [0, {rowCount - 1}]");
      }
      if (options.IndexRightOuter < 0 && options.IndexRightOuter >= rowCount)
      {
        ReportError(destinationTable, $"The index of the right outer point has the invalid value #{options.IndexRightOuter}. It should be in the interval [0, {rowCount - 1}]");
      }

      var leftRegression = GetLeftRightRegression(x, y, options.IndexLeftOuter, options.IndexLeftInner, ProcessOptions.UseRegressionForLeftAndRightLine);

      if (!leftRegression.IsValid)
      {
        ReportError(destinationTable, $"The left regression line is not valid.");
        return;
      }

      var rightRegression = GetLeftRightRegression(x, y, options.IndexRightInner, options.IndexRightOuter, ProcessOptions.UseRegressionForLeftAndRightLine);

      if (!rightRegression.IsValid)
      {
        ReportError(destinationTable, $"The right regression line is not valid.");
        return;
      }

      // both lines must not intersect in the inner region

      var (intersectionX, _) = leftRegression.GetIntersectionPoint(rightRegression);
      var (xinnerleft, xinnerright) = RMath.MinMax(x[options.IndexLeftInner], x[options.IndexRightInner]);

      if (RMath.IsInIntervalCC(intersectionX, xinnerleft, xinnerright))
      {
        ReportError(destinationTable, $"The left and right line intersect in the inner region. This is not allowed.");
      }

      // create the middle regression line
      var middleRegression = GetMiddleRegression(x, y, options.IndexLeftInner, options.IndexRightInner, leftRegression, rightRegression, ProcessOptions.MiddleRegressionLevels.LowerLevel, ProcessOptions.MiddleRegressionLevels.UpperLevel);

      if (!middleRegression.IsValid)
      {
        ReportError(destinationTable, $"The middle regression line is not valid.");
      }


      // now find the point on the middle regression line where the relative y between left and right regression is 0.5
      var xmiddle = (leftRegression.GetA0() + rightRegression.GetA0() - 2 * middleRegression.GetA0()) / (2 * middleRegression.GetA1() - leftRegression.GetA1() - rightRegression.GetA1());
      var ymiddle = middleRegression.GetYOfX(xmiddle);

      // now fill the data table


      destinationTable.DataColumns.EnsureExistence(ColumnNameLeftX, typeof(DoubleColumn), ColumnKind.X, ColumnGroupNumberLeft);
      destinationTable.DataColumns.EnsureExistence(ColumnNameLeftY, typeof(DoubleColumn), ColumnKind.V, ColumnGroupNumberLeft);
      destinationTable.DataColumns.EnsureExistence(ColumnNameRightX, typeof(DoubleColumn), ColumnKind.X, ColumnGroupNumberRight);
      destinationTable.DataColumns.EnsureExistence(ColumnNameRightY, typeof(DoubleColumn), ColumnKind.V, ColumnGroupNumberRight);
      destinationTable.DataColumns.EnsureExistence(ColumnNameMiddleX, typeof(DoubleColumn), ColumnKind.X, ColumnGroupNumberMiddle);
      destinationTable.DataColumns.EnsureExistence(ColumnNameMiddleY, typeof(DoubleColumn), ColumnKind.V, ColumnGroupNumberMiddle);
      destinationTable.DataColumns.EnsureExistence(ColumnNameParameterName, typeof(TextColumn), ColumnKind.X, ColumnGroupParameter);
      destinationTable.DataColumns.EnsureExistence(ColumnNameParameterValue, typeof(DoubleColumn), ColumnKind.V, ColumnGroupParameter);
      if (ProcessOptions.IncludeOriginalPointsInOutput)
      {
        destinationTable.DataColumns.EnsureExistence(ColumnNameCurveX, typeof(DoubleColumn), ColumnKind.X, ColumnGroupCurveValues);
        destinationTable.DataColumns.EnsureExistence(ColumnNameCurveY, typeof(DoubleColumn), ColumnKind.V, ColumnGroupCurveValues);
      }

      // now fill the table

      destinationTable[ColumnNameLeftX][0] = x[options.IndexLeftOuter];
      destinationTable[ColumnNameLeftY][0] = leftRegression.GetYOfX(x[options.IndexLeftOuter]);
      destinationTable[ColumnNameLeftX][1] = x[options.IndexLeftInner];
      destinationTable[ColumnNameLeftY][1] = leftRegression.GetYOfX(x[options.IndexLeftInner]);
      destinationTable[ColumnNameLeftX][2] = x[options.IndexRightInner];
      destinationTable[ColumnNameLeftY][2] = leftRegression.GetYOfX(x[options.IndexRightInner]);

      destinationTable[ColumnNameRightX][0] = x[options.IndexLeftInner];
      destinationTable[ColumnNameRightY][0] = rightRegression.GetYOfX(x[options.IndexLeftInner]);
      destinationTable[ColumnNameRightX][1] = x[options.IndexRightInner];
      destinationTable[ColumnNameRightY][1] = rightRegression.GetYOfX(x[options.IndexRightInner]);
      destinationTable[ColumnNameRightX][2] = x[options.IndexRightOuter];
      destinationTable[ColumnNameRightY][2] = rightRegression.GetYOfX(x[options.IndexRightOuter]);

      var (xl, yl) = leftRegression.GetIntersectionPoint(middleRegression);
      var (xr, yr) = rightRegression.GetIntersectionPoint(middleRegression);

      var xspan = xr - xl;
      var xlo = xl - options.MiddleLineOverlap * xspan;
      destinationTable[ColumnNameMiddleX][0] = xlo;
      destinationTable[ColumnNameMiddleY][0] = middleRegression.GetYOfX(xlo);

      destinationTable[ColumnNameMiddleX][1] = xl;
      destinationTable[ColumnNameMiddleY][1] = yl;

      destinationTable[ColumnNameMiddleX][2] = xmiddle;
      destinationTable[ColumnNameMiddleY][2] = ymiddle;

      destinationTable[ColumnNameMiddleX][3] = xr;
      destinationTable[ColumnNameMiddleY][3] = yr;

      var xro = xr + options.MiddleLineOverlap * xspan;
      destinationTable[ColumnNameMiddleX][4] = xro;
      destinationTable[ColumnNameMiddleY][4] = middleRegression.GetYOfX(xro);

      // now store all parameters
      var (xleft, yleft) = leftRegression.GetIntersectionPoint(middleRegression);
      var (xright, yright) = rightRegression.GetIntersectionPoint(middleRegression);
      int idxPara = 0;
      destinationTable[ColumnNameParameterName][idxPara] = ParameterNameMiddleX;
      destinationTable[ColumnNameParameterValue][idxPara] = xmiddle;
      destinationTable.PropertyBagNotNull.SetValue(ParameterNameMiddleX, xmiddle);
      ++idxPara;
      destinationTable[ColumnNameParameterName][idxPara] = ParameterNameMiddleY;
      destinationTable[ColumnNameParameterValue][idxPara] = ymiddle;
      destinationTable.PropertyBagNotNull.SetValue(ParameterNameMiddleY, ymiddle);
      ++idxPara;
      destinationTable[ColumnNameParameterName][idxPara] = ParameterNameStepMiddleSlope;
      destinationTable[ColumnNameParameterValue][idxPara] = middleRegression.GetA1();
      destinationTable.PropertyBagNotNull.SetValue(ParameterNameStepMiddleSlope, middleRegression.GetA1());
      ++idxPara;
      destinationTable[ColumnNameParameterName][idxPara] = ParameterNameStepHeight;
      destinationTable[ColumnNameParameterValue][idxPara] = Math.Abs(leftRegression.GetYOfX(xmiddle) - rightRegression.GetYOfX(xmiddle));
      destinationTable.PropertyBagNotNull.SetValue(ParameterNameStepHeight, Math.Abs(leftRegression.GetYOfX(xmiddle) - rightRegression.GetYOfX(xmiddle)));
      ++idxPara;
      destinationTable[ColumnNameParameterName][idxPara] = ParameterNameStepWidth;
      destinationTable[ColumnNameParameterValue][idxPara] = Math.Abs(xright - xleft);
      destinationTable.PropertyBagNotNull.SetValue(ParameterNameStepWidth, Math.Abs(xright - xleft));
      ++idxPara;
      destinationTable[ColumnNameParameterName][idxPara] = ParameterNameStepLeftX;
      destinationTable[ColumnNameParameterValue][idxPara] = xleft;
      destinationTable.PropertyBagNotNull.SetValue(ParameterNameStepLeftX, xleft);
      ++idxPara;
      destinationTable[ColumnNameParameterName][idxPara] = ParameterNameStepLeftY;
      destinationTable[ColumnNameParameterValue][idxPara] = yleft;
      destinationTable.PropertyBagNotNull.SetValue(ParameterNameStepLeftY, yleft);
      ++idxPara;
      destinationTable[ColumnNameParameterName][idxPara] = ParameterNameStepLeftSlope;
      destinationTable[ColumnNameParameterValue][idxPara] = leftRegression.GetA1();
      ++idxPara;
      destinationTable[ColumnNameParameterName][idxPara] = ParameterNameStepRightX;
      destinationTable[ColumnNameParameterValue][idxPara] = xright;
      destinationTable.PropertyBagNotNull.SetValue(ParameterNameStepRightX, xright);
      ++idxPara;
      destinationTable[ColumnNameParameterName][idxPara] = ParameterNameStepRightY;
      destinationTable[ColumnNameParameterValue][idxPara] = yright;
      destinationTable.PropertyBagNotNull.SetValue(ParameterNameStepRightY, yright);
      ++idxPara;
      destinationTable[ColumnNameParameterName][idxPara] = ParameterNameStepRightSlope;
      destinationTable[ColumnNameParameterValue][idxPara] = rightRegression.GetA1();
      destinationTable.PropertyBagNotNull.SetValue(ParameterNameStepRightSlope, rightRegression.GetA1());

      // output the curve values
      if (ProcessOptions.IncludeOriginalPointsInOutput)
      {
        for (int i = 0; i < rowCount; ++i)
        {
          destinationTable[ColumnNameCurveX][i] = x[i];
          destinationTable[ColumnNameCurveY][i] = y[i];
        }
      }
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
    public static QuickLinearRegression GetLeftRightRegression(double[] x, double[] y, int index1, int index2, bool useAllPointsForRegression)
    {
      var min = Math.Min(index1, index2);
      var max = Math.Max(index1, index2);
      var result = new QuickLinearRegression();

      if (useAllPointsForRegression)
      {
        for (int i = min; i <= max; ++i)
        {
          result.Add(x[i], y[i]);
        }
      }
      else
      {
        result.Add(x[min], y[min]);
        result.Add(x[max], y[max]);
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
    /// <returns></returns>
    public static QuickLinearRegression GetMiddleRegression(double[] x, double[] y, int index1, int index2, QuickLinearRegression leftRegression, QuickLinearRegression rightRegression, double lowerRegressionLevel, double upperRegressionLevel)
    {
      var min = Math.Min(index1, index2);
      var max = Math.Max(index1, index2);
      var result = new QuickLinearRegression();

      for (int i = min; i <= max; ++i)
      {
        var r = QuickLinearRegression.GetRelativeYBetweenRegressions(leftRegression, rightRegression, x[i], y[i]);
        if (RMath.IsInIntervalCC(r, lowerRegressionLevel, upperRegressionLevel))
        {
          result.Add(x[i], y[i]);
        }
      }
      return result;
    }

    protected void ReportError(DataTable destinationTable, string message)
    {
      Current.Console.WriteLine($"Error in StepEvaluationDataSource: {message}");
      destinationTable.Notes.WriteLine($"{DateTimeOffset.Now}: Error during evaluation the {this.GetType().Name}: {message}");
    }
  }
}
