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

namespace Altaxo.Data
{
  public interface IDataSourceImportOptions : ICloneable, Altaxo.Main.IDocumentLeafNode
  {
    /// <summary>
    /// Gets a value indicating whether the data that are cached in the Altaxo table should be saved within the Altaxo project.
    /// </summary>
    /// <value>
    /// If <c>True</c>, the data of the table attached to this data source are not stored in the Altaxo project file.
    /// </value>
    bool DoNotSaveCachedTableData { get; }

    /// <summary>
    /// Gets a value indicating whether the table script is executed after importing data from this data source.
    /// </summary>
    /// <value>
    /// <c>true</c> if [execute table script after import]; otherwise, <c>false</c>.
    /// </value>
    bool ExecuteTableScriptAfterImport { get; }

    /// <summary>
    /// Gets the cause of a reread of the data source.
    /// </summary>
    /// <value>
    /// The cause of a reread of the data source.
    /// </value>
    ImportTriggerSource ImportTriggerSource { get; }

    /// <summary>
    /// Gets the minimum time interval between updates in seconds. This value is especially important if update notifications from the data source
    /// are fired too frequently. In this case the value limits the update frequency.
    /// </summary>
    /// <value>
    /// The minimum time interval between updates in seconds.
    /// </value>
    double MinimumWaitingTimeAfterUpdateInSeconds { get; }

    /// <summary>
    /// Gets the poll time interval in seconds. This value is needed if the data source does not support update notifications. In this case the table is automatically refreshed when the
    /// poll time interval elapsed.
    /// </summary>
    /// <value>
    /// The poll time interval in seconds.
    /// </value>
    double MaximumWaitingTimeAfterUpdateInSeconds { get; }

    /// <summary>
    /// MinimumWaitingTimeAfterFirstTrigger (default: Zero): designates the minimum time interval that should at least be waited after the first trigger (after an update) and the next update operation.
    /// </summary>
    double MinimumWaitingTimeAfterFirstTriggerInSeconds { get; }

    /// <summary>
    /// MaximumWaitingTimeAfterFirstTrigger (default: Infinity): designates the maximum waiting time after the first trigger (after an update) occured. If this time has elapsed, a new update operation will be executed.
    /// </summary>
    double MaximumWaitingTimeAfterFirstTriggerInSeconds { get; }

    /// <summary>
    /// MinimumWaitingTimeAfterLastTrigger (default: Zero): designates the time interval that at least should be waited between the latest occured trigger (after an update) and the next update operation.
    /// </summary>
    /// <exception cref="System.ArgumentOutOfRangeException">MinimumWaitingTimeAfterLastTrigger must not be negative!</exception>
    double MinimumWaitingTimeAfterLastTriggerInSeconds { get; }
  }

  public class DataSourceImportOptions
    :
    Main.SuspendableDocumentLeafNodeWithEventArgs,
    IDataSourceImportOptions,
    Main.ICopyFrom
  {
    protected ImportTriggerSource _importTriggerSource;
    protected bool _doNotSaveCachedTableData;
    protected bool _executeTableScriptAfterImport;
    protected double _minimumWaitingTimeAfterUpdate = 60;
    protected double _maximumWaitingTimeAfterUpdate = 60;

    protected double _minimumWaitingTimeAfterFirstTrigger = 0;
    protected double _maximumWaitingTimeAfterFirstTrigger = double.PositiveInfinity;
    protected double _minimumWaitingTimeAfterLastTrigger = 0;

    public virtual bool CopyFrom(object obj)
    {
      if (object.ReferenceEquals(this, obj))
        return true;

      var from = obj as DataSourceImportOptions;
      if (null != from)
      {
        _importTriggerSource = from._importTriggerSource;
        _doNotSaveCachedTableData = from._doNotSaveCachedTableData;
        _executeTableScriptAfterImport = from._executeTableScriptAfterImport;
        _minimumWaitingTimeAfterUpdate = from._minimumWaitingTimeAfterUpdate;
        _maximumWaitingTimeAfterUpdate = from._maximumWaitingTimeAfterUpdate;
        _minimumWaitingTimeAfterFirstTrigger = from._minimumWaitingTimeAfterFirstTrigger;
        _maximumWaitingTimeAfterFirstTrigger = from._maximumWaitingTimeAfterFirstTrigger;
        _minimumWaitingTimeAfterLastTrigger = from._minimumWaitingTimeAfterLastTrigger;

        EhSelfChanged(EventArgs.Empty);

        return true;
      }

      return false;
    }

    #region Serialization

    #region Version 0

    /// <summary>
    /// 2014-06-13 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DataSourceImportOptions), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (DataSourceImportOptions)obj;

        info.AddEnum("ImportTriggerSource", s._importTriggerSource);
        info.AddValue("ExecuteTableScriptAfterImport", s._executeTableScriptAfterImport);
        info.AddValue("DoNotSaveCachedTableData", s._doNotSaveCachedTableData);
        info.AddValue("MinimumTimeIntervalBetweenUpdatesInSeconds", s._minimumWaitingTimeAfterUpdate);
        info.AddValue("PollTimeIntervalInSeconds", s._maximumWaitingTimeAfterUpdate);
      }

      protected virtual DataSourceImportOptions SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        var s = (o == null ? new DataSourceImportOptions() : (DataSourceImportOptions)o);

        s._importTriggerSource = (Data.ImportTriggerSource)info.GetEnum("ImportTriggerSource", s._importTriggerSource.GetType());
        s._executeTableScriptAfterImport = info.GetBoolean("ExecuteTableScriptAfterImport");
        s._doNotSaveCachedTableData = info.GetBoolean("DoNotSaveCachedTableData");
        s._minimumWaitingTimeAfterUpdate = info.GetDouble("MinimumTimeIntervalBetweenUpdatesInSeconds");
        s._maximumWaitingTimeAfterUpdate = info.GetDouble("PollTimeIntervalInSeconds");
        return s;
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        var s = SDeserialize(o, info, parent);
        return s;
      }
    }

    #endregion Version 0

    #region Version 1

    /// <summary>
    /// 2014-08-07 added MinimumWaitingTimeAfterFirstTrigger,  MaximumWaitingTimeAfterFirstTrigger, MinimumWaitingTimeAfterLastTrigger
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(DataSourceImportOptions), 1)]
    private class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (DataSourceImportOptions)obj;

        info.AddEnum("ImportTriggerSource", s._importTriggerSource);
        info.AddValue("ExecuteTableScriptAfterImport", s._executeTableScriptAfterImport);
        info.AddValue("DoNotSaveCachedTableData", s._doNotSaveCachedTableData);
        info.AddValue("MinimumWaitingTimeAfterUpdate", s._minimumWaitingTimeAfterUpdate);
        info.AddValue("MaximumWaitingTimeAfterUpdate", s._maximumWaitingTimeAfterUpdate);
        info.AddValue("MinimumWaitingTimeAfterFirstTrigger", s._minimumWaitingTimeAfterFirstTrigger);
        info.AddValue("MaximumWaitingTimeAfterFirstTrigger", s._maximumWaitingTimeAfterFirstTrigger);
        info.AddValue("MinimumWaitingTimeAfterLastTrigger", s._minimumWaitingTimeAfterLastTrigger);
      }

      protected virtual DataSourceImportOptions SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        var s = (o == null ? new DataSourceImportOptions() : (DataSourceImportOptions)o);

        s._importTriggerSource = (Data.ImportTriggerSource)info.GetEnum("ImportTriggerSource", s._importTriggerSource.GetType());
        s._executeTableScriptAfterImport = info.GetBoolean("ExecuteTableScriptAfterImport");
        s._doNotSaveCachedTableData = info.GetBoolean("DoNotSaveCachedTableData");
        s._minimumWaitingTimeAfterUpdate = info.GetDouble("MinimumWaitingTimeAfterUpdate");
        s._maximumWaitingTimeAfterUpdate = info.GetDouble("MaximumWaitingTimeAfterUpdate");
        s._minimumWaitingTimeAfterFirstTrigger = info.GetDouble("MinimumWaitingTimeAfterFirstTrigger");
        s._maximumWaitingTimeAfterFirstTrigger = info.GetDouble("MaximumWaitingTimeAfterFirstTrigger");
        s._minimumWaitingTimeAfterLastTrigger = info.GetDouble("MinimumWaitingTimeAfterLastTrigger");

        return s;
      }

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        var s = SDeserialize(o, info, parent);
        return s;
      }
    }

    #endregion Version 1

    #endregion Serialization

    /// <summary>
    /// Clones this instance.
    /// </summary>
    /// <returns>A clone of this instance.</returns>
    public object Clone()
    {
      var result = MemberwiseClone();
      return result;
    }

    /// <summary>
    /// Gets a value indicating whether the data that are cached in the Altaxo table should be saved within the Altaxo project.
    /// </summary>
    /// <value>
    /// If <c>True</c>, the data of the table attached to this data source are not stored in the Altaxo project file.
    /// </value>
    public bool DoNotSaveCachedTableData
    {
      get { return _doNotSaveCachedTableData; }
      set { SetMemberAndRaiseSelfChanged(ref _doNotSaveCachedTableData, value); }
    }

    /// <summary>
    /// Gets the cause of a reread of the data source.
    /// </summary>
    /// <value>
    /// The cause of a reread of the data source.
    /// </value>
    public Data.ImportTriggerSource ImportTriggerSource
    {
      get { return _importTriggerSource; }
      set { SetMemberEnumAndRaiseSelfChanged(ref _importTriggerSource, value); }
    }

    /// <summary>
    /// Gets a value indicating whether the table script is executed after importing data from this data source.
    /// </summary>
    /// <value>
    /// <c>true</c> if [execute table script after import]; otherwise, <c>false</c>.
    /// </value>
    public bool ExecuteTableScriptAfterImport
    {
      get { return _executeTableScriptAfterImport; }
      set { SetMemberAndRaiseSelfChanged(ref _executeTableScriptAfterImport, value); }
    }

    /// <summary>
    /// Gets the minimum time interval between updates in seconds. This value is especially important if update notifications from the data source
    /// are fired too frequently. In this case the value limits the update frequency.
    /// </summary>
    /// <value>
    /// The minimum time interval between updates in seconds.
    /// </value>
    /// <exception cref="System.ArgumentOutOfRangeException">MinimumTimeIntervalBetweenUpdatesInSeconds must be a value >= 0</exception>
    public double MinimumWaitingTimeAfterUpdateInSeconds
    {
      get { return _minimumWaitingTimeAfterUpdate; }
      set
      {
        if (!(value >= 0))
          throw new ArgumentOutOfRangeException("MinimumTimeIntervalBetweenUpdatesInSeconds must be a value >= 0");

        SetMemberAndRaiseSelfChanged(ref _minimumWaitingTimeAfterUpdate, value);
      }
    }

    /// <summary>
    /// Gets the poll time interval in seconds. This value is needed if the data source does not support update notifications. In this case the table is automatically refreshed when the
    /// poll time interval elapsed.
    /// </summary>
    /// <value>
    /// The poll time interval in seconds.
    /// </value>
    /// <exception cref="System.ArgumentOutOfRangeException">PollTimeIntervalInSeconds must be a value > 0</exception>
    public double MaximumWaitingTimeAfterUpdateInSeconds
    {
      get
      {
        return _maximumWaitingTimeAfterUpdate;
      }
      set
      {
        if (!(value >= 0))
          throw new ArgumentOutOfRangeException("MaximumWaitingTimeAfterUpdateInSeconds must be a value >= 0");

        SetMemberAndRaiseSelfChanged(ref _maximumWaitingTimeAfterUpdate, value);
      }
    }

    /// <summary>
    /// MinimumWaitingTimeAfterFirstTrigger (default: Zero): designates the minimum time interval that should at least be waited after the first trigger (after an update) and the next update operation.
    /// </summary>
    /// <exception cref="System.ArgumentOutOfRangeException">MinimumWaitingTimeAfterFirstTrigger must not be negative!</exception>
    public double MinimumWaitingTimeAfterFirstTriggerInSeconds
    {
      get
      {
        return _minimumWaitingTimeAfterFirstTrigger;
      }
      set
      {
        if (!(value >= 0))
          throw new ArgumentOutOfRangeException("MinimumWaitingTimeAfterFirstTrigger must not be negative!");

        SetMemberAndRaiseSelfChanged(ref _minimumWaitingTimeAfterFirstTrigger, value);
      }
    }

    /// <summary>
    /// MaximumWaitingTimeAfterFirstTrigger (default: Infinity): designates the maximum waiting time after the first trigger (after an update) occured. If this time has elapsed, a new update operation will be executed.
    /// </summary>
    /// <exception cref="System.ArgumentOutOfRangeException">MaximumWaitingTimeAfterFirstTrigger must not be negative!</exception>
    public double MaximumWaitingTimeAfterFirstTriggerInSeconds
    {
      get
      {
        return _maximumWaitingTimeAfterFirstTrigger;
      }
      set
      {
        if (!(value >= 0))
          throw new ArgumentOutOfRangeException("MaximumWaitingTimeAfterFirstTrigger must not be negative!");

        SetMemberAndRaiseSelfChanged(ref _maximumWaitingTimeAfterFirstTrigger, value);
      }
    }

    /// <summary>
    /// MinimumWaitingTimeAfterLastTrigger (default: Zero): designates the time interval that at least should be waited between the latest occured trigger (after an update) and the next update operation.
    /// </summary>
    /// <exception cref="System.ArgumentOutOfRangeException">MinimumWaitingTimeAfterLastTrigger must not be negative!</exception>
    public double MinimumWaitingTimeAfterLastTriggerInSeconds
    {
      get
      {
        return _minimumWaitingTimeAfterLastTrigger;
      }
      set
      {
        if (!(value >= 0))
          throw new ArgumentOutOfRangeException("MinimumWaitingTimeAfterLastTrigger must not be negative!");

        SetMemberAndRaiseSelfChanged(ref _minimumWaitingTimeAfterLastTrigger, value);
      }
    }
  }
}
