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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Altaxo.Collections;
using Altaxo.Drawing;
using Altaxo.Drawing.D3D;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Gdi.Plot.Styles;
using Altaxo.Graph.Graph2D.Plot.Styles;
using Altaxo.Graph.Graph2D.Plot.Styles.ScatterSymbols;
using Altaxo.Gui.Common.Drawing;

namespace Altaxo.Gui.Graph.Gdi.Plot.Styles
{
  /// <summary>
  /// Interaction logic for XYPlotScatterStyleControl.xaml
  /// </summary>
  public partial class ScatterPlotStyleControl : UserControl, IScatterPlotStyleView
  {
    public event Action IndependentColorChanged;

    public event Action ScatterSymbolChanged;

    public event Action CreateNewSymbolSetFromOverrides;

    public event Action SimilarSymbolSetChosen;

    public ScatterPlotStyleControl()
    {
      InitializeComponent();

      var shapeMenuItem = new MenuItem() { Header = "Create new symbol set from overrides", ToolTip = "This will create a new symbol set from the current symbol set and the settings in Overrides" };
      shapeMenuItem.Click += EhCreateNewScatterSymbolSet;
      _cbSymbolShape.ContextMenu.Items.Add(shapeMenuItem);
    }

    private void EhCreateNewScatterSymbolSet(object sender, RoutedEventArgs e)
    {
      CreateNewSymbolSetFromOverrides?.Invoke();
    }

    public void EnableDisableMain(bool bEnable)
    {
      _chkIndependentColor.IsEnabled = bEnable;
      _chkIndependentSize.IsEnabled = bEnable;

      _cbColor.IsEnabled = bEnable;
      _cbSymbolSize.IsEnabled = bEnable;
      _edSymbolSkipFrequency.IsEnabled = bEnable;
    }

    #region IXYPlotScatterStyleView

    public void InitializeSymbolStyle(SelectableListNodeList list)
    {
    }

    public void InitializeSymbolShape(SelectableListNodeList list)
    {
      _cbSymbolShape.SelectedItem = list.FirstSelectedNode?.Tag as IScatterSymbol;
    }

    public SelectableListNodeList SimilarSymbols
    {
      set
      {
        GuiHelper.Initialize(_guiSimilarSymbolSets, value);
      }
    }

    private void EhSimilarSymbolChanged(object sender, SelectionChangedEventArgs e)
    {
      GuiHelper.SynchronizeSelectionFromGui(_guiSimilarSymbolSets);
      SimilarSymbolSetChosen?.Invoke();
    }

    public bool IndependentColor
    {
      get
      {
        return true == _chkIndependentColor.IsChecked;
      }
      set
      {
        _chkIndependentColor.IsChecked = value;
      }
    }

    public NamedColor Color
    {
      get { return _cbColor.SelectedColor; }
      set { _cbColor.SelectedColor = value; }
    }

    public bool IndependentScatterSymbol
    {
      get
      {
        return true == _guiIndependentScatterSymbol.IsChecked;
      }
      set
      {
        _guiIndependentScatterSymbol.IsChecked = value;
      }
    }

    public IScatterSymbol ScatterSymbol
    {
      get { return _cbSymbolShape.SelectedItem; }
      set { _cbSymbolShape.SelectedItem = value; }
    }

    private void EhScatterSymbolChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      ScatterSymbolChanged?.Invoke();
    }

    public SelectableListNodeList Inset
    {
      set
      {
        GuiHelper.Initialize(_guiInset, value);
      }
    }

    private void EhInsetChanged(object sender, SelectionChangedEventArgs e)
    {
      GuiHelper.SynchronizeSelectionFromGui(_guiInset);
    }

    public SelectableListNodeList Frame
    {
      set
      {
        GuiHelper.Initialize(_guiFrame, value);
      }
    }

    private void EhFrameChanged(object sender, SelectionChangedEventArgs e)
    {
      GuiHelper.SynchronizeSelectionFromGui(_guiFrame);
    }

    public bool IndependentSymbolSize
    {
      get { return true == _chkIndependentSize.IsChecked; }
      set { _chkIndependentSize.IsChecked = value; }
    }

    public double SymbolSize
    {
      get { return _cbSymbolSize.SelectedQuantityAsValueInPoints; }
      set { _cbSymbolSize.SelectedQuantityAsValueInPoints = value; }
    }

    public int SkipFrequency
    {
      get
      {
        return _edSymbolSkipFrequency.Value;
      }
      set
      {
        _edSymbolSkipFrequency.Value = value;
      }
    }

    public bool IndependentSkipFrequency
    {
      get { return true == _chkIndependentSkipFreq.IsChecked; }
      set { _chkIndependentSkipFreq.IsChecked = value; }
    }

    public bool IgnoreMissingDataPoints
    {
      get { return true == _guiIgnoreMissingDataPoints.IsChecked; }
      set { _guiIgnoreMissingDataPoints.IsChecked = value; }
    }

    public bool IndependentOnShiftingGroupStyles
    {
      get { return true == _guiIndependentOnShiftingGroupStyles.IsChecked; }
      set { _guiIndependentOnShiftingGroupStyles.IsChecked = value; }
    }

    #endregion IXYPlotScatterStyleView

    private void EhIndependentColorChanged(object sender, RoutedEventArgs e)
    {
      if (IndependentColorChanged is not null)
        IndependentColorChanged();
    }

    public bool ShowPlotColorsOnly
    {
      set
      {
        _cbColor.ShowPlotColorsOnly = value;
      }
    }

    public bool OverrideInset
    {
      get
      {
        return _guiOverrideInset.IsChecked == true;
      }
      set
      {
        _guiOverrideInset.IsChecked = value;
      }
    }

    public bool OverrideFrame
    {
      get
      {
        return _guiOverrideFrame.IsChecked == true;
      }
      set
      {
        _guiOverrideFrame.IsChecked = value;
      }
    }

    public bool OverrideAbsoluteStructureWidth
    {
      get
      {
        return _guiOverrideAbsoluteStructureWidth.IsChecked == true;
      }
      set
      {
        _guiOverrideAbsoluteStructureWidth.IsChecked = value;
      }
    }

    public double OverriddenAbsoluteStructureWidth
    {
      get
      {
        return _guiOverriddenAbsoluteStructureWidth.SelectedQuantityAsValueInPoints;
      }
      set
      {
        _guiOverriddenAbsoluteStructureWidth.SelectedQuantityAsValueInPoints = value;
      }
    }

    public bool OverrideRelativeStructureWidth
    {
      get
      {
        return _guiOverrideRelativeStructureWidth.IsChecked == true;
      }
      set
      {
        _guiOverrideRelativeStructureWidth.IsChecked = value;
      }
    }

    public double OverriddenRelativeStructureWidth
    {
      get
      {
        return _guiOverriddenRelativeStructureWidth.SelectedQuantityAsValueInSIUnits;
      }
      set
      {
        _guiOverriddenRelativeStructureWidth.SelectedQuantityAsValueInSIUnits = value;
      }
    }

    public bool OverridePlotColorInfluence
    {
      get
      {
        return _guiOverridePlotColorInfluence.IsChecked == true;
      }
      set
      {
        _guiOverridePlotColorInfluence.IsChecked = value;
      }
    }

    public PlotColorInfluence OverriddenPlotColorInfluence
    {
      get
      {
        return _guiOverriddenPlotColorInfluence.SelectedValue;
      }
      set
      {
        _guiOverriddenPlotColorInfluence.SelectedValue = value;
      }
    }

    public bool OverrideFillColor
    {
      get
      {
        return _guiOverrideFillColor.IsChecked == true;
      }
      set
      {
        _guiOverrideFillColor.IsChecked = value;
      }
    }

    public NamedColor OverriddenFillColor
    {
      get
      {
        return _guiOverriddenFillColor.SelectedColor;
      }
      set
      {
        _guiOverriddenFillColor.SelectedColor = value;
      }
    }

    public bool OverrideFrameColor
    {
      get
      {
        return _guiOverrideFrameColor.IsChecked == true;
      }
      set
      {
        _guiOverrideFrameColor.IsChecked = value;
      }
    }

    public NamedColor OverriddenFrameColor
    {
      get
      {
        return _guiOverriddenFrameColor.SelectedColor;
      }
      set
      {
        _guiOverriddenFrameColor.SelectedColor = value;
      }
    }

    public bool OverrideInsetColor
    {
      get
      {
        return _guiOverrideInsetColor.IsChecked == true;
      }
      set
      {
        _guiOverrideInsetColor.IsChecked = value;
      }
    }

    public NamedColor OverriddenInsetColor
    {
      get
      {
        return _guiOverriddenInsetColor.SelectedColor;
      }
      set
      {
        _guiOverriddenInsetColor.SelectedColor = value;
      }
    }
  }
}
