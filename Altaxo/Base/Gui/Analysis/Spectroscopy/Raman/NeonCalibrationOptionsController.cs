using System.Collections.Generic;
using Altaxo.Gui.Common;
using Altaxo.Science.Spectroscopy;
using Altaxo.Science.Spectroscopy.Calibration;
using Altaxo.Science.Spectroscopy.Raman;
using Altaxo.Science.Spectroscopy.Resampling;

namespace Altaxo.Gui.Analysis.Spectroscopy.Raman
{

  public interface INeonCalibrationOptionsView : IDataContextAwareView
  {
  }

  [ExpectedTypeOfView(typeof(INeonCalibrationOptionsView))]
  [UserControllerForObject(typeof(NeonCalibrationOptions))]
  public class NeonCalibrationOptionsController : MVCANControllerEditImmutableDocBase<NeonCalibrationOptions, INeonCalibrationOptionsView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_peakFindingController, () =>PeakFindingController = null);
    }

    #region Bindings

    private ItemsController<XAxisUnit> _XAxisUnit;

    public ItemsController<XAxisUnit> XAxisUnit
    {
      get => _XAxisUnit;
      set
      {
        if (!(_XAxisUnit == value))
        {
          _XAxisUnit = value;
          OnPropertyChanged(nameof(XAxisUnit));
        }
      }
    }

    private double _laserWavelength_Nanometer;

    public double LaserWavelength_Nanometer
    {
      get => _laserWavelength_Nanometer;
      set
      {
        if (!(_laserWavelength_Nanometer == value))
        {
          _laserWavelength_Nanometer = value;
          OnPropertyChanged(nameof(LaserWavelength_Nanometer));
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

    private bool _filterOutPeaksCorrespondingToMultipleNistPeaks;

    public bool FilterOutPeaksCorrespondingToMultipleNistPeaks
    {
      get => _filterOutPeaksCorrespondingToMultipleNistPeaks;
      set
      {
        if (!(_filterOutPeaksCorrespondingToMultipleNistPeaks == value))
        {
          _filterOutPeaksCorrespondingToMultipleNistPeaks = value;
          OnPropertyChanged(nameof(FilterOutPeaksCorrespondingToMultipleNistPeaks));
        }
      }
    }



    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if(initData)
      {
        XAxisUnit = new ItemsController<XAxisUnit>(new Collections.SelectableListNodeList(_doc.XAxisUnit));

        if (_doc.LaserWavelength_Nanometer == 0)
          LaserWavelength_Nanometer = 532;
        else
          LaserWavelength_Nanometer = _doc.LaserWavelength_Nanometer;

        FilterOutPeaksCorrespondingToMultipleNistPeaks = _doc.FilterOutPeaksCorrespondingToMultipleNistPeaks;

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

      if(findOptions.Preprocessing.Calibration is not CalibrationNone)
      {
        Current.Gui.InfoMessageBox("This calibration needs the original x/y-values, thus calibration in the preprocessing steps is disabled here.", "Note");
        findOptions = findOptions with { Preprocessing = findOptions.Preprocessing with { Calibration = new CalibrationNone() } };
      }
      if (findOptions.Preprocessing.Resampling is not ResamplingNone)
      {
        Current.Gui.InfoMessageBox("This calibration needs the original x-values, thus resampling in the preprocessing steps is disabled here.", "Note");
        findOptions = findOptions with { Preprocessing = findOptions.Preprocessing with { Resampling = new ResamplingNone() } };
      }

      _doc = _doc with
      {
        LaserWavelength_Nanometer = LaserWavelength_Nanometer,
        XAxisUnit = XAxisUnit.SelectedValue,
        FilterOutPeaksCorrespondingToMultipleNistPeaks = FilterOutPeaksCorrespondingToMultipleNistPeaks,
        PeakFindingOptions = findOptions,
      };

      return ApplyEnd(true, disposeController);
    }

    
  }
}
