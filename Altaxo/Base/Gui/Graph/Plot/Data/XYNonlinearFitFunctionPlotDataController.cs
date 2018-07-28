#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2017 Dr. Dirk Lellinger
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

using Altaxo.Calc.Regression.Nonlinear;
using Altaxo.Graph.Plot.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Gui.Graph.Plot.Data
{
  #region Interfaces

  public interface IXYNonlinearFitFunctionPlotDataView
  {
    event EventHandler EditText;

    void InitializeFunctionText(string text, bool editable);
  }

  #endregion Interfaces

  [UserControllerForObject(typeof(XYNonlinearFitFunctionPlotData), 101)]
  [ExpectedTypeOfView(typeof(IXYNonlinearFitFunctionPlotDataView))]
  internal class XYNonlinearFitFunctionPlotDataController : MVCANControllerEditOriginalDocBase<XYNonlinearFitFunctionPlotData, IXYNonlinearFitFunctionPlotDataView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
      }
      if (_view != null)
      {
        _view.InitializeFunctionText(GetDescriptionText(), false);
      }
    }

    public string GetDescriptionText()
    {
      var stb = new StringBuilder();

      var culture = Altaxo.Settings.GuiCulture.Instance;

      var fitdoc = _doc.FitDocumentCopy;

      var parameters = fitdoc.CurrentParameters;

      var thisFitFunction = fitdoc.FitEnsemble[_doc.FitElementIndex].FitFunction;

      var thisIndepTransformation = (_doc.Function as FitFunctionToScalarFunctionDDWrapper)?.IndependentVariableTransformation;
      var thisTransformation = (_doc.Function as FitFunctionToScalarFunctionDDWrapper)?.DependentVariableTransformation;

      stb.AppendFormat(culture, "Displayed fit function (from FitElement[{0}]): {1}", _doc.FitElementIndex, thisFitFunction?.ToString());
      stb.AppendLine();
      stb.AppendFormat(culture, "Independent variable: {0}, fed with: {1} {2}", thisFitFunction?.IndependentVariableName(0), thisIndepTransformation?.RepresentationAsOperator ?? string.Empty, "x_axis_value");
      stb.AppendLine();
      stb.AppendFormat(culture, "Displayed dependent variable[{0}]: {1} {2}", _doc.DependentVariableIndex, thisTransformation?.RepresentationAsOperator ?? string.Empty, thisFitFunction?.DependentVariableName(_doc.DependentVariableIndex));
      stb.AppendFormat(culture, " ({0} was fitted to : {1})", thisFitFunction?.DependentVariableName(_doc.DependentVariableIndex), fitdoc.FitEnsemble[_doc.FitElementIndex].DependentVariables(_doc.DependentVariableIndex)?.FullName);
      stb.AppendLine();
      stb.AppendLine();

      stb.AppendFormat(culture, "Parameters:");
      stb.AppendLine();
      stb.Append("--------------------------------------------");
      stb.AppendLine();
      stb.AppendFormat(culture, "{0,24}{1,24}{2,24}{3,24}", "Name", "Value", "Variance", "Vary?");
      stb.AppendLine();

      foreach (ParameterSetElement parameter in parameters)
      {
        stb.AppendFormat(culture, "{0,24}{1,24}{2,24}{3,24}", parameter.Name, parameter.Parameter, parameter.Variance, parameter.Vary);
        stb.AppendLine();
      }

      stb.AppendLine();
      stb.AppendLine();
      stb.AppendFormat("Fit elements:");
      stb.AppendLine();
      stb.Append("--------------------------------------------");
      stb.AppendLine();

      for (int iFitElement = 0; iFitElement < fitdoc.FitEnsemble.Count; ++iFitElement)
      {
        stb.AppendFormat(culture, "Fit element[{0}]:", iFitElement);
        stb.AppendLine();
        stb.Append("----------------------");
        stb.AppendLine();

        var fitElement = fitdoc.FitEnsemble[iFitElement];
        stb.AppendFormat(culture, "Fit function: {0}", fitElement.FitFunction?.ToString());
        stb.AppendLine();

        stb.AppendFormat(culture, "DataTable: {0}", fitElement.DataTable?.Name);
        stb.AppendLine();
        stb.AppendFormat(culture, "Group number: {0}", fitElement.GroupNumber);
        stb.AppendLine();
        stb.AppendFormat(culture, "Row selection: {0}", fitElement.DataRowSelection);
        stb.AppendLine();

        for (int i = 0; i < fitElement.NumberOfIndependentVariables; ++i)
        {
          stb.AppendFormat(culture, "IndependentVariable[{0}]: {1} ---> {2}", i, fitElement.FitFunction?.IndependentVariableName(i), fitElement.IndependentVariables(i)?.FullName);
          stb.AppendLine();
        }

        for (int i = 0; i < fitElement.NumberOfDependentVariables; ++i)
        {
          stb.AppendFormat(culture, "DependentVariable[{0}]: {1} ---> {2}", i, fitElement.FitFunction?.DependentVariableName(i), fitElement.DependentVariables(i)?.FullName);
          stb.AppendLine();
        }

        stb.AppendLine();
      }

      return stb.ToString();
    }

    public override bool Apply(bool disposeController)
    {
      return ApplyEnd(true, disposeController);
    }

    protected override void AttachView()
    {
      base.AttachView();
      _view.EditText += this.EhView_EditText;
    }

    protected override void DetachView()
    {
      _view.EditText -= this.EhView_EditText;
      base.DetachView();
    }

    private void EhView_EditText(object sender, EventArgs e)
    {
    }
  }
}
