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

#nullable disable
using System;
using System.Collections.Generic;
using Altaxo.Graph.Graph3D;
using Altaxo.Graph.Graph3D.Shapes;
using Altaxo.Gui.Drawing.D3D;

namespace Altaxo.Gui.Graph.Graph3D.Shapes
{
  /// <summary>
  /// Provides the view contract for <see cref="OpenPathShapeController"/>.
  /// </summary>
  public interface IOpenPathShapeView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Controller for <see cref="OpenPathShapeBase"/>.
  /// </summary>
  [UserControllerForObject(typeof(OpenPathShapeBase), 101)]
  [ExpectedTypeOfView(typeof(IOpenPathShapeView))]
  public class OpenPathShapeController : MVCANControllerEditOriginalDocBase<OpenPathShapeBase, IOpenPathShapeView>
  {

    /// <inheritdoc />
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_penController, () => PenController = null);
      yield return new ControllerAndSetNullMethod(_locationController, () => LocationController = null);
    }

    #region Bindings

    private PenAllPropertiesController _penController;

    /// <summary>
    /// Gets or sets the pen controller.
    /// </summary>
    public PenAllPropertiesController PenController
    {
      get => _penController;
      set
      {
        if (!(_penController == value))
        {
          _penController?.Dispose();
          _penController = value;
          OnPropertyChanged(nameof(PenController));
        }
      }
    }

    private IMVCANController _locationController;

    /// <summary>
    /// Gets or sets the location controller.
    /// </summary>
    public IMVCANController LocationController
    {
      get => _locationController;
      set
      {
        if (!(_locationController == value))
        {
          _locationController?.Dispose();
          _locationController = value;
          OnPropertyChanged(nameof(LocationController));
        }
      }
    }




    #endregion

    /// <inheritdoc />
    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        var locationController = (IMVCANController)Current.Gui.GetController(new object[] { _doc.Location }, typeof(IMVCANController), UseDocument.Directly);
        Current.Gui.FindAndAttachControlTo(locationController);
        LocationController = locationController;

        PenController = new PenAllPropertiesController(_doc.Pen);
      }
    }

    #region IApplyController Members

    /// <inheritdoc />
    public override bool Apply(bool disposeController)
    {
      try
      {
        if (!_locationController.Apply(disposeController))
          return false;

        if (!object.ReferenceEquals(_doc.Location, _locationController.ModelObject))
          _doc.Location.CopyFrom((ItemLocationDirect)_locationController.ModelObject);

        _doc.Pen = PenController.Pen;
      }
      catch (Exception ex)
      {
        Current.Gui.ErrorMessageBox(string.Format("An exception has occurred while applying your settings. The message is: {0}", ex.Message));
        return false;
      }

      return ApplyEnd(true, disposeController);
    }

    #endregion IApplyController Members
  }
}
