#region Copyright

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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;
using Altaxo.Collections;
using Altaxo.Geometry;

namespace Altaxo.Gui
{
  public static partial class GuiHelper
  {
    #region Brush and Pen

    public static System.Windows.Media.Color ToWpf(this System.Drawing.Color c)
    {
      return Color.FromArgb(c.A, c.R, c.G, c.B);
    }

    public static System.Drawing.Color ToSysDraw(this System.Windows.Media.Color c)
    {
      return System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B);
    }

    #endregion Brush and Pen

    #region Point

    public static Point ToWpf(this System.Drawing.PointF pt)
    {
      return new Point(pt.X, pt.Y);
    }

    public static Point ToWpf(this System.Drawing.Point pt)
    {
      return new Point(pt.X, pt.Y);
    }

    public static System.Drawing.PointF ToSysDraw(this Point pt)
    {
      return new System.Drawing.PointF((float)pt.X, (float)pt.Y);
    }

    public static System.Drawing.Point ToSysDrawInt(this Point pt)
    {
      return new System.Drawing.Point((int)pt.X, (int)pt.Y);
    }

    #endregion Point

    #region Rectangle

    public static Rect ToWpf(this System.Drawing.RectangleF rect)
    {
      return new Rect(rect.X, rect.Y, rect.Width, rect.Height);
    }

    public static Rect ToWpf(this System.Drawing.Rectangle rect)
    {
      return new Rect(rect.X, rect.Y, rect.Width, rect.Height);
    }

    public static System.Drawing.RectangleF ToSysDraw(this Rect rect)
    {
      return new System.Drawing.RectangleF((float)rect.X, (float)rect.Y, (float)rect.Width, (float)rect.Height);
    }

    public static System.Drawing.Rectangle ToSysDrawInt(this Rect rect)
    {
      return new System.Drawing.Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
    }

    #endregion Rectangle

    #region Image from System.Drawing to WPF

    public static System.Windows.Media.Imaging.BitmapSource ToWpf(this System.Drawing.Bitmap bitmap)
    {
      using (var stream = new System.IO.MemoryStream())
      {
        var imgFormat = bitmap.PixelFormat == System.Drawing.Imaging.PixelFormat.Format24bppRgb ? System.Drawing.Imaging.ImageFormat.Bmp : System.Drawing.Imaging.ImageFormat.Png;
        bitmap.Save(stream, imgFormat);

        stream.Position = 0;
        var result = new System.Windows.Media.Imaging.BitmapImage();
        result.BeginInit();
        // According to MSDN, "The default OnDemand cache option retains access to the stream until the image is needed."
        // Force the bitmap to load right now so we can dispose the stream.
        result.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
        result.StreamSource = stream;
        result.EndInit();
        result.Freeze();
        return result;
      }
    }

    #endregion Image from System.Drawing to WPF
  }
}
