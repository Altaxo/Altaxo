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
using Altaxo.Graph.Gdi;
using Altaxo.Gui.Common.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Altaxo.Gui.Graph.Gdi.Plot.Styles
{
  /// <summary>
  /// Interaction logic for XYPlotScatterStyleControl.xaml
  /// </summary>
  public partial class DropLinePlotStyleControl : UserControl, IDropLinePlotStyleView
  {
    public event Action IndependentColorChanged;

    private PenControlsGlue _penGlue;

    public DropLinePlotStyleControl()
    {
      InitializeComponent();

      _penGlue = new PenControlsGlue();
      _penGlue.CbBrush = _guiPenMaterial;
      _penGlue.CbDashPattern = _guiPenDashStyle;

      _penGlue.CbStartCap = _cbLineStartCap;
      _penGlue.CbStartCapAbsSize = _cbLineStartCapSize;
      _penGlue.CbStartCapRelSize = _edLineStartCapRelSize;

      _penGlue.CbEndCap = _cbLineEndCap;
      _penGlue.CbEndCapAbsSize = _cbLineEndCapSize;
      _penGlue.CbEndCapRelSize = _edLineEndCapRelSize;
    }

    public bool AdditionalDropTargetIsEnabled
    {
      get
      {
        return _guiEnableUserDefinedDropTarget.IsChecked == true;
      }
      set
      {
        _guiEnableUserDefinedDropTarget.IsChecked = value;
      }
    }

    public int AdditionalDropTargetPerpendicularAxisNumber
    {
      get
      {
        return _guiUserDefinedDropTargetAxis.Value;
      }
      set
      {
        _guiUserDefinedDropTargetAxis.Value = value;
      }
    }

    /// <summary>
    /// Indicates whether _baseValue is a physical value or a logical value.
    /// </summary>
    public bool AdditionalDropTargetUsePhysicalBaseValue
    {
      get
      {
        return _guiUserDefinedUsePhysicalBaseValue.IsChecked == true;
      }
      set
      {
        _guiUserDefinedUsePhysicalBaseValue.IsChecked = value;
      }
    }

    /// <summary>
    /// The y-value where the item normally starts. This is either a logical value (_usePhysicalBaseValue==false) or a physical value.
    /// </summary>
    public Altaxo.Data.AltaxoVariant AdditionalDropTargetBaseValue
    {
      get
      {
        return _guiUserDefinedBaseValue.SelectedValue;
      }
      set
      {
        _guiUserDefinedBaseValue.SelectedValue = value;
      }
    }

    public void InitializeDropLineConditions(SelectableListNodeList names)
    {
      _guiDropLines.Initialize(names);
    }

    public bool IndependentColor
    {
      get
      {
        return true == _guiIndependentColor.IsChecked;
      }
      set
      {
        this._guiIndependentColor.IsChecked = value;
      }
    }

    public PenX Pen
    {
      get { return _penGlue.Pen; }
      set { _penGlue.Pen = value; }
    }

    public int SkipFrequency
    {
      get
      {
        return _guiSkipFrequency.Value;
      }
      set
      {
        this._guiSkipFrequency.Value = value;
      }
    }

    public bool IndependentSkipFrequency
    {
      get
      {
        return true == _guiIndependentSkipFreq.IsChecked;
      }

      set
      {
        _guiIndependentSkipFreq.IsChecked = value;
      }
    }

    public bool IgnoreMissingDataPoints
    {
      get { return true == _guiIgnoreMissingDataPoints.IsChecked; }
      set { this._guiIgnoreMissingDataPoints.IsChecked = value; }
    }

    public bool IndependentOnShiftingGroupStyles
    {
      get { return true == _guiIndependentOnShiftingGroupStyles.IsChecked; }
      set { this._guiIndependentOnShiftingGroupStyles.IsChecked = value; }
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

    public double GapAtStartOffset
    {
      get
      {
        return _guiGapAtStartOffset.SelectedQuantityAsValueInPoints;
      }

      set
      {
        _guiGapAtStartOffset.SelectedQuantityAsValueInPoints = value;
      }
    }

    public double GapAtStartFactor
    {
      get
      {
        return _guiGapAtStartFactor.SelectedQuantityAsValueInSIUnits;
      }

      set
      {
        _guiGapAtStartFactor.SelectedQuantityAsValueInSIUnits = value;
      }
    }

    public double GapAtEndOffset
    {
      get
      {
        return _guiGapAtEndOffset.SelectedQuantityAsValueInPoints;
      }

      set
      {
        _guiGapAtEndOffset.SelectedQuantityAsValueInPoints = value;
      }
    }

    public double GapAtEndFactor
    {
      get
      {
        return _guiGapAtEndFactor.SelectedQuantityAsValueInSIUnits;
      }

      set
      {
        _guiGapAtEndFactor.SelectedQuantityAsValueInSIUnits = value;
      }
    }

    private void EhIndependentColorChanged(object sender, RoutedEventArgs e)
    {
      IndependentColorChanged?.Invoke();
    }

    public bool ShowPlotColorsOnly
    {
      set
      {
        _penGlue.ShowPlotColorsOnly = value;
      }
    }
  }
}
