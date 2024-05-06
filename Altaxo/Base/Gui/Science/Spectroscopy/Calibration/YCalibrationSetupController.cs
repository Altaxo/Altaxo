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
using System.Windows.Input;
using Altaxo.Calc;
using Altaxo.Calc.Interpolation;
using Altaxo.Data;
using Altaxo.Gui.Calc.Interpolation;
using Altaxo.Science.Spectroscopy;

namespace Altaxo.Gui.Science.Spectroscopy.Calibration
{

  public interface IYCalibrationSetupView : IDataContextAwareView { }

  [ExpectedTypeOfView(typeof(IYCalibrationSetupView))]
  [UserControllerForObject(typeof(YCalibrationSetup))]
  public class YCalibrationSetupController : MVCANControllerEditImmutableDocBase<YCalibrationSetup, IYCalibrationSetupView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(PreprocessingController, () => PreprocessingController = null!);
      yield return new ControllerAndSetNullMethod(FunctionController, () => FunctionController = null!);
    }


    public YCalibrationSetupController()
    {
      CmdSwapYColumns = new RelayCommand(EhSwapYColumns, EhCanSwapYColumns);
    }

    #region Bindings

    public ICommand CmdSwapYColumns { get; }

    private string _xColumn;

    public string XColumn
    {
      get => _xColumn;
      set
      {
        if (!(_xColumn == value))
        {
          _xColumn = value;
          OnPropertyChanged(nameof(XColumn));
        }
      }
    }

    private DataColumn _signalColumn;

    public DataColumn SignalColumn
    {
      get => _signalColumn;
      set
      {
        if (!(_signalColumn == value))
        {
          _signalColumn = value;
          OnPropertyChanged(nameof(SignalColumn));
        }
      }
    }

    private DataColumn _darkColumn;

    public DataColumn DarkColumn
    {
      get => _darkColumn;
      set
      {
        if (!(_darkColumn == value))
        {
          _darkColumn = value;
          OnPropertyChanged(nameof(DarkColumn));
        }
      }
    }

    private SpectralPreprocessingController _preprocessingController;

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


    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        XColumn = _doc.XColumn.Name;
        SignalColumn = _doc.YColumn;

        var preprocessingController = new SpectralPreprocessingController();
        preprocessingController.InitializeDocument(_doc.SpectralPreprocessing);
        Current.Gui.FindAndAttachControlTo(preprocessingController);
        PreprocessingController = preprocessingController;

        var functionController = new ScalarFunctionController();
        functionController.InitializeDocument(_doc.CurveShape);
        //Current.Gui.FindAndAttachControlTo(_functionController);
        FunctionController = functionController;

        SmoothResultingCurve = _doc.InterpolationMethod is not null;
        InterpolationMethod = new InterpolationFunctionOptionsController(_doc.InterpolationMethod ?? new PolyharmonicSpline1DOptions() { DerivativeOrder = 2, RegularizationParameter = 1000 });
      }
    }

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


      _doc = new YCalibrationSetup
      {
        XColumn = _doc.XColumn,
        YColumn = SignalColumn,
        SpectralPreprocessing = (SpectralPreprocessingOptionsBase)PreprocessingController.ModelObject,
        CurveShape = curveInstance,
        InterpolationMethod = interpolationMethod,
      };

      return ApplyEnd(true, disposeController);
    }

    private void EhSwapYColumns()
    {
      (DarkColumn, SignalColumn) = (SignalColumn, DarkColumn);
    }

    private bool EhCanSwapYColumns()
    {
      return DarkColumn is not null;
    }

  }
}
