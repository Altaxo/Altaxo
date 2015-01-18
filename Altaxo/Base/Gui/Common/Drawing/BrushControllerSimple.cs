#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

using Altaxo.Graph.Gdi;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Gui.Common.Drawing
{
	public interface IBrushViewSimple
	{
		BrushX Brush { get; set; }
	}

	[UserControllerForObject(typeof(BrushX))]
	[ExpectedTypeOfView(typeof(IBrushViewSimple))]
	public class BrushControllerSimple : IMVCANController
	{
		private BrushX _doc;
		private UseDocument _usage;
		private IBrushViewSimple _view;

		public BrushControllerSimple()
		{
		}

		public BrushControllerSimple(BrushX doc)
		{
			InitializeDocument(doc);
		}

		#region IMVCANController Members

		public bool InitializeDocument(params object[] args)
		{
			if (args == null || args.Length == 0)
				return false;

			if (args[0] != null && !(args[0] is BrushX))
				return false;

			_doc = (BrushX)args[0];

			return true;
		}

		private void Initialize()
		{
			if (_view != null)
			{
				if (_doc != null)
					_view.Brush = _usage == UseDocument.Directly ? _doc : (BrushX)_doc.Clone();
				else
					_view.Brush = BrushX.Empty;
			}
		}

		public UseDocument UseDocumentCopy
		{
			set
			{
				_usage = value;
			}
		}

		#endregion IMVCANController Members

		#region IMVCController Members

		public object ViewObject
		{
			get
			{
				return _view;
			}
			set
			{
				_view = value as IBrushViewSimple;
				Initialize();
			}
		}

		public object ModelObject
		{
			get { return _doc; }
		}

		public void Dispose()
		{
		}

		#endregion IMVCController Members

		#region IApplyController Members

		public bool Apply(bool disposeController)
		{
			if (_doc != null || _view.Brush.IsVisible)
				_doc = _view.Brush;

			return true;
		}

		/// <summary>
		/// Try to revert changes to the model, i.e. restores the original state of the model.
		/// </summary>
		/// <param name="disposeController">If set to <c>true</c>, the controller should release all temporary resources, since the controller is not needed anymore.</param>
		/// <returns>
		///   <c>True</c> if the revert operation was successfull; <c>false</c> if the revert operation was not possible (i.e. because the controller has not stored the original state of the model).
		/// </returns>
		public bool Revert(bool disposeController)
		{
			return false;
		}

		#endregion IApplyController Members
	}
}