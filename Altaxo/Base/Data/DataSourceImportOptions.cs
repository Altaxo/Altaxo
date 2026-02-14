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

namespace Altaxo.Data
{
  public interface IDataSourceImportOptions : Main.IImmutable
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

  public record DataSourceImportOptions : IDataSourceImportOptions, Main.IImmutable
  {
    #region Serialization

    #region Version 0

    /// <summary>
    /// 2014-06-13 initial version.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Data.DataSourceImportOptions", 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (DataSourceImportOptions)obj;

        info.AddEnum("ImportTriggerSource", s.ImportTriggerSource);
        info.AddValue("ExecuteTableScriptAfterImport", s.ExecuteTableScriptAfterImport);
        info.AddValue("DoNotSaveCachedTableData", s.DoNotSaveCachedTableData);
        info.AddValue("MinimumTimeIntervalBetweenUpdatesInSeconds", s.MinimumWaitingTimeAfterUpdateInSeconds);
        info.AddValue("PollTimeIntervalInSeconds", s.MaximumWaitingTimeAfterUpdateInSeconds);
      }

      protected virtual DataSourceImportOptions SDeserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var importTriggerSource = info.GetEnum<Data.ImportTriggerSource>("ImportTriggerSource");
        var executeTableScriptAfterImport = info.GetBoolean("ExecuteTableScriptAfterImport");
        var doNotSaveCachedTableData = info.GetBoolean("DoNotSaveCachedTableData");
        var minimumWaitingTimeAfterUpdate = info.GetDouble("MinimumTimeIntervalBetweenUpdatesInSeconds");
        var maximumWaitingTimeAfterUpdate = info.GetDouble("PollTimeIntervalInSeconds");

        return (o as DataSourceImportOptions ?? new DataSourceImportOptions()) with
        {
          ImportTriggerSource = importTriggerSource,
          ExecuteTableScriptAfterImport = executeTableScriptAfterImport,
          DoNotSaveCachedTableData = doNotSaveCachedTableData,
          MinimumWaitingTimeAfterUpdateInSeconds = minimumWaitingTimeAfterUpdate,
          MaximumWaitingTimeAfterUpdateInSeconds = maximumWaitingTimeAfterUpdate
        };
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
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

        info.AddEnum("ImportTriggerSource", s.ImportTriggerSource);
        info.AddValue("ExecuteTableScriptAfterImport", s.ExecuteTableScriptAfterImport);
        info.AddValue("DoNotSaveCachedTableData", s.DoNotSaveCachedTableData);
        info.AddValue("MinimumWaitingTimeAfterUpdate", s.MinimumWaitingTimeAfterUpdateInSeconds);
        info.AddValue("MaximumWaitingTimeAfterUpdate", s.MaximumWaitingTimeAfterUpdateInSeconds);
        info.AddValue("MinimumWaitingTimeAfterFirstTrigger", s.MinimumWaitingTimeAfterFirstTriggerInSeconds);
        info.AddValue("MaximumWaitingTimeAfterFirstTrigger", s.MaximumWaitingTimeAfterFirstTriggerInSeconds);
        info.AddValue("MinimumWaitingTimeAfterLastTrigger", s.MinimumWaitingTimeAfterLastTriggerInSeconds);
      }

      protected virtual DataSourceImportOptions SDeserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {

        var importTriggerSource = info.GetEnum<Data.ImportTriggerSource>("ImportTriggerSource");
        var executeTableScriptAfterImport = info.GetBoolean("ExecuteTableScriptAfterImport");
        var doNotSaveCachedTableData = info.GetBoolean("DoNotSaveCachedTableData");
        var minimumWaitingTimeAfterUpdate = info.GetDouble("MinimumWaitingTimeAfterUpdate");
        var maximumWaitingTimeAfterUpdate = info.GetDouble("MaximumWaitingTimeAfterUpdate");
        var minimumWaitingTimeAfterFirstTrigger = info.GetDouble("MinimumWaitingTimeAfterFirstTrigger");
        var maximumWaitingTimeAfterFirstTrigger = info.GetDouble("MaximumWaitingTimeAfterFirstTrigger");
        var minimumWaitingTimeAfterLastTrigger = info.GetDouble("MinimumWaitingTimeAfterLastTrigger");

        return (o as DataSourceImportOptions ?? new DataSourceImportOptions()) with
        {
          ImportTriggerSource = importTriggerSource,
          ExecuteTableScriptAfterImport = executeTableScriptAfterImport,
          DoNotSaveCachedTableData = doNotSaveCachedTableData,
          MinimumWaitingTimeAfterUpdateInSeconds = minimumWaitingTimeAfterUpdate,
          MaximumWaitingTimeAfterUpdateInSeconds = maximumWaitingTimeAfterUpdate,
          MinimumWaitingTimeAfterFirstTriggerInSeconds = minimumWaitingTimeAfterFirstTrigger,
          MaximumWaitingTimeAfterFirstTriggerInSeconds = maximumWaitingTimeAfterFirstTrigger,
          MinimumWaitingTimeAfterLastTriggerInSeconds = minimumWaitingTimeAfterLastTrigger
        };
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = SDeserialize(o, info, parent);
        return s;
      }
    }

    #endregion Version 1

    #endregion Serialization



    /// <summary>
    /// Gets a value indicating whether the data that are cached in the Altaxo table should be saved within the Altaxo project.
    /// </summary>
    /// <value>
    /// If <c>True</c>, the data of the table attached to this data source are not stored in the Altaxo project file.
    /// </value>
    public bool DoNotSaveCachedTableData { get; init; }

    /// <summary>
    /// Gets the cause of a reread of the data source.
    /// </summary>
    /// <value>
    /// The cause of a reread of the data source.
    /// </value>
    public Data.ImportTriggerSource ImportTriggerSource { get; init; }


    /// <summary>
    /// Gets a value indicating whether the table script is executed after importing data from this data source.
    /// </summary>
    /// <value>
    /// <c>true</c> if [execute table script after import]; otherwise, <c>false</c>.
    /// </value>
    public bool ExecuteTableScriptAfterImport { get; init; }


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
      get { return field; }
      init
      {
        if (!(value >= 0))
          throw new ArgumentOutOfRangeException("MinimumTimeIntervalBetweenUpdatesInSeconds must be a value >= 0");

        field = value;
      }
    } = 60;

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
        return field;
      }
      init
      {
        if (!(value >= 0))
          throw new ArgumentOutOfRangeException("MaximumWaitingTimeAfterUpdateInSeconds must be a value >= 0");

        field = value;
      }
    } = 60;

    /// <summary>
    /// MinimumWaitingTimeAfterFirstTrigger (default: Zero): designates the minimum time interval that should at least be waited after the first trigger (after an update) and the next update operation.
    /// </summary>
    /// <exception cref="System.ArgumentOutOfRangeException">MinimumWaitingTimeAfterFirstTrigger must not be negative!</exception>
    public double MinimumWaitingTimeAfterFirstTriggerInSeconds
    {
      get
      {
        return field;
      }
      init
      {
        if (!(value >= 0))
          throw new ArgumentOutOfRangeException("MinimumWaitingTimeAfterFirstTrigger must not be negative!");

        field = value;
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
        return field;
      }
      init
      {
        if (!(value >= 0))
          throw new ArgumentOutOfRangeException("MaximumWaitingTimeAfterFirstTrigger must not be negative!");

        field = value;
      }
    } = double.PositiveInfinity;

    /// <summary>
    /// MinimumWaitingTimeAfterLastTrigger (default: Zero): designates the time interval that at least should be waited between the latest occured trigger (after an update) and the next update operation.
    /// </summary>
    /// <exception cref="System.ArgumentOutOfRangeException">MinimumWaitingTimeAfterLastTrigger must not be negative!</exception>
    public double MinimumWaitingTimeAfterLastTriggerInSeconds
    {
      get
      {
        return field;
      }
      init
      {
        if (!(value >= 0))
          throw new ArgumentOutOfRangeException("MinimumWaitingTimeAfterLastTrigger must not be negative!");

        field = value;
      }
    }
  }
}
