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

namespace Altaxo.AddInItems
{
  /// <summary>
  /// Describes an icon association declared in the add-in tree.
  /// </summary>
  public class IconDescriptor
  {
    private Codon _codon;

    /// <summary>
    /// Gets the icon descriptor identifier.
    /// </summary>
    public string Id
    {
      get
      {
        return _codon.Id;
      }
    }

    /// <summary>
    /// Gets the associated language, if any.
    /// </summary>
    public string Language
    {
      get
      {
        return _codon.Properties["language"];
      }
    }

    /// <summary>
    /// Gets the resource name of the icon.
    /// </summary>
    public string Resource
    {
      get
      {
        return _codon.Properties["resource"];
      }
    }

    /// <summary>
    /// Gets the file extensions associated with the icon.
    /// </summary>
    public string[] Extensions
    {
      get
      {
        return _codon.Properties["extensions"].Split(';');
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IconDescriptor"/> class.
    /// </summary>
    /// <param name="codon">The codon that defines the icon descriptor.</param>
    public IconDescriptor(Codon codon)
    {
      this._codon = codon;
    }
  }
}
