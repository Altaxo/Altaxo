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
//    along with ctrl program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

#nullable disable
using System.Collections.Generic;
using Altaxo.Collections;
using Altaxo.Drawing;
using Altaxo.Gui.Common;

namespace Altaxo.Gui.Drawing.ColorManagement
{
  public interface INamedColorView : IDataContextAwareView
  {
  }

  [ExpectedTypeOfView(typeof(INamedColorView))]
  public class NamedColorController : MVCANControllerEditImmutableDocBase<NamedColor, INamedColorView>
  {
    private ColorModelController _subControllerColorModel;

    private ColorCircleController _subControllerColorCircle;

    private ColorPickerController _subControllerColorPicker;

    private NamedColor _initialColor;

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_subControllerColorModel, () => _subControllerColorModel = null);
      yield return new ControllerAndSetNullMethod(_subControllerColorCircle, () => _subControllerColorCircle = null);
      yield return new ControllerAndSetNullMethod(_subControllerColorPicker, () => _subControllerColorPicker = null);
    }

    #region Bindings

    private AxoColor _oldColor;

    public AxoColor OldColor
    {
      get => _oldColor;
      set
      {
        if (!(_oldColor == value))
        {
          _oldColor = value;
          OnPropertyChanged(nameof(OldColor));
        }
      }
    }
    private AxoColor _newColor;

    public AxoColor NewColor
    {
      get => _newColor;
      set
      {
        if (!(_newColor == value))
        {
          _newColor = value;
          OnPropertyChanged(nameof(NewColor));
        }
      }
    }
    private string _colorName;

    public string ColorName
    {
      get => _colorName;
      set
      {
        if (!(_colorName == value))
        {
          _colorName = value;
          OnPropertyChanged(nameof(ColorName));
        }
      }
    }

    private ItemsController<IMVCAController> _subController;

    public ItemsController<IMVCAController> SubController
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
    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _initialColor = _doc;

        _subControllerColorModel = new ColorModelController();
        _subControllerColorModel.InitializeDocument(_doc.Color);
        _subControllerColorModel.MadeDirty += EhController_Dirty;

        _subControllerColorCircle = new ColorCircleController();
        _subControllerColorCircle.InitializeDocument(_doc.Color);
        _subControllerColorCircle.MadeDirty += EhController_Dirty;

        _subControllerColorPicker = new ColorPickerController();
        _subControllerColorPicker.InitializeDocument(_doc.Color);
        _subControllerColorPicker.MadeDirty += EhController_Dirty;

        var controllers = new SelectableListNodeList();
        foreach (var item in GetTabNamesAndControllers())
        {
          controllers.Add(new SelectableListNodeWithController(item.TabName, item.Controller, false) { Controller = item.Controller });
        }

        SubController = new ItemsController<IMVCAController>(controllers, EhSubViewChanged);

        OldColor = _initialColor;
        NewColor = _doc;
        ColorName = _doc.Name;
      }
    }



    private IEnumerable<(string TabName, IMVCAController Controller)> GetTabNamesAndControllers()
    {
      if (_subControllerColorModel is not null)
      {
        if (_subControllerColorModel.ViewObject is null)
          Current.Gui.FindAndAttachControlTo(_subControllerColorModel);
        if (_subControllerColorModel.ViewObject is not null)
          yield return ("Models", _subControllerColorModel);
      }

      if (_subControllerColorCircle is not null)
      {
        if (_subControllerColorCircle.ViewObject is null)
          Current.Gui.FindAndAttachControlTo(_subControllerColorCircle);
        if (_subControllerColorCircle.ViewObject is not null)
          yield return ("Circle", _subControllerColorCircle);
      }

      if (_subControllerColorPicker is not null)
      {
        if (_subControllerColorPicker.ViewObject is null)
          Current.Gui.FindAndAttachControlTo(_subControllerColorPicker);
        if (_subControllerColorPicker.ViewObject is not null)
          yield return ("Picker", _subControllerColorPicker);
      }
    }

    public override bool Apply(bool disposeController)
    {
      var userColorName = ColorName;

      if (!string.IsNullOrEmpty(userColorName) && userColorName != NamedColor.GetColorName(_doc.Color))
      {
        _doc = new NamedColor(_doc.Color, userColorName);

        // TODO if the user has provided a color name, then store this color in a list of colors
      }

      return ApplyEnd(true, disposeController);
    }



    private void EhSubViewChanged(IMVCAController obj)
    {
      // Find controller correspondig to subview
      foreach (var ctrl in new IMVCANDController[] { _subControllerColorModel, _subControllerColorCircle, _subControllerColorPicker })
      {
        if (object.ReferenceEquals(obj, ctrl.ViewObject))
        {
          ctrl.InitializeDocument(_doc.Color);
        }
      }
    }

    private void EhController_Dirty(IMVCANDController ctrl)
    {
      var color = (AxoColor)ctrl.ModelObject;
      color.IsFromArgb = true;
      _doc = new NamedColor(color);
      NewColor = color;
      ColorName = _doc.Name;
    }
  }
}
