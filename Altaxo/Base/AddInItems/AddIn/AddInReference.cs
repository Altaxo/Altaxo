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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Altaxo.Main.Services;

namespace Altaxo.AddInItems
{
  /// <summary>
  /// Represents a versioned reference to an AddIn. Used by <see cref="AddInManifest"/>.
  /// </summary>
  public class AddInReference : ICloneable
  {
    private string _name;
    private Version _minimumVersion;
    private Version _maximumVersion;
    private bool _requirePreload;

    private static Version? _entryVersion;


    public Version MinimumVersion
    {
      get
      {
        return _minimumVersion;
      }
    }

    public Version MaximumVersion
    {
      get
      {
        return _maximumVersion;
      }
    }

    public bool RequirePreload
    {
      get { return _requirePreload; }
    }

    public string Name
    {
      get
      {
        return _name;
      }
      [MemberNotNull(nameof(_name))]
      set
      {
        if (value is null)
          throw new ArgumentNullException("name");
        if (value.Length == 0)
          throw new ArgumentException("name cannot be an empty string", "name");
        _name = value;
      }
    }

    /// <returns>Returns true when the reference is valid.</returns>
    public bool Check(Dictionary<string, Version> addIns, out Version? versionFound)
    {
      if (addIns.TryGetValue(_name, out versionFound))
      {
        return CompareVersion(versionFound, _minimumVersion) >= 0
          && CompareVersion(versionFound, _maximumVersion) <= 0;
      }
      else
      {
        return false;
      }
    }

    /// <summary>
    /// Compares two versions and ignores unspecified fields (unlike Version.CompareTo)
    /// </summary>
    /// <returns>-1 if a &lt; b, 0 if a == b, 1 if a &gt; b</returns>
    private int CompareVersion(Version a, Version b)
    {
      if (a.Major != b.Major)
      {
        return a.Major > b.Major ? 1 : -1;
      }
      if (a.Minor != b.Minor)
      {
        return a.Minor > b.Minor ? 1 : -1;
      }
      if (a.Build < 0 || b.Build < 0)
        return 0;
      if (a.Build != b.Build)
      {
        return a.Build > b.Build ? 1 : -1;
      }
      if (a.Revision < 0 || b.Revision < 0)
        return 0;
      if (a.Revision != b.Revision)
      {
        return a.Revision > b.Revision ? 1 : -1;
      }
      return 0;
    }

    public static AddInReference Create(Properties properties, string? hintPath)
    {
      var reference = new AddInReference(properties["addin"]);
      string version = properties["version"];
      if (version is not null && version.Length > 0)
      {
        int pos = version.IndexOf('-');
        if (pos > 0)
        {
          reference._minimumVersion = ParseVersion(version.Substring(0, pos), hintPath);
          reference._maximumVersion = ParseVersion(version.Substring(pos + 1), hintPath);
        }
        else
        {
          reference._maximumVersion = reference._minimumVersion = ParseVersion(version, hintPath);
        }

        if (reference.Name == "SharpDevelop")
        {
          // HACK: SD 4.1/4.2/4.3 AddIns work with SharpDevelop 4.4
          // Because some 4.1 AddIns restrict themselves to SD 4.1, we extend the
          // supported SD range.
          if (reference._maximumVersion == new Version("4.1") || reference._maximumVersion == new Version("4.2") || reference._maximumVersion == new Version("4.3"))
          {
            reference._maximumVersion = new Version("4.4");
          }
        }
      }
      reference._requirePreload = string.Equals(properties["requirePreload"], "true", StringComparison.OrdinalIgnoreCase);
      return reference;
    }


    internal static Version ParseVersion(string version, string? hintPath)
    {
      if (version is null || version.Length == 0)
        return new Version(0, 0, 0, 0);
      if (version.StartsWith("@"))
      {
        if (version == "@SharpDevelopCoreVersion")
        {
          if (_entryVersion is null)
            _entryVersion = new Version(RevisionClass.Major + "." + RevisionClass.Minor + "." + RevisionClass.Build + "." + RevisionClass.Revision);
          return _entryVersion;
        }
        if (hintPath is not null)
        {
          string fileName = Path.Combine(hintPath, version.Substring(1));
          try
          {
            var info = FileVersionInfo.GetVersionInfo(fileName);
            return new Version(info.FileMajorPart, info.FileMinorPart, info.FileBuildPart, info.FilePrivatePart);
          }
          catch (FileNotFoundException ex)
          {
            throw new AddInLoadException("Cannot get version '" + version + "': " + ex.Message);
          }
        }
        return new Version(0, 0, 0, 0);
      }
      else
      {
        return new Version(version);
      }
    }

    public AddInReference(string name) : this(name, new Version(0, 0, 0, 0), new Version(int.MaxValue, int.MaxValue))
    {
    }

    public AddInReference(string name, Version specificVersion) : this(name, specificVersion, specificVersion)
    {
    }

    public AddInReference(string name, Version minimumVersion, Version maximumVersion)
    {
      Name = name;
      if (minimumVersion is null)
        throw new ArgumentNullException("minimumVersion");
      if (maximumVersion is null)
        throw new ArgumentNullException("maximumVersion");

      this._minimumVersion = minimumVersion;
      this._maximumVersion = maximumVersion;
    }

    public override bool Equals(object? obj)
    {
      if (!(obj is AddInReference))
        return false;
      var b = (AddInReference)obj;
      return _name == b._name && _minimumVersion == b._minimumVersion && _maximumVersion == b._maximumVersion;
    }

    public override int GetHashCode()
    {
      return _name.GetHashCode() ^ _minimumVersion.GetHashCode() ^ _maximumVersion.GetHashCode();
    }

    public override string ToString()
    {
      if (_minimumVersion.ToString() == "0.0.0.0")
      {
        if (_maximumVersion.Major == int.MaxValue)
        {
          return _name;
        }
        else
        {
          return _name + ", version <" + _maximumVersion.ToString();
        }
      }
      else
      {
        if (_maximumVersion.Major == int.MaxValue)
        {
          return _name + ", version >" + _minimumVersion.ToString();
        }
        else if (_minimumVersion == _maximumVersion)
        {
          return _name + ", version " + _minimumVersion.ToString();
        }
        else
        {
          return _name + ", version " + _minimumVersion.ToString() + "-" + _maximumVersion.ToString();
        }
      }
    }

    public AddInReference Clone()
    {
      return new AddInReference(_name, _minimumVersion, _maximumVersion);
    }

    object ICloneable.Clone()
    {
      return Clone();
    }
  }
}
