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

using System;
using System.Collections.Generic;
using System.Linq;
using Altaxo.Calc.FitFunctions.Peaks;
using Altaxo.Collections;
using Altaxo.Gui.Common;
using Altaxo.Gui.Common.PropertyGrid;
using Altaxo.Units;

namespace Altaxo.Gui.Science.Spectroscopy.PeakFitting
{
  public abstract class PeakFittingBaseController<TModel, TView> : MVCANControllerEditImmutableDocBase<TModel, TView> where TView : class
  {

    #region Bindings

    private ItemsController<Type> _fitFunctions;

    public ItemsController<Type> FitFunctions
    {
      get => _fitFunctions;
      set
      {
        if (!(_fitFunctions == value))
        {
          _fitFunctions = value;
          OnPropertyChanged(nameof(FitFunctions));
        }
      }
    }

    protected void EhFitFunctionChanged(Type type)
    {
      if (type is not null && _currentFitFunction?.GetType() != type)
      {
        if (!_fitFunctionInstances.TryGetValue(type, out var fitFunction))
        {
          fitFunction = (IFitFunctionPeak)Activator.CreateInstance(type);
          _fitFunctionInstances.Add(fitFunction.GetType(), fitFunction);
        }

        _currentFitFunction = fitFunction;
        CmdConfigureFitFunction.OnCanExecuteChanged();
      }
    }

    public RelayCommand CmdConfigureFitFunction => field ??= new RelayCommand(EhConfigureFitFunction, EhCanConfigureFitFunction);

    public QuantityWithUnitGuiEnvironment FitWidthScalingFactorEnvironment => RelationEnvironment.Instance;

    private DimensionfulQuantity _fitWidthScalingFactor;

    public DimensionfulQuantity FitWidthScalingFactor
    {
      get => _fitWidthScalingFactor;
      set
      {
        if (!(_fitWidthScalingFactor == value))
        {
          _fitWidthScalingFactor = value;
          OnPropertyChanged(nameof(FitWidthScalingFactor));
        }
      }
    }

    private bool _isMinimalFWHMValueInXUnits;

    public bool IsMinimalFWHMValueInXUnits
    {
      get => _isMinimalFWHMValueInXUnits;
      set
      {
        if (!(_isMinimalFWHMValueInXUnits == value))
        {
          _isMinimalFWHMValueInXUnits = value;
          OnPropertyChanged(nameof(IsMinimalFWHMValueInXUnits));
        }
      }
    }

    private double _minimalFWHMValue;

    public double MinimalFWHMValue
    {
      get => _minimalFWHMValue;
      set
      {
        if (!(_minimalFWHMValue == value))
        {
          _minimalFWHMValue = value;
          OnPropertyChanged(nameof(MinimalFWHMValue));
        }
      }
    }

    #endregion


    protected void InitializeFitFunctions(IFitFunctionPeak fitFunction)
    {
      var ftypeList = new SelectableListNodeList(
          Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(IFitFunctionPeak))
            .Select(t => new SelectableListNode(t.Name, t, false))
            );
      FitFunctions = new ItemsController<Type>(ftypeList, EhFitFunctionChanged);
      FitFunctions.SelectedValue = fitFunction.GetType();
      _currentFitFunction = fitFunction;
      _fitFunctionInstances[fitFunction.GetType()] = fitFunction;
    }


    /// <summary>The currently selected fit function.</summary>
    protected IFitFunctionPeak _currentFitFunction = new Altaxo.Calc.FitFunctions.Peaks.GaussAmplitude();

    /// <summary>Keeps the configured fit functions. Key is the type, value is the instance.</summary>
    protected Dictionary<Type, IFitFunctionPeak> _fitFunctionInstances = [];

    protected void EhConfigureFitFunction()
    {
      if (!EhCanConfigureFitFunction())
        return;

      var controller = (IMVCAController)Current.Gui.GetController([_currentFitFunction], typeof(IMVCAController)) ??
                       new PropertyGridController(_currentFitFunction);

      if (controller is not null)
      {
        if (Current.Gui.ShowDialog(controller, "Configure fit function", showApplyButton: false))
        {
          _currentFitFunction = (IFitFunctionPeak)controller.ModelObject;
          _fitFunctionInstances[_currentFitFunction.GetType()] = _currentFitFunction;
        }
      }
    }

    protected bool EhCanConfigureFitFunction()
    {
      if (_currentFitFunction is null)
        return false;


      var hash = PropertyGridController.GetNameAndTypeOfWritableProperties(_currentFitFunction)
                 .Select(e => e.Name)
                 .ToHashSet();

      hash.Remove(nameof(IFitFunctionPeak.NumberOfTerms));
      hash.Remove(nameof(IFitFunctionPeak.OrderOfBaselinePolynomial));
      return hash.Count > 0;
    }
  }
}
