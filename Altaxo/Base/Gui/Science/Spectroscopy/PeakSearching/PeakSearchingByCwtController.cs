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
using Altaxo.Collections;
using Altaxo.Gui.Common;
using Altaxo.Main.Services;
using Altaxo.Science.Spectroscopy.PeakEnhancement;
using Altaxo.Science.Spectroscopy.PeakSearching;
using Altaxo.Units;

namespace Altaxo.Gui.Science.Spectroscopy.PeakSearching
{
    /// <summary>
/// Defines the contract for peak Searching By Cwt View.
/// </summary>
public interface IPeakSearchingByCwtView : IDataContextAwareView
  {
  }

  /// <summary>
/// Represents a controller for peak Searching By Cwt.
/// </summary>
[UserControllerForObject(typeof(PeakSearchingByCwt))]
  [ExpectedTypeOfView(typeof(IPeakSearchingByCwtView))]
  public class PeakSearchingByCwtController : MVCANControllerEditImmutableDocBase<PeakSearchingByCwt, IPeakSearchingByCwtView>
  {
        /// <inheritdoc />
public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_subControllerPeakEnhancement, () => SubControllerPeakEnhancement = null);
    }

    #region Bindings

    private ItemsController<Type> _wavelet;

        /// <summary>
/// Gets or sets the wavelet.
/// </summary>
/// <value>
/// The wavelet.
/// </value>
public ItemsController<Type> Wavelet
    {
      get => _wavelet;
      set
      {
        if (!(_wavelet == value))
        {
          _wavelet = value;
          OnPropertyChanged(nameof(Wavelet));
        }
      }
    }

    private int _pointsPerOctave;

        /// <summary>
/// Gets or sets the number Of Points Per Octave.
/// </summary>
/// <value>
/// The number Of Points Per Octave.
/// </value>
public int NumberOfPointsPerOctave
    {
      get => _pointsPerOctave;
      set
      {
        if (!(_pointsPerOctave == value))
        {
          _pointsPerOctave = value;
          OnPropertyChanged(nameof(NumberOfPointsPerOctave));
        }
      }
    }


        /// <summary>
/// Gets the number Of Octaves Environment.
/// </summary>
/// <value>
/// The number Of Octaves Environment.
/// </value>
public QuantityWithUnitGuiEnvironment NumberOfOctavesEnvironment => RelationEnvironment.Instance;


    private DimensionfulQuantity _MinimalRidgeLengthInOctaves;

        /// <summary>
/// Gets or sets the minimal Ridge Length In Octaves.
/// </summary>
/// <value>
/// The minimal Ridge Length In Octaves.
/// </value>
public DimensionfulQuantity MinimalRidgeLengthInOctaves
    {
      get => _MinimalRidgeLengthInOctaves;
      set
      {
        if (!(_MinimalRidgeLengthInOctaves == value))
        {
          _MinimalRidgeLengthInOctaves = value;
          OnPropertyChanged(nameof(MinimalRidgeLengthInOctaves));
        }
      }
    }

    private DimensionfulQuantity _minimalWidthOfRidgeMaximumInOctaves;

        /// <summary>
/// Gets or sets the minimal Width Of Ridge Maximum In Octaves.
/// </summary>
/// <value>
/// The minimal Width Of Ridge Maximum In Octaves.
/// </value>
public DimensionfulQuantity MinimalWidthOfRidgeMaximumInOctaves
    {
      get => _minimalWidthOfRidgeMaximumInOctaves;
      set
      {
        if (!(_minimalWidthOfRidgeMaximumInOctaves == value))
        {
          _minimalWidthOfRidgeMaximumInOctaves = value;
          OnPropertyChanged(nameof(MinimalWidthOfRidgeMaximumInOctaves));
        }
      }
    }

        /// <summary>
/// Gets the ratio Environment.
/// </summary>
/// <value>
/// The ratio Environment.
/// </value>
public QuantityWithUnitGuiEnvironment RatioEnvironment => RelationEnvironment.Instance;


    private DimensionfulQuantity _minimalRelativeGaussianAmplitude;

        /// <summary>
/// Gets or sets the minimal Relative Gaussian Amplitude.
/// </summary>
/// <value>
/// The minimal Relative Gaussian Amplitude.
/// </value>
public DimensionfulQuantity MinimalRelativeGaussianAmplitude
    {
      get => _minimalRelativeGaussianAmplitude;
      set
      {
        if (!(_minimalRelativeGaussianAmplitude == value))
        {
          _minimalRelativeGaussianAmplitude = value;
          OnPropertyChanged(nameof(MinimalRelativeGaussianAmplitude));
        }
      }
    }

    private DimensionfulQuantity _minimalSignalToNoiseRatio;

        /// <summary>
/// Gets or sets the minimal Signal To Noise Ratio.
/// </summary>
/// <value>
/// The minimal Signal To Noise Ratio.
/// </value>
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

    private int? _maximalNumberOfPeaks;

        /// <summary>
/// Gets or sets the maximal Number Of Peaks.
/// </summary>
/// <value>
/// The maximal Number Of Peaks.
/// </value>
public int? MaximalNumberOfPeaks
    {
      get => _maximalNumberOfPeaks;
      set
      {
        if (!(_maximalNumberOfPeaks == value))
        {
          _maximalNumberOfPeaks = value;
          OnPropertyChanged(nameof(MaximalNumberOfPeaks));
        }
      }
    }

    private ItemsController<Type> _availablePeakEnhancementMethods;

        /// <summary>
/// Gets or sets the available Peak Enhancement Methods.
/// </summary>
/// <value>
/// The available Peak Enhancement Methods.
/// </value>
public ItemsController<Type> AvailablePeakEnhancementMethods
    {
      get => _availablePeakEnhancementMethods;
      set
      {
        if (!(_availablePeakEnhancementMethods == value))
        {
          _availablePeakEnhancementMethods = value;
          OnPropertyChanged(nameof(AvailablePeakEnhancementMethods));
        }
      }
    }


    private IMVCANController? _subControllerPeakEnhancement;

        /// <summary>
/// Gets or sets the sub Controller Peak Enhancement.
/// </summary>
/// <value>
/// The sub Controller Peak Enhancement.
/// </value>
public IMVCANController? SubControllerPeakEnhancement
    {
      get => _subControllerPeakEnhancement;
      set
      {
        if (!(_subControllerPeakEnhancement == value))
        {
          _subControllerPeakEnhancement?.Dispose();
          _subControllerPeakEnhancement = value;
          OnPropertyChanged(nameof(SubControllerPeakEnhancement));
        }
      }
    }



    #endregion

        /// <inheritdoc />
protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        Wavelet = new ItemsController<Type>(new SelectableListNodeList(
                    Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(IWaveletForPeakSearching))
                    .Select(t => new SelectableListNode(t.Name, t, t == _doc.Wavelet.GetType()))));

        NumberOfPointsPerOctave = _doc.NumberOfPointsPerOctave;
        MinimalRidgeLengthInOctaves = new DimensionfulQuantity(_doc.MinimalRidgeLengthInOctaves, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(NumberOfOctavesEnvironment.DefaultUnit);
        MinimalWidthOfRidgeMaximumInOctaves = new DimensionfulQuantity(_doc.MinimalWidthOfRidgeMaximumInOctaves, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(NumberOfOctavesEnvironment.DefaultUnit);
        MinimalSignalToNoiseRatio = new DimensionfulQuantity(_doc.MinimalSignalToNoiseRatio, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(RatioEnvironment.DefaultUnit);
        MinimalRelativeGaussianAmplitude = new DimensionfulQuantity(_doc.MinimalRelativeGaussianAmplitude, Altaxo.Units.Dimensionless.Unity.Instance).AsQuantityIn(RatioEnvironment.DefaultUnit);
        MaximalNumberOfPeaks = _doc.MaximalNumberOfPeaks;

        // PeakEnhancement
        CreateSubControllerPeakEnhancement();

        var methodTypes = new List<Type>(ReflectionService.GetNonAbstractSubclassesOf(typeof(IPeakEnhancement)));
        methodTypes.Remove(typeof(Altaxo.Science.Spectroscopy.PeakSearching.PeakSearchingNone));
        methodTypes.Sort(new TypeSorter());

        var methods = new SelectableListNodeList();
        foreach (var methodType in methodTypes)
        {
          methods.Add(new SelectableListNode(methodType.Name, methodType, methodType == _doc.GetType()));
        }
        AvailablePeakEnhancementMethods = new ItemsController<Type>(methods, EhPeakEnhancementMethodTypeChanged);
        AvailablePeakEnhancementMethods.SelectedValue = _doc.PeakEnhancement.GetType();
      }
    }

    private void CreateSubControllerPeakEnhancement()
    {
      var subController = (IMVCANController)Current.Gui.GetController(new object[] { _doc.PeakEnhancement }, typeof(IMVCANController));
      if (subController?.GetType() == GetType())
      {
        subController = null;
      }
      if (subController is not null)
      {
        Current.Gui.FindAndAttachControlTo(subController);
      }
      SubControllerPeakEnhancement = subController;
    }

    private void EhPeakEnhancementMethodTypeChanged(Type newMethodType)
    {
      _doc = _doc with { PeakEnhancement = (IPeakEnhancement)Activator.CreateInstance(newMethodType) };
      CreateSubControllerPeakEnhancement();
    }

    private class TypeSorter : IComparer<Type>
    {
            /// <inheritdoc />
public int Compare(Type x, Type y)
      {
        var xn = x.Name.EndsWith("None");
        var yn = y.Name.EndsWith("None");

        if (xn != yn)
        {
          return xn ? -1 : 1;
        }
        else
        {
          return string.Compare(x.Name, y.Name);
        }
      }
    }

        /// <inheritdoc />
public override bool Apply(bool disposeController)
    {
      try
      {
        var wavelet = (IWaveletForPeakSearching)Activator.CreateInstance(Wavelet.SelectedValue);

        _doc = _doc with
        {
          Wavelet = wavelet,
          NumberOfPointsPerOctave = NumberOfPointsPerOctave,
          MinimalRidgeLengthInOctaves = MinimalRidgeLengthInOctaves.AsValueInSIUnits,
          MinimalWidthOfRidgeMaximumInOctaves = MinimalWidthOfRidgeMaximumInOctaves.AsValueInSIUnits,
          MinimalSignalToNoiseRatio = MinimalSignalToNoiseRatio.AsValueInSIUnits,
          MinimalRelativeGaussianAmplitude = MinimalRelativeGaussianAmplitude.AsValueInSIUnits,
          MaximalNumberOfPeaks = MaximalNumberOfPeaks,
        };
      }
      catch (Exception ex)
      {
        Current.Gui.ErrorMessageBox(ex.Message, "Exception applying dialog");
        return ApplyEnd(false, disposeController);
      }

      return ApplyEnd(true, disposeController);
    }
  }
}
