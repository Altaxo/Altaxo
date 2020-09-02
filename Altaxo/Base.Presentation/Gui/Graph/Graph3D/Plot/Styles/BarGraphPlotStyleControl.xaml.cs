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
using Altaxo.Drawing.D3D;
using Altaxo.Gui.Drawing.D3D;

namespace Altaxo.Gui.Graph.Graph3D.Plot.Styles
{
  /// <summary>
  /// Interaction logic for BarGraphPlotStyleControl.xaml
  /// </summary>
  public partial class BarGraphPlotStyleControl : UserControl, IBarGraphPlotStyleView
  {
    private PenControlsGlue _penGlue;

    public event Action IndependentColorChanged;

    public event Action BarShiftStrategyChanged;

    public BarGraphPlotStyleControl()
    {
      InitializeComponent();
      _penGlue = new PenControlsGlue(false)
      {
        CbBrush = _guiPenMaterial,
        CbCrossSection = _guiPenCrossSection
      };
    }

    private void EhIndependentColorChanged(object sender, RoutedEventArgs e)
    {
      IndependentColorChanged?.Invoke();
    }

    private void EhUsePreviousItem_CheckedChanged(object sender, RoutedEventArgs e)
    {
      bool usePrevItem = true == _guiUsePreviousItem.IsChecked;
      _guiGapV.IsEnabled = usePrevItem;
    }

    public bool IndependentColor
    {
      get
      {
        return true == _guiIndependentColor.IsChecked;
      }
      set
      {
        _guiIndependentColor.IsChecked = value;
      }
    }

    public PenX3D Pen
    {
      get
      {
        return _penGlue.Pen;
      }
      set
      {
        if (value is null)
          throw new ArgumentNullException(nameof(value));

        _penGlue.Pen = value;
      }
    }

    public bool UseUniformCrossSectionThickness
    {
      get
      {
        return _guiUseUniformPenThickness.IsChecked == true;
      }
      set
      {
        _guiUseUniformPenThickness.IsChecked = value;
      }
    }

    public SelectableListNodeList BarShiftStrategy
    {
      set
      {
        GuiHelper.Initialize(_guiBarShiftStrategy, value);
      }
    }

    public bool IsEnabledNumberOfBarsInOneDirection { set { _guiMaxNumberOfItems.IsEnabled = value; } }

    public int BarShiftMaxItemsInOneDirection { get { return _guiMaxNumberOfItems.Value; } set { _guiMaxNumberOfItems.Value = value; } }

    public double InnerGapX
    {
      get
      {
        return _guiInnerGapX.SelectedQuantityAsValueInSIUnits;
      }
      set
      {
        _guiInnerGapX.SelectedQuantityAsValueInSIUnits = value;
      }
    }

    public double OuterGapX
    {
      get
      {
        return _guiOuterGapX.SelectedQuantityAsValueInSIUnits;
      }
      set
      {
        _guiOuterGapX.SelectedQuantityAsValueInSIUnits = value;
      }
    }

    public double InnerGapY
    {
      get
      {
        return _guiInnerGapY.SelectedQuantityAsValueInSIUnits;
      }
      set
      {
        _guiInnerGapY.SelectedQuantityAsValueInSIUnits = value;
      }
    }

    public double OuterGapY
    {
      get
      {
        return _guiOuterGapY.SelectedQuantityAsValueInSIUnits;
      }
      set
      {
        _guiOuterGapY.SelectedQuantityAsValueInSIUnits = value;
      }
    }

    public bool UsePhysicalBaseValueV
    {
      get
      {
        return false;
      }
      set
      {
      }
    }

    public double LogicalBaseValueV
    {
      get
      {
        return _guiLogicalBaseValueV.SelectedQuantityAsValueInSIUnits;
      }
      set
      {
        _guiLogicalBaseValueV.SelectedQuantityAsValueInSIUnits = value;
      }
    }

    public bool StartAtPreviousItem
    {
      get
      {
        return true == _guiUsePreviousItem.IsChecked;
      }
      set
      {
        _guiUsePreviousItem.IsChecked = value;
        _guiGapV.IsEnabled = value;
      }
    }

    public double GapV
    {
      get
      {
        return _guiGapV.SelectedQuantityAsValueInSIUnits;
      }
      set
      {
        _guiGapV.SelectedQuantityAsValueInSIUnits = value;
      }
    }

    public bool ShowPlotColorsOnly
    {
      set { _penGlue.ShowPlotColorsOnly = value; }
    }

    private void EhBarShiftStrategyChanged(object sender, SelectionChangedEventArgs e)
    {
      GuiHelper.SynchronizeSelectionFromGui(_guiBarShiftStrategy);
      BarShiftStrategyChanged?.Invoke();
    }
  }
}
