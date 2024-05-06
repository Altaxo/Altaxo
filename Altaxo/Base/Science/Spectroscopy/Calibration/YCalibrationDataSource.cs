#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2023 Dr. Dirk Lellinger
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
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Altaxo.Calc;
using Altaxo.Data;
using Altaxo.Main;

namespace Altaxo.Science.Spectroscopy.Calibration
{
  public class YCalibrationDataSource : TableDataSourceBase, IAltaxoTableDataSource, IYCalibrationDataSource, IHasDocumentReferences
  {
    /// <summary>Name of the column that represents the x-values of the signal after preprocessing.</summary>
    public const string ColumnName_Group0_SpectrumX = "X_Spectrum";

    /// <summary>Name of the column that represents the y-values of the signal after preprocessing.</summary>
    public const string ColumnName_Group0_SpectrumY = "Y_Spectrum";

    /// <summary>Name of the column that represents the y-values of the calibration standard,
    /// evaluated usually from the formula that comes with this standard.</summary>
    public const string ColumnName_Group0_CalibrationStandardY = "Y_CalibrationStandard";

    /// <summary>Name of the column of the scaling denominator, but not smoothed in this case.</summary>
    public const string ColumnName_Group0_ScalingDenominatorNotSmoothed = "ScalingDenominator (not smoothed)";

    /// <summary>Name of the column that represents the values that are used as denominator to convert
    /// an uncalibrated intensity value to a calibrated one.</summary>
    public const string ColumnName_Group0_ScalingDenominator = "ScalingDenominator";


    private IDataSourceImportOptions _importOptions;
    private YCalibrationOptionsDocNode _processOptions;
    private DataTableXYColumnProxy? _processData;

    public Action<IAltaxoTableDataSource>? _dataSourceChanged;

    #region Serialization

    #region Version 0


    /// <summary>
    /// 2022-06-08 initial version.
    /// 2022-08-05 change processOptions from PeakFindingAndFittingOptions to PeakFindingAndFittingOptionsDocNode, change namespace from Altaxo.Data to Altaxo.Science.Spectroscopy, change class name from PeakFindingAndFittingDataSource to PeakSearchingAndFittingDataSource
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(YCalibrationDataSource), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (YCalibrationDataSource)obj;

        info.AddValue("ProcessData", s._processData);
        info.AddValue("ProcessOptions", s._processOptions);
        info.AddValue("ImportOptions", s._importOptions);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        if (o is YCalibrationDataSource s)
          s.DeserializeSurrogate0(info);
        else
          s = new YCalibrationDataSource(info, 0);
        return s;
      }
    }

    [MemberNotNull(nameof(_importOptions), nameof(_processOptions), nameof(_processData))]
    private void DeserializeSurrogate0(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    {
      ChildSetMember(ref _processData, (DataTableXYColumnProxy)info.GetValue("ProcessData", this));
      ChildSetMember(ref _processOptions, info.GetValue<YCalibrationOptionsDocNode>("ProcessOptions", this));
      ChildSetMember(ref _importOptions, (IDataSourceImportOptions)info.GetValue("ImportOptions", this));
    }

    #endregion Version 0

    protected YCalibrationDataSource(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, int version)
    {
      switch (version)
      {
        case 0:
          DeserializeSurrogate0(info);
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof(version));
      }
    }

    #endregion Serialization


    /// <summary>
    /// Initializes a new instance of the <see cref="YCalibrationDataSource"/> class.
    /// </summary>
    /// <param name="importOptions">The data source import options.</param>
    /// <exception cref="ArgumentNullException">
    /// inputData
    /// or
    /// transformationOptions
    /// or
    /// importOptions
    /// </exception>
    public YCalibrationDataSource(IDataSourceImportOptions importOptions)
    {
      if (importOptions is null)
        throw new ArgumentNullException(nameof(importOptions));

      ChildSetMember(ref _importOptions, importOptions);
    }

    public YCalibrationDataSource(DataTableXYColumnProxy? inputData, YCalibrationOptions dataSourceOptions, IDataSourceImportOptions importOptions)
    {
      if (inputData is null)
        throw new ArgumentNullException(nameof(inputData));
      if (dataSourceOptions is null)
        throw new ArgumentNullException(nameof(dataSourceOptions));
      if (importOptions is null)
        throw new ArgumentNullException(nameof(importOptions));

      ChildSetMember(ref _processOptions, new YCalibrationOptionsDocNode(dataSourceOptions));
      ChildSetMember(ref _processData, inputData);
      ChildSetMember(ref _importOptions, importOptions);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="YCalibrationDataSource"/> class.
    /// </summary>
    /// <param name="from">Another instance to copy from.</param>
    public YCalibrationDataSource(YCalibrationDataSource from)
    {
      CopyFrom(from);
    }

    [MemberNotNull(nameof(_importOptions), nameof(_processOptions), nameof(_processData))]
    public void CopyFrom(YCalibrationDataSource from)
    {
      if (ReferenceEquals(this, from))
#pragma warning disable CS8774 // Member must have a non-null SpectralPreprocessingDataSource when exiting.
        return;
#pragma warning restore CS8774 // Member must have a non-null value when exiting.

      using (var token = SuspendGetToken())
      {
        DataTableXYColumnProxy? processData = null;
        IDataSourceImportOptions? importOptions = null;

        CopyHelper.Copy(ref importOptions, from._importOptions);
        CopyHelper.Copy(ref processData, from._processData);

        ProcessOptions = from.ProcessOptions;
        ImportOptions = importOptions;
        ProcessData = processData;
      }
    }

    /// <summary>
    /// Copies from another instance.
    /// </summary>
    /// <param name="obj">The object to copy from.</param>
    /// <returns><c>True</c> if anything could be copied from the object, otherwise <c>false</c>.</returns>
    public bool CopyFrom(object obj)
    {
      if (ReferenceEquals(this, obj))
        return true;

      if (obj is YCalibrationDataSource from)
      {
        CopyFrom(from);
        return true;
      }
      return false;
    }

    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>
    /// A new object that is a copy of this instance.
    /// </returns>
    public object Clone()
    {
      return new YCalibrationDataSource(this);
    }


    #region IAltaxoTableDataSource

    /// <summary>
    /// Fills (or refills) the data table with the processed data. The data source is represented by this instance, the destination table is provided in the argument <paramref name="destinationTable" />.
    /// </summary>
    /// <param name="destinationTable">The destination table.</param>
    /// <param name="reporter"></param>
    public override void FillData_Unchecked(DataTable destinationTable, IProgressReporter reporter)
    {
      FillData(destinationTable, CancellationToken.None);
    }

    /// <summary>
    /// Fills (or refills) the data table with the processed data. The data source is represented by this instance, the destination table is provided in the argument <paramref name="destinationTable" />.
    /// </summary>
    /// <param name="destinationTable">The destination table.</param>
    /// <param name="cancellationToken">CancellationToken used to cancel the task.</param>
    public void FillData(DataTable destinationTable, CancellationToken cancellationToken)
    {
      destinationTable.DataColumns.RemoveColumnsAll();
      destinationTable.PropertyColumns.RemoveColumnsAll();

      var colX = destinationTable.DataColumns.EnsureExistence(ColumnName_Group0_SpectrumX, typeof(DoubleColumn), ColumnKind.X, 0);
      var colY = destinationTable.DataColumns.EnsureExistence(ColumnName_Group0_SpectrumY, typeof(DoubleColumn), ColumnKind.V, 0);
      var colCalStandardY = destinationTable.DataColumns.EnsureExistence(ColumnName_Group0_CalibrationStandardY, typeof(DoubleColumn), ColumnKind.V, 0);

      var yCalibrationOptions = _processOptions.GetYCalibrationOptions();

      DoubleColumn colScalingDenominatorNotSmoothed, colScalingDenominator;
      if (yCalibrationOptions.InterpolationMethod is not null)
      {
        colScalingDenominatorNotSmoothed = (DoubleColumn)destinationTable.DataColumns.EnsureExistence(
            ColumnName_Group0_ScalingDenominatorNotSmoothed, typeof(DoubleColumn), ColumnKind.V, 0);
        colScalingDenominator = (DoubleColumn)destinationTable.DataColumns.EnsureExistence(
            ColumnName_Group0_ScalingDenominator, typeof(DoubleColumn), ColumnKind.V, 0);
      }
      else
      {
        colScalingDenominatorNotSmoothed = (DoubleColumn)destinationTable.DataColumns.EnsureExistence(
            ColumnName_Group0_ScalingDenominator, typeof(DoubleColumn), ColumnKind.V, 0);
        colScalingDenominator = colScalingDenominatorNotSmoothed;
      }

      var srcXCol = _processData.XColumn;
      var srcYCol = _processData.YColumn;

      if (srcXCol is null)
        throw new InvalidOperationException($"Unable to find the x-column of the intensity calibration data");
      if (srcYCol is null)
        throw new InvalidOperationException($"Unable to find the y-column of the intensity calibration data");

      var len = Math.Min(srcYCol.Count ?? 0, srcXCol.Count ?? 0);
      var xArr = new double[len];
      var yArr = new double[len];
      for (var i = 0; i < len; i++)
      {
        xArr[i] = srcXCol[i];
        yArr[i] = srcYCol[i];
      }

      var spectralPreprocessingOptions = yCalibrationOptions.Preprocessing;
      int[]? regions = null;
      (xArr, yArr, regions) = spectralPreprocessingOptions.Execute(xArr, yArr, regions);

      var function = yCalibrationOptions.CurveShape;
      var xList = new List<double>();
      var yList = new List<double>();
      var yStandardList = new List<double>();
      var yScalingDenominatorNotSmoothedList = new List<double>();
      double maxScalingDenominator = double.NegativeInfinity;
      for (int i = 0; i < xArr.Length; i++)
      {
        var x = xArr[i];
        if (RMath.IsInIntervalCC(x, yCalibrationOptions.MinimalValidXValueOfCurve, yCalibrationOptions.MaximalValidXValueOfCurve))
        {

          var yStandard = function.Evaluate(xArr[i]);
          var scalingDenominator = yArr[i] / yStandard;
          xList.Add(xArr[i]);
          yList.Add(yArr[i]);
          yStandardList.Add(yStandard);
          yScalingDenominatorNotSmoothedList.Add(scalingDenominator);
          maxScalingDenominator = Math.Max(maxScalingDenominator, scalingDenominator);
        }
      }

      // if choosen, then smooth the result
      if (yCalibrationOptions.InterpolationMethod is { } smoothingOption)
      {
        var yScalingDenominatorSmoothedList = new List<double>();
        var interpolation = smoothingOption.Interpolate(xList.ToArray(), yScalingDenominatorNotSmoothedList.ToArray());
        maxScalingDenominator = double.NegativeInfinity;
        for (int i = 0; i < xList.Count; i++)
        {
          var scalingDenominator = interpolation.GetYOfX(xList[i]);
          yScalingDenominatorSmoothedList.Add(scalingDenominator);
          maxScalingDenominator = Math.Max(maxScalingDenominator, scalingDenominator);
        }

        for (int i = 0, j = 0; i < xList.Count; i++)
        {
          var gainRatio = maxScalingDenominator / yScalingDenominatorSmoothedList[i];
          if (RMath.IsInIntervalOC(gainRatio, 0, yCalibrationOptions.MaximalGainRatio))
          {
            colX[j] = xList[i];
            colY[j] = yList[i];
            colCalStandardY[j] = yStandardList[i];
            colScalingDenominatorNotSmoothed[j] = yScalingDenominatorNotSmoothedList[i] / maxScalingDenominator;
            colScalingDenominator[j] = yScalingDenominatorSmoothedList[i] / maxScalingDenominator;
            j++;
          }
        }
      }
      else
      {
        for (int i = 0, j = 0; i < xList.Count; i++)
        {
          var gainRatio = maxScalingDenominator / yScalingDenominatorNotSmoothedList[i];
          if (RMath.IsInIntervalOC(gainRatio, 0, yCalibrationOptions.MaximalGainRatio))
          {
            colX[j] = xList[i];
            colY[j] = yList[i];
            colCalStandardY[j] = yStandardList[i];
            colScalingDenominator[j] = yScalingDenominatorNotSmoothedList[i] / maxScalingDenominator;
            j++;
          }
        }
      }
    }

    public bool IsContainingValidYAxisCalibration(DataTable table)
    {
      var xColumn = table.DataColumns.TryGetColumn(ColumnName_Group0_SpectrumX);
      var sColumn = table.DataColumns.TryGetColumn(ColumnName_Group0_ScalingDenominator);
      return xColumn is not null && sColumn is not null && Math.Min(xColumn.Count, sColumn.Count) >= 2;
    }

    public (double x, double yScalingFactor)[] GetYAxisCalibration(DataTable table)
    {
      var xColumn = table.DataColumns.TryGetColumn(ColumnName_Group0_SpectrumX);
      var sColumn = table.DataColumns.TryGetColumn(ColumnName_Group0_ScalingDenominator);

      if (!(xColumn is not null && sColumn is not null && Math.Min(xColumn.Count, sColumn.Count) >= 2))
        throw new InvalidOperationException($"This data source does not contain a valid intensity calibration. Please check this with {nameof(IsContainingValidYAxisCalibration)} beforehand!");

      var len = Math.Min(xColumn.Count, sColumn.Count);
      var result = new (double x, double yScalingFactor)[len];

      for (var i = 0; i < len; ++i)
        result[i] = (xColumn[i], sColumn[i]);

      return result;
    }

    #endregion

    /// <summary>
    /// Gets or sets the input data.
    /// </summary>
    /// <value>
    /// The input data.
    /// </value>
    public DataTableXYColumnProxy ProcessData
    {
      get
      {
        return _processData;
      }
      [MemberNotNull(nameof(_processData))]
      set
      {
        if (ChildSetMember(ref _processData, value ?? throw new ArgumentNullException(nameof(value))))
        {
          EhChildChanged(_processData, EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// Gets or sets the data source import options.
    /// </summary>
    /// <value>
    /// The import options.
    /// </value>
    /// <exception cref="ArgumentNullException">ImportOptions</exception>
    public override IDataSourceImportOptions ImportOptions
    {
      get
      {
        return _importOptions;
      }
      [MemberNotNull(nameof(_importOptions))]
      set
      {
        if (ChildSetMember(ref _importOptions, value ?? throw new ArgumentNullException(nameof(value))))
        {
          EhChildChanged(_importOptions, EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// Gets or sets the options for this data source.
    /// </summary>
    /// <value>
    /// The options for this data source.
    /// </value>
    public YCalibrationOptions ProcessOptions
    {
      get
      {
        return _processOptions.GetYCalibrationOptions();
      }
      [MemberNotNull(nameof(_processOptions))]
      set
      {
        if (value is null)
          throw new ArgumentNullException(nameof(ProcessOptions));

        if (_processOptions is null || !object.Equals(_processOptions.GetYCalibrationOptions(), value))
        {
          ChildSetMember(ref _processOptions, new YCalibrationOptionsDocNode(value));
          EhChildChanged(_processOptions, EventArgs.Empty);
        }
      }
    }

    object IAltaxoTableDataSource.ProcessOptionsObject
    {
      get => ProcessOptions;
      set => ProcessOptions = (YCalibrationOptions)value;
    }

    object IAltaxoTableDataSource.ProcessDataObject
    {
      get => _processData;
      set => ProcessData = (DataTableXYColumnProxy)value;
    }

    #region Change event handling

    protected override bool HandleHighPriorityChildChangeCases(object? sender, ref EventArgs e)
    {
      if (sender is not null && object.ReferenceEquals(_processData, sender)) // incoming call from data proxy
      {
        if (_importOptions.ImportTriggerSource == ImportTriggerSource.DataSourceChanged)
        {
          e = TableDataSourceChangedEventArgs.Empty;
        }
        else
        {
          return true; // if option is not DataSourceChanged, absorb this event
        }
      }

      return base.HandleHighPriorityChildChangeCases(sender, ref e);
    }

    #endregion Change event handling

    #region Document Node functions

    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (_processOptions is not null)
        yield return new Main.DocumentNodeAndName(_processOptions, "ProcessOptions");

      if (_processData is not null)
        yield return new Main.DocumentNodeAndName(_processData, "ProcessData");

      if (_importOptions is not null)
        yield return new Main.DocumentNodeAndName(_importOptions, "ImportOptions");
    }

    #endregion Document Node functions

    /// <summary>
    /// Called after deserization of a data source instance, when it is already associated with a data table.
    /// </summary>
    public void OnAfterDeserialization()
    {
    }

    /// <summary>
    /// Visits all document references.
    /// </summary>
    /// <param name="ReportProxies">The report proxies.</param>
    public void VisitDocumentReferences(Main.DocNodeProxyReporter ReportProxies)
    {
      _processData?.VisitDocumentReferences(ReportProxies);
      _processOptions?.VisitDocumentReferences(ReportProxies);
    }
  }
}
