﻿// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
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
  /// Represents a path to a file.
  /// The equality operator is overloaded to compare for path equality (case insensitive, normalizing paths with '..\')
  /// </summary>
  [TypeConverter(typeof(FileNameConverter))]
  public sealed class FileName : PathName, IEquatable<FileName>
  {
    public FileName(string path)
      : base(path)
    {
    }

    /// <summary>
    /// Creates a FileName instance from the string.
    /// It is valid to pass null or an empty string to this method (in that case, a null reference will be returned).
    /// </summary>
    [return: NotNullIfNotNull("fileName")]
    public static FileName? Create(string? fileName)
    {
      if (string.IsNullOrEmpty(fileName))
        return null;
      else
        return new FileName(fileName);
    }

    [return: NotNullIfNotNull("path")]
    public static explicit operator string?(FileName? path)
    {
      return path?._normalizedPath;
    }

    /// <summary>
    /// Gets the file name (not the full path).
    /// </summary>
    /// <remarks>
    /// Corresponds to <c>System.IO.Path.GetFileName</c>
    /// </remarks>
    public string GetFileName()
    {
      return Path.GetFileName(_normalizedPath);
    }

    /// <summary>
    /// Gets the file extension.
    /// </summary>
    /// <remarks>
    /// Corresponds to <c>System.IO.Path.GetExtension</c>
    /// </remarks>
    public string GetExtension()
    {
      return Path.GetExtension(_normalizedPath);
    }

    /// <summary>
    /// Gets whether this file name has the specified extension.
    /// </summary>
    public bool HasExtension(string extension)
    {
      if (extension is null)
        throw new ArgumentNullException(nameof(extension));
      if (extension.Length == 0 || extension[0] != '.')
        throw new ArgumentException("extension must start with '.'");
      return _normalizedPath.EndsWith(extension, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Gets the file name without extension.
    /// </summary>
    /// <remarks>
    /// Corresponds to <c>System.IO.Path.GetFileNameWithoutExtension</c>
    /// </remarks>
    public string GetFileNameWithoutExtension()
    {
      return Path.GetFileNameWithoutExtension(_normalizedPath);
    }

    #region Equals and GetHashCode implementation

    public override bool Equals(object? obj)
    {
      return Equals(obj as FileName);
    }

    public bool Equals(FileName? other)
    {
      if (other is null)
        return false;
      else
        return string.Equals(_normalizedPath, other._normalizedPath, StringComparison.OrdinalIgnoreCase);
    }

    public override int GetHashCode()
    {
      return StringComparer.OrdinalIgnoreCase.GetHashCode(_normalizedPath);
    }

    public static bool operator ==(FileName left, FileName right)
    {
      if (ReferenceEquals(left, right))
        return true;
      if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
        return false;
      return left.Equals(right);
    }

    public static bool operator !=(FileName left, FileName right)
    {
      return !(left == right);
    }

    [ObsoleteAttribute("Warning: comparing FileName with string results in case-sensitive comparison")]
    public static bool operator ==(FileName left, string right)
    {
      return (string?)left == right;
    }

    [ObsoleteAttribute("Warning: comparing FileName with string results in case-sensitive comparison")]
    public static bool operator !=(FileName left, string right)
    {
      return (string?)left != right;
    }

    [ObsoleteAttribute("Warning: comparing FileName with string results in case-sensitive comparison")]
    public static bool operator ==(string left, FileName right)
    {
      return left == (string?)right;
    }

    [ObsoleteAttribute("Warning: comparing FileName with string results in case-sensitive comparison")]
    public static bool operator !=(string left, FileName right)
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
      return File.Exists(_normalizedPath);
    }

    /// <summary>
    /// Gets the name of the directory from the given file name.
    /// Throws an <see cref="InvalidOperationException"/> if a directory name could not be retrieved.
    /// </summary>
    /// <param name="fullFileName">Full name of the file.</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">Unable to get directory name from file name '{fullFileName}'</exception>
    public static string GetDirectoryName(string fullFileName)
    {
      return Path.GetDirectoryName(fullFileName) ?? throw new InvalidOperationException($"Unable to get directory name from file name '{fullFileName}'");
    }
  }

  public class FileNameConverter : TypeConverter
  {
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
      return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    }

    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
    {
      return destinationType == typeof(FileName) || base.CanConvertTo(context, destinationType);
    }

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
      if (value is string s)
      {
        return FileName.Create(s);
      }
      return base.ConvertFrom(context, culture, value);
    }

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
