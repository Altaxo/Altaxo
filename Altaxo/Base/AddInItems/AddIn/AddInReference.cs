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


    /// <summary>
    /// Gets the minimum accepted version.
    /// </summary>
    public Version MinimumVersion
    {
      get
      {
        return _minimumVersion;
      }
    }

    /// <summary>
    /// Gets the maximum accepted version.
    /// </summary>
    public Version MaximumVersion
    {
      get
      {
        return _maximumVersion;
      }
    }

    /// <summary>
    /// Gets a value indicating whether the referenced add-in must be preloaded.
    /// </summary>
    public bool RequirePreload
    {
      get { return _requirePreload; }
    }

    /// <summary>
    /// Gets or sets the add-in name.
    /// </summary>
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

    /// <summary>
    /// Checks whether the reference can be satisfied from the specified add-ins.
    /// </summary>
    /// <param name="addIns">The available add-ins keyed by identity name.</param>
    /// <param name="versionFound">When this method returns, contains the located version if a matching add-in was found.</param>
    /// <returns><c>true</c> if the reference can be satisfied; otherwise, <c>false</c>.</returns>
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

    /// <summary>
    /// Creates an <see cref="AddInReference"/> from properties.
    /// </summary>
    /// <param name="properties">The properties that describe the add-in reference.</param>
    /// <param name="hintPath">The base path used to resolve referenced version files.</param>
    /// <returns>The created add-in reference.</returns>
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

    /// <summary>
    /// Parses a version string, including add-in specific placeholders.
    /// </summary>
    /// <param name="version">The version string to parse.</param>
    /// <param name="hintPath">The base path used to resolve file-based version references.</param>
    /// <returns>The parsed version.</returns>
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

    /// <summary>
    /// Initializes a new instance of the <see cref="AddInReference"/> class.
    /// </summary>
    /// <param name="name">The referenced add-in name.</param>
    public AddInReference(string name) : this(name, new Version(0, 0, 0, 0), new Version(int.MaxValue, int.MaxValue))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AddInReference"/> class.
    /// </summary>
    /// <param name="name">The referenced add-in name.</param>
    /// <param name="specificVersion">The required add-in version.</param>
    public AddInReference(string name, Version specificVersion) : this(name, specificVersion, specificVersion)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AddInReference"/> class.
    /// </summary>
    /// <param name="name">The referenced add-in name.</param>
    /// <param name="minimumVersion">The minimum accepted version.</param>
    /// <param name="maximumVersion">The maximum accepted version.</param>
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

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
      if (!(obj is AddInReference))
        return false;
      var b = (AddInReference)obj;
      return _name == b._name && _minimumVersion == b._minimumVersion && _maximumVersion == b._maximumVersion;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
      return _name.GetHashCode() ^ _minimumVersion.GetHashCode() ^ _maximumVersion.GetHashCode();
    }

    /// <inheritdoc/>
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

    /// <summary>
    /// Creates a copy of this reference.
    /// </summary>
    /// <returns>A copy of this reference.</returns>
    public AddInReference Clone()
    {
      return new AddInReference(_name, _minimumVersion, _maximumVersion);
    }

    /// <inheritdoc/>
    object ICloneable.Clone()
    {
      return Clone();
    }
  }
}
