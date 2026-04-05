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

  /// <summary>
  /// View interface for editing <see cref="NeonCalibrationOptions"/>.
  /// </summary>
  public interface INeonCalibrationOptionsView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Controller for editing <see cref="NeonCalibrationOptions"/>.
  /// </summary>
  [ExpectedTypeOfView(typeof(INeonCalibrationOptionsView))]
  [UserControllerForObject(typeof(NeonCalibrationOptions))]
  public class NeonCalibrationOptionsController : MVCANControllerEditImmutableDocBase<NeonCalibrationOptions, INeonCalibrationOptionsView>
  {
    /// <inheritdoc/>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_peakFindingController, () => PeakFindingController = null);
    }

    #region Bindings

    private ItemsController<XAxisUnit> _XAxisUnit;

    /// <summary>
    /// Gets or sets the unit used for the x-axis in the calibration data.
    /// </summary>
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
    private void EhXAxisUnitChanged(XAxisUnit obj)
    {
      OnPropertyChanged(nameof(IsLaserWavelengthValid));
    }

    private double _laserWavelength_Nanometer;

    /// <summary>
    /// Gets or sets the laser wavelength in nanometers.
    /// </summary>
    public double LaserWavelength_Nanometer
    {
      get => _laserWavelength_Nanometer;
      set
      {
        if (!(_laserWavelength_Nanometer == value))
        {
          _laserWavelength_Nanometer = value;
          OnPropertyChanged(nameof(LaserWavelength_Nanometer));
          OnPropertyChanged(nameof(IsLaserWavelengthValid));
        }
      }
    }

    /// <summary>
    /// Gets a value indicating whether the current laser wavelength value is valid for the selected x-axis unit.
    /// </summary>
    public bool IsLaserWavelengthValid
    {
      get
      {
        return XAxisUnit.SelectedValue != Altaxo.Science.Spectroscopy.Raman.XAxisUnit.RelativeShiftInverseCentimeter ||
               LaserWavelength_Nanometer > 0;
      }
    }


    private PeakSearchingAndFittingOptionsController _peakFindingController;

    /// <summary>
    /// Gets or sets the controller used to edit peak searching and fitting options.
    /// </summary>
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

    /// <summary>
    /// Gets or sets a value indicating whether peaks corresponding to multiple NIST peaks should be filtered out.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the controller used to edit the interpolation method.
    /// </summary>
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

    /// <summary>
    /// Gets or sets a value indicating whether the interpolation should ignore variance / standard deviation.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the wavelength search tolerance.
    /// </summary>
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

    /// <inheritdoc/>
    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        XAxisUnit = new ItemsController<XAxisUnit>(new Collections.SelectableListNodeList(_doc.XAxisUnit), EhXAxisUnitChanged);
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



    /// <inheritdoc/>
    public override bool Apply(bool disposeController)
    {
      if (XAxisUnit.SelectedValue == Altaxo.Science.Spectroscopy.Raman.XAxisUnit.RelativeShiftInverseCentimeter &&
        !(LaserWavelength_Nanometer > 0))
      {
        Current.Gui.ErrorMessageBox("Please enter a valid laser wavelength!");
        return ApplyEnd(false, disposeController);
      }

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
