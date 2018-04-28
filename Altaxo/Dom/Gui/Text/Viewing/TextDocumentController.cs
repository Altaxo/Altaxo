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

using Altaxo.Graph;
using Altaxo.Gui.Workbench;
using Altaxo.Main;
using Altaxo.Text;
using Altaxo.Text.GuiModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Gui.Text.Viewing
{
	[UserControllerForObject(typeof(TextDocument))]
	[UserControllerForObject(typeof(Altaxo.Text.GuiModels.TextDocumentViewOptions))]
	[ExpectedTypeOfView(typeof(ITextDocumentView))]
	public class TextDocumentController : AbstractViewContent, IDisposable, IMVCANController, ITextDocumentController
	{
		protected ITextDocumentView _view;

		private Altaxo.Text.GuiModels.TextDocumentViewOptions _options;

		public TextDocument TextDocument { get { return _options.Document; } }

		public TextDocumentController()
		{
		}

		public TextDocumentController(TextDocument doc)
		{
			InitializeDocument(doc);
		}

		public bool InitializeDocument(params object[] args)
		{
			if (null == args || args.Length == 0)
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

			if (newOptions == null)
				return false; // not successfull

			if (newOptions.Document == null)
				throw new InvalidProgramException("The provided options do not contain any document");

			if (_options?.Document != null && !object.ReferenceEquals(this.TextDocument, _options.Document))
			{
				throw new InvalidProgramException("The already initialized document and the document in the option class are not identical");
			}

			InternalInitializeDocument(newOptions);

			return true;
		}

		public void PrintShowDialog()
		{
			_view?.PrintShowDialog();
		}

		public UseDocument UseDocumentCopy
		{
			set { }
		}

		protected void InternalInitializeDocument(TextDocumentViewOptions options)
		{
			if (null == options?.Document)
				throw new ArgumentNullException("No document stored inside options");

			_options = options;

			this.Title = GetTitleFromDocumentName(TextDocument);

			TextDocument.TunneledEvent += new WeakActionHandler<object, object, Altaxo.Main.TunnelingEventArgs>(EhDocumentTunneledEvent, (handler) => TextDocument.TunneledEvent -= handler);
		}

		private void EhDocumentTunneledEvent(object arg1, object arg2, TunnelingEventArgs e)
		{
			if (e is Altaxo.Main.DocumentPathChangedEventArgs && _view != null)
			{
				_view.SetDocumentNameAndLocalImages(TextDocument.Name, TextDocument.Images);
				this.Title = GetTitleFromDocumentName(TextDocument);
			}
		}

		private static string GetTitleFromDocumentName(TextDocument doc)
		{
			if (!Altaxo.Main.ProjectFolder.IsValidFolderName(doc.Name))
				return doc.Name;
			else
				return doc.Name + "FolderNotes";
		}

		protected void Initialize(bool initData)
		{
			if (initData)
			{
			}
			if (null != _view)
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
			if (null != _view)
			{
				TextDocument.SourceText = _view.SourceText;
			}
		}

		public bool Apply(bool disposeController)
		{
			throw new NotImplementedException();
		}

		public bool Revert(bool disposeController)
		{
			throw new NotImplementedException();
		}

		public string InsertImageInDocumentAndGetUrl(string fileName)
		{
			var imageProxy = MemoryStreamImageProxy.FromFile(fileName);
			return TextDocument.AddImage(imageProxy);
		}

		public string InsertImageInDocumentAndGetUrl(System.IO.Stream memoryStream, string fileExtension)
		{
			var imageProxy = MemoryStreamImageProxy.FromStream(memoryStream, fileExtension);
			return TextDocument.AddImage(imageProxy);
		}

		/// <summary>
		/// Inserts the provided markdown source text at the current caret position.
		/// </summary>
		/// <param name="text">The text to insert.</param>
		public void InsertSourceTextAtCaretPosition(string text)
		{
			if (null != _view)
			{
				_view.InsertSourceTextAtCaretPosition(text);
			}
			else
			{
				TextDocument.SourceText += text;
			}
		}

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

		public void EhIsViewerSelectedChanged(bool isViewerSelected)
		{
			_options.IsViewerSelected = isViewerSelected;
		}

		public void EhViewerConfigurationChanged(ViewerConfiguration windowConfiguration)
		{
			_options.WindowConfiguration = windowConfiguration;
		}

		public void EhFractionOfEditorWindowChanged(double fractionOfEditor)
		{
			_options.FractionOfSourceEditorWindowVisible = fractionOfEditor;
		}

		public void EhReferencedImageUrlsChanged(IEnumerable<(string Url, int urlSpanStart, int urlSpanEnd)> referencedImageUrls)
		{
			this.TextDocument.ReferencedImageUrls = referencedImageUrls;
		}

		/// <summary>
		/// This event was fired by the markdown edit control before a complete rendering takes place.
		/// Here we update all properties that may influence the rendering (language, spell checking, hyphenation etc.)
		/// </summary>
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

		/// <summary>
		/// Determines whether this controller is able to accept data from the clipboard to be pasted into the text.
		/// Here we catch special cases like pasting of images.
		/// Thus, a return value of false does not mean that the data can not be pasted, it only mean that pasting should be delegated to the source text view.
		/// </summary>
		/// <returns>
		///   <c>true</c> if data from the clipboard can be accepted to be pasted into the text; otherwise, <c>false</c>.
		/// </returns>
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

						if (null != url)
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
				if (null != stream)
				{
					try
					{
						var url = InsertImageInDocumentAndGetUrl(stream, streamFileExtension);
						if (null != url)
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
				this.InsertSourceTextAtCaretPosition(textDocument.SourceText);
				return true;
			}

			return false;
		}

		/// <summary>
		/// Copies the text with the local images to the clipboard.
		/// </summary>
		public void CopyTextWithImages()
		{
			var dao = Current.Gui.GetNewClipboardDataObject();
			Altaxo.Serialization.Clipboard.ClipboardSerialization.PutObjectToDataObject("Altaxo.Text.TextDocument", TextDocument, dao);
			dao.SetData(typeof(string), TextDocument.SourceText);
			Current.Gui.SetClipboardDataObject(dao);
		}

		public override object ViewObject
		{
			get
			{
				return _view;
			}
			set
			{
				if (null != _view)
				{
					DetachView();
				}

				_view = value as ITextDocumentView;

				if (null != _view)
				{
					AttachView();
					Initialize(false);
				}
			}
		}

		public override object ModelObject
		{
			get
			{
				return (TextDocumentViewOptions)_options.Clone();
			}
		}
	}
}
