#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Text.GuiModels;

namespace Altaxo.Gui.Text.Viewing
{
  public interface ITextDocumentView
  {
    ITextDocumentController Controller { set; }

    /// <summary>
    /// Flag that is set if the view is in initialization mode. If in initialization mode, it should not fire events etc, not render the document.
    /// </summary>
    bool IsInInitializationMode { set; }

    /// <summary>
    /// Sets the name of the document, and the local images. This function must be called every time the document name has changed.
    /// </summary>
    /// <param name="documentName">The full name of the document.</param>
    /// <param name="localImages">The dictionary of local images.</param>
    void SetDocumentNameAndLocalImages(string documentName, IReadOnlyDictionary<string, Altaxo.Graph.MemoryStreamImageProxy> localImages);

    string SourceText { get; set; }

    event EventHandler SourceTextChanged;

    string StyleName { set; }
    bool IsViewerSelected { get; set; }
    ViewerConfiguration WindowConfiguration { get; set; }
    double FractionOfEditorWindow { get; set; }

    bool IsLineNumberingEnabled { set; }
    bool IsWordWrappingEnabled { set; }
    bool IsSpellCheckingEnabled { set; }
    bool IsHyphenationEnabled { set; }
    bool IsFoldingEnabled { set; }
    string HighlightingStyle { set; }

    /// <summary>
    /// Sets a value indicating whether this the outline window is visible.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the outline window is visible; otherwise, <c>false</c>.
    /// </value>
    bool IsOutlineWindowVisible { get; set; }

    /// <summary>
    /// Sets the width of the outline window. A value of <see cref="double.NaN"/> indicates
    /// that the width is determined automatically by the contents of the outline window.
    /// </summary>
    /// <value>
    /// The relative width of the outline window (relative to the available width)
    /// </value>
    double OutlineWindowRelativeWidth { set; }

    /// <summary>
    /// Sets the culture for this document. This is important for instance for spell checking.
    /// </summary>
    CultureInfo DocumentCulture { set; }

    void PrintShowDialog();

    /// <summary>
    /// Inserts the provided markdown source text at the current caret position.
    /// </summary>
    /// <param name="text">The text to insert.</param>
    void InsertSourceTextAtCaretPosition(string text);
  }

  public interface ITextDocumentController
  {
    string InsertImageInDocumentAndGetUrl(string fileName);

    string InsertImageInDocumentAndGetUrl(System.IO.Stream memoryStream, string fileExtension);

    /// <summary>
    /// Tests if the provided file name could be accepted as an image.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <returns>True if the name could be accepted; false otherwise.</returns>
    bool CanAcceptImageFileName(string fileName);

    void EhIsViewerSelectedChanged(bool isViewerSelected);

    void EhViewerConfigurationChanged(ViewerConfiguration windowConfiguration);

    void EhFractionOfEditorWindowChanged(double fractionOfEditor);

    void EhReferencedImageUrlsChanged(IEnumerable<(string Url, int urlSpanStart, int urlSpanEnd)> referencedLocalImages);

    void EhBeforeCompleteRendering();

    /// <summary>
    /// Determines whether this controller can accept the current data of the clipboard. If the return value is true,
    /// the controller is used to paste the clipboard data (via <see cref="Paste"/>; otherwise, pasting is delegated further down to the source editor.
    /// </summary>
    /// <returns>
    ///   <c>true</c> if this instance can accept the current data of the clipboard; otherwise, <c>false</c>.
    /// </returns>
    bool CanPaste();

    /// <summary>
    /// Pastes data from the clipboard in the document managed by this controller.
    /// </summary>
    /// <returns>True if pasting clipboard data was successfull; otherwise, <c>false</c>.</returns>
    bool Paste();
  }
}
