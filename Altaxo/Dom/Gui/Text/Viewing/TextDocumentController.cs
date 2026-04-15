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

#nullable disable warnings
using System;
using System.Collections.Generic;
using Altaxo.Drawing;
using Altaxo.Gui.Workbench;
using Altaxo.Main;
using Altaxo.Text;
using Altaxo.Text.GuiModels;

namespace Altaxo.Gui.Text.Viewing
{
  /// <summary>
  /// Controls the view of a text document.
  /// </summary>
  [UserControllerForObject(typeof(TextDocument))]
  [UserControllerForObject(typeof(Altaxo.Text.GuiModels.TextDocumentViewOptions))]
  [ExpectedTypeOfView(typeof(ITextDocumentView))]
  public class TextDocumentController : AbstractViewContent, IDisposable, IMVCANController, ITextDocumentController
  {
    /// <summary>
    /// The view associated with this controller.
    /// </summary>
    protected ITextDocumentView _view;

    private Altaxo.Text.GuiModels.TextDocumentViewOptions _options;

    /// <summary>
    /// Weak event handler for tunneled document events.
    /// </summary>
    protected WeakActionHandler<object, object, TunnelingEventArgs> _weakEventHandlerForDoc_TunneledEvent;

    /// <summary>
    /// Gets the text document managed by this controller.
    /// </summary>
    public TextDocument TextDocument { get { return _options?.Document; } }

    /// <summary>
    /// Initializes a new instance of the <see cref="TextDocumentController"/> class.
    /// </summary>
    public TextDocumentController()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TextDocumentController"/> class.
    /// </summary>
    /// <param name="doc">The document to control.</param>
    public TextDocumentController(TextDocument doc)
    {
      InitializeDocument(doc);
    }

    /// <inheritdoc/>
    public bool InitializeDocument(params object[] args)
    {
      if (args is null || args.Length == 0)
        return false;

      TextDocumentViewOptions newOptions = null;

      if (args[0] is TextDocumentViewOptions notesViewOptions)
      {
        newOptions = notesViewOptions;
      }
      else if (args[0] is TextDocument notesDoc)
      {
        newOptions = new TextDocumentViewOptions(notesDoc);
      }

      if (newOptions is null)
        return false; // not successfull

      if (newOptions.Document is null)
        throw new InvalidProgramException("The provided options do not contain any document");

      if (_options?.Document is not null && !object.ReferenceEquals(TextDocument, _options.Document))
      {
        throw new InvalidProgramException("The already initialized document and the document in the option class are not identical");
      }

      InternalInitializeDocument(newOptions);

      return true;
    }

    /// <summary>
    /// Shows the print dialog for the current document.
    /// </summary>
    public void PrintShowDialog()
    {
      _view?.PrintShowDialog();
    }

    /// <inheritdoc/>
    public UseDocument UseDocumentCopy
    {
      set { }
    }

    /// <summary>
    /// Initializes the controller with the provided view options.
    /// </summary>
    /// <param name="options">The view options.</param>
    protected void InternalInitializeDocument(TextDocumentViewOptions options)
    {
      if (options?.Document is null)
        throw new ArgumentNullException("No document stored inside options");

      _options = options;

      Title = GetTitleFromDocumentName(TextDocument);

      var textDocument = TextDocument;
      {
        // Attention: use LOCAL variables here in order to avoid references to the controller!
        _weakEventHandlerForDoc_TunneledEvent?.Remove();
        textDocument.TunneledEvent += new WeakActionHandler<object, object, Altaxo.Main.TunnelingEventArgs>(EhDocumentTunneledEvent, textDocument, nameof(textDocument.TunneledEvent));
      }
    }

    private void EhDocumentTunneledEvent(object sender, object originalSource, TunnelingEventArgs e)
    {
      if (e is Altaxo.Main.DocumentPathChangedEventArgs && _view is not null)
      {
        _view.SetDocumentNameAndLocalImages(TextDocument.Name, TextDocument.Images);
        Title = GetTitleFromDocumentName(TextDocument);
      }

      if (e is DisposeEventArgs && object.ReferenceEquals(originalSource, TextDocument))
      {
        Current.Workbench.CloseContent(this);
      }
    }

    private static string GetTitleFromDocumentName(TextDocument doc)
    {
      if (!Altaxo.Main.ProjectFolder.IsValidFolderName(doc.Name))
        return doc.Name;
      else
        return doc.Name + "FolderNotes";
    }

    /// <summary>
    /// Initializes the view state.
    /// </summary>
    /// <param name="initData"><see langword="true"/> to initialize data; otherwise, <see langword="false"/>.</param>
    protected void Initialize(bool initData)
    {
      if (initData)
      {
      }
      if (_view is not null)
      {
        _view.IsInInitializationMode = true;
        _view.SetDocumentNameAndLocalImages(TextDocument.Name, TextDocument.Images);
        _view.SourceText = TextDocument.SourceText;
        _view.StyleName = TextDocument.StyleName;

        _view.IsViewerSelected = _options.IsViewerSelected;
        _view.WindowConfiguration = _options.WindowConfiguration;
        _view.FractionOfEditorWindow = _options.FractionOfSourceEditorWindowVisible;
        _view.IsLineNumberingEnabled = _options.IsLineNumberingEnabled ?? _options.Document.GetPropertyValue(TextDocumentViewOptions.PropertyKeyIsLineNumberingEnabled, () => true);
        _view.IsWordWrappingEnabled = _options.IsWordWrappingEnabled ?? _options.Document.GetPropertyValue(TextDocumentViewOptions.PropertyKeyIsWordWrappingEnabled, () => true);
        _view.DocumentCulture = TextDocument.GetPropertyValue(Altaxo.Settings.CultureSettings.PropertyKeyDocumentCulture, () => Current.PropertyService.GetValue(Altaxo.Settings.CultureSettings.PropertyKeyDocumentCulture, Altaxo.Main.Services.RuntimePropertyKind.UserAndApplicationAndBuiltin)).Culture;
        _view.IsSpellCheckingEnabled = _options.IsSpellCheckingEnabled ?? _options.Document.GetPropertyValue(TextDocumentViewOptions.PropertyKeyIsSpellCheckingEnabled, () => true);
        _view.IsHyphenationEnabled = TextDocument.IsHyphenationEnabled ?? TextDocument.GetPropertyValue(TextDocumentViewOptions.PropertyKeyIsHyphenationEnabled, () => true);
        _view.IsFoldingEnabled = _options.IsFoldingEnabled ?? _options.Document.GetPropertyValue(TextDocumentViewOptions.PropertyKeyIsFoldingEnabled, () => true);
        _view.HighlightingStyle = _options.HighlightingStyle ?? _options.Document.GetPropertyValue(TextDocumentViewOptions.PropertyKeyHighlightingStyle, () => "default");
        _view.IsOutlineWindowVisible = _options.IsOutlineWindowVisible ?? _options.Document.GetPropertyValue(TextDocumentViewOptions.PropertyKeyOutlineVisible, () => false);
        _view.OutlineWindowRelativeWidth = _options.OutlineWindowRelativeWidth ?? _options.Document.GetPropertyValue(TextDocumentViewOptions.PropertyKeyOutlineWindowRelativeWidth, () => double.NaN);
        _view.IsInInitializationMode = false;
      }
    }

    private void AttachView()
    {
      _view.SourceTextChanged += EhSourceTextChanged;
      _view.Controller = this;
    }

    private void DetachView()
    {
      _view.SourceTextChanged -= EhSourceTextChanged;
      _view.Controller = null;
    }

    private void EhDocumentChanged(object sender, EventArgs e)
    {
    }

    private void EhSourceTextChanged(object sender, EventArgs e)
    {
      if (_view is not null)
      {
        TextDocument.SourceText = _view.SourceText;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the outline is visible.
    /// </summary>
    public bool IsOutlineVisible
    {
      get
      {
        return _view?.IsOutlineWindowVisible ?? false;
      }
      set
      {
        if (_view is not null)
          _view.IsOutlineWindowVisible = value;
      }
    }

    /// <inheritdoc/>
    public bool Apply(bool disposeController)
    {
      throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public bool Revert(bool disposeController)
    {
      throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public string InsertImageInDocumentAndGetUrl(string fileName)
    {
      var imageProxy = MemoryStreamImageProxy.FromFile(fileName);
      return TextDocument.AddImage(imageProxy);
    }

    /// <inheritdoc/>
    public string InsertImageInDocumentAndGetUrl(System.IO.Stream memoryStream, string fileExtension)
    {
      var imageProxy = MemoryStreamImageProxy.FromStream(memoryStream, fileExtension);
      return TextDocument.AddImage(imageProxy);
    }

    /// <summary>
    /// Inserts the source text at caret position.
    /// </summary>
    /// <param name="text">The text.</param>
    public void InsertSourceTextAtCaretPosition(string text)
    {
      if (_view is not null)
      {
        _view.InsertSourceTextAtCaretPosition(text);
      }
      else
      {
        TextDocument.SourceText += text;
      }
    }

    /// <inheritdoc/>
    public bool CanAcceptImageFileName(string fileName)
    {
      var extension = System.IO.Path.GetExtension(fileName).ToLowerInvariant();
      switch (extension)
      {
        case ".jpg":
        case ".jpeg":
        case ".png":
        case ".bmp":
        case ".tif":
          return true;

        default:
          return false;
      }
    }

    /// <inheritdoc/>
    public void EhIsViewerSelectedChanged(bool isViewerSelected)
    {
      _options.IsViewerSelected = isViewerSelected;
    }

    /// <inheritdoc/>
    public void EhViewerConfigurationChanged(ViewerConfiguration windowConfiguration)
    {
      _options.WindowConfiguration = windowConfiguration;
    }

    /// <inheritdoc/>
    public void EhFractionOfEditorWindowChanged(double fractionOfEditor)
    {
      _options.FractionOfSourceEditorWindowVisible = fractionOfEditor;
    }

    /// <inheritdoc/>
    public void EhReferencedImageUrlsChanged(IEnumerable<(string Url, int urlSpanStart, int urlSpanEnd)> referencedLocalImages)
    {
      TextDocument.ReferencedImageUrls = referencedLocalImages;
    }

    /// <inheritdoc/>
    public void EhBeforeCompleteRendering()
    {
      _view.IsLineNumberingEnabled = _options.IsLineNumberingEnabled ?? _options.Document.GetPropertyValue(TextDocumentViewOptions.PropertyKeyIsLineNumberingEnabled, () => true);
      _view.IsWordWrappingEnabled = _options.IsWordWrappingEnabled ?? _options.Document.GetPropertyValue(TextDocumentViewOptions.PropertyKeyIsWordWrappingEnabled, () => true);
      _view.DocumentCulture = TextDocument.GetPropertyValue(Altaxo.Settings.CultureSettings.PropertyKeyDocumentCulture, () => Current.PropertyService.GetValue(Altaxo.Settings.CultureSettings.PropertyKeyDocumentCulture, Altaxo.Main.Services.RuntimePropertyKind.UserAndApplicationAndBuiltin)).Culture;
      _view.IsSpellCheckingEnabled = _options.IsSpellCheckingEnabled ?? _options.Document.GetPropertyValue(TextDocumentViewOptions.PropertyKeyIsSpellCheckingEnabled, () => true);
      _view.IsHyphenationEnabled = TextDocument.IsHyphenationEnabled ?? TextDocument.GetPropertyValue(TextDocumentViewOptions.PropertyKeyIsHyphenationEnabled, () => true);
      _view.IsFoldingEnabled = _options.IsFoldingEnabled ?? _options.Document.GetPropertyValue(TextDocumentViewOptions.PropertyKeyIsFoldingEnabled, () => true);
      _view.HighlightingStyle = _options.HighlightingStyle ?? _options.Document.GetPropertyValue(TextDocumentViewOptions.PropertyKeyHighlightingStyle, () => "default");
    }

    /// <inheritdoc/>
    public bool CanPaste()
    {
      var dao = Current.Gui.OpenClipboardDataObject();

      if (dao.ContainsFileDropList())
      {
        var fileList = dao.GetFileDropList();
        foreach (var fileName in fileList)
        {
          if (true == CanAcceptImageFileName(fileName))
          {
            return true;
          }
        }
      }
      else if (dao.ContainsImage())
      {
        return true;
      }
      else if (dao.GetDataPresent("Altaxo.Text.TextDocument"))
      {
        return true;
      }
      return false;
    }

    /// <inheritdoc/>
    public bool Paste()
    {
      var dao = Current.Gui.OpenClipboardDataObject();

      if (dao.ContainsFileDropList())
      {
        var fileList = dao.GetFileDropList();
        foreach (var fileName in fileList)
        {
          if (true == CanAcceptImageFileName(fileName))
          {
            string url = InsertImageInDocumentAndGetUrl(fileName);

            if (url is not null)
            {
              InsertSourceTextAtCaretPosition(string.Format("![](local:{0})", url));
              return true;
            }
          }
        }
      }
      else if (dao.ContainsImage())
      {
        var (stream, streamFileExtension) = dao.GetBitmapImageAsOptimizedMemoryStream();
        if (stream is not null)
        {
          try
          {
            var url = InsertImageInDocumentAndGetUrl(stream, streamFileExtension);
            if (url is not null)
            {
              InsertSourceTextAtCaretPosition(string.Format("![](local:{0})", url));
              return true;
            }
          }
          finally
          {
            stream.Dispose();
          }
        }
      }
      else if (dao.GetDataPresent("Altaxo.Text.TextDocument"))
      {
        var textDocument = Altaxo.Serialization.Clipboard.ClipboardSerialization.GetObjectFromClipboard<Altaxo.Text.TextDocument>("Altaxo.Text.TextDocument");
        TextDocument.AddImagesFrom(textDocument);
        InsertSourceTextAtCaretPosition(textDocument.SourceText);
        return true;
      }

      return false;
    }

    /// <summary>
    /// Copies the text with the local images to the clipboard.
    /// </summary>
    /// <summary>
    /// Copies the text together with local images to the clipboard.
    /// </summary>
    public void CopyTextWithImages()
    {
      var dao = Current.Gui.GetNewClipboardDataObject();
      Altaxo.Serialization.Clipboard.ClipboardSerialization.PutObjectToDataObject("Altaxo.Text.TextDocument", TextDocument, dao);
      dao.SetData(typeof(string), TextDocument.SourceText);
      Current.Gui.SetClipboardDataObject(dao);
    }

    /// <summary>
    /// Expands the current text document and stores it into a new text document in the same project folder.
    /// </summary>
    /// <summary>
    /// Expands the current text document into a new document in the same project folder.
    /// </summary>
    public void ExpandTextDocumentIntoNewDocument()
    {
      var newTextDocument = ChildDocumentExpander.ExpandDocumentToNewDocument(TextDocument, TextDocument.Folder);
      Current.ProjectService.OpenOrCreateViewContentForDocument(newTextDocument);
    }

    /// <summary>
    /// Renumerates the figures in the current text document.
    /// </summary>
    public void RenumerateFigures()
    {
      var newSource = FigureRenumerator.RenumerateFigures(TextDocument.SourceText);
      TextDocument.SourceText = newSource;
      if (_view is not null)
        _view.SourceText = newSource;
    }

    /// <inheritdoc/>
    public override object ViewObject
    {
      get
      {
        return _view;
      }
      set
      {
        if (_view is not null)
        {
          DetachView();
        }

        _view = value as ITextDocumentView;

        if (_view is not null)
        {
          AttachView();
          Initialize(false);
        }
      }
    }

    /// <inheritdoc/>
    public override void Dispose()
    {
      ViewObject = null;
      _weakEventHandlerForDoc_TunneledEvent?.Remove();
      _options = null;

      base.Dispose();
    }

    /// <inheritdoc/>
    public override object ModelObject
    {
      get
      {
        return (TextDocumentViewOptions)_options.Clone();
      }
    }
  }
}
