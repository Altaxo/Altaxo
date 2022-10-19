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
using Altaxo.Science.Spectroscopy.PeakSearching;
using Altaxo.Units;

namespace Altaxo.Gui.Science.Spectroscopy.PeakSearching
{
  public interface IPeakSearchingByCwtView : IDataContextAwareView
  {
  }

  [UserControllerForObject(typeof(PeakSearchingByCwt))]
  [ExpectedTypeOfView(typeof(IPeakSearchingByCwtView))]
  public class PeakSearchingByCwtController : MVCANControllerEditImmutableDocBase<PeakSearchingByCwt, IPeakSearchingByCwtView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private ItemsController<Type> _wavelet;

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


    public QuantityWithUnitGuiEnvironment NumberOfOctavesEnvironment => RelationEnvironment.Instance;


    private DimensionfulQuantity _MinimalRidgeLengthInOctaves;

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

    public QuantityWithUnitGuiEnvironment RatioEnvironment => RelationEnvironment.Instance;


    private DimensionfulQuantity _minimalRelativeGaussianAmplitude;

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


    #endregion

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
      }
    }

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
