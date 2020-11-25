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
using System.Linq;
using Altaxo.Drawing;
using Altaxo.Geometry;

namespace Altaxo.Graph.Gdi
{
  /// <summary>
  /// Responsible for retrieving native Gdi brushes from <see cref="BrushX"/> objects.
  /// </summary>
  public partial class PenCacheGdi
  {

    private System.Collections.Concurrent.ConcurrentDictionary<PenXEnv, GdiPen> _dictionary = new System.Collections.Concurrent.ConcurrentDictionary<PenXEnv, GdiPen>();

    public static PenCacheGdi Instance { get; } = new PenCacheGdi();

    /// <summary>
    /// Borrows a brush for temporary use. Please embed the returned <see cref="GdiPen"/> in a using statement, so that it is properly returned to the cache.
    /// </summary>
    /// <param name="b">The <see cref="BrushX"/> in its environment.</param>
    /// <returns>A <see cref="GdiPen"/> object for temporary use.</returns>
    public GdiPen BorrowPen(PenXEnv b)
    {
      if (_dictionary.TryRemove(b, out var gdiBrush))
      {
        return gdiBrush;
      }
      else
      {
        return new GdiPen(b, this);
      }
    }

    /// <summary>
    /// Borrows a brush for temporary use. Please embed the returned <see cref="GdiPen"/> in a using statement, so that it is properly returned to the cache.
    /// </summary>
    /// <param name="pen">The <see cref="PenX"/> that is used as template for the <see cref="GdiPen"/> to return.</param>
    /// <param name="boundingRectangle">The bounding rectangle of the brush.</param>
    /// <param name="effectiveMaximumResolutionDpi">The maximum effective resolution of the graphics.</param>
    /// <returns>A <see cref="GdiPen"/> object for temporary use.</returns>
    public GdiPen BorrowPen(PenX pen, RectangleD2D boundingRectangle, double effectiveMaximumResolutionDpi)
    {
      return BorrowPen(new PenXEnv(pen, boundingRectangle, effectiveMaximumResolutionDpi));
    }

    /// <summary>
    /// Borrows a brush for temporary use. Please embed the returned <see cref="GdiPen"/> in a using statement, so that it is properly returned to the cache.
    /// </summary>
    /// <param name="pen">The <see cref="PenX"/>.</param>
    /// <param name="boundingRectangle">The bounding rectangle of the brush.</param>
    /// <param name="g">The Gdi graphics context (is used to determine the graphics resolution).</param>
    /// <param name="objectScale">The scale of the object (used for scaling the graphics resolution). If in doubt, use 1.</param>
    /// <returns>A <see cref="GdiPen"/> object for temporary use.</returns>
    public GdiPen BorrowPen(PenX pen, RectangleD2D boundingRectangle, Graphics g, double objectScale)
    {
      return BorrowPen(new PenXEnv(pen, boundingRectangle, GetEffectiveMaximumResolution(g, objectScale)));
    }

    public GdiPen BorrowPen(PenX pen)
    {
      return BorrowPen(new PenXEnv(pen, RectangleD2D.Empty, 96));
    }


    /// <summary>
    /// Returns a <see cref="GdiPen"/> object to the cache. The native <see cref="System.Drawing.Brush"/> that is hold by this object must not be modified!
    /// </summary>
    /// <param name="gdiBrush">The GDI brush to return to the cache.</param>
    private void ReturnObject(GdiPen gdiBrush)
    {
      _dictionary.TryAdd(gdiBrush.Key, gdiBrush);
    }


    /// <summary>
    /// Gets all pens currently in this dictionary.
    /// </summary>
    /// <returns></returns>
    public PenXEnv[] GetKeys()
    {
      return _dictionary.Keys.ToArray();
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
