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
  public class CoordinateSystemController : MVCANDControllerEditOriginalDocBase<G2DCoordinateSystem, ITypeAndInstanceView>, ITypeAndInstanceController
  {
    private IMVCAController _instanceController;


    /// <summary>Holds all instantiable subtypes of G2DCoordinateSystem</summary>
    private Type[] _cosSubTypes;

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_instanceController, () => _instanceController = null);
    }

    #region Bindings

    public string TypeLabel => "Type:";

    private SelectableListNodeList _typeNames = new SelectableListNodeList();

    public SelectableListNodeList TypeNames
    {
      get => _typeNames;
      set
      {
        if (!(_typeNames == value))
        {
          _typeNames = value;
          OnPropertyChanged(nameof(TypeNames));
        }
      }
    }

    private Type _selectedType;

    public Type SelectedType
    {
      get => _selectedType;
      set
      {
        if (!(_selectedType == value))
        {
          _selectedType = value;
          if (value is { } t)
          {
            EhTypeChoiceChanged(t);
          }
          OnPropertyChanged(nameof(SelectedType));

        }
      }
    }

    public object? InstanceView => _instanceController?.ViewObject;


    #endregion

    public override void Dispose(bool isDisposing)
    {
      _typeNames = null;
      _cosSubTypes = null;

      base.Dispose(isDisposing);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        // look for coordinate system types
        if (_cosSubTypes is null)
          _cosSubTypes = ReflectionService.GetNonAbstractSubclassesOf(typeof(G2DCoordinateSystem));

        _typeNames.Clear();
        foreach (Type t in _cosSubTypes)
          _typeNames.Add(new SelectableListNode(Current.Gui.GetUserFriendlyClassName(t), t, t == _doc.GetType()));
        OnPropertyChanged(nameof(TypeNames));
        SelectedType = _doc.GetType();

        CreateInstanceController();
      }
    }

    private void CreateInstanceController()
    {
      _instanceController?.Dispose();
      _instanceController = null;

      // To avoid looping when a dedicated controller is unavailable, we first instantiate the controller alone and compare the types
      _instanceController = (IMVCAController)Current.Gui.GetController(new object[] { _doc }, typeof(IMVCAController), UseDocument.Directly);
      if (_instanceController is not null && (_instanceController.GetType() != GetType()))
      {
        Current.Gui.FindAndAttachControlTo(_instanceController);
      }
      else
      {
        _instanceController = null;
      }
      OnPropertyChanged(nameof(InstanceView));

    }

    public override bool Apply(bool disposeController)
    {
      bool result = _instanceController is null || _instanceController.Apply(disposeController);
      if (result == true && _instanceController is not null)
        _doc = (G2DCoordinateSystem)_instanceController.ModelObject;

      return ApplyEnd(result, disposeController);
    }

    private void EhTypeChoiceChanged(Type t)
    {
      if (_doc.GetType() != t)
      {

        _doc = (G2DCoordinateSystem)Activator.CreateInstance(t);

        OnMadeDirty(); // chance for controller up in hierarchy to catch new instance
       _suspendToken?.Dispose(); // Resume suspend of old document
       _suspendToken = _doc.SuspendGetToken(); // suspend the newly created document
        CreateInstanceController();
      }
    }
  }
}

