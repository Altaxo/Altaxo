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

#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using Altaxo.Calc.Regression.Multivariate;
using Altaxo.Gui.Common;

namespace Altaxo.Gui.Worksheet
{
  [ExpectedTypeOfView(typeof(IMultiChildView))]
  [UserControllerForObject(typeof(MultivariateLinearFitParameters), 100)]
  public class MultivariateLinearRegressionController : MVCANControllerEditCopyOfDocBase<MultivariateLinearFitParameters, IMultiChildView>
  {
    private MultiChildController _innerController;

    private ControlViewElement[] _elements = new ControlViewElement[4];
    private SingleChoiceController _ctrl0;
    private BooleanValueController _ctrl1, _ctrl2, _ctrl3;

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_ctrl0, () => _ctrl0 = null);
      yield return new ControllerAndSetNullMethod(_ctrl1, () => _ctrl1 = null);
      yield return new ControllerAndSetNullMethod(_ctrl2, () => _ctrl2 = null);
      yield return new ControllerAndSetNullMethod(_ctrl3, () => _ctrl3 = null);

      yield return new ControllerAndSetNullMethod(_innerController, () => _innerController = null);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        string[] names = new string[_doc.SelectedDataColumns.Count];
        for (int i = 0; i < names.Length; i++)
          names[i] = _doc.Table[_doc.SelectedDataColumns[i]].Name;

        _ctrl0 = new SingleChoiceController(names, 0);
        _ctrl1 = new BooleanValueController(_doc.IncludeIntercept);
        _ctrl2 = new BooleanValueController(_doc.GenerateRegressionValues);
        _ctrl3 = new BooleanValueController(_doc.GenerateRegressionValues);

        _ctrl0.DescriptionText = "Choose the dependent variable:";
        _ctrl1.DescriptionText = "Include intercept";
        _ctrl2.DescriptionText = "Generate prediction values";
        _ctrl3.DescriptionText = "Generate residual values";

        _elements[0] = new ControlViewElement(null, _ctrl0);
        _elements[1] = new ControlViewElement(null, _ctrl1);
        _elements[2] = new ControlViewElement(null, _ctrl2);
        _elements[3] = new ControlViewElement(null, _ctrl3);

        for (int i = 0; i < _elements.Length; i++)
        {
          Current.Gui.FindAndAttachControlTo((IMVCController)_elements[i].Controller);
          _elements[i].View = ((IMVCController)_elements[i].Controller).ViewObject;
        }

        _innerController = new MultiChildController(_elements, false);
      }
    }

    public override bool Apply(bool disposeController)
    {
      bool applyResult;

      if (_innerController.Apply(disposeController))
      {
        _doc.DependentColumnIndexIntoSelection = (int)_ctrl0.ModelObject;
        _doc.IncludeIntercept = (bool)_ctrl1.ModelObject;
        _doc.GenerateRegressionValues = (bool)_ctrl2.ModelObject;
        _doc.GenerateResidualValues = (bool)_ctrl3.ModelObject;
        applyResult = true;
      }
      else
      {
        applyResult = false;
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
