using System.Collections.Generic;
using System.Linq;
using Altaxo.Calc.Interpolation;
using Altaxo.Gui.Calc.Interpolation;
using Altaxo.Gui.Common;
using Altaxo.Science.Spectroscopy;
using Altaxo.Science.Spectroscopy.Calibration;
using Altaxo.Science.Spectroscopy.Raman;
using Altaxo.Science.Spectroscopy.Resampling;

namespace Altaxo.Gui.Science.Spectroscopy.Raman
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
      yield return new ControllerAndSetNullMethod(_peakFindingController, () => PeakFindingController = null);
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

    private InterpolationFunctionOptionsController _interpolationMethod;

    public InterpolationFunctionOptionsController InterpolationMethod
    {
      get => _interpolationMethod;
      set
      {
        if (!(_interpolationMethod == value))
        {
          _interpolationMethod?.Dispose();
          _interpolationMethod = value;
          OnPropertyChanged(nameof(InterpolationMethod));
        }
      }
    }


    private bool _interpolationIgnoreVariance;

    public bool InterpolationIgnoreVariance
    {
      get => _interpolationIgnoreVariance;
      set
      {
        if (!(_interpolationIgnoreVariance == value))
        {
          _interpolationIgnoreVariance = value;
          OnPropertyChanged(nameof(InterpolationIgnoreVariance));
        }
      }
    }

    private double _searchTolerance;

    public double SearchTolerance
    {
      get => _searchTolerance;
      set
      {
        if (!(_searchTolerance == value))
        {
          _searchTolerance = value;
          OnPropertyChanged(nameof(SearchTolerance));
        }
      }
    }


    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
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

        InterpolationMethod = new InterpolationFunctionOptionsController(_doc.InterpolationMethod);
        InterpolationIgnoreVariance = _doc.InterpolationIgnoreStdDev;

        SearchTolerance = _doc.Wavelength_Tolerance_nm;
      }
    }

    public override bool Apply(bool disposeController)
    {
      PeakSearchingAndFittingOptions findOptions;
      IInterpolationFunctionOptions interpolationMethod;
      if (!PeakFindingController.Apply(disposeController))
        return ApplyEnd(false, disposeController);
      else
        findOptions = (PeakSearchingAndFittingOptions)PeakFindingController.ModelObject;

      if (findOptions.Preprocessing.OfType<IXCalibrationTable>().Any())
      {
        Current.Gui.ErrorMessageBox("This calibration needs the original x/y-values, thus please disable x-calibration!", "X-Calibration found!");
        return ApplyEnd(false, disposeController);
      }
      if (findOptions.Preprocessing.Where(p => p is IResampling && p is not ResamplingNone).Any())
      {
        Current.Gui.ErrorMessageBox("This calibration needs the original x-values, thus please disable resampling here!", "Resampling step found!");
        return ApplyEnd(false, disposeController);
      }

      if (!InterpolationMethod.Apply(disposeController))
        return ApplyEnd(false, disposeController);
      else
        interpolationMethod = (IInterpolationFunctionOptions)InterpolationMethod.ModelObject;


      _doc = _doc with
      {
        LaserWavelength_Nanometer = LaserWavelength_Nanometer,
        XAxisUnit = XAxisUnit.SelectedValue,
        FilterOutPeaksCorrespondingToMultipleNistPeaks = FilterOutPeaksCorrespondingToMultipleNistPeaks,
        PeakFindingOptions = findOptions,
        InterpolationMethod = interpolationMethod,
        InterpolationIgnoreStdDev = InterpolationIgnoreVariance,
        Wavelength_Tolerance_nm = SearchTolerance,
      };

      return ApplyEnd(true, disposeController);
    }


  }
}
