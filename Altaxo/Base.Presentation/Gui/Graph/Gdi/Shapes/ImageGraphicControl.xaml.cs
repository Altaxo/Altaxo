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
using Altaxo.Geometry;

namespace Altaxo.Gui.Graph.Gdi.Shapes
{
  using Altaxo.Drawing;
  using Altaxo.Units;
  using AUL = Altaxo.Units.Length;

  /// <summary>
  /// Interaction logic for ImageGraphicControl.xaml
  /// </summary>
  public partial class ImageGraphicControl : UserControl, IImageGraphicView
  {
    public ImageGraphicControl()
    {
      InitializeComponent();
    }

    public PointD2D SourceSize
    {
      set
      {
        _guiSrcSizeX.SelectedQuantity = new DimensionfulQuantity(value.X, AUL.Point.Instance).AsQuantityIn(_guiSrcSizeX.UnitEnvironment.DefaultUnit);
        _guiSrcSizeY.SelectedQuantity = new DimensionfulQuantity(value.Y, AUL.Point.Instance).AsQuantityIn(_guiSrcSizeY.UnitEnvironment.DefaultUnit);
      }
    }

    public AspectRatioPreservingMode AspectPreserving
    {
      get
      {
        if (true == _guiKeepAspectX.IsChecked)
          return AspectRatioPreservingMode.PreserveXPriority;
        else if (true == _guiKeepAspectY.IsChecked)
          return AspectRatioPreservingMode.PreserveYPriority;
        else
          return AspectRatioPreservingMode.None;
      }
      set
      {
        if (value == AspectRatioPreservingMode.PreserveXPriority)
          _guiKeepAspectX.IsChecked = true;
        else if (value == AspectRatioPreservingMode.PreserveYPriority)
          _guiKeepAspectY.IsChecked = true;
        else
          _guiKeepAspectNo.IsChecked = true;
      }
    }

    public bool IsSizeCalculationBasedOnSourceSize
    {
      get
      {
        return _guiScaleWithSource.IsChecked == true;
      }
      set
      {
        if (value)
          _guiScaleWithSource.IsChecked = true;
        else
          _guiScaleWithAbs.IsChecked = true;
      }
    }

    public object LocationView
    {
      set
      {
        _guiLocationHost.Child = (UIElement)value;
      }
    }

    public event Action AspectPreservingChanged;

    private void EhKeepAspectChanged(object sender, RoutedEventArgs e)
    {
      if (AspectPreservingChanged is not null)
        AspectPreservingChanged();
    }

    public event Action ScalingModeChanged;

    private void EhScalingModeChanged(object sender, RoutedEventArgs e)
    {
      if (ScalingModeChanged is not null)
        ScalingModeChanged();
    }

    public event Action ScaleXChanged;

    private void EhScaleXChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if (ScaleXChanged is not null)
        ScaleXChanged();
    }

    public event Action ScaleYChanged;

    private void EhScaleYChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if (ScaleYChanged is not null)
        ScaleYChanged();
    }

    public event Action SizeXChanged;

    private void EhSizeXChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if (SizeXChanged is not null)
        SizeXChanged();
    }

    public event Action SizeYChanged;

    private void EhSizeYChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if (SizeYChanged is not null)
        SizeYChanged();
    }
  }
}
