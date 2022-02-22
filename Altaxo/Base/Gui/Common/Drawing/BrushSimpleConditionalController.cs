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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Drawing;

namespace Altaxo.Gui.Common.Drawing
{
  public interface IBrushSimpleConditionalView : IDataContextAwareView
  {
  }


  [ExpectedTypeOfView(typeof(IBrushSimpleConditionalView))]
  public class BrushSimpleConditionalController : MVCANDControllerEditImmutableDocBase<BrushX, object>
  {
    public BrushSimpleConditionalController(BrushX doc)
    {
      _doc = _originalDoc = doc;
    }

    public BrushX Doc
    {
      get => _isEnabled ? _brush : _brush.WithColor(NamedColors.Transparent);
      set
      {
        Brush = value;
        IsEnabled = value.IsVisible;
      }
    }

    public override object ModelObject => Doc;
    public override object ProvisionalModelObject => Doc;

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private bool _isEnabled;

    public bool IsEnabled
    {
      get => _isEnabled;
      set
      {
        if (!(_isEnabled == value))
        {
          _isEnabled = value;
          OnPropertyChanged(nameof(IsEnabled));
          if(value && _brush.IsInvisible)
          {
            Brush = Brush.WithColor(NamedColors.AliceBlue);
          }
        }
      }
    }

    private BrushX _brush;

    public BrushX Brush
    {
      get => _brush;
      set
      {
        if (!(_brush == value))
        {
          _brush = value;
          OnPropertyChanged(nameof(Brush));
          OnMadeDirty();
        }
      }
    }

    public override bool Apply(bool disposeController)
    {
      return ApplyEnd(true, disposeController);
    }

    protected override void OnMadeDirty()
    {
      base.OnMadeDirty();
      OnPropertyChanged(nameof(Doc));
    }

    #endregion
  }
}
