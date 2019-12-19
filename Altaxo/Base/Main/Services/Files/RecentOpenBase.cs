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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Altaxo.Main;
using Altaxo.Main.Services;

namespace Altaxo.Main.Services
{
  /// <summary>
  /// This class handles the recent open files and the recent open project files. This is a class with almost all functionality neccessary,
  /// with the exeption that recent files are not added to the windows shell jumplist, since this requires a reference to the Wpf Dlls.
  /// Use a class (RecentOpen) derived from here that implements this functionality.
  /// </summary>
  public class RecentOpenBase : IRecentOpen
  {
    /// <summary>
    /// This variable is the maximal length of lastfile/lastopen entries
    /// must be > 0
    /// </summary>
    private int MAX_LENGTH = 10;

    private IList<FileName> _recentFiles = new List<FileName>();
    private IList<FileName> _recentProjects = new List<FileName>();

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
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(List<FileName>), 1)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
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

      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        var s = o as List<FileName> ?? new List<FileName>();
        s.Clear();
        int count = info.OpenArray("Properties");

        for (int i = 0; i < count; ++i)
        {
          s.Add(FileName.Create(info.GetString("e")));
        }
        info.CloseArray(count);
        return s;
      }
    }

    #endregion Serialization

    public RecentOpenBase()
    {
      _recentProjects = Current.PropertyService.GetValue(PropertyKeyRecentProjects, Altaxo.Main.Services.RuntimePropertyKind.UserAndApplicationAndBuiltin, () => new List<FileName>()) ?? new List<FileName>();
      _recentFiles = Current.PropertyService.GetValue(PropertyKeyRecentFiles, Altaxo.Main.Services.RuntimePropertyKind.UserAndApplicationAndBuiltin, () => new List<FileName>());
    }

    public IReadOnlyList<FileName> RecentFiles
    {
      get { return new ReadOnlyCollection<FileName>(_recentFiles); }
    }

    public IReadOnlyList<PathName> RecentProjects
    {
      get { return new ReadOnlyCollection<FileName>(_recentProjects); }
    }

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

    public void ClearRecentFiles()
    {
      _recentFiles.Clear();
      Current.PropertyService.SetValue(PropertyKeyRecentFiles, _recentFiles);
    }

    public void ClearRecentProjects()
    {
      _recentProjects.Clear();
      Current.PropertyService.SetValue(PropertyKeyRecentProjects, _recentProjects);
    }

    public void RemoveRecentProject(PathName pathName)
    {
      if (pathName is FileName name)
    {
      _recentProjects.Remove(name);
      }
      Current.PropertyService.SetValue(PropertyKeyRecentProjects, _recentProjects);
    }

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

    protected virtual void FileRenamed(object sender, FileRenameEventArgs e)
    {
      for (int i = 0; i < _recentFiles.Count; ++i)
      {
        string file = _recentFiles[i].ToString();
        if (e.SourceFile == file)
        {
          _recentFiles.RemoveAt(i);
          _recentFiles.Insert(i, FileName.Create(e.TargetFile));
          break;
        }
      }
    }
  }
}
