#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2004 Dr. Dirk Lellinger
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
using Altaxo.Main.GUI;

namespace Altaxo.Worksheet.GUI
{
  [Altaxo.Main.GUI.UserControllerForObject(typeof(MultivariateLinearFitParameters),100)]
  public class MultivariateLinearRegressionController : Altaxo.Main.GUI.MultiChildController
  {
    MultivariateLinearFitParameters _param;
    IMVCAController[] _controller = new IMVCAController[4];
    Main.GUI.SingleChoiceController _ctrl0;
    Main.GUI.BooleanValueController _ctrl1,_ctrl2, _ctrl3;
    public MultivariateLinearRegressionController(MultivariateLinearFitParameters param)
    {
      _param = param;

      string[] names = new string[param.SelectedDataColumns.Count];
      for(int i=0;i<names.Length;i++)
        names[i] = param.Table[param.SelectedDataColumns[i]].Name;

     
      
      _controller[0] = _ctrl0 = new Main.GUI.SingleChoiceController(names,0);
      _controller[1] = _ctrl1 = new Main.GUI.BooleanValueController(_param.IncludeIntercept);
      _controller[2] = _ctrl2 = new Main.GUI.BooleanValueController(_param.GenerateRegressionValues);
      _controller[3] = _ctrl3 = new Main.GUI.BooleanValueController(_param.GenerateRegressionValues);

      _ctrl0.DescriptionText = "Choose the dependent variable:";
      _ctrl1.DescriptionText = "Include intercept";
      _ctrl2.DescriptionText = "Generate prediction values";
      _ctrl3.DescriptionText = "Generate residual values";

      for(int i=0;i<_controller.Length;i++)
      {
        Current.GUIFactoryService.GetControl(_controller[i]);
      }

      base.Initialize(_controller);
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
