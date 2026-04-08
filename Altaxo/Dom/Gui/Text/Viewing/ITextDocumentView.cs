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
using Altaxo.Drawing;
using Altaxo.Text.GuiModels;

namespace Altaxo.Gui.Text.Viewing
{
  /// <summary>
  /// Defines the view used to display and edit a text document.
  /// </summary>
  public interface ITextDocumentView
  {
    /// <summary>
    /// Sets the controller that manages this view.
    /// </summary>
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
    void SetDocumentNameAndLocalImages(string documentName, IReadOnlyDictionary<string, MemoryStreamImageProxy> localImages);

    /// <summary>
    /// Gets or sets the source text shown in the editor.
    /// </summary>
    string SourceText { get; set; }

    /// <summary>
    /// Occurs when the source text changes.
    /// </summary>
    event EventHandler SourceTextChanged;

    /// <summary>
    /// Sets the name of the active style.
    /// </summary>
    string StyleName { set; }
    /// <summary>
    /// Gets or sets a value indicating whether the viewer pane is selected.
    /// </summary>
    bool IsViewerSelected { get; set; }
    /// <summary>
    /// Gets or sets the viewer window configuration.
    /// </summary>
    ViewerConfiguration WindowConfiguration { get; set; }
    /// <summary>
    /// Gets or sets the fraction of the window used by the editor.
    /// </summary>
    double FractionOfEditorWindow { get; set; }

    /// <summary>
    /// Sets a value indicating whether line numbering is enabled.
    /// </summary>
    bool IsLineNumberingEnabled { set; }
    /// <summary>
    /// Sets a value indicating whether word wrapping is enabled.
    /// </summary>
    bool IsWordWrappingEnabled { set; }
    /// <summary>
    /// Sets a value indicating whether spell checking is enabled.
    /// </summary>
    bool IsSpellCheckingEnabled { set; }
    /// <summary>
    /// Sets a value indicating whether hyphenation is enabled.
    /// </summary>
    bool IsHyphenationEnabled { set; }
    /// <summary>
    /// Sets a value indicating whether code folding is enabled.
    /// </summary>
    bool IsFoldingEnabled { set; }
    /// <summary>
    /// Sets the highlighting style.
    /// </summary>
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

    /// <summary>
    /// Shows a print dialog and prints the current document if confirmed.
    /// </summary>
    void PrintShowDialog();

    /// <summary>
    /// Inserts the provided markdown source text at the current caret position.
    /// </summary>
    /// <param name="text">The text to insert.</param>
    void InsertSourceTextAtCaretPosition(string text);
  }

  /// <summary>
  /// Defines the controller operations required by a text document view.
  /// </summary>
  public interface ITextDocumentController
  {
    /// <summary>
    /// Inserts an image file into the document and returns the inserted image URL.
    /// </summary>
    /// <param name="fileName">The image file name.</param>
    /// <returns>The URL that references the inserted image.</returns>
    string InsertImageInDocumentAndGetUrl(string fileName);

    /// <summary>
    /// Inserts an image stream into the document and returns the inserted image URL.
    /// </summary>
    /// <param name="memoryStream">The image stream.</param>
    /// <param name="fileExtension">The image file extension.</param>
    /// <returns>The URL that references the inserted image.</returns>
    string InsertImageInDocumentAndGetUrl(System.IO.Stream memoryStream, string fileExtension);

    /// <summary>
    /// Tests if the provided file name could be accepted as an image.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <returns>True if the name could be accepted; false otherwise.</returns>
    bool CanAcceptImageFileName(string fileName);

    /// <summary>
    /// Notifies the controller that the viewer selection state changed.
    /// </summary>
    /// <param name="isViewerSelected"><see langword="true"/> if the viewer is selected; otherwise, <see langword="false"/>.</param>
    void EhIsViewerSelectedChanged(bool isViewerSelected);

    /// <summary>
    /// Notifies the controller that the viewer configuration changed.
    /// </summary>
    /// <param name="windowConfiguration">The new viewer configuration.</param>
    void EhViewerConfigurationChanged(ViewerConfiguration windowConfiguration);

    /// <summary>
    /// Notifies the controller that the editor window fraction changed.
    /// </summary>
    /// <param name="fractionOfEditor">The new editor window fraction.</param>
    void EhFractionOfEditorWindowChanged(double fractionOfEditor);

    /// <summary>
    /// Notifies the controller that the referenced local image URLs changed.
    /// </summary>
    /// <param name="referencedLocalImages">The referenced local image URLs and their span positions.</param>
    void EhReferencedImageUrlsChanged(IEnumerable<(string Url, int urlSpanStart, int urlSpanEnd)> referencedLocalImages);

    /// <summary>
    /// Notifies the controller immediately before rendering completes.
    /// </summary>
    void EhBeforeCompleteRendering();

    /// <summary>
    /// Determines whether this controller can accept the current clipboard data. If the return value is <see langword="true"/>,
    /// the controller is used to paste the clipboard data via <see cref="Paste"/>; otherwise, pasting is delegated to the source editor.
    /// </summary>
    /// <returns>
    ///   <c>true</c> if this instance can accept the current data of the clipboard; otherwise, <c>false</c>.
    /// </returns>
    bool CanPaste();

    /// <summary>
    /// Pastes data from the clipboard in the document managed by this controller.
    /// </summary>
    /// <returns><see langword="true"/> if pasting clipboard data was successful; otherwise, <see langword="false"/>.</returns>
    bool Paste();
  }
}
