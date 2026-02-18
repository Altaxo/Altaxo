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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using Altaxo.Data;
using Altaxo.Serialization.Xml;

namespace Altaxo.Science.Spectroscopy
{
  /// <summary>
  /// Table data source for applying the <see cref="PeakSearchingAndFittingOptions"/> to columns of a table.
  /// </summary>
  /// <seealso cref="Altaxo.Data.TableDataSourceBase" />
  /// <seealso cref="Altaxo.Data.IAltaxoTableDataSource" />
  public class PeakSearchingAndFittingDataSource : TableDataSourceBase, Altaxo.Data.IAltaxoTableDataSource
  {
    private PeakSearchingAndFittingOptionsDocNode _processOptions;
    private ListOfXAndYColumn _processData;
    private IDataSourceImportOptions _importOptions;

    public Action<IAltaxoTableDataSource>? _dataSourceChanged;

    #region Serialization

    /// <summary>
    /// This resolver is neccessary because in Version 0 and 1 the ProcessData was stored as DataTableMultipleColumnProxy without X column.
    /// The resolver waits until the deserialization is finished and then tries to resolve the X column from the data table.
    /// </summary>
    private class ColumnProxyResolver
    {
      private bool _isResolved;
      private DataTableMultipleColumnProxy _mcp;
      private PeakSearchingAndFittingDataSource _dataSource;

      public ColumnProxyResolver(DataTableMultipleColumnProxy mcp, PeakSearchingAndFittingDataSource spectralPreprocessingDataSource, IXmlDeserializationInfo info)
      {
        this._mcp = mcp;
        this._dataSource = spectralPreprocessingDataSource;

        info.DeserializationFinished += EhInfo_DeserializationFinished;
      }

      private void EhInfo_DeserializationFinished(IXmlDeserializationInfo info, object documentRoot, bool isFinalCall)
      {
        if (!_isResolved)
        {
          DataColumn? xCol = null;

          if (_mcp.DataTable is { } dt)
          {
            xCol = dt.DataColumns.FindXColumnOfGroup(_mcp.GroupNumber);
          }

          if (isFinalCall && xCol is null)
          {
            var vColProxies = _mcp.GetDataColumnProxies(SpectroscopyCommands.ColumnsV);
            var vCol = (DataColumn)vColProxies.FirstOrDefault(p => p.Document() is DataColumn)?.Document();
            if (vCol is not null)
            {
              var dtc = DataColumnCollection.GetParentDataColumnCollectionOf(vCol);
              if (dtc is not null)
              {
                xCol = dtc.FindXColumnOf(vCol) ?? (dtc.ColumnCount > 0 ? dtc[0] : null);
              }
            }
          }
          if (xCol is not null)
          {
            var xyList = _mcp.TransformToListOfXAndYColumn(xCol, SpectroscopyCommands.ColumnsV, false);
            _dataSource.ChildSetMember(ref _dataSource._processData, xyList);

            _isResolved = true;
            _mcp = null!;
            _dataSource = null!;
          }
        }

        if (!_isResolved && isFinalCall)
        {
          _dataSource.ChildSetMember(ref _dataSource._processData, new ListOfXAndYColumn());
        }
      }
    }

    #region Version 0

    /// <summary>
    /// 2022-06-08 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Data.PeakFindingAndFittingDataSource", 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new InvalidOperationException("Serialization of old version");

        /*
        var s = (PeakFindingAndFittingDataSource)obj;

        info.AddValue("ProcessData", s._processData);
        info.AddValue("ProcessOptions", s._processOptions);
        info.AddValue("ImportOptions", s._importOptions);
        */
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        if (o is PeakSearchingAndFittingDataSource s)
          s.DeserializeSurrogate0(info);
        else
          s = new PeakSearchingAndFittingDataSource(info, 0);
        return s;
      }
    }

    [MemberNotNull(nameof(_importOptions), nameof(_processOptions), nameof(_processData))]
    private void DeserializeSurrogate0(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    {
      var mcp = (DataTableMultipleColumnProxy)info.GetValue("ProcessData", this);
      if (!mcp.ContainsIdentifier(SpectroscopyCommands.ColumnX) && mcp.ContainsIdentifier(SpectroscopyCommands.ColumnsV))
      {
        new ColumnProxyResolver(mcp, this, info);
      }
      else
      {
        ChildSetMember(ref _processData, mcp.TransformToListOfXAndYColumn(SpectroscopyCommands.ColumnX, SpectroscopyCommands.ColumnsV, false));
      }

      ProcessOptions = (PeakSearchingAndFittingOptions)info.GetValue("ProcessOptions", this);
      _importOptions = (IDataSourceImportOptions)info.GetValue("ImportOptions", this);
    }

    #endregion Version 0

    #region Version 1

    /// <summary>
    /// 2022-06-08 initial version.
    /// 2022-08-05 change processOptions from PeakFindingAndFittingOptions to PeakFindingAndFittingOptionsDocNode, change namespace from Altaxo.Data to Altaxo.Science.Spectroscopy, change class name from PeakFindingAndFittingDataSource to PeakSearchingAndFittingDataSource
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Science.Spectroscopy.PeakSearchingAndFittingDataSource", 1)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PeakSearchingAndFittingDataSource)obj;

        info.AddValue("ProcessData", s._processData);
        info.AddValue("ProcessOptions", s._processOptions);
        info.AddValue("ImportOptions", s._importOptions);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        if (o is PeakSearchingAndFittingDataSource s)
          s.DeserializeSurrogate1(info);
        else
          s = new PeakSearchingAndFittingDataSource(info, 1);
        return s;
      }
    }

    [MemberNotNull(nameof(_importOptions), nameof(_processOptions), nameof(_processData))]
    private void DeserializeSurrogate1(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    {
      var mcp = (DataTableMultipleColumnProxy)info.GetValue("ProcessData", this);
      if (!mcp.ContainsIdentifier(SpectroscopyCommands.ColumnX) && mcp.ContainsIdentifier(SpectroscopyCommands.ColumnsV))
      {
        new ColumnProxyResolver(mcp, this, info);
      }
      else
      {
        ChildSetMember(ref _processData, mcp.TransformToListOfXAndYColumn(SpectroscopyCommands.ColumnX, SpectroscopyCommands.ColumnsV, false));
      }

      ChildSetMember(ref _processOptions, (PeakSearchingAndFittingOptionsDocNode)info.GetValue("ProcessOptions", this));
      _importOptions = (IDataSourceImportOptions)info.GetValue("ImportOptions", this);
    }

    #endregion Version 1

    #region Version 2

    /// <summary>
    /// 2022-06-08 initial version.
    /// 2022-08-05 change processOptions from PeakFindingAndFittingOptions to PeakFindingAndFittingOptionsDocNode, change namespace from Altaxo.Data to Altaxo.Science.Spectroscopy, change class name from PeakFindingAndFittingDataSource to PeakSearchingAndFittingDataSource
    /// 2025-10-27 Change DataTableMultipleColumnProxy to ListOfXAndYColumn
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(PeakSearchingAndFittingDataSource), 2)]
    private class XmlSerializationSurrogate2 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (PeakSearchingAndFittingDataSource)obj;

        info.AddValue("ProcessData", s._processData);
        info.AddValue("ProcessOptions", s._processOptions);
        info.AddValue("ImportOptions", s._importOptions);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        if (o is PeakSearchingAndFittingDataSource s)
          s.DeserializeSurrogate2(info);
        else
          s = new PeakSearchingAndFittingDataSource(info, 2);
        return s;
      }
    }

    [MemberNotNull(nameof(_importOptions), nameof(_processOptions), nameof(_processData))]
    private void DeserializeSurrogate2(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    {
      ChildSetMember(ref _processData, (ListOfXAndYColumn)info.GetValue("ProcessData", this));
      ChildSetMember(ref _processOptions, (PeakSearchingAndFittingOptionsDocNode)info.GetValue("ProcessOptions", this));
      _importOptions = (IDataSourceImportOptions)info.GetValue("ImportOptions", this);
    }

    #endregion Version 2

    protected PeakSearchingAndFittingDataSource(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, int version)
    {
      switch (version)
      {
        case 0:
          DeserializeSurrogate0(info);
          break;
        case 1:
          DeserializeSurrogate1(info);
          break;
        case 2:
          DeserializeSurrogate2(info);
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof(version));
      }
    }

    #endregion Serialization

    /// <summary>
    /// Initializes a new instance of the <see cref="ConvertXYVToMatrixDataSource"/> class.
    /// </summary>
    /// <param name="inputData">The input data designates the original source of data (used then for the processing).</param>
    /// <param name="dataSourceOptions">The Fourier transformation options.</param>
    /// <param name="importOptions">The data source import options.</param>
    /// <exception cref="System.ArgumentNullException">
    /// inputData
    /// or
    /// transformationOptions
    /// or
    /// importOptions
    /// </exception>
    public PeakSearchingAndFittingDataSource(ListOfXAndYColumn inputData, PeakSearchingAndFittingOptions dataSourceOptions, IDataSourceImportOptions importOptions)
    {
      if (inputData is null)
        throw new ArgumentNullException(nameof(inputData));
      if (dataSourceOptions is null)
        throw new ArgumentNullException(nameof(dataSourceOptions));
      if (importOptions is null)
        throw new ArgumentNullException(nameof(importOptions));

      ChildSetMember(ref _processOptions, new PeakSearchingAndFittingOptionsDocNode(dataSourceOptions));
      ChildSetMember(ref _processData, inputData);
      _importOptions = importOptions;

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PeakSearchingAndFittingDataSource"/> class.
    /// </summary>
    /// <param name="from">Another instance to copy from.</param>
    public PeakSearchingAndFittingDataSource(PeakSearchingAndFittingDataSource from)
    {
      CopyFrom(from);
    }

    [MemberNotNull(nameof(_importOptions), nameof(_processOptions), nameof(_processData))]
    public void CopyFrom(PeakSearchingAndFittingDataSource from)
    {
      if (ReferenceEquals(this, from))
#pragma warning disable CS8774 // Member must have a non-null SpectralPreprocessingDataSource when exiting.
        return;
#pragma warning restore CS8774 // Member must have a non-null value when exiting.

      using (var token = SuspendGetToken())
      {
        ListOfXAndYColumn? processData = null;

        _importOptions = from._importOptions;
        CopyHelper.Copy(ref processData, from._processData);

        ProcessOptions = from.ProcessOptions;
        ImportOptions = _importOptions;
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

      if (obj is PeakSearchingAndFittingDataSource from)
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
      return new PeakSearchingAndFittingDataSource(this);
    }

    #region IAltaxoTableDataSource

    /// <summary>
    /// Fills (or refills) the data table with the processed data. The data source is represented by this instance, the destination table is provided in the argument <paramref name="destinationTable" />.
    /// </summary>
    /// <param name="destinationTable">The destination table.</param>
    /// <param name="reporter"></param>
    public override void FillData_Unchecked(DataTable destinationTable, IProgressReporter reporter)
    {
      var peakFindingAndFittingOptions = _processOptions.GetPeakSearchingAndFittingOptions();
      SpectroscopyCommands.ExecutePeakFindingAndFitting(_processData, peakFindingAndFittingOptions, destinationTable, reporter, reporter?.CancellationToken ?? CancellationToken.None, reporter?.CancellationTokenHard ?? CancellationToken.None);
    }

    /// <summary>
    /// Occurs when the data source has changed and the import trigger source is DataSourceChanged. The argument is the sender of this event.
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
    public ListOfXAndYColumn ProcessData
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
    /// <exception cref="System.ArgumentNullException">ImportOptions</exception>
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
    /// Gets or sets the options for this data source.
    /// </summary>
    /// <value>
    /// The options for this data source.
    /// </value>
    public PeakSearchingAndFittingOptions ProcessOptions
    {
      get
      {
        return _processOptions.GetPeakSearchingAndFittingOptions();
      }
      [MemberNotNull(nameof(_processOptions))]
      set
      {
        if (value is null)
          throw new ArgumentNullException(nameof(ProcessOptions));

        if (_processOptions is null || !object.Equals(_processOptions.GetPeakSearchingAndFittingOptions(), value))
        {
          ChildSetMember(ref _processOptions, new PeakSearchingAndFittingOptionsDocNode(value));
          EhChildChanged(_processOptions, EventArgs.Empty);
        }
      }
    }

    object IAltaxoTableDataSource.ProcessOptionsObject
    {
      get => ProcessOptions;
      set => ProcessOptions = (PeakSearchingAndFittingOptions)value;
    }

    object IAltaxoTableDataSource.ProcessDataObject
    {
      get => _processData;
      set => ProcessData = (ListOfXAndYColumn)value;
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

    #endregion IAltaxoTableDataSource

  }
}
