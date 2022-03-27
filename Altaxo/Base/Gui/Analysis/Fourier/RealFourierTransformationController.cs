#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

#nullable disable
using System.Collections.Generic;
using Altaxo.Data;
using Altaxo.Gui.Common.BasicTypes;
using Altaxo.Main;

namespace Altaxo.Gui.Analysis.Fourier
{
  public interface IRealFourierTransformationView : IDataContextAwareView
  {
  }

  [ExpectedTypeOfView(typeof(IRealFourierTransformationView))]
  [UserControllerForObject(typeof(AnalysisRealFourierTransformationCommands.RealFourierTransformOptions))]
  public class RealFourierTransformationController : MVCANControllerEditOriginalDocBase<AnalysisRealFourierTransformationCommands.RealFourierTransformOptions, IRealFourierTransformationView>
  {
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_outputQuantitiesController, () => OutputQuantitiesController = null);
      yield return new ControllerAndSetNullMethod(_outputPlacementController, () => OutputPlacementController = null);
    }

    #region Bindings

    string _columnToTransform;
    public string ColumnToTransform
    {
      get => _columnToTransform;
    }

    public string XIncrement { get; set; }
    public bool XIncrementWarning { get; set; }


    private EnumValueController _outputQuantitiesController;

    public EnumValueController OutputQuantitiesController
    {
      get => _outputQuantitiesController;
      set
      {
        if (!(_outputQuantitiesController == value))
        {
          _outputQuantitiesController?.Dispose();
         _outputQuantitiesController = value;
          OnPropertyChanged(nameof(OutputQuantitiesController));
        }
      }
    }
    private EnumValueController  _outputPlacementController;

    public EnumValueController  OutputPlacementController
    {
      get => _outputPlacementController;
      set
      {
        if (!(_outputPlacementController == value))
        {
          _outputPlacementController?.Dispose();
          _outputPlacementController = value;
          OnPropertyChanged(nameof(OutputPlacementController));
        }
      }
    }


    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        OutputQuantitiesController = new EnumValueController(_doc.Output);
        OutputPlacementController = new EnumValueController(_doc.OutputPlacement);


        _columnToTransform = AbsoluteDocumentPath.GetPathString(_doc.ColumnToTransform, int.MaxValue);

        string xInc = _doc.XIncrementValue.ToString();
        if (_doc.XIncrementMessage is not null)
          xInc += string.Format(" ({0})", _doc.XIncrementMessage);
        XIncrement = xInc;
        XIncrementWarning = _doc.XIncrementMessage is not null;
      }
    }

    public override bool Apply(bool disposeController)
    {
      if (false == _outputQuantitiesController.Apply(disposeController))
        return ApplyEnd(false, disposeController);
      if (false == _outputPlacementController.Apply(disposeController))
        return ApplyEnd(false, disposeController);

      _doc.Output = (AnalysisRealFourierTransformationCommands.RealFourierTransformOutput)_outputQuantitiesController.ModelObject;
      _doc.OutputPlacement = (AnalysisRealFourierTransformationCommands.RealFourierTransformOutputPlacement)_outputPlacementController.ModelObject;
      return ApplyEnd(true, disposeController);
    }
  }
}
