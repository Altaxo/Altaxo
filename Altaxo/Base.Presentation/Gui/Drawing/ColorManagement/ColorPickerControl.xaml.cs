#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2017 Dr. Dirk Lellinger
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

using Altaxo.Drawing;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Altaxo.Gui.Drawing.ColorManagement
{
  public partial class ColorPickerControl : UserControl, IColorPickerView
  {
    private AxoColor _oldColor, _newColor;

    public event Action<AxoColor> CurrentColorChanged;

    //
    // Initialization

    public ColorPickerControl()
      : this(AxoColors.Red)
    {
    }

    public ColorPickerControl(AxoColor oldColor)
    {
      _oldColor = oldColor;
      _newColor = _oldColor;
      InitializeComponent();
    }

    public AxoColor SelectedColor
    {
      get
      {
        return _newColor;
      }
      set
      {
        if (value != _newColor)
        {
          _newColor = value;

          UpdateControlValues();
          UpdateControlVisuals();
        }
      }
    }

    // Completes initialization after all XAML member vars have been initialized.
    protected override void OnInitialized(EventArgs e)
    {
      base.OnInitialized(e);

      UpdateControlValues();
      UpdateControlVisuals();

      colorComb.ColorSelected += new EventHandler<ColorEventArgs>(EhColorCombControl_ColorSelected);
      brightnessSlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(EhBrightnessSlider_ValueChanged);
      opacitySlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(EhOpacitySlider_ValueChanged);
    }

    //
    // Implementation

    private bool _notUserInitiated;

    // Updates values of controls when new DA is set (or upon initialization).
    private void UpdateControlValues()
    {
      _notUserInitiated = true;
      try
      {
        // Set nominal color on comb.
        //Color nc = m_selectedDA.Color;
        Color nc = GuiHelper.ToWpf(_newColor);
        float f = Math.Max(Math.Max(nc.ScR, nc.ScG), nc.ScB);
        if (f < 0.001f) // black
          nc = Color.FromScRgb(1f, 1f, 1f, 1f);
        else
          nc = Color.FromScRgb(1f, nc.ScR / f, nc.ScG / f, nc.ScB / f);
        colorComb.SelectedColor = nc;

        // Set brightness and opacity.
        brightnessSlider.Value = f;
        opacitySlider.Value = _newColor.ScA;
      }
      finally
      {
        _notUserInitiated = false;
      }
    }

    // Updates visual properties of all controls, in response to any change.
    private void UpdateControlVisuals()
    {
      Color c = GuiHelper.ToWpf(_newColor);

      // Update LGB for brightnessSlider
      Border sb1 = brightnessSlider.Parent as Border;
      LinearGradientBrush lgb1 = sb1.Background as LinearGradientBrush;
      lgb1.GradientStops[1].Color = colorComb.SelectedColor;

      // Update LGB for opacitySlider
      Color c2a = Color.FromScRgb(0f, c.ScR, c.ScG, c.ScB);
      Color c2b = Color.FromScRgb(1f, c.ScR, c.ScG, c.ScB);
      Border sb2 = opacitySlider.Parent as Border;
      LinearGradientBrush lgb2 = sb2.Background as LinearGradientBrush;
      lgb2.GradientStops[0].Color = c2a;
      lgb2.GradientStops[1].Color = c2b;
    }

    //
    // Event Handlers

    private void EhColorCombControl_ColorSelected(object sender, ColorEventArgs e)
    {
      if (_notUserInitiated)
        return;

      float a, f, r, g, b;
      a = (float)opacitySlider.Value;
      f = (float)brightnessSlider.Value;

      Color nc = e.Color;
      r = f * nc.ScR;
      g = f * nc.ScG;
      b = f * nc.ScB;

      _newColor = AxoColor.FromScRgb(a, r, g, b);
      UpdateControlVisuals();
      CurrentColorChanged?.Invoke(_newColor);
    }

    private void EhBrightnessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
      if (_notUserInitiated)
        return;

      Color nc = colorComb.SelectedColor;
      float f = (float)e.NewValue;

      float a, r, g, b;
      a = (float)opacitySlider.Value;
      r = f * nc.ScR;
      g = f * nc.ScG;
      b = f * nc.ScB;

      _newColor = AxoColor.FromScRgb(a, r, g, b);
      UpdateControlVisuals();
      CurrentColorChanged?.Invoke(_newColor);
    }

    private void EhOpacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
      if (_notUserInitiated)
        return;

      var c = _newColor;
      float a = (float)e.NewValue;

      _newColor = AxoColor.FromScRgb(a, c.ScR, c.ScG, c.ScB);
      UpdateControlVisuals();
      CurrentColorChanged?.Invoke(_newColor);
    }
  }
}
