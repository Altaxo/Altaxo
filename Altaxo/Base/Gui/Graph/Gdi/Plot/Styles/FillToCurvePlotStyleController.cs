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

using Altaxo.Drawing;
using Altaxo.Drawing.ColorManagement;
using Altaxo.Graph;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Plot.Styles;
using Altaxo.Gui.Graph.Plot.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui.Graph.Gdi.Plot.Styles
{
  public interface IFillToCurvePlotStyleView
  {
    bool UseFill { get; set; }

    bool ShowPlotColorsOnlyForFillBrush { set; }

    bool IndependentFillColor { get; set; }

    BrushX FillBrush { get; set; }

    bool UseFrame { get; set; }

    bool ShowPlotColorsOnlyForFramePen { set; }

    bool IndependentFrameColor { get; set; }

    PenX FramePen { get; set; }

    bool FillToPreviousItem { get; set; }

    bool FillToNextItem { get; set; }

    /// <summary>Occurs when the user choice for IndependentColor of the fill brush has changed.</summary>
    event Action IndependentFillColorChanged;

    /// <summary>Occurs when the user choice for IndependentColor of the frame pen has changed.</summary>
    event Action IndependentFrameColorChanged;

    /// <summary>Occurs when the user checked or unchecked the "use fill" checkbox.</summary>
    event Action UseFillChanged;

    /// <summary>Occurs when the user checked or unchecked the "use frame" checkbox.</summary>
    event Action UseFrameChanged;

    /// <summary>Occurs when the fill brush has changed by user interaction.</summary>
    event Action FillBrushChanged;

    /// <summary>Occurs when the  frame pen has changed by user interaction.</summary>
    event Action FramePenChanged;
  }

  [UserControllerForObject(typeof(FillToCurvePlotStyle))]
  [ExpectedTypeOfView(typeof(IFillToCurvePlotStyleView))]
  public class FillToCurvePlotStyleController : MVCANControllerEditOriginalDocBase<FillToCurvePlotStyle, IFillToCurvePlotStyleView>
  {
    /// <summary>Tracks the presence of a color group style in the parent collection.</summary>
    private ColorGroupStylePresenceTracker _colorGroupStyleTracker;

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    public override void Dispose(bool isDisposing)
    {
      _colorGroupStyleTracker = null;
      base.Dispose(isDisposing);
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _colorGroupStyleTracker = new ColorGroupStylePresenceTracker(_doc, EhColorGroupStyleAddedOrRemoved);
      }
      if (_view != null)
      {
        _view.UseFill = _doc.FillBrush != null && _doc.FillBrush.IsVisible;
        _view.IndependentFillColor = _doc.IndependentFillColor;
        _view.ShowPlotColorsOnlyForFillBrush = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentFillColor);
        _view.FillBrush = null != _doc.FillBrush ? _doc.FillBrush : new BrushX(NamedColors.Transparent);

        _view.UseFrame = _doc.FramePen != null && _doc.FramePen.IsVisible;
        _view.IndependentFrameColor = _doc.IndependentFrameColor;
        _view.ShowPlotColorsOnlyForFramePen = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentFrameColor);
        _view.FramePen = null != _doc.FramePen ? _doc.FramePen : new PenX(NamedColors.Transparent);

        _view.FillToPreviousItem = _doc.FillToPreviousItem;
        _view.FillToNextItem = _doc.FillToNextItem;
      }
    }

    public override bool Apply(bool disposeController)
    {
      if (_view.UseFill)
      {
        _doc.IndependentFillColor = _view.IndependentFillColor;
        _doc.FillBrush = _view.UseFill ? _view.FillBrush : null;
      }
      else
      {
        _doc.IndependentFillColor = true;
        _doc.FillBrush = null;
      }

      if (_view.UseFrame)
      {
        _doc.IndependentFrameColor = _view.IndependentFrameColor;
        _doc.FramePen = _view.FramePen;
      }
      else
      {
        _doc.IndependentFrameColor = true;
        _doc.FramePen = null;
      }

      _doc.FillToPreviousItem = _view.FillToPreviousItem;
      _doc.FillToNextItem = _view.FillToNextItem;

      return ApplyEnd(true, disposeController);
    }

    protected override void AttachView()
    {
      base.AttachView();
      _view.UseFillChanged += EhUseFillChanged;
      _view.IndependentFillColorChanged += EhIndependentFillColorChanged;

      _view.UseFrameChanged += EhUseFrameChanged;
      _view.IndependentFrameColorChanged += EhIndependentFrameColorChanged;

      _view.FillBrushChanged += EhFillBrushChanged;
      _view.FramePenChanged += EhFramePenChanged;
    }

    protected override void DetachView()
    {
      _view.UseFillChanged -= EhUseFillChanged;
      _view.IndependentFillColorChanged -= EhIndependentFillColorChanged;

      _view.UseFrameChanged -= EhUseFrameChanged;
      _view.IndependentFrameColorChanged -= EhIndependentFrameColorChanged;

      _view.FillBrushChanged -= EhFillBrushChanged;
      _view.FramePenChanged -= EhFramePenChanged;

      base.DetachView();
    }

    #region Color management

    private void EhColorGroupStyleAddedOrRemoved()
    {
      if (null != _view)
      {
        _doc.IndependentFillColor = _view.IndependentFillColor;
        _doc.IndependentFrameColor = _view.IndependentFrameColor;
        if (_view.UseFill)
          _view.ShowPlotColorsOnlyForFillBrush = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentFillColor);
        if (_view.UseFrame)
          _view.ShowPlotColorsOnlyForFramePen = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentFrameColor);
      }
    }

    private void EhIndependentFillColorChanged()
    {
      if (null != _view)
      {
        _doc.IndependentFillColor = _view.IndependentFillColor;
        if (false == _view.IndependentFillColor && _view.UseFrame && false == _view.IndependentFrameColor)
          InternalSetFillColorToFrameColor();
        _view.ShowPlotColorsOnlyForFillBrush = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentFillColor);
      }
    }

    private void EhIndependentFrameColorChanged()
    {
      if (null != _view)
      {
        _doc.IndependentFrameColor = _view.IndependentFrameColor;
        if (false == _view.IndependentFrameColor && _view.UseFill && false == _view.IndependentFillColor)
          InternalSetFrameColorToFillColor();
        _view.ShowPlotColorsOnlyForFramePen = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentFrameColor);
      }
    }

    private void EhFillBrushChanged()
    {
      if (null != _view)
      {
        if (_view.UseFill && false == _view.IndependentFillColor && _view.UseFrame && false == _view.IndependentFrameColor)
        {
          if (_view.FramePen.Color != _view.FillBrush.Color)
            InternalSetFrameColorToFillColor();
        }
      }
    }

    private void EhFramePenChanged()
    {
      if (null != _view)
      {
        if (_view.UseFill && false == _view.IndependentFillColor && _view.UseFrame && false == _view.IndependentFrameColor)
        {
          if (_view.FillBrush.Color != _view.FramePen.Color)
            InternalSetFillColorToFrameColor();
        }
      }
    }

    private void InternalSetFillColorToFrameColor()
    {
      var newBrush = _view.FillBrush.Clone();
      newBrush.Color = _view.FramePen.Color;
      _view.FillBrush = newBrush;
    }

    private void InternalSetFrameColorToFillColor()
    {
      var newPen = _view.FramePen.Clone();
      newPen.Color = _view.FillBrush.Color;
      _view.FramePen = newPen;
    }

    private void EhUseFillChanged()
    {
      var newValue = _view.UseFill;

      if (true == newValue)
      {
        if (_view.UseFrame && false == _view.IndependentFrameColor)
        {
          InternalSetFillColorToFrameColor();
        }
        else if (null == _view.FillBrush || _view.FillBrush.IsInvisible)
        {
          _view.FillBrush = new BrushX(ColorSetManager.Instance.BuiltinDarkPlotColors[0]);
        }
      }
      _view.UseFill = newValue; // to enable/disable gui items in the control
    }

    private void EhUseFrameChanged()
    {
      var newValue = _view.UseFrame;

      if (true == newValue)
      {
        if (_view.UseFill && false == _view.IndependentFillColor)
        {
          InternalSetFrameColorToFillColor();
        }
        else if (null == _view.FramePen || _view.FramePen.IsInvisible)
        {
          _view.FramePen = new PenX(ColorSetManager.Instance.BuiltinDarkPlotColors[0]);
        }
      }

      _view.UseFrame = newValue; // to enable/disable gui items in the control
    }

    #endregion Color management
  }
}
