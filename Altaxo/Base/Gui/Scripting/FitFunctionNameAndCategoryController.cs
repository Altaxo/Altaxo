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

using Altaxo.Gui.Common;
using Altaxo.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Scripting
{
	[ExpectedTypeOfView(typeof(IMultiChildView))]
	public class FitFunctionNameAndCategoryController : MVCANControllerEditOriginalDocBase<FitFunctionScript, IMultiChildView>
	{
		private MultiChildController _innerController;

		private string _tempName;
		private string _tempCategory;
		private string _tempDescription;
		private bool _tempShouldSave = false;

		private ISingleValueController _controllerName;
		private ISingleValueController _controllerCategory;
		private ISingleValueController _controllerDescription;
		private IBooleanValueController _controllerShouldSaveInUserData;

		public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
		{
			yield return new ControllerAndSetNullMethod(_controllerName, () => _controllerName = null);
			yield return new ControllerAndSetNullMethod(_controllerCategory, () => _controllerCategory = null);
			yield return new ControllerAndSetNullMethod(_controllerDescription, () => _controllerDescription = null);
			yield return new ControllerAndSetNullMethod(_controllerShouldSaveInUserData, () => _controllerShouldSaveInUserData = null);

			yield return new ControllerAndSetNullMethod(_innerController, () => _innerController = null);
		}

		protected override void Initialize(bool initData)
		{
			base.Initialize(initData);

			if (initData)
			{
				_tempName = _doc.FitFunctionName;
				_tempCategory = _doc.FitFunctionCategory;
				_tempDescription = _doc.FitFunctionDescription;

				_controllerName = (ISingleValueController)Current.Gui.GetControllerAndControl(new object[] { _tempName }, typeof(ISingleValueController));
				_controllerCategory = (ISingleValueController)Current.Gui.GetControllerAndControl(new object[] { _tempCategory }, typeof(ISingleValueController));
				_controllerDescription = (ISingleValueController)Current.Gui.GetControllerAndControl(new object[] { _tempDescription }, typeof(ISingleValueController));
				_controllerShouldSaveInUserData = (IBooleanValueController)Current.Gui.GetControllerAndControl(new object[] { _tempShouldSave }, typeof(IBooleanValueController));

				_controllerName.DescriptionText = "Enter fit function name:";
				_controllerCategory.DescriptionText = "Enter fit function category:";
				_controllerDescription.DescriptionText = "Enter fit function description:";
				_controllerShouldSaveInUserData.DescriptionText = "Save in user fit functions directory?";

				_innerController = new MultiChildController(new ControlViewElement[]{
        new ControlViewElement( null, _controllerName, _controllerName.ViewObject),
        new ControlViewElement( null, _controllerCategory, _controllerCategory.ViewObject),
        new ControlViewElement( null, _controllerDescription, _controllerDescription.ViewObject),
        new ControlViewElement( null, _controllerShouldSaveInUserData, _controllerShouldSaveInUserData.ViewObject) },
					false);
			}
		}

		public override bool Apply(bool disposeController)
		{
			bool applyResult = false;

			bool result1, result2, result3;

			result1 = _controllerName.Apply(disposeController);
			if (result1)
				_tempName = (string)_controllerName.ModelObject;

			result2 = _controllerCategory.Apply(disposeController);
			if (result2)
				_tempCategory = (string)_controllerCategory.ModelObject;

			result3 = _controllerDescription.Apply(disposeController);
			if (result3)
				_tempDescription = (string)_controllerDescription.ModelObject;

			if (result1 && result2 && result3)
			{
				// make sure that the name is not empty
				_tempName = _tempName.Trim();
				if (_tempName == string.Empty)
				{
					Current.Gui.ErrorMessageBox("Name must not be empty!");
					return false;
				}

				_doc.FitFunctionName = _tempName;
				_doc.FitFunctionCategory = _tempCategory;
				_doc.FitFunctionDescription = _tempDescription;

				applyResult = true;
				if (_controllerShouldSaveInUserData.Apply(disposeController) && true == ((bool)_controllerShouldSaveInUserData.ModelObject))
				{
					if (!Current.FitFunctionService.SaveUserDefinedFitFunction(_doc))
						applyResult = false; // Cancel the end of dialog
				}
			}

			return ApplyEnd(applyResult, disposeController);
		}

		protected override void AttachView()
		{
			base.AttachView();
			if (null != _innerController)
			{
				_innerController.ViewObject = _view;
			}
		}

		protected override void DetachView()
		{
			if (null != _innerController)
			{
				_innerController.ViewObject = null;
			}
			base.DetachView();
		}
	}
}