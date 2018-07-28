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

using Altaxo.Drawing;
using Altaxo.Graph;
using Altaxo.Graph.Gdi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Altaxo.Gui.Common.Drawing
{
  /// <summary>
  /// Interaction logic for BrushAllPropertiesControl.xaml
  /// </summary>
  public partial class BrushAllPropertiesControl : UserControl, IBrushViewAdvanced
  {
    public BrushAllPropertiesControl()
    {
      InitializeComponent();
    }

    #region Brush type

    public BrushType BrushType
    {
      get { return _cbBrushType.BrushType; }
      set { _cbBrushType.BrushType = value; }
    }

    public event Action BrushTypeChanged;

    private void EhBrushTypeChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if (null != BrushTypeChanged)
        BrushTypeChanged();
    }

    #endregion Brush type

    #region ForeColor

    public NamedColor ForeColor
    {
      get { return _cbColor.SelectedColor; }
      set { _cbColor.SelectedColor = value; }
    }

    public event Action ForeColorChanged;

    private void EhForeColorChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if (null != ForeColorChanged)
        ForeColorChanged();
    }

    public void ForeColorEnable(bool enable)
    {
      var vis = enable ? Visibility.Visible : Visibility.Collapsed;
      _cbColor.Visibility = vis;
      _lblForeColor.Visibility = vis;
    }

    #endregion ForeColor

    #region BackColor

    public NamedColor BackColor
    {
      get { return _cbBackColor.SelectedColor; }
      set { _cbBackColor.SelectedColor = value; }
    }

    public event Action BackColorChanged;

    private void EhBackColorChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if (null != BackColorChanged)
        BackColorChanged();
    }

    public void BackColorEnable(bool enable)
    {
      var vis = enable ? Visibility.Visible : Visibility.Collapsed;
      _cbBackColor.Visibility = vis;
      _lblBackColor.Visibility = vis;
    }

    #endregion BackColor

    #region ExchangeColors

    public bool ExchangeColors
    {
      get { return _chkExchangeColors.IsChecked == true; }
      set { _chkExchangeColors.IsChecked = value; }
    }

    public event Action ExchangeColorsChanged;

    private void EhExchangeColorsChanged(object sender, RoutedEventArgs e)
    {
      if (null != ExchangeColorsChanged)
        ExchangeColorsChanged();
    }

    public void ExchangeColorsEnable(bool enable)
    {
      var vis = enable ? Visibility.Visible : Visibility.Collapsed;
      _chkExchangeColors.Visibility = vis;
      _lblExchangeColors.Visibility = vis;
    }

    #endregion ExchangeColors

    #region RestrictColorToPlotColorsOnly

    public bool RestrictBrushColorToPlotColorsOnly
    {
      set
      {
        _cbColor.ShowPlotColorsOnly = value;
      }
    }

    #endregion RestrictColorToPlotColorsOnly

    #region WrapMode

    public System.Drawing.Drawing2D.WrapMode WrapMode
    {
      get { return _cbWrapMode.WrapMode; }
      set { _cbWrapMode.WrapMode = value; }
    }

    public event Action WrapModeChanged;

    private void EhWrapModeChanged(object sender, SelectionChangedEventArgs e)
    {
      if (null != WrapModeChanged)
        WrapModeChanged();
    }

    public void WrapModeEnable(bool enable)
    {
      var vis = enable ? Visibility.Visible : Visibility.Collapsed;
      _cbWrapMode.Visibility = vis;
      _lblWrapMode.Visibility = vis;
    }

    #endregion WrapMode

    #region GradientFocus

    public double GradientFocus
    {
      get { return _cbGradientFocus.SelectedQuantityInSIUnits; }
      set { _cbGradientFocus.SelectedQuantityInSIUnits = value; }
    }

    public event Action GradientFocusChanged;

    private void EhGradientFocusChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if (null != GradientFocusChanged)
        GradientFocusChanged();
    }

    public void GradientFocusEnable(bool enable)
    {
      var vis = enable ? Visibility.Visible : Visibility.Collapsed;
      _cbGradientFocus.Visibility = vis;
      _lblGradientFocus.Visibility = vis;
    }

    #endregion GradientFocus

    #region GradientScale

    public double GradientColorScale
    {
      get { return _cbColorScale.SelectedQuantityInSIUnits; }
      set { _cbColorScale.SelectedQuantityInSIUnits = value; }
    }

    public event Action GradientColorScaleChanged;

    private void EhColorScaleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if (null != GradientColorScaleChanged)
        GradientColorScaleChanged();
    }

    public void GradientColorScaleEnable(bool enable)
    {
      var vis = enable ? Visibility.Visible : Visibility.Collapsed;
      _cbColorScale.Visibility = vis;
      _lblColorScale.Visibility = vis;
    }

    #endregion GradientScale

    #region GradientAngle

    public double GradientAngle
    {
      get { return _cbGradientAngle.SelectedQuantityAsValueInDegrees; }
      set { _cbGradientAngle.SelectedQuantityAsValueInDegrees = value; }
    }

    public event Action GradientAngleChanged;

    private void EhGradientAngleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if (null != GradientAngleChanged)
        GradientAngleChanged();
    }

    public void GradientAngleEnable(bool enable)
    {
      var vis = enable ? Visibility.Visible : Visibility.Collapsed;
      _cbGradientAngle.Visibility = vis;
      _lblGradientAngle.Visibility = vis;
    }

    #endregion GradientAngle

    #region TextureOffsetX

    public double TextureOffsetX
    {
      get { return _guiTextureOffsetX.SelectedQuantityAsValueInSIUnits; }
      set { _guiTextureOffsetX.SelectedQuantityAsValueInSIUnits = value; }
    }

    public event Action TextureOffsetXChanged;

    private void EhTextureOffsetXChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if (null != TextureOffsetXChanged)
        TextureOffsetXChanged();
    }

    public void TextureOffsetXEnable(bool enable)
    {
      var vis = enable ? Visibility.Visible : Visibility.Collapsed;
      _guiTextureOffsetX.Visibility = vis;
      _lblTextureOffsetX.Visibility = vis;
    }

    #endregion TextureOffsetX

    #region TextureOffsetY

    public double TextureOffsetY
    {
      get { return _guiTextureOffsetY.SelectedQuantityAsValueInSIUnits; }
      set { _guiTextureOffsetY.SelectedQuantityAsValueInSIUnits = value; }
    }

    public event Action TextureOffsetYChanged;

    private void EhTextureOffsetYChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if (null != TextureOffsetYChanged)
        TextureOffsetYChanged();
    }

    public void TextureOffsetYEnable(bool enable)
    {
      var vis = enable ? Visibility.Visible : Visibility.Collapsed;
      _guiTextureOffsetY.Visibility = vis;
      _lblTextureOffsetY.Visibility = vis;
    }

    #endregion TextureOffsetY

    #region TextureImage

    public void InitTextureImage(ImageProxy proxy, BrushType imageType)
    {
      _cbTextureImage.TextureImageType = imageType;
      _cbTextureImage.TextureImage = proxy;
    }

    public ImageProxy TextureImage
    {
      get { return _cbTextureImage.TextureImage; }
    }

    public event Action TextureImageChanged;

    private void EhTextureImageChanged(object sender, SelectionChangedEventArgs e)
    {
      if (null != TextureImageChanged)
        TextureImageChanged();
    }

    public void TextureImageEnable(bool enable)
    {
      var vis = enable ? Visibility.Visible : Visibility.Collapsed;
      _cbTextureImage.Visibility = vis;
      _lblTextureImage.Visibility = vis;
    }

    #endregion TextureImage

    #region Texture scaling

    public ITextureScalingView TextureScalingView
    {
      get { return _guiTextureScaling; }
    }

    public void TextureScalingViewEnable(bool enable)
    {
      var vis = enable ? Visibility.Visible : Visibility.Collapsed;
      _guiTextureScaling.Visibility = vis;
    }

    #endregion Texture scaling

    #region Additional properties

    public Main.IInstancePropertyView AdditionalPropertiesView
    {
      get { return _guiAdditionalProperties; }
    }

    #endregion Additional properties

    #region Preview Panel

    private GdiToWpfBitmap _previewBitmap;

    public void UpdatePreview(BrushX brush)
    {
      if (null == _previewPanel || null == brush)
        return;

      int height = (int)_previewPanel.ActualHeight;
      int width = (int)_previewPanel.ActualWidth;
      if (height <= 0)
        height = 64;
      if (width <= 0)
        width = 64;

      if (null == _previewBitmap)
      {
        _previewBitmap = new GdiToWpfBitmap(width, height);
        _previewPanel.Source = _previewBitmap.WpfBitmap;
      }

      if (width != _previewBitmap.GdiRectangle.Width || height != _previewBitmap.GdiRectangle.Height)
      {
        _previewBitmap.Resize(width, height);
        _previewPanel.Source = _previewBitmap.WpfBitmap;
      }

      using (var grfx = _previewBitmap.BeginGdiPainting())
      {
        var fullRect = _previewBitmap.GdiRectangle;

        grfx.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
        grfx.FillRectangle(System.Drawing.Brushes.Transparent, fullRect);
        grfx.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;

        var r2 = fullRect;
        r2.Inflate(-r2.Width / 4, -r2.Height / 4);
        //grfx.FillRectangle(System.Drawing.Brushes.Black, r2);

        brush.SetEnvironment(fullRect, BrushX.GetEffectiveMaximumResolution(grfx));
        grfx.FillRectangle(brush, fullRect);

        _previewBitmap.EndGdiPainting();
      }
    }

    public event Action PreviewPanelSizeChanged;

    private void EhPreviewPanelSizeChanged(object sender, SizeChangedEventArgs e)
    {
      if (null != PreviewPanelSizeChanged)
        PreviewPanelSizeChanged();
    }

    #endregion Preview Panel
  }
}
