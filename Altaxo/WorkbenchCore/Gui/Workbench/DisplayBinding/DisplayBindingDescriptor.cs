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
  public class DisplayBindingDescriptor
  {
    private object? _binding;
    private bool _isSecondary;
    private Codon? _codon;

    /// <summary>
    /// Gets the IDisplayBinding or ISecondaryDisplayBinding if it is already loaded,
    /// otherwise returns null.
    /// </summary>
    public object? GetLoadedBinding()
    {
      return _binding;
    }

    public IDisplayBinding? Binding
    {
      get
      {
        if (_codon != null && _binding == null)
        {
          _binding = _codon.AddIn.CreateObject(_codon.Properties["class"]);
        }
        return _binding as IDisplayBinding;
      }
    }

    public ISecondaryDisplayBinding? SecondaryBinding
    {
      get
      {
        if (_codon != null && _binding == null)
        {
          _binding = _codon.AddIn.CreateObject(_codon.Properties["class"]);
        }
        return _binding as ISecondaryDisplayBinding;
      }
    }

    public bool IsSecondary
    {
      get { return _isSecondary; }
    }

    public string? Id { get; set; }
    public string? Title { get; set; }
    public string? FileNameRegex { get; set; }

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

    public DisplayBindingDescriptor(IDisplayBinding binding)
    {
      if (binding is null)
        throw new ArgumentNullException(nameof(binding));

      _isSecondary = false;
      this._binding = binding;
    }

    public DisplayBindingDescriptor(ISecondaryDisplayBinding binding)
    {
      if (binding is null)
        throw new ArgumentNullException(nameof(binding));

      _isSecondary = true;
      this._binding = binding;
    }

    /// <summary>
    /// Gets if the display binding can possibly open the file.
    /// If this method returns false, it cannot open it; if the method returns
    /// true, it *might* open it.
    /// Call Binding.CanCreateContentForFile() to know for sure if the binding
    /// will open the file.
    /// </summary>
    /// <remarks>
    /// This method is used to skip loading addins like the ResourceEditor which cannot
    /// attach to a certain file name for sure.
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

    public override string ToString()
    {
      return string.Format("[DisplayBindingDescriptor Id={1} Binding={0}]", _binding, Id);
    }
  }
}
