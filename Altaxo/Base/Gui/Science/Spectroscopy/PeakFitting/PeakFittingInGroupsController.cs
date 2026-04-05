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

using System.Collections.Generic;
using Altaxo.Science.Spectroscopy.PeakFitting;
using Altaxo.Units;

namespace Altaxo.Gui.Science.Spectroscopy.PeakFitting
{

  /// <summary>
  /// View interface for peak fitting in groups.
  /// </summary>
  public interface IPeakFittingInGroupsView : IDataContextAwareView { }


  /// <summary>
  /// Controller for <see cref="PeakFittingInGroups"/>.
  /// </summary>
  [UserControllerForObject(typeof(PeakFittingInGroups))]
  [ExpectedTypeOfView(typeof(IPeakFittingInGroupsView))]
  public class PeakFittingInGroupsController : PeakFittingBaseController<PeakFittingInGroups, IPeakFittingInGroupsView>
  {
    /// <inheritdoc/>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private DimensionfulQuantity _minimalGroupSeparationFWHMFactor;

    /// <summary>
    /// Gets or sets the minimal group separation in FWHM factors.
    /// </summary>
    public DimensionfulQuantity MinimalGroupSeparationFWHMFactor
    {
      get => _minimalGroupSeparationFWHMFactor;
      set
      {
        if (!(_minimalGroupSeparationFWHMFactor == value))
        {
          _minimalGroupSeparationFWHMFactor = value;
          OnPropertyChanged(nameof(MinimalGroupSeparationFWHMFactor));
        }
      }
    }

    private int _minimumOrderOfBaselinePolynomial;

    /// <summary>
    /// Gets or sets the minimum order of the baseline polynomial.
    /// </summary>
    public int MinimumOrderOfBaselinePolynomial
    {
      get => _minimumOrderOfBaselinePolynomial;
      set
      {
        if (!(_minimumOrderOfBaselinePolynomial == value))
        {
          _minimumOrderOfBaselinePolynomial = value;
          OnPropertyChanged(nameof(MinimumOrderOfBaselinePolynomial));
        }
      }
    }

    private int _maximumOrderOfBaselinePolynomial;

    /// <summary>
    /// Gets or sets the maximum order of the baseline polynomial.
    /// </summary>
    public int MaximumOrderOfBaselinePolynomial
    {
      get => _maximumOrderOfBaselinePolynomial;
      set
      {
        if (!(_maximumOrderOfBaselinePolynomial == value))
        {
          _maximumOrderOfBaselinePolynomial = value;
          OnPropertyChanged(nameof(MaximumOrderOfBaselinePolynomial));
        }
      }
    }

    private int _numberOfPeaksAtMaximalOrderOfBaselinePolynomial;

    /// <summary>
    /// Gets or sets the number of peaks at the maximal baseline polynomial order.
    /// </summary>
    public int NumberOfPeaksAtMaximalOrderOfBaselinePolynomial
    {
      get => _numberOfPeaksAtMaximalOrderOfBaselinePolynomial;
      set
      {
        if (!(_numberOfPeaksAtMaximalOrderOfBaselinePolynomial == value))
        {
          _numberOfPeaksAtMaximalOrderOfBaselinePolynomial = value;
          OnPropertyChanged(nameof(NumberOfPeaksAtMaximalOrderOfBaselinePolynomial));
        }
      }
    }



    private bool _isEvaluatingSeparateVariances;

    /// <summary>
    /// Gets or sets a value indicating whether separate variances are evaluated.
    /// </summary>
    public bool IsEvaluatingSeparateVariances
    {
      get => _isEvaluatingSeparateVariances;
      set
      {
        if (!(_isEvaluatingSeparateVariances == value))
        {
          _isEvaluatingSeparateVariances = value;
          OnPropertyChanged(nameof(IsEvaluatingSeparateVariances));
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

        FitWidthScalingFactor = new DimensionfulQuantity(_doc.FitWidthScalingFactor, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(FitWidthScalingFactorEnvironment.DefaultUnit);

        IsMinimalFWHMValueInXUnits = _doc.IsMinimalFWHMValueInXUnits;
        MinimalFWHMValue = _doc.MinimalFWHMValue;

        MinimalGroupSeparationFWHMFactor = new DimensionfulQuantity(_doc.MinimalGroupSeparationFWHMFactor, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(FitWidthScalingFactorEnvironment.DefaultUnit);

        MinimumOrderOfBaselinePolynomial = _doc.MinimalOrderOfBaselinePolynomial;
        MaximumOrderOfBaselinePolynomial = _doc.MaximalOrderOfBaselinePolynomial;
        NumberOfPeaksAtMaximalOrderOfBaselinePolynomial = _doc.NumberOfPeaksAtMaximalOrderOfBaselinePolynomial;
        IsEvaluatingSeparateVariances = _doc.IsEvaluatingSeparateVariances;
      }
    }

    /// <inheritdoc/>
    public override bool Apply(bool disposeController)
    {
      _doc = _doc with
      {
        FitFunction = _currentFitFunction,
        FitWidthScalingFactor = FitWidthScalingFactor.AsValueInSIUnits,
        IsMinimalFWHMValueInXUnits = IsMinimalFWHMValueInXUnits,
        MinimalFWHMValue = MinimalFWHMValue,
        MinimalGroupSeparationFWHMFactor = MinimalGroupSeparationFWHMFactor.AsValueInSIUnits,
        MinimalOrderOfBaselinePolynomial = MinimumOrderOfBaselinePolynomial,
        MaximalOrderOfBaselinePolynomial = MaximumOrderOfBaselinePolynomial,
        NumberOfPeaksAtMaximalOrderOfBaselinePolynomial = NumberOfPeaksAtMaximalOrderOfBaselinePolynomial,
        IsEvaluatingSeparateVariances = IsEvaluatingSeparateVariances,
      };

      return ApplyEnd(true, disposeController);
    }


  }
}
