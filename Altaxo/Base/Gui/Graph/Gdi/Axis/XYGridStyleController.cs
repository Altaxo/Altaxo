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
using Altaxo.Drawing;
using Altaxo.Graph.Gdi.Axis;
using Altaxo.Gui.Common.Drawing;

namespace Altaxo.Gui.Graph.Gdi.Axis
{
  /// <summary>
  /// Provides the view contract for <see cref="XYGridStyleController"/>.
  /// </summary>
  public interface IXYGridStyleView : IDataContextAwareView
  {
  }

  /// <summary>
  /// Controller for <see cref="GridStyle"/>.
  /// </summary>
  [UserControllerForObject(typeof(GridStyle))]
  [ExpectedTypeOfView(typeof(IXYGridStyleView))]
  public class XYGridStyleController : MVCANControllerEditOriginalDocBase<GridStyle, IXYGridStyleView>
  {
   

    /// <inheritdoc />
    public override System.Collections.Generic.IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_majorController, () => _majorController = null);
      yield return new ControllerAndSetNullMethod(_majorController, () => _minorController = null);
    }

    #region Binding

    private bool  _showGrid;

    /// <summary>
    /// Gets or sets a value indicating whether the grid is shown.
    /// </summary>
    public bool  ShowGrid
    {
      get => _showGrid;
      set
      {
        if (!(_showGrid == value))
        {
          _showGrid = value;
          OnPropertyChanged(nameof(ShowGrid));
          OnPropertyChanged(nameof(EnableMinorCheck));
          OnPropertyChanged(nameof(EnableMajorView));
          OnPropertyChanged(nameof(EnableMinorView));
        }
      }
    }

    private bool _showZeroOnly;

    /// <summary>
    /// Gets or sets a value indicating whether only the zero grid line is shown.
    /// </summary>
    public bool ShowZeroOnly
    {
      get => _showZeroOnly;
      set
      {
        if (!(_showZeroOnly == value))
        {
          _showZeroOnly = value;
          OnPropertyChanged(nameof(ShowZeroOnly));
          OnPropertyChanged(nameof(EnableMinorCheck));
          OnPropertyChanged(nameof(EnableMinorView));
        }
      }
    }

    private bool _showMinorGrid;

    /// <summary>
    /// Gets or sets a value indicating whether minor grid lines are shown.
    /// </summary>
    public bool ShowMinorGrid
    {
      get => _showMinorGrid;
      set
      {
        if (!(_showMinorGrid == value))
        {
          _showMinorGrid = value;
          OnPropertyChanged(nameof(ShowMinorGrid));
          OnPropertyChanged(nameof(EnableMinorView));
        }
      }
    }

    /// <summary>
    /// Gets a value indicating whether the show minor check box is enabled
    /// </summary>
    public bool EnableMinorCheck => ShowGrid && !ShowZeroOnly;

    /// <summary>
    /// Gets a value indicating whether the show minor grid view is enabled
    /// </summary>
    public bool EnableMajorView => ShowGrid;

    /// <summary>
    /// Gets a value indicating whether the show minor grid view is enabled
    /// </summary>
    public bool EnableMinorView => ShowGrid && !ShowZeroOnly && ShowMinorGrid;


    private IMVCANDController _majorController;
    

    /// <summary>
    /// Gets or sets the controller for the major grid pen.
    /// </summary>
    public IMVCANDController MajorController
    {
      get => _majorController;
      set
      {
        if (!(_majorController == value))
        {
          _majorController = value;
          OnPropertyChanged(nameof(MajorController));
        }
      }
    }

    private IMVCANDController _minorController;

    /// <summary>
    /// Gets or sets the controller for the minor grid pen.
    /// </summary>
    public IMVCANDController MinorController
    {
      get => _minorController;
      set
      {
        if (!(_minorController == value))
        {
          _minorController = value;
          OnPropertyChanged(nameof(MinorController));
        }
      }
    }





    #endregion

    /// <inheritdoc />
    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        MajorController = new ColorTypeThicknessPenController(_doc.MajorPen);
        MinorController = new ColorTypeThicknessPenController(_doc.MinorPen);

        ShowMinorGrid =_doc.ShowMinor;
        ShowZeroOnly =_doc.ShowZeroOnly;
        ShowGrid = _doc.ShowGrid;

      }
    }

    /// <inheritdoc />
    public override bool Apply(bool disposeController)
    {
      if (!_majorController.Apply(disposeController))
        return false;
      else
        _doc.MajorPen = (PenX)_majorController.ModelObject;


      if (!_minorController.Apply(disposeController))
        return false;
      else
        _doc.MinorPen = (PenX)_minorController.ModelObject;

      _doc.ShowGrid = ShowGrid;
      _doc.ShowZeroOnly = ShowZeroOnly;
      _doc.ShowMinor = ShowMinorGrid;

      return ApplyEnd(true, disposeController);
    }
  }
}
