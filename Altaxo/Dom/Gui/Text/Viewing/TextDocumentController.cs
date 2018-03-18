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
		public ITextDocumentView _view;

		protected TextDocument _doc;

		public TextDocument Doc { get { return _doc; } }

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
			if (args[0] is TextDocument notesDoc)
			{
				InternalInitializeDocument(notesDoc);
			}
			else if (args[0] is Altaxo.Text.GuiModels.TextDocumentViewOptions notesViewOptions)
			{
				if (this._doc == null)
				{
					InternalInitializeDocument(notesViewOptions.Document);
				}
				else if (!object.ReferenceEquals(this._doc, notesViewOptions.Document))
				{
					throw new InvalidProgramException("The already initialized document and the document in the option class are not identical");
				}
			}
			else
			{
				return false;
			}
			return true;
		}

		public UseDocument UseDocumentCopy
		{
			set { }
		}

		protected void InternalInitializeDocument(TextDocument doc)
		{
			if (_doc != null)
				throw new ApplicationException(nameof(_doc) + " is already initialized");
			_doc = doc ?? throw new ArgumentNullException(nameof(doc));

			this.Title = GetTitleFromDocumentName(_doc);

			_doc.TunneledEvent += new WeakActionHandler<object, object, Altaxo.Main.TunnelingEventArgs>(EhDocumentTunneledEvent, (handler) => _doc.TunneledEvent -= handler);
		}

		private void EhDocumentTunneledEvent(object arg1, object arg2, TunnelingEventArgs e)
		{
			if (e is Altaxo.Main.DocumentPathChangedEventArgs && _view != null)
			{
				_view.SetDocumentNameAndLocalImages(_doc.Name, _doc.Images);
				this.Title = GetTitleFromDocumentName(_doc);
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
				_view.SetDocumentNameAndLocalImages(_doc.Name, _doc.Images);
				_view.SourceText = _doc.SourceText;
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
				_doc.SourceText = _view.SourceText;
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
			return _doc.AddImage(imageProxy);
		}

		public string InsertImageInDocumentAndGetUrl(System.IO.MemoryStream memoryStream)
		{
			var imageProxy = MemoryStreamImageProxy.FromStream(memoryStream);
			return _doc.AddImage(imageProxy);
		}

		public bool CanAcceptImageFileName(string fileName)
		{
			var extension = System.IO.Path.GetExtension(fileName).ToLowerInvariant();
			switch (extension)
			{
				case ".jpg":
				case ".png":
				case ".bmp":
				case ".tif":
					return true;

				default:
					return false;
			}
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
					Initialize(false);
					AttachView();
				}
			}
		}

		public override object ModelObject
		{
			get
			{
				return new Altaxo.Text.GuiModels.TextDocumentViewOptions(_doc);
			}
		}
	}
}
