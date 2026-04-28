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
using System.IO;
using System.Linq;
using Altaxo.Main.Services;

namespace Altaxo.Gui.Workbench
{
  /// <summary>
  /// This class defines the display binding interface, it is a factory
  /// structure, which creates IViewContents.
  /// </summary>
  public interface IDisplayBinding
  {
    /// <summary>
    /// Determines whether this binding should be preferred for the specified file.
    /// </summary>
    /// <param name="fileName">The file name to evaluate.</param>
    /// <returns><see langword="true"/> if this binding is preferred; otherwise, <see langword="false"/>.</returns>
    bool IsPreferredBindingForFile(FileName fileName);

    /// <summary>
    /// Determines whether this binding can create content for the specified file.
    /// </summary>
    /// <param name="fileName">The file name to evaluate.</param>
    /// <returns><see langword="true"/> if this binding can create content for the file; otherwise, <see langword="false"/>.</returns>
    bool CanCreateContentForFile(FileName fileName);

    /// <summary>
    /// Detects how likely the specified content can be handled by this binding.
    /// </summary>
    /// <param name="fileName">The file name to inspect.</param>
    /// <param name="fileContent">The file content stream.</param>
    /// <param name="detectedMimeType">The MIME type detected for the file.</param>
    /// <returns>A score indicating how well the content matches this binding.</returns>
    double AutoDetectFileContent(FileName fileName, Stream fileContent, string detectedMimeType);

    /// <summary>
    /// Creates a new view content object for the specified file.
    /// </summary>
    /// <param name="file">The opened file.</param>
    /// <returns>A newly created view content object.</returns>
    IViewContent? CreateContentForFile(OpenedFile file);
  }
}
