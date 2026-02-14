#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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
using Altaxo.Data;

namespace Altaxo.DataConnection
{
  public class AltaxoOleDbDataSource : TableDataSourceBase, Altaxo.Data.IAltaxoTableDataSource
  {
    protected Data.IDataSourceImportOptions _importOptions;
    private OleDbDataQuery _dataQuery = OleDbDataQuery.Empty;

    protected Altaxo.Main.TriggerBasedUpdate? _triggerBasedUpdate;

    /// <summary>Indicates that serialization of the whole AltaxoDocument (!) is still in progress. Data sources should not be updated during serialization.</summary>
    [NonSerialized]
    protected bool _isDeserializationInProgress;

    #region Construction

    public AltaxoOleDbDataSource(string selectionStatement, AltaxoOleDbConnectionString connectionString)
    {
      _importOptions = new Data.DataSourceImportOptions();
      _dataQuery = new OleDbDataQuery(selectionStatement, connectionString);
    }

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    protected AltaxoOleDbDataSource(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    {
    }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

    protected AltaxoOleDbDataSource(AltaxoOleDbDataSource from)
    {
      CopyFrom(from);
    }

    [MemberNotNull(nameof(_dataQuery), nameof(_importOptions))]
    public virtual void CopyFrom(AltaxoOleDbDataSource from)
    {
      if (ReferenceEquals(this, from))
#pragma warning disable CS8774 // Member must have a non-null value when exiting.
        return;
#pragma warning restore CS8774 // Member must have a non-null value when exiting.

      _importOptions = from._importOptions;
      CopyHelper.CopyImmutable<OleDbDataQuery>(ref _dataQuery, from._dataQuery);
    }

    public virtual bool CopyFrom(object obj)
    {
      if (ReferenceEquals(this, obj))
        return true;

      if (obj is AltaxoOleDbDataSource from)
      {
        CopyFrom(from);
        EhSelfChanged(EventArgs.Empty);
        return true;
      }
      return false;
    }

    /// <summary>
    /// Clones this instance.
    /// </summary>
    /// <returns>A clone of this instance.</returns>
    public object Clone()
    {
      return new AltaxoOleDbDataSource(this);
    }

    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      yield break;
    }

    #endregion Construction

    #region Serialization

    #region Version 0

    /// <summary>
    /// 2014-06-13 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AltaxoOleDbDataSource), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (AltaxoOleDbDataSource)obj;

        info.AddValue("DataQuery", s._dataQuery);
        info.AddValue("ImportOptions", s._importOptions);
      }

      protected virtual AltaxoOleDbDataSource SDeserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (o is null ? new AltaxoOleDbDataSource(info) : (AltaxoOleDbDataSource)o);

        s._isDeserializationInProgress = true;
        s._dataQuery = (OleDbDataQuery)info.GetValue("DataQuery", s);
        s._importOptions = info.GetValue<DataSourceImportOptions>("ImportOptions", this);

        info.AfterDeserializationHasCompletelyFinished += s.EhAfterDeserializationHasCompletelyFinished;
        return s;
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = SDeserialize(o, info, parent);
        return s;
      }
    }

    #endregion Version 0

    #endregion Serialization

    #region Properties

    public override Main.IDocumentNode? ParentObject
    {
      get
      {
        return base.ParentObject;
      }
      set
      {
        base.ParentObject = value;
        UpdateWatching();
      }
    }

    public OleDbDataQuery DataQuery
    {
      get
      {
        return _dataQuery;
      }
      set
      {
        if (value is null)
          throw new ArgumentNullException(nameof(DataQuery));

        var oldValue = _dataQuery;
        _dataQuery = value;

        if (!object.Equals(oldValue, value))
        {
          UpdateWatching();
        }
      }
    }

    public override Data.IDataSourceImportOptions ImportOptions
    {
      get
      {
        return _importOptions;
      }
      set
      {
        if (value is null)
          throw new ArgumentNullException(nameof(ImportOptions));

        var oldValue = _importOptions;

        _importOptions = value;

        if (!object.Equals(oldValue, value))
        {
          UpdateWatching();
        }
      }
    }

    object IAltaxoTableDataSource.ProcessOptionsObject
    {
      get => DataQuery;
      set => DataQuery = (OleDbDataQuery)value;
    }

    object IAltaxoTableDataSource.ProcessDataObject
    {
      get => null;
      set { }
    }

    #endregion Properties

    /// <summary>
    /// Fills (or refills) the data. The data source is represented by this instance, the destination table is provided in the argument <paramref name="destinationTable" />.
    /// </summary>
    /// <param name="destinationTable">The destination table.</param>
    /// <param name="reporter"></param>
    public override void FillData_Unchecked(Data.DataTable destinationTable, IProgressReporter reporter)
    {
      if (destinationTable is null)
      {
        throw new ArgumentNullException(nameof(destinationTable));
      }

      var tableConnector = new AltaxoTableConnector(destinationTable);
      _dataQuery.ReadDataFromOleDbConnection(tableConnector.ReadAction);
    }


    public void OnAfterDeserialization()
    {
      // Note: it is not neccessary to call UpdateWatching here; UpdateWatching is called when the table connects to this data source via subscription to the DataSourceChanged event
    }

    private void EhUpdateByTimerQueue()
    {
      if (_parent is not null)
      {
        if (!IsSuspended)
        {
          EhChildChanged(_dataQuery, TableDataSourceChangedEventArgs.Empty);
        }
      }
      else
        SwitchOffWatching();
    }

    protected override void OnResume(int eventCount)
    {
      base.OnResume(eventCount);

      // UpdateWatching should only be called if something concerning the watch (Times etc.) has changed during the suspend phase
      // Otherwise it will cause endless loops because UpdateWatching triggers immediatly an EhUpdateByTimerQueue event, which triggers an UpdateDataSource event, which leads to another Suspend and then Resume, which calls OnResume(). So the loop is closed.
      if (_triggerBasedUpdate is null)
        UpdateWatching(); // Compromise - we update only if the watch is off
    }

    public void UpdateWatching()
    {
      SwitchOffWatching();

      if (_isDeserializationInProgress)
        return; // in serialization process - wait until serialization has finished

      if (IsSuspended)
        return; // in update operation - wait until finished

      if (_parent is null)
        return; // No listener - no need to watch

      if (_importOptions.ImportTriggerSource != ImportTriggerSource.DataSourceChanged)
        return; // DataSource is updated manually

      SwitchOnWatching();
    }

    private void SwitchOnWatching()
    {
      _triggerBasedUpdate = new Main.TriggerBasedUpdate(Current.TimerQueue)
      {
        MinimumWaitingTimeAfterUpdate = TimeSpanExtensions.FromSecondsAccurate(_importOptions.MinimumWaitingTimeAfterUpdateInSeconds),
        MaximumWaitingTimeAfterUpdate = TimeSpanExtensions.FromSecondsAccurate(Math.Max(_importOptions.MinimumWaitingTimeAfterUpdateInSeconds, _importOptions.MaximumWaitingTimeAfterUpdateInSeconds))
      };
      _triggerBasedUpdate.UpdateAction += EhUpdateByTimerQueue;
    }

    private void SwitchOffWatching()
    {
      var disp = _triggerBasedUpdate;
      _triggerBasedUpdate = null;
      disp?.Dispose();
    }

    protected override void Dispose(bool disposing)
    {
      if (!IsDisposed)
      {
        SwitchOffWatching();
      }
      base.Dispose(disposing);
    }

    public void VisitDocumentReferences(Main.DocNodeProxyReporter ReportProxies)
    {
    }

    private void EhAfterDeserializationHasCompletelyFinished()
    {
      _isDeserializationInProgress = false;
      UpdateWatching();
    }
  }
}
