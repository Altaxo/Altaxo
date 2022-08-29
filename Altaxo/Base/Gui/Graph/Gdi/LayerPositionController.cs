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
using Altaxo.Graph.Gdi;

namespace Altaxo.Gui.Graph.Gdi
{
  public interface ILayerPositionView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Summary description for LayerPositionController.
  /// </summary>
  [ExpectedTypeOfView(typeof(ILayerPositionView))]
  public class LayerPositionController : MVCANDControllerEditOriginalDocBase<IItemLocation, ILayerPositionView>
  {
    // the document
    private HostLayer _layer;

    private Dictionary<Type, IItemLocation> _instances;

    protected bool _isRootLayerPosition = false;

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_subController, () => _subController = null);
    }

    #region Bindings

    private bool _UseDirectPositioning;

    public bool UseDirectPositioning
    {
      get => _UseDirectPositioning;
      set
      {
        if (!(_UseDirectPositioning == value))
        {
          _UseDirectPositioning = value;
          EhPositioningTypeChanged();
          OnPropertyChanged(nameof(UseDirectPositioning));
          OnPropertyChanged(nameof(UseGridPositioning));
        }
      }
    }

    public bool UseGridPositioning
    {
      get => !UseDirectPositioning;
      set => UseDirectPositioning = !value;
    }


    public bool IsPositioningChoiceVisible
    {
      get => !IsRootLayerPosition;
    }

    public bool IsRootLayerPosition
    {
      get
      {
        return (_layer is not null) && (_layer.ParentObject is GraphDocument);
      }
    }

    private IMVCANController _subController;

    public IMVCANController SubController
    {
      get => _subController;
      set
      {
        if (!(_subController == value))
        {
          _subController = value;
          OnPropertyChanged(nameof(SubController));
        }
      }
    }



    #endregion

    public override void Dispose(bool isDisposing)
    {
      _layer = null;
      _instances = null;

      base.Dispose(isDisposing);
    }

    public override bool InitializeDocument(params object[] args)
    {
      if (args.Length < 2)
        return false;
      if (args[1] is not HostLayer hl)
        return false;
      _layer = hl;

      return base.InitializeDocument(args);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _instances = new Dictionary<Type, IItemLocation>
        {
          { _doc.GetType(), _doc }
        };

        if (_layer.ParentLayer is null && !(_doc is ItemLocationDirect))
          _doc = new ItemLocationDirect();

        UseDirectPositioning = _doc is ItemLocationDirect;
        CreateSubController();
      }
    }

    public override bool Apply(bool disposeController)
    {
      if(false==_subController.Apply(disposeController))
        return ApplyEnd(false, disposeController);

      return ApplyEnd(true, disposeController);
    }

    private void CreateSubController()
    {
      if (_doc is ItemLocationDirect)
      {
        ItemLocationDirectController ctrl;
        var subController = ctrl = new ItemLocationDirectController() { UseDocumentCopy = UseDocument.Directly };
        if (IsRootLayerPosition)
        {
          ctrl.ShowAnchorElements(false, false);
          ctrl.ShowPositionElements(false, false);
        }
        subController.InitializeDocument(_doc, _layer.ParentLayerSize);
        SubController = subController;
      }
      else if (_doc is ItemLocationByGrid)
      {
        if (_layer.ParentLayer is null)
          throw new InvalidOperationException("This should not be happen; the calling routine must ensure that ItemLocationDirect is used when no parent layer is present");
        _layer.ParentLayer.CreateGridIfNullOrEmpty();
        var subController = new ItemLocationByGridController() { UseDocumentCopy = UseDocument.Directly };
        subController.InitializeDocument(_doc, _layer.ParentLayer.Grid);
        SubController = subController;
      }
      Current.Gui.FindAndAttachControlTo(_subController);
    }

    private void EhPositioningTypeChanged()
    {
      if (_subController is not null && _subController.Apply(false))
      {
        _instances[_subController.ModelObject.GetType()] = (IItemLocation)_subController.ModelObject;
      }

      bool useDirectPositioning = UseDirectPositioning || _layer.ParentLayer is null; // if this is the root layer, then choice of grid positioning is not available

      IItemLocation oldDoc = _doc;
      IItemLocation newDoc = null;

      if (useDirectPositioning)
      {
        if (_instances.ContainsKey(typeof(ItemLocationDirect)))
          newDoc = _instances[typeof(ItemLocationDirect)];
        else
          newDoc = new ItemLocationDirect();
      }
      else
      {
        if (_instances.ContainsKey(typeof(ItemLocationByGrid)))
          newDoc = _instances[typeof(ItemLocationByGrid)];
        else
          newDoc = new ItemLocationByGrid();
      }

      if (!object.ReferenceEquals(oldDoc, newDoc))
      {
        _doc = newDoc;
        OnMadeDirty(); // change for super-controller to pick up new instance

        if (_suspendToken is not null)
        {
          _suspendToken.Dispose();
          _suspendToken = _doc.SuspendGetToken();
        }

        CreateSubController();

        UseDirectPositioning = useDirectPositioning;
      }
    }
  }
}
