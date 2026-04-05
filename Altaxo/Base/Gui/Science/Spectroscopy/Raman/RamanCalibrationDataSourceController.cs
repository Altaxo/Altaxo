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
using System.Windows.Input;
using Altaxo.Data;
using Altaxo.Science.Spectroscopy.Raman;

namespace Altaxo.Gui.Science.Spectroscopy.Raman
{
  /// <summary>
  /// View interface for editing <see cref="RamanCalibrationDataSource"/>.
  /// </summary>
  public interface IRamanCalibrationDataSourceView : IDataContextAwareView { }


  /// <summary>
  /// Controller for editing a <see cref="RamanCalibrationDataSource"/>.
  /// </summary>
  [ExpectedTypeOfView(typeof(IRamanCalibrationDataSourceView))]
  [UserControllerForObject(typeof(RamanCalibrationDataSource))]
  public class RamanCalibrationDataSourceController : MVCANControllerEditOriginalDocBase<RamanCalibrationDataSource, IRamanCalibrationDataSourceView>, IMVCSupportsApplyCallback
  {
    private IMVCANController _inputOptionsController;

    private IMVCANController _neonCalibrationData1Controller;
    private IMVCANController _neonCalibrationOptions1Controller;
    private IMVCANController _neonCalibrationData2Controller;
    private IMVCANController _neonCalibrationOptions2Controller;
    private IMVCANController _siliconCalibrationDataController;
    private IMVCANController _siliconCalibrationOptionsController;

    /// <summary>
    /// Occurs after the document was successfully applied.
    /// </summary>
    public event Action SuccessfullyApplied;

    /// <summary>Tab identifier for the input options section.</summary>
    public const string TabInputOptions = "InputOptions";
    /// <summary>Tab identifier for the process options section.</summary>
    public const string TabProcessOptions = "ProcessOptions";
    /// <summary>Tab identifier for process data section.</summary>
    public const string TabProcessData = "ProcessData";

    /// <inheritdoc/>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_inputOptionsController, () => InputOptionsController = null);
      yield return new ControllerAndSetNullMethod(_neonCalibrationOptions1Controller, () => NeonCalibrationOptions1Controller = null);
      yield return new ControllerAndSetNullMethod(_neonCalibrationData1Controller, () => NeonCalibrationData1Controller = null);
      yield return new ControllerAndSetNullMethod(_neonCalibrationOptions2Controller, () => NeonCalibrationOptions2Controller = null);
      yield return new ControllerAndSetNullMethod(_neonCalibrationData2Controller, () => NeonCalibrationData2Controller = null);
      yield return new ControllerAndSetNullMethod(_siliconCalibrationOptionsController, () => SiliconCalibrationOptionsController = null);
      yield return new ControllerAndSetNullMethod(_siliconCalibrationDataController, () => SiliconCalibrationDataController = null);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RamanCalibrationDataSourceController"/> class.
    /// </summary>
    public RamanCalibrationDataSourceController()
    {
      CmdRemoveNeonCalibration2 = new RelayCommand(EhRemoveNeonCalibration2, EhCanRemoveNeonCalibration2);
    }



    #region Bindings

    /// <summary>
    /// Gets the command that removes the second neon calibration (if present).
    /// </summary>
    public ICommand CmdRemoveNeonCalibration2 { get; }


    /// <summary>
    /// Gets or sets the controller responsible for editing the import options.
    /// </summary>
    public IMVCANController InputOptionsController
    {
      get { return _inputOptionsController; }
      set
      {
        if (!object.ReferenceEquals(value, _inputOptionsController))
        {
          _inputOptionsController?.Dispose();
          _inputOptionsController = value;
          OnPropertyChanged(nameof(InputOptionsController));
        }
      }
    }

    /// <summary>
    /// Gets or sets the controller for neon calibration 1 options.
    /// </summary>
    public IMVCANController NeonCalibrationOptions1Controller
    {
      get { return _neonCalibrationOptions1Controller; }
      set
      {
        if (!object.ReferenceEquals(value, _neonCalibrationOptions1Controller))
        {
          _neonCalibrationOptions1Controller?.Dispose();
          _neonCalibrationOptions1Controller = value;
          OnPropertyChanged(nameof(NeonCalibrationOptions1Controller));
        }
      }
    }

    /// <summary>
    /// Gets or sets the controller for neon calibration 1 data.
    /// </summary>
    public IMVCANController NeonCalibrationData1Controller
    {
      get { return _neonCalibrationData1Controller; }
      set
      {
        if (!object.ReferenceEquals(value, _neonCalibrationData1Controller))
        {
          _neonCalibrationData1Controller?.Dispose();
          _neonCalibrationData1Controller = value;
          OnPropertyChanged(nameof(NeonCalibrationData1Controller));
        }
      }
    }

    /// <summary>
    /// Gets or sets the controller for neon calibration 2 options.
    /// </summary>
    public IMVCANController NeonCalibrationOptions2Controller
    {
      get { return _neonCalibrationOptions2Controller; }
      set
      {
        if (!object.ReferenceEquals(value, _neonCalibrationOptions2Controller))
        {
          _neonCalibrationOptions2Controller?.Dispose();
          _neonCalibrationOptions2Controller = value;
          OnPropertyChanged(nameof(NeonCalibrationOptions2Controller));
        }
      }
    }

    /// <summary>
    /// Gets or sets the controller for neon calibration 2 data.
    /// </summary>
    public IMVCANController NeonCalibrationData2Controller
    {
      get { return _neonCalibrationData2Controller; }
      set
      {
        if (!object.ReferenceEquals(value, _neonCalibrationData2Controller))
        {
          _neonCalibrationData2Controller?.Dispose();
          _neonCalibrationData2Controller = value;
          OnPropertyChanged(nameof(NeonCalibrationData2Controller));
        }
      }
    }

    /// <summary>
    /// Gets or sets the controller for silicon calibration options.
    /// </summary>
    public IMVCANController SiliconCalibrationOptionsController
    {
      get { return _siliconCalibrationOptionsController; }
      set
      {
        if (!object.ReferenceEquals(value, _siliconCalibrationOptionsController))
        {
          _siliconCalibrationOptionsController?.Dispose();
          _siliconCalibrationOptionsController = value;
          OnPropertyChanged(nameof(SiliconCalibrationOptionsController));
        }
      }
    }

    /// <summary>
    /// Gets or sets the controller for silicon calibration data.
    /// </summary>
    public IMVCANController SiliconCalibrationDataController
    {
      get { return _siliconCalibrationDataController; }
      set
      {
        if (!object.ReferenceEquals(value, _siliconCalibrationDataController))
        {
          _siliconCalibrationDataController?.Dispose();
          _siliconCalibrationDataController = value;
          OnPropertyChanged(nameof(SiliconCalibrationDataController));
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
        InputOptionsController = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _doc.ImportOptions }, typeof(IMVCANController), UseDocument.Directly);

        if (_doc.NeonCalibrationOptions1 is not null)
        {
          NeonCalibrationOptions1Controller = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _doc.NeonCalibrationOptions1 }, typeof(IMVCANController), UseDocument.Directly);
          NeonCalibrationData1Controller = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _doc.NeonCalibrationData1 }, typeof(IMVCANController), UseDocument.Directly);
        }
        if (_doc.NeonCalibrationOptions2 is not null)
        {
          NeonCalibrationOptions2Controller = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _doc.NeonCalibrationOptions2 }, typeof(IMVCANController), UseDocument.Directly);
          NeonCalibrationData2Controller = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _doc.NeonCalibrationData2 }, typeof(IMVCANController), UseDocument.Directly);
        }
        if (_doc.SiliconCalibrationOptions is not null)
        {
          SiliconCalibrationOptionsController = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _doc.SiliconCalibrationOptions }, typeof(IMVCANController), UseDocument.Directly);
          SiliconCalibrationDataController = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _doc.SiliconCalibrationData }, typeof(IMVCANController), UseDocument.Directly);
        }

      }
    }

    /// <inheritdoc/>
    public override bool Apply(bool disposeController)
    {

      if (_inputOptionsController.Apply(disposeController))
      {
        _doc.ImportOptions = (IDataSourceImportOptions)_inputOptionsController.ModelObject;
      }
      else
      {
        return ApplyEnd(false, disposeController);
      }

      { // Neon Calibration 1
        // ----------------------------------------------------------------------
        NeonCalibrationOptions? options = null;
        DataTableXYColumnProxy? data = null;

        if (_neonCalibrationOptions1Controller is not null)
        {
          if (_neonCalibrationOptions1Controller.Apply(disposeController))
          {
            options = (NeonCalibrationOptions)_neonCalibrationOptions1Controller.ModelObject;
          }
          else
          {
            return ApplyEnd(false, disposeController);
          }
        }

        if (_neonCalibrationData1Controller is not null)
        {
          if (_neonCalibrationData1Controller.Apply(disposeController))
          {
            data = (DataTableXYColumnProxy)_neonCalibrationData1Controller.ModelObject;
          }
          else
          {
            return ApplyEnd(false, disposeController);
          }
        }

        if (options is not null || data is not null)
        {
          _doc.SetNeonCalibration1(options ?? _doc.NeonCalibrationOptions1, data ?? _doc.NeonCalibrationData1);
        }
      }

      { // Neon Calibration 2
        // ----------------------------------------------------------------------
        NeonCalibrationOptions? options = null;
        DataTableXYColumnProxy? data = null;

        if (_neonCalibrationOptions2Controller is not null)
        {
          if (_neonCalibrationOptions2Controller.Apply(disposeController))
          {
            options = (NeonCalibrationOptions)_neonCalibrationOptions2Controller.ModelObject;
          }
          else
          {
            return ApplyEnd(false, disposeController);
          }
        }

        if (_neonCalibrationData2Controller is not null)
        {
          if (_neonCalibrationData2Controller.Apply(disposeController))
          {
            data = (DataTableXYColumnProxy)_neonCalibrationData2Controller.ModelObject;
          }
          else
          {
            return ApplyEnd(false, disposeController);
          }
        }

        if (options is not null || data is not null)
        {
          _doc.SetNeonCalibration2(options ?? _doc.NeonCalibrationOptions2, data ?? _doc.NeonCalibrationData2);
        }
      }

      { // Silicon Calibration
        // ----------------------------------------------------------------------
        SiliconCalibrationOptions? options = null;
        DataTableXYColumnProxy? data = null;

        if (_siliconCalibrationOptionsController is not null)
        {
          if (_siliconCalibrationOptionsController.Apply(disposeController))
          {
            options = (SiliconCalibrationOptions)_siliconCalibrationOptionsController.ModelObject;
          }
          else
          {
            return ApplyEnd(false, disposeController);
          }
        }

        if (_siliconCalibrationDataController is not null)
        {
          if (_siliconCalibrationDataController.Apply(disposeController))
          {
            data = (DataTableXYColumnProxy)_siliconCalibrationDataController.ModelObject;
          }
          else
          {
            return ApplyEnd(false, disposeController);
          }
        }

        if (options is not null || data is not null)
        {
          _doc.SetSiliconCalibration(options ?? _doc.SiliconCalibrationOptions, data ?? _doc.SiliconCalibrationData);
        }
      }


      SuccessfullyApplied?.Invoke();


      return ApplyEnd(true, disposeController);
    }

    private bool EhCanRemoveNeonCalibration2()
    {
      return _doc is not null && (_doc.NeonCalibrationData2 is not null || _doc.NeonCalibrationOptions2 is not null);
    }

    private void EhRemoveNeonCalibration2()
    {
      if (_doc is not null)
      {
        _doc.ClearNeonCalibration2();
        NeonCalibrationOptions2Controller = null;
        NeonCalibrationData2Controller = null;
      }
    }
  }
}
