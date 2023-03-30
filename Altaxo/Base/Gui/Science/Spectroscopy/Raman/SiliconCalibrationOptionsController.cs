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
using System.Linq;
using Altaxo.Science.Spectroscopy;
using Altaxo.Science.Spectroscopy.Calibration;
using Altaxo.Science.Spectroscopy.Raman;
using Altaxo.Science.Spectroscopy.Resampling;
using Altaxo.Units;

namespace Altaxo.Gui.Science.Spectroscopy.Raman
{

  public interface ISiliconCalibrationOptionsView : IDataContextAwareView
  {
  }

  [ExpectedTypeOfView(typeof(ISiliconCalibrationOptionsView))]
  [UserControllerForObject(typeof(SiliconCalibrationOptions))]
  public class SiliconCalibrationOptionsController : MVCANControllerEditImmutableDocBase<SiliconCalibrationOptions, ISiliconCalibrationOptionsView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_peakFindingController, () => PeakFindingController = null);
    }

    public SiliconCalibrationOptionsController()
    {
      TemperatureEnvironment = new QuantityWithUnitGuiEnvironment(new IUnit[]
      {
        Altaxo.Units.Temperature.DegreesCelsius.Instance,
        Altaxo.Units.Temperature.DegreesFahrenheit.Instance,
        Altaxo.Units.Temperature.Kelvin.Instance,
      }
      );
    }

    #region Bindings

    private DimensionfulQuantity _temperature;

    public QuantityWithUnitGuiEnvironment TemperatureEnvironment { get; }

    public DimensionfulQuantity Temperature
    {
      get => _temperature;
      set
      {
        if (!(_temperature == value))
        {
          _temperature = value;
          OnPropertyChanged(nameof(Temperature));
        }
      }
    }

    private double _shiftTolerance;

    public double ShiftTolerance
    {
      get => _shiftTolerance;
      set
      {
        if (!(_shiftTolerance == value))
        {
          _shiftTolerance = value;
          OnPropertyChanged(nameof(ShiftTolerance));
        }
      }
    }





    private PeakSearchingAndFittingOptionsController _peakFindingController;

    public PeakSearchingAndFittingOptionsController PeakFindingController
    {
      get => _peakFindingController;
      set
      {
        if (!(_peakFindingController == value))
        {
          _peakFindingController?.Dispose();
          _peakFindingController = value;
          OnPropertyChanged(nameof(PeakFindingController));
        }
      }
    }



    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        ShiftTolerance = _doc.RelativeShift_Tolerance_invcm;
        Temperature = _doc.Temperature;
        var peakFindingController = new PeakSearchingAndFittingOptionsController();
        peakFindingController.InitializeDocument(_doc.PeakFindingOptions);
        Current.Gui.FindAndAttachControlTo(peakFindingController);
        PeakFindingController = peakFindingController;
      }
    }

    public override bool Apply(bool disposeController)
    {
      PeakSearchingAndFittingOptions findOptions;
      if (!PeakFindingController.Apply(disposeController))
        return ApplyEnd(false, disposeController);
      else
        findOptions = (PeakSearchingAndFittingOptions)PeakFindingController.ModelObject;

      if (findOptions.Preprocessing.OfType<IXCalibrationTable>().Any())
      {
        Current.Gui.InfoMessageBox("This calibration needs the original x/y-values, thus please disable x-calibration here.", "X-calibration found");
        return ApplyEnd(false, disposeController);
      }
      if (findOptions.Preprocessing.Where(p => p is IResampling && p is not ResamplingNone).Any())
      {
        Current.Gui.ErrorMessageBox("This calibration needs the original x-values, thus please disable resampling here!", "Resampling step found!");
        return ApplyEnd(false, disposeController);
      }


      _doc = _doc with
      {
        RelativeShift_Tolerance_invcm = ShiftTolerance,
        Temperature = Temperature,
        PeakFindingOptions = findOptions,
      };

      return ApplyEnd(true, disposeController);
    }


  }
}
