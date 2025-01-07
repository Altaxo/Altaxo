#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2025 Dr. Dirk Lellinger
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
  public class FourPointPeakEvaluationDataSource : TableDataSourceBaseImmutableOptions<FourPointPeakEvaluationOptions, XAndYColumn>
  {
    public const string ColumnNameLineX = "LineX";
    public const string ColumnNameLineY = "LineY";
    public const string ColumnNameInnerLineX = "InnerLineX";
    public const string ColumnNameInnerLineY = "InnerLineY";
    public const string ColumnNameCurveX = "CurveX";
    public const string ColumnNameCurveY = "CurveY";
    public const string ColumnNameParameterName = "ParameterName";
    public const string ColumnNameParameterValue = "ParameterValue";
    public const int ColumnGroupNumberLine = 0;
    public const int ColumnGroupNumberInnerLine = 1;
    public const int ColumnGroupParameter = 2;
    public const int ColumnGroupCurveValues = 3;

    public const string ParameterNameAreaLeftX = "AreaLeftBorderX";
    public const string ParameterNameAreaRightX = "AreaRightBorderX";
    public const string ParameterNameAreaValue = "AreaValue";
    public const string ParameterNameHeight = "PeakHeight";
    public const string ParameterNamePeakX = "PeakPositionX";
    public const string ParameterNameFWHM = "PeakFWHM";

    public IEnumerable<string> AllParameterNames
    {
      get
      {
        yield return ParameterNameAreaLeftX;
        yield return ParameterNameAreaRightX;
        yield return ParameterNameAreaValue;
        yield return ParameterNameHeight;
        yield return ParameterNamePeakX;
        yield return ParameterNameFWHM;
      }
    }

    #region Serialization

    #region Version 0

    /// <summary>
    /// 2025-01-06 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(FourPointPeakEvaluationDataSource), 0)]
    private class XmlSerializationSurrogate0 : XmlSerializationSurrogateBase
    {
      public override object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return new FourPointPeakEvaluationDataSource(info, 0);
      }
    }

    protected FourPointPeakEvaluationDataSource(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, int version)
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
    public FourPointPeakEvaluationDataSource(XAndYColumn inputData, FourPointPeakEvaluationOptions dataSourceOptions, IDataSourceImportOptions importOptions)
        : base(inputData, dataSourceOptions, importOptions)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FourPointPeakEvaluationDataSource"/> class.
    /// </summary>
    /// <param name="from">Another instance to copy from.</param>
    public FourPointPeakEvaluationDataSource(FourPointPeakEvaluationDataSource from)
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

      var baselineRegression = GetBaselineRegression(x, y, options.IndexLeftOuter, options.IndexRightOuter);

      if (!baselineRegression.IsValid)
      {
        ReportError(destinationTable, $"The baseline regression line is not valid.");
        return;
      }

      var innerLeftX = RMath.InterpolateLinear(options.IndexLeftInner, x);
      var innerRightX = RMath.InterpolateLinear(options.IndexRightInner, x);

      // Perform the area calculation
      var area = CalculateArea(x, y, baselineRegression, options.IndexLeftInner, options.IndexRightInner);
      var (height, peakX, fwhm) = CalculatePeakParameters(x, y, baselineRegression, options.IndexLeftInner, options.IndexRightInner);


      // now fill the data table


      destinationTable.DataColumns.EnsureExistence(ColumnNameLineX, typeof(DoubleColumn), ColumnKind.X, ColumnGroupNumberLine);
      destinationTable.DataColumns.EnsureExistence(ColumnNameLineY, typeof(DoubleColumn), ColumnKind.V, ColumnGroupNumberLine);
      destinationTable.DataColumns.EnsureExistence(ColumnNameInnerLineX, typeof(DoubleColumn), ColumnKind.X, ColumnGroupNumberInnerLine);
      destinationTable.DataColumns.EnsureExistence(ColumnNameInnerLineY, typeof(DoubleColumn), ColumnKind.V, ColumnGroupNumberInnerLine);
      destinationTable.DataColumns.EnsureExistence(ColumnNameParameterName, typeof(TextColumn), ColumnKind.X, ColumnGroupParameter);
      destinationTable.DataColumns.EnsureExistence(ColumnNameParameterValue, typeof(DoubleColumn), ColumnKind.V, ColumnGroupParameter);
      if (ProcessOptions.IncludeOriginalPointsInOutput)
      {
        destinationTable.DataColumns.EnsureExistence(ColumnNameCurveX, typeof(DoubleColumn), ColumnKind.X, ColumnGroupCurveValues);
        destinationTable.DataColumns.EnsureExistence(ColumnNameCurveY, typeof(DoubleColumn), ColumnKind.V, ColumnGroupCurveValues);
      }

      // now fill the table
      var xLeftOuter = RMath.InterpolateLinear(options.IndexLeftOuter, x);
      var xRightOuter = RMath.InterpolateLinear(options.IndexRightOuter, x);
      destinationTable[ColumnNameLineX][0] = xLeftOuter;
      destinationTable[ColumnNameLineY][0] = baselineRegression.GetYOfX(xLeftOuter);
      destinationTable[ColumnNameLineX][1] = xRightOuter;
      destinationTable[ColumnNameLineY][1] = baselineRegression.GetYOfX(xRightOuter);

      var xLeftInner = RMath.InterpolateLinear(options.IndexLeftInner, x);
      var xRightInner = RMath.InterpolateLinear(options.IndexRightInner, x);
      destinationTable[ColumnNameInnerLineX][0] = xLeftInner;
      destinationTable[ColumnNameInnerLineY][0] = baselineRegression.GetYOfX(xLeftInner);
      destinationTable[ColumnNameInnerLineX][1] = xRightInner;
      destinationTable[ColumnNameInnerLineY][1] = baselineRegression.GetYOfX(xRightInner);

      // output the curve values
      if (ProcessOptions.IncludeOriginalPointsInOutput)
      {
        for (int i = 0; i < rowCount; ++i)
        {
          destinationTable[ColumnNameCurveX][i] = x[i];
          destinationTable[ColumnNameCurveY][i] = y[i];
        }
      }

      // now store all parameters
      int idxPara = 0;
      destinationTable[ColumnNameParameterName][idxPara] = ParameterNameAreaLeftX;
      destinationTable[ColumnNameParameterValue][idxPara] = xLeftInner;
      destinationTable.PropertyBagNotNull.SetValue(ParameterNameAreaLeftX, xLeftInner);
      ++idxPara;
      destinationTable[ColumnNameParameterName][idxPara] = ParameterNameAreaRightX;
      destinationTable[ColumnNameParameterValue][idxPara] = xRightInner;
      destinationTable.PropertyBagNotNull.SetValue(ParameterNameAreaRightX, xRightInner);
      ++idxPara;
      destinationTable[ColumnNameParameterName][idxPara] = ParameterNameAreaValue;
      destinationTable[ColumnNameParameterValue][idxPara] = area;
      destinationTable.PropertyBagNotNull.SetValue(ParameterNameAreaValue, area);
      ++idxPara;
      destinationTable[ColumnNameParameterName][idxPara] = ParameterNameHeight;
      destinationTable[ColumnNameParameterValue][idxPara] = height;
      destinationTable.PropertyBagNotNull.SetValue(ParameterNameHeight, height);
      ++idxPara;
      destinationTable[ColumnNameParameterName][idxPara] = ParameterNamePeakX;
      destinationTable[ColumnNameParameterValue][idxPara] = peakX;
      destinationTable.PropertyBagNotNull.SetValue(ParameterNamePeakX, peakX);
      ++idxPara;
      destinationTable[ColumnNameParameterName][idxPara] = ParameterNameFWHM;
      destinationTable[ColumnNameParameterValue][idxPara] = fwhm;
      destinationTable.PropertyBagNotNull.SetValue(ParameterNameFWHM, fwhm);
      ++idxPara;
    }

    /// <summary>
    /// Gets the regression for the left or the right line.
    /// </summary>
    /// <param name="x">The x values.</param>
    /// <param name="y">The y values.</param>
    /// <param name="index1">The start index.</param>
    /// <param name="index2">The end index (inclusive).</param>
    /// to create the linear regression; otherwise, only point[index1] and point[index2] are used to calculate the line.</param>
    /// <returns>The regression that forms a line under the peak.</returns>
    public static QuickLinearRegression GetBaselineRegression(double[] x, double[] y, double index1, double index2)
    {
      var min = Math.Min(index1, index2);
      var max = Math.Max(index1, index2);
      var result = new QuickLinearRegression();

      {
        result.Add(RMath.InterpolateLinear(min, x), RMath.InterpolateLinear(min, y));
        result.Add(RMath.InterpolateLinear(max, x), RMath.InterpolateLinear(max, y));
      }

      return result;
    }

    private static int IntFloorOrCeiling(double x, int direction)
    {
      return direction switch
      {
        -1 => (int)Math.Floor(x),
        1 => (int)Math.Ceiling(x),
        _ => throw new ArgumentOutOfRangeException(nameof(direction), $"The value {direction} is not allowed. It must be -1 or 1."),
      };
    }

    /// <summary>
    /// Gets the area.
    /// </summary>
    /// <param name="x">The x values.</param>
    /// <param name="y">The y values.</param>
    /// <param name="baselineRegression">The regression that forms the baseline of the peak.</param>
    /// <param name="index1">The start index (start of the integration).</param>
    /// <param name="index2">The end index (end of the integration).</param>
    /// <returns>The area under the y-values to the baseline.</returns>
    public static double CalculateArea(double[] x, double[] y, QuickLinearRegression baselineRegression, double index1, double index2)
    {
      var dir = Math.Sign(index2 - index1);
      double area;
      if (dir == 0) // both indices are the same
      {
        area = 0;
      }
      else
      {
        var prev_x = RMath.InterpolateLinear(index1, x);
        var prev_y = RMath.InterpolateLinear(index1, y);
        var end_x = RMath.InterpolateLinear(index2, x);
        var end_y = RMath.InterpolateLinear(index2, y);
        if (Math.Floor(index1) == Math.Floor(index2)) // both indices are in the same interval
        {
          area = 0.5 * (end_x - prev_x) * ((end_y - baselineRegression.GetYOfX(end_x)) + (prev_y - baselineRegression.GetYOfX(prev_y)));
        }
        else
        {
          var iStart = IntFloorOrCeiling(index1, dir);
          var iEnd = IntFloorOrCeiling(index2, -dir);
          area = 0;

          if (iStart != index1)
          {
            area += 0.5 * (x[iStart] - prev_x) * ((y[iStart] - baselineRegression.GetYOfX(x[iStart])) + (prev_y - baselineRegression.GetYOfX(prev_x)));
          }
          if (iEnd != index2)
          {
            area += 0.5 * (end_x - x[iEnd]) * ((end_y - baselineRegression.GetYOfX(end_x)) + (y[iEnd] - baselineRegression.GetYOfX(x[iEnd])));
          }

          for (int i = iStart; i != iEnd; i += dir)
          {
            var x1 = x[i];
            var y1 = y[i];
            var x2 = x[i + dir];
            var y2 = y[i + dir];
            area += 0.5 * (x2 - x1) * ((y2 - baselineRegression.GetYOfX(x2)) + (y1 - baselineRegression.GetYOfX(x1)));
          }

        }
      }
      return area;
    }

    /// <summary>
    /// Gets the area.
    /// </summary>
    /// <param name="x">The x values.</param>
    /// <param name="y">The y values.</param>
    /// <param name="baselineRegression">The regression that forms the baseline of the peak.</param>
    /// <param name="index1">The start index (start of the integration).</param>
    /// <param name="index2">The end index (end of the integration).</param>
    /// <returns>The area under the y-values to the baseline.</returns>
    public static (double Height, double PeakX, double FWHM) CalculatePeakParameters(double[] x, double[] y, QuickLinearRegression baselineRegression, double index1, double index2)
    {
      var dir = Math.Sign(index2 - index1);
      if (dir == 0) // both indices are the same
      {
        var xpeak = RMath.InterpolateLinear(index1, x);
        var height = RMath.InterpolateLinear(index1, y) - baselineRegression.GetYOfX(xpeak);
        var fwhm = 0;
        return (height, xpeak, fwhm);
      }
      else
      {
        // Note that we here only linearly interpolate between the points.
        // Thus, for every segment, we have to consider only the start end end of the segment.
        var iStart = IntFloorOrCeiling(index1, dir);
        var iEnd = IntFloorOrCeiling(index2, -dir);
        var height = double.NegativeInfinity;
        var peakX = double.NaN;
        var peakIdx = double.NaN;
        var fwhm = double.NaN;
        double dy;

        if (iStart != index1)
        {
          dy = Math.Abs(y[iStart] - baselineRegression.GetYOfX(x[iStart]));
          if (height < dy)
          {
            height = dy;
            peakIdx = iStart;
          }
          dy = Math.Abs(RMath.InterpolateLinear(index1, y) - baselineRegression.GetYOfX(RMath.InterpolateLinear(index1, x)));
          if (height < dy)
          {
            height = dy;
            peakIdx = index1;
          }
        }
        if (iEnd != index2)
        {
          dy = Math.Abs(y[iEnd] - baselineRegression.GetYOfX(x[iEnd]));
          if (height < dy)
          {
            height = dy;
            peakIdx = iEnd;
          }
          dy = Math.Abs(RMath.InterpolateLinear(index2, y) - baselineRegression.GetYOfX(RMath.InterpolateLinear(index2, x)));
          if (height < dy)
          {
            height = dy;
            peakIdx = index2;
          }
        }

        for (int i = iStart; i != iEnd; i += dir)
        {
          var x1 = x[i];
          var y1 = y[i];
          var x2 = x[i + dir];
          var y2 = y[i + dir];
          dy = Math.Abs(y1 - baselineRegression.GetYOfX(x1));
          if (height < dy)
          {
            height = dy;
            peakIdx = i;
          }
          dy = Math.Abs(y2 - baselineRegression.GetYOfX(x2));
          if (height < dy)
          {
            height = dy;
            peakIdx = i + dir;
          }
        }

        // now that we have the x at maximum, we can calculate the FWHM
        if (!double.IsNaN(peakIdx))
        {
          height = Math.Abs(RMath.InterpolateLinear(peakIdx, y) - baselineRegression.GetYOfX(RMath.InterpolateLinear(peakIdx, x)));
          peakX = RMath.InterpolateLinear(peakIdx, x);
        }
        return (height, peakX, fwhm);
      }
    }

    protected void ReportError(DataTable destinationTable, string message)
    {
      Current.Console.WriteLine($"Error in StepEvaluationDataSource: {message}");
      destinationTable.Notes.WriteLine($"{DateTimeOffset.Now}: Error during evaluation the {this.GetType().Name}: {message}");
    }
  }
}
