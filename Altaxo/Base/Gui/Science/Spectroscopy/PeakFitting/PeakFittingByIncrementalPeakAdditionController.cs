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
using Altaxo.Science.Spectroscopy.PeakFitting;
using Altaxo.Units;

namespace Altaxo.Gui.Science.Spectroscopy.PeakFitting
{
  /// <summary>
  /// View interface for peak fitting by incremental peak addition.
  /// </summary>
  public interface IPeakFittingByIncrementalPeakAdditionView : IDataContextAwareView { }

  /// <summary>
  /// Controller for <see cref="PeakFittingByIncrementalPeakAddition"/>.
  /// </summary>
  [UserControllerForObject(typeof(PeakFittingByIncrementalPeakAddition))]
  [ExpectedTypeOfView(typeof(IPeakFittingByIncrementalPeakAdditionView))]
  public class PeakFittingByIncrementalPeakAdditionController : PeakFittingBaseController<PeakFittingByIncrementalPeakAddition, IPeakFittingByIncrementalPeakAdditionView>
  {
    /// <inheritdoc/>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private int _orderOfBaselinePolynomial;

    /// <summary>
    /// Gets or sets the order of the baseline polynomial.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the maximum number of peaks.
    /// </summary>
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


    /// <summary>
    /// Gets the unit environment for relative-height values.
    /// </summary>
    public QuantityWithUnitGuiEnvironment MinimalRelativeHeightEnvironment => RelationEnvironment.Instance;

    private DimensionfulQuantity _minimalRelativeHeight;

    /// <summary>
    /// Gets or sets the minimal relative height.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the minimal signal-to-noise ratio.
    /// </summary>
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

    /// <summary>
    /// Gets or sets a value indicating whether separate peaks are used for error evaluation.
    /// </summary>
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

    private DimensionfulQuantity _prunePeaksSumChiSquareFactor;

    /// <summary>
    /// Gets or sets the prune-peaks chi-square factor.
    /// </summary>
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

    /// <inheritdoc/>
    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        InitializeFitFunctions(_doc.FitFunction);

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

    /// <inheritdoc/>
    public override bool Apply(bool disposeController)
    {
      try
      {
        _doc = _doc with
        {
          FitFunction = _currentFitFunction,
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
