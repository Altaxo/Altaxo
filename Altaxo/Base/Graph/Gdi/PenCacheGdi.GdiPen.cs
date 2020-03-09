#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2020 Dr. Dirk Lellinger
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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using Altaxo.Drawing;
using Altaxo.Geometry;

namespace Altaxo.Graph.Gdi
{
  public partial class PenCacheGdi
  {
    /// <summary>
    /// Class used to wrap a native Gdi <see cref="System.Drawing.Brush"/>, and to give it back to the cache if it is no longer in use.
    /// </summary>
    public class GdiPen : IDisposable
    {
      /// <summary>
      /// Gets the <see cref="BrushX"/> together with its environment, that is used to create the native Gdi <see cref="System.Drawing.Brush"/>.
      /// </summary>
      public PenXEnv Key { get; }

      /// <summary>
      /// Gets the native Gdi <see cref="System.Drawing.Brush"/>. This object must <b>not be modified</b>! If it is modified, the function <see cref="DoNotReturnToCache"/> must be called in order
      /// to avoid giving the modified object back to the cache!
      /// </summary>
      public Pen Pen { get; }

      private PenCacheGdi _parentCache;

      /// <summary>
      /// Performs an implicit conversion from <see cref="GdiPen"/> to <see cref="System.Drawing.Brush"/>.
      /// </summary>
      /// <param name="pen">The <see cref="GdiPen"/> object to convert.</param>
      /// <returns>
      /// The <see cref="System.Drawing.Brush"/> object.
      /// </returns>
      public static implicit operator System.Drawing.Pen(GdiPen pen)
      {
        return pen?.Pen;
      }


      /// <summary>
      /// By disposing this object, it is returned to the cache. You should therefore no longer used it!
      /// </summary>
      public void Dispose()
      {
        _parentCache?.ReturnObject(this);
      }

      /// <summary>
      /// Call this function if you need to modify the native <see cref="Brush"/> object. By calling this function,
      /// this object is not returned to the cache.
      /// </summary>
      public void DoNotReturnToCache()
      {
        _parentCache = null;
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="GdiPen"/> class.
      /// </summary>
      /// <param name="key">The <see cref="PenX"/> object in its environment.</param>
      /// <param name="cache">The cache that manages this object.</param>
      public GdiPen(PenXEnv key, PenCacheGdi cache)
      {
        Key = key;
        Pen = CreateGdiPen(key.PenX, key.BrushBoundingRectangle, key.EffectiveMaximumResolutionDpi);
        _parentCache = cache;
      }

      /// <summary>
      /// Sets the Gdi pen dash style to reflect the <see cref="DashStyleEx"/> object.
      /// </summary>
      /// <param name="pen">The Gdi pen to set the dash style for.</param>
      /// <param name="dashStyleEx">The <see cref="DashStyleEx"/> object used to set the dash style of the Gdi pen.</param>
      public static void SetPenDash(Pen pen, DashStyleEx dashStyleEx)
      {
        pen.DashStyle = dashStyleEx.KnownStyle;
        if (dashStyleEx.IsCustomStyle)
          pen.DashPattern = dashStyleEx.CustomStyle;
      }

      private static void SetCachedDashProperties(PenX p, out DashStyle dashStyle, out float[] dashPattern, out float dashOffset)
      {
        var value = p.DashPattern;
        dashStyle = DashStyle.Solid;
        dashPattern = null;
        dashOffset = 0;

        if (value is Drawing.DashPatterns.Solid)
        {
          dashStyle = DashStyle.Solid;
        }
        else if (value is Drawing.DashPatterns.Dash)
        {
          dashStyle = DashStyle.Dash;
        }
        else if (value is Drawing.DashPatterns.Dot)
        {
          dashStyle = DashStyle.Dot;
        }
        else if (value is Drawing.DashPatterns.DashDot)
        {
          dashStyle = DashStyle.DashDot;
        }
        else if (value is Drawing.DashPatterns.DashDotDot)
        {
          dashStyle = DashStyle.DashDotDot;
        }
        else
        {
          dashStyle = DashStyle.Custom;
          dashPattern = value.Select(x => (float)x).ToArray();
          dashOffset = (float)value.DashOffset;
        }
      }


      public static Pen CreateGdiPen(PenX p, RectangleD2D boundingRectangle, double maximumEffectiveResolutionDpi)
      {
        var pen = new Pen(Color.Black);

        // now set the optional Pen properties
        if (pen.Width != p.Width)
          pen.Width = (float)(p.Width);

        if (p.Alignment != PenAlignment.Center)
          pen.Alignment = p.Alignment;

        if (p.Brush is null && p.Color != NamedColors.Black)
          pen.Color = ToGdi(p.Color);

        if (p.Brush is { } brush)
        {
          var brushGdi = BrushCacheGdi.Instance.BorrowBrush(brush, boundingRectangle, maximumEffectiveResolutionDpi);
          pen.Brush = brushGdi;
          brushGdi.DoNotReturnToCache();
        }

        if (p.CompoundArray is { } compoundArray && compoundArray.Length > 0)
          pen.CompoundArray = compoundArray;

        if (!(p.DashPattern is Drawing.DashPatterns.Solid))
        {
          SetCachedDashProperties(p, out var cachedDashStyle, out var cachedDashPattern, out var cachedDashOffset);

          if (cachedDashStyle != DashStyle.Solid)
            pen.DashStyle = cachedDashStyle;

          if (!(p.DashCap == DashCap.Flat))
            pen.DashCap = p.DashCap;

          if (0 != cachedDashOffset)
            pen.DashOffset = cachedDashOffset;

          if (null != cachedDashPattern)
            pen.DashPattern = cachedDashPattern;
        }

        if (!(p.EndCap is LineCaps.FlatCap))
          p.EndCap.SetEndCap(pen);

        if (p.LineJoin != LineJoin.Miter)
          pen.LineJoin = p.LineJoin;

        if (p.MiterLimit != 10)
          pen.MiterLimit = (float)p.MiterLimit;

        if (!(p.StartCap is LineCaps.FlatCap))
          p.StartCap.SetStartCap(pen);

        if (!(p.Transform is null || p.Transform.IsIdentity))
          pen.Transform = p.Transform;

        return pen;
      }

      private static System.Drawing.Color ToGdi(NamedColor color)
      {
        var c = color.Color;
        return System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B);
      }

      private static Color GetColor1(BrushX x)
      {
        return x.ExchangeColors ? ToGdi(x.BackColor) : ToGdi(x.Color);
      }

      private static Color GetColor2(BrushX x)
      {
        return x.ExchangeColors ? ToGdi(x.Color) : ToGdi(x.BackColor);
      }

      private static Bitmap GetDefaultTextureBitmap()
      {
        var result = new Bitmap(3, 3, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        result.SetPixel(1, 1, System.Drawing.Color.Black);
        return result;
      }
    }

  }


} // end of namespace
