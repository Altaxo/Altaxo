﻿#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Altaxo.Collections;
using Altaxo.Drawing;
using Altaxo.Geometry;
using Altaxo.Graph.Gdi;

namespace Altaxo.Gui
{
  public static partial class GuiHelper
  {
    #region Brush and Pen

    public static System.Windows.Media.Brush ToWpf(this BrushX brushx)
    {
      System.Windows.Media.Color c = ToWpf(brushx.Color);
      var result = new System.Windows.Media.SolidColorBrush(c);
      return result;
    }

    public static System.Windows.Media.Pen ToWpf(this PenX penx)
    {
      System.Windows.Media.Color c = ToWpf(penx.Color);
      var result = new System.Windows.Media.Pen(new System.Windows.Media.SolidColorBrush(c), penx.Width);
      return result;
    }

    public static Altaxo.Drawing.AxoColor ToAxo(this System.Drawing.Color c)
    {
      return Altaxo.Drawing.AxoColor.FromArgb(c.A, c.R, c.G, c.B);
    }

    public static System.Drawing.Color ToGdi(this Altaxo.Drawing.AxoColor c)
    {
      return System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B);
    }

    public static System.Windows.Media.Color ToWpf(this Altaxo.Drawing.AxoColor c)
    {
      if (c.IsFromArgb)
        return Color.FromArgb(c.A, c.R, c.G, c.B);
      else
        return Color.FromScRgb(c.ScA, c.ScR, c.ScG, c.ScB);
    }

    public static System.Windows.Media.Color ToWpf(this Altaxo.Drawing.NamedColor c)
    {
      return ToWpf(c.Color);
    }

    public static Altaxo.Drawing.AxoColor ToAxo(this System.Windows.Media.Color c)
    {
      return Altaxo.Drawing.AxoColor.FromScRgb(c.ScA, c.ScR, c.ScG, c.ScB);
    }

    #endregion Brush and Pen

    #region Panel Helpers

    public static void InitializeChoicePanel<TChoiceGuiElement>(Panel panel, SelectableListNodeList choices) where TChoiceGuiElement : ToggleButton, new()
    {
      panel.Tag = choices;
      panel.Children.Clear();
      foreach (var choice in choices)
      {
        var rb = new TChoiceGuiElement
        {
          Content = choice.Text,
          Tag = choice
        };
        rb.SetBinding(ToggleButton.IsCheckedProperty, new System.Windows.Data.Binding("IsSelected") { Source = choice, Mode = System.Windows.Data.BindingMode.TwoWay });
        panel.Children.Add(rb);
      }
    }

    #endregion Panel Helpers

    #region Image Proxy converters

    /// <summary>Converts <see cref="Altaxo.Drawing.ImageProxy"/> instances to Wpf <see cref="ImageSource"/> instances.</summary>
    /// <param name="proxy">The proxy.</param>
    /// <returns></returns>
    public static ImageSource ToWpf(ImageProxy proxy)
    {
      var stream = proxy.GetContentStream();
      var decoder = System.Windows.Media.Imaging.BitmapDecoder.Create(stream, System.Windows.Media.Imaging.BitmapCreateOptions.None, System.Windows.Media.Imaging.BitmapCacheOption.Default);
      return decoder.Frames[0];
    }

    /// <summary>Converts a brush to a Wpf <see cref="ImageSource"/> instance.</summary>
    /// <param name="brush">The brush to convert.</param>
    /// <param name="xsize">The horizontal number of pixels of the image.</param>
    /// <param name="ysize">The vertical number of pixels of the image.</param>
    /// <returns>An image that represents the brush.</returns>
    public static ImageSource ToWpf(BrushX brush, int xsize, int ysize)
    {
      using (var bmp = new System.Drawing.Bitmap(xsize, ysize, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
      {
        using (var g = System.Drawing.Graphics.FromImage(bmp))
        {
          using (var brushGdi = Altaxo.Graph.Gdi.BrushCacheGdi.Instance.BorrowBrush(brush, new RectangleD2D(0, 0, xsize, ysize), 1))
          {
            g.FillRectangle(brushGdi, 0, 0, xsize, ysize);
          }
        }
        var stream = SystemDrawingImageProxyExtensions.GetStreamFromImage(bmp, System.Drawing.Imaging.ImageFormat.Png);
        var decoder = System.Windows.Media.Imaging.BitmapDecoder.Create(stream, System.Windows.Media.Imaging.BitmapCreateOptions.None, System.Windows.Media.Imaging.BitmapCacheOption.Default);
        return decoder.Frames[0];
      }
    }

    #endregion Image Proxy converters

    #region Image Conversion Wpf <==> Gdi

    /// <summary>
    /// Converts a Wpf <see cref="System.Windows.Media.Imaging.BitmapSource"/> to a Gdi <see cref="System.Drawing.Bitmap"/>.
    /// Note that this works only with 32 bits BRGA bitmaps.
    /// </summary>
    /// <param name="wpfBitmapSource">The Wpf bitmap source.</param>
    /// <returns>The Gdi bitmap.</returns>
    public static System.Drawing.Bitmap ToGdi(System.Windows.Media.Imaging.BitmapSource wpfBitmapSource)
    {
      if (wpfBitmapSource is null)
        throw new ArgumentNullException(nameof(wpfBitmapSource));

      var gdiBitmap = new System.Drawing.Bitmap(
        wpfBitmapSource.PixelWidth,
        wpfBitmapSource.PixelHeight,
        System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

      var bitmapData = gdiBitmap.LockBits(
        new System.Drawing.Rectangle(System.Drawing.Point.Empty, gdiBitmap.Size),
        System.Drawing.Imaging.ImageLockMode.WriteOnly,
        System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

      wpfBitmapSource.CopyPixels(
        Int32Rect.Empty,
        bitmapData.Scan0,
        bitmapData.Height * bitmapData.Stride,
        bitmapData.Stride);

      gdiBitmap.UnlockBits(bitmapData);

      return gdiBitmap;
    }

    /// <summary>
    /// Converts a Gdi <see cref="System.Drawing.Image"/> into a Wpf <see cref="System.Windows.Media.Imaging.BitmapSource"/>.
    /// For this, the Gdi image is converted into a Gdi bitmap, and then converted to a Wpf BitmapSource.
    /// </summary>
    /// <param name="gdiImage">The Gdi image to convert.</param>
    /// <returns>A Wpf <see cref="System.Windows.Media.Imaging.BitmapSource"/>.</returns>
    public static System.Windows.Media.Imaging.BitmapSource ToBitmapSource(this System.Drawing.Image gdiImage)
    {
      if (gdiImage is null)
        throw new ArgumentNullException(nameof(gdiImage));

      System.Windows.Media.Imaging.BitmapSource wpfBitmapSource = null;

      using (var gdiBitmap = new System.Drawing.Bitmap(gdiImage))
      {
        wpfBitmapSource = gdiBitmap.ToBitmapSource();
      }
      return wpfBitmapSource;
    }

    /// <summary>
    /// Converts a <see cref="System.Drawing.Bitmap"/> into a WPF <see cref="System.Windows.Media.Imaging.BitmapSource"/>.
    /// </summary>
    /// <remarks>Uses GDI to do the conversion. Hence the call to the marshalled DeleteObject.
    /// </remarks>
    /// <param name="gdiBitmap">The source bitmap.</param>
    /// <returns>A Wpf <see cref="System.Windows.Media.Imaging.BitmapSource"/>.</returns>
    public static System.Windows.Media.Imaging.BitmapSource ToBitmapSource(this System.Drawing.Bitmap gdiBitmap)
    {
      if (gdiBitmap is null)
        throw new ArgumentNullException(nameof(gdiBitmap));

      System.Windows.Media.Imaging.BitmapSource wpfBitmapSource = null;

      var hBitmap = gdiBitmap.GetHbitmap();

      try
      {
        wpfBitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
            hBitmap,
            IntPtr.Zero,
            Int32Rect.Empty,
            System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
      }
      catch (Win32Exception)
      {
        wpfBitmapSource = null;
      }
      finally
      {
        DeleteObject(hBitmap);
      }

      return wpfBitmapSource;
    }

    [System.Runtime.InteropServices.DllImport("gdi32.dll")]
    [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
    private static extern bool DeleteObject(IntPtr hObject);

    #endregion Image Conversion Wpf <==> Gdi
  }
}
