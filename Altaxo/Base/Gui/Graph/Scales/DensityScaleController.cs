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
using Altaxo.Collections;
using Altaxo.Graph.Scales;

namespace Altaxo.Gui.Graph.Scales
{
  #region Interfaces

  public interface IDensityScaleView
  {
    void InitializeAxisType(SelectableListNodeList names);

    void SetRescalingView(object guiobject);

    void SetScaleView(object guiobject);

    event Action AxisTypeChanged;
  }

  #endregion Interfaces

  /// <summary>
  /// Lets the user choose a numerical scale.
  /// </summary>
  [ExpectedTypeOfView(typeof(IDensityScaleView))]
  // [UserControllerForObject(typeof(NumericalScale),101)] // outcommented since this causes an infinite loop when searching for detailed scale controllers
  public class DensityScaleController : MVCANDControllerEditOriginalDocInstanceCanChangeBase<Scale, IDensityScaleView>
  {
    protected IMVCAController _rescalingController;

    protected IMVCAController _scaleController;

    protected SelectableListNodeList _scaleTypes;

    public override System.Collections.Generic.IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_rescalingController, () => _rescalingController = null);
      yield return new ControllerAndSetNullMethod(_scaleController, () => _scaleController = null);
    }

    public override void Dispose(bool isDisposing)
    {
      _scaleTypes = null;

      base.Dispose(isDisposing);
    }

    public DensityScaleController(Action<Scale> SetInstanceInParentNode)
      : base(SetInstanceInParentNode)
    {
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      InitScaleTypes(initData);
      InitScaleController(initData);
      InitRescalingController(initData);
    }

    public override bool Apply(bool disposeController)
    {
      if (null != _scaleController)
      {
        if (false == _scaleController.Apply(disposeController))
          return false;
      }

      if (null != _rescalingController)
      {
        if (false == _rescalingController.Apply(disposeController))
          return false;
      }

      return ApplyEnd(true, disposeController);
    }

    protected override void AttachView()
    {
      base.AttachView();
      _view.AxisTypeChanged += EhView_AxisTypeChanged;
    }

    protected override void DetachView()
    {
      _view.AxisTypeChanged -= EhView_AxisTypeChanged;
      base.DetachView();
    }

    public void InitScaleTypes(bool bInit)
    {
      if (bInit)
      {
        _scaleTypes = new SelectableListNodeList();
        Type[] classes = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(NumericalScale));
        for (int i = 0; i < classes.Length; i++)
        {
          if (classes[i] == typeof(LinkedScale))
            continue;
          var node = new SelectableListNode(Current.Gui.GetUserFriendlyClassName(classes[i]), classes[i], _doc.GetType() == classes[i]);
          _scaleTypes.Add(node);
        }
      }

      if (null != _view)
        _view.InitializeAxisType(_scaleTypes);
    }

    public void InitScaleController(bool bInit)
    {
      if (bInit)
      {
        object scaleObject = _doc;
        _scaleController = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { scaleObject }, typeof(IMVCAController), UseDocument.Directly);
      }
      if (null != _view)
      {
        _view.SetScaleView(null == _scaleController ? null : _scaleController.ViewObject);
      }
    }

    public void InitRescalingController(bool bInit)
    {
      if (bInit)
      {
        object rescalingObject = _doc.RescalingObject;
        if (rescalingObject != null)
          _rescalingController = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { rescalingObject, _doc }, typeof(IMVCAController), UseDocument.Directly);
        else
          _rescalingController = null;
      }
      if (null != _view)
      {
        _view.SetRescalingView(null != _rescalingController ? _rescalingController.ViewObject : null);
      }
    }

    #region View event handlers

    public void EhView_AxisTypeChanged()
    {
      var axistype = (Type)_scaleTypes.FirstSelectedNode.Tag;

      try
      {
        if (axistype != _doc.GetType())
        {
          // replace the current axis by a new axis of the type axistype
          Scale oldScale = _doc;
          var newScale = (Scale)System.Activator.CreateInstance(axistype);

          // Try to set the same org and end as the axis before
          // this will fail for instance if we switch from linear to logarithmic with negative bounds
          try
          {
            if (newScale.RescalingObject is Altaxo.Main.ICopyFrom)
              newScale.RescalingObject.CopyFrom(oldScale.RescalingObject);
          }
          catch (Exception)
          {
          }

          _doc = newScale;

          OnDocumentInstanceChanged(oldScale, newScale);
          OnMadeDirty(); // chance for controllers up in hierarchy to catch new instance

          if (null != _suspendToken)
          {
            _suspendToken.Dispose();
            _suspendToken = _doc.SuspendGetToken();
          }

          InitScaleController(true);
          // now we have also to replace the controller and the control for the axis boundaries
          InitRescalingController(true);
        }
      }
      catch (Exception)
      {
      }
    }

    #endregion View event handlers
  }
}
