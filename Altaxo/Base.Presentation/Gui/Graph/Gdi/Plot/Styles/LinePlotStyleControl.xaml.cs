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
  using Altaxo.Drawing;
  using Common.Drawing;

  /// <summary>
  /// Interaction logic for XYPlotLineStyleControl.xaml
  /// </summary>
  public partial class LinePlotStyleControl : UserControl, ILinePlotStyleView
  {
    private PenControlsGlue _linePenGlue;

    public event Action IndependentLineColorChanged;

    public event Action UseLineChanged;

    public event Action LinePenChanged;

    public LinePlotStyleControl()
    {
      InitializeComponent();

      _linePenGlue = new PenControlsGlue(false);
      _linePenGlue.PenChanged += new EventHandler(EhLinePenChanged);
      _linePenGlue.CbBrush = _guiLineBrush;
      _linePenGlue.CbDashPattern = _guiLineDashStyle;
      _linePenGlue.CbLineThickness = _guiLineThickness1;
    }

    #region Event handlers

    private void EhIndependentDashStyleChanged(object sender, RoutedEventArgs e)
    {
    }

    private void EhUseLineConnectChanged(object sender, RoutedEventArgs e)
    {
      GuiHelper.SynchronizeSelectionFromGui(_guiLineConnect);
      if (null != _guiLineConnect.SelectedItem) // null for SelectedItem can happen when the DataSource is chaning
        UseLineChanged?.Invoke();
    }

    private void EhIndependentLineColorChanged(object sender, RoutedEventArgs e)
    {
      IndependentLineColorChanged?.Invoke();
    }

    private void EhLinePenChanged(object sender, EventArgs e)
    {
      LinePenChanged?.Invoke();
    }

    #endregion Event handlers

    #region IXYPlotLineStyleView

    #region Line pen

    public bool IndependentLineColor
    {
      get
      {
        return true == _guiIndependentLineColor.IsChecked;
      }
      set
      {
        _guiIndependentLineColor.IsChecked = value;
      }
    }

    public bool IndependentDashStyle
    {
      get
      {
        return true == _guiIndependentDashStyle.IsChecked;
      }
      set
      {
        _guiIndependentDashStyle.IsChecked = value;
      }
    }

    public bool ShowPlotColorsOnlyForLinePen
    {
      set { _linePenGlue.ShowPlotColorsOnly = value; }
    }

    public PenX LinePen
    {
      get
      {
        return _linePenGlue.Pen;
      }
      set
      {
        if (value == null)
          throw new ArgumentNullException("FramePen");
        _linePenGlue.Pen = value;
      }
    }

    #endregion Line pen

    public bool IndependentSymbolSize
    {
      get
      {
        return true == _guiIndependentSymbolSize.IsChecked;
      }

      set
      {
        _guiIndependentSymbolSize.IsChecked = value;
      }
    }

    public double SymbolSize
    {
      get
      {
        return _guiSymbolSize.SelectedQuantityAsValueInPoints;
      }
      set
      {
        _guiSymbolSize.SelectedQuantityAsValueInPoints = value;
      }
    }

    public bool UseSymbolGap
    {
      get
      {
        return _guiUseLineSymbolGap.IsChecked == true;
      }

      set
      {
        _guiUseLineSymbolGap.IsChecked = value;
      }
    }

    public double SymbolGapOffset
    {
      get
      {
        return _guiSymbolGapOffset.SelectedQuantityAsValueInPoints;
      }

      set
      {
        _guiSymbolGapOffset.SelectedQuantityAsValueInPoints = value;
      }
    }

    public double SymbolGapFactor
    {
      get
      {
        return _guiSymbolGapFactor.SelectedQuantityAsValueInSIUnits;
      }

      set
      {
        _guiSymbolGapFactor.SelectedQuantityAsValueInSIUnits = value;
      }
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

    #endregion IXYPlotLineStyleView
  }
}
