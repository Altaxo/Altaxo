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
using System.IO;
using Altaxo.Drawing;
using Altaxo.Geometry;

namespace Altaxo.Graph.Gdi
{
  /// <summary>
  /// Responsible for retrieving native Gdi brushes from <see cref="BrushX"/> objects.
  /// </summary>
  public class BrushCacheGdi
  {
    /// <summary>
    /// Class used to wrap a native Gdi <see cref="System.Drawing.Brush"/>, and to give it back to the cache if it is no longer in use.
    /// </summary>
    public class GdiBrush : IDisposable
    {
      /// <summary>
      /// Gets the <see cref="BrushX"/> together with its environment, that is used to create the native Gdi <see cref="System.Drawing.Brush"/>.
      /// </summary>
      public BrushXEnv Key { get; }

      /// <summary>
      /// Gets the native Gdi <see cref="System.Drawing.Brush"/>. This object must <b>not be modified</b>! If it is modified, the function <see cref="DoNotReturnToCache"/> must be called in order
      /// to avoid giving the modified object back to the cache!
      /// </summary>
      public Brush Brush { get; }

      private BrushCacheGdi _parentCache;

      /// <summary>
      /// Performs an implicit conversion from <see cref="GdiBrush"/> to <see cref="System.Drawing.Brush"/>.
      /// </summary>
      /// <param name="bh">The <see cref="GdiBrush"/> object to convert.</param>
      /// <returns>
      /// The <see cref="System.Drawing.Brush"/> object.
      /// </returns>
      public static implicit operator System.Drawing.Brush(GdiBrush bh)
      {
        return bh?.Brush;
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
      /// Initializes a new instance of the <see cref="GdiBrush"/> class.
      /// </summary>
      /// <param name="key">The <see cref="BrushX"/> object in its environment.</param>
      /// <param name="cache">The cache that manages this object.</param>
      public GdiBrush(BrushXEnv key, BrushCacheGdi cache)
      {
        Key = key;
        Brush = CreateGdiBrush(key.BrushX, key.BrushBoundingRectangle, key.EffectiveMaximumResolutionDpi);
        _parentCache = cache;
      }

      /// <summary>
      /// Creates a GDI <see cref="System.Drawing.Brush"/> from a <see cref="BrushX"/> object and its environment.
      /// </summary>
      /// <param name="t">The <see cref="BrushX"/>.</param>
      /// <param name="brushBoundingRectangle">The brush bounding rectangle.</param>
      /// <param name="effectiveMaximumResolutionDpi">The effective maximum resolution in dpi.</param>
      /// <returns></returns>
      public static Brush CreateGdiBrush(BrushX t, RectangleD2D brushBoundingRectangle, double effectiveMaximumResolutionDpi)
      {
        Brush br = null;
        switch (t.BrushType)
        {
          case BrushType.SolidBrush:
            br = new SolidBrush(ToGdi(t.Color));
            break;

          case BrushType.LinearGradientBrush:
          case BrushType.TriangularShapeLinearGradientBrush:
          case BrushType.SigmaBellShapeLinearGradientBrush:
            if (brushBoundingRectangle.IsEmpty)
              brushBoundingRectangle = new RectangleD2D(0, 0, 1000, 1000);
            LinearGradientBrush lgb;
            br = lgb = new LinearGradientBrush((RectangleF)brushBoundingRectangle, GetColor1(t), GetColor2(t), (float)-t.GradientAngle);
            if (t.WrapMode != WrapMode.Clamp)
              lgb.WrapMode = t.WrapMode;
            if (t.BrushType == Gdi.BrushType.TriangularShapeLinearGradientBrush)
              lgb.SetBlendTriangularShape((float)t.TextureOffsetX, (float)t.GradientColorScale);
            else if (t.BrushType == Gdi.BrushType.SigmaBellShapeLinearGradientBrush)
              lgb.SetSigmaBellShape((float)t.TextureOffsetX, (float)t.GradientColorScale);
            break;

          case BrushType.PathGradientBrush:
          case BrushType.TriangularShapePathGradientBrush:
          case Gdi.BrushType.SigmaBellShapePathGradientBrush:
            {
              var p = new GraphicsPath();
              if (brushBoundingRectangle.IsEmpty)
                brushBoundingRectangle = new RectangleD2D(0, 0, 1000, 1000);
              var outerRectangle = brushBoundingRectangle.OuterCircleBoundingBox;
              p.AddEllipse((RectangleF)outerRectangle);
              var pgb = new PathGradientBrush(p);
              if (t.ExchangeColors)
              {
                pgb.SurroundColors = new Color[] { ToGdi(t.BackColor) };
                pgb.CenterColor = ToGdi(t.Color);
              }
              else
              {
                pgb.SurroundColors = new Color[] { ToGdi(t.Color) };
                pgb.CenterColor = ToGdi(t.BackColor);
              }
              pgb.WrapMode = t.WrapMode;
              if (t.BrushType == Gdi.BrushType.TriangularShapePathGradientBrush)
                pgb.SetBlendTriangularShape(1, (float)t.GradientColorScale);
              if (t.BrushType == Gdi.BrushType.SigmaBellShapePathGradientBrush)
                pgb.SetSigmaBellShape(1, (float)t.GradientColorScale);
              pgb.CenterPoint = (PointF)(outerRectangle.Location + new PointD2D(outerRectangle.Width * t.TextureOffsetX, outerRectangle.Height * t.TextureOffsetY));
              br = pgb;
            }
            break;

          case BrushType.HatchBrush:
          case BrushType.SyntheticTextureBrush:
          case BrushType.TextureBrush:
            if (brushBoundingRectangle.IsEmpty)
              brushBoundingRectangle = new RectangleD2D(0, 0, 1000, 1000);

            Image img = null;
            VectorD2D finalSize = VectorD2D.Empty;
            VectorD2D sourceSize = VectorD2D.Empty;
            double blowFactor;

            if (t.TextureImage is IHatchBrushTexture)
            {
              sourceSize = (t.TextureImage as IHatchBrushTexture).Size;
              finalSize = t.TextureScale.GetResultingSize(sourceSize, (VectorD2D)brushBoundingRectangle.Size);
              blowFactor = Math.Max(Math.Abs(finalSize.X / sourceSize.X), Math.Abs(finalSize.Y / sourceSize.Y));
              var str = (t.TextureImage as IHatchBrushTexture).GetContentStream(effectiveMaximumResolutionDpi * blowFactor, t.ExchangeColors ? t.BackColor : t.Color, t.ExchangeColors ? t.Color : t.BackColor);
              img = SystemDrawingImageProxyExtensions.GetImage(str, disposeStream: true);
            }
            else if (t.TextureImage is ISyntheticRepeatableTexture)
            {
              sourceSize = (t.TextureImage as IHatchBrushTexture).Size;
              finalSize = t.TextureScale.GetResultingSize(sourceSize, (VectorD2D)brushBoundingRectangle.Size);
              blowFactor = Math.Max(Math.Abs(finalSize.X / sourceSize.X), Math.Abs(finalSize.Y / sourceSize.Y));
              var str = (t.TextureImage as ISyntheticRepeatableTexture).GetContentStream(effectiveMaximumResolutionDpi * blowFactor);
              img = SystemDrawingImageProxyExtensions.GetImage(str, disposeStream: true);
            }
            else if (t.TextureImage != null)
            {
              var str = t.TextureImage.GetContentStream();
              img = SystemDrawingImageProxyExtensions.GetImage(str, disposeStream: true);
              sourceSize = new VectorD2D(img.Width * 72.0 / img.HorizontalResolution, img.Height * 72.0 / img.VerticalResolution);
              finalSize = t.TextureScale.GetResultingSize(sourceSize, (VectorD2D)brushBoundingRectangle.Size);
            }

            if (img == null)
            {
              img = GetDefaultTextureBitmap();
              sourceSize = new VectorD2D(img.Width * 72.0 / img.HorizontalResolution, img.Height * 72.0 / img.VerticalResolution);
              finalSize = t.TextureScale.GetResultingSize(sourceSize, (VectorD2D)brushBoundingRectangle.Size);
            }

            // Bug in GDI+: a bitmap created from a stream
            // needs to be drawn once in order to be used by a TextureBrush
            // otherwise, creation of the TextureBrush will cause a OutOfMemoryException
            using (var g = Graphics.FromImage(img))
            {
              g.DrawLine(Pens.Transparent, 0, 0, 1, 0); // Hotfix to avoid OutOfMemoryException
            }


            var tb = new TextureBrush(img)
            {
              WrapMode = t.WrapMode
            };

            double xscale = finalSize.X / img.Width;
            double yscale = finalSize.Y / img.Height;

            if (0 != t.TextureOffsetX || 0 != t.TextureOffsetY)
              tb.TranslateTransform((float)(-finalSize.X * t.TextureOffsetX), (float)(-finalSize.Y * t.TextureOffsetY));

            if (0 != t.GradientAngle)
              tb.RotateTransform((float)(-t.GradientAngle));

            if (xscale != 1 || yscale != 1)
              tb.ScaleTransform((float)xscale, (float)yscale);

            br = tb;
            break;
        } // end of switch
        return br;
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

    private System.Collections.Concurrent.ConcurrentDictionary<BrushXEnv, GdiBrush> _dictionary = new System.Collections.Concurrent.ConcurrentDictionary<BrushXEnv, GdiBrush>();

    public static BrushCacheGdi Instance { get; } = new BrushCacheGdi();

    /// <summary>
    /// Borrows a brush for temporary use. Please embed the returned <see cref="GdiBrush"/> in a using statement, so that it is properly returned to the cache.
    /// </summary>
    /// <param name="b">The <see cref="BrushX"/> in its environment.</param>
    /// <returns>A <see cref="GdiBrush"/> object for temporary use.</returns>
    public GdiBrush BorrowBrush(BrushXEnv b)
    {
      if (_dictionary.TryRemove(b, out var gdiBrush))
      {
        return gdiBrush;
      }
      else
      {
        return new GdiBrush(b, this);
      }
    }

    /// <summary>
    /// Borrows a brush for temporary use. Please embed the returned <see cref="GdiBrush"/> in a using statement, so that it is properly returned to the cache.
    /// </summary>
    /// <param name="brush">The <see cref="BrushX"/>.</param>
    /// <param name="boundingRectangle">The bounding rectangle of the brush.</param>
    /// <param name="effectiveMaximumResolutionDpi">The maximum effective resolution of the graphics.</param>
    /// <returns>A <see cref="GdiBrush"/> object for temporary use.</returns>
    public GdiBrush BorrowBrush(BrushX brush, RectangleD2D boundingRectangle, double effectiveMaximumResolutionDpi)
    {
      return BorrowBrush(new BrushXEnv(brush, boundingRectangle, effectiveMaximumResolutionDpi));
    }

    /// <summary>
    /// Borrows a brush for temporary use. Please embed the returned <see cref="GdiBrush"/> in a using statement, so that it is properly returned to the cache.
    /// </summary>
    /// <param name="brush">The <see cref="BrushX"/>.</param>
    /// <param name="boundingRectangle">The bounding rectangle of the brush.</param>
    /// <param name="g">The Gdi graphics context (is used to determine the graphics resolution).</param>
    /// <param name="objectScale">The scale of the object (used for scaling the graphics resolution). If in doubt, use 1.</param>
    /// <returns>A <see cref="GdiBrush"/> object for temporary use.</returns>
    public GdiBrush BorrowBrush(BrushX brush, RectangleD2D boundingRectangle, Graphics g, double objectScale)
    {
      return BorrowBrush(new BrushXEnv(brush, boundingRectangle, GetEffectiveMaximumResolution(g, objectScale)));
    }



    /// <summary>
    /// Returns a <see cref="GdiBrush"/> object to the cache. The native <see cref="System.Drawing.Brush"/> that is hold by this object must not be modified!
    /// </summary>
    /// <param name="gdiBrush">The GDI brush to return to the cache.</param>
    private void ReturnObject(GdiBrush gdiBrush)
    {
      _dictionary.TryAdd(gdiBrush.Key, gdiBrush);
    }


    /// <summary>
    /// Gets the effective maximum resolution in dpi.
    /// </summary>
    /// <param name="g">The Gdi graphics context that determines the basic resolution.</param>
    /// <param name="objectScale">The object scale used to scale the basic resolution.</param>
    /// <returns>The effective resolution in dpi.</returns>
    public static double GetEffectiveMaximumResolution(Graphics g, double objectScale)
    {
      double maxDpi = Math.Max(g.DpiX, g.DpiY) * g.PageScale;
      var e = g.Transform.Elements;
      var scaleX = e[0] * e[0] + e[1] * e[1];
      var scaleY = (e[0] * e[3] - e[1] * e[2]) / Math.Sqrt(scaleX);
      maxDpi *= Math.Max(scaleX, scaleY);
      maxDpi *= objectScale;
      return maxDpi;
    }

    /// <summary>
    /// Gets the effective maximum resolution in dpi for a object scale of 1. See also <see cref="GetEffectiveMaximumResolution(Graphics, double)"/>.
    /// </summary>
    /// <param name="g">The Gdi graphics context that determines the basic resolution.</param>
    /// <returns>The effective resolution in dpi.</returns>
    public static double GetEffectiveMaximumResolution(Graphics g)
    {
      return GetEffectiveMaximumResolution(g, 1);
    }

  }


} // end of namespace
