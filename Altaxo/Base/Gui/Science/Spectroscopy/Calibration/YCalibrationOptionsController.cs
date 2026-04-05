#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2023 Dr. Dirk Lellinger
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
using Altaxo.Calc;
using Altaxo.Calc.Interpolation;
using Altaxo.Gui.Calc.Interpolation;
using Altaxo.Science.Spectroscopy;
using Altaxo.Science.Spectroscopy.Calibration;

namespace Altaxo.Gui.Science.Spectroscopy.Calibration
{
  /// <summary>
  /// View interface for y-calibration options.
  /// </summary>
  public interface IYCalibrationOptionsView : IDataContextAwareView { }

  /// <summary>
  /// Controller for <see cref="YCalibrationOptions"/>.
  /// </summary>
  [ExpectedTypeOfView(typeof(IYCalibrationOptionsView))]
  [UserControllerForObject(typeof(YCalibrationOptions))]
  public class YCalibrationOptionsController : MVCANControllerEditImmutableDocBase<YCalibrationOptions, IYCalibrationOptionsView>
  {
    /// <inheritdoc/>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(PreprocessingController, () => PreprocessingController = null!);
      yield return new ControllerAndSetNullMethod(FunctionController, () => FunctionController = null!);
    }

    #region Bindings

    private SpectralPreprocessingController _preprocessingController;

    /// <summary>
    /// Gets or sets the preprocessing controller.
    /// </summary>
    public SpectralPreprocessingController PreprocessingController
    {
      get => _preprocessingController;
      set
      {
        if (!(_preprocessingController == value))
        {
          _preprocessingController?.Dispose();
          _preprocessingController = value;
          OnPropertyChanged(nameof(PreprocessingController));
        }
      }
    }

    private ScalarFunctionController _functionController;

    /// <summary>
    /// Gets or sets the calibration-function controller.
    /// </summary>
    public ScalarFunctionController FunctionController
    {
      get => _functionController;
      set
      {
        if (!(_functionController == value))
        {
          _functionController?.Dispose();
          _functionController = value;
          OnPropertyChanged(nameof(FunctionController));
        }
      }
    }

    private bool _smoothResultingCurve;

    /// <summary>
    /// Gets or sets a value indicating whether the resulting curve is smoothed.
    /// </summary>
    public bool SmoothResultingCurve
    {
      get => _smoothResultingCurve;
      set
      {
        if (!(_smoothResultingCurve == value))
        {
          _smoothResultingCurve = value;
          OnPropertyChanged(nameof(SmoothResultingCurve));
        }
      }
    }


    private InterpolationFunctionOptionsController _interpolationMethod;

    /// <summary>
    /// Gets or sets the interpolation method controller.
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

    private double _minimalValidXValueOfCurve;

    /// <summary>
    /// Gets or sets the minimum valid X value of the calibration curve.
    /// </summary>
    public double MinimalValidXValueOfCurve
    {
      get => _minimalValidXValueOfCurve;
      set
      {
        if (!(_minimalValidXValueOfCurve == value))
        {
          _minimalValidXValueOfCurve = value;
          OnPropertyChanged(nameof(MinimalValidXValueOfCurve));
        }
      }
    }


    private double _maximalValidXValueOfCurve;

    /// <summary>
    /// Gets or sets the maximum valid X value of the calibration curve.
    /// </summary>
    public double MaximalValidXValueOfCurve
    {
      get => _maximalValidXValueOfCurve;
      set
      {
        if (!(_maximalValidXValueOfCurve == value))
        {
          _maximalValidXValueOfCurve = value;
          OnPropertyChanged(nameof(MaximalValidXValueOfCurve));
        }
      }
    }

    private double _maximalGainRatio;

    /// <summary>
    /// Gets or sets the maximum gain ratio.
    /// </summary>
    public double MaximalGainRatio
    {
      get => _maximalGainRatio;
      set
      {
        if (!(_maximalGainRatio == value))
        {
          _maximalGainRatio = value;
          OnPropertyChanged(nameof(MaximalGainRatio));
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
        var preprocessingController = new SpectralPreprocessingController();
        preprocessingController.InitializeDocument(_doc.Preprocessing);
        Current.Gui.FindAndAttachControlTo(preprocessingController);
        PreprocessingController = preprocessingController;

        var functionController = new ScalarFunctionController();
        functionController.InitializeDocument(_doc.CurveShape);
        //Current.Gui.FindAndAttachControlTo(_functionController);
        FunctionController = functionController;

        SmoothResultingCurve = _doc.InterpolationMethod is not null;
        InterpolationMethod = new InterpolationFunctionOptionsController(_doc.InterpolationMethod ?? new PolyharmonicSpline1DOptions() { DerivativeOrder = 2, RegularizationParameter = 1000 });

        MinimalValidXValueOfCurve = _doc.MinimalValidXValueOfCurve;
        MaximalValidXValueOfCurve = _doc.MaximalValidXValueOfCurve;
        MaximalGainRatio = _doc.MaximalGainRatio;
      }
    }

    /// <inheritdoc/>
    public override bool Apply(bool disposeController)
    {
      if (!PreprocessingController.Apply(disposeController))
      {
        return ApplyEnd(false, disposeController);
      }

      if (!FunctionController.Apply(disposeController))
      {
        return ApplyEnd(false, disposeController);
      }

      var curveInstance = (IScalarFunctionDD)FunctionController.ModelObject;


      IInterpolationFunctionOptions? interpolationMethod = null;
      if (SmoothResultingCurve)
      {
        if (!InterpolationMethod.Apply(disposeController))
          return ApplyEnd(false, disposeController);
        else
          interpolationMethod = (IInterpolationFunctionOptions)InterpolationMethod.ModelObject;
      }

      _doc = new YCalibrationOptions
      {
        Preprocessing = (SpectralPreprocessingOptionsBase)PreprocessingController.ModelObject,
        CurveShape = curveInstance,
        InterpolationMethod = interpolationMethod,
        MinimalValidXValueOfCurve = MinimalValidXValueOfCurve,
        MaximalValidXValueOfCurve = MaximalValidXValueOfCurve,
        MaximalGainRatio = MaximalGainRatio,
      };

      return ApplyEnd(true, disposeController);
    }
  }
}
