using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Data
{
	public interface IDataSourceImportOptions : ICloneable
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
		double MinimumTimeIntervalBetweenUpdatesInSeconds { get; }

		/// <summary>
		/// Gets the poll time interval in seconds. This value is needed if the data source does not support update notifications. In this case the table is automatically refreshed when the
		/// poll time interval elapsed.
		/// </summary>
		/// <value>
		/// The poll time interval in seconds.
		/// </value>
		double PollTimeIntervalInSeconds { get; }
	}

	public class DataSourceImportOptions : IDataSourceImportOptions, Main.ICopyFrom
	{
		protected ImportTriggerSource _importTriggerSource;
		protected bool _doNotSaveCachedTableData;
		protected bool _executeTableScriptAfterImport;
		protected double _minimumTimeIntervalBetweenUpdatesInSeconds = 60;
		protected double _pollTimeIntervalInSeconds = 60;

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
				_minimumTimeIntervalBetweenUpdatesInSeconds = from._minimumTimeIntervalBetweenUpdatesInSeconds;
				_pollTimeIntervalInSeconds = from._pollTimeIntervalInSeconds;
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
				info.AddValue("MinimumTimeIntervalBetweenUpdatesInSeconds", s._minimumTimeIntervalBetweenUpdatesInSeconds);
				info.AddValue("PollTimeIntervalInSeconds", s._pollTimeIntervalInSeconds);
			}

			protected virtual DataSourceImportOptions SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = (o == null ? new DataSourceImportOptions() : (DataSourceImportOptions)o);

				s._importTriggerSource = (Data.ImportTriggerSource)info.GetEnum("ImportTriggerSource", s._importTriggerSource.GetType());
				s._executeTableScriptAfterImport = info.GetBoolean("ExecuteTableScriptAfterImport");
				s._doNotSaveCachedTableData = info.GetBoolean("DoNotSaveCachedTableData");
				s._minimumTimeIntervalBetweenUpdatesInSeconds = info.GetDouble("MinimumTimeIntervalBetweenUpdatesInSeconds");
				s._pollTimeIntervalInSeconds = info.GetDouble("PollTimeIntervalInSeconds");
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

		/// <summary>
		/// Clones this instance.
		/// </summary>
		/// <returns>A clone of this instance.</returns>
		public object Clone()
		{
			var result = this.MemberwiseClone();
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
			set { _doNotSaveCachedTableData = value; }
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
			set { _importTriggerSource = value; }
		}

		/// <summary>
		/// Occurs when the data source has changed and the import trigger source is DataSourceChanged. The argument is the sender of this event.
		/// </summary>
		public event Action<Data.IAltaxoTableDataSource> DataSourceChanged;

		/// <summary>
		/// Gets a value indicating whether the table script is executed after importing data from this data source.
		/// </summary>
		/// <value>
		/// <c>true</c> if [execute table script after import]; otherwise, <c>false</c>.
		/// </value>
		public bool ExecuteTableScriptAfterImport
		{
			get { return _executeTableScriptAfterImport; }
			set { _executeTableScriptAfterImport = value; }
		}

		public double MinimumTimeIntervalBetweenUpdatesInSeconds
		{
			get { return _minimumTimeIntervalBetweenUpdatesInSeconds; }
			set
			{
				if (!(value >= 0))
					throw new ArgumentOutOfRangeException("MinimumTimeIntervalBetweenUpdatesInSeconds must be a value >= 0");

				_minimumTimeIntervalBetweenUpdatesInSeconds = value;
			}
		}

		public double PollTimeIntervalInSeconds
		{
			get
			{
				return _pollTimeIntervalInSeconds;
			}
			set
			{
				if (!(value > 0))
					throw new ArgumentOutOfRangeException("PollTimeIntervalInSeconds must be a value > 0");

				_pollTimeIntervalInSeconds = value;
			}
		}
	}
}