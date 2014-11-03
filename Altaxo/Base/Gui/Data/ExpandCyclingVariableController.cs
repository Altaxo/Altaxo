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

namespace Altaxo.Gui.Data
{
	using Altaxo.Collections;
	using Altaxo.Data;

	public interface IExpandCyclingVariableView
	{
		void SetDataControl(object control);

		void SetOptionsControl(object control);
	}

	[UserControllerForObject(typeof(ExpandCyclingVariableColumnDataAndOptions))]
	[ExpectedTypeOfView(typeof(IExpandCyclingVariableView))]
	public class ExpandCyclingVariableController : MVCANControllerBase<ExpandCyclingVariableColumnDataAndOptions, IExpandCyclingVariableView>
	{
		private IMVCANController _dataController;
		private IMVCANController _optionsController;

		protected override void Initialize(bool initData)
		{
			if (initData)
			{
				_dataController = new ExpandCyclingVariableDataController() { UseDocumentCopy = UseDocument.Directly };
				_dataController.InitializeDocument(_doc.Data);

				_optionsController = new ExpandCyclingVariableOptionsController { UseDocumentCopy = UseDocument.Directly };
				_optionsController.InitializeDocument(_doc.Options);
				Current.Gui.FindAndAttachControlTo(_dataController);
				Current.Gui.FindAndAttachControlTo(_optionsController);
			}
			if (null != _view)
			{
				_view.SetDataControl(_dataController.ViewObject);
				_view.SetOptionsControl(_optionsController.ViewObject);
			}
		}

		public override bool Apply()
		{
			if (!_dataController.Apply())
				return false;
			if (!_optionsController.Apply())
				return false;

			if (!object.ReferenceEquals(_originalDoc, _doc))
				CopyHelper.Copy(ref _originalDoc, _doc);

			return true;
		}
	}
}