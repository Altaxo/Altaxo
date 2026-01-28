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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Altaxo.Data;

namespace Altaxo.Serialization.Omnic
{
  /// <summary>
  /// Table data source for importing Omnic SPG (spectrum group) files.
  /// </summary>
  public class OmnicSPGImportDataSource : TableDataSourceBase, Altaxo.Data.IAltaxoTableDataSource
  {
    private IDataSourceImportOptions _importOptions;

    private OmnicSPGImportOptions _processOptions;

    private List<AbsoluteAndRelativeFileName> _asciiFiles = new List<AbsoluteAndRelativeFileName>();

    private HashSet<string> _resolvedAsciiFileNames = new HashSet<string>();

    protected bool _isDirty = false;

    protected System.IO.FileSystemWatcher[] _fileSystemWatchers = new System.IO.FileSystemWatcher[0];

    protected Altaxo.Main.TriggerBasedUpdate? _triggerBasedUpdate;

    /// <summary>Indicates that serialization of the whole AltaxoDocument (!) is still in progress. Data sources should not be updated during serialization.</summary>
    [NonSerialized]
    protected bool _isDeserializationInProgress;

    #region Serialization

    #region Version 0

    /// <summary>
    /// 2026-01-27 Initial version
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(OmnicSPGImportDataSource), 1)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public virtual void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (OmnicSPGImportDataSource)obj;

        info.AddValue("ImportOptions", s._importOptions);
        info.AddValue("ProcessOptions", s._processOptions);
        info.AddArray("ProcessData", s._asciiFiles.ToArray(), s._asciiFiles.Count);
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        if (o is OmnicSPGImportDataSource s)
          s.DeserializeSurrogate0(info);
        else
          s = new OmnicSPGImportDataSource(info, 0);
        return s;
      }
    }

    [MemberNotNull(nameof(_importOptions), nameof(_processOptions))]
    private void DeserializeSurrogate0(Altaxo.Serialization.Xml.IXmlDeserializationInfo info)
    {
      _isDeserializationInProgress = true;
      ChildSetMember(ref _importOptions, (IDataSourceImportOptions)info.GetValue("ImportOptions", this));
      _processOptions = (OmnicSPGImportOptions)info.GetValue("ProcessOptions", this);
      var count = info.OpenArray("ProcessData");
      for (int i = 0; i < count; ++i)
        _asciiFiles.Add((AbsoluteAndRelativeFileName)info.GetValue("e", this));
      info.CloseArray(count);

      info.AfterDeserializationHasCompletelyFinished += EhAfterDeserializationHasCompletelyFinished;
    }

    #endregion Version 0

    /// <summary>
    /// Deserialization constructor.
    /// </summary>
    protected OmnicSPGImportDataSource(Altaxo.Serialization.Xml.IXmlDeserializationInfo info, int version)
    {
      switch (version)
      {
        case 0:
          DeserializeSurrogate0(info);
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof(version));
      }
    }

    #endregion Serialization

    #region Construction

    [MemberNotNull(nameof(_asciiFiles), nameof(_importOptions), nameof(_processOptions))]
    private void CopyFrom(OmnicSPGImportDataSource from)
    {
      using (var token = SuspendGetToken())
      {
        _asciiFiles = new List<AbsoluteAndRelativeFileName>(CopyHelper.GetEnumerationMembersNotNullCloned(from._asciiFiles));
        ChildCloneToMember(ref _importOptions, from._importOptions);
        _processOptions = from._processOptions;
        EhSelfChanged(EventArgs.Empty);
        token.Resume();
      }
    }

    /// <inheritdoc/>
    public bool CopyFrom(object obj)
    {
      if (ReferenceEquals(this, obj))
        return true;

      var from = obj as OmnicSPGImportDataSource;
      if (from is not null)
      {
        CopyFrom(from);
        return true;
      }
      return false;
    }




    /// <summary>
    /// Initializes a new instance of the <see cref="OmnicSPGImportDataSource"/> class for a single file.
    /// </summary>
    /// <param name="fileName">The file name of the SPG file.</param>
    /// <param name="options">The import options.</param>
    public OmnicSPGImportDataSource(string fileName, OmnicSPGImportOptions options)
      : this(new string[] { fileName }, options)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OmnicSPGImportDataSource"/> class for multiple files.
    /// </summary>
    /// <param name="fileNames">The file names of the SPG files.</param>
    /// <param name="options">The import options.</param>
    public OmnicSPGImportDataSource(IEnumerable<string> fileNames, OmnicSPGImportOptions options)
    {
      _asciiFiles = new List<AbsoluteAndRelativeFileName>();
      foreach (var fileName in fileNames)
      {
        _asciiFiles.Add(new AbsoluteAndRelativeFileName(fileName));
      }
      _processOptions = options;
      _importOptions = new DataSourceImportOptions() { ParentObject = this };
    }

    public OmnicSPGImportDataSource(OmnicSPGImportDataSource from)
    {
      CopyFrom(from);
    }

    /// <inheritdoc/>
    public object Clone()
    {
      return new OmnicSPGImportDataSource(this);
    }

    /// <inheritdoc/>
    protected override IEnumerable<Main.DocumentNodeAndName> GetDocumentNodeChildrenWithName()
    {
      if (_importOptions is not null)
        yield return new Main.DocumentNodeAndName(_importOptions, () => _importOptions = null!, "ImportOptions");
    }

    #endregion Construction

    /// <inheritdoc/>
    protected override void OnResume(int eventCount)
    {
      base.OnResume(eventCount);

      // UpdateWatching should only be called if something concerning the watch (Times etc.) has changed during the suspend phase
      // Otherwise it will cause endless loops because UpdateWatching triggers immediatly an EhUpdateByTimerQueue event, which triggers an UpdateDataSource event, which leads to another Suspend and then Resume, which calls OnResume(). So the loop is closed.
      if (_triggerBasedUpdate is null)
        UpdateWatching(); // Compromise - we update only if the watch is off
    }

    /// <inheritdoc/>
    public override void FillData_Unchecked(DataTable destinationTable, IProgressReporter reporter)
    {
      var validFileNames = _asciiFiles.Select(x => x.GetResolvedFileNameOrNull()).OfType<string>().Where(x => !string.IsNullOrEmpty(x)).ToArray();

      if (validFileNames.Length > 0)
      {
        destinationTable.DataColumns.RemoveColumnsAll();
        destinationTable.PropCols.RemoveColumnsAll();
        new OmnicSPGImporter().Import(validFileNames, destinationTable, _processOptions, attachDataSource: false);
      }

      var invalidFileNames = _asciiFiles.Where(x => string.IsNullOrEmpty(x.GetResolvedFileNameOrNull())).ToArray();
      if (invalidFileNames.Length > 0)
      {
        var stb = new StringBuilder();
        stb.AppendLine("The following file names could not be resolved:");
        foreach (var fn in invalidFileNames)
        {
          stb.AppendLine(fn.AbsoluteFileName);
        }
        stb.AppendLine("(End of file names that could not be resolved)");

        throw new ApplicationException(stb.ToString());
      }
    }

    #region Properties

    /// <summary>
    /// Gets or sets the (single) source file name.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the number of source file names is not exactly one.</exception>
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
        string? oldName = null;
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

    /// <summary>
    /// Gets or sets the source file names.
    /// </summary>
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

    /// <summary>
    /// Gets the number of source file names.
    /// </summary>
    public int SourceFileNameCount
    {
      get
      {
        return _asciiFiles.Count;
      }
    }

    /// <inheritdoc/>
    public override IDataSourceImportOptions ImportOptions
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

        if (!object.ReferenceEquals(oldValue, value))
        {
          UpdateWatching();
        }
      }
    }

    /// <summary>
    /// Gets or sets the strongly typed process (import) options.
    /// </summary>
    public OmnicSPGImportOptions ProcessOptions
    {
      get
      {
        return _processOptions;
      }
      set
      {
        _processOptions = value;
      }
    }

    /// <inheritdoc/>
    object IAltaxoTableDataSource.ProcessOptionsObject
    {
      get => _processOptions;
      set => ProcessOptions = (OmnicSPGImportOptions)value;
    }

    /// <inheritdoc/>
    object IAltaxoTableDataSource.ProcessDataObject
    {
      get => SourceFileNames;
      set => SourceFileNames = (IEnumerable<string>)value;
    }

    #endregion Properties

    private void SetAbsoluteRelativeFilePath(AbsoluteAndRelativeFileName value)
    {
      if (value is null)
        throw new ArgumentNullException("value");

      var oldValue = _asciiFiles.Count == 1 ? _asciiFiles[0] : null;
      _asciiFiles.Clear();
      _asciiFiles.Add(value);

      if (!value.Equals(oldValue))
      {
        UpdateWatching();
      }
    }

    /// <inheritdoc/>
    public void OnAfterDeserialization()
    {
      // Note: it is not necessary to call UpdateWatching here; UpdateWatching is called when the table connects to this data source via subscription to the DataSourceChanged event
    }

    /// <inheritdoc/>
    public void VisitDocumentReferences(Main.DocNodeProxyReporter ReportProxies)
    {
    }

    /// <summary>
    /// Starts or updates file system watching according to the current import options.
    /// </summary>
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

      var validFileNames = _asciiFiles.Select(x => x.GetResolvedFileNameOrNull()).OfType<string>().Where(x => !string.IsNullOrEmpty(x)).ToArray();
      if (0 == validFileNames.Length)
        return;  // No file name set

      _resolvedAsciiFileNames = new HashSet<string>(validFileNames);

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
        {
          EhChildChanged(this, TableDataSourceChangedEventArgs.Empty);
        }
      }
      else
        SwitchOffWatching();
    }

    private void EhTriggerByFileSystemWatcher(object sender, System.IO.FileSystemEventArgs e)
    {
      if (!_resolvedAsciiFileNames.Contains(e.FullPath))
        return;

      if (_triggerBasedUpdate is not null)
      {
        _triggerBasedUpdate.Trigger();
      }
    }

    private void EhAfterDeserializationHasCompletelyFinished()
    {
      _isDeserializationInProgress = false;
      UpdateWatching();
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
      if (!IsDisposed)
        SwitchOffWatching();

      base.Dispose(disposing);
    }

    /// <inheritdoc/>
    public (IReadOnlyList<string> FileExtensions, string Explanation) GetFileExtensions()
    {
      return new OmnicSPGImporter().GetFileExtensions();
    }
  }
}
