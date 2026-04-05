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
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altaxo.Drawing;

namespace Altaxo.Gui.Drawing.ColorManagement
{
  /// <summary>
  /// View interface for selecting a color.
  /// </summary>
  public interface IColorPickerView
  {
    /// <summary>
    /// Gets or sets the selected color.
    /// </summary>
    AxoColor SelectedColor { get; set; }

    /// <summary>
    /// Occurs when the current color changes.
    /// </summary>
    event Action<AxoColor> CurrentColorChanged;
  }

  /// <summary>
  /// Controller to pick a custom color.
  /// </summary>
  [ExpectedTypeOfView(typeof(IColorPickerView))]
  public class ColorPickerController : MVCANDControllerEditImmutableDocBase<AxoColor, IColorPickerView>
  {
    /// <inheritdoc/>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    /// <inheritdoc/>
    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (_view is not null)
      {
        _view.SelectedColor = _doc;
      }
    }

    /// <inheritdoc/>
    public override bool Apply(bool disposeController)
    {
      _doc = _view.SelectedColor;

      return ApplyEnd(true, disposeController);
    }

    /// <inheritdoc/>
    protected override void AttachView()
    {
      base.AttachView();

      _view.CurrentColorChanged += EhCurrentColorChanged;
    }

    /// <inheritdoc/>
    protected override void DetachView()
    {
      _view.CurrentColorChanged -= EhCurrentColorChanged;

      base.DetachView();
    }

    private void EhCurrentColorChanged(AxoColor color)
    {
      _doc = color;
      OnMadeDirty();
    }
  }
}
