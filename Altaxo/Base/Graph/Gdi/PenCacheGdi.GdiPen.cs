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
using Altaxo.Graph.Gdi.LineCaps;

#nullable enable

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

      /// <summary>
      /// The parent cache of this object. Can intendendly be set to null. In this case, the cached pen object is not returned to the cache.
      /// </summary>
      private PenCacheGdi? _parentCache;

      /// <summary>
      /// Performs an implicit conversion from <see cref="GdiPen"/> to <see cref="System.Drawing.Brush"/>.
      /// </summary>
      /// <param name="pen">The <see cref="GdiPen"/> object to convert.</param>
      /// <returns>
      /// The <see cref="System.Drawing.Brush"/> object.
      /// </returns>
      public static implicit operator System.Drawing.Pen(GdiPen pen)
      {
        if (pen is null)
          throw new ArgumentNullException(nameof(pen));

        return pen.Pen;
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
      /// Creates a Gdi+ <see cref="System.Drawing.Pen"/> from a independent <see cref="PenX"/>, a bounding rectangle, and the effective resolution of the drawing.
      /// </summary>
      /// <param name="p">The system independend pen object..</param>
      /// <param name="boundingRectangle">The bounding rectangle.</param>
      /// <param name="maximumEffectiveResolutionDpi">The maximum effective resolution of the drawing in dpi.</param>
      /// <returns>The corresponding <see cref="System.Drawing.Pen"/> object. This value must be disposed if no longer in use.</returns>
      public static Pen CreateGdiPen(PenX p, RectangleD2D boundingRectangle, double maximumEffectiveResolutionDpi)
      {
        var gdiPen = new Pen(Color.Black);

        // now set the optional Pen properties
        if (gdiPen.Width != p.Width)
          gdiPen.Width = (float)(p.Width);

        if (p.Alignment != PenAlignment.Center)
          gdiPen.Alignment = p.Alignment;

        if (!(p.Brush.IsSolidBrush))
          gdiPen.Brush = BrushCacheGdi.GdiBrush.CreateGdiBrush(p.Brush, boundingRectangle, maximumEffectiveResolutionDpi);
        else if (p.Color != NamedColors.Black)
          gdiPen.Color = ToGdi(p.Color);

        if (p.CompoundArray is { } compoundArray && compoundArray.Length > 0)
          gdiPen.CompoundArray = compoundArray.Select(x => (float)x).ToArray();

        if (!(p.DashPattern is Drawing.DashPatterns.Solid))
        {
          var (gdiDashStyle, gdiDashPattern, gdiDashOffset) = GetGdiDashProperties(p);

          if (gdiDashStyle != DashStyle.Solid)
            gdiPen.DashStyle = gdiDashStyle;

          if (!(p.DashCap == DashCap.Flat))
            gdiPen.DashCap = p.DashCap;

          if (0 != gdiDashOffset)
            gdiPen.DashOffset = gdiDashOffset;

          if (null != gdiDashPattern)
            gdiPen.DashPattern = gdiDashPattern;
        }

        if (!(p.EndCap is null) && !(p.EndCap is LineCaps.FlatCap))
        {
          GdiLineCapBase.SetEndCap(gdiPen, p.EndCap);
        }

        if (p.LineJoin != LineJoin.Miter)
          gdiPen.LineJoin = p.LineJoin;

        if (p.MiterLimit != 10)
          gdiPen.MiterLimit = (float)p.MiterLimit;

        if (!(p.StartCap is null) && !(p.StartCap is LineCaps.FlatCap))
        {
          GdiLineCapBase.SetStartCap(gdiPen, p.StartCap);
        }

        if (!(p.Transformation is null || p.Transformation.Matrix.IsIdentity))
        {
          var x = p.Transformation.Matrix;
          gdiPen.Transform = new Matrix((float)x.M11, (float)x.M12, (float)x.M21, (float)x.M22, (float)x.M31, (float)x.M32);
        }

        return gdiPen;
      }

      private static (DashStyle GdiDashStyle, float[]? GdiDashPattern, float GdiDashOffset) GetGdiDashProperties(PenX p)
      {
        var value = p.DashPattern;

        return value switch
        {
          Drawing.DashPatterns.Solid _ => (DashStyle.Solid, null, 0),
          Drawing.DashPatterns.Dash _ => (DashStyle.Dash, null, 0),
          Drawing.DashPatterns.Dot _ => (DashStyle.Dot, null, 0),
          Drawing.DashPatterns.DashDot _ => (DashStyle.DashDot, null, 0),
          Drawing.DashPatterns.DashDotDot _ => (DashStyle.DashDotDot, null, 0),
          _ => (DashStyle.Custom, value.Select(x => (float)x).ToArray(), (float)value.DashOffset),
        };
      }


      private static System.Drawing.Color ToGdi(NamedColor color)
      {
        var c = color.Color;
        return System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B);
      }
    }
  }


} // end of namespace
