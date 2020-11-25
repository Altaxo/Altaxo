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

namespace Altaxo.Gui.Common.Drawing
{
  using Altaxo.Drawing;
  using Altaxo.Units;
  using AUL = Altaxo.Units.Length;

  /// <summary>
  /// Interaction logic for TextureScalingView.xaml
  /// </summary>
  public partial class TextureScalingControl : UserControl, ITextureScalingView
  {
    public TextureScalingControl()
    {
      InitializeComponent();
    }

    private void EhScalingModeChanged(object sender, RoutedEventArgs e)
    {
      if (ScalingModeChanged is not null)
        ScalingModeChanged();
    }

    private void EhKeepAspectChanged(object sender, RoutedEventArgs e)
    {
      if (AspectPreservingChanged is not null)
        AspectPreservingChanged();
    }

    private void EhXSizeChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if (XChanged is not null)
        XChanged();
    }

    private void EhYSizeChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if (YChanged is not null)
        YChanged();
    }

    private void EhXScaleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if (XChanged is not null)
        XChanged();
    }

    private void EhYScaleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if (YChanged is not null)
        YChanged();
    }

    public TextureScalingMode ScalingMode
    {
      get
      {
        if (true == _guiScaleWithSource.IsChecked)
          return TextureScalingMode.Source;
        if (true == _guiScaleWithDest.IsChecked)
          return TextureScalingMode.Destination;
        else
          return TextureScalingMode.Absolute;
      }
      set
      {
        if (value == TextureScalingMode.Source)
          _guiScaleWithSource.IsChecked = true;
        else if (value == TextureScalingMode.Destination)
          _guiScaleWithDest.IsChecked = true;
        else
          _guiScaleWithAbs.IsChecked = true;
      }
    }

    public event Action? ScalingModeChanged;

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

    public event Action? AspectPreservingChanged;

    public double XScale
    {
      get
      {
        return _guiXScale.SelectedQuantityInSIUnits;
      }
      set
      {
        _guiXScale.SelectedQuantityInSIUnits = value;
      }
    }

    public double YScale
    {
      get
      {
        return _guiYScale.SelectedQuantityInSIUnits;
      }
      set
      {
        _guiYScale.SelectedQuantityInSIUnits = value;
      }
    }

    public double XSize
    {
      get
      {
        return _guiXSize.SelectedQuantity.AsValueIn(AUL.Point.Instance);
      }
      set
      {
        _guiXSize.SelectedQuantityAsValueInSIUnits = new DimensionfulQuantity(value, AUL.Point.Instance).AsValueInSIUnits;
      }
    }

    public double YSize
    {
      get
      {
        return _guiYSize.SelectedQuantity.AsValueIn(AUL.Point.Instance);
      }
      set
      {
        _guiYSize.SelectedQuantityAsValueInSIUnits = new DimensionfulQuantity(value, AUL.Point.Instance).AsValueInSIUnits;
      }
    }

    public event Action? XChanged;

    public event Action? YChanged;

    public bool ShowSizeNotScale
    {
      set
      {
        var sizevis = value ? Visibility.Visible : Visibility.Collapsed;
        var scalevis = value ? Visibility.Collapsed : Visibility.Visible;
        _guiLabelXSize.Visibility = sizevis;
        _guiLabelYSize.Visibility = sizevis;
        _guiXSize.Visibility = sizevis;
        _guiYSize.Visibility = sizevis;

        _guiLabelXScale.Visibility = scalevis;
        _guiLabelYScale.Visibility = scalevis;
        _guiXScale.Visibility = scalevis;
        _guiYScale.Visibility = scalevis;
      }
    }
  }
}
