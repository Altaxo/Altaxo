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
using Altaxo.Data;

namespace Altaxo.Science.Signals
{
  /// <summary>
  /// Data source that evaluates a step using a four-point method.
  /// </summary>
  public class FourPointStepEvaluationDataSource : TableDataSourceBaseImmutableOptions<FourPointStepEvaluationOptions, XAndYColumn>
  {
    /// <summary>Column name for the left evaluation line x-values.</summary>
    public const string ColumnNameLeftX = "LeftX";
    /// <summary>Column name for the left evaluation line y-values.</summary>
    public const string ColumnNameLeftY = "LeftY";
    /// <summary>Column name for the right evaluation line x-values.</summary>
    public const string ColumnNameRightX = "RightX";
    /// <summary>Column name for the right evaluation line y-values.</summary>
    public const string ColumnNameRightY = "RightY";
    /// <summary>Column name for the middle evaluation line x-values.</summary>
    public const string ColumnNameMiddleX = "MiddleX";
    /// <summary>Column name for the middle evaluation line y-values.</summary>
    public const string ColumnNameMiddleY = "MiddleY";
    /// <summary>Column name for the optional original-curve x-values.</summary>
    public const string ColumnNameCurveX = "CurveX";
    /// <summary>Column name for the optional original-curve y-values.</summary>
    public const string ColumnNameCurveY = "CurveY";
    /// <summary>Column name for parameter names.</summary>
    public const string ColumnNameParameterName = "ParameterName";
    /// <summary>Column name for parameter values.</summary>
    public const string ColumnNameParameterValue = "ParameterValue";
    /// <summary>Column group number for the left evaluation line.</summary>
    public const int ColumnGroupNumberLeft = 0;
    /// <summary>Column group number for the right evaluation line.</summary>
    public const int ColumnGroupNumberRight = 1;
    /// <summary>Column group number for the middle evaluation line.</summary>
    public const int ColumnGroupNumberMiddle = 2;
    /// <summary>Column group number for calculated parameters.</summary>
    public const int ColumnGroupParameter = 3;
    /// <summary>Column group number for optional curve values.</summary>
    public const int ColumnGroupCurveValues = 4;

    /// <summary>Parameter name for the x-coordinate of the step midpoint.</summary>
    public const string ParameterNameMiddleX = "StepMiddleX";
    /// <summary>Parameter name for the y-coordinate of the step midpoint.</summary>
    public const string ParameterNameMiddleY = "StepMiddleY";
    /// <summary>Parameter name for the slope of the middle step line.</summary>
    public const string ParameterNameStepMiddleSlope = "StepMiddleSlope";
    /// <summary>Parameter name for the step height.</summary>
    public const string ParameterNameStepHeight = "StepHeight";
    /// <summary>Parameter name for the significance of the step.</summary>
    public const string ParameterNameStepSignificance = "StepSignificance";
    /// <summary>Parameter name for the step width.</summary>
    public const string ParameterNameStepWidth = "StepWidth";
    /// <summary>Parameter name for the left x-position of the step.</summary>
    public const string ParameterNameStepLeftX = "StepLeftX";
    /// <summary>Parameter name for the left y-position of the step.</summary>
    public const string ParameterNameStepLeftY = "StepLeftY";
    /// <summary>Parameter name for the slope of the left regression line.</summary>
    public const string ParameterNameStepLeftSlope = "StepLeftSlope";
    /// <summary>Parameter name for the right x-position of the step.</summary>
    public const string ParameterNameStepRightX = "StepRightX";
    /// <summary>Parameter name for the right y-position of the step.</summary>
    public const string ParameterNameStepRightY = "StepRightY";
    /// <summary>Parameter name for the slope of the right regression line.</summary>
    public const string ParameterNameStepRightSlope = "StepRightSlope";

    /// <summary>
    /// Gets all parameter names written by this data source.
    /// </summary>
    public IEnumerable<string> AllParameterNames
    {
      get
      {
        yield return ParameterNameMiddleX;
        yield return ParameterNameMiddleY;
        yield return ParameterNameStepMiddleSlope;
        yield return ParameterNameStepHeight;
        yield return ParameterNameStepSignificance;
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
      /// <inheritdoc />
      public override object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        return new FourPointStepEvaluationDataSource(info, 0);
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FourPointStepEvaluationDataSource"/> class from serialized data.
    /// </summary>
    /// <param name="info">The deserialization info.</param>
    /// <param name="version">The serialized version.</param>
    protected FourPointStepEvaluationDataSource(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, int version)
    : base(info, version)
    {
    }

    #endregion Version 0

    #endregion Serialization

    /// <summary>
    /// Initializes a new instance of the <see cref="FourPointStepEvaluationDataSource"/> class.
    /// </summary>
    /// <param name="inputData">The input data designates the original source of data (used then for the processing).</param>
    /// <param name="dataSourceOptions">The evaluation options.</param>
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
    /// <param name="reporter">The progress reporter.</param>
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

      var stepEvaluation = FourPointStepEvaluation.CreateFromIndices(x, y, options.IndexLeftOuter, options.IndexLeftInner, options.IndexRightInner, options.IndexRightOuter, options.UseRegressionForLeftAndRightLine, options.MiddleRegressionLevels, false);

      if (stepEvaluation.HasErrors)
      {
        ReportError(destinationTable, $"The step evaluation has errors: {stepEvaluation.Errors}");
        return;
      }

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

      destinationTable[ColumnNameLeftX][0] = RMath.InterpolateLinear(options.IndexLeftOuter, x);
      destinationTable[ColumnNameLeftY][0] = stepEvaluation.LeftRegression.GetYOfX(RMath.InterpolateLinear(options.IndexLeftOuter, x));
      destinationTable[ColumnNameLeftX][1] = RMath.InterpolateLinear(options.IndexLeftInner, x);
      destinationTable[ColumnNameLeftY][1] = stepEvaluation.LeftRegression.GetYOfX(RMath.InterpolateLinear(options.IndexLeftInner, x));
      destinationTable[ColumnNameLeftX][2] = RMath.InterpolateLinear(options.IndexRightInner, x);
      destinationTable[ColumnNameLeftY][2] = stepEvaluation.LeftRegression.GetYOfX(RMath.InterpolateLinear(options.IndexRightInner, x));

      destinationTable[ColumnNameRightX][0] = RMath.InterpolateLinear(options.IndexLeftInner, x);
      destinationTable[ColumnNameRightY][0] = stepEvaluation.RightRegression.GetYOfX(RMath.InterpolateLinear(options.IndexLeftInner, x));
      destinationTable[ColumnNameRightX][1] = RMath.InterpolateLinear(options.IndexRightInner, x);
      destinationTable[ColumnNameRightY][1] = stepEvaluation.RightRegression.GetYOfX(RMath.InterpolateLinear(options.IndexRightInner, x));
      destinationTable[ColumnNameRightX][2] = RMath.InterpolateLinear(options.IndexRightOuter, x);
      destinationTable[ColumnNameRightY][2] = stepEvaluation.RightRegression.GetYOfX(RMath.InterpolateLinear(options.IndexRightOuter, x));

      var (xl, yl) = stepEvaluation.IntersectionPointLeftMiddle;
      var (xr, yr) = stepEvaluation.IntersectionPointRightMiddle;

      var xspan = xr - xl;
      var xlo = xl - options.MiddleLineOverlap * xspan;
      destinationTable[ColumnNameMiddleX][0] = xlo;
      destinationTable[ColumnNameMiddleY][0] = stepEvaluation.MiddleRegression.GetYOfX(xlo);

      destinationTable[ColumnNameMiddleX][1] = xl;
      destinationTable[ColumnNameMiddleY][1] = yl;

      destinationTable[ColumnNameMiddleX][2] = stepEvaluation.MiddlePointX;
      destinationTable[ColumnNameMiddleY][2] = stepEvaluation.MiddlePointY;

      destinationTable[ColumnNameMiddleX][3] = xr;
      destinationTable[ColumnNameMiddleY][3] = yr;

      var xro = xr + options.MiddleLineOverlap * xspan;
      destinationTable[ColumnNameMiddleX][4] = xro;
      destinationTable[ColumnNameMiddleY][4] = stepEvaluation.MiddleRegression.GetYOfX(xro);

      // now store all parameters
      var (xmiddle, ymiddle) = stepEvaluation.MiddlePoint;
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
      destinationTable[ColumnNameParameterValue][idxPara] = stepEvaluation.StepSlope;
      destinationTable.PropertyBagNotNull.SetValue(ParameterNameStepMiddleSlope, stepEvaluation.StepSlope);
      ++idxPara;
      destinationTable[ColumnNameParameterName][idxPara] = ParameterNameStepHeight;
      destinationTable[ColumnNameParameterValue][idxPara] = stepEvaluation.StepHeight;
      destinationTable.PropertyBagNotNull.SetValue(ParameterNameStepHeight, stepEvaluation.StepHeight);
      ++idxPara;
      destinationTable[ColumnNameParameterName][idxPara] = ParameterNameStepSignificance;
      destinationTable[ColumnNameParameterValue][idxPara] = stepEvaluation.StepSignificance;
      destinationTable.PropertyBagNotNull.SetValue(ParameterNameStepSignificance, stepEvaluation.StepSignificance);
      ++idxPara;
      destinationTable[ColumnNameParameterName][idxPara] = ParameterNameStepWidth;
      destinationTable[ColumnNameParameterValue][idxPara] = stepEvaluation.StepWidth;
      destinationTable.PropertyBagNotNull.SetValue(ParameterNameStepWidth, stepEvaluation.StepWidth);
      ++idxPara;
      destinationTable[ColumnNameParameterName][idxPara] = ParameterNameStepLeftX;
      destinationTable[ColumnNameParameterValue][idxPara] = xl;
      destinationTable.PropertyBagNotNull.SetValue(ParameterNameStepLeftX, xl);
      ++idxPara;
      destinationTable[ColumnNameParameterName][idxPara] = ParameterNameStepLeftY;
      destinationTable[ColumnNameParameterValue][idxPara] = yl;
      destinationTable.PropertyBagNotNull.SetValue(ParameterNameStepLeftY, yl);
      ++idxPara;
      destinationTable[ColumnNameParameterName][idxPara] = ParameterNameStepLeftSlope;
      destinationTable[ColumnNameParameterValue][idxPara] = stepEvaluation.LeftRegression.GetA1();
      ++idxPara;
      destinationTable[ColumnNameParameterName][idxPara] = ParameterNameStepRightX;
      destinationTable[ColumnNameParameterValue][idxPara] = xr;
      destinationTable.PropertyBagNotNull.SetValue(ParameterNameStepRightX, xr);
      ++idxPara;
      destinationTable[ColumnNameParameterName][idxPara] = ParameterNameStepRightY;
      destinationTable[ColumnNameParameterValue][idxPara] = yr;
      destinationTable.PropertyBagNotNull.SetValue(ParameterNameStepRightY, yr);
      ++idxPara;
      destinationTable[ColumnNameParameterName][idxPara] = ParameterNameStepRightSlope;
      destinationTable[ColumnNameParameterValue][idxPara] = stepEvaluation.RightRegression.GetA1();
      destinationTable.PropertyBagNotNull.SetValue(ParameterNameStepRightSlope, stepEvaluation.RightRegression.GetA1());

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
    /// Reports an evaluation error.
    /// </summary>
    /// <param name="destinationTable">The destination table.</param>
    /// <param name="message">The error message.</param>
    protected void ReportError(DataTable destinationTable, string message)
    {
      Current.Console.WriteLine($"Error in StepEvaluationDataSource: {message}");
      destinationTable.Notes.WriteLine($"{DateTimeOffset.Now}: Error during evaluation the {this.GetType().Name}: {message}");
    }
  }
}
