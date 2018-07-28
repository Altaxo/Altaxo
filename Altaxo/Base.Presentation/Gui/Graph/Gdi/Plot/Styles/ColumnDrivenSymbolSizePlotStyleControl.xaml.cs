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

using Altaxo.Gui.Graph.Plot.Data;
using Altaxo.Gui.Graph.Scales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Altaxo.Gui.Graph.Gdi.Plot.Styles
{
  /// <summary>
  /// Interaction logic for ColumnDrivenColorPlotStyleControl.xaml
  /// </summary>
  public partial class ColumnDrivenSymbolSizePlotStyleControl : UserControl, IColumnDrivenSymbolSizePlotStyleView
  {
    public ColumnDrivenSymbolSizePlotStyleControl()
    {
      InitializeComponent();
    }

    public void Init_DataColumn(string boxText, string toolTip, int status)
    {
      this._guiDataColumn.Text = boxText;
      this._guiDataColumn.ToolTip = toolTip;
      this._guiDataColumn.Background = DefaultSeverityColumnColors.GetSeverityColor(status);
    }

    public void Init_DataColumnTransformation(string boxText, string toolTip)
    {
      if (null == boxText)
      {
        this._guiDataColumnTransformation.Visibility = Visibility.Collapsed;
      }
      else
      {
        this._guiDataColumnTransformation.Text = boxText;
        this._guiDataColumnTransformation.ToolTip = toolTip;
        this._guiDataColumnTransformation.Visibility = Visibility.Visible;
      }
    }

    #region IColumnDrivenSymbolSizePlotStyleView

    public IDensityScaleView ScaleView
    {
      get { return _ctrlScale; }
    }

    public double SymbolSizeAt0
    {
      get
      {
        return _cbSymbolSizeAt0.SelectedQuantityAsValueInPoints;
      }
      set
      {
        _cbSymbolSizeAt0.SelectedQuantityAsValueInPoints = value;
      }
    }

    public double SymbolSizeAt1
    {
      get
      {
        return _cbSymbolSizeAt1.SelectedQuantityAsValueInPoints;
      }
      set
      {
        _cbSymbolSizeAt1.SelectedQuantityAsValueInPoints = value;
      }
    }

    public double SymbolSizeAbove
    {
      get
      {
        return _cbSymbolSizeAbove.SelectedQuantityAsValueInPoints;
      }
      set
      {
        _cbSymbolSizeAbove.SelectedQuantityAsValueInPoints = value;
      }
    }

    public double SymbolSizeBelow
    {
      get
      {
        return _cbSymbolSizeBelow.SelectedQuantityAsValueInPoints;
      }
      set
      {
        _cbSymbolSizeBelow.SelectedQuantityAsValueInPoints = value;
      }
    }

    public double SymbolSizeInvalid
    {
      get
      {
        return _cbSymbolSizeInvalid.SelectedQuantityAsValueInPoints;
      }
      set
      {
        _cbSymbolSizeInvalid.SelectedQuantityAsValueInPoints = value;
      }
    }

    public int NumberOfSteps
    {
      get
      {
        return (int)_edNumberOfSteps.Value;
      }
      set
      {
        _edNumberOfSteps.Value = value;
      }
    }

    #endregion IColumnDrivenSymbolSizePlotStyleView
  }
}
