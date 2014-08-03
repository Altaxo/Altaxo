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

namespace Altaxo.Serialization.Ascii
{
	public class AsciiImportDataSource : DisposableBase, Altaxo.Data.IAltaxoTableDataSource
	{
		private IDataSourceImportOptions _importOptions;

		private AsciiImportOptions _asciiImportOptions;

		private AbsoluteAndRelativeFilePath _asciiFilePath;

		protected Action<IAltaxoTableDataSource> _dataSourceChanged;

		protected Main.EventSuppressor _eventSuppressor;

		protected bool _isDirty = false;

		protected System.IO.FileSystemWatcher _fileSystemWatcher;

		protected Altaxo.Main.TriggerBasedUpdate _triggerBasedUpdate;

		private bool _isDisposed;

		#region Serialization

		#region Version 0

		/// <summary>
		/// 2014-07-28 initial version.
		/// </summary>
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AsciiImportDataSource), 0)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				var s = (AsciiImportDataSource)obj;

				info.AddValue("InputData", s._asciiFilePath);
				info.AddValue("AsciiImportOptions", s._asciiImportOptions);
				info.AddValue("ImportOptions", s._importOptions);
			}

			protected virtual AsciiImportDataSource SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = (o == null ? new AsciiImportDataSource() : (AsciiImportDataSource)o);

				s._asciiFilePath = (AbsoluteAndRelativeFilePath)info.GetValue("InputData");
				s._asciiImportOptions = (AsciiImportOptions)info.GetValue("TransformationOptions");
				s._importOptions = (IDataSourceImportOptions)info.GetValue("ImportOptions");

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

		#region Construction

		public bool CopyFrom(object obj)
		{
			if (object.ReferenceEquals(this, obj))
				return true;

			var from = obj as AsciiImportDataSource;
			if (null != from)
			{
				using (var token = SuppressEventsGettingToken())
				{
					AsciiImportOptions asciiImportOptions = null;
					IDataSourceImportOptions importOptions = null;
					AbsoluteAndRelativeFilePath asciiFilePath = null;

					CopyHelper.Copy(ref importOptions, from._importOptions);
					CopyHelper.Copy(ref asciiImportOptions, from._asciiImportOptions);
					CopyHelper.Copy(ref asciiFilePath, from._asciiFilePath);

					this.SetAbsoluteRelativeFilePath(asciiFilePath);
					this.AsciiImportOptions = asciiImportOptions;
					this.ImportOptions = importOptions;

					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Deserialization constructor
		/// </summary>
		protected AsciiImportDataSource()
		{
			_eventSuppressor = new Main.EventSuppressor(EhResumeSuppressedEvents);
		}

		public AsciiImportDataSource(string fileName, AsciiImportOptions options)
		{
			_eventSuppressor = new Main.EventSuppressor(EhResumeSuppressedEvents);
			_asciiFilePath = new AbsoluteAndRelativeFilePath(fileName);
			_asciiImportOptions = options.Clone();
			_importOptions = new DataSourceImportOptions();
		}

		public AsciiImportDataSource(AsciiImportDataSource from)
		{
			_eventSuppressor = new Main.EventSuppressor(EhResumeSuppressedEvents);
			CopyFrom(from);
		}

		public object Clone()
		{
			return new AsciiImportDataSource(this);
		}

		#endregion Construction

		private void EhResumeSuppressedEvents()
		{
			var ev = _dataSourceChanged;
			if (null != ev)
				ev(this);

			UpdateWatching();
		}

		public void FillData(DataTable destinationTable)
		{
			string fileName = _asciiFilePath.GetResolvedFileNameOrNull();

			if (string.IsNullOrEmpty(fileName))
				return;

			using (var stream = new System.IO.FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite))
			{
				AsciiImporter.Import(stream, AsciiImporter.FileUrlStart + fileName, destinationTable, _asciiImportOptions);
			}
		}

		#region Properties

		public event Action<IAltaxoTableDataSource> DataSourceChanged
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

		public string SourceFileName
		{
			get
			{
				return _asciiFilePath.GetResolvedFileNameOrNull() ?? _asciiFilePath.AbsoluteFilePath;
			}
			set
			{
				var oldName = SourceFileName;
				_asciiFilePath = new AbsoluteAndRelativeFilePath(value);

				if (oldName != SourceFileName)
				{
					UpdateWatching();
				}
			}
		}

		public IDataSourceImportOptions ImportOptions
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

				if (!object.ReferenceEquals(oldValue, value))
				{
					UpdateWatching();
				}
			}
		}

		public AsciiImportOptions AsciiImportOptions
		{
			get
			{
				return _asciiImportOptions.Clone();
			}
			set
			{
				_asciiImportOptions = value.Clone();
			}
		}

		#endregion Properties

		private void SetAbsoluteRelativeFilePath(AbsoluteAndRelativeFilePath value)
		{
			if (null == value)
				throw new NotImplementedException();

			var oldValue = _asciiFilePath;
			_asciiFilePath = value;

			if (!value.Equals(oldValue))
			{
				UpdateWatching();
			}
		}

		public void OnAfterDeserialization()
		{
			UpdateWatching();
		}

		public void VisitDocumentReferences(Main.DocNodeProxyReporter ReportProxies)
		{
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

		public void UpdateWatching()
		{
			SwitchOffWatching();

			if (_eventSuppressor.PeekDisabled)
				return; // in update operation - wait until finished

			if (null == _dataSourceChanged)
				return; // No listener - no need to watch

			if (_importOptions.ImportTriggerSource != ImportTriggerSource.DataSourceChanged)
				return; // DataSource is updated manually

			var filePath = _asciiFilePath.GetResolvedFileNameOrNull();
			if (string.IsNullOrEmpty(filePath))
				return; // No file name set

			SwitchOnWatching(filePath);
		}

		private void SwitchOnWatching(string filePath)
		{
			_triggerBasedUpdate = new Main.TriggerBasedUpdate(Current.TimerQueue);
			_triggerBasedUpdate.MinimumWaitingTimeAfterUpdate = TimeSpanAccurate.FromSeconds(_importOptions.MinimumTimeIntervalBetweenUpdatesInSeconds);
			_triggerBasedUpdate.MaximumWaitingTimeAfterUpdate = TimeSpanAccurate.FromSeconds(Math.Max(_importOptions.MinimumTimeIntervalBetweenUpdatesInSeconds, _importOptions.PollTimeIntervalInSeconds));
			_triggerBasedUpdate.UpdateAction += EhUpdateByTimerQueue;

			try
			{
				_fileSystemWatcher = new System.IO.FileSystemWatcher(System.IO.Path.GetDirectoryName(filePath));
				_fileSystemWatcher.Changed += EhTriggerByFileSystemWatcher;
				_fileSystemWatcher.IncludeSubdirectories = false;
				_fileSystemWatcher.EnableRaisingEvents = true;
			}
			catch (Exception ex)
			{
				_fileSystemWatcher = null;
			}
		}

		private void SwitchOffWatching()
		{
			IDisposable disp;
			disp = _fileSystemWatcher;
			_fileSystemWatcher = null;
			if (null != disp)
				disp.Dispose();

			disp = _triggerBasedUpdate;
			_triggerBasedUpdate = null;
			if (null != disp)
				disp.Dispose();
		}

		public void EhUpdateByTimerQueue()
		{
			var ev = _dataSourceChanged;
			if (null != ev)
				ev(this);
		}

		private void EhTriggerByFileSystemWatcher(object sender, System.IO.FileSystemEventArgs e)
		{
			if (e.FullPath != _asciiFilePath.AbsoluteFilePath)
				return;

			if (null != _triggerBasedUpdate)
			{
				_triggerBasedUpdate.Trigger();
			}
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
	}
}