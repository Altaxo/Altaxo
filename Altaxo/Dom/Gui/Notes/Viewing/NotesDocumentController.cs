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

using Altaxo.Gui.Workbench;
using Altaxo.Notes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Gui.Notes.Viewing
{
	[UserControllerForObject(typeof(NotesDocument))]
	[UserControllerForObject(typeof(Altaxo.Notes.GuiModels.NotesDocumentViewOptions))]
	[ExpectedTypeOfView(typeof(INotesDocumentView))]
	public class NotesDocumentController : AbstractViewContent, IDisposable, IMVCANController
	{
		public INotesDocumentView _view;

		protected NotesDocument _doc;

		public NotesDocument Doc { get { return _doc; } }

		public NotesDocumentController()
		{
		}

		public NotesDocumentController(NotesDocument doc)
		{
			InitializeDocument(doc);
		}

		public bool InitializeDocument(params object[] args)
		{
			if (null == args || args.Length == 0)
				return false;
			if (args[0] is NotesDocument notesDoc)
			{
				InternalInitializeDocument(notesDoc);
			}
			else if (args[0] is Altaxo.Notes.GuiModels.NotesDocumentViewOptions notesViewOptions)
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

		protected void InternalInitializeDocument(NotesDocument doc)
		{
			if (_doc != null)
				throw new ApplicationException(nameof(_doc) + " is already initialized");
			_doc = doc ?? throw new ArgumentNullException(nameof(doc));

			this.Title = _doc.Name;
		}

		protected void Initialize(bool initData)
		{
			if (initData)
			{
			}
			if (null != _view)
			{
				_view.SourceText = _doc.SourceText;
			}
		}

		private void AttachView()
		{
			_view.SourceTextChanged += EhSourceTextChanged;
		}

		private void DetachView()
		{
			_view.SourceTextChanged -= EhSourceTextChanged;
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

				_view = value as INotesDocumentView;

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
				return new Altaxo.Notes.GuiModels.NotesDocumentViewOptions(_doc);
			}
		}
	}
}
