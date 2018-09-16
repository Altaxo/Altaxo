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
using Altaxo.Gui.Graph.Gdi.Plot.ColorProvider;
using Altaxo.Gui.Graph.Plot.Data;
using Altaxo.Gui.Graph.Scales;

namespace Altaxo.Gui.Graph.Gdi.Plot.Styles
{
  /// <summary>
  /// Interaction logic for ColumnDrivenColorPlotStyleControl.xaml
  /// </summary>
  public partial class ColumnDrivenColorPlotStyleControl : UserControl, IColumnDrivenColorPlotStyleView
  {
    public ColumnDrivenColorPlotStyleControl()
    {
      InitializeComponent();
    }

    public void Init_DataColumn(string boxText, string toolTip, int status)
    {
      _guiDataColumn.Text = boxText;
      _guiDataColumn.ToolTip = toolTip;
      _guiDataColumn.Background = DefaultSeverityColumnColors.GetSeverityColor(status);
    }

    public void Init_DataColumnTransformation(string boxText, string toolTip)
    {
      if (null == boxText)
      {
        _guiDataColumnTransformation.Visibility = Visibility.Collapsed;
      }
      else
      {
        _guiDataColumnTransformation.Text = boxText;
        _guiDataColumnTransformation.ToolTip = toolTip;
        _guiDataColumnTransformation.Visibility = Visibility.Visible;
      }
    }

    #region IColumnDrivenColorPlotStyleView

    public IDensityScaleView ScaleView
    {
      get { return _ctrlScale; }
    }

    public IColorProviderView ColorProviderView
    {
      get { return _colorProviderControl; }
    }

    #endregion IColumnDrivenColorPlotStyleView
  }
}
