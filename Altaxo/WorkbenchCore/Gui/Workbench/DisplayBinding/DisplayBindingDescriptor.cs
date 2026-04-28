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
using System.Text.RegularExpressions;
using Altaxo.AddInItems;
using Altaxo.Main.Services;

namespace Altaxo.Gui.Workbench
{
  /// <summary>
  /// Describes a display binding defined by an add-in or by an existing binding instance.
  /// </summary>
  public class DisplayBindingDescriptor
  {
    private object? _binding;
    private bool _isSecondary;
    private Codon? _codon;

    /// <summary>
    /// Gets the loaded binding, if any.
    /// </summary>
    /// <returns>The loaded binding, or <see langword="null"/> if it has not been created yet.</returns>
    public object? GetLoadedBinding()
    {
      return _binding;
    }

    /// <summary>
    /// Gets the primary display binding instance.
    /// </summary>
    public IDisplayBinding? Binding
    {
      get
      {
        if (_codon is not null && _binding is null)
        {
          _binding = _codon.AddIn.CreateObject(_codon.Properties["class"]);
        }
        return _binding as IDisplayBinding;
      }
    }

    /// <summary>
    /// Gets the secondary display binding instance.
    /// </summary>
    public ISecondaryDisplayBinding? SecondaryBinding
    {
      get
      {
        if (_codon is not null && _binding is null)
        {
          _binding = _codon.AddIn.CreateObject(_codon.Properties["class"]);
        }
        return _binding as ISecondaryDisplayBinding;
      }
    }

    /// <summary>
    /// Gets a value indicating whether this descriptor refers to a secondary binding.
    /// </summary>
    public bool IsSecondary
    {
      get { return _isSecondary; }
    }

    /// <summary>
    /// Gets or sets the binding identifier.
    /// </summary>
    public string? Id { get; set; }
    /// <summary>
    /// Gets or sets the display title.
    /// </summary>
    public string? Title { get; set; }
    /// <summary>
    /// Gets or sets the file name pattern used to match files.
    /// </summary>
    public string? FileNameRegex { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DisplayBindingDescriptor"/> class from an add-in codon.
    /// </summary>
    /// <param name="codon">The codon that describes the binding.</param>
    public DisplayBindingDescriptor(Codon codon)
    {
      if (codon is null)
        throw new ArgumentNullException(nameof(codon));

      _isSecondary = codon.Properties["type"] == "Secondary";
      if (!_isSecondary && codon.Properties["type"] != "" && codon.Properties["type"] != "Primary")
        MessageService.ShowWarning("Unknown display binding type: " + codon.Properties["type"]);

      this._codon = codon;
      Id = codon.Id;

      string title = codon.Properties["title"];
      if (string.IsNullOrEmpty(title))
        Title = codon.Id;
      else
        Title = title;

      FileNameRegex = codon.Properties["fileNamePattern"];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DisplayBindingDescriptor"/> class for a primary binding.
    /// </summary>
    /// <param name="binding">The primary display binding.</param>
    public DisplayBindingDescriptor(IDisplayBinding binding)
    {
      if (binding is null)
        throw new ArgumentNullException(nameof(binding));

      _isSecondary = false;
      this._binding = binding;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DisplayBindingDescriptor"/> class for a secondary binding.
    /// </summary>
    /// <param name="binding">The secondary display binding.</param>
    public DisplayBindingDescriptor(ISecondaryDisplayBinding binding)
    {
      if (binding is null)
        throw new ArgumentNullException(nameof(binding));

      _isSecondary = true;
      this._binding = binding;
    }

    /// <summary>
    /// Determines whether the binding can possibly open the specified file.
    /// </summary>
    /// <param name="fileName">The file name to inspect.</param>
    /// <returns><see langword="true"/> if the binding might open the file; otherwise, <see langword="false"/>.</returns>
    /// <remarks>
    /// This method is used to skip loading add-ins like the ResourceEditor, which cannot
    /// determine with certainty whether they can attach to a certain file name.
    /// </remarks>
    public bool CanOpenFile(string fileName)
    {
      string? fileNameRegex = StringParser.Parse(FileNameRegex);
      if (fileNameRegex is null || fileNameRegex.Length == 0) // no regex specified
        return true;
      if (fileName is null) // regex specified but file has no name
        return false;
      return Regex.IsMatch(fileName, fileNameRegex, RegexOptions.IgnoreCase);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
      return string.Format("[DisplayBindingDescriptor Id={1} Binding={0}]", _binding, Id);
    }
  }
}
