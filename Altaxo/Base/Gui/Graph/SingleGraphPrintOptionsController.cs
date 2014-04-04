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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Graph
{
	using Altaxo.Collections;
	using Altaxo.Graph;

	public interface ISingleGraphPrintOptionsView
	{
		void Init_PrintLocation(SelectableListNodeList list);

		void Init_FitGraphToPrintIfLarger(bool val);

		void Init_FitGraphToPrintIfSmaller(bool val);

		void Init_PrintCopMarks(bool val);

		void Init_RotatePageAutomatically(bool value);

		void Init_TilePages(bool value);

		void Init_UseFixedZoomFactor(bool val);

		void Init_ZoomFactor(double val);

		event Action PrintLocationChanged;

		event Action<bool> FitGraphToPrintIfLargerChanged;

		event Action<bool> FitGraphToPrintIfSmallerChanged;

		event Action<bool> PrintCropMarksChanged;

		event Action<bool> RotatePageAutomaticallyChanged;

		event Action<bool> TilePagesChanged;

		event Action<bool> UseFixedZoomFactorChanged;

		event Action<double> ZoomFactorChanged;
	}

	[ExpectedTypeOfView(typeof(ISingleGraphPrintOptionsView))]
	[UserControllerForObject(typeof(SingleGraphPrintOptions))]
	public class SingleGraphPrintOptionsController : IMVCANController
	{
		private SingleGraphPrintOptions _doc, _originalDoc;
		private ISingleGraphPrintOptionsView _view;
		private UseDocument _useDocumentCopy;

		private SelectableListNodeList _printLocationList;

		private void Initialize(bool initData)
		{
			if (initData)
			{
				_printLocationList = new SelectableListNodeList(_doc.PrintLocation);
			}
			if (null != _view)
			{
				_view.Init_PrintLocation(_printLocationList);
				_view.Init_FitGraphToPrintIfLarger(_doc.FitGraphToPrintIfLarger);
				_view.Init_FitGraphToPrintIfSmaller(_doc.FitGraphToPrintIfSmaller);
				_view.Init_PrintCopMarks(_doc.PrintCropMarks);
				_view.Init_RotatePageAutomatically(_doc.RotatePageAutomatically);
				_view.Init_TilePages(_doc.TilePages);
				_view.Init_UseFixedZoomFactor(_doc.UseFixedZoomFactor);
				_view.Init_ZoomFactor(_doc.ZoomFactor);
			}
		}

		private void EhPrintLocationChanged()
		{
			_doc.PrintLocation = (SingleGraphPrintLocation)_printLocationList.FirstSelectedNode.Tag;
		}

		private void EhFitGraphToPrintIfLargerChanged(bool val)
		{
			_doc.FitGraphToPrintIfLarger = val;
		}

		private void EhFitGraphToPrintIfSmallerChanged(bool val)
		{
			_doc.FitGraphToPrintIfSmaller = val;
		}

		private void EhPrintCropMarksChanged(bool val)
		{
			_doc.PrintCropMarks = val;
		}

		private void EhRotatePageAutomaticallyChanged(bool val)
		{
			_doc.RotatePageAutomatically = val;
		}

		private void EhTilePagesChanged(bool val)
		{
			_doc.TilePages = val;
		}

		private void EhUseFixedZoomFactorChanged(bool val)
		{
			_doc.UseFixedZoomFactor = val;
		}

		private void EhZoomFactorChanged(double val)
		{
			_doc.ZoomFactor = val;
		}

		#region IMVCANController

		public bool InitializeDocument(params object[] args)
		{
			if (null == args || args.Length == 0 || !(args[0] is SingleGraphPrintOptions))
				return false;

			_originalDoc = (SingleGraphPrintOptions)args[0];
			if (_useDocumentCopy == UseDocument.Copy)
				_doc = (SingleGraphPrintOptions)_originalDoc.Clone();
			else
				_doc = _originalDoc;

			Initialize(true);

			return true;
		}

		public UseDocument UseDocumentCopy
		{
			set { _useDocumentCopy = value; }
		}

		public object ViewObject
		{
			get
			{
				return _view;
			}
			set
			{
				if (null != _view)
				{
					_view.PrintLocationChanged -= this.EhPrintLocationChanged;
					_view.FitGraphToPrintIfLargerChanged -= this.EhFitGraphToPrintIfLargerChanged;
					_view.FitGraphToPrintIfSmallerChanged -= this.EhFitGraphToPrintIfSmallerChanged;
					_view.PrintCropMarksChanged -= this.EhPrintCropMarksChanged;
					_view.RotatePageAutomaticallyChanged -= this.EhRotatePageAutomaticallyChanged;
					_view.TilePagesChanged -= this.EhTilePagesChanged;
					_view.UseFixedZoomFactorChanged -= this.EhUseFixedZoomFactorChanged;
					_view.ZoomFactorChanged -= this.EhZoomFactorChanged;
				}

				_view = value as ISingleGraphPrintOptionsView;

				if (null != _view)
				{
					Initialize(false);

					_view.PrintLocationChanged += this.EhPrintLocationChanged;
					_view.FitGraphToPrintIfLargerChanged += this.EhFitGraphToPrintIfLargerChanged;
					_view.FitGraphToPrintIfSmallerChanged += this.EhFitGraphToPrintIfSmallerChanged;
					_view.PrintCropMarksChanged += this.EhPrintCropMarksChanged;
					_view.RotatePageAutomaticallyChanged += this.EhRotatePageAutomaticallyChanged;
					_view.TilePagesChanged += this.EhTilePagesChanged;
					_view.UseFixedZoomFactorChanged += this.EhUseFixedZoomFactorChanged;
					_view.ZoomFactorChanged += this.EhZoomFactorChanged;
				}
			}
		}

		public object ModelObject
		{
			get { return _originalDoc; }
		}

		public void Dispose()
		{
		}

		public bool Apply()
		{
			if (_useDocumentCopy == UseDocument.Copy)
				_originalDoc.CopyFrom(_doc);

			return true;
		}

		#endregion IMVCANController
	}
}