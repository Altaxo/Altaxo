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
using Altaxo.Main;

namespace Altaxo.Data
{
  /// <summary>
  /// A <see cref="TableDataSourceBase"/> which has options that are immutable.
  /// </summary>
  /// <typeparam name="TOptions">The type of the options.</typeparam>
  /// <typeparam name="TData">The type of the data.</typeparam>
  /// <seealso cref="Altaxo.Data.TableDataSourceBase" />
  /// <seealso cref="Altaxo.Data.IAltaxoTableDataSource" />
  public abstract class TableDataSourceBaseImmutableOptions<TOptions, TData> :
    TableDataSourceBase,
    Altaxo.Data.IAltaxoTableDataSource
    where TOptions : Main.IImmutable
    where TData : IDocumentLeafNode, ICloneable
  {
    protected TOptions _processOptions;
    protected TData _processData;
    protected IDataSourceImportOptions _importOptions;
    public Action<IAltaxoTableDataSource>? _dataSourceChanged;

    #region Serialization

    protected abstract class XmlSerializationSurrogateBase : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (TableDataSourceBaseImmutableOptions<TOptions, TData>)obj;
        info.AddValue("ProcessData", s._processData);
        info.AddValue("ProcessOptions", s._processOptions);
        info.AddValue("ImportOptions", s._importOptions);
      }

      public abstract object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent);
    }

    protected TableDataSourceBaseImmutableOptions(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, int version)
    {
      var processData = info.GetValue<TData>("ProcessData", this);
      var processOptions = info.GetValue<TOptions>("ProcessOptions", this);
      var importOptions = info.GetValue<IDataSourceImportOptions>("ImportOptions", this);

      ChildSetMember(ref _processData, processData);
      ProcessOptions = processOptions;
      _importOptions = importOptions;
    }

    #endregion


    /// <summary>
    /// Initializes a new instance of the <see cref="TableDataSourceBaseImmutableOptions{TOptions, TData}"/> class.
    /// </summary>
    /// <param name="inputData">The input data.</param>
    /// <param name="dataSourceOptions">The data source options. This class or record must be immutable.</param>
    /// <param name="importOptions">The import options.</param>
    /// <exception cref="Markdig.Helpers.ThrowHelper.ArgumentNullException(System.String)">
    /// inputData
    /// or
    /// dataSourceOptions
    /// or
    /// importOptions
    /// </exception>
    public TableDataSourceBaseImmutableOptions(TData inputData, TOptions dataSourceOptions, IDataSourceImportOptions importOptions)
    {
      if (inputData is null)
        throw new ArgumentNullException(nameof(inputData));
      if (dataSourceOptions is null)
        throw new ArgumentNullException(nameof(dataSourceOptions));
      if (importOptions is null)
        throw new ArgumentNullException(nameof(importOptions));

      _processOptions = dataSourceOptions;
      ChildSetMember(ref _processData, inputData);
      _importOptions = importOptions;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TableDataSourceBaseImmutableOptions{TOptions, TData}"/> class.
    /// </summary>
    /// <param name="from">Another instance to copy from.</param>
    public TableDataSourceBaseImmutableOptions(TableDataSourceBaseImmutableOptions<TOptions, TData> from)
    {
      CopyFrom(from);
    }

    [MemberNotNull(nameof(_importOptions), nameof(_processOptions), nameof(_processData))]
    public void CopyFrom(TableDataSourceBaseImmutableOptions<TOptions, TData> from)
    {
      if (ReferenceEquals(this, from))
#pragma warning disable CS8774 // Member must have a non-null SpectralPreprocessingDataSource when exiting.
        return;
#pragma warning restore CS8774 // Member must have a non-null value when exiting.

      using (var token = SuspendGetToken())
      {
        TData? processData = default;
        IDataSourceImportOptions? importOptions = null;

        _importOptions = from._importOptions;
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

      if (obj is TableDataSourceBaseImmutableOptions<TOptions, TData> from)
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
      return Activator.CreateInstance(this.GetType(), this);
    }

    #region IAltaxoTableDataSource

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
    public TData ProcessData
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
    public TOptions ProcessOptions
    {
      get
      {
        return _processOptions;
      }
      [MemberNotNull(nameof(_processOptions))]
      set
      {
        if (value is null)
          throw new ArgumentNullException(nameof(ProcessOptions));
        if (_processOptions is null || !object.Equals(_processOptions, value))
        {
          _processOptions = value;
          EhChildChanged(_processOptions, EventArgs.Empty);
        }
      }
    }

    object IAltaxoTableDataSource.ProcessOptionsObject
    {
      get => ProcessOptions;
      set => ProcessOptions = (TOptions)value;
    }

    object IAltaxoTableDataSource.ProcessDataObject
    {
      get => _processData;
      set => ProcessData = (TData)value;
    }

    #region Change event handling    

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (_processOptions is IDocumentLeafNode podln)
        yield return new Main.DocumentNodeAndName(podln, "ProcessOptions");

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
      if (_processData is IHasDocumentReferences pdr)
      {
        pdr.VisitDocumentReferences(ReportProxies);
      }
      if (_processOptions is IHasDocumentReferences por)
      {
        por.VisitDocumentReferences(ReportProxies);
      }
    }

    #endregion IAltaxoTableDataSource

  }
}
