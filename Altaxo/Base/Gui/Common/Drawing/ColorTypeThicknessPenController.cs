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

namespace Altaxo.Gui.Common.Drawing
{
	#region interfaces

	public interface IColorTypeThicknessPenView
	{
		IColorTypeThicknessPenViewEventSink Controller { get; set; }

		PenX DocPen { get; set; }

		void SetShowPlotColorsOnly(bool restrictChoiceToThisCollection);
	}

	public interface IColorTypeThicknessPenViewEventSink
	{
		void EhView_ShowFullPenDialog();
	}

	public interface IColorTypeThicknessPenController : IColorTypeThicknessPenViewEventSink, IMVCAController
	{
	}

	#endregion interfaces

	/// <summary>
	/// Summary description for ColorTypeWidthPenController.
	/// </summary>
	public class ColorTypeThicknessPenController : IColorTypeThicknessPenController
	{
		private PenX _doc;
		private PenX _tempDoc;
		private IColorTypeThicknessPenView _view;
		private bool _showPlotColorsOnly;

		public ColorTypeThicknessPenController(PenX doc)
		{
			if (doc == null) throw new ArgumentNullException("doc");
			_doc = doc;
			_tempDoc = (PenX)doc.Clone();
		}

		private void Initialize()
		{
			if (_view != null)
			{
				_view.DocPen = _tempDoc;
				_view.SetShowPlotColorsOnly(_showPlotColorsOnly);
			}
		}

		public void SetShowPlotColorsOnly(bool showPlotColorsOnly)
		{
			_showPlotColorsOnly = showPlotColorsOnly;
			if (null != _view)
				_view.SetShowPlotColorsOnly(_showPlotColorsOnly);
		}

		#region IColorTypeThicknessPenViewEventSink Members

		public void EhView_ShowFullPenDialog()
		{
			var ctrl = new PenAllPropertiesController(_tempDoc);
			Current.Gui.ShowDialog(ctrl, "Pen properties");
			if (null != _view)
				_view.DocPen = _tempDoc;
		}

		#endregion IColorTypeThicknessPenViewEventSink Members

		#region IMVCController Members

		public object ViewObject
		{
			get
			{
				return _view;
			}
			set
			{
				if (_view != null)
					_view.Controller = null;

				_view = value as IColorTypeThicknessPenView;

				Initialize();

				if (_view != null)
					_view.Controller = this;
			}
		}

		public object ModelObject
		{
			get
			{
				return _doc;
			}
		}

		public void Dispose()
		{
		}

		#endregion IMVCController Members

		#region IApplyController Members

		public bool Apply()
		{
			_doc.CopyFrom(_view.DocPen);
			return true;
		}

		#endregion IApplyController Members
	}
}