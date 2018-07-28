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

using Altaxo.Collections;
using Altaxo.Drawing.D3D;
using Altaxo.Graph.Gdi;
using Altaxo.Graph.Graph3D.Plot.Styles;
using Altaxo.Gui.Common.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Altaxo.Gui.Graph.Graph3D.Plot.Styles
{
  /// <summary>
  /// Interaction logic for XYPlotScatterStyleControl.xaml
  /// </summary>
  public partial class ScatterPlotStyleControl : UserControl, IScatterPlotStyleView
  {
    public event Action IndependentColorChanged;

    private bool _enableDisableAll = false;
    private int _suppressEvents = 0;

    public ScatterPlotStyleControl()
    {
      InitializeComponent();
    }

    public void EnableDisableMain(bool bEnable)
    {
      this._chkIndependentColor.IsEnabled = bEnable;
      this._chkIndependentSize.IsEnabled = bEnable;

      this._cbColor.IsEnabled = bEnable;
      this._cbSymbolSize.IsEnabled = bEnable;
      this._edSymbolSkipFrequency.IsEnabled = bEnable;
    }

    #region IXYPlotScatterStyleView

    public void InitializeSymbolStyle(SelectableListNodeList list)
    {
    }

    public void InitializeSymbolShape(SelectableListNodeList list)
    {
      _cbSymbolShape.SelectedItem = list.FirstSelectedNode?.Tag as Altaxo.Graph.Graph3D.Plot.Styles.IScatterSymbol;
    }

    public bool IndependentColor
    {
      get
      {
        return true == _chkIndependentColor.IsChecked;
      }
      set
      {
        this._chkIndependentColor.IsChecked = value;
      }
    }

    public IMaterial SymbolMaterial
    {
      get { return _cbColor.SelectedMaterial; }
      set { _cbColor.SelectedMaterial = value; }
    }

    public IScatterSymbol SymbolShape
    {
      get { return _cbSymbolShape.SelectedItem; }
      set { _cbSymbolShape.SelectedItem = value; }
    }

    public bool IndependentSymbolSize
    {
      get { return true == _chkIndependentSize.IsChecked; }
      set { this._chkIndependentSize.IsChecked = value; }
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
        this._edSymbolSkipFrequency.Value = value;
      }
    }

    public bool IndependentSkipFrequency
    {
      get { return true == _chkIndependentSkipFreq.IsChecked; }
      set { this._chkIndependentSkipFreq.IsChecked = value; }
    }

    #endregion IXYPlotScatterStyleView

    private void EhIndependentColorChanged(object sender, RoutedEventArgs e)
    {
      if (null != IndependentColorChanged)
        IndependentColorChanged();
    }

    public bool ShowPlotColorsOnly
    {
      set
      {
        _cbColor.ShowPlotColorsOnly = value;
      }
    }
  }
}
