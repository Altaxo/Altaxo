#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
using Altaxo.Calc.Regression.Multivariate;

using Altaxo.Gui;
using Altaxo.Gui.Common;

namespace Altaxo.Worksheet.GUI
{
  [UserControllerForObject(typeof(MultivariateLinearFitParameters),100)]
  public class MultivariateLinearRegressionController : Altaxo.Gui.Common.MultiChildController
  {
    MultivariateLinearFitParameters _param;
    ControlViewElement[] _elements = new ControlViewElement[4];
    SingleChoiceController _ctrl0;
    BooleanValueController _ctrl1,_ctrl2, _ctrl3;
    public MultivariateLinearRegressionController(MultivariateLinearFitParameters param)
    {
      _param = param;

      string[] names = new string[param.SelectedDataColumns.Count];
      for(int i=0;i<names.Length;i++)
        names[i] = param.Table[param.SelectedDataColumns[i]].Name;

     
      
       _ctrl0 = new SingleChoiceController(names,0);
       _ctrl1 = new BooleanValueController(_param.IncludeIntercept);
       _ctrl2 = new BooleanValueController(_param.GenerateRegressionValues);
       _ctrl3 = new BooleanValueController(_param.GenerateRegressionValues);

        _ctrl0.DescriptionText = "Choose the dependent variable:";
        _ctrl1.DescriptionText = "Include intercept";
        _ctrl2.DescriptionText = "Generate prediction values";
        _ctrl3.DescriptionText = "Generate residual values";

      _elements[0] = new ControlViewElement(null,_ctrl0);
      _elements[1] = new ControlViewElement(null, _ctrl1);
      _elements[2] = new ControlViewElement(null, _ctrl2);
      _elements[3] = new ControlViewElement(null, _ctrl3);

      for(int i=0;i<_elements.Length;i++)
      {
        Current.Gui.FindAndAttachControlTo((IMVCController)_elements[i].Controller);
        _elements[i].View = ((IMVCController)_elements[i].Controller).ViewObject;
      }


      base.Initialize(_elements,false);
    }

    public override object ModelObject
    {
      get
      {
        return _param;
      }
    }
    public override bool Apply()
    {
      if(base.Apply())
      {
      
        _param.DependentColumnIndexIntoSelection = (int)_ctrl0.ModelObject;
        _param.IncludeIntercept = (bool)_ctrl1.ModelObject;
        _param.GenerateRegressionValues = (bool)_ctrl2.ModelObject;
        _param.GenerateResidualValues = (bool)_ctrl3.ModelObject;
        return true;
      }
      else
        return false;
    }
  }
}
