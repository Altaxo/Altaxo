#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
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
#endregion

using System;
using System.Text;
using Altaxo.Main.GUI;
using Altaxo.Scripting;

namespace Altaxo.Gui.Scripting
{
  public class FitFunctionNameAndCategoryController : MultiChildController
  {
    FitFunctionScript _doc;
    string _tempName;
    string _tempCategory;
    string _tempDescription;
    bool _tempShouldSave=false;

    ISingleValueController _controllerName;
    ISingleValueController _controllerCategory;
    ISingleValueController _controllerDescription;
    IBooleanValueController _controllerShouldSaveInUserData;

    public FitFunctionNameAndCategoryController(FitFunctionScript doc)
    {
      _doc = doc;
      _tempName = _doc.FitFunctionName;
      _tempCategory = _doc.FitFunctionCategory;

      _controllerName = (ISingleValueController)Current.Gui.GetControllerAndControl(new object[] { _tempName }, typeof(ISingleValueController));
      _controllerCategory = (ISingleValueController)Current.Gui.GetControllerAndControl(new object[] { _tempCategory }, typeof(ISingleValueController));
      _controllerCategory = (ISingleValueController)Current.Gui.GetControllerAndControl(new object[] { _tempDescription }, typeof(ISingleValueController));
      _controllerShouldSaveInUserData = (IBooleanValueController)Current.Gui.GetControllerAndControl(new object[] { _tempShouldSave }, typeof(IBooleanValueController));


      _controllerName.DescriptionText = "Enter fit function name:";
      _controllerCategory.DescriptionText = "Enter fit function category:";
      _controllerShouldSaveInUserData.DescriptionText = "Save in user fit functions directory?";

      base.Initialize(new IMVCAController[] { _controllerName, _controllerCategory, _controllerDescription, _controllerShouldSaveInUserData });
    }

    public override object ModelObject
    {
      get
      {
        return _doc;
      }
    }

    public override bool Apply()
    {
      bool result1, result2, result3;

      result1 = _controllerName.Apply();
      if (result1)
        _tempName = (string)_controllerName.ModelObject;

      
      result2 = _controllerCategory.Apply();
      if (result2)
        _tempCategory = (string)_controllerCategory.ModelObject;

      result3 = _controllerDescription.Apply();
      if (result3)
        _tempDescription = (string)_controllerCategory.ModelObject;

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


        if (_controllerShouldSaveInUserData.Apply() && true == ((bool)_controllerShouldSaveInUserData.ModelObject))
        {
          if (!Current.FitFunctionService.SaveUserDefinedFitFunction(_doc))
            return false; // Cancel the end of dialog


        }
        return true;
      }

      return false;
    }

  }
}
