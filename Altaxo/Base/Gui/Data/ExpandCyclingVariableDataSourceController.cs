#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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

using Altaxo.Data;
using Altaxo.Worksheet.Commands.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Data
{
	public interface IExpandCyclingVariableDataSourceView
	{
		void SetFourierTransformation2DOptionsControl(object p);

		void SetImportOptionsControl(object p);

		void SetInputDataControl(object p);
	}

	[ExpectedTypeOfView(typeof(IExpandCyclingVariableDataSourceView))]
	[UserControllerForObject(typeof(ExpandCyclingVariableColumnDataSource))]
	public class ExpandCyclingVariableDataSourceController : MVCANControllerEditCopyOfDocBase<ExpandCyclingVariableColumnDataSource, IExpandCyclingVariableDataSourceView>, IMVCSupportsApplyCallback
	{
		private IMVCANController _dataSourceOptionsController;
		private IMVCANController _processOptionsController;
		private IMVCANController _processDataController;

		public event Action SuccessfullyApplied;

		public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
		{
			yield return new ControllerAndSetNullMethod(_dataSourceOptionsController, () => _dataSourceOptionsController = null);
			yield return new ControllerAndSetNullMethod(_processOptionsController, () => _processOptionsController = null);
			yield return new ControllerAndSetNullMethod(_processDataController, () => _processDataController = null);
		}

		protected override void Initialize(bool initData)
		{
			if (initData)
			{
				_dataSourceOptionsController = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _doc.ImportOptions }, typeof(IMVCANController), UseDocument.Directly);
				_processOptionsController = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _doc.ExpandCyclingVariableColumnOptions }, typeof(IMVCANController), UseDocument.Directly);

				_processDataController = new ExpandCyclingVariableDataController() { UseDocumentCopy = UseDocument.Directly };
				_processDataController.InitializeDocument(_doc.InputData);
				Current.Gui.FindAndAttachControlTo(_processDataController);
			}

			if (null != _view)
			{
				_view.SetImportOptionsControl(_dataSourceOptionsController.ViewObject);
				_view.SetFourierTransformation2DOptionsControl(_processOptionsController.ViewObject);
				if (null != _processDataController)
				{
					_view.SetInputDataControl(_processDataController.ViewObject);
				}
			}
		}

		public override bool Apply(bool disposeController)
		{
			bool result;

			result = _dataSourceOptionsController.Apply(disposeController);
			if (!result) return result;

			result = _processOptionsController.Apply(disposeController);
			if (!result) return result;

			if (null != _processDataController)
			{
				result = _processDataController.Apply(disposeController);
				if (!result) return result;
			}

			if (!object.ReferenceEquals(_originalDoc, _doc))
				CopyHelper.Copy(ref _originalDoc, _doc);

			var ev = SuccessfullyApplied;
			if (null != ev)
			{
				ev();
			}

			return true;
		}
	}
}