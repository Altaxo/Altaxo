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

using Altaxo.Graph;
using Altaxo.Graph.Gdi;
using System;
using System.Collections.Generic;

namespace Altaxo.Gui.Graph.Gdi
{
  #region Interfaces

  public interface ILayerPositionView
  {
    bool UseDirectPositioning { get; set; }

    object SubPositionView { set; }

    event Action PositioningTypeChanged;

    bool IsPositioningTypeChoiceVisible { set; }
  }

  #endregion Interfaces

  /// <summary>
  /// Summary description for LayerPositionController.
  /// </summary>
  [ExpectedTypeOfView(typeof(ILayerPositionView))]
  public class LayerPositionController : MVCANDControllerEditOriginalDocBase<IItemLocation, ILayerPositionView>
  {
    // the document
    private HostLayer _layer;

    private IMVCANController _subController;

    private Dictionary<Type, IItemLocation> _instances;

    protected bool _isRootLayerPosition = false;

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_subController, () => _subController = null);
    }

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
      if (!(args[1] is HostLayer))
        return false;
      _layer = (HostLayer)args[1];

      return base.InitializeDocument(args);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _instances = new Dictionary<Type, IItemLocation>();
        _instances.Add(_doc.GetType(), _doc);

        if (_layer.ParentLayer == null && !(_doc is ItemLocationDirect))
          _doc = new ItemLocationDirect();

        CreateSubController();
      }

      if (null != _view)
      {
        _view.UseDirectPositioning = _doc is ItemLocationDirect;
        _view.SubPositionView = _subController.ViewObject;
        _view.IsPositioningTypeChoiceVisible = !IsRootLayerPosition;
      }
    }

    public override bool Apply(bool disposeController)
    {
      var result = _subController.Apply(disposeController);
      if (result == false)
        return result;

      return ApplyEnd(true, disposeController);
    }

    protected override void AttachView()
    {
      base.AttachView();
      _view.PositioningTypeChanged += EhPositioningTypeChanged;
    }

    protected override void DetachView()
    {
      _view.PositioningTypeChanged -= EhPositioningTypeChanged;
      base.DetachView();
    }

    private void CreateSubController()
    {
      if (_doc is ItemLocationDirect)
      {
        ItemLocationDirectController ctrl;
        _subController = ctrl = new ItemLocationDirectController() { UseDocumentCopy = UseDocument.Directly };
        if (IsRootLayerPosition)
        {
          ctrl.ShowAnchorElements(false, false);
          ctrl.ShowPositionElements(false, false);
        }
        _subController.InitializeDocument(_doc, _layer.ParentLayerSize);
      }
      else if (_doc is ItemLocationByGrid)
      {
        if (null == _layer.ParentLayer)
          throw new InvalidOperationException("This should not be happen; the calling routine must ensure that ItemLocationDirect is used when no parent layer is present");
        _layer.ParentLayer.CreateGridIfNullOrEmpty();
        _subController = new ItemLocationByGridController() { UseDocumentCopy = UseDocument.Directly };
        _subController.InitializeDocument(_doc, _layer.ParentLayer.Grid);
      }
      Current.Gui.FindAndAttachControlTo(_subController);
    }

    private void EhPositioningTypeChanged()
    {
      if (_subController.Apply(false))
        _instances[_subController.ModelObject.GetType()] = (IItemLocation)_subController.ModelObject;

      bool useDirectPositioning = _view.UseDirectPositioning || _layer.ParentLayer == null; // if this is the root layer, then choice of grid positioning is not available

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

        if (null != _suspendToken)
        {
          _suspendToken.Dispose();
          _suspendToken = _doc.SuspendGetToken();
        }

        CreateSubController();

        _view.UseDirectPositioning = useDirectPositioning;
        _view.SubPositionView = _subController.ViewObject;
      }
    }

    public bool IsRootLayerPosition
    {
      get
      {
        return (null != _layer) && (_layer.ParentObject is GraphDocument);
      }
    }
  }
}
