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

		private List<AbsoluteAndRelativeFileName> _asciiFiles = new List<AbsoluteAndRelativeFileName>();

		private HashSet<string> _resolvedAsciiFileNames = new HashSet<string>();

		protected Action<IAltaxoTableDataSource> _dataSourceChanged;

		protected Main.EventSuppressor _eventSuppressor;

		protected bool _isDirty = false;

		protected System.IO.FileSystemWatcher[] _fileSystemWatchers = new System.IO.FileSystemWatcher[0];

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

				info.AddValue("ImportOptions", s._importOptions);
				info.AddValue("AsciiImportOptions", s._asciiImportOptions);

				info.AddArray("AsciiFiles", s._asciiFiles.ToArray(), s._asciiFiles.Count);
			}

			protected virtual AsciiImportDataSource SDeserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				var s = (o == null ? new AsciiImportDataSource() : (AsciiImportDataSource)o);

				s._importOptions = (IDataSourceImportOptions)info.GetValue("ImportOptions");
				s._asciiImportOptions = (AsciiImportOptions)info.GetValue("AsciiImportOptions");
				var count = info.OpenArray("AsciiFiles");
				for (int i = 0; i < count; ++i)
					s._asciiFiles.Add((AbsoluteAndRelativeFileName)info.GetValue("e"));
				info.CloseArray(count);

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
					AbsoluteAndRelativeFileName asciiFilePath = null;

					CopyHelper.Copy(ref importOptions, from._importOptions);
					CopyHelper.Copy(ref asciiImportOptions, from._asciiImportOptions);
					_asciiFiles = new List<AbsoluteAndRelativeFileName>(CopyHelper.GetEnumerationMembersCloned(from._asciiFiles));

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
			_asciiFiles = new List<AbsoluteAndRelativeFileName>();
		}

		public AsciiImportDataSource(string fileName, AsciiImportOptions options)
			: this(new string[] { fileName }, options)
		{
		}

		public AsciiImportDataSource(IEnumerable<string> fileNames, AsciiImportOptions options)
		{
			_eventSuppressor = new Main.EventSuppressor(EhResumeSuppressedEvents);
			_asciiFiles = new List<AbsoluteAndRelativeFileName>();
			foreach (var fileName in fileNames)
			{
				_asciiFiles.Add(new AbsoluteAndRelativeFileName(fileName));
			}
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
			var validFileNames = _asciiFiles.Select(x => x.GetResolvedFileNameOrNull()).Where(x => !string.IsNullOrEmpty(x)).ToArray();

			if (validFileNames.Length == 0)
				return;

			if (_asciiFiles.Count == 1)
			{
				using (var stream = new System.IO.FileStream(validFileNames[0], System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite))
				{
					AsciiImporter.ImportFromAsciiStream(destinationTable, stream, AsciiImporter.FileUrlStart + validFileNames[0], _asciiImportOptions);
				}
			}
			else
			{
				if (_asciiImportOptions.ImportMultipleStreamsVertically)
					FileCommands.ImportAsciiToSingleWorksheetVertically(destinationTable, validFileNames, _asciiImportOptions);
				else
					FileCommands.ImportAsciiToSingleWorksheetHorizontally(destinationTable, validFileNames, _asciiImportOptions);
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
				if (_asciiFiles.Count != 1)
					throw new InvalidOperationException("In order to get the source file name, the number of files has to be one");

				return _asciiFiles[0].GetResolvedFileNameOrNull() ?? _asciiFiles[0].AbsoluteFileName;
			}
			set
			{
				string oldName = null;
				if (_asciiFiles.Count == 1)
					oldName = SourceFileName;

				_asciiFiles.Clear();
				_asciiFiles.Add(new AbsoluteAndRelativeFileName(value));

				if (oldName != SourceFileName)
				{
					UpdateWatching();
				}
			}
		}

		public IEnumerable<string> SourceFileNames
		{
			get
			{
				return _asciiFiles.Select(x => x.GetResolvedFileNameOrNull() ?? x.AbsoluteFileName);
			}
			set
			{
				_asciiFiles.Clear();
				foreach (var name in value)
					_asciiFiles.Add(new AbsoluteAndRelativeFileName(name));

				UpdateWatching();
			}
		}

		public int SourceFileNameCount
		{
			get
			{
				return _asciiFiles.Count;
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

		private void SetAbsoluteRelativeFilePath(AbsoluteAndRelativeFileName value)
		{
			if (null == value)
				throw new ArgumentNullException("value");

			var oldValue = _asciiFiles.Count == 1 ? _asciiFiles[0] : null;
			_asciiFiles.Clear();
			_asciiFiles.Add(value);

			if (!value.Equals(oldValue))
			{
				UpdateWatching();
			}
		}

		public void OnAfterDeserialization()
		{
			// Note: it is not neccessary to call UpdateWatching here; UpdateWatching is called when the table connects to this data source via subscription to the DataSourceChanged event
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

			var validFileNames = _asciiFiles.Select(x => x.GetResolvedFileNameOrNull()).Where(x => !string.IsNullOrEmpty(x)).ToArray();
			if (0 == validFileNames.Length)
				return;  // No file name set

			_resolvedAsciiFileNames = new HashSet<string>(validFileNames);

			SwitchOnWatching(validFileNames);
		}

		private void SwitchOnWatching(string[] validFileNames)
		{
			_triggerBasedUpdate = new Main.TriggerBasedUpdate(Current.TimerQueue);
			_triggerBasedUpdate.MinimumWaitingTimeAfterUpdate = TimeSpanExtensions.FromSecondsAccurate(_importOptions.MinimumWaitingTimeAfterUpdateInSeconds);
			_triggerBasedUpdate.MaximumWaitingTimeAfterUpdate = TimeSpanExtensions.FromSecondsAccurate(Math.Max(_importOptions.MinimumWaitingTimeAfterUpdateInSeconds, _importOptions.MaximumWaitingTimeAfterUpdateInSeconds));
			_triggerBasedUpdate.MinimumWaitingTimeAfterFirstTrigger = TimeSpanExtensions.FromSecondsAccurate(_importOptions.MinimumWaitingTimeAfterFirstTriggerInSeconds);
			_triggerBasedUpdate.MinimumWaitingTimeAfterLastTrigger = TimeSpanExtensions.FromSecondsAccurate(_importOptions.MinimumWaitingTimeAfterLastTriggerInSeconds);
			_triggerBasedUpdate.MaximumWaitingTimeAfterFirstTrigger = TimeSpanExtensions.FromSecondsAccurate(Math.Max(_importOptions.MaximumWaitingTimeAfterFirstTriggerInSeconds, _importOptions.MinimumWaitingTimeAfterFirstTriggerInSeconds));

			_triggerBasedUpdate.UpdateAction += EhUpdateByTimerQueue;

			var directories = new HashSet<string>(validFileNames.Select(x => System.IO.Path.GetDirectoryName(x)));
			var watchers = new List<System.IO.FileSystemWatcher>();
			foreach (var directory in directories)
			{
				try
				{
					var watcher = new System.IO.FileSystemWatcher(directory);
					watcher.NotifyFilter = System.IO.NotifyFilters.LastWrite | System.IO.NotifyFilters.Size;
					watcher.Changed += EhTriggerByFileSystemWatcher;
					watcher.IncludeSubdirectories = false;
					watcher.EnableRaisingEvents = true;
					watchers.Add(watcher);
				}
				catch (Exception ex)
				{
				}
			}
			_fileSystemWatchers = watchers.ToArray();
		}

		private void SwitchOffWatching()
		{
			IDisposable disp = null;

			var watchers = _fileSystemWatchers;
			_fileSystemWatchers = new System.IO.FileSystemWatcher[0];

			for (int i = 0; i < watchers.Length; ++i)
			{
				disp = watchers[i];
				if (null != disp)
					disp.Dispose();
			}

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
			else
				SwitchOffWatching();
		}

		private void EhTriggerByFileSystemWatcher(object sender, System.IO.FileSystemEventArgs e)
		{
			if (!_resolvedAsciiFileNames.Contains(e.FullPath))
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