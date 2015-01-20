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

using Altaxo.Graph.Gdi.Plot;
using Altaxo.Gui.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Gui.Graph
{
	[UserControllerForObject(typeof(DensityImagePlotItem))]
	[ExpectedTypeOfView(typeof(ITabbedElementView))]
	internal class DensityImagePlotItemController : TabbedElementController, IMVCANController
	{
		private bool _useDocumentCopy;
		private DensityImagePlotItem _originalDoc;
		private DensityImagePlotItem _doc;

		private IMVCANController _styleController;

		/// <summary>Controls the option view where users can copy the image to disc or save the image.</summary>
		private IMVCANController _optionsController;

		/// <summary>Controls the data view, in which the user can chose which columns to use in the plot item.</summary>
		private IMVCANController _dataController;

		public DensityImagePlotItemController()
		{
		}

		public bool InitializeDocument(params object[] args)
		{
			if (args == null || args.Length == 0)
				return false;

			if (!(args[0] is DensityImagePlotItem))
				return false;
			else
				_originalDoc = _doc = (DensityImagePlotItem)args[0];

			if (_useDocumentCopy)
				_doc = (DensityImagePlotItem)_originalDoc.Clone();

			InitializeStyle();
			InitializeDataView();
			InitializeOptionView();
			BringTabToFront(0);

			return true;
		}

		public UseDocument UseDocumentCopy
		{
			set { _useDocumentCopy = UseDocument.Copy == value; }
		}

		private void InitializeStyle()
		{
			_styleController = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _doc.Style }, typeof(IMVCANController), UseDocument.Directly);
			this.AddTab("Style", _styleController, _styleController.ViewObject);
		}

		private void InitializeOptionView()
		{
			_optionsController = new DensityImagePlotItemOptionController();
			_optionsController.InitializeDocument(_doc);
			Current.Gui.FindAndAttachControlTo(_optionsController);
			this.AddTab("Options", _optionsController, _optionsController.ViewObject);
		}

		private void InitializeDataView()
		{
			_dataController = new XYZMeshedColumnPlotDataController { UseDocumentCopy = UseDocument.Directly };
			_dataController.InitializeDocument(_doc.Data);
			Current.Gui.FindAndAttachControlTo(_dataController);
			this.AddTab("Data", _dataController, _dataController.ViewObject);
		}

		#region IMVCController Members

		public override object ModelObject
		{
			get { return _originalDoc; }
		}

		#endregion IMVCController Members

		#region IApplyController Members

		public override bool Apply(bool disposeController)
		{
			bool result = true;

			if (_styleController != null)
			{
				if (!_styleController.Apply(disposeController))
					return false;
			}

			if (_dataController != null)
			{
				if (!_dataController.Apply(disposeController))
					return false;
			}

			if (_useDocumentCopy)
				CopyHelper.Copy(ref _originalDoc, _doc);

			return result;
		}

		#endregion IApplyController Members
	}
}