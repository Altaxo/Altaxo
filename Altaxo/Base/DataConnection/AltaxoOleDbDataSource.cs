using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.DataConnection
{
	public class AltaxoOleDbDataSource : Altaxo.Data.IAltaxoTableDataSource
	{
		protected Data.IDataSourceImportOptions _importOptions;
		private OleDbDataQuery _dataQuery = OleDbDataQuery.Empty;

		public AltaxoOleDbDataSource(string selectionStatement, AltaxoOleDbConnectionString connectionString)
		{
			_importOptions = new Data.DataSourceImportOptions();
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
				CopyHelper.Copy(ref _importOptions, from._importOptions);
				CopyHelper.CopyImmutable(ref _dataQuery, from._dataQuery);
				return true;
			}
			return false;
		}

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

				s._dataQuery = (OleDbDataQuery)info.GetValue("DataQuery", s);
				s._importOptions = (Data.DataSourceImportOptions)info.GetValue("ImportOptions", s);
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
			var result = new AltaxoOleDbDataSource();
			result.CopyFrom(this);
			return result;
		}

		#region Properties

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
					StopDataSourceMonitoring();
					MayStartDataSourceMonitoring();
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
					MayStartDataSourceMonitoring();
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
			destinationTable.Suspend();
			try
			{
				var tableConnector = new AltaxoTableConnector(destinationTable);
				this._dataQuery.ReadDataFromOleDbConnection(tableConnector.ReadAction);
			}
			finally
			{
				destinationTable.Resume();
			}
		}

		private Action<Data.IAltaxoTableDataSource> _dataSourceChanged;

		/// <summary>
		/// Occurs when the data source has changed and the import trigger source is DataSourceChanged. The argument is the sender of this event.
		/// </summary>
		public event Action<Data.IAltaxoTableDataSource> DataSourceChanged
		{
			add
			{
				_dataSourceChanged += value;
				MayStartDataSourceMonitoring();
			}
			remove
			{
				_dataSourceChanged -= value;
				if (null == _dataSourceChanged)
					StopDataSourceMonitoring();
			}
		}

		public void OnAfterDeserialization()
		{
			if (_importOptions.ImportTriggerSource != Data.ImportTriggerSource.Manual)
			{
				var ev = _dataSourceChanged;
				if (null != ev)
					ev(this);
			}

			MayStartDataSourceMonitoring();
		}

		private System.Threading.Timer _timer;

		private void MayStartDataSourceMonitoring()
		{
			if (_importOptions.ImportTriggerSource != Data.ImportTriggerSource.DataSourceChanged)
			{
				StopDataSourceMonitoring();
				return;
			}

			if (_importOptions.ImportTriggerSource == Data.ImportTriggerSource.DataSourceChanged && null == _timer)
			{
				var interval = Math.Max(_importOptions.PollTimeIntervalInSeconds, _importOptions.MinimumTimeIntervalBetweenUpdatesInSeconds);
				if (!(interval > 0))
					interval = 60;
				if (!(interval <= int.MaxValue / 1000.0))
					interval = int.MaxValue / 1000;

				_timer = new System.Threading.Timer(EhTimer, null, 0, (int)(interval * 1000));
			}
		}

		private void EhTimer(object state)
		{
			var ev = _dataSourceChanged;
			if (null != ev)
				ev(this);
			else
				StopDataSourceMonitoring();
		}

		private void StopDataSourceMonitoring()
		{
			var timer = _timer;
			if (null != timer)
			{
				timer.Dispose();
				_timer = null;
			}
		}

		public void Dispose()
		{
			var timer = _timer;
			if (null != timer)
			{
				timer.Dispose();
				_timer = null;
			}
		}
	}
}