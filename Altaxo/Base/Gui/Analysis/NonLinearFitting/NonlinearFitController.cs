#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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
using System.Threading;
using System.Windows.Input;
using Altaxo.Calc.LinearAlgebra;
using Altaxo.Calc.Optimization;
using Altaxo.Calc.Regression.Nonlinear;
using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Data.Transformations;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Plot;
using Altaxo.Graph.Gdi.Plot.Styles;
using Altaxo.Graph.Plot.Data;
using Altaxo.Gui.Scripting;
using Altaxo.Main.Services;
using Altaxo.Scripting;
using Altaxo.Units;

namespace Altaxo.Gui.Analysis.NonLinearFitting
{
  public interface INonlinearFitView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Summary description for NonlinearFitController.
  /// </summary>
  /// <seealso cref="Altaxo.Gui.IMVCAController" />
  [UserControllerForObject(typeof(NonlinearFitDocument))]
  [ExpectedTypeOfView(typeof(INonlinearFitView))]
  public class NonlinearFitController : MVCANControllerEditImmutableDocBase<NonlinearFitDocument, INonlinearFitView>
  {
    private ISpacedInterval _generationInterval;
    private double _sigmaSquare;
    private int _numberOfFitPoints;
    private double[] _covarianceMatrix; // length of covariance matrix is always a square number

    /// <summary>
    /// If a fit was made, new function plot items with a new Guid identifier are created. This is the identifier of the old function plot items.
    /// The identifier is used to identify the old function plot items in the layer, and to replace them by the new items.
    /// </summary>
    protected string _previousFitDocumentIdentifier;

    private XYPlotLayer _activeLayer;

    public override bool InitializeDocument(params object[] args)
    {
      if (args is null || args.Length == 0)
        return false;

      if (args[0] is NonlinearFitDocument fitdoc)
        _originalDoc = _doc = fitdoc;
      else
        return false;


      if (args.Length > 1 && args[1] is string prevId)
        _previousFitDocumentIdentifier = prevId;

      if (args.Length > 2 && args[2] is XYPlotLayer plotLayer)
        _activeLayer = plotLayer;

      Initialize(true);
      return true;
    }

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_fitFunctionSelectionController, () => FitFunctionSelectionController = null);
      yield return new ControllerAndSetNullMethod(_fitEnsembleController, () => FitEnsembleController = null);
      yield return new ControllerAndSetNullMethod(_parameterController, () => ParameterController = null);
      yield return new ControllerAndSetNullMethod(_generationIntervalController, () => GenerationIntervalController = null);
    }

    #region Bindings

    #region Tabs

    public const string TabSelection = "Selection";
    public const string TabDetails = "Details";
    public const string TabFit = "Fit";
    public const string TabSimulation = "Simulation";

    private string _selectedTab;

    public string SelectedTab
    {
      get => _selectedTab;
      set
      {
        if (!(_selectedTab == value))
        {
          _selectedTab = value;
          OnPropertyChanged(nameof(SelectedTab));
        }
      }
    }


    #endregion

    #region FitFunctionSelection
    private ICommand _cmdSelectFitFunction;
    public ICommand CmdSelectFitFunction => _cmdSelectFitFunction ??= new RelayCommand(EhView_SelectFitFunction);

    private ICommand _cmdCreateNewFitFunction;
    public ICommand CmdCreateNewFitFunction => _cmdCreateNewFitFunction ??= new RelayCommand(EhView_NewFitFunction);

    private FitFunctionSelectionController _fitFunctionSelectionController;

    public FitFunctionSelectionController FitFunctionSelectionController
    {
      get => _fitFunctionSelectionController;
      set
      {
        if (!(_fitFunctionSelectionController == value))
        {
          _fitFunctionSelectionController?.Dispose();
          _fitFunctionSelectionController = value;
          OnPropertyChanged(nameof(FitFunctionSelectionController));
        }
      }
    }

    #endregion

    #region FitEnsemble


    private IMVCANController _fitEnsembleController;

    public IMVCANController FitEnsembleController
    {
      get => _fitEnsembleController;
      set
      {
        if (!(_fitEnsembleController == value))
        {
          _fitEnsembleController?.Dispose();
          _fitEnsembleController = value;
          OnPropertyChanged(nameof(FitEnsembleController));
        }
      }
    }


    #endregion

    #region MakeFit

    private ICommand _cmdCopyParameterNV;
    public ICommand CmdCopyParameterNV => _cmdCopyParameterNV ??= new RelayCommand(EhView_CopyParameterNV);

    private ICommand _cmdCopyParameterNVV;
    public ICommand CmdCopyParameterNVV => _cmdCopyParameterNVV ??= new RelayCommand(EhView_CopyParameterNVV);

    private ICommand _cmdCopyParameterVAsCDef;
    public ICommand CmdCopyParameterVAsCDef => _cmdCopyParameterVAsCDef ??= new RelayCommand(EhView_CopyParameterVAsCDef);

    private ICommand _cmdCopyParameterV;
    public ICommand CmdCopyParameterV => _cmdCopyParameterV ??= new RelayCommand(EhView_CopyParameterV);

    private ICommand _cmdCopyParameterWithCVM;
    public ICommand CmdCopyParameterWithCVM => _cmdCopyParameterWithCVM ??= new RelayCommand(EhView_CopyParameterNCM);


    private ICommand _cmdCopyParameterNSVCVInOneRow;
    public ICommand CmdCopyParameterNSVCVInOneRow => _cmdCopyParameterNSVCVInOneRow ??= new RelayCommand(EhView_CopyParameterNSVCVInOneRow);


    private ICommand _cmdPasteParameterV;
    public ICommand CmdPasteParameterV => _cmdPasteParameterV ??= new RelayCommand(EhView_PasteParameterV);

    private ICommand _cmdEvaluateChiSquare;
    public ICommand CmdEvaluateChiSquare => _cmdEvaluateChiSquare ??= new RelayCommand(EhView_EvaluateChiSqr);

    private ICommand _cmdDoFit;
    public ICommand CmdDoFit => _cmdDoFit ??= new RelayCommand(EhView_DoFit);

    private ICommand _cmdDoSimplex;
    public ICommand CmdDoSimplex => _cmdDoSimplex ??= new RelayCommand(EhView_DoSimplex);

    private double _chiSquareValue;

    public double ChiSquareValue
    {
      get => _chiSquareValue;
      set
      {
        if (!(_chiSquareValue == value))
        {
          _chiSquareValue = value;
          OnPropertyChanged(nameof(ChiSquareValue));
        }
      }
    }


    private bool _showUnusedDependentVariables = true;

    public bool ShowUnusedDependentVariables
    {
      get => _showUnusedDependentVariables;
      set
      {
        if (!(_showUnusedDependentVariables == value))
        {
          _showUnusedDependentVariables = value;
          OnPropertyChanged(nameof(ShowUnusedDependentVariables));
        }
      }
    }

    private bool _showConfidenceBands;

    public bool ShowConfidenceBands
    {
      get => _showConfidenceBands;
      set
      {
        if (!(_showConfidenceBands == value))
        {
          _showConfidenceBands = value;
          OnPropertyChanged(nameof(ShowConfidenceBands));
        }
      }
    }

    public QuantityWithUnitGuiEnvironment ConfidenceLevelEnvironment => RelationEnvironment.Instance;

    private DimensionfulQuantity _confidenceLevel = new DimensionfulQuantity(0.95, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(RelationEnvironment.Instance.DefaultUnit);

    public DimensionfulQuantity ConfidenceLevel
    {
      get => _confidenceLevel;
      set
      {
        if (!(_confidenceLevel == value))
        {
          _confidenceLevel = value;
          OnPropertyChanged(nameof(ConfidenceLevel));
        }
      }
    }




    private IMVCANController _parameterController;

    public IMVCANController ParameterController
    {
      get => _parameterController;
      set
      {
        if (!(_parameterController == value))
        {
          _parameterController?.Dispose();
          _parameterController = value;
          OnPropertyChanged(nameof(ParameterController));
        }
      }
    }


    #endregion

    #region Simulate

    private bool _useUnusedDependentVarsAlsoInSimulation;

    public bool UseUnusedDependentVarsAlsoInSimulation
    {
      get => _useUnusedDependentVarsAlsoInSimulation;
      set
      {
        if (!(_useUnusedDependentVarsAlsoInSimulation == value))
        {
          _useUnusedDependentVarsAlsoInSimulation = value;
          OnPropertyChanged(nameof(UseUnusedDependentVarsAlsoInSimulation));
        }
      }
    }

    private ICommand _cmdDoSimulation;
    public ICommand CmdDoSimulation => _cmdDoSimulation ??= new RelayCommand(EhView_DoSimulation);


    private bool _simulationGenerateFromIndependentVars;

    public bool SimulationGenerateFromIndependentVars
    {
      get => _simulationGenerateFromIndependentVars;
      set
      {
        if (!(_simulationGenerateFromIndependentVars == value))
        {
          _simulationGenerateFromIndependentVars = value;
          OnPropertyChanged(nameof(SimulationGenerateFromIndependentVars));
          OnPropertyChanged(nameof(SimulationFromEquallySpacedInterval));
        }
      }
    }

    public bool SimulationFromEquallySpacedInterval
    {
      get => !SimulationGenerateFromIndependentVars;
      set => SimulationGenerateFromIndependentVars = !value;
    }


    private Common.EquallySpacedIntervalController _generationIntervalController;

    public Common.EquallySpacedIntervalController GenerationIntervalController
    {
      get => _generationIntervalController;
      set
      {
        if (!(_generationIntervalController == value))
        {
          _generationIntervalController?.Dispose();
          _generationIntervalController = value;
          OnPropertyChanged(nameof(GenerationIntervalController));
        }
      }
    }


    #endregion

    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        ParameterController = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _doc.CurrentParameters }, typeof(IMVCANController));
        FitEnsembleController = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _doc.FitEnsemble }, typeof(IMVCANController));

        var fitFunctionSelectionController = new FitFunctionSelectionController(_doc.FitEnsemble.Count == 0 ? null : _doc.FitEnsemble[0].FitFunction);
        fitFunctionSelectionController.FitFunctionSelected += EhController_SelectFitFunction;
        Current.Gui.FindAndAttachControlTo(fitFunctionSelectionController);
        FitFunctionSelectionController = fitFunctionSelectionController;

        {
          var fitEnsemble = _doc.FitEnsemble;
          fitEnsemble.Changed += new WeakEventHandler(EhFitEnsemble_Changed, fitEnsemble, nameof(fitEnsemble.Changed));
        }

        _generationInterval = new LinearlySpacedIntervalByStartCountStep(0, 1000, 1);
        var generationIntervalController = new Common.EquallySpacedIntervalController();
        generationIntervalController.InitializeDocument(_generationInterval);
        GenerationIntervalController = generationIntervalController;

        if (_activeLayer is not null)
        {
          var (hasConfidenceItems, confidenceLevel) = HasConfidencePlotItems(_activeLayer);
          ShowConfidenceBands = hasConfidenceItems;
          if (hasConfidenceItems)
          {
            ConfidenceLevel = new DimensionfulQuantity(confidenceLevel, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(ConfidenceLevelEnvironment.DefaultUnit);
          }
        }

        SelectedTab = TabSelection;
      }
    }

    private void EhFitEnsemble_Changed(object sender, EventArgs e)
    {
      _parameterController?.InitializeDocument(_doc.CurrentParameters);

    }


    public void EhView_EvaluateChiSqr()
    {
      if (true == _parameterController.Apply(false))
      {
        var fitAdapter = new LevMarAdapter2(_doc.FitEnsemble, _doc.CurrentParameters);

        var fitThread = new System.Threading.Thread(new System.Threading.ThreadStart(() => ChiSquareValue = fitAdapter.Value));
        fitThread.Start();
        Current.Gui.ShowBackgroundCancelDialog(10000, fitThread, null);
        if (!(fitThread.ThreadState.HasFlag(System.Threading.ThreadState.Aborted)))
        {
          _covarianceMatrix = null;
          _numberOfFitPoints = fitAdapter.NumberOfObservations;
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
      if (_showConfidenceBands)
      {
        var level = ConfidenceLevel.AsValueInSIUnits;
        if (!(level > 0 && level < 1))
        {
          Current.Gui.ErrorMessageBox("Confidence level must be > 0 and < 1");
          return;
        }
        _confidenceLevel = new DimensionfulQuantity(level, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(ConfidenceLevelEnvironment.DefaultUnit);
      }

      if (true == _parameterController.Apply(false))
      {
        // test if there are any parameters to vary
        if (_doc.CurrentParameters.Where(x => x.Vary).FirstOrDefault() is null)
        {
          Current.Gui.ErrorMessageBox("You should select at least one parameter to vary!");
          return;
        }

        var fitAdapter = new LevMarAdapter2(_doc.FitEnsemble, _doc.CurrentParameters);
        var fit = new LevenbergMarquardtMinimizerNonAllocating();


        var backgroundMonitor = new ExternalDrivenBackgroundMonitor();
        var initialGuess = _doc.CurrentParameters.Where(e => e.Vary == true).Select(e => e.Parameter).ToList();
        var fitThread = new System.Threading.Thread(new System.Threading.ThreadStart(() => fit.FindMinimum(fitAdapter, initialGuess, null, null, null, null, backgroundMonitor.CancellationTokenHard)));
        fitThread.Start();
        Current.Gui.ShowBackgroundCancelDialog(10000, fitThread, backgroundMonitor);
        if (!(fitThread.ThreadState.HasFlag(System.Threading.ThreadState.Aborted)))
        {
          ChiSquareValue = fitAdapter.Value;
          _sigmaSquare = fitAdapter.SigmaSquare;
          _numberOfFitPoints = fitAdapter.NumberOfObservations;
          _covarianceMatrix = (double[])fitAdapter.CovarianceMatrix.Clone();

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

    private class ReportCostMonitor : ExternalDrivenBackgroundMonitor
    {
      private CancellationTokenSource _cancellationTokenSource;

      private string _reportText = "Operation in progress - please wait ...";
      private bool _reportTextDirty = true;

      public ReportCostMonitor()
      {
        _cancellationTokenSource = new CancellationTokenSource();
      }

      public CancellationToken GetCancellationToken()
      {
        return _cancellationTokenSource.Token;
      }

      public void NewMinimumCostValueAvailable(double minCost)
      {
        _reportText = string.Format("Minimum cost value so far: {0}", minCost);
        _reportTextDirty = true;
      }

      public new bool CancellationPending => _cancellationTokenSource.IsCancellationRequested;

      public new bool HasReportText => _reportTextDirty;

      public new bool ShouldReportNow => _reportTextDirty;

      public new double GetProgressFraction()
      {
        return 0;
      }

      public new string GetReportText()
      {
        return _reportText;
      }

      public new void ReportProgress(string text)
      {
      }

      public new void ReportProgress(string text, double progressValue)
      {
      }

      public new void SetCancellationPending()
      {
        _cancellationTokenSource.Cancel();
      }

      public new void SetShouldReportNow()
      {
        _reportTextDirty = true;
      }
    }

    public void EhView_DoSimplex()
    {
      if (true == _parameterController.Apply(false))
      {
        // test if there are any parameters to vary
        if (_doc.CurrentParameters.Where(x => x.Vary).FirstOrDefault() is null)
        {
          Current.Gui.ErrorMessageBox("You should select at least one parameter to vary!");
          return;
        }

        //        _doc.FitEnsemble.InitializeParametersFromParameterSet(_doc.CurrentParameters);

        var fitAdapter = new LevMarAdapter2(_doc.FitEnsemble, _doc.CurrentParameters);
        var fit = new Altaxo.Calc.Optimization.NelderMeadSimplex(1E-7, 200);
        var initialGuess = Vector<double>.Build.DenseOfEnumerable(_doc.CurrentParameters.Where(e => e.Vary == true).Select(e => e.Parameter));

        var reportMonitor = new ReportCostMonitor();
        var threadStart = new System.Threading.ThreadStart(() => fit.FindMinimum(fitAdapter.ToObjectiveFunction(), initialGuess)); //(reportMonitor.GetCancellationToken(), reportMonitor.NewMinimumCostValueAvailable))
        var fitThread = new System.Threading.Thread(threadStart);
        fitThread.Start();
        Current.Gui.ShowBackgroundCancelDialog(10000, fitThread, reportMonitor);
        if (!(fitThread.ThreadState.HasFlag(System.Threading.ThreadState.Aborted)))
        {
          ChiSquareValue = fitAdapter.Value;

          fitAdapter.CopyParametersBackTo(_doc.CurrentParameters);

          // remove covariance matrix because we don't have covariances now
          _covarianceMatrix = null;

          OnAfterFittingStep();
        }
      }
      else
      {
        Current.Gui.ErrorMessageBox("Some of your parameter input is not valid!");
      }
    }

    public void EhView_DoSimulation()
    {
      var useInterval = SimulationFromEquallySpacedInterval;
      var generateUnusedDependentVariables = UseUnusedDependentVarsAlsoInSimulation;

      if (useInterval)
      {
        if (_generationIntervalController.Apply(false))
        {
          _generationInterval = (ISpacedInterval)GenerationIntervalController.ModelObject;
        }
        else
        {
          Current.Gui.ErrorMessageBox("Your interval specification contains errors, please correct them!");
          return;
        }
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
      Current.Gui.ShowBackgroundCancelDialog(10000, simulationThread, null);
    }

    private enum SelectionChoice { SelectAsOnly, SelectAsAdditional, SelectAsReplacementForAllExisting };

    private SelectionChoice _lastSelectionChoice = SelectionChoice.SelectAsOnly;

    private void Select(IFitFunction func)
    {
      bool changed = false;
      if (_doc.FitEnsemble.Count == 0) // Fitting is fresh, we can add the function silently
      {
        var newele = new FitElement(func);
        _doc.FitEnsemble.Add(newele);
        _doc.SetDefaultParametersForFitElement(0);
        changed = true;
      }
      else if (_doc.FitEnsemble.Count > 0 && _doc.FitEnsemble[_doc.FitEnsemble.Count - 1].FitFunction is null)
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

          switch (_lastSelectionChoice)
          {
            case SelectionChoice.SelectAsOnly:
              {
                for (int i = _doc.FitEnsemble.Count - 1; i >= 1; --i)
                  _doc.FitEnsemble.RemoveAt(i);

                _doc.FitEnsemble[0].FitFunction = func;

                _doc.SetDefaultParametersForFitElement(0);

                changed = true;
              }
              break;

            case SelectionChoice.SelectAsAdditional:
              {
                var newele = new FitElement(func);
                _doc.FitEnsemble.Add(newele);
                _doc.SetDefaultParametersForFitElement(_doc.FitEnsemble.Count - 1);
                changed = true;
              }
              break;

            case SelectionChoice.SelectAsReplacementForAllExisting:
              {
                // we have to replace the fit function in all existing fit functions by the new function
                for (int i = _doc.FitEnsemble.Count - 1; i >= 0; --i)
                {
                  _doc.FitEnsemble[i].FitFunction = func;
                }
                changed = true;
              }
              break;

            default:
              {
                throw new NotImplementedException();
              }
          }
        }
      }

      if (changed)
      {
        // _doc.FitEnsemble.InitializeParameterSetFromEnsembleParameters(_doc.CurrentParameters);

        _fitEnsembleController.InitializeDocument(_doc.FitEnsemble);
        _parameterController.InitializeDocument(_doc.CurrentParameters);
      }
    }

    public void EhView_SelectFitFunction()
    {
      if (_fitFunctionSelectionController.Apply(false))
      {
        Select((IFitFunction)_fitFunctionSelectionController.ModelObject);
        SelectedTab = TabDetails;
      }
    }

    private void EhController_SelectFitFunction(IFitFunctionInformation fitFunctionInformation)
    {
      Select(fitFunctionInformation.CreateFitFunction());
      SelectedTab = TabDetails;
    }

    public void EhView_NewFitFunction()
    {
      var script = new FitFunctionScript();

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
        _fitFunctionSelectionController.Refresh();
        SelectedTab = TabDetails;
      }
    }

    public void OnAfterFittingStep()
    {
      _parameterController.InitializeDocument(_doc.CurrentParameters);

      if (_activeLayer is not null)
      {
        CreateOrReplaceFunctionPlotItems(_activeLayer);
      }
    }

    private (bool hasItems, double confidenceLevel) HasConfidencePlotItems(XYPlotLayer xylayer)
    {
      var cbdata = TreeNodeExtensions.TakeFromHereToFirstLeaves((IGPlotItem)xylayer.PlotItems)
          .OfType<XYFunctionPlotItem>()
          .Where((fpi) => fpi.Data is XYNonlinearFitFunctionConfidenceBandPlotData).FirstOrDefault();

      if (cbdata is not null)
      {
        return (true, ((XYNonlinearFitFunctionConfidenceBandPlotData)cbdata.Data).ConfidenceLevel);
      }
      else
      {
        return (false, 0);
      }
    }

    private void CreateOrReplaceFunctionPlotItems(XYPlotLayer xylayer)
    {
      _previousFitDocumentIdentifier = CreateOrReplaceFunctionPlotItems(
        xylayer,
        _doc,
        _previousFitDocumentIdentifier,
        _showUnusedDependentVariables,
        _showConfidenceBands,
        _confidenceLevel.AsValueInSIUnits,
        _sigmaSquare,
        _numberOfFitPoints,
        _covarianceMatrix
        );
    }

    /// <summary>
    /// Creates the or replace function plot items after a successful Levenberg-Marquardt-Fit.
    /// </summary>
    /// <param name="xylayer">The XY-Plot Layer for which to replace the plot items.</param>
    /// <param name="doc">The fit document.</param>
    /// <param name="fitAdapter">The Levenberg-Marquardt fit adapter.</param>
    /// <param name="previousFitDocumentIdentifier">The fit document identifier of the previous fit.</param>
    /// <param name="showUnusedDependentVariables">If set to <c>true</c>, shows the unused dependent variables of the fit, too.</param>
    /// <param name="showConfidenceBands">If set to <c>true</c>, shows the confidence bands of the fit (next parameter is the confidence level).</param>
    /// <param name="confidenceLevel">The confidence level (only used if <paramref name="showConfidenceBands"/> is set to true).</param>
    /// <returns>The new identifier of the fit.</returns>
    public static string CreateOrReplaceFunctionPlotItems(
        XYPlotLayer xylayer,
        NonlinearFitDocument doc,
        LevMarAdapter2 fitAdapter,
        string previousFitDocumentIdentifier,
        bool showUnusedDependentVariables,
        bool showConfidenceBands = false,
        double confidenceLevel = 0
      )
    {
      return CreateOrReplaceFunctionPlotItems(
        xylayer,
        doc,
        previousFitDocumentIdentifier,
        showUnusedDependentVariables,
        showConfidenceBands,
        confidenceLevel,
        fitAdapter.Value,
        fitAdapter.NumberOfObservations,
        (double[])fitAdapter.CovarianceMatrix.Clone()
        );

    }

    /// <summary>
    /// Creates or replaces function plot items after a successful Levenberg-Marquardt-Fit.
    /// </summary>
    /// <param name="xylayer">The XY-Plot Layer for which to replace the plot items.</param>
    /// <param name="doc">The fit document.</param>
    /// <param name="previousFitDocumentIdentifier">The fit document identifier of the previous fit.</param>
    /// <param name="showUnusedDependentVariables">If set to <c>true</c>, shows the unused dependent variables of the fit, too.</param>
    /// <param name="showConfidenceBands">If set to <c>true</c>, shows the confidence bands of the fit (next parameter is the confidence level).</param>
    /// <param name="confidenceLevel">The confidence level (only used if <paramref name="showConfidenceBands"/> is set to true).</param>
    /// <param name="sigmaSquare">The Sigma² of fit.</param>
    /// <param name="numberOfFitPoints">Number of fit points.</param>
    /// <param name="covarianceMatrix">The covariance matrix of the fit.</param>
    /// <returns>The new identifier of the fit.</returns>
    public static string CreateOrReplaceFunctionPlotItems(
      XYPlotLayer xylayer,
      NonlinearFitDocument doc,
      string previousFitDocumentIdentifier,
      bool showUnusedDependentVariables,
      bool showConfidenceBands,
      double confidenceLevel,
      double sigmaSquare,
      int numberOfFitPoints,
      double[] covarianceMatrix
      )
    {
      // collect the old plot items and put them into a dictionary whose key is a combination of fit element index and dependentVariableIndex and
      // a third number which is 0 for the fit function, -1 for the lower confidence band, and +1 for the upper confidence band
      var oldItemsDictionary = new Dictionary<(int fitElementIndex, int dependentVariableIndex, int itemType), XYFunctionPlotItem>();

      // dictionary with holds the available y-data columns of this layer and their transformations.
      var dataColumnsAndTheirTransformation = new Dictionary<(DataColumn xColumn, DataColumn yColumn), (IVariantToVariantTransformation xDataTransformation, IVariantToVariantTransformation yDataTransformation)>();

      foreach (var pi in TreeNodeExtensions.TakeFromHereToFirstLeaves((IGPlotItem)xylayer.PlotItems))
      {
        if (pi is XYNonlinearFitFunctionPlotItem plotItem && plotItem.FitDocumentIdentifier == previousFitDocumentIdentifier)
        {
          oldItemsDictionary.Add((plotItem.FitElementIndex, plotItem.DependentVariableIndex, 0), plotItem);
        }

        if (pi is XYFunctionPlotItem fpi &&
            fpi.Data is XYNonlinearFitFunctionConfidenceBandPlotData fcPlotData &&
            fcPlotData.FitDocumentIdentifier == previousFitDocumentIdentifier)
        {
          oldItemsDictionary.Add((fcPlotData.FitElementIndex, fcPlotData.DependentVariableIndex, fcPlotData.IsLowerBand ? -1 : 1), fpi);
        }

        if (pi is XYColumnPlotItem columnPlotItem)
        {
          SplitInDataColumnAndTransformation(columnPlotItem.Data.XColumn, out var xdataColumn, out var xtransfo);
          SplitInDataColumnAndTransformation(columnPlotItem.Data.YColumn, out var ydataColumn, out var ytransfo);
          if (xdataColumn is not null && ydataColumn is not null)
            dataColumnsAndTheirTransformation[(xdataColumn, ydataColumn)] = (xtransfo, ytransfo);
        }
      }

      var newFitDocumentIdentifier = Guid.NewGuid().ToString(); // used to identify all curves with the same fit document

      // now create a plot item for each dependent variable, reuse if possible the style from the old items
      int funcNumber = 0;
      for (int idxFitEnsemble = 0; idxFitEnsemble < doc.FitEnsemble.Count; idxFitEnsemble++)
      {
        FitElement fitEle = doc.FitEnsemble[idxFitEnsemble];

        for (int idxDependentVariable = 0; idxDependentVariable < fitEle.NumberOfDependentVariables; idxDependentVariable++, funcNumber++)
        {
          oldItemsDictionary.TryGetValue((idxFitEnsemble, idxDependentVariable, 0), out var oldPlotItem);

          // if unused variables should not be shown, we remove them from the plot item collection
          if (!showUnusedDependentVariables && fitEle.DependentVariables(idxDependentVariable) is null)
          {
            if (oldPlotItem is not null)
              oldPlotItem.ParentCollection.Remove(oldPlotItem);
            if (oldItemsDictionary.TryGetValue((idxFitEnsemble, idxDependentVariable, -1), out var oldPlotItemLowerConfBand))
              oldPlotItemLowerConfBand.ParentCollection.Remove(oldPlotItemLowerConfBand);
            if (oldItemsDictionary.TryGetValue((idxFitEnsemble, idxDependentVariable, +1), out var oldPlotItemUpperConfBand))
              oldPlotItemUpperConfBand.ParentCollection.Remove(oldPlotItemUpperConfBand);

            continue;
          }

          // Plot style for the new item
          var newPlotStyle = oldPlotItem?.Style.Clone() ?? new G2DPlotStyleCollection(LineScatterPlotStyleKind.Line, xylayer.GetPropertyContext());

          // get the transformation for the plot item
          IVariantToVariantTransformation functionIndepVarPlotItemTransformation = null;
          IVariantToVariantTransformation functionDepVarPlotItemTransformation = null;
          var indepVar = fitEle.IndependentVariables(0);
          var depVar = fitEle.DependentVariables(idxDependentVariable);
          SplitInDataColumnAndTransformation(indepVar, out var indepVarDataColumn, out var indepFitVarTransformation);
          SplitInDataColumnAndTransformation(depVar, out var depVarDataColumn, out var depFitVarTransformation);

          if (indepVarDataColumn is not null && depVarDataColumn is not null && dataColumnsAndTheirTransformation.TryGetValue((indepVarDataColumn, depVarDataColumn), out var orgTransformations))
          {
            functionIndepVarPlotItemTransformation = GetFitFunctionIndependentTransformation(orgTransformations.xDataTransformation, indepFitVarTransformation);
            functionDepVarPlotItemTransformation = GetFitFunctionDependentTransformation(orgTransformations.yDataTransformation, depFitVarTransformation, fitEle.GetDependentVariableTransformation(idxDependentVariable));
          }

          // Plot items for confidence intervals
          if (showConfidenceBands && covarianceMatrix is not null)
          {
            { // generate new item for the lower confidence band
              oldItemsDictionary.TryGetValue((idxFitEnsemble, idxDependentVariable, -1), out var oldPlotItemLowerConfBand);
              var newPlotStyleLowerConfBand = oldPlotItemLowerConfBand?.Style.Clone();
              if (newPlotStyleLowerConfBand is null)
              {
                newPlotStyleLowerConfBand = new G2DPlotStyleCollection(LineScatterPlotStyleKind.Line, xylayer.GetPropertyContext());

                if (newPlotStyleLowerConfBand.Styles.OfType<LinePlotStyle>().FirstOrDefault() is { } fstyle)
                {
                  fstyle.LinePen = fstyle.LinePen.WithDashPattern(Altaxo.Drawing.DashPatterns.DashDot.Instance);
                }
              }

              var newPlotDataLowerConfBand = new XYNonlinearFitFunctionConfidenceBandPlotData(
                  false,
                  true, // lower confidence band
                  confidenceLevel,
                  newFitDocumentIdentifier,
                  doc,
                  idxFitEnsemble,
                  idxDependentVariable,
                  functionDepVarPlotItemTransformation,
                  0,
                  functionIndepVarPlotItemTransformation,
                  numberOfFitPoints,
                  sigmaSquare,
                  covarianceMatrix
                  );
              var newPlotItemLowerConfBand = new XYFunctionPlotItem(newPlotDataLowerConfBand, newPlotStyleLowerConfBand);

              if (oldPlotItemLowerConfBand is not null)
                oldPlotItemLowerConfBand.ParentCollection.Replace(oldPlotItemLowerConfBand, newPlotItemLowerConfBand);
              else
                xylayer.PlotItems.Add(newPlotItemLowerConfBand);
            }

            { // generate new item for the upper confidence band
              oldItemsDictionary.TryGetValue((idxFitEnsemble, idxDependentVariable, +1), out var oldPlotItemUpperConfBand);
              var newPlotStyleUpperConfBand = oldPlotItemUpperConfBand?.Style.Clone();
              if (newPlotStyleUpperConfBand is null)
              {
                newPlotStyleUpperConfBand = new G2DPlotStyleCollection(LineScatterPlotStyleKind.Line, xylayer.GetPropertyContext());
                if (newPlotStyleUpperConfBand.Styles.OfType<LinePlotStyle>().First() is { } fstyle)
                {
                  fstyle.LinePen = fstyle.LinePen.WithDashPattern(Altaxo.Drawing.DashPatterns.DashDot.Instance);
                }
              }

              var newPlotDataUpperConfBand = new XYNonlinearFitFunctionConfidenceBandPlotData(
                  false, // confidence band
                  false, // upper confidence band
                  confidenceLevel,
                  newFitDocumentIdentifier,
                  doc,
                  idxFitEnsemble,
                  idxDependentVariable,
                  functionDepVarPlotItemTransformation,
                  0,
                  functionIndepVarPlotItemTransformation,
                  numberOfFitPoints,
                  sigmaSquare,
                  covarianceMatrix
                  );
              var newPlotItemUpperConf = new XYFunctionPlotItem(newPlotDataUpperConfBand, newPlotStyleUpperConfBand);

              if (oldPlotItemUpperConfBand is not null)
                oldPlotItemUpperConfBand.ParentCollection.Replace(oldPlotItemUpperConfBand, newPlotItemUpperConf);
              else
                xylayer.PlotItems.Add(newPlotItemUpperConf);
            }
          }
          else // do not show confidence bands, thus we delete them here
          {
            oldItemsDictionary.TryGetValue((idxFitEnsemble, idxDependentVariable, -1), out var oldPlotItemLowerConfBand);
            oldItemsDictionary.TryGetValue((idxFitEnsemble, idxDependentVariable, +1), out var oldPlotItemUpperConfBand);

            if (oldPlotItemLowerConfBand is not null)
              oldPlotItemLowerConfBand.ParentCollection.Remove(oldPlotItemLowerConfBand);
            if (oldPlotItemUpperConfBand is not null)
              oldPlotItemUpperConfBand.ParentCollection.Remove(oldPlotItemUpperConfBand);
          }

          // New plot item for fit function
          var newPlotItem = new XYNonlinearFitFunctionPlotItem(newFitDocumentIdentifier, doc, idxFitEnsemble, idxDependentVariable, functionDepVarPlotItemTransformation, 0, functionIndepVarPlotItemTransformation, newPlotStyle);

          if (oldPlotItem is not null)
          {
            oldPlotItem.ParentCollection.Replace(oldPlotItem, newPlotItem);
          }
          else
          {
            xylayer.PlotItems.Add(newPlotItem);
          }
        }
      }

      return newFitDocumentIdentifier;
    }

    private static IVariantToVariantTransformation GetFitFunctionDependentTransformation(IVariantToVariantTransformation transformationOfOriginalDataColumn, IVariantToVariantTransformation transformationOfFitDependentVariable, IVariantToVariantTransformation transformationOfDependentVariableFitFunctionOutput)
    {
      return CompoundTransformation.TryGetCompoundTransformationWithSimplification(new[] { (transformationOfDependentVariableFitFunctionOutput, false), (transformationOfFitDependentVariable, true), (transformationOfOriginalDataColumn, false) });
    }

    private static IVariantToVariantTransformation GetFitFunctionIndependentTransformation(IVariantToVariantTransformation transformationOfOriginalDataColumn, IVariantToVariantTransformation transformationOfFitIndependentVariable)
    {
      if (transformationOfOriginalDataColumn is null && transformationOfFitIndependentVariable is null)
      {
        return null;
      }
      else if (transformationOfOriginalDataColumn is not null && transformationOfFitIndependentVariable is null)
      {
        return transformationOfOriginalDataColumn.BackTransformation;
      }
      else if (transformationOfOriginalDataColumn is null && transformationOfFitIndependentVariable is not null)
      {
        return transformationOfFitIndependentVariable;
      }
      else // both transformations are not null
      {
        return CompoundTransformation.TryGetCompoundTransformationWithSimplification(new[] { transformationOfOriginalDataColumn.BackTransformation, transformationOfFitIndependentVariable });
      }
    }

    private static void SplitInDataColumnAndTransformation(IReadableColumn ycolumn, out DataColumn dataColumn, out IVariantToVariantTransformation transformation)
    {
      dataColumn = null;
      transformation = null;

      if (ycolumn is ITransformedReadableColumn tyc)
      {
        var originalDataColumn = tyc.GetUnderlyingDataColumnOrDefault();
        if (originalDataColumn is not null)
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
      var fitAdapter = new LevMarAdapter2(_doc.FitEnsemble, _doc.CurrentParameters);

      int numberOfData;

      if (calculateUnusedDependentVariablesAlso)
      {
        numberOfData = 0;
        for (int i = 0; i < _doc.FitEnsemble.Count; i++)
          numberOfData += fitAdapter.GetValidNumericRows(i).Count * _doc.FitEnsemble[i].NumberOfDependentVariables;
      }
      else
      {
        numberOfData = fitAdapter.NumberOfObservations;
      }

      // calculate the resulting values
      var resultingValues = Vector<double>.Build.Dense(numberOfData);
      fitAdapter.EvaluateModelValues(resultingValues, calculateUnusedDependentVariablesAlso);

      int nextStartOfDependentValues = 0;
      for (int i = 0; i < _doc.FitEnsemble.Count; i++)
      {
        FitElement fitEle = _doc.FitEnsemble[i];
        int startOfDependentValues = nextStartOfDependentValues;
        Collections.IAscendingIntegerCollection validRows = fitAdapter.GetValidNumericRows(i);
        nextStartOfDependentValues += validRows.Count * fitEle.NumberOfUsedDependentVariables;

        Altaxo.Data.DataTable parentTable = fitEle.DataTable;
        if (parentTable is null)
          continue;

        if (calculateUnusedDependentVariablesAlso)
        {
          // now we have the parent table, we can add columns to it as we need for the independent variables
          int numDependentVars = fitEle.NumberOfDependentVariables;
          for (int k = 0; k < fitEle.NumberOfDependentVariables; k++)
          {
            var col = new DoubleColumn();
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
            var col = new DoubleColumn();
            for (int j = 0; j < validRows.Count; j++)
              col[validRows[j]] = resultingValues[startOfDependentValues + k + j * inUse.Length];

            string name = null;
            int groupNumber = 0;
            if (fitEle.DependentVariables(inUse[k]) is DataColumn)
            {
              var srcCol = (DataColumn)fitEle.DependentVariables(inUse[k]);
              var srcTable = DataColumnCollection.GetParentDataColumnCollectionOf(srcCol);
              if (srcTable is not null)
              {
                name = srcTable.GetColumnName(srcCol);
                groupNumber = srcTable.GetColumnGroup(srcCol);
              }
            }
            if (name is null)
              fitEle.FitFunction.DependentVariableName(inUse[k]);

            parentTable.DataColumns.Add(col, name + ".Sim", ColumnKind.V, groupNumber);
          }
        }
      }
    }

    public void OnSimulationWithInterval(bool calculateUnusedDependentVariablesAlso, ISpacedInterval interval)
    {
      // we investigate for every fit element the corresponding table, and add columns to that table
      var fitAdapter = new LevMarAdapter2(_doc.FitEnsemble, _doc.CurrentParameters);

      int intervalCount = (int)interval.Count;
      for (int i = 0; i < _doc.FitEnsemble.Count; i++)
      {
        FitElement fitEle = _doc.FitEnsemble[i];

        Altaxo.Data.DataTable parentTable = fitEle.DataTable;
        if (parentTable is null)
          continue;

        double[] X = new double[fitEle.NumberOfIndependentVariables];
        double[] Y = new double[fitEle.NumberOfDependentVariables];
        double[] P = new double[fitEle.NumberOfParameters];
        var xCols = new DoubleColumn[fitEle.NumberOfIndependentVariables];
        var yCols = new DoubleColumn[fitEle.NumberOfDependentVariables];

        for (int k = 0; k < xCols.Length; k++)
          xCols[k] = new DoubleColumn();
        for (int k = 0; k < yCols.Length; k++)
          yCols[k] = new DoubleColumn();

        fitAdapter.CopyParametersForFitElement(i, P);
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
      if (table is null)
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
      if (col is null)
        return;

      int len = Math.Max(col.Count, _doc.CurrentParameters.Count);
      for (int i = 0; i < len; i++)
        _doc.CurrentParameters[i].Parameter = col[i];

      _parameterController.InitializeDocument(_doc.CurrentParameters);
    }

    public void EhView_CopyParameterV()
    {
      if (true == _parameterController.Apply(false))
      {
        var dao = Current.Gui.GetNewClipboardDataObject();
        var col = new Altaxo.Data.DoubleColumn();
        for (int i = 0; i < _doc.CurrentParameters.Count; i++)
          col[i] = _doc.CurrentParameters[i].Parameter;

        var tb = new Altaxo.Data.DataTable();
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
      if (true == _parameterController.Apply(false))
      {
        var dao = Current.Gui.GetNewClipboardDataObject();
        var stb = new System.Text.StringBuilder();
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
      if (true == _parameterController.Apply(false))
      {
        var dao = Current.Gui.GetNewClipboardDataObject();
        var txt = new Altaxo.Data.TextColumn();
        var col = new Altaxo.Data.DoubleColumn();

        for (int i = 0; i < _doc.CurrentParameters.Count; i++)
        {
          txt[i] = _doc.CurrentParameters[i].Name;
          col[i] = _doc.CurrentParameters[i].Parameter;
        }

        var tb = new Altaxo.Data.DataTable();
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
      if (true == _parameterController.Apply(false))
      {
        var dao = Current.Gui.GetNewClipboardDataObject();
        var txt = new Altaxo.Data.TextColumn();
        var col = new Altaxo.Data.DoubleColumn();
        var var = new Altaxo.Data.DoubleColumn();

        for (int i = 0; i < _doc.CurrentParameters.Count; i++)
        {
          txt[i] = _doc.CurrentParameters[i].Name;
          col[i] = _doc.CurrentParameters[i].Parameter;
          var[i] = _doc.CurrentParameters[i].Variance;
        }

        var tb = new Altaxo.Data.DataTable();
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

    public void EhView_CopyParameterNCM()
    {
      if (_covarianceMatrix is null)
      {
        Current.Gui.ErrorMessageBox("First, please do make a fit to get the covariances, then try again!");
        return;
      }

      if (true == _parameterController.Apply(false))
      {
        var dao = Current.Gui.GetNewClipboardDataObject();
        var txt = new Altaxo.Data.TextColumn();
        var col = new Altaxo.Data.DoubleColumn();
        var var = new DoubleColumn[_doc.CurrentParameters.Count];
        for (int i = 0; i < _doc.CurrentParameters.Count; i++)
          var[i] = new DoubleColumn();

        for (int i = 0; i < _doc.CurrentParameters.Count; i++)
        {
          txt[i] = _doc.CurrentParameters[i].Name;
          col[i] = _doc.CurrentParameters[i].Parameter;
          for (int j = 0; j < _doc.CurrentParameters.Count; j++)
            var[j][i] = _covarianceMatrix[i * _doc.CurrentParameters.Count + j];
        }

        var tb = new Altaxo.Data.DataTable();
        tb.DataColumns.Add(txt, "Name", Altaxo.Data.ColumnKind.V, 0);
        tb.DataColumns.Add(col, "Value", Altaxo.Data.ColumnKind.V, 0);
        for (int i = 0; i < _doc.CurrentParameters.Count; i++)
          tb.DataColumns.Add(var[i], "CV_" + _doc.CurrentParameters[i].Name, Altaxo.Data.ColumnKind.V, 0);

        // amdend sigma_square and N
        int nRow = _doc.CurrentParameters.Count;
        txt[nRow] = "SigmaSquare";
        col[nRow] = _sigmaSquare;
        ++nRow;
        txt[nRow] = "N";
        col[nRow] = _numberOfFitPoints;

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

    public void EhView_CopyParameterNSVCVInOneRow()
    {
      if (_covarianceMatrix is null)
      {
        Current.Gui.ErrorMessageBox("First, please do make a fit to get the covariances, then try again!");
        return;
      }

      if (true == _parameterController.Apply(false))
      {
        var dao = Current.Gui.GetNewClipboardDataObject();
        var col = new Altaxo.Data.DoubleColumn();
        int nRow = 0;

        col[nRow++] = _numberOfFitPoints;
        col[nRow++] = _sigmaSquare;

        for (int i = 0; i < _doc.CurrentParameters.Count; i++)
        {
          col[nRow++] = _doc.CurrentParameters[i].Parameter;
          for (int j = 0; j < _doc.CurrentParameters.Count; j++)
            col[nRow++] = _covarianceMatrix[i * _doc.CurrentParameters.Count + j];
        }

        var tb = new Altaxo.Data.DataTable();
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


    public override bool Apply(bool disposeController)
    {
      return ApplyEnd(true, disposeController);
    }


  }
}
