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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Altaxo.Graph.Graph2D.Plot.Styles.ScatterSymbols;

namespace Altaxo.Gui.Graph.Graph2D.Plot.Styles.ScatterSymbols
{
  /// <summary>
  /// Interaction logic for PlotColorInfluenceControl.xaml
  /// </summary>
  public partial class PlotColorInfluenceControl : UserControl
  {
    public event EventHandler? SelectedValueChanged;

    public PlotColorInfluenceControl()
    {
      InitializeComponent();
    }

    public PlotColorInfluence SelectedValue
    {
      get
      {
        PlotColorInfluence result = PlotColorInfluence.None;

        if (_guiFillAlpha.IsChecked == true)
          result |= PlotColorInfluence.FillColorPreserveAlpha;
        if (_guiFillFull.IsChecked == true)
          result |= PlotColorInfluence.FillColorFull;

        if (_guiFrameAlpha.IsChecked == true)
          result |= PlotColorInfluence.FrameColorPreserveAlpha;
        if (_guiFrameFull.IsChecked == true)
          result |= PlotColorInfluence.FrameColorFull;

        if (_guiInsetAlpha.IsChecked == true)
          result |= PlotColorInfluence.InsetColorPreserveAlpha;
        if (_guiInsetFull.IsChecked == true)
          result |= PlotColorInfluence.InsetColorFull;

        return result;
      }
      set
      {
        if (value.HasFlag(PlotColorInfluence.FillColorFull))
          _guiFillFull.IsChecked = true;
        else if (value.HasFlag(PlotColorInfluence.FillColorPreserveAlpha))
          _guiFillAlpha.IsChecked = true;
        else
          _guiFillNone.IsChecked = true;

        if (value.HasFlag(PlotColorInfluence.FrameColorFull))
          _guiFrameFull.IsChecked = true;
        else if (value.HasFlag(PlotColorInfluence.FrameColorPreserveAlpha))
          _guiFrameAlpha.IsChecked = true;
        else
          _guiFrameNone.IsChecked = true;

        if (value.HasFlag(PlotColorInfluence.InsetColorFull))
          _guiInsetFull.IsChecked = true;
        else if (value.HasFlag(PlotColorInfluence.InsetColorPreserveAlpha))
          _guiInsetAlpha.IsChecked = true;
        else
          _guiInsetNone.IsChecked = true;
      }
    }

    private void EhSelectedValueChanged(object sender, RoutedEventArgs e)
    {
      SelectedValueChanged?.Invoke(this, EventArgs.Empty);
    }
  }
}
