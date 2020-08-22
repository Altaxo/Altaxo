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

#nullable enable
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using Altaxo.Drawing;
using Altaxo.Geometry;

namespace Altaxo.Graph.Gdi
{
  public partial class BrushCacheGdi
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

      /// <summary>
      /// The parent cache of this object. Can intendendly be set to null. In this case, the cached pen object is not returned to the cache.
      /// </summary>
      private BrushCacheGdi? _parentCache;

      /// <summary>
      /// Performs an implicit conversion from <see cref="GdiBrush"/> to <see cref="System.Drawing.Brush"/>.
      /// </summary>
      /// <param name="brush">The <see cref="GdiBrush"/> object to convert.</param>
      /// <returns>
      /// The <see cref="System.Drawing.Brush"/> object.
      /// </returns>
      public static implicit operator System.Drawing.Brush(GdiBrush brush)
      {
        if (brush is null)
          throw new ArgumentNullException(nameof(brush));

        return brush.Brush;
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
      /// <param name="brush">The <see cref="BrushX"/>.</param>
      /// <param name="brushBoundingRectangle">The brush bounding rectangle.</param>
      /// <param name="effectiveMaximumResolutionDpi">The effective maximum resolution in dpi.</param>
      /// <returns></returns>
      public static Brush CreateGdiBrush(BrushX brush, RectangleD2D brushBoundingRectangle, double effectiveMaximumResolutionDpi)
      {
        Brush? gdiBrush = null;

        switch (brush.BrushType)
        {
          case BrushType.SolidBrush:
            gdiBrush = new SolidBrush(ToGdi(brush.Color));
            break;

          case BrushType.LinearGradientBrush:
          case BrushType.TriangularShapeLinearGradientBrush:
          case BrushType.SigmaBellShapeLinearGradientBrush:
            if (brushBoundingRectangle.IsEmpty)
              brushBoundingRectangle = new RectangleD2D(0, 0, 1000, 1000);
            LinearGradientBrush lgb;
            gdiBrush = lgb = new LinearGradientBrush((RectangleF)brushBoundingRectangle, GetColor1(brush), GetColor2(brush), (float)-brush.GradientAngle);
            if (brush.WrapMode != WrapMode.Clamp)
              lgb.WrapMode = brush.WrapMode;
            if (brush.BrushType == BrushType.TriangularShapeLinearGradientBrush)
              lgb.SetBlendTriangularShape((float)brush.TextureOffsetX, (float)brush.GradientColorScale);
            else if (brush.BrushType == BrushType.SigmaBellShapeLinearGradientBrush)
              lgb.SetSigmaBellShape((float)brush.TextureOffsetX, (float)brush.GradientColorScale);
            break;

          case BrushType.PathGradientBrush:
          case BrushType.TriangularShapePathGradientBrush:
          case BrushType.SigmaBellShapePathGradientBrush:
            {
              var p = new GraphicsPath();
              if (brushBoundingRectangle.IsEmpty)
                brushBoundingRectangle = new RectangleD2D(0, 0, 1000, 1000);
              var outerRectangle = brushBoundingRectangle.OuterCircleBoundingBox;
              p.AddEllipse((RectangleF)outerRectangle);
              var pgb = new PathGradientBrush(p);
              if (brush.ExchangeColors)
              {
                pgb.SurroundColors = new Color[] { ToGdi(brush.BackColor) };
                pgb.CenterColor = ToGdi(brush.Color);
              }
              else
              {
                pgb.SurroundColors = new Color[] { ToGdi(brush.Color) };
                pgb.CenterColor = ToGdi(brush.BackColor);
              }
              pgb.WrapMode = brush.WrapMode;
              if (brush.BrushType == BrushType.TriangularShapePathGradientBrush)
                pgb.SetBlendTriangularShape(1, (float)brush.GradientColorScale);
              if (brush.BrushType == BrushType.SigmaBellShapePathGradientBrush)
                pgb.SetSigmaBellShape(1, (float)brush.GradientColorScale);
              pgb.CenterPoint = (PointF)(outerRectangle.Location + new PointD2D(outerRectangle.Width * brush.TextureOffsetX, outerRectangle.Height * brush.TextureOffsetY));
              gdiBrush = pgb;
            }
            break;

          case BrushType.HatchBrush:
          case BrushType.SyntheticTextureBrush:
          case BrushType.TextureBrush:
            if (brushBoundingRectangle.IsEmpty)
              brushBoundingRectangle = new RectangleD2D(0, 0, 1000, 1000);

            Image? img = null;
            VectorD2D finalSize = VectorD2D.Empty;
            VectorD2D sourceSize = VectorD2D.Empty;
            double blowFactor;

            if (brush.TextureImage is IHatchBrushTexture hatchBrushTexture)
            {
              sourceSize = hatchBrushTexture.Size;
              finalSize = brush.TextureScale.GetResultingSize(sourceSize, (VectorD2D)brushBoundingRectangle.Size);
              blowFactor = Math.Max(Math.Abs(finalSize.X / sourceSize.X), Math.Abs(finalSize.Y / sourceSize.Y));
              var str = hatchBrushTexture.GetContentStream(effectiveMaximumResolutionDpi * blowFactor, brush.ExchangeColors ? brush.BackColor : brush.Color, brush.ExchangeColors ? brush.Color : brush.BackColor);
              img = SystemDrawingImageProxyExtensions.GetImage(str, disposeStream: true);
            }
            else if (brush.TextureImage is ISyntheticRepeatableTexture syntheticRepeatableTexture)
            {
              sourceSize = syntheticRepeatableTexture.Size;
              finalSize = brush.TextureScale.GetResultingSize(sourceSize, (VectorD2D)brushBoundingRectangle.Size);
              blowFactor = Math.Max(Math.Abs(finalSize.X / sourceSize.X), Math.Abs(finalSize.Y / sourceSize.Y));
              var str = syntheticRepeatableTexture.GetContentStream(effectiveMaximumResolutionDpi * blowFactor);
              img = SystemDrawingImageProxyExtensions.GetImage(str, disposeStream: true);
            }
            else if (brush.TextureImage is { } otherTextureImage)
            {
              var str = otherTextureImage.GetContentStream();
              img = SystemDrawingImageProxyExtensions.GetImage(str, disposeStream: true);
              sourceSize = new VectorD2D(img.Width * 72.0 / img.HorizontalResolution, img.Height * 72.0 / img.VerticalResolution);
              finalSize = brush.TextureScale.GetResultingSize(sourceSize, (VectorD2D)brushBoundingRectangle.Size);
            }

            if (img is null)
            {
              img = GetDefaultTextureBitmap();
              sourceSize = new VectorD2D(img.Width * 72.0 / img.HorizontalResolution, img.Height * 72.0 / img.VerticalResolution);
              finalSize = brush.TextureScale.GetResultingSize(sourceSize, (VectorD2D)brushBoundingRectangle.Size);
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
              WrapMode = brush.WrapMode
            };

            double xscale = finalSize.X / img.Width;
            double yscale = finalSize.Y / img.Height;

            if (0 != brush.TextureOffsetX || 0 != brush.TextureOffsetY)
              tb.TranslateTransform((float)(-finalSize.X * brush.TextureOffsetX), (float)(-finalSize.Y * brush.TextureOffsetY));

            if (0 != brush.GradientAngle)
              tb.RotateTransform((float)(-brush.GradientAngle));

            if (xscale != 1 || yscale != 1)
              tb.ScaleTransform((float)xscale, (float)yscale);

            gdiBrush = tb;
            break;
          default:
            throw new InvalidProgramException($"Brush type not handled: {brush.BrushType}");
        } // end of switch
        return gdiBrush;
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
