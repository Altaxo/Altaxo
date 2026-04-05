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
using System.Collections.ObjectModel;
using Altaxo.Analysis.Statistics.Histograms;
using Altaxo.Collections;
using Altaxo.Gui.Common;

namespace Altaxo.Gui.Analysis.Statistics
{
  /// <summary>
  /// Defines the view contract for histogram creation.
  /// </summary>
  public interface IHistogramCreationView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Controller for <see cref="HistogramCreationInformation"/>.
  /// </summary>
  [UserControllerForObject(typeof(HistogramCreationInformation))]
  [ExpectedTypeOfView(typeof(IHistogramCreationView))]
  public class HistogramCreationController : MVCANControllerEditOriginalDocBase<HistogramCreationInformation, IHistogramCreationView>
  {
    /// <inheritdoc/>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      if (_binningController is not null)
        yield return new ControllerAndSetNullMethod(_binningController, () => BinningController = null);
    }

    #region Bindings

    private ObservableCollection<string> _errors;

    /// <summary>
    /// Gets or sets the current error messages.
    /// </summary>
    public ObservableCollection<string> Errors
    {
      get => _errors;
      set
      {
        if (!(_errors == value))
        {
          _errors = value;
          OnPropertyChanged(nameof(Errors));
        }
      }
    }

    private ObservableCollection<string> _warnings;

    /// <summary>
    /// Gets or sets the current warning messages.
    /// </summary>
    public ObservableCollection<string> Warnings
    {
      get => _warnings;
      set
      {
        if (!(_warnings == value))
        {
          _warnings = value;
          OnPropertyChanged(nameof(Warnings));
        }
      }
    }


    private double _numberOfValuesOriginal;

    /// <summary>
    /// Gets or sets the original number of values.
    /// </summary>
    public double NumberOfValuesOriginal
    {
      get => _numberOfValuesOriginal;
      set
      {
        if (!(_numberOfValuesOriginal == value))
        {
          _numberOfValuesOriginal = value;
          OnPropertyChanged(nameof(NumberOfValuesOriginal));
        }
      }
    }
    private double _numberOfValuesFiltered;

    /// <summary>
    /// Gets or sets the filtered number of values.
    /// </summary>
    public double NumberOfValuesFiltered
    {
      get => _numberOfValuesFiltered;
      set
      {
        if (!(_numberOfValuesFiltered == value))
        {
          _numberOfValuesFiltered = value;
          OnPropertyChanged(nameof(NumberOfValuesFiltered));
        }
      }
    }
    private double _numberOfNaNValues;

    /// <summary>
    /// Gets or sets the number of NaN values.
    /// </summary>
    public double NumberOfNaNValues
    {
      get => _numberOfNaNValues;
      set
      {
        if (!(_numberOfNaNValues == value))
        {
          _numberOfNaNValues = value;
          OnPropertyChanged(nameof(NumberOfNaNValues));
        }
      }
    }
    private double _numberOfInfiniteValues;

    /// <summary>
    /// Gets or sets the number of infinite values.
    /// </summary>
    public double NumberOfInfiniteValues
    {
      get => _numberOfInfiniteValues;
      set
      {
        if (!(_numberOfInfiniteValues == value))
        {
          _numberOfInfiniteValues = value;
          OnPropertyChanged(nameof(NumberOfInfiniteValues));
        }
      }
    }
    private double _minimumValue;

    /// <summary>
    /// Gets or sets the minimum value.
    /// </summary>
    public double MinimumValue
    {
      get => _minimumValue;
      set
      {
        if (!(_minimumValue == value))
        {
          _minimumValue = value;
          OnPropertyChanged(nameof(MinimumValue));
        }
      }
    }
    private double _maximumValue;

    /// <summary>
    /// Gets or sets the maximum value.
    /// </summary>
    public double MaximumValue
    {
      get => _maximumValue;
      set
      {
        if (!(_maximumValue == value))
        {
          _maximumValue = value;
          OnPropertyChanged(nameof(MaximumValue));
        }
      }
    }
    private bool _ignoreNaNValues;
    /// <summary>
    /// Gets or sets a value indicating whether NaN values are ignored.
    /// </summary>
    public bool IgnoreNaNValues
    {
      get => _ignoreNaNValues;
      set
      {
        if (!(_ignoreNaNValues == value))
        {
          _ignoreNaNValues = value;
          OnPropertyChanged(nameof(IgnoreNaNValues));
        }
      }
    }
    private bool _ignoreInfiniteValues;

    /// <summary>
    /// Gets or sets a value indicating whether infinite values are ignored.
    /// </summary>
    public bool IgnoreInfiniteValues
    {
      get => _ignoreInfiniteValues;
      set
      {
        if (!(_ignoreInfiniteValues == value))
        {
          _ignoreInfiniteValues = value;
          OnPropertyChanged(nameof(IgnoreInfiniteValues));
        }
      }
    }
    private bool _ignoreValuesBelowLowerBoundary;

    /// <summary>
    /// Gets or sets a value indicating whether values below the lower boundary are ignored.
    /// </summary>
    public bool IgnoreValuesBelowLowerBoundary
    {
      get => _ignoreValuesBelowLowerBoundary;
      set
      {
        if (!(_ignoreValuesBelowLowerBoundary == value))
        {
          _ignoreValuesBelowLowerBoundary = value;
          OnPropertyChanged(nameof(IgnoreValuesBelowLowerBoundary));
        }
      }
    }
    private bool _isLowerBoundaryInclusive;

    /// <summary>
    /// Gets or sets a value indicating whether the lower boundary is inclusive.
    /// </summary>
    public bool IsLowerBoundaryInclusive
    {
      get => _isLowerBoundaryInclusive;
      set
      {
        if (!(_isLowerBoundaryInclusive == value))
        {
          _isLowerBoundaryInclusive = value;
          OnPropertyChanged(nameof(IsLowerBoundaryInclusive));
        }
      }
    }
    private double _lowerBoundary;

    /// <summary>
    /// Gets or sets the lower boundary.
    /// </summary>
    public double LowerBoundary
    {
      get => _lowerBoundary;
      set
      {
        if (!(_lowerBoundary == value))
        {
          _lowerBoundary = value;
          OnPropertyChanged(nameof(LowerBoundary));
        }
      }
    }
    private bool _ignoreValuesAboveUpperBoundary;

    /// <summary>
    /// Gets or sets a value indicating whether values above the upper boundary are ignored.
    /// </summary>
    public bool IgnoreValuesAboveUpperBoundary
    {
      get => _ignoreValuesAboveUpperBoundary;
      set
      {
        if (!(_ignoreValuesAboveUpperBoundary == value))
        {
          _ignoreValuesAboveUpperBoundary = value;
          OnPropertyChanged(nameof(IgnoreValuesAboveUpperBoundary));
        }
      }
    }
    private bool _isUpperBoundaryInclusive;

    /// <summary>
    /// Gets or sets a value indicating whether the upper boundary is inclusive.
    /// </summary>
    public bool IsUpperBoundaryInclusive
    {
      get => _isUpperBoundaryInclusive;
      set
      {
        if (!(_isUpperBoundaryInclusive == value))
        {
          _isUpperBoundaryInclusive = value;
          OnPropertyChanged(nameof(IsUpperBoundaryInclusive));
        }
      }
    }
    private double _upperBoundary;

    /// <summary>
    /// Gets or sets the upper boundary.
    /// </summary>
    public double UpperBoundary
    {
      get => _upperBoundary;
      set
      {
        if (!(_upperBoundary == value))
        {
          _upperBoundary = value;
          OnPropertyChanged(nameof(UpperBoundary));
        }
      }
    }
    private bool _useAutomaticBinning;

    /// <summary>
    /// Gets or sets a value indicating whether automatic binning is used.
    /// </summary>
    public bool UseAutomaticBinning
    {
      get => _useAutomaticBinning;
      set
      {
        if (!(_useAutomaticBinning == value))
        {
          _useAutomaticBinning = value;
          OnPropertyChanged(nameof(UseAutomaticBinning));
          EhAutomaticBinningTypeChanged();
        }
      }
    }
    private ItemsController<Type> _binningTypes;

    /// <summary>
    /// Gets or sets the available binning types.
    /// </summary>
    public ItemsController<Type> BinningTypes
    {
      get => _binningTypes;
      set
      {
        if (!(_binningTypes == value))
        {
          _binningTypes = value;
          OnPropertyChanged(nameof(BinningTypes));
        }
      }
    }


    private IMVCANController _binningController;

    /// <summary>
    /// Gets or sets the controller for the selected binning settings.
    /// </summary>
    public IMVCANController BinningController
    {
      get => _binningController;
      set
      {
        if (!(_binningController == value))
        {
          _binningController?.Dispose();
          _binningController = value;
          OnPropertyChanged(nameof(BinningController));
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

        var binningTypes = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(IBinning));
        var binningNodes = new SelectableListNodeList();
        foreach (var type in binningTypes)
          binningNodes.Add(new SelectableListNode(type.ToString(), type, type == _doc.CreationOptions.Binning.GetType()));
        BinningTypes = new ItemsController<Type>(binningNodes, EhBinningTypeChanged);
        BinningController = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _doc.CreationOptions.Binning }, typeof(IMVCANController), UseDocument.Directly);
        Errors = new ObservableCollection<string>(_doc.Errors);
        Warnings = new ObservableCollection<string>(_doc.Warnings);

        NumberOfValuesOriginal = _doc.NumberOfValuesOriginal;
        NumberOfValuesFiltered = _doc.NumberOfValuesFiltered;
        NumberOfNaNValues = _doc.NumberOfNaNValues;
        NumberOfInfiniteValues = _doc.NumberOfInfiniteValues;
        MinimumValue = _doc.MinimumValue;
        MaximumValue = _doc.MaximumValue;

        IgnoreNaNValues = _doc.CreationOptions.IgnoreNaN;
        IgnoreInfiniteValues = _doc.CreationOptions.IgnoreInfinity;

        IgnoreValuesBelowLowerBoundary = _doc.CreationOptions.LowerBoundaryToIgnore.HasValue;
        IsLowerBoundaryInclusive = _doc.CreationOptions.IsLowerBoundaryInclusive;
        if (_doc.CreationOptions.LowerBoundaryToIgnore.HasValue)
          LowerBoundary = _doc.CreationOptions.LowerBoundaryToIgnore.Value;

        IgnoreValuesAboveUpperBoundary = _doc.CreationOptions.UpperBoundaryToIgnore.HasValue;
        IsUpperBoundaryInclusive = _doc.CreationOptions.IsUpperBoundaryInclusive;
        if (_doc.CreationOptions.UpperBoundaryToIgnore.HasValue)
          UpperBoundary = _doc.CreationOptions.UpperBoundaryToIgnore.Value;
      }
    }

    /// <inheritdoc/>
    public override bool Apply(bool disposeController)
    {
      _doc.CreationOptions.IgnoreNaN = IgnoreNaNValues;
      _doc.CreationOptions.IgnoreInfinity = IgnoreInfiniteValues;

      if (IgnoreValuesBelowLowerBoundary)
      {
        _doc.CreationOptions.IsLowerBoundaryInclusive = IsLowerBoundaryInclusive;
        _doc.CreationOptions.LowerBoundaryToIgnore = LowerBoundary;
      }
      else
      {
        _doc.CreationOptions.IsLowerBoundaryInclusive = true;
        _doc.CreationOptions.LowerBoundaryToIgnore = null;
      }

      if (IgnoreValuesAboveUpperBoundary)
      {
        _doc.CreationOptions.IsUpperBoundaryInclusive = IsUpperBoundaryInclusive;
        _doc.CreationOptions.UpperBoundaryToIgnore = UpperBoundary;
      }
      else
      {
        _doc.CreationOptions.IsUpperBoundaryInclusive = true;
        _doc.CreationOptions.UpperBoundaryToIgnore = null;
      }

      _doc.CreationOptions.IsUserDefinedBinningType = !UseAutomaticBinning;

      if (!_binningController.Apply(disposeController))
        return ApplyEnd(false, disposeController);

      bool shouldShowDialog = HistogramCreation.PopulateHistogramCreationInformation(_doc);

      if (disposeController) // user pressed ok
      {
        if (ShouldLeaveDialogOpen(_doc))
        {
          Initialize(true);
          return ApplyEnd(false, disposeController);
        }
        else
        {
          return ApplyEnd(true, disposeController);
        }
      }
      else
      {
        // we pressed apply thus we must update the gui
        if (ShouldLeaveDialogOpen(_doc))
        {
          Initialize(true);
          return ApplyEnd(false, disposeController);
        }
        else
        {
          Initialize(true);
          return ApplyEnd(true, disposeController);
        }
      }
    }



    private void EhBinningTypeChanged(Type bintype)
    {
      if (bintype is not null)
      {

        if (_doc.CreationOptions.Binning.GetType() == bintype)
          return;

        var binning = (IBinning)Activator.CreateInstance(bintype);

        _doc.CreationOptions.Binning = binning;

        HistogramCreation.PopulateHistogramCreationInformation(_doc);
        Initialize(true);
      }
    }

    private void EhAutomaticBinningTypeChanged()
    {
      var wasUserBefore = _doc.CreationOptions.IsUserDefinedBinningType;
      _doc.CreationOptions.IsUserDefinedBinningType = !UseAutomaticBinning;

      if (!_doc.CreationOptions.IsUserDefinedBinningType && wasUserBefore)
      {
        HistogramCreation.PopulateHistogramCreationInformation(_doc);
        Initialize(true);
      }
    }

    private static bool ShouldLeaveDialogOpen(HistogramCreationInformation histInfo)
    {
      bool showDialog;
      switch (histInfo.UserInteractionLevel)
      {
        case Gui.UserInteractionLevel.None:
          showDialog = false;
          break;

        case Gui.UserInteractionLevel.InteractOnErrors:
          showDialog = histInfo.Errors.Count > 0;
          break;

        case Gui.UserInteractionLevel.InteractOnWarningsAndErrors:
          showDialog = histInfo.Errors.Count > 0 || histInfo.Warnings.Count > 0;
          break;

        case Gui.UserInteractionLevel.InteractAlways:
          showDialog = histInfo.Errors.Count > 0 || histInfo.Warnings.Count > 0;
          break;

        default:
          throw new NotImplementedException("userInteractionLevel");
      }
      return showDialog;
    }
  }
}
