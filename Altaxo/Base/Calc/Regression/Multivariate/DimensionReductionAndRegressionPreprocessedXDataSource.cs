#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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
using Altaxo.Data;

namespace Altaxo.Calc.Regression.Multivariate
{
  /// <summary>
  /// Data source for a table that contains preprocessed spectra originating from a <see cref="DimensionReductionAndRegressionDataSource"/>.
  /// Thus, this data source contains only a reference to a table containing the <see cref="DimensionReductionAndRegressionDataSource"/>.
  /// All data required to obtain the preprocessed spectra are contained in that data source.
  /// </summary>
  public class DimensionReductionAndRegressionPreprocessedXDataSource : TableDataSourceBase, Altaxo.Data.IAltaxoTableDataSource
  {
    private DataTableProxy _processData;
    private IDataSourceImportOptions _importOptions;

    /// <summary>
    /// Backing field for the <see cref="DataSourceChanged"/> event.
    /// </summary>
    public Action<IAltaxoTableDataSource>? _dataSourceChanged;

    #region Serialization

    #region Version 0

    /// <summary>
    /// 2022-06-28 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DimensionReductionAndRegressionPreprocessedXDataSource), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (DimensionReductionAndRegressionPreprocessedXDataSource)obj;

        info.AddValue("ProcessData", s._processData);
        info.AddValue("ImportOptions", s._importOptions);
      }



      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        if (o is DimensionReductionAndRegressionPreprocessedXDataSource s)
          s.DeserializeSurrogate0(info);
        else
          s = new DimensionReductionAndRegressionPreprocessedXDataSource(info, 0);
        return s;
      }
    }

    [MemberNotNull(nameof(_importOptions), nameof(_processData))]
    void DeserializeSurrogate0(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    {
      ChildSetMember(ref _processData, (DataTableProxy)info.GetValue("ProcessData", this));
      _importOptions = info.GetValue<IDataSourceImportOptions>("ImportOptions", this);

      ProcessData = _processData;
    }



    #endregion Version 0

    /// <summary>
    /// Initializes a new instance of the <see cref="DimensionReductionAndRegressionPreprocessedXDataSource"/> class during XML deserialization.
    /// </summary>
    /// <param name="info">The XML deserialization info.</param>
    /// <param name="version">The serialized version.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the version is not supported.</exception>
    protected DimensionReductionAndRegressionPreprocessedXDataSource(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, int version)
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
    /// Initializes a new instance of the <see cref="DimensionReductionAndRegressionPreprocessedXDataSource"/> class.
    /// </summary>
    /// <param name="inputData">The input data designates the table that provides the preprocessing settings.</param>
    /// <param name="importOptions">The data source import options.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="inputData"/> or <paramref name="importOptions"/> is <c>null</c>.</exception>
    public DimensionReductionAndRegressionPreprocessedXDataSource(DataTableProxy inputData, IDataSourceImportOptions importOptions)
    {
      if (inputData is null)
        throw new ArgumentNullException(nameof(inputData));
      if (importOptions is null)
        throw new ArgumentNullException(nameof(importOptions));

      using (var token = SuspendGetToken())
      {
        ImportOptions = importOptions;
        ProcessData = inputData;
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DimensionReductionAndRegressionPreprocessedXDataSource"/> class by copying from another instance.
    /// </summary>
    /// <param name="from">Another instance to copy from.</param>
    public DimensionReductionAndRegressionPreprocessedXDataSource(DimensionReductionAndRegressionPreprocessedXDataSource from)
    {
      CopyFrom(from);
    }

    [MemberNotNull(nameof(_importOptions), nameof(_processData))]
    void CopyFrom(DimensionReductionAndRegressionPreprocessedXDataSource from)
    {
      using (var token = SuspendGetToken())
      {
        DataTableProxy? inputData = null;
        IDataSourceImportOptions? importOptions = null;

        _importOptions = from._importOptions;
        CopyHelper.Copy(ref inputData, from._processData);

        ImportOptions = importOptions;
        ProcessData = inputData;

      }
    }

    /// <summary>
    /// Copies from another instance.
    /// </summary>
    /// <param name="obj">The object to copy from.</param>
    /// <returns><c>true</c> if anything could be copied from the object; otherwise, <c>false</c>.</returns>
    public bool CopyFrom(object obj)
    {
      if (ReferenceEquals(this, obj))
        return true;

      if (obj is DimensionReductionAndRegressionPreprocessedXDataSource from)
      {
        CopyFrom(from);
        return true;
      }
      return false;
    }

    /// <inheritdoc/>
    public object Clone()
    {
      return new DimensionReductionAndRegressionPreprocessedXDataSource(this);
    }

    #region IAltaxoTableDataSource

    /// <inheritdoc/>
    public override void FillData_Unchecked(DataTable destinationTable, IProgressReporter reporter)
    {
      destinationTable.DataColumns.RemoveColumnsAll();
      var srctable = _processData.Document;
      var dataSource = srctable.DataSource as DimensionReductionAndRegressionDataSource;
      dataSource.ProcessOptions.WorksheetAnalysis.CalculatePreprocessedSpectra(srctable, destinationTable);
    }

    /// <summary>
    /// Occurs when the data source has changed and the import trigger source is <see cref="ImportTriggerSource.DataSourceChanged"/>.
    /// The argument is the sender of this event.
    /// </summary>
    public event Action<Data.IAltaxoTableDataSource> DataSourceChanged
    {
      add
      {
        bool isFirst = _dataSourceChanged is null;
        _dataSourceChanged += value;
        if (isFirst)
        {
          //EhInputDataChanged(this, EventArgs.Empty);
        }
      }
      remove
      {
        _dataSourceChanged -= value;
        bool isLast = _dataSourceChanged is null;
        if (isLast)
        {
        }
      }
    }

    /// <summary>
    /// Gets or sets the input data.
    /// </summary>
    /// <value>
    /// The input data.
    /// </value>
    public DataTableProxy ProcessData
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

    /// <inheritdoc/>
    public override Data.IDataSourceImportOptions ImportOptions
    {
      get
      {
        return _importOptions;
      }
      [MemberNotNull(nameof(_importOptions))]
      set
      {
        if (!object.Equals(_importOptions, value ?? throw new ArgumentNullException(nameof(value))))
        {
          _importOptions = value;
          EhChildChanged(_importOptions, EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// Gets or sets the data source options.
    /// </summary>
    /// <remarks>
    /// This data source does not use additional options; therefore, the getter always returns <c>null</c> and the setter is ignored.
    /// </remarks>
    /// <value>
    /// The data source options.
    /// </value>
    public DimensionReductionAndRegressionOptions ProcessOptions
    {
      get
      {
        return null;
      }
      set
      {
      }
    }



    /// <inheritdoc/>
    object IAltaxoTableDataSource.ProcessOptionsObject
    {
      get => null;
      set { }
    }

    /// <inheritdoc/>
    object IAltaxoTableDataSource.ProcessDataObject
    {
      get => _processData;
      set => ProcessData = (DataTableProxy)value;
    }

    #region Change event handling

    /// <inheritdoc/>
    protected override bool HandleHighPriorityChildChangeCases(object? sender, ref EventArgs e)
    {
      if (object.ReferenceEquals(_processData, sender)) // incoming call from data proxy
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

    /// <inheritdoc/>
    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (_processData is not null)
        yield return new Main.DocumentNodeAndName(_processData, "ProcessData");
    }

    #endregion Document Node functions

    /// <inheritdoc/>
    public void OnAfterDeserialization()
    {
    }

    /// <inheritdoc/>
    public void VisitDocumentReferences(Main.DocNodeProxyReporter ReportProxies)
    {
      if (_processData is not null)
      {
        ReportProxies(_processData, this, nameof(ProcessData));
      }
    }

    #endregion IAltaxoTableDataSource
  }
}

