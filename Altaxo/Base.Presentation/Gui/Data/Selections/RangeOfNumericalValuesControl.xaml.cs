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
using Altaxo.Gui.Common;
using Altaxo.Gui.Graph.Plot.Data;

namespace Altaxo.Gui.Data.Selections
{
  /// <summary>
  /// Interaction logic for RangeOfPhysicalValuesControl.xaml
  /// </summary>
  public partial class RangeOfNumericalValuesControl : UserControl, IRangeOfNumericalValuesView
  {
    public RangeOfNumericalValuesControl()
    {
      InitializeComponent();
    }

    public void Init_Column(string boxText, string toolTip, int status)
    {
      _guiColumn.Text = boxText;
      _guiColumn.ToolTip = toolTip;
      _guiColumn.Background = DefaultSeverityColumnColors.GetSeverityColor(status);
    }

    public void Init_ColumnTransformation(string boxText, string toolTip)
    {
      if (boxText is null)
      {
        _guiColumnTransformation.Visibility = Visibility.Collapsed;
      }
      else
      {
        _guiColumnTransformation.Text = boxText;
        _guiColumnTransformation.ToolTip = toolTip;
        _guiColumnTransformation.Visibility = Visibility.Visible;
      }
    }

    public void Init_LowerInclusive(Altaxo.Collections.SelectableListNodeList list)
    {
      GuiHelper.Initialize(_guiLowerInclusive, list);
    }

    public void Init_UpperInclusive(Altaxo.Collections.SelectableListNodeList list)
    {
      GuiHelper.Initialize(_guiUpperInclusive, list);
    }

    public void Init_Index(int idx)
    {
      _guiDataLabel.Content = string.Format("Col#{0}:", idx);
    }

    public double LowerValue
    {
      get { return _guiFromValue.Value; }
      set
      {
        _guiFromValue.Value = value;
      }
    }

    public double UpperValue
    {
      get { return _guiToValue.Value; }
      set
      {
        _guiToValue.Value = value;
      }
    }

    private void EhLowerInclusiveChanged(object sender, SelectionChangedEventArgs e)
    {
      GuiHelper.SynchronizeSelectionFromGui(_guiLowerInclusive);
    }

    private void EhUpperInclusiveChanged(object sender, SelectionChangedEventArgs e)
    {
      GuiHelper.SynchronizeSelectionFromGui(_guiUpperInclusive);
    }
  }
}
