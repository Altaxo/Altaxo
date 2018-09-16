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
using Altaxo.Graph.Gdi;
using Altaxo.Gui.Common.Drawing;
using Altaxo.Gui.Graph.Graph3D.Plot.Data;
using Altaxo.Gui.Graph.Plot.Data;

namespace Altaxo.Gui.Graph.Gdi.Plot.Styles
{
  /// <summary>
  /// Interaction logic for ErrorBarPlotStyleControl.xaml
  /// </summary>
  public partial class VectorCartesicPlotStyleControl : UserControl, IVectorCartesicPlotStyleView
  {
    private PenControlsGlue _strokePenGlue;

    public event Action IndependentColorChanged;

    public VectorCartesicPlotStyleControl()
    {
      InitializeComponent();

      _strokePenGlue = new PenControlsGlue
      {
        CbBrush = _guiPenColor,
        CbEndCap = _guiLineEndCap
      };
    }

    #region IErrorBarPlotStyleView Members

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

    public PenX Pen
    {
      get
      {
        return _strokePenGlue.Pen;
      }
      set
      {
        _strokePenGlue.Pen = value;
      }
    }

    public bool IgnoreMissingDataPoints
    {
      get { return true == _guiIgnoreMissingDataPoints.IsChecked; }
      set { _guiIgnoreMissingDataPoints.IsChecked = value; }
    }

    public bool IndependentOnShiftingGroupStyles
    {
      get
      {
        return true == _guiIndependentOnShiftingGroupStyles.IsChecked;
      }
      set
      {
        _guiIndependentOnShiftingGroupStyles.IsChecked = value;
      }
    }

    public int SkipFrequency
    {
      get
      {
        return _edSkipFrequency.Value;
      }
      set
      {
        _edSkipFrequency.Value = value;
      }
    }

    #endregion IErrorBarPlotStyleView Members

    public bool ShowPlotColorsOnly
    {
      set { _strokePenGlue.ShowPlotColorsOnly = value; }
    }

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

    public double LineWidth1Offset
    {
      get
      {
        return _guiLineWidth1Offset.SelectedQuantityAsValueInPoints;
      }

      set
      {
        _guiLineWidth1Offset.SelectedQuantityAsValueInPoints = value;
      }
    }

    public double LineWidth1Factor
    {
      get
      {
        return _guiLineWidth1Factor.SelectedQuantityAsValueInSIUnits;
      }

      set
      {
        _guiLineWidth1Factor.SelectedQuantityAsValueInSIUnits = value;
      }
    }

    public double EndCapSizeOffset
    {
      get
      {
        return _guiEndCapSizeOffset.SelectedQuantityAsValueInPoints;
      }

      set
      {
        _guiEndCapSizeOffset.SelectedQuantityAsValueInPoints = value;
      }
    }

    public double EndCapSizeFactor
    {
      get
      {
        return _guiEndCapSizeFactor.SelectedQuantityAsValueInSIUnits;
      }

      set
      {
        _guiEndCapSizeFactor.SelectedQuantityAsValueInSIUnits = value;
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

    public bool UseManualVectorLength
    {
      get
      {
        return _guiUseManualVectorLength.IsChecked == true;
      }

      set
      {
        _guiUseManualVectorLength.IsChecked = value;
      }
    }

    public double VectorLengthOffset
    {
      get
      {
        return _guiVectorLengthOffset.SelectedQuantityAsValueInPoints;
      }

      set
      {
        _guiVectorLengthOffset.SelectedQuantityAsValueInPoints = value;
      }
    }

    public double VectorLengthFactor
    {
      get
      {
        return _guiVectorLengthFactor.SelectedQuantityAsValueInSIUnits;
      }

      set
      {
        _guiVectorLengthFactor.SelectedQuantityAsValueInSIUnits = value;
      }
    }

    public bool IndependentSkipFrequency
    {
      get
      {
        return _guiIndependentSkipFrequency.IsChecked == true;
      }

      set
      {
        _guiIndependentSkipFrequency.IsChecked = value;
      }
    }

    private void EhIndependentColorChanged(object sender, RoutedEventArgs e)
    {
      if (null != IndependentColorChanged)
        IndependentColorChanged();
    }

    public void Initialize_ColumnX(string boxText, string toolTip, int status)
    {
      _guiColumnX.Text = boxText;
      _guiColumnX.ToolTip = toolTip;
      _guiColumnX.Background = DefaultSeverityColumnColors.GetSeverityColor(status);
    }

    public void Initialize_ColumnXTransformation(string transformationTextToShow, string transformationToolTip)
    {
      if (null == transformationTextToShow)
      {
        _guiColumnXTransformation.Visibility = Visibility.Collapsed;
      }
      else
      {
        _guiColumnXTransformation.Text = transformationTextToShow;
        _guiColumnXTransformation.ToolTip = transformationToolTip;
        _guiColumnXTransformation.Visibility = Visibility.Visible;
      }
    }

    public void Initialize_ColumnY(string boxText, string toolTip, int status)
    {
      _guiColumnY.Text = boxText;
      _guiColumnY.ToolTip = toolTip;
      _guiColumnY.Background = DefaultSeverityColumnColors.GetSeverityColor(status);
    }

    public void Initialize_ColumnYTransformation(string transformationTextToShow, string transformationToolTip)
    {
      if (null == transformationTextToShow)
      {
        _guiColumnYTransformation.Visibility = Visibility.Collapsed;
      }
      else
      {
        _guiColumnYTransformation.Text = transformationTextToShow;
        _guiColumnYTransformation.ToolTip = transformationToolTip;
        _guiColumnYTransformation.Visibility = Visibility.Visible;
      }
    }

    public void Initialize_MeaningOfValues(SelectableListNodeList list)
    {
      _guiMeaningOfValues.Initialize(list);
    }
  }
}
