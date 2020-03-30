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
  using Altaxo.Drawing;
  using Altaxo.Gui.Common.Drawing;

  /// <summary>
  /// Interaction logic for FillToCurvePlotStyleControl.xaml
  /// </summary>
  public partial class FillToCurvePlotStyleControl : UserControl, IFillToCurvePlotStyleView
  {
    private PenControlsGlue _framePenGlue;

    public event Action IndependentFillColorChanged;

    public event Action IndependentFrameColorChanged;

    public event Action UseFillChanged;

    public event Action UseFrameChanged;

    public event Action FillBrushChanged;

    public event Action FramePenChanged;

    public FillToCurvePlotStyleControl()
    {
      InitializeComponent();

      _framePenGlue = new PenControlsGlue(false);
      _framePenGlue.PenChanged += new EventHandler(EhFramePenChanged);
      _framePenGlue.CbBrush = _guiFramePen;
      _framePenGlue.CbLineThickness = _guiFramePenWidth;
      _framePenGlue.CbDashPattern = _guiFrameDashStyle;
    }

    #region Event handlers

    private void EhUseFillChanged(object sender, RoutedEventArgs e)
    {
      if (null != UseFillChanged)
        UseFillChanged();
    }

    private void EhIndependentFillColorChanged(object sender, RoutedEventArgs e)
    {
      if (null != IndependentFillColorChanged)
        IndependentFillColorChanged();
    }

    private void EhFillBrushChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if (null != FillBrushChanged)
        FillBrushChanged();
    }

    private void EhUseFrameChanged(object sender, RoutedEventArgs e)
    {
      if (null != UseFrameChanged)
        UseFrameChanged();
    }

    private void EhIndependentFrameColorChanged(object sender, RoutedEventArgs e)
    {
      if (null != IndependentFrameColorChanged)
        IndependentFrameColorChanged();
    }

    private void EhFramePenChanged(object sender, EventArgs e)
    {
      if (null != FramePenChanged)
        FramePenChanged();
    }

    #endregion Event handlers

    #region IFillToCurvePlotStyleView Members

    public bool UseFill
    {
      get
      {
        return true == _guiUseFill.IsChecked;
      }
      set
      {
        _guiUseFill.IsChecked = value;
        _guiIndependentFillColor.IsEnabled = value;
        _guiFillBrush.IsEnabled = value;
      }
    }

    public bool IndependentFillColor
    {
      get
      {
        return true == _guiIndependentFillColor.IsChecked;
      }
      set
      {
        _guiIndependentFillColor.IsChecked = value;
      }
    }

    public bool ShowPlotColorsOnlyForFillBrush
    {
      set { _guiFillBrush.ShowPlotColorsOnly = value; }
    }

    public BrushX FillBrush
    {
      get
      {
        return _guiFillBrush.SelectedBrush;
      }
      set
      {
        _guiFillBrush.SelectedBrush = value ?? throw new ArgumentNullException(nameof(FillBrush));
      }
    }

    public bool UseFrame
    {
      get
      {
        return true == _guiUseFrame.IsChecked;
      }
      set
      {
        _guiUseFrame.IsChecked = value;
        _guiIndependentFrameColor.IsEnabled = value;
        _guiFramePen.IsEnabled = value;
        _guiFramePenWidth.IsEnabled = value;
        _guiFrameDashStyle.IsEnabled = value;
      }
    }

    public bool IndependentFrameColor
    {
      get
      {
        return true == _guiIndependentFrameColor.IsChecked;
      }
      set
      {
        _guiIndependentFrameColor.IsChecked = value;
      }
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

    public bool FillToPreviousItem
    {
      get
      {
        return true == _chkFillPrevious.IsChecked;
      }
      set
      {
        _chkFillPrevious.IsChecked = value;
      }
    }

    public bool FillToNextItem
    {
      get
      {
        return true == _chkFillNext.IsChecked;
      }
      set
      {
        _chkFillNext.IsChecked = value;
      }
    }

    #endregion IFillToCurvePlotStyleView Members
  }
}
