using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using Altaxo.Data;

namespace Altaxo.Science.Spectroscopy.Calibration
{


  public class IntensityCalibrationDataSource : TableDataSourceBase, IAltaxoTableDataSource, IYCalibrationDataSource
  {
    public const string ColumnName_Group0_SpectrumX = "X";
    public const string ColumnName_Group0_ScalingFactor = "IntensityScaling";
    public const string ProxyColumnGroupName_SignalSpectrum = "SignalSpectrum";
    public const string ProxyColumnGroupName_DarkSpectrum = "DarkSpectrum";


    private IDataSourceImportOptions _importOptions;
    private IntensityCalibrationOptions _processOptions;
    private DataTableMultipleColumnProxy? _processData;

    public Action<IAltaxoTableDataSource>? _dataSourceChanged;

    #region Serialization

    #region Version 0


    /// <summary>
    /// 2022-06-08 initial version.
    /// 2022-08-05 change processOptions from PeakFindingAndFittingOptions to PeakFindingAndFittingOptionsDocNode, change namespace from Altaxo.Data to Altaxo.Science.Spectroscopy, change class name from PeakFindingAndFittingDataSource to PeakSearchingAndFittingDataSource
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(IntensityCalibrationDataSource), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (IntensityCalibrationDataSource)obj;

        info.AddValue("ProcessData", s._processData);
        info.AddValue("ProcessOptions", s._processOptions);
        info.AddValue("ImportOptions", s._importOptions);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        if (o is IntensityCalibrationDataSource s)
          s.DeserializeSurrogate0(info);
        else
          s = new IntensityCalibrationDataSource(info, 0);
        return s;
      }
    }

    [MemberNotNull(nameof(_importOptions), nameof(_processOptions), nameof(_processData))]
    private void DeserializeSurrogate0(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    {
      ChildSetMember(ref _processData, (DataTableMultipleColumnProxy)info.GetValue("ProcessData", this));
      _processOptions = info.GetValue<IntensityCalibrationOptions>("ProcessOptions", this);
      ChildSetMember(ref _importOptions, (IDataSourceImportOptions)info.GetValue("ImportOptions", this));
    }

    #endregion Version 0

    protected IntensityCalibrationDataSource(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, int version)
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
    /// Initializes a new instance of the <see cref="IntensityCalibrationDataSource"/> class.
    /// </summary>
    /// <param name="importOptions">The data source import options.</param>
    /// <exception cref="ArgumentNullException">
    /// inputData
    /// or
    /// transformationOptions
    /// or
    /// importOptions
    /// </exception>
    public IntensityCalibrationDataSource(IDataSourceImportOptions importOptions)
    {
      if (importOptions is null)
        throw new ArgumentNullException(nameof(importOptions));

      ChildSetMember(ref _importOptions, importOptions);
    }

    public IntensityCalibrationDataSource(DataTableMultipleColumnProxy? dataSource, IntensityCalibrationOptions options, IDataSourceImportOptions importOptions)
    {
      if (importOptions is null)
        throw new ArgumentNullException(nameof(importOptions));

      ChildSetMember(ref _importOptions, importOptions);
      _processOptions = options;
      ChildSetMember(ref _processData, dataSource);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IntensityCalibrationDataSource"/> class.
    /// </summary>
    /// <param name="from">Another instance to copy from.</param>
    public IntensityCalibrationDataSource(IntensityCalibrationDataSource from)
    {
      IDataSourceImportOptions importOptions = null;
      CopyHelper.Copy(ref importOptions, from._importOptions);
      ChildSetMember(ref _importOptions, importOptions);

      DataTableMultipleColumnProxy data = null;
      CopyHelper.Copy(ref data, from._processData);
      ChildSetMember(ref _processData, data);

      _processOptions = from._processOptions;
    }

    [MemberNotNull(nameof(_importOptions))]
    private void CopyFrom(IntensityCalibrationDataSource from)
    {
      using (var token = SuspendGetToken())
      {
        DataTableMultipleColumnProxy data = null;
        IDataSourceImportOptions importOptions = null;

        CopyHelper.Copy(ref data, from._processData);
        CopyHelper.Copy(ref importOptions, from._importOptions);
        _importOptions = importOptions;
        ChildSetMember(ref _processData, data);
        _processOptions = from._processOptions;
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

      if (obj is IntensityCalibrationDataSource from)
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
      return new IntensityCalibrationDataSource(this);
    }


    #region IAltaxoTableDataSource

    /// <summary>
    /// Fills (or refills) the data table with the processed data. The data source is represented by this instance, the destination table is provided in the argument <paramref name="destinationTable" />.
    /// </summary>
    /// <param name="destinationTable">The destination table.</param>
    /// <param name="reporter"></param>
    public void FillData(DataTable destinationTable, IProgressReporter reporter = null)
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
      var colY = destinationTable.DataColumns.EnsureExistence(ColumnName_Group0_ScalingFactor, typeof(DoubleColumn), ColumnKind.V, 0);

      var signal = _processData.GetDataColumnOrNull(ProxyColumnGroupName_SignalSpectrum);
      var dark = _processData.GetDataColumnOrNull(ProxyColumnGroupName_DarkSpectrum);
      var xcol = _processData.DataTable.DataColumns.FindXColumnOf(signal);
      var resulting = new DoubleColumn();

      var function = _processOptions.CurveShape;
      var para = _processOptions.CurveParameters.Select(x => x.Value).ToArray();
      var X = new double[1];
      var Y = new double[1];
      for (int i = 0; i < Math.Min(signal.Count, xcol.Count); i++)
      {
        X[0] = xcol[i];
        function.Evaluate(X, para, Y);
        if (dark is null)
          resulting[i] = Y[0] / signal[i];
        else
          resulting[i] = Y[0] / (signal[i] - dark[i]);
      }

      colX.Data = xcol;
      colY.Data = resulting;
    }

    public bool IsContainingValidYAxisCalibration(DataTable table)
    {
      var xColumn = table.DataColumns.TryGetColumn(ColumnName_Group0_SpectrumX);
      var sColumn = table.DataColumns.TryGetColumn(ColumnName_Group0_ScalingFactor);
      return xColumn is not null && sColumn is not null && Math.Min(xColumn.Count, sColumn.Count) >= 2;
    }

    public (double x, double yScalingFactor)[] GetYAxisCalibration(DataTable table)
    {
      var xColumn = table.DataColumns.TryGetColumn(ColumnName_Group0_SpectrumX);
      var sColumn = table.DataColumns.TryGetColumn(ColumnName_Group0_ScalingFactor);

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
    /// Gets or sets the data source import options.
    /// </summary>
    /// <value>
    /// The import options.
    /// </value>
    /// <exception cref="ArgumentNullException">ImportOptions</exception>
    public IDataSourceImportOptions ImportOptions
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

    public object ProcessOptionsObject { get => _processOptions; set => _processOptions = (value as IntensityCalibrationOptions) ?? throw new ArgumentNullException(nameof(ProcessOptionsObject)); }
    public object ProcessDataObject
    {
      get => _processData;
      set
      {
        if (value is DataTableMultipleColumnProxy mcp)
        {
          ChildSetMember(ref _processData, mcp);
        }
        else
        {
          throw new ArgumentNullException(nameof(ProcessDataObject));
        }
      }
    }

    #region Change event handling

    protected override bool HandleHighPriorityChildChangeCases(object? sender, ref EventArgs e)
    {
      if (ReferenceEquals(_processData, sender)) // incoming call from data proxy
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
      if (_processData is not null)
        yield return new Main.DocumentNodeAndName(_processData, "IntensityCalibrationData1");
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
      if (_processData is not null)
        _processData.VisitDocumentReferences(ReportProxies);
    }
  }
}
