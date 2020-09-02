#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2017 Dr. Dirk Lellinger
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Drawing;

namespace Altaxo.Gui.Drawing.ColorManagement
{
  public interface INamedColorView
  {
    void InitializeSubViews(IEnumerable<Tuple<string, object>> tabsNamesAndViews);

    void SetOldColor(AxoColor oldColor);

    void SetNewColor(AxoColor oldColor);

    string ColorName { get; set; }

    /// <summary>
    /// Occurs when the sub view has changed. Parameter is the content (view) of the subview currently selected.
    /// </summary>
    event Action<object> SubViewChanged;
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
      }
      if (_view is not null)
      {
        _view.InitializeSubViews(GetTabNamesAndViews());
        _view.SetOldColor(_initialColor);
        _view.SetNewColor(_doc);
        _view.ColorName = _doc.Name;
      }
    }

    private IEnumerable<Tuple<string, object>> GetTabNamesAndViews()
    {
      if (_subControllerColorModel is not null)
      {
        if (_subControllerColorModel.ViewObject is null)
          Current.Gui.FindAndAttachControlTo(_subControllerColorModel);
        if (_subControllerColorModel.ViewObject is not null)
          yield return new Tuple<string, object>("Models", _subControllerColorModel.ViewObject);
      }

      if (_subControllerColorCircle is not null)
      {
        if (_subControllerColorCircle.ViewObject is null)
          Current.Gui.FindAndAttachControlTo(_subControllerColorCircle);
        if (_subControllerColorCircle.ViewObject is not null)
          yield return new Tuple<string, object>("Circle", _subControllerColorCircle.ViewObject);
      }

      if (_subControllerColorPicker is not null)
      {
        if (_subControllerColorPicker.ViewObject is null)
          Current.Gui.FindAndAttachControlTo(_subControllerColorPicker);
        if (_subControllerColorPicker.ViewObject is not null)
          yield return new Tuple<string, object>("Picker", _subControllerColorPicker.ViewObject);
      }
    }

    public override bool Apply(bool disposeController)
    {
      var userColorName = _view.ColorName;

      if (!string.IsNullOrEmpty(userColorName) && userColorName != NamedColor.GetColorName(_doc.Color))
      {
        _doc = new NamedColor(_doc.Color, userColorName);

        // TODO if the user has provided a color name, then store this color in a list of colors
      }

      return ApplyEnd(true, disposeController);
    }

    protected override void AttachView()
    {
      base.AttachView();

      _view.SubViewChanged += EhSubViewChanged;
    }

    protected override void DetachView()
    {
      _view.SubViewChanged -= EhSubViewChanged;
      base.DetachView();
    }

    private void EhSubViewChanged(object obj)
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
      _view?.SetNewColor(color);
      _view.ColorName = _doc.Name;
    }
  }
}
