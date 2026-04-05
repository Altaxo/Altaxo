// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Altaxo.Main;
using Altaxo.Main.Services;

namespace Altaxo.Main.Services
{
  /// <summary>
  /// This class handles the recent open files and the recent open project files. It provides almost all functionality necessary,
  /// with the exception that recent files are not added to the Windows shell jump list, since this requires a reference to the WPF assemblies.
  /// Use a class (RecentOpen) derived from here that implements this functionality.
  /// </summary>
  public class RecentOpenBase : IRecentOpen
  {
    /// <summary>
    /// The maximum length of recent-file and recent-project entries.
    /// </summary>
    private int MAX_LENGTH = 10;

    private IList<FileName> _recentFiles = new List<FileName>();
    private IList<FileName> _recentProjects = new List<FileName>();

    /// <summary>
    /// Gets the property key used to persist recent files.
    /// </summary>
    public static readonly Altaxo.Main.Properties.PropertyKey<IList<FileName>> PropertyKeyRecentFiles =
        new Altaxo.Main.Properties.PropertyKey<IList<FileName>>(
            "E89CEABA-079C-4523-A594-CCDB67023BA7",
            "App\\RecentFiles",
            Altaxo.Main.Properties.PropertyLevel.Application);
    private static readonly Altaxo.Main.Properties.PropertyKey<IList<FileName>> PropertyKeyRecentProjects =
    new Altaxo.Main.Properties.PropertyKey<IList<FileName>>(
        "0C3B80BC-BE6B-4270-B59A-AAF5BA9CF00C",
        "App\\RecentProjects",
        Altaxo.Main.Properties.PropertyLevel.Application);

    #region Serialization

    /// <summary>
    ///
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("mscorlib", "System.Collections.Generic.List`1[ICSharpCode.Core.FileName]", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("System.Private.CoreLib", "System.Collections.Generic.List`1[Altaxo.Main.Services.FileName]", 1)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("mscorlib", "System.Collections.Generic.List`1[Altaxo.Main.Services.FileName]", 1)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(List<FileName>), 1)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <inheritdoc/>
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (List<FileName>)obj;

        var count = s.Count;

        info.CreateArray("Properties", count);
        foreach (var entry in s)
        {
          info.AddValue("e", entry);
        }
        info.CommitArray();
      }

      /// <inheritdoc/>
      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = o as List<FileName> ?? new List<FileName>();
        s.Clear();
        int count = info.OpenArray("Properties");

        for (int i = 0; i < count; ++i)
        {
          var fn = FileName.Create(info.GetString("e"));
          if (!(fn is null))
            s.Add(fn);
        }
        info.CloseArray(count);
        return s;
      }
    }

    #endregion Serialization

    /// <summary>
    /// Initializes a new instance of the <see cref="RecentOpenBase"/> class.
    /// </summary>
    public RecentOpenBase()
    {
      _recentProjects = Current.PropertyService.GetValue(PropertyKeyRecentProjects, Altaxo.Main.Services.RuntimePropertyKind.UserAndApplicationAndBuiltin, () => new List<FileName>()) ?? new List<FileName>();
      _recentFiles = Current.PropertyService.GetValue(PropertyKeyRecentFiles, Altaxo.Main.Services.RuntimePropertyKind.UserAndApplicationAndBuiltin, () => new List<FileName>());
    }

    /// <inheritdoc/>
    public IReadOnlyList<FileName> RecentFiles
    {
      get { return new ReadOnlyCollection<FileName>(_recentFiles); }
    }

    /// <inheritdoc/>
    public IReadOnlyList<PathName> RecentProjects
    {
      get { return new ReadOnlyCollection<FileName>(_recentProjects); }
    }

    /// <inheritdoc/>
    public void AddRecentFile(FileName name)
    {
      _recentFiles.Remove(name); // remove if the filename is already in the list

      while (_recentFiles.Count >= MAX_LENGTH)
      {
        _recentFiles.RemoveAt(_recentFiles.Count - 1);
      }

      _recentFiles.Insert(0, name);

      Current.PropertyService.SetValue(PropertyKeyRecentFiles, _recentFiles);
    }

    /// <inheritdoc/>
    public void ClearRecentFiles()
    {
      _recentFiles.Clear();
      Current.PropertyService.SetValue(PropertyKeyRecentFiles, _recentFiles);
    }

    /// <inheritdoc/>
    public void ClearRecentProjects()
    {
      _recentProjects.Clear();
      Current.PropertyService.SetValue(PropertyKeyRecentProjects, _recentProjects);
    }

    /// <inheritdoc/>
    public void RemoveRecentProject(PathName pathName)
    {
      if (pathName is FileName name)
      {
        _recentProjects.Remove(name);
      }
      Current.PropertyService.SetValue(PropertyKeyRecentProjects, _recentProjects);
    }

    /// <inheritdoc/>
    public virtual void AddRecentProject(PathName pathName)
    {
      if (pathName is FileName name)
      {
        _recentProjects.Remove(name);

        while (_recentProjects.Count >= MAX_LENGTH)
        {
          _recentProjects.RemoveAt(_recentProjects.Count - 1);
        }

        _recentProjects.Insert(0, name);
      }

      Current.PropertyService.SetValue(PropertyKeyRecentProjects, _recentProjects);
    }

    /// <summary>
    /// Handles removal of a file from the recent file list.
    /// </summary>
    protected virtual void FileRemoved(object sender, FileEventArgs e)
    {
      for (int i = 0; i < _recentFiles.Count; ++i)
      {
        string file = _recentFiles[i].ToString();
        if (e.FileName == file)
        {
          _recentFiles.RemoveAt(i);
          break;
        }
      }
    }

    /// <summary>
    /// Handles renaming of a file in the recent file list.
    /// </summary>
    protected virtual void FileRenamed(object sender, FileRenameEventArgs e)
    {
      for (int i = 0; i < _recentFiles.Count; ++i)
      {
        string file = _recentFiles[i].ToString();
        if (e.SourceFile == file)
        {
          _recentFiles.RemoveAt(i);
          if (FileName.Create(e.TargetFile) is { } fn)
            _recentFiles.Insert(i, fn);
          break;
        }
      }
    }
  }
}
