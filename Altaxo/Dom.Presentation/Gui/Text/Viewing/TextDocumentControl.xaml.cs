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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Altaxo.Drawing;
using Altaxo.Gui.Markdown;
using Altaxo.Text.GuiModels;

namespace Altaxo.Gui.Text.Viewing
{
  /// <summary>
  /// Interaction logic for TextDocumentControl.xaml
  /// </summary>
  public partial class TextDocumentControl : UserControl, ITextDocumentView
  {
    private ImageProvider _imageProvider;
    private ITextDocumentController _controller;
    private string _documentName;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextDocumentControl"/> class.
    /// </summary>
    public TextDocumentControl()
    {
      InitializeComponent();
      _guiEditor.FractionOfEditorChanged += EhEditor_FractionOfEditorChanged;
      _guiEditor.ViewingConfigurationChanged += EhEditor_ViewingConfigurationChanged;
      _guiEditor.IsViewerSelectedChanged += EhEditor_IsViewerSelectedChanged;
      _guiEditor.BeforeCompleteRendering += EhEditor_BeforeCompleteRendering;
    }

    private void EhImageProvider_ReferencedImageUrlsChanged(ICollection<(string url, int spanStart, int spanEnd)> obj)
    {
      _controller?.EhReferencedImageUrlsChanged(obj);
    }

    /// <inheritdoc/>
    public ITextDocumentController Controller
    {
      set
      {
        _controller = value;
      }
    }

    /// <inheritdoc/>
    public bool IsInInitializationMode { set { _guiEditor.IsInInitializationMode = value; } }

    /// <inheritdoc/>
    public void SetDocumentNameAndLocalImages(string documentName, IReadOnlyDictionary<string, MemoryStreamImageProxy> localImages)
    {
      _documentName = documentName;
      var folder = Altaxo.Main.ProjectFolder.GetFolderPart(_documentName);
      if (_imageProvider is null || _imageProvider.AltaxoFolderLocation != folder || _imageProvider.LocalImages != localImages)
      {
        if (_imageProvider is not null)
        {
          _imageProvider.ReferencedImageUrlsChanged -= EhImageProvider_ReferencedImageUrlsChanged;
        }

        _imageProvider = new ImageProvider(folder, localImages);
        _guiEditor.ImageProvider = _imageProvider;

        _imageProvider.ReferencedImageUrlsChanged += EhImageProvider_ReferencedImageUrlsChanged;
      }
    }

    /// <inheritdoc/>
    public string SourceText
    {
      get { return _guiEditor.SourceText; }
      set { _guiEditor.SourceText = value; }
    }

    /// <inheritdoc/>
    public void InsertSourceTextAtCaretPosition(string text)
    {
      _guiEditor.InsertSourceTextAtCaretPosition(text);
    }

    /// <inheritdoc/>
    public string StyleName
    {
      set
      {
        _guiEditor.StyleName = value;
      }
    }

    /// <inheritdoc/>
    public event EventHandler SourceTextChanged
    {
      add
      {
        _guiEditor.SourceTextChanged += value;
      }
      remove
      {
        _guiEditor.SourceTextChanged -= value;
      }
    }

    /// <inheritdoc/>
    public bool IsViewerSelected
    {
      get
      {
        return _guiEditor.IsViewerSelected;
      }
      set
      {
        _guiEditor.IsViewerSelected = value;
      }
    }

    private void EhEditor_IsViewerSelectedChanged(object sender, EventArgs e)
    {
      _controller?.EhIsViewerSelectedChanged(_guiEditor.IsViewerSelected);
    }

    private void EhEditor_BeforeCompleteRendering(object sender, EventArgs e)
    {
      _controller?.EhBeforeCompleteRendering();
    }

    /// <inheritdoc/>
    public ViewerConfiguration WindowConfiguration
    {
      get
      {
        return (ViewerConfiguration)_guiEditor.ViewingConfiguration;
      }
      set
      {
        _guiEditor.ViewingConfiguration = (Markdown.ViewingConfiguration)value;
      }
    }

    private void EhEditor_ViewingConfigurationChanged(object arg1, ViewingConfiguration arg2)
    {
      _controller?.EhViewerConfigurationChanged(WindowConfiguration);
    }

    /// <inheritdoc/>
    public double FractionOfEditorWindow
    {
      get
      {
        return _guiEditor.FractionOfEditorWindow;
      }
      set
      {
        _guiEditor.FractionOfEditorWindow = value;
      }
    }

    /// <inheritdoc/>
    public bool IsWordWrappingEnabled
    {
      set
      {
        _guiEditor.IsWordWrapEnabled = value;
      }
    }

    /// <inheritdoc/>
    public bool IsLineNumberingEnabled
    {
      set
      {
        _guiEditor.IsLineNumberingEnabled = value;
      }
    }

    /// <inheritdoc/>
    public bool IsOutlineWindowVisible
    {
      get
      {
        return _guiEditor.IsOutlineWindowVisible;
      }
      set
      {
        _guiEditor.IsOutlineWindowVisible = value;
      }
    }

    /// <inheritdoc/>
    public double OutlineWindowRelativeWidth
    {
      get
      {
        return _guiEditor.OutlineWindowRelativeWidth;
      }
      set
      {
        _guiEditor.OutlineWindowRelativeWidth = value;
      }
    }

    /// <inheritdoc/>
    public System.Globalization.CultureInfo DocumentCulture
    {
      set
      {
        _guiEditor.DocumentCulture = value;
      }
    }

    /// <inheritdoc/>
    public bool IsSpellCheckingEnabled
    {
      set
      {
        _guiEditor.IsSpellCheckingEnabled = value;
      }
    }

    /// <inheritdoc/>
    public bool IsHyphenationEnabled
    {
      set
      {
        _guiEditor.IsHyphenationEnabled = value;
      }
    }

    /// <inheritdoc/>
    public bool IsFoldingEnabled
    {
      set
      {
        _guiEditor.IsFoldingEnabled = value;
      }
    }

    /// <inheritdoc/>
    public string HighlightingStyle
    {
      set
      {
        _guiEditor.HighlightingStyle = value;
      }
    }

    /// <inheritdoc/>
    public void PrintShowDialog()
    {
      _imageProvider.TargetResolution = 600;
      Altaxo.Gui.Markdown.Commands.RefreshViewer.Execute(null, null);

      _guiEditor.PrintShowDialog("Altaxo-" + _documentName);

      _imageProvider.TargetResolution = ImageProvider.DefaultTargetResolution;
      Altaxo.Gui.Markdown.Commands.RefreshViewer.Execute(null, null);
    }

    private void EhEditor_FractionOfEditorChanged(object sender, EventArgs e)
    {
      _controller?.EhFractionOfEditorWindowChanged(FractionOfEditorWindow);
    }

    private void EhPreviewKeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.F12 && Altaxo.Gui.Markdown.Commands.ToggleBetweenEditorAndViewer.CanExecute(null, null))
      {
        Altaxo.Gui.Markdown.Commands.ToggleBetweenEditorAndViewer.Execute(null, null);
        e.Handled = true;
      }

      if (e.Key == Key.F5 && Altaxo.Gui.Markdown.Commands.RefreshViewer.CanExecute(null, null))
      {
        Altaxo.Gui.Markdown.Commands.RefreshViewer.Execute(null, null);
        e.Handled = true;
      }
    }

    #region Pasting of images

    private string InsertImageInDocumentAndGetUrl(string fileName)
    {
      return _controller?.InsertImageInDocumentAndGetUrl(fileName);
    }

    private string InsertImageInDocumentAndGetUrl(ImageSource imgSource)
    {
      // before we can give the image to the controller, we have to create a stream from it

      if (imgSource is BitmapSource bmpSource)
      {
        var pngStream = new System.IO.MemoryStream();
        BitmapEncoder pngEncoder = new PngBitmapEncoder();
        pngEncoder.Frames.Add(BitmapFrame.Create(bmpSource));
        pngEncoder.Save(pngStream);
        pngStream.Seek(0, System.IO.SeekOrigin.Begin);

        var jpgStream = new System.IO.MemoryStream();
        BitmapEncoder jpgEncoder = new JpegBitmapEncoder();
        jpgEncoder.Frames.Add(BitmapFrame.Create(bmpSource));
        jpgEncoder.Save(jpgStream);
        jpgStream.Seek(0, System.IO.SeekOrigin.Begin);

        var stream = pngStream.Length < jpgStream.Length ? pngStream : jpgStream;
        var strExt = pngStream.Length < jpgStream.Length ? ".png" : ".jpg";
        var altStream = pngStream.Length < jpgStream.Length ? jpgStream : pngStream;
        altStream.Dispose();

        return _controller?.InsertImageInDocumentAndGetUrl(stream, strExt);
      }

      return null;
    }

    private void EhPreviewExecuted(object sender, ExecutedRoutedEventArgs e)
    {
      if (e.Command == ApplicationCommands.Paste)
      {
        if (true == _controller?.Paste())
        {
          e.Handled = true;
        }
      }
    }

    private void EhPreviewCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      if (e.Command == ApplicationCommands.Paste)
      {
        if (true == _controller?.CanPaste())
        {
          e.CanExecute = true;
          e.Handled = true;
        }
      }
    }

    #endregion Pasting of images
  }
}
