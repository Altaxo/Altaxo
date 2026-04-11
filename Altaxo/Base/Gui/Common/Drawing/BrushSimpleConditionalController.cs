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
  /// <summary>
  /// Defines the view contract for simple conditional brush editing.
  /// </summary>
  public interface IBrushSimpleConditionalView : IDataContextAwareView
  {
  }


  /// <summary>
  /// Controller for editing a brush that can be enabled or disabled.
  /// </summary>
  [ExpectedTypeOfView(typeof(IBrushSimpleConditionalView))]
  public class BrushSimpleConditionalController : MVCANDControllerEditImmutableDocBase<BrushX, object>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="BrushSimpleConditionalController"/> class.
    /// </summary>
    /// <param name="doc">The brush document to edit.</param>
    public BrushSimpleConditionalController(BrushX doc)
    {
      _doc = _originalDoc = doc ?? new BrushX(NamedColors.Transparent);
      
    }

    /// <summary>
    /// Gets or sets the effective brush document.
    /// </summary>
    public BrushX Doc
    {
      get => _isEnabled ? Brush : Brush.WithColor(NamedColors.Transparent);
      set
      {
        Brush = value ?? new BrushX(NamedColors.Transparent);
        IsEnabled = value.IsVisible;
      }
    }

    /// <inheritdoc/>
    public override object ModelObject => Doc;
    /// <inheritdoc/>
    public override object ProvisionalModelObject => Doc;

    /// <inheritdoc/>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private bool _isEnabled;

    /// <summary>
    /// Gets or sets a value indicating whether the brush is enabled.
    /// </summary>
    public bool IsEnabled
    {
      get => _isEnabled;
      set
      {
        if (!(_isEnabled == value))
        {
          _isEnabled = value;
          OnPropertyChanged(nameof(IsEnabled));
          if(value && Brush.IsInvisible)
          {
            Brush = Brush.WithColor(NamedColors.AliceBlue);
          }
        }
      }
    }


    /// <summary>
    /// Gets or sets the brush.
    /// </summary>
    public BrushX Brush
    {
      get => _doc;
      set
      {
        if (!(_doc == value))
        {
          _doc = value;
          OnPropertyChanged(nameof(Brush));
          OnMadeDirty();
        }
      }
    }

    /// <inheritdoc/>
    public override bool Apply(bool disposeController)
    {
      return ApplyEnd(true, disposeController);
    }

    /// <inheritdoc/>
    protected override void OnMadeDirty()
    {
      base.OnMadeDirty();
      OnPropertyChanged(nameof(Doc));
    }

    #endregion
  }
}
