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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Altaxo.Drawing;
using Altaxo.Graph;
using Altaxo.Graph.Gdi.Background;
using Altaxo.Gui.Drawing;
using Altaxo.Gui.Graph.Gdi.Background;
using Altaxo.Gui.Graph.Plot.Data;
using Altaxo.Units;

namespace Altaxo.Gui.Graph.Gdi.Plot.Styles
{
  /// <summary>
  /// Interaction logic for XYPlotLabelStyleControl.xaml
  /// </summary>
  public partial class LabelPlotStyleControl : UserControl, ILabelPlotStyleView
  {
    private FontXControlsGlue _fontControlsGlue;

    private BackgroundControlsGlue _backgroundGlue;

    public event Action? LabelColumnSelected;

    public event Action? LabelColorLinkageChanged;

    public event Action? BackgroundColorLinkageChanged;

    public event Action? LabelBrushChanged;

    public event Action? BackgroundBrushChanged;

    public event Action? UseBackgroundChanged;

    public LabelPlotStyleControl()
    {
      InitializeComponent();

      DefaultSeverityColumnColors.NormalColor = _guiLabelColumn.Background;

      _fontControlsGlue = new FontXControlsGlue() { CbFontFamily = _cbFontFamily, CbFontStyle = _cbFontStyle };
      _backgroundGlue = new BackgroundControlsGlue() { CbStyle = _cbBackgroundStyle, CbBrush = _cbBackgroundBrush };
      _backgroundGlue.BackgroundStyleChanged += EhBackgroundStyleInstanceChanged;
      _backgroundGlue.BackgroundBrushChanged += EhBackgroundBrushChanged;
    }

    private void EhSelectLabelColumn_Click(object sender, RoutedEventArgs e)
    {
      if (LabelColumnSelected is not null)
        LabelColumnSelected();
    }

    private void EhIndependentColor_CheckChanged(object sender, RoutedEventArgs e)
    {
      if (LabelColorLinkageChanged is not null)
        LabelColorLinkageChanged();
    }

    private void EhAttachToAxis_CheckedChanged(object sender, RoutedEventArgs e)
    {
      _guiAttachedAxis.IsEnabled = true == _guiAttachToAxis.IsChecked;
    }

    #region IXYPlotLabelStyleView

    public void Init_LabelColumn(string boxText, string toolTip, int status)
    {
      _guiLabelColumn.Text = boxText;
      _guiLabelColumn.ToolTip = toolTip;
      _guiLabelColumn.Background = DefaultSeverityColumnColors.GetSeverityColor(status);
    }

    public void Init_Transformation(string boxText, string toolTip)
    {
      if (boxText is null)
      {
        _guiLabelTransformation.Visibility = Visibility.Collapsed;
      }
      else
      {
        _guiLabelTransformation.Text = boxText;
        _guiLabelTransformation.ToolTip = toolTip;
        _guiLabelTransformation.Visibility = Visibility.Visible;
      }
    }

    public string LabelFormatString
    {
      get
      {
        return _guiLabelFormat.Text;
      }
      set
      {
        _guiLabelFormat.Text = value;
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

    public int SkipFrequency
    {
      get
      {
        return _guiSkipFrequency.Value;
      }
      set
      {
        _guiSkipFrequency.Value = value;
      }
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

    public new IBackgroundStyle Background
    {
      get
      {
        return _backgroundGlue.BackgroundStyle;
      }
      set
      {
        _backgroundGlue.BackgroundStyle = value;
      }
    }

    public double SelectedRotation
    {
      get
      {
        return _guiRotation.SelectedQuantityAsValueInDegrees;
      }
      set
      {
        _guiRotation.SelectedQuantityAsValueInDegrees = value;
      }
    }

    public double OffsetXPoints
    {
      get
      {
        return _guiOffsetXPoints.SelectedQuantityAsValueInPoints;
      }

      set
      {
        _guiOffsetXPoints.SelectedQuantityAsValueInPoints = value;
      }
    }

    public double OffsetXEmUnits
    {
      get
      {
        return _guiOffsetXEmUnits.SelectedQuantityAsValueInSIUnits;
      }

      set
      {
        _guiOffsetXEmUnits.SelectedQuantityAsValueInSIUnits = value;
      }
    }

    public double OffsetXSymbolSizeUnits
    {
      get
      {
        return _guiOffsetXSymbolSizeUnits.SelectedQuantityAsValueInSIUnits;
      }

      set
      {
        _guiOffsetXSymbolSizeUnits.SelectedQuantityAsValueInSIUnits = value;
      }
    }

    public double OffsetYPoints
    {
      get
      {
        return _guiOffsetYPoints.SelectedQuantityAsValueInPoints;
      }

      set
      {
        _guiOffsetYPoints.SelectedQuantityAsValueInPoints = value;
      }
    }

    public double OffsetYEmUnits
    {
      get
      {
        return _guiOffsetYEmUnits.SelectedQuantityAsValueInSIUnits;
      }

      set
      {
        _guiOffsetYEmUnits.SelectedQuantityAsValueInSIUnits = value;
      }
    }

    public double OffsetYSymbolSizeUnits
    {
      get
      {
        return _guiOffsetYSymbolSizeUnits.SelectedQuantityAsValueInSIUnits;
      }

      set
      {
        _guiOffsetYSymbolSizeUnits.SelectedQuantityAsValueInSIUnits = value;
      }
    }

    public double FontSizeOffset
    {
      get
      {
        return _guiFontSizeOffset.SelectedQuantityAsValueInPoints;
      }

      set
      {
        _guiFontSizeOffset.SelectedQuantityAsValueInPoints = value;
      }
    }

    public double FontSizeFactor
    {
      get
      {
        return _guiFontSizeFactor.SelectedQuantityAsValueInSIUnits;
      }

      set
      {
        _guiFontSizeFactor.SelectedQuantityAsValueInSIUnits = value;
      }
    }

    public FontX SelectedFont
    {
      get
      {
        return _fontControlsGlue.SelectedFont;
      }
      set
      {
        _fontControlsGlue.SelectedFont = value;
      }
    }

    public BrushX LabelBrush
    {
      get
      {
        return _guiLabelBrush.SelectedBrush;
      }
      set
      {
        _guiLabelBrush.SelectedBrush = value;
      }
    }

    public void Init_AlignmentX(Collections.SelectableListNodeList list)
    {
      GuiHelper.Initialize(_guiAlignmentX, list);
    }

    public void Init_AlignmentY(Collections.SelectableListNodeList list)
    {
      GuiHelper.Initialize(_guiAlignmentY, list);
    }

    public bool AttachToAxis
    {
      get
      {
        return true == _guiAttachToAxis.IsChecked;
      }
      set
      {
        _guiAttachToAxis.IsChecked = value;
        _guiAttachedAxis.IsEnabled = value;
      }
    }

    public void Init_AttachedAxis(Collections.SelectableListNodeList names)
    {
      GuiHelper.Initialize(_guiAttachedAxis, names);
    }

    public bool IndependentColor
    {
      get
      {
        return true == _guiIndependentLabelColor.IsChecked;
      }
      set
      {
        _guiIndependentLabelColor.IsChecked = value;
      }
    }

    public bool ShowPlotColorsOnly
    {
      set
      {
        _guiLabelBrush.ShowPlotColorsOnly = value;
      }
    }

    #endregion IXYPlotLabelStyleView

    private void EhAlignmentXChanged(object sender, SelectionChangedEventArgs e)
    {
      GuiHelper.SynchronizeSelectionFromGui(_guiAlignmentX);
    }

    private void EhAlignmentYChanged(object sender, SelectionChangedEventArgs e)
    {
      GuiHelper.SynchronizeSelectionFromGui(_guiAlignmentY);
    }

    private void EhAttachedAxisChanged(object sender, SelectionChangedEventArgs e)
    {
      GuiHelper.SynchronizeSelectionFromGui(_guiAttachedAxis);
    }

    private void EhLabelBrushChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if (LabelBrushChanged is not null)
        LabelBrushChanged();
    }

    private void EhBackgroundBrushChanged(object? sender, EventArgs e)
    {
      if (BackgroundBrushChanged is not null)
        BackgroundBrushChanged();
    }

    public void InitializeBackgroundColorLinkage(Collections.SelectableListNodeList list)
    {
      _guiBackgroundColorLinkage.Initialize(list);
    }

    public bool ShowPlotColorsOnlyForBackgroundBrush
    {
      set { _backgroundGlue.ShowPlotColorsOnly = value; }
    }

    private void EhBackgroundStyleInstanceChanged(object? sender, EventArgs e)
    {
      if (UseBackgroundChanged is not null)
        UseBackgroundChanged();
    }

    private void EhBackgroundColorLinkageChanged(object? sender, EventArgs e)
    {
          BackgroundColorLinkageChanged?.Invoke();
    }
  }
}
