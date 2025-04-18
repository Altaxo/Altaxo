﻿#region Copyright

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

#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Altaxo.Scripting;

namespace Altaxo.Data
{
  public class ProcessSourceTablesScriptDataSource : TableDataSourceBase, Altaxo.Data.IAltaxoTableDataSource
  {
    private ProcessSourceTablesScript _processOptions;
    private ProcessSourceTablesScriptData _processData;
    private IDataSourceImportOptions _importOptions;

    public Action<IAltaxoTableDataSource>? _dataSourceChanged;

    #region Serialization

    #region Version 0

    /// <summary>
    /// 2014-11-02 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ProcessSourceTablesScriptDataSource), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ProcessSourceTablesScriptDataSource)obj;

        info.AddValue("ProcessData", s._processData);
        info.AddValue("ProcessOptions", s._processOptions);
        info.AddValue("ImportOptions", s._importOptions);
      }



      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        if (o is ProcessSourceTablesScriptDataSource s)
          s.DeserializeSurrogate0(info);
        else
          s = new ProcessSourceTablesScriptDataSource(info, 0);
        return s;
      }
    }

    [MemberNotNull(nameof(_importOptions), nameof(_processOptions), nameof(_processData))]
    private void DeserializeSurrogate0(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    {
      ChildSetMember(ref _processData, (ProcessSourceTablesScriptData)info.GetValue("ProcessData", this));
      ChildSetMember(ref _processOptions, info.GetValue<ProcessSourceTablesScript>("ProcessOptions", this));
      ChildSetMember(ref _importOptions, (IDataSourceImportOptions)info.GetValue("ImportOptions", this));
    }

    #endregion Version 0

    protected ProcessSourceTablesScriptDataSource(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, int version)
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
    /// Initializes a new instance of the <see cref="ProcessSourceTablesScriptDataSource"/> class.
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
    public ProcessSourceTablesScriptDataSource(ProcessSourceTablesScriptData inputData, ProcessSourceTablesScript dataSourceOptions, IDataSourceImportOptions importOptions)
    {
      if (inputData is null)
        throw new ArgumentNullException(nameof(inputData));
      if (dataSourceOptions is null)
        throw new ArgumentNullException(nameof(dataSourceOptions));
      if (importOptions is null)
        throw new ArgumentNullException(nameof(importOptions));


      ChildSetMember(ref _processOptions, dataSourceOptions);
      ChildSetMember(ref _importOptions, importOptions);
      ChildSetMember(ref _processData, inputData);

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProcessSourceTablesScriptDataSource"/> class.
    /// </summary>
    /// <param name="from">Another instance to copy from.</param>
    public ProcessSourceTablesScriptDataSource(ProcessSourceTablesScriptDataSource from)
    {
      CopyFrom(from);
    }

    [MemberNotNull(nameof(_importOptions), nameof(_processOptions), nameof(_processData))]
    private void CopyFrom(ProcessSourceTablesScriptDataSource from)
    {
      using (var token = SuspendGetToken())
      {
        ProcessSourceTablesScriptData? inputData = null;
        ProcessSourceTablesScript? processOptions = null;
        IDataSourceImportOptions? importOptions = null;

        CopyHelper.Copy(ref importOptions, from._importOptions);
        CopyHelper.Copy(ref processOptions, from._processOptions);
        CopyHelper.Copy(ref inputData, from._processData);

        ImportOptions = importOptions;
        ProcessData = inputData;
        ProcessOptions = processOptions;

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

      if (obj is ProcessSourceTablesScriptDataSource from)
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
      return new ProcessSourceTablesScriptDataSource(this);
    }

    #region IAltaxoTableDataSource

    /// <summary>
    /// Fills (or refills) the data table with the processed data. The data source is represented by this instance, the destination table is provided in the argument <paramref name="destinationTable" />.
    /// </summary>
    /// <param name="destinationTable">The destination table.</param>
    /// <param name="reporter"></param>
    public override void FillData_Unchecked(DataTable destinationTable, IProgressReporter reporter)
    {
      destinationTable.DataColumns.RemoveColumnsAll();
      destinationTable.PropertyColumns.RemoveColumnsAll();
      _processOptions.ExecuteWithoutExceptionCatching(destinationTable, _processData.Tables, reporter);
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
    /// The input data. This data is the input for the 2D-Fourier transformation.
    /// </value>
    public ProcessSourceTablesScriptData ProcessData
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
    /// <exception cref="System.ArgumentNullException">FourierTransformation2DOptions</exception>
    public ProcessSourceTablesScript ProcessOptions
    {
      get
      {
        return _processOptions;
      }
      [MemberNotNull(nameof(_processOptions))]
      set
      {
        if (!object.Equals(_processOptions, value))
        {
          _processOptions = value;
          EhChildChanged(_processOptions, EventArgs.Empty);
        }
      }
    }

    object IAltaxoTableDataSource.ProcessOptionsObject
    {
      get => _processOptions;
      set => ProcessOptions = (ProcessSourceTablesScript)value;
    }

    object IAltaxoTableDataSource.ProcessDataObject
    {
      get => _processData;
      set => ProcessData = (ProcessSourceTablesScriptData)value;
    }

    #region Change event handling

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

    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (_processData is not null)
        yield return new Main.DocumentNodeAndName(_processData, "ProcessData");
      if (_processOptions is not null)
        yield return new Main.DocumentNodeAndName(_processOptions, "ProcessOptions");
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
      {
        _processData.VisitDocumentReferences(ReportProxies);
      }
    }

    #endregion IAltaxoTableDataSource
  }
}
