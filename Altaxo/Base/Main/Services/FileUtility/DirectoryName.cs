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
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;

namespace Altaxo.Main.Services
{
  /// <summary>
  /// Represents a path to a directory.
  /// The equality operator is overloaded to compare for path equality (case insensitive, normalizing paths with '..\')
  /// </summary>
  [TypeConverter(typeof(DirectoryNameConverter))]
  public sealed class DirectoryName : PathName
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DirectoryName"/> class.
    /// </summary>
    /// <param name="path">The directory path.</param>
    public DirectoryName(string path)
      : base(path)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DirectoryName"/> class from an existing directory name.
    /// </summary>
    /// <param name="path">The directory name to copy.</param>
    [Obsolete("The input already is a DirectoryName")]
    public DirectoryName(DirectoryName path)
      : base(path)
    {
    }

    /// <summary>
    /// Creates a DirectoryName instance from the string.
    /// It is valid to pass null or an empty string to this method (in that case, a null reference will be returned).
    /// </summary>
    /// <param name="directoryName">The directory path.</param>
    /// <returns>A <see cref="DirectoryName"/> instance, or <see langword="null"/> if <paramref name="directoryName"/> is null or empty.</returns>
    [return: NotNullIfNotNull("directoryName")]
    public static DirectoryName? Create(string directoryName)
    {
      if (string.IsNullOrEmpty(directoryName))
        return null!;
      else
        return new DirectoryName(directoryName);
    }

    /// <summary>
    /// Returns the specified directory name instance.
    /// </summary>
    /// <param name="directoryName">The directory name instance.</param>
    /// <returns>The provided <paramref name="directoryName"/>.</returns>
    [Obsolete("The input already is a DirectoryName")]
    public static DirectoryName Create(DirectoryName directoryName)
    {
      return directoryName;
    }

    /// <summary>
    /// Combines this directory name with a relative path.
    /// </summary>
    /// <param name="relativePath">The relative directory path to combine.</param>
    /// <returns>The combined directory path, or <see langword="null"/> if <paramref name="relativePath"/> is <see langword="null"/>.</returns>
    [return: NotNullIfNotNull("relativePath")]
    public DirectoryName? Combine(DirectoryName? relativePath)
    {
      if (relativePath is null)
        return null;
      return DirectoryName.Create(Path.Combine(_normalizedPath, relativePath!));
    }

    /// <summary>
    /// Combines this directory name with a relative path.
    /// </summary>
    /// <param name="relativePath">The relative file path to combine.</param>
    /// <returns>The combined file path, or <see langword="null"/> if <paramref name="relativePath"/> is <see langword="null"/>.</returns>
    [return: NotNullIfNotNull("relativePath")]
    public FileName? Combine(FileName? relativePath)
    {
      if (relativePath is { } rpath)
        return FileName.Create(Path.Combine(_normalizedPath, rpath!));
      else
        return null;

    }

    /// <summary>
    /// Combines this directory name with a relative path.
    /// </summary>
    /// <param name="relativeFileName">The relative file name to combine.</param>
    /// <returns>The combined file path, or <see langword="null"/> if <paramref name="relativeFileName"/> is <see langword="null"/>.</returns>
    [return: NotNullIfNotNull("relativeFileName")]
    public FileName? CombineFile(string? relativeFileName)
    {
      if (relativeFileName is null)
        return null;
      return FileName.Create(Path.Combine(_normalizedPath, relativeFileName));
    }

    /// <summary>
    /// Combines this directory name with a relative path.
    /// </summary>
    /// <param name="relativeDirectoryName">The relative directory name to combine.</param>
    /// <returns>The combined directory path, or <see langword="null"/> if <paramref name="relativeDirectoryName"/> is <see langword="null"/>.</returns>
    [return: NotNullIfNotNull("relativeDirectoryName")]
    public DirectoryName? CombineDirectory(string? relativeDirectoryName)
    {
      if (relativeDirectoryName is null)
        return null;
      return DirectoryName.Create(Path.Combine(_normalizedPath, relativeDirectoryName));
    }

    /// <summary>
    /// Converts the specified absolute path into a relative path (relative to <c>this</c>).
    /// </summary>
    /// <param name="path">The absolute directory path.</param>
    /// <returns>The relative directory path, or <see langword="null"/> if <paramref name="path"/> is <see langword="null"/>.</returns>
    [return: NotNullIfNotNull("path")]
    public DirectoryName? GetRelativePath(DirectoryName? path)
    {
      if (path is null)
        return null;
      return DirectoryName.Create(FileUtility.GetRelativePath(_normalizedPath, path!));
    }

    /// <summary>
    /// Converts the specified absolute path into a relative path (relative to <c>this</c>).
    /// </summary>
    /// <param name="path">The absolute file path.</param>
    /// <returns>The relative file path, or <see langword="null"/> if <paramref name="path"/> is <see langword="null"/>.</returns>
    [return: NotNullIfNotNull("path")]
    public FileName? GetRelativePath(FileName? path)
    {
      if (path is null)
        return null;
      return FileName.Create(FileUtility.GetRelativePath(_normalizedPath, path!));
    }

    /// <summary>
    /// Gets the directory name as a string, including a trailing backslash.
    /// </summary>
    /// <returns>The directory name with a trailing backslash.</returns>
    public string ToStringWithTrailingBackslash()
    {
      if (_normalizedPath.EndsWith("\\", StringComparison.Ordinal))
        return _normalizedPath; // trailing backslash exists in normalized version for root of drives ("C:\")
      else
        return _normalizedPath + "\\";
    }

    /// <summary>
    /// Converts a <see cref="DirectoryName"/> to its string path.
    /// </summary>
    /// <param name="path">The directory path.</param>
    /// <returns>The normalized string path.</returns>
    [return: NotNullIfNotNull("path")]
    public static implicit operator string?(DirectoryName? path)
    {
      return path?._normalizedPath;
    }

    #region Equals and GetHashCode implementation

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
      return Equals(obj as DirectoryName);
    }

    /// <summary>
    /// Determines whether this instance and another directory name are equal.
    /// </summary>
    /// <param name="other">The other directory name to compare with this instance.</param>
    /// <returns><see langword="true"/> if the directory names are equal; otherwise, <see langword="false"/>.</returns>
    public bool Equals(DirectoryName? other)
    {
      if (other is null)
        return false;
      else
        return string.Equals(_normalizedPath, other._normalizedPath, StringComparison.OrdinalIgnoreCase);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
      return StringComparer.OrdinalIgnoreCase.GetHashCode(_normalizedPath);
    }

    /// <summary>
    /// Determines whether two directory names are equal.
    /// </summary>
    /// <param name="left">The left directory name.</param>
    /// <param name="right">The right directory name.</param>
    public static bool operator ==(DirectoryName? left, DirectoryName? right)
    {
      return ReferenceEquals(left, right) || (left is not null && right is not null && left.Equals(right));
    }

    /// <summary>
    /// Determines whether two directory names are not equal.
    /// </summary>
    /// <param name="left">The left directory name.</param>
    /// <param name="right">The right directory name.</param>
    public static bool operator !=(DirectoryName left, DirectoryName right)
    {
      return !(left == right);
    }

    /// <summary>
    /// Determines whether a directory name and a string path are equal.
    /// </summary>
    /// <param name="left">The left directory name.</param>
    /// <param name="right">The right string path.</param>
    [ObsoleteAttribute("Warning: comparing DirectoryName with string results in case-sensitive comparison")]
    public static bool operator ==(DirectoryName left, string right)
    {
      return (string?)left == right;
    }

    /// <summary>
    /// Determines whether a directory name and a string path are not equal.
    /// </summary>
    /// <param name="left">The left directory name.</param>
    /// <param name="right">The right string path.</param>
    [ObsoleteAttribute("Warning: comparing DirectoryName with string results in case-sensitive comparison")]
    public static bool operator !=(DirectoryName left, string right)
    {
      return (string?)left != right;
    }

    /// <summary>
    /// Determines whether a string path and a directory name are equal.
    /// </summary>
    /// <param name="left">The left string path.</param>
    /// <param name="right">The right directory name.</param>
    [ObsoleteAttribute("Warning: comparing DirectoryName with string results in case-sensitive comparison")]
    public static bool operator ==(string left, DirectoryName right)
    {
      return left == (string?)right;
    }

    /// <summary>
    /// Determines whether a string path and a directory name are not equal.
    /// </summary>
    /// <param name="left">The left string path.</param>
    /// <param name="right">The right directory name.</param>
    [ObsoleteAttribute("Warning: comparing DirectoryName with string results in case-sensitive comparison")]
    public static bool operator !=(string left, DirectoryName right)
    {
      return left != (string?)right;
    }

    #endregion Equals and GetHashCode implementation

    /// <summary>
    /// Returns true if this directory exists in the file system.
    /// </summary>
    /// <returns>True if this directory exists in the file system.</returns>
    public override bool Exists()
    {
      return Directory.Exists(_normalizedPath);
    }

  }

  /// <summary>
  /// Converts between strings and <see cref="DirectoryName"/> instances.
  /// </summary>
  public class DirectoryNameConverter : TypeConverter
  {
    /// <inheritdoc/>
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
      return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    }

    /// <inheritdoc/>
    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
    {
      return destinationType == typeof(DirectoryName) || base.CanConvertTo(context, destinationType);
    }

    /// <inheritdoc/>
    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
      if (value is string s)
      {
        return DirectoryName.Create(s);
      }
      return base.ConvertFrom(context, culture, value);
    }

    /// <inheritdoc/>
    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture,
                                     object? value, Type destinationType)
    {
      if (destinationType == typeof(string))
      {
        return value?.ToString() ?? string.Empty;
      }
      return base.ConvertTo(context, culture, value, destinationType);
    }
  }
}
