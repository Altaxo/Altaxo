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

namespace Altaxo.Gui.Graph.Gdi
{
  using Altaxo.Geometry;
  using Altaxo.Graph;

  /// <summary>
  /// Interaction logic for LayerPositionControl.xaml
  /// </summary>
  public partial class ItemLocationDirectControl : UserControl, IItemLocationDirectView
  {
    public ItemLocationDirectControl()
    {
      InitializeComponent();
    }

    public void InitializeXPosition(Altaxo.Units.DimensionfulQuantity x, QuantityWithUnitGuiEnvironment env)
    {
      _guiPositionX.UnitEnvironment = env;
      _guiPositionX.SelectedQuantity = x;
    }

    public void InitializeYPosition(Altaxo.Units.DimensionfulQuantity x, QuantityWithUnitGuiEnvironment env)
    {
      _guiPositionY.UnitEnvironment = env;
      _guiPositionY.SelectedQuantity = x;
    }

    public void ShowSizeElements(bool isVisible, bool isEnabled)
    {
      var vis = isVisible ? Visibility.Visible : Visibility.Collapsed;
      _guiSizeX.Visibility = vis;
      _guiSizeX.IsEnabled = isEnabled;
      _guiSizeY.Visibility = vis;
      _guiSizeY.IsEnabled = isEnabled;
      _guiSizeLabelX.Visibility = vis;
      _guiSizeLabelY.Visibility = vis;
    }

    public void ShowScaleElements(bool isVisible, bool isEnabled)
    {
      var vis = isVisible ? Visibility.Visible : Visibility.Collapsed;

      _guiScaleX.Visibility = vis;
      _guiScaleX.IsEnabled = isEnabled;
      _guiScaleY.Visibility = vis;
      _guiScaleY.IsEnabled = isEnabled;
      _guiLabelScaleX.Visibility = vis;
      _guiLabelScaleY.Visibility = vis;
    }

    public void ShowPositionElements(bool isVisible, bool isEnabled)
    {
      var vis = isVisible ? Visibility.Visible : Visibility.Collapsed;

      _guiPositionX.Visibility = vis;
      _guiPositionX.IsEnabled = isEnabled;
      _guiPositionY.Visibility = vis;
      _guiPositionY.IsEnabled = isEnabled;
      _guiLabelPositionX.Visibility = vis;
      _guiLabelPositionY.Visibility = vis;
    }

    public void ShowAnchorElements(bool isVisible, bool isEnabled)
    {
      var vis = isVisible ? Visibility.Visible : Visibility.Collapsed;

      _guiLocalAnchor.Visibility = vis;
      _guiLocalAnchor.IsEnabled = isEnabled;
      _guiParentAnchor.Visibility = vis;
      _guiParentAnchor.IsEnabled = isEnabled;
    }

    public void InitializeYSize(Altaxo.Units.DimensionfulQuantity x, QuantityWithUnitGuiEnvironment env)
    {
      _guiSizeY.UnitEnvironment = env;
      _guiSizeY.SelectedQuantity = x;
    }

    public void InitializeXSize(Altaxo.Units.DimensionfulQuantity x, QuantityWithUnitGuiEnvironment env)
    {
      _guiSizeX.UnitEnvironment = env;
      _guiSizeX.SelectedQuantity = x;
    }

    public Altaxo.Units.DimensionfulQuantity XPosition
    {
      get { return _guiPositionX.SelectedQuantity; }
    }

    public Altaxo.Units.DimensionfulQuantity YPosition
    {
      get { return _guiPositionY.SelectedQuantity; }
    }

    public Altaxo.Units.DimensionfulQuantity XSize
    {
      get { return _guiSizeX.SelectedQuantity; }
    }

    public Altaxo.Units.DimensionfulQuantity YSize
    {
      get { return _guiSizeY.SelectedQuantity; }
    }

    public double Rotation
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

    public double Shear
    {
      get
      {
        return _guiShear.SelectedQuantityInSIUnits;
      }
      set
      {
        _guiShear.SelectedQuantityInSIUnits = value;
      }
    }

    public double ScaleX
    {
      get
      {
        return _guiScaleX.SelectedQuantityInSIUnits;
      }
      set
      {
        _guiScaleX.SelectedQuantityInSIUnits = value;
      }
    }

    public double ScaleY
    {
      get
      {
        return _guiScaleY.SelectedQuantityInSIUnits;
      }
      set
      {
        _guiScaleY.SelectedQuantityInSIUnits = value;
      }
    }

    public void InitializePivot(RADouble pivotX, RADouble pivotY, PointD2D sizeOfTextGraphic)
    {
      _guiLocalAnchor.SetSelectedPivot(pivotX, pivotY, sizeOfTextGraphic);
    }

    public RADouble PivotX
    {
      get
      {
        return _guiLocalAnchor.SelectedPivotX;
      }
    }

    public RADouble PivotY
    {
      get
      {
        return _guiLocalAnchor.SelectedPivotY;
      }
    }

    public void InitializeReference(RADouble pivotX, RADouble pivotY, PointD2D sizeOfTextGraphic)
    {
      _guiParentAnchor.SetSelectedPivot(pivotX, pivotY, sizeOfTextGraphic);
    }

    public RADouble ReferenceX
    {
      get
      {
        return _guiParentAnchor.SelectedPivotX;
      }
    }

    public RADouble ReferenceY
    {
      get
      {
        return _guiParentAnchor.SelectedPivotY;
      }
    }

    public event Action? SizeXChanged;

    private void EhSizeXChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      var actn = SizeXChanged;
      if (actn is not null)
        actn();
    }

    public event Action? SizeYChanged;

    private void EhSizeYChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      var actn = SizeYChanged;
      if (actn is not null)
        actn();
    }

    public event Action? ScaleXChanged;

    private void EhScaleXChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      var actn = ScaleXChanged;
      if (actn is not null)
        actn();
    }

    public event Action? ScaleYChanged;

    private void EhScaleYChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      var actn = ScaleYChanged;
      if (actn is not null)
        actn();
    }
  }
}
