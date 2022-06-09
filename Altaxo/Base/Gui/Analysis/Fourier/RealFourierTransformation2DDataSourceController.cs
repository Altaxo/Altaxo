#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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
using System.Text;
using Altaxo.Worksheet.Commands.Analysis;

namespace Altaxo.Gui.Analysis.Fourier
{
  public interface IRealFourierTransformation2DDataSourceView : IDataContextAwareView
  {
  }

  [ExpectedTypeOfView(typeof(IRealFourierTransformation2DDataSourceView))]
  [UserControllerForObject(typeof(FourierTransformation2DDataSource))]
  public class RealFourierTransformation2DDataSourceController : MVCANControllerEditOriginalDocBase<FourierTransformation2DDataSource, IRealFourierTransformation2DDataSourceView>, IMVCSupportsApplyCallback
  {

    public event Action SuccessfullyApplied;

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_dataSourceOptionsController, () => DataSourceOptionsController = null);
      yield return new ControllerAndSetNullMethod(_fourierTransformationOptionsController, () => FourierTransformationOptionsController = null);
      yield return new ControllerAndSetNullMethod(_inputDataController, () => InputDataController = null);
    }

    #region Bindings

    private IMVCANController _dataSourceOptionsController;

    public IMVCANController DataSourceOptionsController
    {
      get => _dataSourceOptionsController;
      set
      {
        if (!(_dataSourceOptionsController == value))
        {
          _dataSourceOptionsController?.Dispose();
          _dataSourceOptionsController = value;
          OnPropertyChanged(nameof(DataSourceOptionsController));
        }
      }
    }

    private IMVCANController _fourierTransformationOptionsController;


    public IMVCANController FourierTransformationOptionsController
    {
      get => _fourierTransformationOptionsController;
      set
      {
        if (!(_fourierTransformationOptionsController == value))
        {
          _fourierTransformationOptionsController?.Dispose();
          _fourierTransformationOptionsController = value;
          OnPropertyChanged(nameof(FourierTransformationOptionsController));
        }
      }
    }

    private IMVCANController _inputDataController;

    public IMVCANController InputDataController
    {
      get => _inputDataController;
      set
      {
        if (!(_inputDataController == value))
        {
          _inputDataController?.Dispose();
          _inputDataController = value;
          OnPropertyChanged(nameof(InputDataController));
        }
      }
    }



    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        DataSourceOptionsController = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _doc.ImportOptions }, typeof(IMVCANController), UseDocument.Directly);
        FourierTransformationOptionsController = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _doc.ProcessOptions }, typeof(IMVCANController), UseDocument.Directly);
        InputDataController = (IMVCANController)Current.Gui.GetControllerAndControl(new object[] { _doc.ProcessData }, typeof(IMVCANController), UseDocument.Directly);
      }
    }

    public override bool Apply(bool disposeController)
    {
      bool result;

      result = _dataSourceOptionsController.Apply(disposeController);
      if (!result)
        return result;

      result = _fourierTransformationOptionsController.Apply(disposeController);
      if (!result)
        return result;

      if (_inputDataController is not null)
      {
        result = _inputDataController.Apply(disposeController);
        if (!result)
          return result;
      }

      SuccessfullyApplied?.Invoke();
      return ApplyEnd(true, disposeController);
    }
  }
}
