#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2024 Dr. Dirk Lellinger
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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Altaxo.Calc.FitFunctions.Peaks;
using Altaxo.Collections;
using Altaxo.Gui.Common;
using Altaxo.Science.Spectroscopy.PeakFitting;
using Altaxo.Science.Spectroscopy.PeakFitting.MultipleSpectra;
using Altaxo.Units;

namespace Altaxo.Gui.Science.Spectroscopy.PeakFitting.MultipleSpectra
{
  /// <summary>
  /// View interface for peak fitting of multiple spectra by incremental peak addition.
  /// </summary>
  public interface IPeakFittingOfMultipleSpectraByIncrementalPeakAdditionView : IDataContextAwareView { }

  /// <summary>
  /// Controller for <see cref="PeakFittingOfMultipleSpectraByIncrementalPeakAddition"/>.
  /// </summary>
  [UserControllerForObject(typeof(PeakFittingOfMultipleSpectraByIncrementalPeakAddition))]
  [ExpectedTypeOfView(typeof(IPeakFittingOfMultipleSpectraByIncrementalPeakAdditionView))]
  public class PeakFittingOfMultipleSpectraByIncrementalPeakAdditionController : MVCANControllerEditImmutableDocBase<PeakFittingOfMultipleSpectraByIncrementalPeakAddition, IPeakFittingOfMultipleSpectraByIncrementalPeakAdditionView>
  {
    /// <inheritdoc/>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private ItemsController<Type> _fitFunctions;

    /// <summary>
    /// Gets or sets the selectable peak fit functions.
    /// </summary>
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
    /// Gets the quantity environment for the minimal relative height.
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

    private bool _isMinimalFWHMValueInXUnits;

    /// <summary>
    /// Gets or sets a value indicating whether the minimal FWHM value is specified in x units.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the minimal FWHM value.
    /// </summary>
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

    /// <summary>
    /// Gets the quantity environment for the fit-width scaling factor.
    /// </summary>
    public QuantityWithUnitGuiEnvironment FitWidthScalingFactorEnvironment => RelationEnvironment.Instance;


    private DimensionfulQuantity _fitWidthScalingFactor;

    /// <summary>
    /// Gets or sets the fit-width scaling factor.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the pruning factor for the sum of chi-square values.
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

    private ItemsController<PeakAdditionOrder> _peakAdditionOrder;

    /// <summary>
    /// Gets or sets the peak-addition order.
    /// </summary>
    public ItemsController<PeakAdditionOrder> PeakAdditionOrder
    {
      get => _peakAdditionOrder;
      set
      {
        if (!(_peakAdditionOrder == value))
        {
          _peakAdditionOrder?.Dispose();
          _peakAdditionOrder = value;
          OnPropertyChanged(nameof(PeakAdditionOrder));
        }
      }
    }


    /// <summary>
    /// Editable fixed-position and FWHM data.
    /// </summary>
    public class EditablePositionFWHM : IEditableObject, INotifyPropertyChanged
    {
      /// <summary>
      /// Occurs when a property value changes.
      /// </summary>
      public event PropertyChangedEventHandler? PropertyChanged;

      /// <summary>
      /// Raises the <see cref="PropertyChanged"/> event.
      /// </summary>
      /// <param name="propertyName">The name of the changed property.</param>
      protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

      /// <summary>
      /// Initializes a new instance of the <see cref="EditablePositionFWHM"/> class.
      /// </summary>
      public EditablePositionFWHM()
      {

      }

      /// <summary>
      /// Initializes a new instance of the <see cref="EditablePositionFWHM"/> class.
      /// </summary>
      public EditablePositionFWHM(double position, double initialFWHMValue, double? minimalFWHMValue, double? maximalFWHMValue)
      {
        Position = position;
        InitialFWHMValue = initialFWHMValue;
        MinimalFWHMValue = minimalFWHMValue;
        MaximalFWHMValue = maximalFWHMValue;
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="EditablePositionFWHM"/> class.
      /// </summary>
      public EditablePositionFWHM((double position, double initialFWHMValue, double? minimalFWHMValue, double? maximalFWHMValue) v)
      {
        Position = v.position;
        InitialFWHMValue = v.initialFWHMValue;
        MinimalFWHMValue = v.minimalFWHMValue;
        MaximalFWHMValue = v.maximalFWHMValue;
      }

      /// <inheritdoc/>
      public void BeginEdit()
      {
      }

      /// <inheritdoc/>
      public void CancelEdit()
      {
      }

      /// <inheritdoc/>
      public void EndEdit()
      {
      }

      private double _position;

      /// <summary>
      /// Gets or sets the peak position.
      /// </summary>
      public double Position
      {
        get => _position;
        set
        {
          if (!(_position == value))
          {
            _position = value;
            OnPropertyChanged(nameof(Position));
          }
        }
      }

      private double _initialFWHMValue;

      /// <summary>
      /// Gets or sets the initial FWHM value.
      /// </summary>
      public double InitialFWHMValue
      {
        get => _initialFWHMValue;
        set
        {
          if (!(_initialFWHMValue == value))
          {
            _initialFWHMValue = value;
            OnPropertyChanged(nameof(InitialFWHMValue));
          }
        }
      }

      private double? _minimalFWHMValue;

      /// <summary>
      /// Gets or sets the minimal allowed FWHM value.
      /// </summary>
      public double? MinimalFWHMValue
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
      private double? _maximalFWHMValue;

      /// <summary>
      /// Gets or sets the maximal allowed FWHM value.
      /// </summary>
      public double? MaximalFWHMValue
      {
        get => _maximalFWHMValue;
        set
        {
          if (!(_maximalFWHMValue == value))
          {
            _maximalFWHMValue = value;
            OnPropertyChanged(nameof(MaximalFWHMValue));
          }
        }
      }
    }

    /// <summary>
    /// Gets the editable collection of fixed peak positions and FWHM values.
    /// </summary>
    public ObservableCollection<EditablePositionFWHM> FixedPositions { get; protected set; } = new();

    #endregion

    /// <inheritdoc/>
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

        PeakAdditionOrder = new ItemsController<PeakAdditionOrder>(new SelectableListNodeList(_doc.PeakAdditionOrder));
        PeakAdditionOrder.SelectedValue = _doc.PeakAdditionOrder;

        FixedPositions.Clear();
        FixedPositions.AddRange(_doc.FixedPeakPositions.Select(x => new EditablePositionFWHM(x)));
      }
    }

    /// <inheritdoc/>
    public override bool Apply(bool disposeController)
    {
      // test the fixed positions
      for (int i = 0; i < FixedPositions.Count; ++i)
      {
        var fp = FixedPositions[i];
        if (!(fp.InitialFWHMValue > 0))
        {
          Current.Gui.ErrorMessageBox($"For the fixed position #{i}, the initial FWHM value must be >=0, but currently is {fp.InitialFWHMValue}");
          return ApplyEnd(false, disposeController);
        }
        if (fp.MinimalFWHMValue.HasValue && !(fp.MinimalFWHMValue.Value <= fp.InitialFWHMValue))
        {
          Current.Gui.ErrorMessageBox($"For the fixed position #{i}, the minimal FWHM value must be <= the initial FWHM value, but currently is {fp.MinimalFWHMValue}");
          return ApplyEnd(false, disposeController);
        }
        if (fp.MaximalFWHMValue.HasValue && !(fp.MaximalFWHMValue.Value >= fp.InitialFWHMValue))
        {
          Current.Gui.ErrorMessageBox($"For the fixed position #{i}, the maximal FWHM value must be >= the initial FWHM value, but currently is {fp.MaximalFWHMValue}");
          return ApplyEnd(false, disposeController);
        }
      }

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
          PeakAdditionOrder = PeakAdditionOrder.SelectedValue,
          FixedPeakPositions = FixedPositions.Select(x => (x.Position, x.InitialFWHMValue, x.MinimalFWHMValue, x.MaximalFWHMValue)).ToArray(),
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
