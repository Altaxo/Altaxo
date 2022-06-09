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
using Altaxo.Data;

namespace Altaxo.Gui.Data
{
  [ExpectedTypeOfView(typeof(ICommonDataSourceViewN))]
  public class DataSourceControllerBase<TItem> : MVCANControllerEditOriginalDocBase<TItem, ICommonDataSourceViewN> where TItem : IAltaxoTableDataSource
  {
    private IMVCANController _inputOptionsController;
    private IMVCANController _processOptionsController;
    private IMVCANController _processDataController;

    public event Action SuccessfullyApplied;

    public const string TabInputOptions = "InputOptions";
    public const string TabProcessOptions = "ProcessOptions";
    public const string TabProcessData = "ProcessData";

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_inputOptionsController, () => _inputOptionsController = null);
      yield return new ControllerAndSetNullMethod(_processOptionsController, () => _processOptionsController = null);
      yield return new ControllerAndSetNullMethod(_processDataController, () => _processDataController = null);
    }

    #region Bindings


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

    public IMVCANController ProcessOptionsController
    {
      get { return _processOptionsController; }
      set
      {
        if (!object.ReferenceEquals(value, _processOptionsController))
        {
          _processOptionsController?.Dispose();
          _processOptionsController = value;
          OnPropertyChanged(nameof(ProcessOptionsController));
        }
      }
    }

    public IMVCANController ProcessDataController
    {
      get { return _processDataController; }
      set
      {
        if (!object.ReferenceEquals(value, _processDataController))
        {
          _processDataController?.Dispose();
          _processDataController = value;
          OnPropertyChanged(nameof(ProcessDataController));
        }
      }
    }




    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        InputOptionsController = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _doc.ImportOptions }, typeof(IMVCANController), UseDocument.Directly);
        ProcessOptionsController = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _doc.ProcessOptionsObject }, typeof(IMVCANController), UseDocument.Directly);
        ProcessDataController = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _doc.ProcessDataObject }, typeof(IMVCANController), UseDocument.Directly);
      }
    }

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


      if (_processOptionsController.Apply(disposeController))
      {
        _doc.ProcessOptionsObject = _processOptionsController.ModelObject;
      }
      else
      {
        return ApplyEnd(false, disposeController);
      }

      if (_processDataController is not null)
      {
        if (_processDataController.Apply(disposeController))
        {
          _doc.ProcessDataObject = _processDataController.ModelObject;
        }
        else
        {
          return ApplyEnd(false, disposeController);
        }
      }


      SuccessfullyApplied?.Invoke();


      return ApplyEnd(true, disposeController);
    }
  }

  [UserControllerForObject(typeof(SpectralPreprocessingDataSource))]
  public class SpectralPreprocessingDataSourceController : DataSourceControllerBase<SpectralPreprocessingDataSource>
  {
  }
}
