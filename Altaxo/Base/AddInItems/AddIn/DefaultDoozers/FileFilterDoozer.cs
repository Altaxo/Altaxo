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
using Altaxo.Main.Services;

namespace Altaxo.AddInItems
{
  /// <summary>
  /// Creates file filter entries for OpenFileDialogs or SaveFileDialogs.
  /// </summary>
  /// <attribute name="name" use="required">
  /// The name of the file filter entry.
  /// </attribute>
  /// <attribute name="extensions" use="required">
  /// The extensions associated with this file filter entry.
  /// </attribute>
  /// <usage>Only in /SharpDevelop/Workbench/FileFilter</usage>
  /// <returns>
  /// <see cref="FileFilterDescriptor"/> in the format "name|extensions".
  /// </returns>
  public class FileFilterDoozer : IDoozer
  {
    /// <inheritdoc/>
    public bool HandleConditions
    {
      get
      {
        return false;
      }
    }

    /// <inheritdoc/>
    public object BuildItem(BuildItemArgs args)
    {
      Codon codon = args.Codon;
      return new FileFilterDescriptor
      {
        Name = StringParser.Parse(codon.Properties["name"]),
        Extensions = codon.Properties["extensions"],
        MimeType = codon.Properties["mimeType"]
      };
    }
  }

  /// <summary>
  /// Describes a file filter entry for file dialogs.
  /// </summary>
  public sealed class FileFilterDescriptor
  {
    /// <summary>
    /// Gets or sets the display name of the filter.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the file extensions that belong to the filter.
    /// </summary>
    public string Extensions { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the MIME type associated with the filter.
    /// </summary>
    public string MimeType { get; set; } = string.Empty;

    /// <summary>
    /// Gets whether this descriptor matches the specified file extension.
    /// </summary>
    /// <param name="extension">The file extension starting with <c>.</c>.</param>
    /// <returns><see langword="true"/> if the descriptor contains the specified extension; otherwise, <see langword="false"/>.</returns>
    public bool ContainsExtension(string extension)
    {
      if (string.IsNullOrEmpty(extension))
        return false;
      int index = Extensions.IndexOf("*" + extension, StringComparison.OrdinalIgnoreCase);
      int matchLength = index + extension.Length + 1;
      if (index < 0 || matchLength > Extensions.Length)
        return false;
      return matchLength == Extensions.Length || Extensions[matchLength] == ';';
    }

    /// <inheritdoc/>
    public override string ToString()
    {
      return Name + "|" + Extensions;
    }
  }
}
