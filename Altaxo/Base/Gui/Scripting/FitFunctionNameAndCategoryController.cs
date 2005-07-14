using System;
using System.Collections.Generic;
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

    ISingleValueController _controllerName;
    ISingleValueController _controllerCategory;


    public FitFunctionNameAndCategoryController(FitFunctionScript doc)
    {
      _doc = doc;
      _tempName = _doc.FitFunctionName;
      _tempCategory = _doc.FitFunctionCategory;

      _controllerName = (ISingleValueController)Current.Gui.GetControllerAndControl(new object[] { _tempName }, typeof(ISingleValueController));
      _controllerCategory = (ISingleValueController)Current.Gui.GetControllerAndControl(new object[] { _tempCategory }, typeof(ISingleValueController));

      _controllerName.DescriptionText = "Enter fit function name:";
      _controllerCategory.DescriptionText = "Enter fit function category:";

      base.Initialize(new IMVCAController[] { _controllerName, _controllerCategory });
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
      bool result1, result2;

      result1 = _controllerName.Apply();
      if (result1)
        _tempName = (string)_controllerName.ModelObject;

      
      result2 = _controllerCategory.Apply();
      if (result2)
        _tempCategory = (string)_controllerCategory.ModelObject;

      if (result1 && result2)
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
        return true;
      }

      return false;
    }

  }
}
