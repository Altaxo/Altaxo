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
using System.IO;
using Altaxo.Drawing;
using Altaxo.Geometry;

namespace Altaxo.Graph.Gdi
{
  /// <summary>
  /// Responsible for retrieving native Gdi brushes from <see cref="BrushX"/> objects.
  /// </summary>
  public partial class BrushCacheGdi
  {
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
