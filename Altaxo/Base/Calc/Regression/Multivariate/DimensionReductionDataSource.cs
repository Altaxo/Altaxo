#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
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
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Data;

namespace Altaxo.Calc.Regression.Multivariate
{
  /// <summary>
  /// Data source for algorithms based on dimension reduction (principal component analysis (PCA), non-negative matrix factorization (NMF), etc.).
  /// </summary>
  public class DimensionReductionDataSource : TableDataSourceBase, Altaxo.Data.IAltaxoTableDataSource
  {
    private DimensionReductionOptions _processOptions;
    private DataTableMatrixProxy _processData;
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
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DimensionReductionDataSource), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (DimensionReductionDataSource)obj;

        info.AddValue("ProcessData", s._processData);
        info.AddValue("ProcessOptions", s._processOptions);
        info.AddValue("ImportOptions", s._importOptions);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        if (o is DimensionReductionDataSource s)
          s.DeserializeSurrogate0(info);
        else
          s = new DimensionReductionDataSource(info, 0);
        return s;
      }
    }

    [MemberNotNull(nameof(_importOptions), nameof(_processOptions), nameof(_processData))]
    private void DeserializeSurrogate0(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    {
      ChildSetMember(ref _processData, info.GetValue<DataTableMatrixProxy>("ProcessData", this));
      _processOptions = info.GetValue<DimensionReductionOptions>("ProcessOptions", this);
      ChildSetMember(ref _importOptions, (IDataSourceImportOptions)info.GetValue("ImportOptions", this));
      ProcessData = _processData;
    }



    #endregion Version 0

    /// <summary>
    /// Initializes a new instance of the <see cref="DimensionReductionDataSource"/> class during XML deserialization.
    /// </summary>
    /// <param name="info">The XML deserialization info.</param>
    /// <param name="version">The serialized version.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the version is not supported.</exception>
    protected DimensionReductionDataSource(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, int version)
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
    /// Initializes a new instance of the <see cref="DimensionReductionDataSource"/> class.
    /// </summary>
    /// <param name="inputData">The input data that designates the original source of data used for processing.</param>
    /// <param name="dataSourceOptions">The dimension reduction options.</param>
    /// <param name="importOptions">The data source import options.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="inputData"/>, <paramref name="dataSourceOptions"/>, or <paramref name="importOptions"/> is <c>null</c>.
    /// </exception>
    public DimensionReductionDataSource(DataTableMatrixProxy inputData, DimensionReductionOptions dataSourceOptions, IDataSourceImportOptions importOptions)
    {
      if (inputData is null)
        throw new ArgumentNullException(nameof(inputData));
      if (dataSourceOptions is null)
        throw new ArgumentNullException(nameof(dataSourceOptions));
      if (importOptions is null)
        throw new ArgumentNullException(nameof(importOptions));

      using (var token = SuspendGetToken())
      {
        ProcessOptions = dataSourceOptions;
        ImportOptions = importOptions;
        ProcessData = inputData;
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DimensionReductionDataSource"/> class by copying from another instance.
    /// </summary>
    /// <param name="from">Another instance to copy from.</param>
    public DimensionReductionDataSource(DimensionReductionDataSource from)
    {
      CopyFrom(from);
    }

    [MemberNotNull(nameof(_importOptions), nameof(_processOptions), nameof(_processData))]
    private void CopyFrom(DimensionReductionDataSource from)
    {
      using (var token = SuspendGetToken())
      {
        DimensionReductionOptions? dataSourceOptions = null;
        DataTableMatrixProxy? inputData = null;
        IDataSourceImportOptions? importOptions = null;

        CopyHelper.Copy(ref importOptions, from._importOptions);
        CopyHelper.CopyI(ref dataSourceOptions, from._processOptions);
        CopyHelper.Copy(ref inputData, from._processData);

        ProcessOptions = dataSourceOptions;
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

      if (obj is DimensionReductionDataSource from)
      {
        CopyFrom(from);
        return true;
      }
      return false;
    }

    /// <inheritdoc/>
    public object Clone()
    {
      return new DimensionReductionDataSource(this);
    }

    #region IAltaxoTableDataSource

    /// <inheritdoc/>
    public override void FillData_Unchecked(DataTable destinationTable, IProgressReporter reporter)
    {
      destinationTable.DataColumns.RemoveColumnsAll();

      LinearAlgebraProviderSettings.ApplyAlgebraProviderSetting();

      var (matrix, xOfXRaw, columnNumbers) = _processData.GetMatrixAndColumnNumbers(transposeMatrix: true);

      var (xOfXPreprocessed, matrixXPreprocessed, region, auxiliaryData) = ProcessOptions.SinglePreprocessing.Execute(xOfXRaw, matrix, null);

      var result = _processOptions.DimensionReductionMethod.ExecuteDimensionReduction(matrixXPreprocessed);

      result.SaveResultToTable(_processOptions.OutputOptions, _processData.DataTable, destinationTable, columnNumbers, xOfXPreprocessed, matrixXPreprocessed, auxiliaryData);
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
    public DataTableMatrixProxy ProcessData
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
        if (ChildSetMember(ref _importOptions, value ?? throw new ArgumentNullException(nameof(value))))
        {
          EhChildChanged(_importOptions, EventArgs.Empty);
        }
      }
    }

    /// <summary>
    /// Gets or sets the data source options.
    /// </summary>
    /// <value>
    /// The data source options.
    /// </value>
    public DimensionReductionOptions ProcessOptions
    {
      get
      {
        return _processOptions;
      }
      [MemberNotNull(nameof(_processOptions))]
      set
      {
        if (!object.ReferenceEquals(_processOptions, value))
        {
          _processOptions = value;
          EhChildChanged(_processOptions, EventArgs.Empty);
        }
      }
    }

    /// <inheritdoc/>
    object IAltaxoTableDataSource.ProcessOptionsObject
    {
      get => _processOptions;
      set => ProcessOptions = (DimensionReductionOptions)value;
    }

    /// <inheritdoc/>
    object IAltaxoTableDataSource.ProcessDataObject
    {
      get => _processData;
      set => ProcessData = (DataTableMatrixProxy)value;
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

      if (_importOptions is not null)
        yield return new Main.DocumentNodeAndName(_importOptions, "ImportOptions");

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
        _processData.VisitDocumentReferences(ReportProxies);
      }
    }

    #endregion IAltaxoTableDataSource
  }
}

