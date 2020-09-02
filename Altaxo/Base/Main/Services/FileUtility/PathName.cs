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
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Altaxo.Main.Services
{
  /// <summary>
  /// Represents a path to a file or directory.
  /// </summary>
  public abstract class PathName
  {
    protected readonly string _normalizedPath;

    protected PathName(string path)
    {
      if (path is null)
        throw new ArgumentNullException(nameof(path));
      if (path.Length == 0)
        throw new ArgumentException("The empty string is not a valid path");
      _normalizedPath = FileUtility.NormalizePath(path);
    }

    protected PathName(PathName path)
    {
      if (path is null)
        throw new ArgumentNullException("path");
      _normalizedPath = path._normalizedPath;
    }
    /*
    [return: NotNullIfNotNull("path")]
    public static explicit operator string?(PathName? path)
    {
      return path?._normalizedPath;
    }
    */

    [return: MaybeNull]
    [return: NotNullIfNotNull("path")]
    public static implicit operator string(PathName? path)
    {
      return path?._normalizedPath;
    }


    [return: MaybeNull]
    [return: NotNullIfNotNull("path")]
    public static string ToStringPath(PathName? path)
    {
      return path?._normalizedPath;
    }

    public override string ToString()
    {
      return _normalizedPath;
    }

    /// <summary>
    /// Gets whether this path is relative.
    /// </summary>
    public bool IsRelative
    {
      get { return !Path.IsPathRooted(_normalizedPath); }
    }

    /// <summary>
    /// Gets the directory name.
    /// </summary>
    /// <remarks>
    /// Corresponds to <c>System.IO.Path.GetDirectoryName</c>
    /// </remarks>
    public DirectoryName GetParentDirectory()
    {
      DirectoryName? result;
      if (_normalizedPath.Length < 2 || _normalizedPath[1] != ':')
        result = DirectoryName.Create(Path.Combine(_normalizedPath, ".."));
      else
        result = DirectoryName.Create(Path.GetDirectoryName(_normalizedPath) ?? string.Empty);

      if (result is null)
        throw new InvalidOperationException($"Can not get parent directory of {_normalizedPath}");

      return result;
    }

    /// <summary>
    /// Returns true if this directory exists in the file system.
    /// </summary>
    /// <returns>True if this directory exists in the file system.</returns>
    public abstract bool Exists();

    /// <summary>
    /// Creates a path from a name that could be a file or a folder name. If a file with the given name exists in the file system,
    /// it is decided that this is a file name, and a <see cref="FileName"/> is returned. If a folder with the given name exists
    /// in the file system, then it is decided that this is a folder, and a <see cref="DirectoryName"/> is returned.
    /// If neither of the two cases above applies, null is returned.
    /// </summary>
    /// <param name="fileOrFolderName">Name of the file or folder.</param>
    /// <returns>Either a <see cref="FileName"/>, a <see cref="DirectoryName"/>, or null if neither a file nor folder with this name exists.</returns>
    public static PathName? CreateFromExisting(string fileOrFolderName)
    {
      try
      {
        if (System.IO.File.Exists(fileOrFolderName))
          return new FileName(fileOrFolderName);
      }
      catch (Exception)
      {
      }

      try
      {
        if (System.IO.Directory.Exists(fileOrFolderName))
          return new DirectoryName(fileOrFolderName);
      }
      catch (Exception)
      {
      }
      return null;
    }
  }
}
