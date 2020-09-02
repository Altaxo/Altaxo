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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Altaxo.Gui.Graph.Graph3D
{
  using Altaxo.Geometry;
  using Altaxo.Graph.Graph3D;

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

    public void InitializeZPosition(Altaxo.Units.DimensionfulQuantity x, QuantityWithUnitGuiEnvironment env)
    {
      _guiPositionZ.UnitEnvironment = env;
      _guiPositionZ.SelectedQuantity = x;
    }

    public void ShowSizeElements(bool isVisible, bool isEnabled)
    {
      var vis = isVisible ? Visibility.Visible : Visibility.Collapsed;
      _guiSizeX.Visibility = vis;
      _guiSizeX.IsEnabled = isEnabled;
      _guiSizeY.Visibility = vis;
      _guiSizeY.IsEnabled = isEnabled;
      _guiSizeZ.Visibility = vis;
      _guiSizeZ.IsEnabled = isEnabled;
      _guiSizeLabelX.Visibility = vis;
      _guiSizeLabelY.Visibility = vis;
      _guiSizeLabelZ.Visibility = vis;
    }

    public void ShowScaleElements(bool isVisible, bool isEnabled)
    {
      var vis = isVisible ? Visibility.Visible : Visibility.Collapsed;

      _guiScaleX.Visibility = vis;
      _guiScaleX.IsEnabled = isEnabled;
      _guiScaleY.Visibility = vis;
      _guiScaleY.IsEnabled = isEnabled;
      _guiScaleZ.Visibility = vis;
      _guiScaleZ.IsEnabled = isEnabled;
      _guiLabelScaleX.Visibility = vis;
      _guiLabelScaleY.Visibility = vis;
      _guiLabelScaleZ.Visibility = vis;
    }

    public void ShowPositionElements(bool isVisible, bool isEnabled)
    {
      var vis = isVisible ? Visibility.Visible : Visibility.Collapsed;

      _guiPositionX.Visibility = vis;
      _guiPositionX.IsEnabled = isEnabled;
      _guiPositionY.Visibility = vis;
      _guiPositionY.IsEnabled = isEnabled;
      _guiPositionZ.Visibility = vis;
      _guiPositionZ.IsEnabled = isEnabled;
      _guiLabelPositionX.Visibility = vis;
      _guiLabelPositionY.Visibility = vis;
      _guiLabelPositionZ.Visibility = vis;
    }

    public void ShowAnchorElements(bool isVisible, bool isEnabled)
    {
      var vis = isVisible ? Visibility.Visible : Visibility.Collapsed;

      _guiLocalAnchor.Visibility = vis;
      _guiLocalAnchor.IsEnabled = isEnabled;
      _guiParentAnchor.Visibility = vis;
      _guiParentAnchor.IsEnabled = isEnabled;
    }

    public void InitializeXSize(Altaxo.Units.DimensionfulQuantity x, QuantityWithUnitGuiEnvironment env)
    {
      _guiSizeX.UnitEnvironment = env;
      _guiSizeX.SelectedQuantity = x;
    }

    public void InitializeYSize(Altaxo.Units.DimensionfulQuantity x, QuantityWithUnitGuiEnvironment env)
    {
      _guiSizeY.UnitEnvironment = env;
      _guiSizeY.SelectedQuantity = x;
    }

    public void InitializeZSize(Altaxo.Units.DimensionfulQuantity x, QuantityWithUnitGuiEnvironment env)
    {
      _guiSizeZ.UnitEnvironment = env;
      _guiSizeZ.SelectedQuantity = x;
    }

    public Altaxo.Units.DimensionfulQuantity XPosition
    {
      get { return _guiPositionX.SelectedQuantity; }
    }

    public Altaxo.Units.DimensionfulQuantity YPosition
    {
      get { return _guiPositionY.SelectedQuantity; }
    }

    public Altaxo.Units.DimensionfulQuantity ZPosition
    {
      get { return _guiPositionZ.SelectedQuantity; }
    }

    public Altaxo.Units.DimensionfulQuantity XSize
    {
      get { return _guiSizeX.SelectedQuantity; }
    }

    public Altaxo.Units.DimensionfulQuantity YSize
    {
      get { return _guiSizeY.SelectedQuantity; }
    }

    public Altaxo.Units.DimensionfulQuantity ZSize
    {
      get { return _guiSizeZ.SelectedQuantity; }
    }

    public double RotationX
    {
      get
      {
        return _guiRotationX.SelectedQuantityAsValueInDegrees;
      }
      set
      {
        _guiRotationX.SelectedQuantityAsValueInDegrees = value;
      }
    }

    public double RotationY
    {
      get
      {
        return _guiRotationY.SelectedQuantityAsValueInDegrees;
      }
      set
      {
        _guiRotationY.SelectedQuantityAsValueInDegrees = value;
      }
    }

    public double RotationZ
    {
      get
      {
        return _guiRotationZ.SelectedQuantityAsValueInDegrees;
      }
      set
      {
        _guiRotationZ.SelectedQuantityAsValueInDegrees = value;
      }
    }

    public double ShearX
    {
      get
      {
        return _guiShearX.SelectedQuantityInSIUnits;
      }
      set
      {
        _guiShearX.SelectedQuantityInSIUnits = value;
      }
    }

    public double ShearY
    {
      get
      {
        return _guiShearY.SelectedQuantityInSIUnits;
      }
      set
      {
        _guiShearY.SelectedQuantityInSIUnits = value;
      }
    }

    public double ShearZ
    {
      get
      {
        return _guiShearZ.SelectedQuantityInSIUnits;
      }
      set
      {
        _guiShearZ.SelectedQuantityInSIUnits = value;
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

    public double ScaleZ
    {
      get
      {
        return _guiScaleZ.SelectedQuantityInSIUnits;
      }
      set
      {
        _guiScaleZ.SelectedQuantityInSIUnits = value;
      }
    }

    public void InitializePivot(RADouble pivotX, RADouble pivotY, RADouble pivotZ, VectorD3D sizeOfTextGraphic)
    {
      _guiLocalAnchor.SetSelectedPivot(pivotX, pivotY, pivotZ, sizeOfTextGraphic);
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

    public RADouble PivotZ
    {
      get
      {
        return _guiLocalAnchor.SelectedPivotZ;
      }
    }

    public void InitializeReference(RADouble pivotX, RADouble pivotY, RADouble pivotZ, VectorD3D sizeOfTextGraphic)
    {
      _guiParentAnchor.SetSelectedPivot(pivotX, pivotY, pivotZ, sizeOfTextGraphic);
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

    public RADouble ReferenceZ
    {
      get
      {
        return _guiParentAnchor.SelectedPivotZ;
      }
    }

    public event Action SizeXChanged;

    private void EhSizeXChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      var actn = SizeXChanged;
      if (actn is not null)
        actn();
    }

    public event Action SizeYChanged;

    private void EhSizeYChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      var actn = SizeYChanged;
      if (actn is not null)
        actn();
    }

    public event Action SizeZChanged;

    private void EhSizeZChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      var actn = SizeZChanged;
      if (actn is not null)
        actn();
    }

    public event Action ScaleXChanged;

    private void EhScaleXChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      var actn = ScaleXChanged;
      if (actn is not null)
        actn();
    }

    public event Action ScaleYChanged;

    private void EhScaleYChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      var actn = ScaleYChanged;
      if (actn is not null)
        actn();
    }

    public event Action ScaleZChanged;

    private void EhScaleZChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      var actn = ScaleZChanged;
      if (actn is not null)
        actn();
    }
  }
}
