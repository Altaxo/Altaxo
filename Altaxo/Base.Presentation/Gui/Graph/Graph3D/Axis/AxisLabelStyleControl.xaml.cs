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

namespace Altaxo.Gui.Graph.Graph3D.Axis
{
  using Altaxo.Drawing.D3D;
  using Altaxo.Graph.Graph3D;
  using Altaxo.Graph.Graph3D.Background;
  using Background;
  using Drawing.D3D;

  /// <summary>
  /// Interaction logic for XYAxisLabelStyleControl.xaml
  /// </summary>
  public partial class AxisLabelStyleControl : UserControl, IAxisLabelStyleView
  {
    private BackgroundControlsGlue _backgroundGlue;
    private FontX3DGlue _fontGlue;

    public AxisLabelStyleControl()
    {
      InitializeComponent();
      _backgroundGlue = new BackgroundControlsGlue();
      _backgroundGlue.CbBrush = _guiBackgroundBrush;
      _backgroundGlue.CbStyle = _guiBackgroundStyle;

      _fontGlue = new FontX3DGlue();
      _fontGlue.GuiFontFamily = _guiFontFamily;
      _fontGlue.GuiFontStyle = _guiFontStyle;
      _fontGlue.GuiFontSize = _guiFontSize;
    }

    public event Action LabelStyleChanged;

    private void m_cbLabelStyle_SelectionChangeCommitted(object sender, SelectionChangedEventArgs e)
    {
      e.Handled = true;
      GuiHelper.SynchronizeSelectionFromGui(_guiLabelStyle);
      if (null != LabelStyleChanged)
        LabelStyleChanged();
    }

    #region IXYAxisLabelStyleView

    public FontX3D LabelFont
    {
      get
      {
        return _fontGlue.SelectedFont;
      }
      set
      {
        _fontGlue.SelectedFont = value;
      }
    }

    public IMaterial LabelMaterial
    {
      get
      {
        return _guiLabelMaterial.SelectedMaterial;
      }
      set
      {
        _guiLabelMaterial.SelectedMaterial = value;
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

    public void AlignmentX_Initialize(Collections.SelectableListNodeList items)
    {
      GuiHelper.Initialize(_guiAlignmentX, items);
    }

    public void AlignmentY_Initialize(Collections.SelectableListNodeList items)
    {
      GuiHelper.Initialize(_guiAlignmentY, items);
    }

    public void AlignmentZ_Initialize(Collections.SelectableListNodeList items)
    {
      GuiHelper.Initialize(_guiAlignmentZ, items);
    }

    public bool AutomaticAlignment
    {
      get
      {
        return true == _guiAutomaticAlignment.IsChecked;
      }
      set
      {
        _guiAutomaticAlignment.IsChecked = value;
        this._guiAlignmentX.IsEnabled = false == _guiAutomaticAlignment.IsChecked;
        this._guiAlignmentY.IsEnabled = false == _guiAutomaticAlignment.IsChecked;
      }
    }

    public double RotationX
    {
      get
      {
        return this._guiRotationX.SelectedQuantityAsValueInDegrees;
      }
      set
      {
        this._guiRotationX.SelectedQuantityAsValueInDegrees = value;
      }
    }

    public double RotationY
    {
      get
      {
        return this._guiRotationY.SelectedQuantityAsValueInDegrees;
      }
      set
      {
        this._guiRotationY.SelectedQuantityAsValueInDegrees = value;
      }
    }

    public double RotationZ
    {
      get
      {
        return this._guiRotationZ.SelectedQuantityAsValueInDegrees;
      }
      set
      {
        this._guiRotationZ.SelectedQuantityAsValueInDegrees = value;
      }
    }

    public double OffsetX
    {
      get
      {
        return this._guiOffsetX.SelectedQuantityAsValueInSIUnits;
      }
      set
      {
        this._guiOffsetX.SelectedQuantityAsValueInSIUnits = value;
      }
    }

    public double OffsetY
    {
      get
      {
        return _guiOffsetY.SelectedQuantityAsValueInSIUnits;
      }
      set
      {
        this._guiOffsetY.SelectedQuantityAsValueInSIUnits = value;
      }
    }

    public double OffsetZ
    {
      get
      {
        return _guiOffsetZ.SelectedQuantityAsValueInSIUnits;
      }
      set
      {
        this._guiOffsetZ.SelectedQuantityAsValueInSIUnits = value;
      }
    }

    public void LabelStyle_Initialize(Collections.SelectableListNodeList items)
    {
      GuiHelper.Initialize(this._guiLabelStyle, items);
    }

    public string SuppressedLabelsByValue
    {
      get { return _guiSuppressLabelValues.Text; }
      set { _guiSuppressLabelValues.Text = value; }
    }

    public string SuppressedLabelsByNumber
    {
      get { return _guiSuppressLabelsByNumber.Text; }
      set { _guiSuppressLabelsByNumber.Text = value; }
    }

    public string PrefixText
    {
      get { return _guiPrefixText.Text; }
      set { _guiPrefixText.Text = value; }
    }

    public string PostfixText
    {
      get { return _guiPostfixText.Text; }
      set { _guiPostfixText.Text = value; }
    }

    public Collections.SelectableListNodeList LabelSides
    {
      set
      {
        _guiLabelSide.ItemsSource = value;
      }
    }

    /// <summary>Sets the label formatting specific GUI control. If no specific options are available, this property is set to <c>null</c>.</summary>
    /// <value>The label formatting specific GUI control.</value>
    public object LabelFormattingSpecificGuiControl
    {
      set
      {
        _guiLabelStyleSpecific.Child = value as UIElement;
      }
    }

    #endregion IXYAxisLabelStyleView

    public EventHandler EhSelectedFontChanged { get; set; }

    private void _chkAutomaticAlignment_CheckedChanged(object sender, RoutedEventArgs e)
    {
      _guiAlignmentX.IsEnabled = false == _guiAutomaticAlignment.IsChecked;
      _guiAlignmentY.IsEnabled = false == _guiAutomaticAlignment.IsChecked;
      _guiAlignmentZ.IsEnabled = false == _guiAutomaticAlignment.IsChecked;
    }

    private void EhAlignmentX_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      GuiHelper.SynchronizeSelectionFromGui(_guiAlignmentX);
    }

    private void EhAlignmentY_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      GuiHelper.SynchronizeSelectionFromGui(_guiAlignmentY);
    }

    private void EhAlignmentZ_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      GuiHelper.SynchronizeSelectionFromGui(_guiAlignmentZ);
    }
  }
}
