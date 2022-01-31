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
using Altaxo.Collections;
using Altaxo.Graph.Scales.Ticks;
using Altaxo.Gui.Common;

namespace Altaxo.Gui.Graph.Scales.Ticks
{
  public interface ITickSpacingView : IDataContextAwareView
  {
  }

  [ExpectedTypeOfView(typeof(ITickSpacingView))]
  [UserControllerForObject(typeof(TickSpacing))]
  public class TickSpacingController : MVCANDControllerEditOriginalDocBase<TickSpacing, ITickSpacingView>
  {
    ItemsController<Type> _tickSpacingTypes;

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_tickSpacingDetailsController, () => _tickSpacingDetailsController = null);
    }

    #region Bindings

    public ItemsController<Type> TickSpacingTypes => _tickSpacingTypes;

    private IMVCAController _tickSpacingDetailsController;

    public IMVCAController TickSpacingDetailsController
    {
      get => _tickSpacingDetailsController;
      set
      {
        if (!(_tickSpacingDetailsController == value))
        {
          _tickSpacingDetailsController = value;
          OnPropertyChanged(nameof(TickSpacingDetailsController));
        }
      }
    }


    #endregion

    public override void Dispose(bool isDisposing)
    {
      _tickSpacingTypes = null;
      base.Dispose(isDisposing);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        var tickSpacingTypes = new SelectableListNodeList();
        Type[] classes = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(TickSpacing));
        for (int i = 0; i < classes.Length; i++)
        {
          var node = new SelectableListNode(Current.Gui.GetUserFriendlyClassName(classes[i]), classes[i], _doc.GetType() == classes[i]);
          tickSpacingTypes.Add(node);
        }
        _tickSpacingTypes = new ItemsController<Type>(tickSpacingTypes, EhView_TickSpacingTypeChanged);

        if (_doc is not null)
          _tickSpacingDetailsController = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { _doc }, typeof(IMVCAController), UseDocument.Directly);
        else
          _tickSpacingDetailsController = null;

      }
    }

    public override bool Apply(bool disposeController)
    {
      if (_tickSpacingDetailsController is not null && false == _tickSpacingDetailsController.Apply(disposeController))
        return false;

      return ApplyEnd(true, disposeController);
    }

    public void EhView_TickSpacingTypeChanged(Type spaceType)
    {
      if (spaceType is null)
        return;
      if (spaceType == _doc.GetType())
        return;

      _doc = (TickSpacing)Activator.CreateInstance(spaceType);

      OnMadeDirty(); // this is the chance for the controller above in hierarchy to test for a new document instance and use it

      if (_suspendToken is not null)
      {
        _suspendToken.Dispose();
        _suspendToken = _doc.SuspendGetToken(); // now we suspend the new document
      }

      if (_doc is not null)
        TickSpacingDetailsController = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { _doc }, typeof(IMVCAController), UseDocument.Directly);
      else
        TickSpacingDetailsController = null;
    }
  }
}
