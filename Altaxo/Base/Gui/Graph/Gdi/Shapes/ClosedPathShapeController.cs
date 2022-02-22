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
using System;
using System.Collections.Generic;
using System.Text;
using Altaxo.Drawing;
using Altaxo.Graph;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Shapes;
using Altaxo.Gui.Common.Drawing;

namespace Altaxo.Gui.Graph.Gdi.Shapes
{
  public interface IClosedPathShapeView : IDataContextAwareView
  {
   
  }

  [UserControllerForObject(typeof(ClosedPathShapeBase))]
  [ExpectedTypeOfView(typeof(IClosedPathShapeView))]
  public class ClosedPathShapeController : MVCANControllerEditOriginalDocBase<ClosedPathShapeBase, IClosedPathShapeView>
  {
  

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_locationController, () => _locationController = null);
      yield return new ControllerAndSetNullMethod(_brushController, () => _brushController = null);
      yield return new ControllerAndSetNullMethod(_penController, () => _penController = null);
    }

    #region Bindings

    private PenSimpleConditionalController _penController;

    public PenSimpleConditionalController PenController
    {
      get => _penController;
      set
      {
        if (!(_penController == value))
        {
          _penController = value;
          OnPropertyChanged(nameof(PenController));
        }
      }
    }

    private BrushSimpleConditionalController _brushController;

    public BrushSimpleConditionalController BrushController
    {
      get => _brushController;
      set
      {
        if (!(_brushController == value))
        {
          _brushController = value;
          OnPropertyChanged(nameof(BrushController));
        }
      }
    }




    private IMVCANController _locationController;

    public IMVCANController LocationController
    {
      get => _locationController;
      set
      {
        if (!(_locationController == value))
        {
          _locationController = value;
          OnPropertyChanged(nameof(LocationController));
        }
      }
    }


    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _locationController = (IMVCANController)Current.Gui.GetController(new object[] { _doc.Location }, typeof(IMVCANController), UseDocument.Directly);
        Current.Gui.FindAndAttachControlTo(_locationController);

        PenController = new PenSimpleConditionalController(_doc.Pen);
        BrushController = new BrushSimpleConditionalController(_doc.Brush);
      }
    }

    public override bool Apply(bool disposeController)
    {
      if (!_locationController.Apply(disposeController))
        return false;

      _doc.Pen = PenController.Pen;
      _doc.Brush = BrushController.Doc;

      if (!object.ReferenceEquals(_doc.Location, _locationController.ModelObject))
        _doc.Location.CopyFrom((ItemLocationDirect)_locationController.ModelObject);

      return ApplyEnd(true, disposeController);
    }
  }
}
