﻿#region Copyright

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
  #region Interfaces

  public interface IXYGridStyleView
  {
    void InitializeBegin();

    void InitializeEnd();

    void InitializeMajorGridStyle(IColorTypeThicknessPenController controller);

    void InitializeMinorGridStyle(IColorTypeThicknessPenController controller);

    void InitializeShowGrid(bool value);

    void InitializeShowMinorGrid(bool value);

    void InitializeShowZeroOnly(bool value);

    void InitializeElementEnabling(bool majorstyle, bool minorstyle, bool showminor, bool showzeroonly);

    event Action<bool> ShowGridChanged;

    event Action<bool> ShowMinorGridChanged;

    event Action<bool> ShowZeroOnlyChanged;
  }

  #endregion Interfaces

  /// <summary>
  /// Summary description for XYGridStyleController.
  /// </summary>
  [UserControllerForObject(typeof(GridStyle))]
  [ExpectedTypeOfView(typeof(IXYGridStyleView))]
  public class XYGridStyleController : MVCANControllerEditOriginalDocBase<GridStyle, IXYGridStyleView>
  {
    private IColorTypeThicknessPenController _majorController;
    private IColorTypeThicknessPenController _minorController;

    public override System.Collections.Generic.IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield return new ControllerAndSetNullMethod(_majorController, () => _majorController = null);
      yield return new ControllerAndSetNullMethod(_majorController, () => _minorController = null);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _majorController = new ColorTypeThicknessPenController(_doc.MajorPen);
        _minorController = new ColorTypeThicknessPenController(_doc.MinorPen);
      }

      if (_view is not null)
      {
        _view.InitializeBegin();

        _view.InitializeMajorGridStyle(_majorController);
        _view.InitializeMinorGridStyle(_minorController);
        _view.InitializeShowMinorGrid(_doc.ShowMinor);
        _view.InitializeShowZeroOnly(_doc.ShowZeroOnly);
        _view.InitializeShowGrid(_doc.ShowGrid);
        InitializeElementEnabling();

        _view.InitializeEnd();
      }
    }

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


      return ApplyEnd(true, disposeController);
    }

    protected override void AttachView()
    {
      base.AttachView();
      _view.ShowGridChanged += EhView_ShowGridChanged;
      _view.ShowMinorGridChanged += EhView_ShowMinorGridChanged;
      _view.ShowZeroOnlyChanged += EhView_ShowZeroOnlyChanged;
    }

    protected override void DetachView()
    {
      _view.ShowGridChanged -= EhView_ShowGridChanged;
      _view.ShowMinorGridChanged -= EhView_ShowMinorGridChanged;
      _view.ShowZeroOnlyChanged -= EhView_ShowZeroOnlyChanged;
      base.DetachView();
    }

    public void InitializeElementEnabling()
    {
      if (_view is not null)
      {
        bool majorstyle = _doc.ShowGrid;
        bool showzeroonly = _doc.ShowGrid;
        bool showminor = _doc.ShowGrid && !_doc.ShowZeroOnly;
        bool minorstyle = _doc.ShowMinor && showminor;
        _view.InitializeElementEnabling(majorstyle, minorstyle, showminor, showzeroonly);
      }
    }

    #region IXYGridStyleViewEventSink Members

    public void EhView_ShowGridChanged(bool newval)
    {
      _doc.ShowGrid = newval;
      InitializeElementEnabling();
    }

    public void EhView_ShowMinorGridChanged(bool newval)
    {
      _doc.ShowMinor = newval;
      InitializeElementEnabling();
    }

    public void EhView_ShowZeroOnlyChanged(bool newval)
    {
      _doc.ShowZeroOnly = newval;
      if (newval == true && _doc.ShowMinor)
      {
        _doc.ShowMinor = false;
        _view.InitializeShowMinorGrid(_doc.ShowMinor);
      }
      InitializeElementEnabling();
    }

    #endregion IXYGridStyleViewEventSink Members
  }
}
