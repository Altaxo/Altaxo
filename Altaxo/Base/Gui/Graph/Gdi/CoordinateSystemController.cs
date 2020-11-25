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
using Altaxo.Collections;
using Altaxo.Graph.Gdi;
using Altaxo.Gui.Common;
using Altaxo.Main.Services;

namespace Altaxo.Gui.Graph.Gdi
{
  [UserControllerForObject(typeof(G2DCoordinateSystem))]
  [ExpectedTypeOfView(typeof(ITypeAndInstanceView))]
  public class CoordinateSystemController : MVCANDControllerEditOriginalDocBase<G2DCoordinateSystem, ITypeAndInstanceView>
  {
    private IMVCAController _instanceController;

    private SelectableListNodeList _choiceList;

    /// <summary>Holds all instantiable subtypes of G2DCoordinateSystem</summary>
    private Type[] _cosSubTypes;

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_instanceController, () => _instanceController = null);
    }

    public override void Dispose(bool isDisposing)
    {
      _choiceList = null;
      _cosSubTypes = null;

      base.Dispose(isDisposing);
    }

    #region IMVCController Members

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        // look for coordinate system types
        if (_cosSubTypes is null)
          _cosSubTypes = ReflectionService.GetNonAbstractSubclassesOf(typeof(G2DCoordinateSystem));

        if (_choiceList is null)
          _choiceList = new SelectableListNodeList();
        _choiceList.Clear();
        foreach (Type t in _cosSubTypes)
          _choiceList.Add(new SelectableListNode(Current.Gui.GetUserFriendlyClassName(t), t, t == _doc.GetType()));
      }

      if (_view is not null)
      {
        // look for a controller-control
        _view.TypeLabel = "Type:";
        _view.InitializeTypeNames(_choiceList);

        // To avoid looping when a dedicated controller is unavailable, we first instantiate the controller alone and compare the types
        _instanceController = (IMVCAController)Current.Gui.GetController(new object[] { _doc }, typeof(IMVCAController), UseDocument.Directly);
        if (_instanceController is not null && (_instanceController.GetType() != GetType()))
        {
          Current.Gui.FindAndAttachControlTo(_instanceController);
          if (_instanceController.ViewObject is not null)
            _view.SetInstanceControl(_instanceController.ViewObject);
        }
        else
        {
          _instanceController = null;
          _view.SetInstanceControl(null);
        }
      }
    }

    public override bool Apply(bool disposeController)
    {
      bool result = _instanceController is null || _instanceController.Apply(disposeController);
      return ApplyEnd(result, disposeController);
    }

    protected override void AttachView()
    {
      base.AttachView();
      _view.TypeChoiceChanged += EhTypeChoiceChanged;
    }

    protected override void DetachView()
    {
      _view.TypeChoiceChanged -= EhTypeChoiceChanged;
      base.DetachView();
    }

    private void EhTypeChoiceChanged(object sender, EventArgs e)
    {
      var sel = _choiceList.FirstSelectedNode;

      if (sel is not null)
      {
        var t = (System.Type)sel.Tag;
        if (_doc.GetType() != t)
        {
          _doc = (G2DCoordinateSystem)Activator.CreateInstance((System.Type)sel.Tag);

          OnMadeDirty(); // chance for controller up in hierarchy to catch new instance

          if (_suspendToken is not null)
          {
            _suspendToken.Dispose();
            _suspendToken = _doc.SuspendGetToken();
          }

          Initialize(true);
        }
      }
    }

    #endregion IMVCController Members
  }
}
