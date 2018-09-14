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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Altaxo.Data;

namespace Altaxo.DataConnection
{
  public class AltaxoOleDbDataSource : TableDataSourceBase, Altaxo.Data.IAltaxoTableDataSource
  {
    protected Data.IDataSourceImportOptions _importOptions;
    private OleDbDataQuery _dataQuery = OleDbDataQuery.Empty;

    protected Altaxo.Main.TriggerBasedUpdate _triggerBasedUpdate;

    private int _updateReentrancyCount;

    /// <summary>Indicates that serialization of the whole AltaxoDocument (!) is still in progress. Data sources should not be updated during serialization.</summary>
    [NonSerialized]
    protected bool _isDeserializationInProgress;

    #region Construction

    public AltaxoOleDbDataSource(string selectionStatement, AltaxoOleDbConnectionString connectionString)
    {
      _importOptions = new Data.DataSourceImportOptions() { ParentObject = this };
      _dataQuery = new OleDbDataQuery(selectionStatement, connectionString);
    }

    protected AltaxoOleDbDataSource()
    {
    }

    public virtual bool CopyFrom(object obj)
    {
      if (object.ReferenceEquals(this, obj))
        return true;

      var from = obj as AltaxoOleDbDataSource;
      if (null != from)
      {
        ChildCopyToMember(ref _importOptions, from._importOptions);
        CopyHelper.CopyImmutable(ref _dataQuery, from._dataQuery);
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
      var result = new AltaxoOleDbDataSource();
      result.CopyFrom(this);
      return result;
    }

    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (null != _importOptions)
        yield return new Main.DocumentNodeAndName(_importOptions, () => _importOptions = null, "ImportOptions");
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

      protected virtual AltaxoOleDbDataSource SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        var s = (o == null ? new AltaxoOleDbDataSource() : (AltaxoOleDbDataSource)o);

        s._isDeserializationInProgress = true;
        s._dataQuery = (OleDbDataQuery)info.GetValue("DataQuery", s);
        s.ChildSetMember(ref s._importOptions, (Data.DataSourceImportOptions)info.GetValue("ImportOptions", s));

        info.AfterDeserializationHasCompletelyFinished += s.EhAfterDeserializationHasCompletelyFinished;
        return s;
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        var s = SDeserialize(o, info, parent);
        return s;
      }
    }

    #endregion Version 0

    #endregion Serialization

    #region Properties

    public override Main.IDocumentNode ParentObject
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
        if (null == value)
          throw new ArgumentNullException("DataQuery");

        var oldValue = _dataQuery;
        _dataQuery = value;

        if (!object.Equals(oldValue, value))
        {
          UpdateWatching();
        }
      }
    }

    public Data.IDataSourceImportOptions ImportOptions
    {
      get
      {
        return _importOptions;
      }
      set
      {
        if (null == value)
          throw new ArgumentNullException("ImportOptions");

        var oldValue = _importOptions;

        _importOptions = value;

        if (!object.Equals(oldValue, value))
        {
          UpdateWatching();
        }
      }
    }

    #endregion Properties

    /// <summary>
    /// Fills (or refills) the data. The data source is represented by this instance, the destination table is provided in the argument <paramref name="destinationTable" />.
    /// </summary>
    /// <param name="destinationTable">The destination table.</param>
    public void FillData(Data.DataTable destinationTable)
    {
      if (null == destinationTable)
        throw new ArgumentNullException("destinationTable");

      int reentrancyCount = Interlocked.Increment(ref _updateReentrancyCount);
      if (1 == reentrancyCount)
      {
        try
        {
          using (var token = destinationTable.SuspendGetToken())
          {
            var tableConnector = new AltaxoTableConnector(destinationTable);
            _dataQuery.ReadDataFromOleDbConnection(tableConnector.ReadAction);
          }
        }
        finally
        {
          Interlocked.Decrement(ref _updateReentrancyCount);
        }
      }
    }

    public void OnAfterDeserialization()
    {
      // Note: it is not neccessary to call UpdateWatching here; UpdateWatching is called when the table connects to this data source via subscription to the DataSourceChanged event
    }

    private void EhUpdateByTimerQueue()
    {
      if (null != _parent)
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
      if (null == _triggerBasedUpdate)
        UpdateWatching(); // Compromise - we update only if the watch is off
    }

    public void UpdateWatching()
    {
      SwitchOffWatching();

      if (_isDeserializationInProgress)
        return; // in serialization process - wait until serialization has finished

      if (IsSuspended)
        return; // in update operation - wait until finished

      if (null == _parent)
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
      IDisposable disp;

      disp = _triggerBasedUpdate;
      _triggerBasedUpdate = null;
      if (null != disp)
        disp.Dispose();
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
