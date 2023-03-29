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
using Altaxo.Science.Spectroscopy.PeakFitting;
using Altaxo.Units;

namespace Altaxo.Gui.Science.Spectroscopy.PeakFitting
{
  public interface IPeakFittingByIncrementalPeakAdditionView : IDataContextAwareView { }

  [UserControllerForObject(typeof(PeakFittingByIncrementalPeakAddition))]
  [ExpectedTypeOfView(typeof(IPeakFittingByIncrementalPeakAdditionView))]
  public class PeakFittingByIncrementalPeakAdditionController : MVCANControllerEditImmutableDocBase<PeakFittingByIncrementalPeakAddition, IPeakFittingByIncrementalPeakAdditionView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

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

    private int _orderOfBaselinePolynomial;

    public int OrderOfBaselinePolynomial
    {
      get => _orderOfBaselinePolynomial;
      set
      {
        if (!(_orderOfBaselinePolynomial == value))
        {
          _orderOfBaselinePolynomial = value;
          OnPropertyChanged(nameof(OrderOfBaselinePolynomial));
        }
      }
    }

    private int _maximumNumberOfPeaks;

    public int MaximumNumberOfPeaks
    {
      get => _maximumNumberOfPeaks;
      set
      {
        if (!(_maximumNumberOfPeaks == value))
        {
          _maximumNumberOfPeaks = value;
          OnPropertyChanged(nameof(MaximumNumberOfPeaks));
        }
      }
    }


    public QuantityWithUnitGuiEnvironment MinimalRelativeHeightEnvironment => RelationEnvironment.Instance;

    private DimensionfulQuantity _minimalRelativeHeight;

    public DimensionfulQuantity MinimalRelativeHeight
    {
      get => _minimalRelativeHeight;
      set
      {
        if (!(_minimalRelativeHeight == value))
        {
          _minimalRelativeHeight = value;
          OnPropertyChanged(nameof(MinimalRelativeHeight));
        }
      }
    }

    private DimensionfulQuantity _minimalSignalToNoiseRatio;

    public DimensionfulQuantity MinimalSignalToNoiseRatio
    {
      get => _minimalSignalToNoiseRatio;
      set
      {
        if (!(_minimalSignalToNoiseRatio == value))
        {
          _minimalSignalToNoiseRatio = value;
          OnPropertyChanged(nameof(MinimalSignalToNoiseRatio));
        }
      }
    }


    private bool _useSeparatePeaksForErrorEvaluation;

    public bool UseSeparatePeaksForErrorEvaluation
    {
      get => _useSeparatePeaksForErrorEvaluation;
      set
      {
        if (!(_useSeparatePeaksForErrorEvaluation == value))
        {
          _useSeparatePeaksForErrorEvaluation = value;
          OnPropertyChanged(nameof(UseSeparatePeaksForErrorEvaluation));
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

    private DimensionfulQuantity _prunePeaksSumChiSquareFactor;

    public DimensionfulQuantity PrunePeaksSumChiSquareFactor
    {
      get => _prunePeaksSumChiSquareFactor;
      set
      {
        if (!(_prunePeaksSumChiSquareFactor == value))
        {
          _prunePeaksSumChiSquareFactor = value;
          OnPropertyChanged(nameof(PrunePeaksSumChiSquareFactor));
        }
      }
    }


    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        var ftypeList = new SelectableListNodeList(
          Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(IFitFunctionPeak))
            .Select(t => new SelectableListNode(t.Name, t, false))
            );
        FitFunctions = new ItemsController<Type>(ftypeList);
        FitFunctions.SelectedValue = _doc.FitFunction.GetType();

        OrderOfBaselinePolynomial = _doc.OrderOfBaselinePolynomial;

        MaximumNumberOfPeaks = _doc.MaximumNumberOfPeaks;

        MinimalRelativeHeight = new DimensionfulQuantity(_doc.MinimalRelativeHeight, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(MinimalRelativeHeightEnvironment.DefaultUnit);

        MinimalSignalToNoiseRatio = new DimensionfulQuantity(_doc.MinimalSignalToNoiseRatio, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(MinimalRelativeHeightEnvironment.DefaultUnit);

        IsMinimalFWHMValueInXUnits = _doc.IsMinimalFWHMValueInXUnits;
        MinimalFWHMValue = _doc.MinimalFWHMValue;

        UseSeparatePeaksForErrorEvaluation = _doc.FitWidthScalingFactor.HasValue;

        FitWidthScalingFactor = new DimensionfulQuantity(_doc.FitWidthScalingFactor ?? 2, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(MinimalRelativeHeightEnvironment.DefaultUnit);

        PrunePeaksSumChiSquareFactor = new DimensionfulQuantity(_doc.PrunePeaksSumChiSquareFactor, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(MinimalRelativeHeightEnvironment.DefaultUnit);
      }
    }

    public override bool Apply(bool disposeController)
    {
      try
      {
        _doc = _doc with
        {
          FitFunction = (IFitFunctionPeak)Activator.CreateInstance(FitFunctions.SelectedValue),
          OrderOfBaselinePolynomial = OrderOfBaselinePolynomial,
          MaximumNumberOfPeaks = MaximumNumberOfPeaks,
          MinimalRelativeHeight = MinimalRelativeHeight.AsValueInSIUnits,
          MinimalSignalToNoiseRatio = MinimalSignalToNoiseRatio.AsValueInSIUnits,
          IsMinimalFWHMValueInXUnits = IsMinimalFWHMValueInXUnits,
          MinimalFWHMValue = MinimalFWHMValue,
          FitWidthScalingFactor = UseSeparatePeaksForErrorEvaluation ? (FitWidthScalingFactor.AsValueInSIUnits == 0 ? null : FitWidthScalingFactor.AsValueInSIUnits) : null,
          PrunePeaksSumChiSquareFactor = PrunePeaksSumChiSquareFactor.AsValueInSIUnits,
        };
      }
      catch (Exception ex)
      {
        Current.Gui.ErrorMessageBox(ex.Message);
        return ApplyEnd(false, disposeController);
      }

      return ApplyEnd(true, disposeController);
    }


  }
}
