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

namespace Altaxo.Science.Spectroscopy
{
  public class SpectralPreprocessingDataSource : TableDataSourceBase, Altaxo.Data.IAltaxoTableDataSource
  {
    private SpectralPreprocessingOptionsDocNode _processOptions;
    private DataTableMultipleColumnProxy _processData;
    private IDataSourceImportOptions _importOptions;

    public Action<IAltaxoTableDataSource>? _dataSourceChanged;

    #region Serialization

    #region Version 0

    /// <summary>
    /// 2022-06-08 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Data.SpectralPreprocessingDataSource", 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new InvalidOperationException("Try to serialize old version");

        /*
        var s = (SpectralPreprocessingDataSource)obj;

        info.AddValue("ProcessData", s._processData);
        info.AddValue("ProcessOptions", s._processOptions);
        info.AddValue("ImportOptions", s._importOptions);
        */
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        if (o is SpectralPreprocessingDataSource s)
          s.DeserializeSurrogate0(info);
        else
          s = new SpectralPreprocessingDataSource(info, 0);
        return s;
      }
    }

    [MemberNotNull(nameof(_importOptions), nameof(_processOptions), nameof(_processData))]
    private void DeserializeSurrogate0(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    {
      ChildSetMember(ref _processData, info.GetValue<DataTableMultipleColumnProxy>("ProcessData", this));
      ProcessOptions = info.GetValue<SpectralPreprocessingOptionsBase>("ProcessOptions", this);
      ChildSetMember(ref _importOptions, info.GetValue<IDataSourceImportOptions>("ImportOptions", this));
    }

    #endregion Version 0

    #region Version 1

    /// <summary>
    /// 2022-06-08 initial version.
    /// 2022-08-05 change processOptions from SpectralPreprocessingOptions to SpectralPreprocessingOptionsDocNode, change namespace from Altaxo.Data to Altaxo.Science.Spectroscopy
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(SpectralPreprocessingDataSource), 1)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (SpectralPreprocessingDataSource)obj;

        info.AddValue("ProcessData", s._processData);
        info.AddValue("ProcessOptions", s._processOptions);
        info.AddValue("ImportOptions", s._importOptions);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        if (o is SpectralPreprocessingDataSource s)
          s.DeserializeSurrogate1(info);
        else
          s = new SpectralPreprocessingDataSource(info, 1);
        return s;
      }
    }

    [MemberNotNull(nameof(_importOptions), nameof(_processOptions), nameof(_processData))]
    private void DeserializeSurrogate1(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    {
      ChildSetMember(ref _processData, (DataTableMultipleColumnProxy)info.GetValue("ProcessData", this));
      ChildSetMember(ref _processOptions, (SpectralPreprocessingOptionsDocNode)info.GetValue("ProcessOptions", this));
      ChildSetMember(ref _importOptions, (IDataSourceImportOptions)info.GetValue("ImportOptions", this));
    }

    #endregion Version 1

    protected SpectralPreprocessingDataSource(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, int version)
    {
      switch (version)
      {
        case 0:
          DeserializeSurrogate0(info);
          break;
        case 1:
          DeserializeSurrogate1(info);
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
    public SpectralPreprocessingDataSource(DataTableMultipleColumnProxy inputData, SpectralPreprocessingOptionsBase dataSourceOptions, IDataSourceImportOptions importOptions)
    {
      if (inputData is null)
        throw new ArgumentNullException(nameof(inputData));
      if (dataSourceOptions is null)
        throw new ArgumentNullException(nameof(dataSourceOptions));
      if (importOptions is null)
        throw new ArgumentNullException(nameof(importOptions));

      ChildSetMember(ref _processOptions, new SpectralPreprocessingOptionsDocNode(dataSourceOptions));
      ChildSetMember(ref _processData, inputData);
      ChildSetMember(ref _importOptions, importOptions);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SpectralPreprocessingDataSource"/> class.
    /// </summary>
    /// <param name="from">Another instance to copy from.</param>
    public SpectralPreprocessingDataSource(SpectralPreprocessingDataSource from)
    {
      CopyFrom(from);
    }

    [MemberNotNull(nameof(_importOptions), nameof(_processOptions), nameof(_processData))]
    public void CopyFrom(SpectralPreprocessingDataSource from)
    {
      if (ReferenceEquals(this, from))
#pragma warning disable CS8774 // Member must have a non-null SpectralPreprocessingDataSource when exiting.
        return;
#pragma warning restore CS8774 // Member must have a non-null value when exiting.

      using (var token = SuspendGetToken())
      {
        DataTableMultipleColumnProxy? processData = null;
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

      if (obj is SpectralPreprocessingDataSource from)
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
      return new SpectralPreprocessingDataSource(this);
    }

    #region IAltaxoTableDataSource

    /// <summary>
    /// Fills (or refills) the data table with the processed data. The data source is represented by this instance, the destination table is provided in the argument <paramref name="destinationTable" />.
    /// </summary>
    /// <param name="destinationTable">The destination table.</param>
    /// <param name="reporter"></param>
    public override void FillData_Unchecked(DataTable destinationTable, IProgressReporter reporter)
    {
      var spectralPreprocessingOptions = _processOptions.GetSpectralPreprocessingOptions();
      SpectroscopyCommands.ExecuteSpectralPreprocessing(_processData, spectralPreprocessingOptions, destinationTable);
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
    public DataTableMultipleColumnProxy ProcessData
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
    /// Gets or sets the options for this data source.
    /// </summary>
    /// <value>
    /// The options for this data source.
    /// </value>
    public SpectralPreprocessingOptionsBase ProcessOptions
    {
      get
      {
        return _processOptions.GetSpectralPreprocessingOptions();
      }
      [MemberNotNull(nameof(_processOptions))]
      set
      {
        if (value is null)
          throw new ArgumentNullException(nameof(ProcessOptions));

        if (_processOptions is null || !object.Equals(_processOptions.GetSpectralPreprocessingOptions(), value))
        {
          ChildSetMember(ref _processOptions, new SpectralPreprocessingOptionsDocNode(value));
          EhChildChanged(_processOptions, EventArgs.Empty);
        }
      }
    }

    object IAltaxoTableDataSource.ProcessOptionsObject
    {
      get => ProcessOptions;
      set => ProcessOptions = (SpectralPreprocessingOptionsBase)value;
    }

    object IAltaxoTableDataSource.ProcessDataObject
    {
      get => _processData;
      set => ProcessData = (DataTableMultipleColumnProxy)value;
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

    #endregion IAltaxoTableDataSource

  }
}
