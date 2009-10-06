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

using Altaxo.Data;
using Altaxo.Graph.Gdi;
using Altaxo.Scripting;
using Altaxo.Gui.Scripting;
using Altaxo.Calc.Regression.Nonlinear;

using Altaxo.Graph.Gdi.Plot;
using Altaxo.Graph.Gdi.Plot.Data;
using Altaxo.Graph.Gdi.Plot.Styles;
using Altaxo.Graph.Plot.Data;

namespace Altaxo.Gui.Analysis.NonLinearFitting
{
	#region interfaces
	public interface INonlinearFitView
	{
		INonlinearFitViewEventSink Controller { get; set; }
		void SetParameterControl(object control);
		void SetSelectFunctionControl(object control);
		void SetFitEnsembleControl(object control);
		void SetChiSquare(double chiSquare);
		void SwitchToFitEnsemblePage();
		object GetGenerationIntervalControl();
	}

	public interface INonlinearFitViewEventSink
	{
		void EhView_DoFit();
		void EhView_DoSimplex();
		void EhView_EvaluateChiSqr();
		void EhView_SelectFitFunction();
		void EhView_NewFitFunction();
		void EhView_CopyParameterV();
		void EhView_CopyParameterVAsCDef();
		void EhView_CopyParameterNV();
		void EhView_CopyParameterNVV();
		void EhView_PasteParameterV();
		void EhView_DoSimulation(bool useInterval, bool generateUnusedVarsAlso);
	}

	#endregion
	/// <summary>
	/// Summary description for NonlinearFitController.
	/// </summary>
	[UserControllerForObject(typeof(NonlinearFitDocument))]
	[ExpectedTypeOfView(typeof(INonlinearFitView))]
	public class NonlinearFitController : INonlinearFitViewEventSink, IMVCAController
	{
		NonlinearFitDocument _doc;
		INonlinearFitView _view;

		IMVCANController _parameterController;
		FitFunctionSelectionController _funcselController;
		IFitEnsembleController _fitEnsembleController;
		Common.EquallySpacedInterval _generationInterval;
		Common.EquallySpacedIntervalController _generationIntervalController;

		double _chiSquare;

		public NonlinearFitController(NonlinearFitDocument doc)
		{
			_doc = doc;
			_parameterController = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _doc.CurrentParameters }, typeof(IMVCANController));
			_fitEnsembleController = (IFitEnsembleController)Current.Gui.GetControllerAndControl(new object[] { _doc.FitEnsemble }, typeof(IFitEnsembleController));

			_funcselController = new FitFunctionSelectionController(_doc.FitEnsemble.Count == 0 ? null : _doc.FitEnsemble[0].FitFunction);
			Current.Gui.FindAndAttachControlTo(_funcselController);

			_doc.FitEnsemble.Changed += new EventHandler(EhFitEnsemble_Changed);

			_generationInterval = new Common.EquallySpacedInterval();
			_generationIntervalController = new Common.EquallySpacedIntervalController();
			_generationIntervalController.InitializeDocument(_generationInterval);
		}

		public void Initialize()
		{
			if (_view != null)
			{
				_view.SetParameterControl(_parameterController.ViewObject);
				_view.SetSelectFunctionControl(_funcselController.ViewObject);
				_view.SetFitEnsembleControl(_fitEnsembleController.ViewObject);
			}
		}

		private void EhFitEnsemble_Changed(object sender, EventArgs e)
		{
			_parameterController.InitializeDocument(_doc.CurrentParameters);
		}

		#region  INonlinearFitViewEventSink

		public void EhView_DoSimplex()
		{
			if (true == this._parameterController.Apply())
			{
				//        _doc.FitEnsemble.InitializeParametersFromParameterSet(_doc.CurrentParameters);

				LevMarAdapter fitAdapter = new LevMarAdapter(_doc.FitEnsemble, _doc.CurrentParameters);

				Current.Gui.ShowBackgroundCancelDialog(10000, new System.Threading.ThreadStart(fitAdapter.DoSimplexMinimization),  null);

				this._chiSquare = fitAdapter.ResultingChiSquare;

				fitAdapter.CopyParametersBackTo(_doc.CurrentParameters);

				//_doc.FitEnsemble.InitializeParametersFromParameterSet(_doc.CurrentParameters);
				//_doc.FitEnsemble.DistributeParameters();

				OnAfterFittingStep();
			}
			else
			{
				Current.Gui.ErrorMessageBox("Some of your parameter input is not valid!");
			}
		}


		public void EhView_DoFit()
		{
			if (true == this._parameterController.Apply())
			{
				//        _doc.FitEnsemble.InitializeParametersFromParameterSet(_doc.CurrentParameters);

				LevMarAdapter fitAdapter = new LevMarAdapter(_doc.FitEnsemble, _doc.CurrentParameters);

				Current.Gui.ShowBackgroundCancelDialog(10000,  new System.Threading.ThreadStart(fitAdapter.Fit), null);

				this._chiSquare = fitAdapter.ResultingChiSquare;

				fitAdapter.CopyParametersBackTo(_doc.CurrentParameters);

				//_doc.FitEnsemble.InitializeParametersFromParameterSet(_doc.CurrentParameters);
				//_doc.FitEnsemble.DistributeParameters();

				OnAfterFittingStep();
			}
			else
			{
				Current.Gui.ErrorMessageBox("Some of your parameter input is not valid!");
			}
		}

		public void EhView_DoSimulation(bool useInterval, bool generateUnusedDependentVariables)
		{
			if (useInterval && !_generationIntervalController.Apply())
			{
				Current.Gui.ErrorMessageBox("Your interval specification contains errors, please correct them!");
				return;
			}
			if (useInterval)
				OnSimulationWithInterval(generateUnusedDependentVariables, _generationInterval);
			else
				OnSimulation(generateUnusedDependentVariables);
		}

		public void EhView_EvaluateChiSqr()
		{
			if (true == this._parameterController.Apply())
			{
				LevMarAdapter fitAdapter = new LevMarAdapter(_doc.FitEnsemble, _doc.CurrentParameters);
				this._chiSquare = fitAdapter.EvaluateChiSquare();
				//_doc.FitEnsemble.InitializeParametersFromParameterSet(_doc.CurrentParameters);
				//_doc.FitEnsemble.DistributeParameters();
				OnAfterFittingStep();
			}
			else
			{
				Current.Gui.ErrorMessageBox("Some of your parameter input is not valid!");
			}
		}


		enum SelectionChoice { SelectAsOnly, SelectAsAdditional };
		SelectionChoice _lastSelectionChoice = SelectionChoice.SelectAsOnly;
		void Select(IFitFunction func)
		{
			bool changed = false;
			if (_doc.FitEnsemble.Count == 0) // Fitting is fresh, we can add the function silently
			{
				FitElement newele = new FitElement();
				newele.FitFunction = func;
				_doc.FitEnsemble.Add(newele);
				_doc.SetDefaultParametersForFitElement(0);
				changed = true;
			}
			else if (_doc.FitEnsemble.Count > 0 && _doc.FitEnsemble[_doc.FitEnsemble.Count - 1].FitFunction == null)
			{
				_doc.FitEnsemble[_doc.FitEnsemble.Count - 1].FitFunction = func;
				_doc.SetDefaultParametersForFitElement(_doc.FitEnsemble.Count - 1);
				changed = true;
			}
			else // Count>0, and there is already a fit function, we
			{ // have to ask the user whether he wants to discard the old functions or keep them

				System.Enum selchoice = _lastSelectionChoice;
				if (Current.Gui.ShowDialog(ref selchoice, "As only or as additional?"))
				{
					_lastSelectionChoice = (SelectionChoice)selchoice;
					if (_lastSelectionChoice == SelectionChoice.SelectAsAdditional)
					{
						FitElement newele = new FitElement();
						newele.FitFunction = func;
						_doc.FitEnsemble.Add(newele);
						_doc.SetDefaultParametersForFitElement(_doc.FitEnsemble.Count - 1);
						changed = true;
					}
					else // select as only
					{
						_doc.FitEnsemble[0].FitFunction = func;
						_doc.SetDefaultParametersForFitElement(0);

						for (int i = _doc.FitEnsemble.Count - 1; i >= 1; --i)
							_doc.FitEnsemble.RemoveAt(i);

						changed = true;
					}
				}
			}

			if (changed)
			{
				// _doc.FitEnsemble.InitializeParameterSetFromEnsembleParameters(_doc.CurrentParameters);

				this._fitEnsembleController.Refresh();

			}
		}


		public void EhView_SelectFitFunction()
		{


			if (_funcselController.Apply())
			{
				Select((IFitFunction)_funcselController.ModelObject);
				_view.SwitchToFitEnsemblePage();
			}

		}

		public void EhView_NewFitFunction()
		{
			FitFunctionScript script = new FitFunctionScript();

			object scriptAsObject = script;
			if (Current.Gui.ShowDialog(ref scriptAsObject, "Create fit function"))
			{
				script = (FitFunctionScript)scriptAsObject;

				Current.Gui.ShowDialog(new FitFunctionNameAndCategoryController(script), "Name your script");

				Current.Project.FitFunctionScripts.Add(script);

				Select(script);
				_funcselController.Refresh();
				_view.SwitchToFitEnsemblePage();

			}
		}

		System.Collections.ArrayList _functionPlotItems = new System.Collections.ArrayList();
		public void OnAfterFittingStep()
		{
			_parameterController.InitializeDocument(_doc.CurrentParameters);

			if (_view != null)
				_view.SetChiSquare(this._chiSquare);

			if (_doc.FitContext is Altaxo.Gui.Graph.Viewing.IGraphController)
			{
				// for every dependent variable in the FitEnsemble, create a function graph
				var graph = _doc.FitContext as Altaxo.Gui.Graph.Viewing.IGraphController;

				int funcNumber = 0;
				for (int i = 0; i < _doc.FitEnsemble.Count; i++)
				{
					FitElement fitEle = _doc.FitEnsemble[i];

					for (int k = 0; k < fitEle.NumberOfDependentVariables; k++, funcNumber++)
					{
						if (funcNumber < _functionPlotItems.Count && _functionPlotItems[funcNumber] != null)
						{
							XYFunctionPlotItem plotItem = (XYFunctionPlotItem)_functionPlotItems[funcNumber];
							FitFunctionToScalarFunctionDDWrapper wrapper = (FitFunctionToScalarFunctionDDWrapper)plotItem.Data.Function;
							wrapper.Initialize(fitEle.FitFunction, k, 0, _doc.GetParametersForFitElement(i));
						}
						else
						{
							FitFunctionToScalarFunctionDDWrapper wrapper = new FitFunctionToScalarFunctionDDWrapper(fitEle.FitFunction, k, _doc.GetParametersForFitElement(i));
							XYFunctionPlotData plotdata = new XYFunctionPlotData(wrapper);
							XYFunctionPlotItem plotItem = new XYFunctionPlotItem(plotdata, new G2DPlotStyleCollection(LineScatterPlotStyleKind.Line));
							graph.ActiveLayer.PlotItems.Add(plotItem);
							_functionPlotItems.Add(plotItem);
						}
					}
				}

				// if there are more elements in _functionPlotItems, remove them from the graph
				for (int i = _functionPlotItems.Count - 1; i >= funcNumber; --i)
				{
					if (_functionPlotItems[i] != null)
					{
						graph.ActiveLayer.PlotItems.Remove((IGPlotItem)_functionPlotItems[i]);
						_functionPlotItems.RemoveAt(i);

					}
				}
				graph.RefreshGraph();
			}
		}

		public void OnSimulation(bool calculateUnusedDependentVariablesAlso)
		{
			// we investigate for every fit element the corresponding table, and add columns to that table
			LevMarAdapter fitAdapter = new LevMarAdapter(_doc.FitEnsemble, _doc.CurrentParameters);

			int numberOfData;

			if (calculateUnusedDependentVariablesAlso)
			{
				numberOfData = 0;
				for (int i = 0; i < _doc.FitEnsemble.Count; i++)
					numberOfData += fitAdapter.GetValidNumericRows(i).Count * _doc.FitEnsemble[i].NumberOfDependentVariables;
			}
			else
			{
				numberOfData = fitAdapter.NumberOfData;
			}



			// calculate the resulting values
			double[] resultingValues = new double[numberOfData];
			fitAdapter.EvaluateFitValues(resultingValues,calculateUnusedDependentVariablesAlso);

			int nextStartOfDependentValues = 0;
			for (int i = 0; i < _doc.FitEnsemble.Count; i++)
			{
				FitElement fitEle = _doc.FitEnsemble[i];
				int startOfDependentValues = nextStartOfDependentValues;
				Collections.IAscendingIntegerCollection validRows = fitAdapter.GetValidNumericRows(i);
				nextStartOfDependentValues += validRows.Count * fitEle.NumberOfDependentVariables;

				Altaxo.Data.DataTable parentTable = fitEle.GetParentDataTable();
				if (parentTable == null)
					continue;

				if (calculateUnusedDependentVariablesAlso)
				{
					// now we have the parent table, we can add columns to it as we need for the independent variables
					int numDependentVars = fitEle.NumberOfDependentVariables;
					for (int k = 0; k < fitEle.NumberOfDependentVariables; k++)
					{
						DoubleColumn col = new DoubleColumn();
						for (int j = 0; j < validRows.Count; j++)
							col[validRows[j]] = resultingValues[startOfDependentValues + k + j * numDependentVars];

						parentTable.DataColumns.Add(col, fitEle.FitFunction.DependentVariableName(k) + ".Sim", ColumnKind.V);
					}
				}
				else // use only the used dependent variables
				{
					int[] inUse = fitAdapter.GetDependentVariablesInUse(i);
					// copy the evaluation result to the output array (interleaved)
					for (int k = 0; k < inUse.Length; ++k)
					{
						DoubleColumn col = new DoubleColumn();
						for (int j = 0; j < validRows.Count; j++)
							col[validRows[j]] = resultingValues[startOfDependentValues + k + j * inUse.Length];

						parentTable.DataColumns.Add(col, fitEle.FitFunction.DependentVariableName(inUse[k]) + ".Sim", ColumnKind.V);
					}
				}
			}
		}


		public void OnSimulationWithInterval(bool calculateUnusedDependentVariablesAlso, Common.EquallySpacedInterval interval)
		{
			// we investigate for every fit element the corresponding table, and add columns to that table
			LevMarAdapter fitAdapter = new LevMarAdapter(_doc.FitEnsemble, _doc.CurrentParameters);

			int intervalCount = (int)interval.Count;
			for (int i = 0; i < _doc.FitEnsemble.Count; i++)
			{
				FitElement fitEle = _doc.FitEnsemble[i];

				Altaxo.Data.DataTable parentTable = fitEle.GetParentDataTable();
				if (parentTable == null)
					continue;


				double[] X = new double[fitEle.NumberOfIndependentVariables];
				double[] Y = new double[fitEle.NumberOfDependentVariables];
				double[] P = new double[fitEle.NumberOfParameters];
				DoubleColumn[] xCols = new DoubleColumn[fitEle.NumberOfIndependentVariables];
				DoubleColumn[] yCols = new DoubleColumn[fitEle.NumberOfDependentVariables];

				for (int k = 0; k < xCols.Length; k++)
					xCols[k] = new DoubleColumn();
				for (int k = 0; k < yCols.Length; k++)
					yCols[k] = new DoubleColumn();

				fitAdapter.GetParameters(i, P);
				for(int k=0;k<intervalCount;k++)
				{
					double xx = interval[k];

					for (int j = 0; j < X.Length; j++)
					{
						X[j] = xx;
						xCols[j][k] = xx;
					}
					fitEle.FitFunction.Evaluate(X, P, Y);
					for (int j = 0; j < Y.Length; j++)
						yCols[j][k] = Y[j];
				}

				int newGroup = parentTable.DataColumns.GetUnusedColumnGroupNumber();


				for (int k = 0; k < fitEle.NumberOfIndependentVariables; k++)
					parentTable.DataColumns.Add(xCols[k], fitEle.FitFunction.DependentVariableName(k) + ".Sim", ColumnKind.X,newGroup);

				if (calculateUnusedDependentVariablesAlso)
				{
					// now we have the parent table, we can add columns to it as we need for the independent variables
					for (int k = 0; k < fitEle.NumberOfDependentVariables; k++)
						parentTable.DataColumns.Add(yCols[k], fitEle.FitFunction.DependentVariableName(k) + ".Sim", ColumnKind.V,newGroup);
				}
				else // use only the used dependent variables
				{
					int[] inUse = fitAdapter.GetDependentVariablesInUse(i);
					// copy the evaluation result to the output array (interleaved)
					for (int k = 0; k < inUse.Length; ++k)
						parentTable.DataColumns.Add(yCols[inUse[k]], fitEle.FitFunction.DependentVariableName(inUse[k]) + ".Sim", ColumnKind.V,newGroup);
				}
			}
		}


		public void EhView_PasteParameterV()
		{
			Altaxo.Data.DataTable table = Altaxo.Worksheet.Commands.EditCommands.GetTableFromClipboard();
			if (null == table)
				return;
			Altaxo.Data.DoubleColumn col = null;
			// Find the first column that contains numeric values
			for (int i = 0; i < table.DataColumnCount; i++)
			{
				if (table[i] is Altaxo.Data.DoubleColumn)
				{
					col = table[i] as Altaxo.Data.DoubleColumn;
					break;
				}
			}
			if (null == col)
				return;

			int len = Math.Max(col.Count, _doc.CurrentParameters.Count);
			for (int i = 0; i < len; i++)
				_doc.CurrentParameters[i].Parameter = col[i];

			_parameterController.InitializeDocument(_doc.CurrentParameters);
		}

		public void EhView_CopyParameterV()
		{
			if (true == this._parameterController.Apply())
			{
				var dao = Current.Gui.GetNewClipboardDataObject();
				Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn();
				for (int i = 0; i < _doc.CurrentParameters.Count; i++)
					col[i] = _doc.CurrentParameters[i].Parameter;

				Altaxo.Data.DataTable tb = new Altaxo.Data.DataTable();
				tb.DataColumns.Add(col, "Value", Altaxo.Data.ColumnKind.V, 0);
				Altaxo.Worksheet.Commands.EditCommands.WriteAsciiToClipBoardIfDataCellsSelected(
					tb, new Altaxo.Collections.AscendingIntegerCollection(),
					new Altaxo.Collections.AscendingIntegerCollection(),
					new Altaxo.Collections.AscendingIntegerCollection(),
					dao);
				Current.Gui.SetClipboardDataObject(dao, true);
			}
			else
			{
				Current.Gui.ErrorMessageBox("Some of your parameter input is not valid!");
			}
		}

		public void EhView_CopyParameterVAsCDef()
		{
			if (true == this._parameterController.Apply())
			{
				var dao = Current.Gui.GetNewClipboardDataObject();
				System.Text.StringBuilder stb = new System.Text.StringBuilder();
				for (int i = 0; i < _doc.CurrentParameters.Count; i++)
				{
					stb.Append("double ");
					stb.Append(_doc.CurrentParameters[i].Name);
					stb.Append(" = ");
					stb.Append(Altaxo.Serialization.NumberConversion.ToString(_doc.CurrentParameters[i].Parameter));
					stb.Append(";\r\n");
				}
				dao.SetData(typeof(string), stb.ToString());
				Current.Gui.SetClipboardDataObject(dao, true);
			}
			else
			{
				Current.Gui.ErrorMessageBox("Some of your parameter input is not valid!");
			}
		}

		public void EhView_CopyParameterNV()
		{
			if (true == this._parameterController.Apply())
			{
				var dao = Current.Gui.GetNewClipboardDataObject();
				Altaxo.Data.TextColumn txt = new Altaxo.Data.TextColumn();
				Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn();

				for (int i = 0; i < _doc.CurrentParameters.Count; i++)
				{
					txt[i] = _doc.CurrentParameters[i].Name;
					col[i] = _doc.CurrentParameters[i].Parameter;
				}

				Altaxo.Data.DataTable tb = new Altaxo.Data.DataTable();
				tb.DataColumns.Add(txt, "Name", Altaxo.Data.ColumnKind.V, 0);
				tb.DataColumns.Add(col, "Value", Altaxo.Data.ColumnKind.V, 0);
				Altaxo.Worksheet.Commands.EditCommands.WriteAsciiToClipBoardIfDataCellsSelected(
					tb, new Altaxo.Collections.AscendingIntegerCollection(),
					new Altaxo.Collections.AscendingIntegerCollection(),
					new Altaxo.Collections.AscendingIntegerCollection(),
					dao);
				Current.Gui.SetClipboardDataObject(dao, true);
			}
			else
			{
				Current.Gui.ErrorMessageBox("Some of your parameter input is not valid!");
			}
		}
		public void EhView_CopyParameterNVV()
		{
			if (true == this._parameterController.Apply())
			{
				var dao = Current.Gui.GetNewClipboardDataObject();
				Altaxo.Data.TextColumn txt = new Altaxo.Data.TextColumn();
				Altaxo.Data.DoubleColumn col = new Altaxo.Data.DoubleColumn();
				Altaxo.Data.DoubleColumn var = new Altaxo.Data.DoubleColumn();

				for (int i = 0; i < _doc.CurrentParameters.Count; i++)
				{
					txt[i] = _doc.CurrentParameters[i].Name;
					col[i] = _doc.CurrentParameters[i].Parameter;
					var[i] = _doc.CurrentParameters[i].Variance;
				}

				Altaxo.Data.DataTable tb = new Altaxo.Data.DataTable();
				tb.DataColumns.Add(txt, "Name", Altaxo.Data.ColumnKind.V, 0);
				tb.DataColumns.Add(col, "Value", Altaxo.Data.ColumnKind.V, 0);
				tb.DataColumns.Add(var, "Variance", Altaxo.Data.ColumnKind.V, 0);
				Altaxo.Worksheet.Commands.EditCommands.WriteAsciiToClipBoardIfDataCellsSelected(
					tb, new Altaxo.Collections.AscendingIntegerCollection(),
					new Altaxo.Collections.AscendingIntegerCollection(),
					new Altaxo.Collections.AscendingIntegerCollection(),
					dao);
				Current.Gui.SetClipboardDataObject(dao, true);
			}
			else
			{
				Current.Gui.ErrorMessageBox("Some of your parameter input is not valid!");
			}
		}

		#endregion

		#region IMVCController Members

		public object ViewObject
		{
			get
			{

				return _view;
			}
			set
			{
				if (_view != null)
					_view.Controller = null;

				_view = value as INonlinearFitView;

				Initialize();

				if (_view != null)
				{
					_view.Controller = this;
					_generationIntervalController.ViewObject = _view.GetGenerationIntervalControl();
				}
			}
		}

		public object ModelObject
		{
			get
			{
				return _doc;
			}
		}

		#endregion

		#region IApplyController Members

		public bool Apply()
		{
			return true;
		}

		#endregion


	}
}
