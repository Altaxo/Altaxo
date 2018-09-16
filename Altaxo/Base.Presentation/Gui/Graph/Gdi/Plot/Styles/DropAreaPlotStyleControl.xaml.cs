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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Altaxo.Gui.Graph.Gdi.Plot.Styles
{
  using Altaxo.Collections;
  using Altaxo.Graph.Gdi;
  using Common.Drawing;

  /// <summary>
  /// Interaction logic for XYPlotLineStyleControl.xaml
  /// </summary>
  public partial class DropAreaPlotStyleControl : UserControl, IDropAreaPlotStyleView
  {
    private PenControlsGlue _framePenGlue;

    public event Action IndependentLineColorChanged;

    public event Action UseLineChanged;

    public event Action LinePenChanged;

    public event Action FillColorLinkageChanged;

    public event Action FrameColorLinkageChanged;

    public event Action FillBrushChanged;

    public event Action FramePenChanged;

    public DropAreaPlotStyleControl()
    {
      InitializeComponent();

      _framePenGlue = new PenControlsGlue(false);
      _framePenGlue.PenChanged += new EventHandler(EhFramePenChanged);
      _framePenGlue.CbBrush = _guiFrameBrush;
      _framePenGlue.CbDashPattern = _guiFrameDashStyle;
      _framePenGlue.CbLineThickness = _guiFrameThickness1;
    }

    private void EhUseLineConnectChanged(object sender, RoutedEventArgs e)
    {
      GuiHelper.SynchronizeSelectionFromGui(_guiLineConnect);
      if (null != _guiLineConnect.SelectedItem) // null for SelectedItem can happen when the DataSource is chaning
        UseLineChanged?.Invoke();
    }

    public void InitializeLineConnect(SelectableListNodeList list)
    {
      GuiHelper.Initialize(_guiLineConnect, list);
    }

    public bool ConnectCircular
    {
      get { return true == _guiConnectCircular.IsChecked; }
      set { _guiConnectCircular.IsChecked = value; }
    }

    public bool IgnoreMissingDataPoints
    {
      get { return true == _guiIgnoreMissingPoints.IsChecked; }
      set { _guiIgnoreMissingPoints.IsChecked = value; }
    }

    public bool IndependentOnShiftingGroupStyles
    {
      get { return true == _guiIndependentOnShiftingGroupStyles.IsChecked; }
      set { _guiIndependentOnShiftingGroupStyles.IsChecked = value; }
    }

    public void InitializeFillDirection(SelectableListNodeList list)
    {
      GuiHelper.Initialize(_guiFillDirection, list);
    }

    private void EhFillDirectionChanged(object sender, SelectionChangedEventArgs e)
    {
      GuiHelper.SynchronizeSelectionFromGui(_guiFillDirection);
    }

    public void InitializeFillColorLinkage(SelectableListNodeList list)
    {
      _guiFillColorLinkage.Initialize(list);
    }

    private void EhFillColorLinkageChanged()
    {
      FillColorLinkageChanged?.Invoke();
    }

    public bool ShowPlotColorsOnlyForFillBrush
    {
      set { _guiFillBrush.ShowPlotColorsOnly = value; }
    }

    public Altaxo.Graph.Gdi.BrushX FillBrush
    {
      get
      {
        return _guiFillBrush.SelectedBrush;
      }
      set
      {
        if (null == value)
          throw new ArgumentNullException("FillBrush");
        _guiFillBrush.SelectedBrush = value;
      }
    }

    private void EhFillBrushChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      FillBrushChanged?.Invoke();
    }

    public void InitializeFrameColorLinkage(SelectableListNodeList list)
    {
      _guiFrameColorLinkage.Initialize(list);
    }

    private void EhFrameColorLinkageChanged()
    {
      FrameColorLinkageChanged?.Invoke();
    }

    public bool ShowPlotColorsOnlyForFramePen
    {
      set { _framePenGlue.ShowPlotColorsOnly = value; }
    }

    public PenX FramePen
    {
      get
      {
        return _framePenGlue.Pen;
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException("FramePen");
        _framePenGlue.Pen = value;
      }
    }

    private void EhFramePenChanged(object sender, EventArgs e)
    {
      FramePenChanged?.Invoke();
    }
  }
}
