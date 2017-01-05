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

using System.Collections.Generic;
using Altaxo.Calc.Regression.Nonlinear;
using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Plot;
using Altaxo.Graph.Gdi.Plot.Styles;
using Altaxo.Graph.Plot.Data;
using Altaxo.Gui.Scripting;
using Altaxo.Scripting;
using System;
using Altaxo.Data.Transformations;

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

	#endregion interfaces

	/// <summary>
	/// Summary description for NonlinearFitController.
	/// </summary>
	/// <seealso cref="Altaxo.Gui.Analysis.NonLinearFitting.INonlinearFitViewEventSink" />
	/// <seealso cref="Altaxo.Gui.IMVCAController" />
	[UserControllerForObject(typeof(NonlinearFitDocument))]
	[ExpectedTypeOfView(typeof(INonlinearFitView))]
	public class NonlinearFitController : INonlinearFitViewEventSink, IMVCANController
	{
		private NonlinearFitDocument _doc;
		private INonlinearFitView _view;

		private IMVCANController _parameterController;
		private FitFunctionSelectionController _funcselController;
		private IFitEnsembleController _fitEnsembleController;
		private Common.EquallySpacedInterval _generationInterval;
		private Common.EquallySpacedIntervalController _generationIntervalController;
		private double _chiSquare;

		/// <summary>
		/// If a fit was made, new function plot items with a new Guid identifier are created. This is the identifier of the old function plot items.
		/// The identifier is used to identify the old function plot items in the layer, and to replace them by the new items.
		/// </summary>
		protected string _previousFitDocumentIdentifier;

		private XYPlotLayer _activeLayer;

		public bool InitializeDocument(params object[] args)
		{
			if (args == null || args.Length == 0)
				return false;

			if (!(args[0] is NonlinearFitDocument))
				return false;

			_doc = (NonlinearFitDocument)args[0];

			if (args.Length > 1 && args[1] is string)
				_previousFitDocumentIdentifier = (string)args[1];

			if (args.Length > 2 && args[2] is XYPlotLayer)
				_activeLayer = (XYPlotLayer)args[2];

			Initialize(true);
			return true;
		}

		public UseDocument UseDocumentCopy { get; set; }

		public void Initialize(bool initData)
		{
			if (initData)
			{
				_parameterController = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _doc.CurrentParameters }, typeof(IMVCANController));
				_fitEnsembleController = (IFitEnsembleController)Current.Gui.GetControllerAndControl(new object[] { _doc.FitEnsemble }, typeof(IFitEnsembleController));

				_funcselController = new FitFunctionSelectionController(_doc.FitEnsemble.Count == 0 ? null : _doc.FitEnsemble[0].FitFunction);
				Current.Gui.FindAndAttachControlTo(_funcselController);

				{
					var fitEnsemble = _doc.FitEnsemble;
					fitEnsemble.Changed += new WeakEventHandler(EhFitEnsemble_Changed, handler => fitEnsemble.Changed -= handler);
				}

				_generationInterval = new Common.EquallySpacedInterval();
				_generationIntervalController = new Common.EquallySpacedIntervalController();
				_generationIntervalController.InitializeDocument(_generationInterval);
			}

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

		#region INonlinearFitViewEventSink

		public void EhView_EvaluateChiSqr()
		{
			if (true == this._parameterController.Apply(false))
			{
				LevMarAdapter fitAdapter = new LevMarAdapter(_doc.FitEnsemble, _doc.CurrentParameters);

				var fitThread = new System.Threading.Thread(new System.Threading.ThreadStart(() => this._chiSquare = fitAdapter.EvaluateChiSquare()));
				fitThread.Start();
				Current.Gui.ShowBackgroundCancelDialog(10000, null, fitThread);
				if (!(fitThread.ThreadState.HasFlag(System.Threading.ThreadState.Aborted)))
				{
					OnAfterFittingStep();
				}
			}
			else
			{
				Current.Gui.ErrorMessageBox("Some of your parameter input is not valid!");
			}
		}

		public void EhView_DoFit()
		{
			if (true == this._parameterController.Apply(false))
			{
				//        _doc.FitEnsemble.InitializeParametersFromParameterSet(_doc.CurrentParameters);

				LevMarAdapter fitAdapter = new LevMarAdapter(_doc.FitEnsemble, _doc.CurrentParameters);

				var fitThread = new System.Threading.Thread(new System.Threading.ThreadStart(fitAdapter.Fit));
				fitThread.Start();
				Current.Gui.ShowBackgroundCancelDialog(10000, null, fitThread);
				if (!(fitThread.ThreadState.HasFlag(System.Threading.ThreadState.Aborted)))
				{
					this._chiSquare = fitAdapter.ResultingChiSquare;

					fitAdapter.CopyParametersBackTo(_doc.CurrentParameters);

					//_doc.FitEnsemble.InitializeParametersFromParameterSet(_doc.CurrentParameters);
					//_doc.FitEnsemble.DistributeParameters();

					OnAfterFittingStep();
				}
			}
			else
			{
				Current.Gui.ErrorMessageBox("Some of your parameter input is not valid!");
			}
		}

		public void EhView_DoSimplex()
		{
			if (true == this._parameterController.Apply(false))
			{
				//        _doc.FitEnsemble.InitializeParametersFromParameterSet(_doc.CurrentParameters);

				LevMarAdapter fitAdapter = new LevMarAdapter(_doc.FitEnsemble, _doc.CurrentParameters);

				var fitThread = new System.Threading.Thread(new System.Threading.ThreadStart(fitAdapter.DoSimplexMinimization));
				fitThread.Start();
				Current.Gui.ShowBackgroundCancelDialog(10000, null, fitThread);
				if (!(fitThread.ThreadState.HasFlag(System.Threading.ThreadState.Aborted)))
				{
					this._chiSquare = fitAdapter.ResultingChiSquare;

					fitAdapter.CopyParametersBackTo(_doc.CurrentParameters);

					//_doc.FitEnsemble.InitializeParametersFromParameterSet(_doc.CurrentParameters);
					//_doc.FitEnsemble.DistributeParameters();

					OnAfterFittingStep();
				}
			}
			else
			{
				Current.Gui.ErrorMessageBox("Some of your parameter input is not valid!");
			}
		}

		public void EhView_DoSimulation(bool useInterval, bool generateUnusedDependentVariables)
		{
			if (useInterval && !_generationIntervalController.Apply(false))
			{
				Current.Gui.ErrorMessageBox("Your interval specification contains errors, please correct them!");
				return;
			}

			System.Threading.Thread simulationThread;
			if (useInterval)
			{
				var generationInterval = _generationInterval; // use local variable for thread
				simulationThread = new System.Threading.Thread(new System.Threading.ThreadStart(() => OnSimulationWithInterval(generateUnusedDependentVariables, generationInterval)));
			}
			else
			{
				simulationThread = new System.Threading.Thread(new System.Threading.ThreadStart(() => OnSimulation(generateUnusedDependentVariables)));
			}

			simulationThread.Start();
			Current.Gui.ShowBackgroundCancelDialog(10000, null, simulationThread);
		}

		private enum SelectionChoice { SelectAsOnly, SelectAsAdditional };

		private SelectionChoice _lastSelectionChoice = SelectionChoice.SelectAsOnly;

		private void Select(IFitFunction func)
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
			if (_funcselController.Apply(false))
			{
				Select((IFitFunction)_funcselController.ModelObject);
				_view.SwitchToFitEnsemblePage();
			}
		}

		public void EhView_NewFitFunction()
		{
			FitFunctionScript script = new FitFunctionScript();

			Label_EditScript:
			object scriptAsObject = script;
			if (Current.Gui.ShowDialog(ref scriptAsObject, "Create fit function"))
			{
				script = (FitFunctionScript)scriptAsObject;

				var nameController = new FitFunctionNameAndCategoryController();
				nameController.InitializeDocument(script);
				if (!Current.Gui.ShowDialog(nameController, "Name your script"))
					goto Label_EditScript;

				Current.Project.FitFunctionScripts.Add(script);

				Select(script);
				_funcselController.Refresh();
				_view.SwitchToFitEnsemblePage();
			}
		}

		public void OnAfterFittingStep()
		{
			_parameterController.InitializeDocument(_doc.CurrentParameters);

			if (_view != null)
				_view.SetChiSquare(this._chiSquare);

			if (null != _activeLayer)
			{
				CreateOrReplaceFunctionPlotItems(_activeLayer);
			}
		}

		private void CreateOrReplaceFunctionPlotItems(XYPlotLayer xylayer)
		{
			// collect the old plot items and put them into a dictionary whose key is a combination of fit element index and dependentVariableIndex
			var oldItemsDictionary = new Dictionary<Tuple<int, int>, XYNonlinearFitFunctionPlotItem>();
			// dictionary with holds the available y-data columns of this layer and their transformations.
			var dataColumnsAndTheirTransformation = new Dictionary<DataColumn, IVariantToVariantTransformation>();

			foreach (var pi in TreeNodeExtensions.TakeFromHereToFirstLeaves((IGPlotItem)xylayer.PlotItems))
			{
				if (pi is XYNonlinearFitFunctionPlotItem plotItem && plotItem.FitDocumentIdentifier == _previousFitDocumentIdentifier)
				{
					oldItemsDictionary.Add(new Tuple<int, int>(plotItem.FitElementIndex, plotItem.DependentVariableIndex), plotItem);
				}

				if (pi is XYColumnPlotItem columnPlotItem)
				{
					SplitInDataColumnAndTransformation(columnPlotItem.Data.YColumn, out var dataColumn, out var transfo);
					if (null != dataColumn)
						dataColumnsAndTheirTransformation[dataColumn] = transfo;
				}
			}

			var newFitDocumentIdentifier = Guid.NewGuid().ToString(); // used to identify all curves with the same fit document

			// now create a plot item for each dependent variable, reuse if possible the style from the old items
			int funcNumber = 0;
			for (int idxFitEnsemble = 0; idxFitEnsemble < _doc.FitEnsemble.Count; idxFitEnsemble++)
			{
				FitElement fitEle = _doc.FitEnsemble[idxFitEnsemble];

				for (int idxDependentVariable = 0; idxDependentVariable < fitEle.NumberOfDependentVariables; idxDependentVariable++, funcNumber++)
				{
					oldItemsDictionary.TryGetValue(new Tuple<int, int>(idxFitEnsemble, idxDependentVariable), out var oldPlotItem);
					var newPlotStyle = oldPlotItem?.Style.Clone() ?? new G2DPlotStyleCollection(LineScatterPlotStyleKind.Line, xylayer.GetPropertyContext());

					// get the transformation for the plot item
					IVariantToVariantTransformation functionPlotItemTransformation = null;
					var depVar = fitEle.DependentVariables(idxDependentVariable);
					SplitInDataColumnAndTransformation(depVar, out var depVarDataColumn, out var transformationOfFitDependentVariable);
					if (null != depVarDataColumn && dataColumnsAndTheirTransformation.TryGetValue(depVarDataColumn, out var transformationOfOriginalDataColumn))
					{
						functionPlotItemTransformation = GetFitFunctionTransformation(transformationOfOriginalDataColumn, transformationOfFitDependentVariable);
					}

					var newPlotItem = new XYNonlinearFitFunctionPlotItem(newFitDocumentIdentifier, _doc, idxFitEnsemble, idxDependentVariable, functionPlotItemTransformation, newPlotStyle);

					if (null != oldPlotItem)
					{
						oldPlotItem.ParentCollection.Replace(oldPlotItem, newPlotItem);
					}
					else
					{
						xylayer.PlotItems.Add(newPlotItem);
					}
				}
			}

			_previousFitDocumentIdentifier = newFitDocumentIdentifier;
		}

		private IVariantToVariantTransformation GetFitFunctionTransformation(IVariantToVariantTransformation transformationOfOriginalDataColumn, IVariantToVariantTransformation transformationOfFitDependentVariable)
		{
			if (null == transformationOfOriginalDataColumn && null == transformationOfFitDependentVariable)
			{
				return null;
			}
			else if (null != transformationOfOriginalDataColumn && null == transformationOfFitDependentVariable)
			{
				return transformationOfOriginalDataColumn;
			}
			else if (null == transformationOfOriginalDataColumn && null != transformationOfFitDependentVariable)
			{
				return transformationOfFitDependentVariable.BackTransformation;
			}
			else // both transformations are not null
			{
				return CompoundTransformation.TryGetCompoundTransformationWithSimplification(new[] { transformationOfOriginalDataColumn, transformationOfFitDependentVariable.BackTransformation });
			}
		}

		private void SplitInDataColumnAndTransformation(IReadableColumn ycolumn, out DataColumn dataColumn, out IVariantToVariantTransformation transformation)
		{
			dataColumn = null;
			transformation = null;

			if (ycolumn is ITransformedReadableColumn tyc)
			{
				var originalDataColumn = tyc.GetUnderlyingDataColumnOrDefault();
				if (null != originalDataColumn)
				{
					dataColumn = originalDataColumn;
					transformation = tyc.Transformation;
				}
			}
			else if (ycolumn is DataColumn dc)
			{
				dataColumn = dc;
				transformation = null;
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
			fitAdapter.EvaluateFitValues(resultingValues, calculateUnusedDependentVariablesAlso);

			int nextStartOfDependentValues = 0;
			for (int i = 0; i < _doc.FitEnsemble.Count; i++)
			{
				FitElement fitEle = _doc.FitEnsemble[i];
				int startOfDependentValues = nextStartOfDependentValues;
				Collections.IAscendingIntegerCollection validRows = fitAdapter.GetValidNumericRows(i);
				nextStartOfDependentValues += validRows.Count * fitEle.NumberOfUsedDependentVariables;

				Altaxo.Data.DataTable parentTable = fitEle.DataTable;
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

						string name = null;
						int groupNumber = 0;
						if (fitEle.DependentVariables(inUse[k]) is DataColumn)
						{
							var srcCol = (DataColumn)fitEle.DependentVariables(inUse[k]);
							var srcTable = DataColumnCollection.GetParentDataColumnCollectionOf(srcCol);
							if (srcTable != null)
							{
								name = srcTable.GetColumnName(srcCol);
								groupNumber = srcTable.GetColumnGroup(srcCol);
							}
						}
						if (null == name)
							fitEle.FitFunction.DependentVariableName(inUse[k]);

						parentTable.DataColumns.Add(col, name + ".Sim", ColumnKind.V, groupNumber);
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

				Altaxo.Data.DataTable parentTable = fitEle.DataTable;
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
				for (int k = 0; k < intervalCount; k++)
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
					parentTable.DataColumns.Add(xCols[k], fitEle.FitFunction.DependentVariableName(k) + ".Sim", ColumnKind.X, newGroup);

				if (calculateUnusedDependentVariablesAlso)
				{
					// now we have the parent table, we can add columns to it as we need for the independent variables
					for (int k = 0; k < fitEle.NumberOfDependentVariables; k++)
						parentTable.DataColumns.Add(yCols[k], fitEle.FitFunction.DependentVariableName(k) + ".Sim", ColumnKind.V, newGroup);
				}
				else // use only the used dependent variables
				{
					int[] inUse = fitAdapter.GetDependentVariablesInUse(i);
					// copy the evaluation result to the output array (interleaved)
					for (int k = 0; k < inUse.Length; ++k)
						parentTable.DataColumns.Add(yCols[inUse[k]], fitEle.FitFunction.DependentVariableName(inUse[k]) + ".Sim", ColumnKind.V, newGroup);
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
			if (true == this._parameterController.Apply(false))
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
			if (true == this._parameterController.Apply(false))
			{
				var dao = Current.Gui.GetNewClipboardDataObject();
				System.Text.StringBuilder stb = new System.Text.StringBuilder();
				for (int i = 0; i < _doc.CurrentParameters.Count; i++)
				{
					stb.AppendFormat(System.Globalization.CultureInfo.InvariantCulture, "double {0} = {1};\r\n", _doc.CurrentParameters[i].Name, _doc.CurrentParameters[i].Parameter);
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
			if (true == this._parameterController.Apply(false))
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
			if (true == this._parameterController.Apply(false))
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

		#endregion INonlinearFitViewEventSink

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

				if (_view != null)
				{
					_view.Controller = this;
					Initialize(false);
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

		public void Dispose()
		{
		}

		#endregion IMVCController Members

		#region IApplyController Members

		public bool Apply(bool disposeController)
		{
			return true;
		}

		/// <summary>
		/// Try to revert changes to the model, i.e. restores the original state of the model.
		/// </summary>
		/// <param name="disposeController">If set to <c>true</c>, the controller should release all temporary resources, since the controller is not needed anymore.</param>
		/// <returns>
		///   <c>True</c> if the revert operation was successfull; <c>false</c> if the revert operation was not possible (i.e. because the controller has not stored the original state of the model).
		/// </returns>
		public bool Revert(bool disposeController)
		{
			return false;
		}

		#endregion IApplyController Members
	}
}