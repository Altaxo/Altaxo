#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2026 Dr. Dirk Lellinger
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
using System.Linq;
using System.Text;
using Altaxo.Data;

namespace Altaxo.Serialization
{
  public abstract class FileImportTableDataSourceBase : TableDataSourceBase, ICloneable, Altaxo.Data.IAltaxoTableDataSource
  {
    protected IDataSourceImportOptions _importOptions;

    protected List<AbsoluteAndRelativeFileName> _files = [];

    private HashSet<string> _resolvedFileNames = [];

    protected bool _isDirty = false;

    protected System.IO.FileSystemWatcher[] _fileSystemWatchers = [];

    protected Altaxo.Main.TriggerBasedUpdate? _triggerBasedUpdate;

    /// <summary>Indicates that serialization of the whole AltaxoDocument (!) is still in progress. Data sources should not be updated during serialization.</summary>
    [NonSerialized]
    protected bool _isDeserializationInProgress;

    protected FileImportTableDataSourceBase()
    {
      _importOptions = new DataSourceImportOptions();
    }

    protected FileImportTableDataSourceBase(IEnumerable<string> fileNames)
      : this()
    {
      foreach (var fileName in fileNames)
        _files.Add(new AbsoluteAndRelativeFileName(fileName));
    }

    protected FileImportTableDataSourceBase(FileImportTableDataSourceBase from)
    {
      using (var token = SuspendGetToken())
      {
        _files = CopyHelper.GetEnumerationMembersNotNullCloned(from._files).ToList();
        _importOptions = from._importOptions;
        EhSelfChanged(EventArgs.Empty);
        token.Resume();
      }
    }

    protected abstract override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName();

    public abstract void VisitDocumentReferences(Main.DocNodeProxyReporter ReportProxies);


    protected override void OnResume(int eventCount)
    {
      base.OnResume(eventCount);

      // UpdateWatching should only be called if something concerning the watch (Times etc.) has changed during the suspend phase
      // Otherwise it will cause endless loops because UpdateWatching triggers immediatly an EhUpdateByTimerQueue event, which triggers an UpdateDataSource event, which leads to another Suspend and then Resume, which calls OnResume(). So the loop is closed.
      if (_triggerBasedUpdate is null)
        UpdateWatching(); // Compromise - we update only if the watch is off
    }

    public override void FillData_Unchecked(DataTable destinationTable, IProgressReporter reporter)
    {
      var validFileNames = _files.Select(x => x.GetResolvedFileNameOrNull()).OfType<string>().Where(x => !string.IsNullOrEmpty(x)).ToArray();

      if (validFileNames.Length > 0)
      {
        destinationTable.DataColumns.RemoveColumnsAll();
        destinationTable.PropCols.RemoveColumnsAll();
        ImportFromFiles(validFileNames, destinationTable, reporter);
      }

      var invalidFileNames = _files.Where(x => string.IsNullOrEmpty(x.GetResolvedFileNameOrNull())).ToArray();
      if (invalidFileNames.Length > 0)
      {
        var stb = new StringBuilder();
        stb.AppendLine("The following file names could not be resolved:");
        foreach (var fn in invalidFileNames)
          stb.AppendLine(fn.AbsoluteFileName);
        stb.AppendLine("(End of file names that could not be resolved)");

        throw new ApplicationException(stb.ToString());
      }
    }

    protected abstract void ImportFromFiles(string[] validFileNames, DataTable destinationTable, IProgressReporter reporter);

    public string SourceFileName
    {
      get
      {
        if (_files.Count != 1)
          throw new InvalidOperationException("In order to get the source file name, the number of files has to be one");

        return _files[0].GetResolvedFileNameOrNull() ?? _files[0].AbsoluteFileName;
      }
      set
      {
        string? oldName = null;
        if (_files.Count == 1)
          oldName = SourceFileName;

        _files.Clear();
        _files.Add(new AbsoluteAndRelativeFileName(value));

        if (oldName != SourceFileName)
          UpdateWatching();
      }
    }

    public IEnumerable<string> SourceFileNames
    {
      get => _files.Select(x => x.GetResolvedFileNameOrNull() ?? x.AbsoluteFileName);
      set
      {
        _files.Clear();
        foreach (var name in value)
          _files.Add(new AbsoluteAndRelativeFileName(name));

        UpdateWatching();
      }
    }

    public virtual object ProcessDataObject
    {
      get => SourceFileNames;
      set => SourceFileNames = (IEnumerable<string>)value;
    }

    public int SourceFileNameCount => _files.Count;

    public override IDataSourceImportOptions ImportOptions
    {
      get => _importOptions;
      set
      {
        if (value is null)
          throw new ArgumentNullException(nameof(ImportOptions));

        var oldValue = _importOptions;
        _importOptions = value;

        if (!ReferenceEquals(oldValue, value))
          UpdateWatching();
      }
    }

    public abstract object ProcessOptionsObject { get; set; }

    protected void SetSingleFile(AbsoluteAndRelativeFileName value)
    {
      if (value is null)
        throw new ArgumentNullException(nameof(value));

      var oldValue = _files.Count == 1 ? _files[0] : null;
      _files.Clear();
      _files.Add(value);

      if (!value.Equals(oldValue))
        UpdateWatching();
    }

    public void OnAfterDeserialization()
    {
      // Note: it is not neccessary to call UpdateWatching here; UpdateWatching is called when the table connects to this data source via subscription to the DataSourceChanged event
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

      var validFileNames = _files.Select(x => x.GetResolvedFileNameOrNull()).OfType<string>().Where(x => !string.IsNullOrEmpty(x)).ToArray();
      if (0 == validFileNames.Length)
        return;  // No file name set

      _resolvedFileNames = new HashSet<string>(validFileNames);

      SwitchOnWatching(validFileNames);
    }

    private void SwitchOnWatching(string[] validFileNames)
    {
      _triggerBasedUpdate = new Main.TriggerBasedUpdate(Current.TimerQueue)
      {
        MinimumWaitingTimeAfterUpdate = TimeSpanExtensions.FromSecondsAccurate(_importOptions.MinimumWaitingTimeAfterUpdateInSeconds),
        MaximumWaitingTimeAfterUpdate = TimeSpanExtensions.FromSecondsAccurate(Math.Max(_importOptions.MinimumWaitingTimeAfterUpdateInSeconds, _importOptions.MaximumWaitingTimeAfterUpdateInSeconds)),
        MinimumWaitingTimeAfterFirstTrigger = TimeSpanExtensions.FromSecondsAccurate(_importOptions.MinimumWaitingTimeAfterFirstTriggerInSeconds),
        MinimumWaitingTimeAfterLastTrigger = TimeSpanExtensions.FromSecondsAccurate(_importOptions.MinimumWaitingTimeAfterLastTriggerInSeconds),
        MaximumWaitingTimeAfterFirstTrigger = TimeSpanExtensions.FromSecondsAccurate(Math.Max(_importOptions.MaximumWaitingTimeAfterFirstTriggerInSeconds, _importOptions.MinimumWaitingTimeAfterFirstTriggerInSeconds))
      };

      _triggerBasedUpdate.UpdateAction += EhUpdateByTimerQueue;

      var directories = new HashSet<string>(validFileNames.Select(x => System.IO.Path.GetDirectoryName(x)).OfType<string>());
      var watchers = new List<System.IO.FileSystemWatcher>();
      foreach (var directory in directories)
      {
        try
        {
          var watcher = new System.IO.FileSystemWatcher(directory)
          {
            NotifyFilter = System.IO.NotifyFilters.LastWrite | System.IO.NotifyFilters.Size
          };
          watcher.Changed += EhTriggerByFileSystemWatcher;
          watcher.IncludeSubdirectories = false;
          watcher.EnableRaisingEvents = true;
          watchers.Add(watcher);
        }
        catch (Exception)
        {
        }
      }
      _fileSystemWatchers = watchers.ToArray();
    }

    private void SwitchOffWatching()
    {
      IDisposable? disp = null;

      var watchers = _fileSystemWatchers;
      _fileSystemWatchers = new System.IO.FileSystemWatcher[0];

      for (int i = 0; i < watchers.Length; ++i)
      {
        disp = watchers[i];
        if (disp is not null)
          disp.Dispose();
      }

      disp = _triggerBasedUpdate;
      _triggerBasedUpdate = null;
      if (disp is not null)
        disp.Dispose();
    }

    public void EhUpdateByTimerQueue()
    {
      if (_parent is not null)
      {
        if (!IsSuspended) // no events during the suspend phase
          EhChildChanged(this, TableDataSourceChangedEventArgs.Empty);
      }
      else
      {
        SwitchOffWatching();
      }
    }

    private void EhTriggerByFileSystemWatcher(object sender, System.IO.FileSystemEventArgs e)
    {
      if (!_resolvedFileNames.Contains(e.FullPath))
        return;

      _triggerBasedUpdate?.Trigger();
    }

    protected void EhAfterDeserializationHasCompletelyFinished()
    {
      _isDeserializationInProgress = false;
      UpdateWatching();
    }

    protected override void Dispose(bool disposing)
    {
      if (!IsDisposed)
        SwitchOffWatching();

      base.Dispose(disposing);
    }

    public abstract (IReadOnlyList<string> FileExtensions, string Explanation) GetFileExtensions();

    public abstract object Clone();

    public abstract bool CopyFrom(object obj);
  }

  public abstract class FileImportTableDataSourceBase<TProcessOptions> : FileImportTableDataSourceBase, IAltaxoTableDataSource where TProcessOptions : Main.IImmutable
  {
    protected TProcessOptions _processOptions;

    protected FileImportTableDataSourceBase()
    {
    }

    protected FileImportTableDataSourceBase(IEnumerable<string> fileNames)
      : base(fileNames)
    {
    }

    protected FileImportTableDataSourceBase(FileImportTableDataSourceBase<TProcessOptions> from)
      : base(from)
    {
      using (var token = SuspendGetToken())
      {
        _processOptions = from._processOptions;
        EhSelfChanged(EventArgs.Empty);
        token.Resume();
      }
    }

    public override object ProcessOptionsObject
    {
      get => _processOptions;
      set => _processOptions = ((TProcessOptions)value);
    }

    public TProcessOptions ProcessOptions
    {
      get => _processOptions;
      set => _processOptions = value;
    }



    public override bool CopyFrom(object obj)
    {
      if (ReferenceEquals(this, obj))
        return true;

      if (obj is FileImportTableDataSourceBase<TProcessOptions> from)
      {
        using (var token = SuspendGetToken())
        {
          _files = new List<AbsoluteAndRelativeFileName>(CopyHelper.GetEnumerationMembersNotNullCloned(from._files));
          _importOptions = from._importOptions;
          _processOptions = from._processOptions;
          EhSelfChanged(EventArgs.Empty);
          token.Resume();
        }
        return true;
      }

      return false;
    }

    public override void VisitDocumentReferences(Main.DocNodeProxyReporter ReportProxies)
    {
    }

    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (_processOptions is Main.IDocumentNode docNode)
        yield return new Main.DocumentNodeAndName(docNode, "ProcessOptions");
    }
  }
}
