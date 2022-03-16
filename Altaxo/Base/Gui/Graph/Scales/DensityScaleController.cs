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
using Altaxo.Collections;
using Altaxo.Graph.Scales;
using Altaxo.Gui.Common;

namespace Altaxo.Gui.Graph.Scales
{
  public interface IDensityScaleView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Lets the user choose a numerical scale.
  /// </summary>
  [ExpectedTypeOfView(typeof(IDensityScaleView))]
  // [UserControllerForObject(typeof(NumericalScale),101)] // outcommented since this causes an infinite loop when searching for detailed scale controllers
  public class DensityScaleController : MVCANDControllerEditOriginalDocInstanceCanChangeBase<Scale, IDensityScaleView>
  {
    public override System.Collections.Generic.IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_rescalingController, () => _rescalingController = null);
      yield return new ControllerAndSetNullMethod(_scaleController, () => _scaleController = null);
    }

    #region Bindings

    private ItemsController<Type> _scaleTypes;

    public ItemsController<Type> ScaleTypes
    {
      get => _scaleTypes;
      set
      {
        if (!(_scaleTypes == value))
        {
          _scaleTypes = value;
          OnPropertyChanged(nameof(ScaleTypes));
        }
      }
    }


    private IMVCAController _scaleController;

    public IMVCAController ScaleController
    {
      get => _scaleController;
      set
      {
        if (!(_scaleController == value))
        {
          _scaleController = value;
          OnPropertyChanged(nameof(ScaleController));
        }
      }
    }

    private IMVCAController _rescalingController;

    public IMVCAController RescalingController
    {
      get => _rescalingController;
      set
      {
        if (!(_rescalingController == value))
        {
          _rescalingController = value;
          OnPropertyChanged(nameof(RescalingController));
        }
      }
    }

    #endregion

    public override void Dispose(bool isDisposing)
    {
      _scaleTypes?.Dispose();
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

      if (initData)
      {
        // InitScaleTypes
        var scaleTypes = new SelectableListNodeList();
        Type[] classes = Altaxo.Main.Services.ReflectionService.GetNonAbstractSubclassesOf(typeof(Scale));
        for (int i = 0; i < classes.Length; i++)
        {
          if (classes[i] == typeof(LinkedScale))
            continue;
          var node = new SelectableListNode(Current.Gui.GetUserFriendlyClassName(classes[i]), classes[i], _doc.GetType() == classes[i]);
          scaleTypes.Add(node);
        }
        _scaleTypes = new ItemsController<Type>(scaleTypes, EhView_ScaleTypeChanged);

        InitScaleController();
        InitRescalingController();
      }
    }

    public override bool Apply(bool disposeController)
    {
      if (_scaleController is not null)
      {
        if (false == _scaleController.Apply(disposeController))
          return false;
      }

      if (_rescalingController is not null)
      {
        if (false == _rescalingController.Apply(disposeController))
          return false;
      }

      return ApplyEnd(true, disposeController);
    }

    protected void InitScaleController()
    {
      ScaleController = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { _doc }, typeof(IMVCAController), UseDocument.Directly);
    }

    public void InitRescalingController()
    {
       if(_doc.RescalingObject is { } rescalingObject)
        if (rescalingObject is not null)
          RescalingController = (IMVCAController)Current.Gui.GetControllerAndControl(new object[] { rescalingObject, _doc }, typeof(IMVCAController), UseDocument.Directly);
        else
          RescalingController = null;
    }

    #region View event handlers

    public void EhView_ScaleTypeChanged(Type axistype)
    {
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

          if (_suspendToken is not null)
          {
            _suspendToken.Dispose();
            _suspendToken = _doc.SuspendGetToken();
          }

          InitScaleController();
          // now we have also to replace the controller and the control for the axis boundaries
          InitRescalingController();
        }
      }
      catch (Exception)
      {
      }
    }

    #endregion View event handlers
  }
}
