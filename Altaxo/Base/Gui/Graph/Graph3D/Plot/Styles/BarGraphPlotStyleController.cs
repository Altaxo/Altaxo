#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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
using System.Text;
using Altaxo.Collections;
using Altaxo.Drawing;
using Altaxo.Drawing.ColorManagement;
using Altaxo.Drawing.D3D;
using Altaxo.Graph;
using Altaxo.Graph.Graph3D;
using Altaxo.Graph.Graph3D.Plot.Styles;
using Altaxo.Graph.Plot.Groups;
using Altaxo.Gui.Graph;
using Altaxo.Gui.Graph.Plot.Groups;

namespace Altaxo.Gui.Graph.Graph3D.Plot.Styles
{
  #region Interfaces

  public interface IBarGraphPlotStyleView
  {
    bool ShowPlotColorsOnly { set; }

    bool IndependentColor { get; set; }

    PenX3D Pen { get; set; }

    bool UseUniformCrossSectionThickness { get; set; }

    SelectableListNodeList BarShiftStrategy { set; }

    event Action BarShiftStrategyChanged;

    int BarShiftMaxItemsInOneDirection { get; set; }

    bool IsEnabledNumberOfBarsInOneDirection { set; }

    double InnerGapX { get; set; }

    double OuterGapX { get; set; }

    double InnerGapY { get; set; }

    double OuterGapY { get; set; }

    bool UsePhysicalBaseValueV { get; set; }

    double LogicalBaseValueV { get; set; }

    bool StartAtPreviousItem { get; set; }

    double GapV { get; set; }

    /// <summary>Occurs when the user choice for IndependentColor has changed.</summary>
    event Action IndependentColorChanged;
  }

  #endregion Interfaces

  [UserControllerForObject(typeof(BarGraphPlotStyle))]
  [ExpectedTypeOfView(typeof(IBarGraphPlotStyleView))]
  public class BarGraphPlotStyleController : MVCANControllerEditOriginalDocBase<BarGraphPlotStyle, IBarGraphPlotStyleView>
  {
    /// <summary>Tracks the presence of a color group style in the parent collection.</summary>
    private ColorGroupStylePresenceTracker _colorGroupStyleTracker;

    private SelectableListNodeList _barShiftStrategy;

    public override IEnumerable<ControllerAndSetNullMethod> GetSubControllers()
    {
      yield break;
    }

    protected override void Initialize(bool initData)
    {
      base.Initialize(initData);

      if (initData)
      {
        _colorGroupStyleTracker = new ColorGroupStylePresenceTracker(_doc, EhColorGroupStyleAddedOrRemoved);

        _barShiftStrategy = new SelectableListNodeList(_doc.BarShiftStrategy);
      }
      if (_view != null)
      {
        _view.IndependentColor = _doc.IndependentColor;
        _view.ShowPlotColorsOnly = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentColor);
        _view.Pen = _doc.Pen;
        _view.UseUniformCrossSectionThickness = _doc.UseUniformCrossSectionThickness;

        _view.BarShiftStrategy = _barShiftStrategy;

        _view.IsEnabledNumberOfBarsInOneDirection = _doc.BarShiftStrategy == BarShiftStrategy3D.ManualFirstXThenY || _doc.BarShiftStrategy == BarShiftStrategy3D.ManualFirstYThenX;
        _view.BarShiftMaxItemsInOneDirection = _doc.BarShiftMaxItemsInOneDirection;

        _view.InnerGapX = _doc.InnerGapX;
        _view.OuterGapX = _doc.OuterGapX;
        _view.InnerGapY = _doc.InnerGapY;
        _view.OuterGapY = _doc.OuterGapY;
        _view.UsePhysicalBaseValueV = _doc.UsePhysicalBaseValue;
        _view.LogicalBaseValueV = _doc.UsePhysicalBaseValue ? 0 : _doc.BaseValue;
        _view.StartAtPreviousItem = _doc.StartAtPreviousItem;
        _view.GapV = _doc.PreviousItemGapV;
      }
    }

    public override bool Apply(bool disposeController)
    {
      _doc.IndependentColor = _view.IndependentColor;
      _doc.Pen = _view.Pen;
      _doc.UseUniformCrossSectionThickness = _view.UseUniformCrossSectionThickness;

      _doc.BarShiftStrategy = (BarShiftStrategy3D)_barShiftStrategy.FirstSelectedNode.Tag;
      _doc.BarShiftMaxItemsInOneDirection = _view.BarShiftMaxItemsInOneDirection;

      _doc.InnerGapX = _view.InnerGapX;
      _doc.OuterGapX = _view.OuterGapX;
      _doc.InnerGapY = _view.InnerGapY;
      _doc.OuterGapY = _view.OuterGapY;

      _doc.UsePhysicalBaseValue = _view.UsePhysicalBaseValueV;
      if (_view.UsePhysicalBaseValueV)
      {
        // who can parse this string? Only the v (z) -scale know how to parse it
      }
      else
      {
        _doc.BaseValue = _view.LogicalBaseValueV;
      }

      _doc.StartAtPreviousItem = _view.StartAtPreviousItem;
      _doc.PreviousItemGapV = _view.GapV;

      return ApplyEnd(true, disposeController);
    }

    protected override void AttachView()
    {
      _view.IndependentColorChanged += EhIndependentColorChanged;
      _view.BarShiftStrategyChanged += EhBarShiftStrategyChanged;
      base.AttachView();
    }

    protected override void DetachView()
    {
      _view.IndependentColorChanged -= EhIndependentColorChanged;
      _view.BarShiftStrategyChanged -= EhBarShiftStrategyChanged;
      base.DetachView();
    }

    private void EhColorGroupStyleAddedOrRemoved()
    {
      if (null != _view)
      {
        _doc.IndependentColor = _view.IndependentColor;
        _view.ShowPlotColorsOnly = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentColor);
      }
    }

    private void EhBarShiftStrategyChanged()
    {
      if (null == _barShiftStrategy.FirstSelectedNode)
        return;

      _doc.BarShiftStrategy = (BarShiftStrategy3D)_barShiftStrategy.FirstSelectedNode.Tag;
      _view.IsEnabledNumberOfBarsInOneDirection = _view.IsEnabledNumberOfBarsInOneDirection = _doc.BarShiftStrategy == BarShiftStrategy3D.ManualFirstXThenY || _doc.BarShiftStrategy == BarShiftStrategy3D.ManualFirstYThenX;
    }

    private void EhIndependentColorChanged()
    {
      if (null != _view)
      {
        _doc.IndependentColor = _view.IndependentColor;
        _view.ShowPlotColorsOnly = _colorGroupStyleTracker.MustUsePlotColorsOnly(_doc.IndependentColor);
      }
    }
  }
}
