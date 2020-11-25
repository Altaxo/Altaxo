#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2015 Dr. Dirk Lellinger
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

#nullable disable warnings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Altaxo.Geometry;
using Altaxo.Graph.Graph3D;

namespace Altaxo.Gui.Graph.Graph3D.Shapes
{
  using Altaxo.Units;
  using AUL = Altaxo.Units.Length;

  /// <summary>
  /// Interaction logic for AnchoringControl.xaml
  /// </summary>
  public partial class AnchoringControl : UserControl
  {
    private RadioButton[,] _buttons;
    private RADouble[] _pivots;

    private bool _useRadioGrid;

    private QuantityWithUnitGuiEnvironment _xSizeEnvironment, _ySizeEnvironment, _zSizeEnvironment;

    private ChangeableRelativePercentUnit _percentLayerXSizeUnit = new ChangeableRelativePercentUnit("% X-Size", "%", new DimensionfulQuantity(1, AUL.Point.Instance));
    private ChangeableRelativePercentUnit _percentLayerYSizeUnit = new ChangeableRelativePercentUnit("% Y-Size", "%", new DimensionfulQuantity(1, AUL.Point.Instance));
    private ChangeableRelativePercentUnit _percentLayerZSizeUnit = new ChangeableRelativePercentUnit("% Z-Size", "%", new DimensionfulQuantity(1, AUL.Point.Instance));

    public AnchoringControl()
    {
      InitializeComponent();
      _buttons = new RadioButton[3, 3] { { _guiXNear, _guiXCenter, _guiXFar }, { _guiYNear, _guiYCenter, _guiYFar }, { _guiZNear, _guiZCenter, _guiZFar } };
      // set group names
      for (int i = 0; i < 3; ++i)
      {
        var groupName = Guid.NewGuid().ToString();
        for (int j = 0; j < 3; ++j)
          _buttons[i, j].GroupName = groupName; // we have to give the radio buttons in a row a unique group name. Is is not sufficient to use a static name, since it is possible to have two of these controls at the same user control
      }
      _pivots = new RADouble[3];
      SetRadioButton();
    }

    /// <summary>
    /// Sets the title of the group box.
    /// </summary>
    /// <value>
    /// The title.
    /// </value>
    public string Title
    {
      set
      {
        _guiMainBox.Header = value;
      }
    }

    /// <summary>
    /// Sets the selected pivot values for X and Y. Additionally, the reference size is required.
    /// </summary>
    /// <param name="pivotX">The pivot x value.</param>
    /// <param name="pivotY">The pivot y value.</param>
    /// <param name="referenceSize">Size of the reference area.</param>
    public void SetSelectedPivot(RADouble pivotX, RADouble pivotY, RADouble pivotZ, VectorD3D referenceSize)
    {
      _pivots[0] = pivotX;
      _pivots[1] = pivotY;
      _pivots[2] = pivotZ;

      _percentLayerXSizeUnit.ReferenceQuantity = new DimensionfulQuantity(referenceSize.X, AUL.Point.Instance);
      _percentLayerYSizeUnit.ReferenceQuantity = new DimensionfulQuantity(referenceSize.Y, AUL.Point.Instance);
      _percentLayerZSizeUnit.ReferenceQuantity = new DimensionfulQuantity(referenceSize.Z, AUL.Point.Instance);
      _xSizeEnvironment = new QuantityWithUnitGuiEnvironment(GuiLengthUnits.Collection, _percentLayerXSizeUnit);
      _ySizeEnvironment = new QuantityWithUnitGuiEnvironment(GuiLengthUnits.Collection, _percentLayerYSizeUnit);
      _zSizeEnvironment = new QuantityWithUnitGuiEnvironment(GuiLengthUnits.Collection, _percentLayerZSizeUnit);

      _guiPivotX.UnitEnvironment = _xSizeEnvironment;
      _guiPivotY.UnitEnvironment = _ySizeEnvironment;
      _guiPivotZ.UnitEnvironment = _zSizeEnvironment;
      _guiPivotX.SelectedQuantity = _pivots[0].IsAbsolute ? new DimensionfulQuantity(_pivots[0].Value, AUL.Point.Instance) : new DimensionfulQuantity(_pivots[0].Value * 100, _percentLayerXSizeUnit);
      _guiPivotY.SelectedQuantity = _pivots[1].IsAbsolute ? new DimensionfulQuantity(_pivots[1].Value, AUL.Point.Instance) : new DimensionfulQuantity(_pivots[1].Value * 100, _percentLayerYSizeUnit);
      _guiPivotZ.SelectedQuantity = _pivots[2].IsAbsolute ? new DimensionfulQuantity(_pivots[2].Value, AUL.Point.Instance) : new DimensionfulQuantity(_pivots[2].Value * 100, _percentLayerZSizeUnit);

      if (CanUseRadioGridView())
      {
        SetRadioButton();
        SetUseOfRadioGrid(true);
      }
      else
      {
        SetUseOfRadioGrid(false);
      }
      SetVisibilityOfSwitchButton();
    }

    /// <summary>
    /// Gets the selected pivot x value.
    /// </summary>
    /// <value>
    /// The selected pivot x value.
    /// </value>
    public RADouble SelectedPivotX
    {
      get
      {
        return _pivots[0];
      }
    }

    /// <summary>
    /// Gets the selected pivot y value.
    /// </summary>
    /// <value>
    /// The selected pivot y value.
    /// </value>
    public RADouble SelectedPivotY
    {
      get
      {
        return _pivots[1];
      }
    }

    /// <summary>
    /// Gets the selected pivot y value.
    /// </summary>
    /// <value>
    /// The selected pivot y value.
    /// </value>
    public RADouble SelectedPivotZ
    {
      get
      {
        return _pivots[2];
      }
    }

    private void EhRadioChecked(object sender, RoutedEventArgs e)
    {
      for (int i = 0; i < 3; ++i)
      {
        for (int j = 0; j < 3; ++j)
        {
          if (object.Equals(sender, _buttons[j, i]))
          {
            _pivots[j] = RADouble.NewRel(0.5 * i);
          }
        }
      }
    }

    private void SetRadioButton()
    {
      int i = (int)(_pivots[0].Value * 2);
      int j = (int)(_pivots[1].Value * 2);
      int k = (int)(_pivots[2].Value * 2);
      _buttons[0, i].IsChecked = true;
      _buttons[1, j].IsChecked = true;
      _buttons[2, k].IsChecked = true;
    }

    private void SetUseOfRadioGrid(bool useRadioGrid)
    {
      _useRadioGrid = useRadioGrid;
      if (useRadioGrid)
      {
        _guiRadioGridView.Visibility = System.Windows.Visibility.Visible;
        _guiNumericView.Visibility = System.Windows.Visibility.Collapsed;
      }
      else
      {
        _guiRadioGridView.Visibility = System.Windows.Visibility.Collapsed;
        _guiNumericView.Visibility = System.Windows.Visibility.Visible;
      }
    }

    private void SetVisibilityOfSwitchButton()
    {
      if (_useRadioGrid)
      {
        _guiSwitchToNumericView.Visibility = System.Windows.Visibility.Visible;
        _guiSwitchToRadioView.Visibility = System.Windows.Visibility.Collapsed;
      }
      else // currently in numeric view
      {
        _guiSwitchToNumericView.Visibility = System.Windows.Visibility.Collapsed;
        _guiSwitchToRadioView.Visibility = System.Windows.Visibility.Visible;
        _guiSwitchToRadioView.IsEnabled = CanUseRadioGridView();
      }
    }

    private bool CanUseRadioGridView()
    {
      bool useRadioView = true;
      useRadioView &= _pivots[0].IsRelative && (_pivots[0].Value == 0 || _pivots[0].Value == 0.5 || _pivots[0].Value == 1);
      useRadioView &= _pivots[1].IsRelative && (_pivots[1].Value == 0 || _pivots[1].Value == 0.5 || _pivots[1].Value == 1);
      useRadioView &= _pivots[2].IsRelative && (_pivots[2].Value == 0 || _pivots[2].Value == 0.5 || _pivots[2].Value == 1);
      return useRadioView;
    }

    private void EhNumericPivotXChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      var quant = _guiPivotX.SelectedQuantity;
      if (object.ReferenceEquals(quant.Unit, _percentLayerXSizeUnit))
        _pivots[0] = RADouble.NewRel(quant.Value / 100);
      else
        _pivots[0] = RADouble.NewAbs(quant.AsValueIn(AUL.Point.Instance));

      SetVisibilityOfSwitchButton();
    }

    private void EhNumericPivotYChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      var quant = _guiPivotY.SelectedQuantity;
      if (object.ReferenceEquals(quant.Unit, _percentLayerYSizeUnit))
        _pivots[1] = RADouble.NewRel(quant.Value / 100);
      else
        _pivots[1] = RADouble.NewAbs(quant.AsValueIn(AUL.Point.Instance));

      SetVisibilityOfSwitchButton();
    }

    private void EhNumericPivotZChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      var quant = _guiPivotZ.SelectedQuantity;
      if (object.ReferenceEquals(quant.Unit, _percentLayerZSizeUnit))
        _pivots[2] = RADouble.NewRel(quant.Value / 100);
      else
        _pivots[2] = RADouble.NewAbs(quant.AsValueIn(AUL.Point.Instance));

      SetVisibilityOfSwitchButton();
    }

    private void EhSwitchToNumericView(object sender, RoutedEventArgs e)
    {
      SetUseOfRadioGrid(false);
      SetVisibilityOfSwitchButton();
    }

    private void EhSwitchToGraphicalView(object sender, RoutedEventArgs e)
    {
      SetUseOfRadioGrid(true);
      SetVisibilityOfSwitchButton();
    }
  }
}
