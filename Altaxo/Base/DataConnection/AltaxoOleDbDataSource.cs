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

using Altaxo.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Altaxo.DataConnection
{
	public class AltaxoOleDbDataSource : DisposableBase, Altaxo.Data.IAltaxoTableDataSource
	{
		protected Data.IDataSourceImportOptions _importOptions;
		private OleDbDataQuery _dataQuery = OleDbDataQuery.Empty;
		private int _updateReentrancyCount;

		protected Main.EventSuppressor _eventSuppressor;

		protected Altaxo.Main.TriggerBasedUpdate _triggerBasedUpdate;

		private bool _isDisposed;

		private Action<Data.IAltaxoTableDataSource> _dataSourceChanged;

		#region Construction

		public AltaxoOleDbDataSource(string selectionStatement, AltaxoOleDbConnectionString connectionString)
		{
			_eventSuppressor = new Main.EventSuppressor(EhResumeSuppressedEvents);
			_importOptions = new Data.DataSourceImportOptions();
			_dataQuery = new OleDbDataQuery(selectionStatement, connectionString);
		}

		protected AltaxoOleDbDataSource()
		{
			_eventSuppressor = new Main.EventSuppressor(EhResumeSuppressedEvents);
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

		#region Properties

		/// <summary>
		/// Occurs when the data source has changed and the import trigger source is DataSourceChanged. The argument is the sender of this event.
		/// </summary>
		public event Action<Data.IAltaxoTableDataSource> DataSourceChanged
		{
			add
			{
				bool isFirst = null == _dataSourceChanged;
				_dataSourceChanged += value;
				if (isFirst)
					UpdateWatching();
			}
			remove
			{
				_dataSourceChanged -= value;
				bool isLast = null == _dataSourceChanged;
				if (isLast)
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
				destinationTable.Suspend();
				try
				{
					var tableConnector = new AltaxoTableConnector(destinationTable);
					this._dataQuery.ReadDataFromOleDbConnection(tableConnector.ReadAction);
				}
				finally
				{
					destinationTable.Resume();
					Interlocked.Decrement(ref _updateReentrancyCount);
				}
			}
		}

		public void OnAfterDeserialization()
		{
			// Note: it is not neccessary to call UpdateWatching here; UpdateWatching is called when the table connects to this data source via subscription to the DataSourceChanged event
		}

		/// <summary>
		/// Suppresses the events by getting a token. When the token is disposed, events will be resumed again.
		/// </summary>
		/// <returns>Suppress token.</returns>
		public Main.ISuppressToken SuppressEventsGettingToken()
		{
			return _eventSuppressor.Suspend();
		}

		/// <summary>
		/// Resumes the events.
		/// </summary>
		/// <param name="token">The suppress token.</param>
		public void ResumeEvents(ref Main.ISuppressToken token)
		{
			_eventSuppressor.Resume(ref token);
		}

		protected virtual void OnDataSourceChanged()
		{
			var ev = _dataSourceChanged;
			if (null != ev)
				ev(this);
		}

		private void EhUpdateByTimerQueue()
		{
			var ev = _dataSourceChanged;
			if (null != ev)
				ev(this);
			else
				SwitchOffWatching();
		}

		private void EhResumeSuppressedEvents()
		{
			var ev = _dataSourceChanged;
			if (null != ev)
				ev(this);

			UpdateWatching();
		}

		public void UpdateWatching()
		{
			SwitchOffWatching();

			if (_eventSuppressor.PeekDisabled)
				return; // in update operation - wait until finished

			if (null == _dataSourceChanged)
				return; // No listener - no need to watch

			if (_importOptions.ImportTriggerSource != ImportTriggerSource.DataSourceChanged)
				return; // DataSource is updated manually

			SwitchOnWatching();
		}

		private void SwitchOnWatching()
		{
			_triggerBasedUpdate = new Main.TriggerBasedUpdate(Current.TimerQueue);
			_triggerBasedUpdate.MinimumWaitingTimeAfterUpdate = TimeSpanExtensions.FromSecondsAccurate(_importOptions.MinimumWaitingTimeAfterUpdateInSeconds);
			_triggerBasedUpdate.MaximumWaitingTimeAfterUpdate = TimeSpanExtensions.FromSecondsAccurate(Math.Max(_importOptions.MinimumWaitingTimeAfterUpdateInSeconds, _importOptions.MaximumWaitingTimeAfterUpdateInSeconds));
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
			if (!_isDisposed)
			{
				_isDisposed = true;

				_dataSourceChanged = null;

				SwitchOffWatching();
			}
		}

		public void VisitDocumentReferences(Main.DocNodeProxyReporter ReportProxies)
		{
		}
	}
}