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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Main;

namespace Altaxo.Gui.Analysis.Fourier
{
  public interface IRealFourierTransformationView
  {
    void SetColumnToTransform(string val);

    void SetXIncrement(string val, bool bMarkAsWarning);

    void SetOutputQuantities(SelectableListNodeList list);

    void SetCreationOptions(SelectableListNodeList list);
  }

  [ExpectedTypeOfView(typeof(IRealFourierTransformationView))]
  [UserControllerForObject(typeof(AnalysisRealFourierTransformationCommands.RealFourierTransformOptions))]
  public class RealFourierTransformationController : MVCANControllerEditOriginalDocBase<AnalysisRealFourierTransformationCommands.RealFourierTransformOptions, IRealFourierTransformationView>
  {
    private SelectableListNodeList _outputQuantities;
    private SelectableListNodeList _creationOptions;

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    public override void Dispose(bool isDisposing)
    {
      _outputQuantities = null;
      _creationOptions = null;

      base.Dispose(isDisposing);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _outputQuantities = new SelectableListNodeList();
        _creationOptions = new SelectableListNodeList();
      }

      if (_view != null)
      {
        var yColName = AbsoluteDocumentPath.GetPathString(_doc.ColumnToTransform, int.MaxValue);
        _view.SetColumnToTransform(yColName);

        string xInc = _doc.XIncrementValue.ToString();
        if (_doc.XIncrementMessage != null)
          xInc += string.Format(" ({0})", _doc.XIncrementMessage);
        _view.SetXIncrement(xInc, _doc.XIncrementMessage != null);

        _outputQuantities.FillWithFlagEnumeration(_doc.Output);
        _view.SetOutputQuantities(_outputQuantities);

        _creationOptions.FillWithEnumeration(_doc.OutputPlacement);
        _view.SetCreationOptions(_creationOptions);
      }
    }

    public override bool Apply(bool disposeController)
    {
      _doc.Output = (AnalysisRealFourierTransformationCommands.RealFourierTransformOutput)_outputQuantities.GetFlagEnumValueAsInt32();
      _doc.OutputPlacement = (AnalysisRealFourierTransformationCommands.RealFourierTransformOutputPlacement)_creationOptions.FirstSelectedNode.Tag;
      return ApplyEnd(true, disposeController);
    }
  }
}
