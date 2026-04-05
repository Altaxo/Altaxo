#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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
using System.Collections.Generic;
using Altaxo.Drawing;

namespace Altaxo.Gui.Graph.ColorManagement
{
  /// <summary>
  /// View contract for choosing a named color.
  /// </summary>
  public interface INamedColorChoiceView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Controller for selecting a <see cref="NamedColor"/>.
  /// </summary>
  [ExpectedTypeOfView(typeof(INamedColorChoiceView))]
  public class NamedColorChoiceController : MVCANControllerEditImmutableDocBase<NamedColor, INamedColorChoiceView>
  {
    /// <inheritdoc/>
    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    #region Bindings

    private bool _ShowPlotColorsOnly;

    /// <summary>
    /// Gets or sets a value indicating whether only plot colors are allowed.
    /// </summary>
    public bool ShowPlotColorsOnly
    {
      get => _ShowPlotColorsOnly;
      set
      {
        if (!(_ShowPlotColorsOnly == value))
        {
          _ShowPlotColorsOnly = value;
          OnPropertyChanged(nameof(ShowPlotColorsOnly));
        }
      }
    }


    /// <summary>
    /// Gets or sets the selected color.
    /// </summary>
    public NamedColor SelectedColor
    {
      get => _doc;
      set
      {
        if (!(_doc == value))
        {
          _doc = value;
          OnPropertyChanged(nameof(SelectedColor));
        }
      }
    }


    #endregion

    /// <inheritdoc/>
    public override bool Apply(bool disposeController)
    {
      if (ShowPlotColorsOnly && _doc.ParentColorSet is null)
      {
        Current.Gui.ErrorMessageBox("You have chosen a custom color, but a plot color is required. Please choose one of the defined plot colors.", "Custom colors not allowed");
        return false;
      }

      if (_doc.ParentColorSet is null)
      {
        if (!Current.Gui.YesNoMessageBox(
          "You have chosen a custom color. This is not recommended, because a custom color does not belong to a color set.\r\n" +
          "The recommended way is to define a new color set which contains the color of your choice.\r\n" +
          "Do you want to use this color nevertheless?", "Warning", false))
          return false;
      }

      return ApplyEnd(true, disposeController);
    }
  }
}
