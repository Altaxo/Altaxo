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
  public interface IOpenPathShapeView : IDataContextAwareView
  {
  }

  [UserControllerForObject(typeof(OpenPathShapeBase), 101)]
  [ExpectedTypeOfView(typeof(IOpenPathShapeView))]
  public class OpenPathShapeController : MVCANControllerEditOriginalDocBase<OpenPathShapeBase, IOpenPathShapeView>
  {
    private IMVCANController _locationController;
    private PenAllPropertiesController _penController;

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_locationController, () => _locationController = null);
      yield return new ControllerAndSetNullMethod(_penController, () => _penController = null);
    }

    #region Bindings
    public object PenController => _penController;

    public object LocationView => _locationController?.ViewObject;

    #endregion

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _locationController = (IMVCANController)Current.Gui.GetController(new object[] { _doc.Location }, typeof(IMVCANController), UseDocument.Directly);
        Current.Gui.FindAndAttachControlTo(_locationController);
        _penController = new PenAllPropertiesController(_doc.Pen);
      }
    }

    #region IApplyController Members

    public override bool Apply(bool disposeController)
    {
      try
      {
        if (!_locationController.Apply(disposeController))
          return false;

        if (!_penController.Apply(disposeController))
          return false;

        if (!object.ReferenceEquals(_doc.Location, _locationController.ModelObject))
          _doc.Location.CopyFrom((ItemLocationDirect)_locationController.ModelObject);

        if (!object.ReferenceEquals(_doc.Pen, _penController.ModelObject))
          _doc.Pen = (PenX)_penController.ModelObject;
      }
      catch (Exception ex)
      {
        Current.Gui.ErrorMessageBox(string.Format("An exception has occured during applying of your settings. The message is: {0}", ex.Message));
        return false;
      }

      return ApplyEnd(true, disposeController);
    }

    #endregion IApplyController Members
  }
}
